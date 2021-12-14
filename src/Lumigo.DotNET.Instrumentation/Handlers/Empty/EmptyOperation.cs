using Amazon.Runtime;

namespace Lumigo.DotNET.Instrumentation.Handlers.Empty
{
    public class EmptyOperation : IOperationHandler
    {
        public void HandleOperationAfter(IExecutionContext context) { }

        public void HandleOperationBefore(IExecutionContext context) { }
    }
}
