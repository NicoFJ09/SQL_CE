using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class Insert
    {
        internal OperationStatus Execute()
        {
            Console.WriteLine("Inserting...");
            return OperationStatus.Success;
        }
    }
}
