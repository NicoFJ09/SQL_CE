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
                    return new CreateDataBase().Execute();
                case "setDBStructure":
                    Console.WriteLine("Set DataBase - Query");
                    return new SetDataBase().Execute();
                case "createTableStructure":
                    Console.WriteLine("Creating Table - Query");
                    return HandleCreateTable(commandNode);
                case "dropTableStructure":
                    Console.WriteLine("Drop Table - Query");
                    return new DropTable().Execute();
                case "selectFromStructure":
                    Console.WriteLine("Select From - Query");
                    return HandleSelect(commandNode);

                default:
                    throw new UnknownSQLSentenceException();
            }
            
        }
        private static OperationStatus HandleCreateTable(ParseTreeNode root)
        {
            // Extract table name (should be at index 2)
            var tableName = root.ChildNodes[2].Token.Text;  // This accesses the table name
            Console.WriteLine($"Parsed table name: {tableName}");  // Debugging log
            var columnNodes = root.ChildNodes[3];  // This accesses the column definitions (columnDefining)
            Console.WriteLine($"Column nodes count: {columnNodes.ChildNodes.Count}");  // Debugging log
            var columns = new List<(string columnName, string columnType)>();

            // Loop through all column definitions
            foreach (var columnNode in columnNodes.ChildNodes)
            {
                Console.WriteLine($"Processing column node: {columnNode}");
                // Each columnNode is a columnDefinition
                if (columnNode.ChildNodes.Count < 2)
                {
                    throw new SyntaxErrorException("Invalid column definition.");
                }

                var columnName = columnNode.ChildNodes[0].Token.Text;  // Column name (identifier)
                var columnTypeNode = columnNode.ChildNodes[1];  // Column type (columnType)

                // Determine the column type
                
                var actualColumnTypeNode = columnTypeNode.ChildNodes[0];

                Console.WriteLine($"Column type term: {actualColumnTypeNode.Term.Name}");  // Debug output
                string columnType = actualColumnTypeNode.Term.Name 
                switch
                {
                    
                    "INTEGER" => "INTEGER",
                    "DOUBLE" => "DOUBLE",
                    "DATETIME" => "DATETIME",
                    "varchar" => $"VARCHAR({columnTypeNode.ChildNodes[1].Token.Text})",  // Extract size for VARCHAR
                    _ => throw new InvalidOperationException("Unknown column type")
                };

                columns.Add((columnName, columnType));
            }

            // Log parsed information
            Console.WriteLine($"Creating table: {tableName} with columns:");
            foreach (var (name, type) in columns)
            {
                Console.WriteLine($"  - Column: {name}, Type: {type}");
            }

            // Call the actual CreateTable method (implement this as needed)
            return new CreateTable().Execute();
        }

        private static OperationStatus HandleSelect(ParseTreeNode root)
        {
            // Implement the SELECT logic here
            return new Select().Execute();
        }
    }
}
