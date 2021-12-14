using System;
using System.Collections.Generic;

namespace Lumigo.DotNET.Instrumentation.Handlers.Cognito
{
    public class CognitoOperationsFactory : BaseOperationsFactory
    {
        protected override Dictionary<string, Func<IOperationHandler>> Operations => new Dictionary<string, Func<IOperationHandler>>()
        {

        };
    }
}
