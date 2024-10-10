using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class CreateDataBase
    {
        internal OperationStatus Execute(string dbname)
        {
            Console.WriteLine("Creating DataBase...");
            Console.WriteLine($"DBName: {dbname}");
            var store = Store.GetInstance();
            store.CreateDatabase(dbname);
            return OperationStatus.Success;
        }
    }
}