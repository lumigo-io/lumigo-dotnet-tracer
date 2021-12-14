using System;
using Amazon.Runtime;
using Amazon.S3.Model;

namespace Lumigo.DotNET.Instrumentation.Handlers.S3.Operations
{
    public class GetObjectRequestHandler : IOperationHandler
    {
        public void HandleOperationAfter(IExecutionContext context)
        {
        }

        public void HandleOperationBefore(IExecutionContext context)
        {
        }
    }
}
