using System;
using System.Collections.Generic;

namespace Lumigo.DotNET.Instrumentation.Handlers.SimpleSystemsManagement
{
    public class SimpleSystemsManagementOperationsFactory : BaseOperationsFactory
    {
        protected override Dictionary<string, Func<IOperationHandler>> Operations => new Dictionary<string, Func<IOperationHandler>>()
        {

        };
    }
}
