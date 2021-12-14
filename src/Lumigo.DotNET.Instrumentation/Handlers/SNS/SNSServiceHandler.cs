using System;
using Amazon.Runtime;

namespace Lumigo.DotNET.Instrumentation.Handlers.SNS
{
    public class SNSServiceHandler : BaseServiceHandler
    {
        private static SNSOperationsFactory factory = new SNSOperationsFactory();


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
