using System;
using Amazon.Runtime;

namespace Lumigo.DotNET.Instrumentation.Handlers.SimpleSystemsManagement
{
    public class SimpleSystemsManagementServiceHandler : BaseServiceHandler
    {
        private static SimpleSystemsManagementOperationsFactory factory = new SimpleSystemsManagementOperationsFactory();


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
