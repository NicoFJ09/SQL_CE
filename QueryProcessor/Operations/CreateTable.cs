using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class CreateTable
    {
        internal OperationStatus Execute(string tableName, List<string> columns, string treeType)
        {
            return Store.GetInstance().CreateTable(tableName, columns, treeType);
        }
    }
}
