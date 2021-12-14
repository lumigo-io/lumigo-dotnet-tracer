using System;
using Amazon.Runtime;

namespace Lumigo.DotNET.Instrumentation.Handlers.SQS
{
    public class SQSServiceHandler : BaseServiceHandler
    {
        private static SQSOperationsFactory factory = new SQSOperationsFactory();


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
