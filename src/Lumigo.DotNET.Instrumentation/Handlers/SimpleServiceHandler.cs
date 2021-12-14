using System;
using Amazon.Runtime;

namespace Lumigo.DotNET.Instrumentation.Handlers
{
    public class SimpleServiceHandler<TOpFactory> : BaseServiceHandler where TOpFactory : IFactory<string, IOperationHandler>
    {
        readonly IFactory<string, IOperationHandler> factory = Activator.CreateInstance<TOpFactory>();
        public override void HandleAfter(IExecutionContext executionContext)
        {
            var operation = executionContext.RequestContext.RequestName;
            var instance = factory.GetInstace(operation);

            if (!(instance is null))
            {
                instance.HandleOperationAfter(executionContext);
            }
        }

        public override void HandleBefore(IExecutionContext executionContext)
        {
            var operation = executionContext.RequestContext.RequestName;
            var instance = factory.GetInstace(operation);

            if (!(instance is null))
            {
                instance.HandleOperationBefore(executionContext);
            }
        }
    }
}
