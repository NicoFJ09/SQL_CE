using System.Data.Common;
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
        var update = ToTerm("UPDATE");
        var delete = ToTerm("DELETE");
        var insert = ToTerm("INSERT");
        var into = ToTerm("INTO");
        var values = ToTerm("VALUES");
        var database = ToTerm("DATABASE");
        var index = ToTerm("INDEX");
        var on = ToTerm("ON");
        var of = ToTerm("OF");
        var type = ToTerm("TYPE");
        var where =  ToTerm("WHERE");
        var comparator = ToTerm(">")|"<"|"="|"LIKE"|"NOT";
        var orderBy = ToTerm("ORDER BY");
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
        var createIndexStructure = new NonTerminal("createIndexStructure");
        var updateStructure = new NonTerminal("updateStructure");
        var deleteFromStructure = new NonTerminal("deleteFromStructure");
        var insertIntoStructure = new NonTerminal("insertIntoStructure");


        // Grammar Rules

        columnDefinition.Rule = identifier + columnType;
        columnDefining.Rule = MakePlusRule(columnDefining, ToTerm(","), columnDefinition);
        createTableStructure.Rule = create + table + identifier + "(" + columnDefining + ")"
                                  | create + table + identifier;
        selectFromStructure.Rule = select + identifier + from + identifier |
                                   select + identifier + from + identifier + where + identifier + comparator + identifier|
                                   select + identifier + from + identifier + where + identifier + comparator + identifier + orderBy + identifier + identifier;
        createDBStructure.Rule = create + database + identifier;
        setDBStructure.Rule = set + database + identifier;
        dropTableStructure.Rule = drop + table + identifier;
        createIndexStructure.Rule = create + index + identifier + on + identifier + "(" + identifier + ")" + of + type + identifier;
        updateStructure.Rule = update + identifier + set + identifier + comparator + identifier
                             | update + identifier + set + identifier + comparator + identifier + where + identifier + comparator + identifier;
        deleteFromStructure.Rule = delete + from + identifier
                                 | delete + from + identifier + where + identifier + comparator + identifier;
        insertIntoStructure.Rule = insert + into + identifier + values + "(" + columnDefining + ")";

        this.Root = new NonTerminal("statement");
        this.Root.Rule = createTableStructure | selectFromStructure
                        | createDBStructure   | setDBStructure
                        | dropTableStructure  | createIndexStructure
                        | updateStructure     | deleteFromStructure
                        | insertIntoStructure;

        MarkPunctuation("(", ")", ",");
        RegisterBracePair("(",")");

    }
} 