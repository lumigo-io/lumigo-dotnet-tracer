using System;
using Amazon.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Lumigo.DotNET.Utilities.Extensions;

namespace Lumigo.DotNET.Instrumentation.Handlers.Empty
{
    public class EmptyHandler : BaseServiceHandler
    {
        private JsonSerializerSettings _jsonSerializerSettings;

        // Property with lazy initialization
        private JsonSerializerSettings JsonSerializerSettings
        {
            get
            {
                if (_jsonSerializerSettings == null)
                {
                    _jsonSerializerSettings = new JsonSerializerSettings
                    {
                        ContractResolver = new DynamicIgnoreResolver(),
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        Error = HandleSerializationError
                    };
                }
                return _jsonSerializerSettings;
            }
        }

        private void HandleSerializationError(object sender, ErrorEventArgs args)
        {
            var currentError = args.ErrorContext.Error.Message;
            Logger.logDebug("Failed to Serialize JSON" + currentError);

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
