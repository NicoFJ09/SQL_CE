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
        internal OperationStatus Execute()
        {
            Console.WriteLine("Creating DataBase...");
            return OperationStatus.Success;
        }
    }
}
