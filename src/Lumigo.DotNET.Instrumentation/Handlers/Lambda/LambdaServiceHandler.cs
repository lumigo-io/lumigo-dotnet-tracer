using System;
using Amazon.Runtime;

namespace Lumigo.DotNET.Instrumentation.Handlers.Lambda
{
    public class LambdaServiceHandler : BaseServiceHandler
    {
        private static LambdaOperationsFactory factory = new LambdaOperationsFactory();


        public override void HandleAfter(IExecutionContext executionContext)
        {
            factory.GetInstace(executionContext.RequestContext.RequestName).HandleOperationAfter(executionContext);

        }

        public override void HandleBefore(IExecutionContext executionContext)
        {
            factory.GetInstace(executionContext.RequestContext.RequestName).HandleOperationAfter(executionContext);
        }
    }
}
