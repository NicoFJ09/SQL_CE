using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class InsertInto
    {
        internal OperationStatus Execute()
        {
            Console.WriteLine("Inserting Into...");
            return OperationStatus.Success;
        }
    }
}
