using Irony.Parsing;

public class SQLGrammar : Grammar
{
    public SQLGrammar() : base(caseSensitive: false)
    {
        // Terminal Terms
        var create = ToTerm("CREATE");
        var set = ToTerm("SET");
        var drop = ToTerm("DROP");
        var table = ToTerm("TABLE");
        var select = ToTerm("SELECT");
        var from = ToTerm("FROM");
        var database = ToTerm("DATABASE");
        var identifier = new IdentifierTerminal("identifier");

        // Non-Terminal Terms

        var varchar = new NonTerminal("varchar", ToTerm("VARCHAR") + "(" + new NumberLiteral("size") + ")");

        var columnType = new NonTerminal("columnType", ToTerm("INTEGER")|"DOUBLE"| varchar |ToTerm("DATETIME"));
        var columnDefinition = new NonTerminal("columnDefinition");
        var columnDefining = new NonTerminal("columnDefining");
        var createTableStructure = new NonTerminal("createTableStructure");
        var selectFromStructure = new NonTerminal("selectFromStructure");
        var createDBStructure = new NonTerminal("createDBStructure");
        var setDBStructure = new NonTerminal("setDBStructure");
        var dropTableStructure = new NonTerminal("dropTableStructure");

        // Grammar Rules

        columnDefinition.Rule = identifier + columnType;
        columnDefining.Rule = MakePlusRule(columnDefining, ToTerm(","), columnDefinition);
        createTableStructure.Rule = create + table + identifier + "(" + columnDefining + ")"
                                  | create + table + identifier;
        selectFromStructure.Rule = select + "*" + from + identifier;
        createDBStructure.Rule = create + database + identifier;
        setDBStructure.Rule = set + database + identifier;
        dropTableStructure.Rule = drop + table + identifier;

        this.Root = new NonTerminal("statement");
        this.Root.Rule = createTableStructure | selectFromStructure
                        | createDBStructure   | setDBStructure
                        | dropTableStructure;

        MarkPunctuation("(", ")", ",");
        RegisterBracePair("(",")");

    }
} 