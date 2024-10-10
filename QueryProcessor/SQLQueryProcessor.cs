using Entities;
using QueryProcessor.Exceptions;
using QueryProcessor.Operations;
using Irony.Parsing;
using System.Data;

namespace QueryProcessor
{
    public class SQLQueryProcessor
    {
        private static void PrintParseTree(ParseTreeNode node, int level)
        {
            Console.WriteLine(new string(' ', level * 2) + node.Term.Name + (node.Term.Name == "identifier" ? ": " + node.Token.Text : ""));
            foreach (var child in node.ChildNodes)
            {
                PrintParseTree(child, level + 1);
            }
        }

        public static OperationStatus Execute(string sentence)
        {
            SQLGrammar grammar = new SQLGrammar();
            LanguageData language = new LanguageData(grammar);
            Parser parser = new Parser(language);

            ParseTree tree = parser.Parse(sentence);
            ParseTreeNode root = tree.Root;



            if (tree.HasErrors())
            {
                Console.WriteLine($"Syntax error: {tree.ParserMessages[0].Message}");
                throw new SyntaxErrorException(tree.ParserMessages[0].Message);
            }
            PrintParseTree(root, 0);

            var commandNode = root.ChildNodes[0];
            
            switch (commandNode.Term.Name)
            {
                case "createDBStructure":
                    Console.WriteLine("Creating DataBase - Query");
                    return HandleCreateDB(commandNode);
                case "setDBStructure":
                    Console.WriteLine("Set DataBase - Query");
                    return HandleSetDB(commandNode);
                case "createTableStructure":
                    Console.WriteLine("Creating Table - Query");
                    return HandleCreateTable(commandNode);
                case "dropTableStructure":
                    Console.WriteLine("Drop Table - Query");
                    return new DropTable().Execute();
                case "selectFromStructure":
                    Console.WriteLine("Select From - Query");
                    return HandleSelect(commandNode);
                case "createIndexStructure":
                    Console.WriteLine("Creating Index - Query");
                    return HandleCreateIndex(commandNode);
                case "updateStructure":
                    Console.WriteLine("Updating - Query");
                    return new Update().Execute();
                case "deleteFromStructure":
                    Console.WriteLine("Deleting From - Query");
                    return new Delete().Execute();
                case "insertIntoStructure":
                    Console.WriteLine("Insert Into - Query");
                    return new InsertInto().Execute();

                default:
                    throw new UnknownSQLSentenceException();
            }
            
        }

    private static OperationStatus HandleCreateDB(ParseTreeNode root)
            {
                // Extract table name (should be at index 2)
                var DBName = root.ChildNodes[2].Token.Text;  
                Console.WriteLine($"Parsed table name: {DBName}");  

                return new CreateDataBase().Execute(DBName);
            }

    private static OperationStatus HandleSetDB(ParseTreeNode root)
            {
                // Extract table name (should be at index 2)
                var DBName = root.ChildNodes[2].Token.Text;  
                Console.WriteLine($"Parsed table name: {DBName}");  

                return new SetDataBase().Execute(DBName);
            }

    private static OperationStatus HandleCreateTable(ParseTreeNode root)
    {
        // Extract table name (should be at index 2)
        var tableName = root.ChildNodes[2].Token.Text;  
        Console.WriteLine($"Parsed table name: {tableName}");  
        var treeType = root.ChildNodes[3].Token.Text;
        Console.WriteLine($"Parsed tree type: {treeType}");

        var columnNodes = root.ChildNodes[4]; 
        Console.WriteLine($"Column nodes count: {columnNodes.ChildNodes.Count}"); 
        var columns = new List<(string columnName, string columnType)>();

        foreach (var columnNode in columnNodes.ChildNodes)
        {
            Console.WriteLine($"Processing column node: {columnNode}");
            // Each columnNode is a columnDefinition
            if (columnNode.ChildNodes.Count < 2)
            {
                throw new SyntaxErrorException("Invalid column definition.");
            }

            var columnName = columnNode.ChildNodes[0].Token.Text;  
            var columnTypeNode = columnNode.ChildNodes[1];  

            // Determine the column type
            
            var actualColumnTypeNode = columnTypeNode.ChildNodes[0];

            Console.WriteLine($"Column type term: {actualColumnTypeNode.Term.Name}");  // Debug output
            string columnType = actualColumnTypeNode.Term.Name 
            switch
            {
                
                "INTEGER" => "INTEGER",
                "DOUBLE" => "DOUBLE",
                "DATETIME" => "DATETIME",
                //"varchar" => $"VARCHAR({columnTypeNode.ChildNodes[1].Token.Text})",  // Extract size for VARCHAR
                _ => throw new InvalidOperationException("Unknown column type")
            };

            columns.Add((columnName, columnType));
        }

        Console.WriteLine($"Creating table: {tableName} with columns:");
        foreach (var (name, type) in columns)
        {
            Console.WriteLine($"  - Column: {name}, Type: {type}");
        }

        var columnsStringList = columns.Select(c => $"{c.columnName} {c.columnType}").ToList();
        Console.WriteLine("Columns as strings:");
        foreach (var columnString in columnsStringList)
        {
            Console.WriteLine(columnString);
        }
        return new CreateTable().Execute(tableName, columnsStringList, treeType);
    }
    private static OperationStatus HandleCreateIndex(ParseTreeNode root)
    {
        // Extract table name (should be at index 4)
        var tableNameNode = root.ChildNodes[4];
        if (tableNameNode == null || tableNameNode.Token == null)
        {
            throw new ArgumentNullException(nameof(tableNameNode), "Table name node or its token is null.");
        }
        var tableName = tableNameNode.Token.Text;
        Console.WriteLine($"Parsed table name: {tableName}");

        // Extract column name (should be at index 6)
        var columnNameNode = root.ChildNodes[5];
        if (columnNameNode == null || columnNameNode.Token == null)
        {
            throw new ArgumentNullException(nameof(columnNameNode), "Column name node or its token is null.");
        }
        var columnName = columnNameNode.Token.Text;
        Console.WriteLine($"Parsed column name: {columnName}");

        // Llamada al método Execute con los nombres extraídos
        return new CreateIndex().Execute(tableName, columnName);
    }

        private static OperationStatus HandleSelect(ParseTreeNode root)
        {
            // Implement the SELECT logic here
            return new Select().Execute();
        }
    }
}
