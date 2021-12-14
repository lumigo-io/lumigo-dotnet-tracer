using Amazon.Runtime;

namespace Lumigo.DotNET.Instrumentation.Handlers.Cognito
{
    public class CognitoServiceHandler : BaseServiceHandler
    {
        private static CognitoOperationsFactory factory = new CognitoOperationsFactory();


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
