using System;
using Amazon.Runtime;

namespace Lumigo.DotNET.Instrumentation.Handlers.Cloudwatch
{
    public class CloudwatchEventsServiceHandler : BaseServiceHandler
    {
        private static CloudwatchEventsOperationsFactory factory = new CloudwatchEventsOperationsFactory();


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
