using System;
using Amazon.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Lumigo.DotNET.Utilities.Extensions;

namespace Lumigo.DotNET.Instrumentation.Handlers.Empty
{
    public class EmptyHandler : BaseServiceHandler
    {
        JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DynamicIgnoreResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Error = HandleSerializationError
        };

        private void HandleSerializationError(object sender, ErrorEventArgs args)
        {
            var currentError = args.ErrorContext.Error.Message;
            Logger.LogWarning($"Serialization error: {currentError}");

            // Ignore the error and continue serialization
            args.ErrorContext.Handled = true;
        }

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
