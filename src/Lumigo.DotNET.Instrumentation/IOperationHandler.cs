using Amazon.Runtime;

namespace Lumigo.DotNET.Instrumentation
{
    public interface IOperationHandler
    {
        void HandleOperationBefore(IExecutionContext context);
        void HandleOperationAfter(IExecutionContext context);
    }
}
