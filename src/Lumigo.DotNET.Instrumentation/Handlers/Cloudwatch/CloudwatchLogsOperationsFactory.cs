using System;
using System.Collections.Generic;

namespace Lumigo.DotNET.Instrumentation.Handlers.Cloudwatch
{
    public class CloudwatchLogsOperationsFactory : BaseOperationsFactory
    {
        protected override Dictionary<string, Func<IOperationHandler>> Operations => new Dictionary<string, Func<IOperationHandler>>()
        {

        };
    }
}
