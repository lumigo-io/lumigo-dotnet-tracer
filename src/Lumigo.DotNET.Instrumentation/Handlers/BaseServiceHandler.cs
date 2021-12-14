using System;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Lumigo.DotNET.Utilities;
using Lumigo.DotNET.Parser.Spans;
using Lumigo.DotNET.Utilities.Extensions;

namespace Lumigo.DotNET.Instrumentation.Handlers
{
    public abstract class BaseServiceHandler : PipelineHandler, IServiceHandler
    {
        private readonly SpansContainer spansContainer;
        public BaseServiceHandler()
        {
            spansContainer = SpansContainer.GetInstance();
        }

        public abstract void HandleAfter(IExecutionContext executionContext);
        public abstract void HandleBefore(IExecutionContext executionContext);

        public override void InvokeSync(IExecutionContext executionContext)
        {
            SafeUtils.SafeExecute(() => HandleBefore(executionContext), "Failed to HandleBefore");

            long startTime = DateTime.UtcNow.ToMilliseconds();
            base.InvokeSync(executionContext);

            SafeUtils.SafeExecute(() =>
            {
                HandleAfter(executionContext);
                spansContainer.AddHttpSpan(executionContext, null, startTime);
            }, "Failed to HandleAfter");
        }

        public override Task<T> InvokeAsync<T>(IExecutionContext executionContext)
        {
            SafeUtils.SafeExecute(() => HandleBefore(executionContext), "Failed to HandleBefore");

            long startTime = DateTime.UtcNow.ToMilliseconds();
            var result = base.InvokeAsync<T>(executionContext).Result;

            SafeUtils.SafeExecute(() =>
            {
                HandleAfter(executionContext);
                spansContainer.AddHttpSpan(executionContext, result, startTime);
            }, "Failed to HandleAfter");

            return Task.FromResult(result);
        }

        private void BuildSpan(IExecutionContext context, HttpSpan span)
        {
            var resoureType = context?.RequestContext?.ServiceMetaData.ServiceId;
            var serviceName = context?.RequestContext?.ServiceMetaData.ServiceId;
            var operationName = context?.RequestContext?.RequestName;

            var endpoint = context?.RequestContext?.Request?.Endpoint?.ToString();
            var region = context?.RequestContext?.ClientConfig?.RegionEndpoint?.SystemName;
            var envRegion = Environment.GetEnvironmentVariable("AWS_REGION");
        }
    }
}
