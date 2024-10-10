using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class CreateIndex
    {
        internal OperationStatus Execute(string tableName, string columnName)
        {
            return Store.GetInstance().IndexColumn(tableName, columnName);
        }
    }
}
