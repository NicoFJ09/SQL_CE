using Entities;
using QueryProcessor.Exceptions;
using QueryProcessor.Operations;
using Irony.Parsing;
using System.Data;
using Microsoft.VisualBasic;
using System.Linq;

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
                    return HandleDropTable(commandNode);
                case "selectFromStructure":
                    Console.WriteLine("Select From - Query");
                    return HandleSelect(commandNode);
                case "createIndexStructure":
                    Console.WriteLine("Creating Index - Query");
                    return new CreateIndex().Execute();
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
            Console.WriteLine($"Parsed db name: {DBName}");  

            return new CreateDataBase().Execute(DBName);
        }

private static OperationStatus HandleSetDB(ParseTreeNode root)
        {
            // Extract table name (should be at index 2)
            var DBName = root.ChildNodes[2].Token.Text;  
            Console.WriteLine($"Parsed db name: {DBName}");  

            return new SetDataBase().Execute(DBName);
        }

        private static OperationStatus HandleCreateTable(ParseTreeNode root)
        {
            // Extract table name (should be at index 2)
            var tableName = root.ChildNodes[2].Token.Text;  
            Console.WriteLine($"Parsed table name: {tableName}");  
            var columnNodes = root.ChildNodes[3]; 
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
            List<string> columnas = new List<string>();
            foreach (var (name, type) in columns)
            {
                Console.WriteLine($"  - Column: {name}, Type: {type}");
                string nombre = name.ToString();
                string tipo = type.ToString();
                columnas.Add(nombre);
                columnas.Add(tipo);
            }
            string[] totalColumnas = columnas.ToArray();

            return new CreateTable().Execute(tableName, totalColumnas);
        }

        private static OperationStatus HandleDropTable(ParseTreeNode root)
        {
            var tableName = root.ChildNodes[2].Token.Text;  
            Console.WriteLine($"Parsed table name: {tableName}");  
            return new DropTable().Execute(tableName);
        }


        private static OperationStatus HandleSelect(ParseTreeNode root)
        {
            // Implement the SELECT logic here
            return new Select().Execute();
        }
    }
}
