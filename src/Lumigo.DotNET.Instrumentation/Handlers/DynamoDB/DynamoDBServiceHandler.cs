using System;
using Amazon.Runtime;

namespace Lumigo.DotNET.Instrumentation.Handlers.DynamoDB
{
    public class DynamoDBServiceHandler : BaseServiceHandler
    {
        private static DynamoDBOperationsFactory factory = new DynamoDBOperationsFactory();


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
