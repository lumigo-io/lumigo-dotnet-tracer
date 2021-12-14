using Amazon.Runtime;

namespace Lumigo.DotNET.Instrumentation
{
    public interface IServiceHandler : IPipelineHandler
    {
        void HandleBefore(IExecutionContext executionContext);
        void HandleAfter(IExecutionContext executionContext);
    }
}
