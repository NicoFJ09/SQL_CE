using Irony.Parsing;

public class SQLGrammar : Grammar
{
    public SQLGrammar() : base(caseSensitive: false)
    {
        // Terminal Terms
        var create = ToTerm("CREATE");
        var table = ToTerm("TABLE");
        var identifier = new IdentifierTerminal("identifier");

        // Non-Terminal Terms

        var columnType = new NonTerminal("columnType", ToTerm("INTEGER")|"DOUBLE"|"VARCHAR"|"DATETIME");
        var columnDefinition = new NonTerminal("columnDefinition");
        var columnDefining = new NonTerminal("columnDefining");
        var createTableStructure = new NonTerminal("createTableStructure");

        // Grammar Rules

        columnDefinition.Rule = identifier + columnType;
        columnDefining.Rule = MakePlusRule(columnDefining, ToTerm(","), columnDefinition);
        createTableStructure.Rule = create + table + identifier + "(" + columnDefining + ")";

        this.Root = new NonTerminal("statement");
        this.Root.Rule = createTableStructure;

        MarkPunctuation("(", ")", ",");
        RegisterBracePair("(",")");

    }
} 