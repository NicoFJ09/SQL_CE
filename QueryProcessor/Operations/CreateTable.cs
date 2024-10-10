using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class CreateTable
    {
        internal OperationStatus Execute(string tableName, string[] columns)
        {
            return Store.GetInstance().CreateTable(tableName, columns);
        }
    }
}
