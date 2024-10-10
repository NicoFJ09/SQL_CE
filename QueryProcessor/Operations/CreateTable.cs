using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class CreateTable
    {
        internal OperationStatus Execute(string tableName)
        {
            return Store.GetInstance().CreateTable(tableName);
        }
    }
}
