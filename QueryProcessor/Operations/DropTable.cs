using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class DropTable
    {
        internal OperationStatus Execute(string tableName)
        {
            Console.WriteLine($"Dropping Table...{tableName}");
            return OperationStatus.Success;
        }
    }
}
