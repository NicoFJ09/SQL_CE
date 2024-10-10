using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class SetDataBase
    {
        internal OperationStatus Execute(string dbname)
        {
            Console.WriteLine("Setting Database...");
            Console.WriteLine($"Setting in Database: {dbname}");
            return OperationStatus.Success;
        }
    }
}
