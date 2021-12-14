using Lumigo.DotNET.Instrumentation.Handlers.Empty;

namespace Lumigo.DotNET.Instrumentation
{
    public abstract class BaseOperationsFactory : BaseFactory<string, IOperationHandler>
    {
        protected BaseOperationsFactory() : base(new EmptyOperation()) { }
    }
}
