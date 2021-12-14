using System;
using System.Collections.Generic;

namespace Lumigo.DotNET.Instrumentation.Handlers.SNS
{
    public class SNSOperationsFactory : BaseOperationsFactory
    {
        protected override Dictionary<string, Func<IOperationHandler>> Operations => new Dictionary<string, Func<IOperationHandler>>()
        {

        };
    }
}
