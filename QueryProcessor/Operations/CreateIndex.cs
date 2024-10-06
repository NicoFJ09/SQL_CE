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
        internal OperationStatus Execute()
        {
            Console.WriteLine("Creating Index...");
            return OperationStatus.Success;
        }
    }
}
