using System;
using Amazon.Runtime;

namespace Lumigo.DotNET.Instrumentation.Handlers.Cloudwatch
{
    public class CloudwatchLogsServiceHandler : BaseServiceHandler
    {
        private static CloudwatchLogsOperationsFactory factory = new CloudwatchLogsOperationsFactory();


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
