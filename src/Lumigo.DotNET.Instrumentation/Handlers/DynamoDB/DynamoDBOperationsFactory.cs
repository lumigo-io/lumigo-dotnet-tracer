using System;
using System.Collections.Generic;

namespace Lumigo.DotNET.Instrumentation.Handlers.DynamoDB
{
    public class DynamoDBOperationsFactory : BaseOperationsFactory
    {
        protected override Dictionary<string, Func<IOperationHandler>> Operations => new Dictionary<string, Func<IOperationHandler>>()
        {

        };
    }
}
