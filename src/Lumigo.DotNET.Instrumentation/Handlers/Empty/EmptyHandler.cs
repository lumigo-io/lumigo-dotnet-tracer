using System;
using Amazon.Runtime;
using Newtonsoft.Json;
using Lumigo.DotNET.Utilities.Extensions;

namespace Lumigo.DotNET.Instrumentation.Handlers.Empty
{
    public class EmptyHandler : BaseServiceHandler
    {
        JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new IgnoreStreamsResolver() };

        private string _serviceName;

        public EmptyHandler(string serviceName)
        {
            this._serviceName = serviceName;
        }

        public override void HandleAfter(IExecutionContext executionContext)
        {
        }

        public override void HandleBefore(IExecutionContext executionContext)
        {
            var serviceId = executionContext?.RequestContext?.ServiceMetaData?.ServiceId;
        }
    }
}
