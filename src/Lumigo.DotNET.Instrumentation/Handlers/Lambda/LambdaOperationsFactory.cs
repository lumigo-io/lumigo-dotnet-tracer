using System;
using System.Collections.Generic;

namespace Lumigo.DotNET.Instrumentation.Handlers.Lambda
{
    public class LambdaOperationsFactory : BaseOperationsFactory
    {
        protected override Dictionary<string, Func<IOperationHandler>> Operations => new Dictionary<string, Func<IOperationHandler>>()
        {

        };
    }
}
