using System;
using System.Collections.Generic;

namespace Lumigo.DotNET.Instrumentation.Handlers.SQS
{
    public class SQSOperationsFactory : BaseOperationsFactory
    {
        protected override Dictionary<string, Func<IOperationHandler>> Operations => new Dictionary<string, Func<IOperationHandler>>()
        {

        };
    }
}
