using System;
using System.Collections.Generic;
using Lumigo.DotNET.Instrumentation.Handlers.S3.Operations;

namespace Lumigo.DotNET.Instrumentation.Handlers.S3
{
    public class S3OperationsFactory : BaseOperationsFactory
    {
        protected override Dictionary<string, Func<IOperationHandler>> Operations => new Dictionary<string, Func<IOperationHandler>>()
        {
            { "GetObjectRequest", () => new GetObjectRequestHandler() },
            { "GetRequest", () => new GetObjectRequestHandler() },
        };
    }
}
