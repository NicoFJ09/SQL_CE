using ApiInterface.Exceptions;
using ApiInterface.InternalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiInterface.Processors
{
    internal class ProcessorFactory
    {
        internal static IProcessor Create(Request request)
        {
            if (request.RequestType is RequestType.SQLSentence)
            {
                Console.WriteLine("Yes it reaches processor factory, error is in parsing");
                return new SQLSentenceProcessor(request);
            }
            throw new UnknowRequestTypeException();
        }
    }
}
