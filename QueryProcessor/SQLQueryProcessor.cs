using Entities;
using QueryProcessor.Exceptions;
using QueryProcessor.Operations;
using Irony.Parsing;
using System.Data;

namespace QueryProcessor
{
    public class SQLQueryProcessor
    {
        public static OperationStatus Execute(string sentence)
        {
            SQLGrammar grammar = new SQLGrammar();
            LanguageData language = new LanguageData(grammar);
            Parser parser = new Parser(language);

            ParseTree tree = parser.Parse(sentence);
            ParseTreeNode root = tree.Root;
            Console.WriteLine("Parse tree: " + tree.ToString()); // Log the parse tree


            if (tree.HasErrors())
            {
                Console.WriteLine("Tree was null");
                throw new SyntaxErrorException(tree.ParserMessages[0].Message);
            }
            //if (root.Term.Name == "createTableStructure")
            switch (root.Term.Name)
            {
            case "createTableStructure":
                var tableName = root.ChildNodes[2].Token.Text;
                var columnNodes = root.ChildNodes[4];
                var columns = new List<(string columnName, string columnType)>();
                
                foreach (var columnNode in columnNodes.ChildNodes)
                {
                    var columnName = columnNode.ChildNodes[0].Token.Text;
                    var columnType = columnNode.ChildNodes[1].Token.Text;
                    columns.Add((columnName, columnType));
                }
                foreach (var (name, type) in columns)
                {
                    Console.WriteLine($"Column: {name}, Type: {type}");
                }
                
                Console.WriteLine("Workinggg");

                return new CreateTable().Execute();
            // }
            // else
            // {
            default:
                throw new UnknownSQLSentenceException();
            }
            
        }
    }
}
