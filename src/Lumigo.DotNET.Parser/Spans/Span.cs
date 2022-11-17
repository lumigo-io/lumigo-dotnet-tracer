using Newtonsoft.Json;
using System.Collections.Generic;

namespace Lumigo.DotNET.Parser.Spans
{
    public class Span
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("started")]
        public long Started { get; set; }

        [JsonProperty("ended")]
        public long Ended { get; set; }

        [JsonProperty("runtime")]
        public string Runtime { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("memoryAllocated")]
        public string MemoryAllocated { get; set; }

        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("maxFinishTime")]
        public long MaxFinishTime { get; set; }

        [JsonProperty("event")]
        public string Event { get; set; }

        [JsonProperty("envs")]
        public string Envs { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("reporterRTT")]
        public double ReporterRTT { get; set; }

        [JsonProperty("error")]
        public ErrorModel Error { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("return_value")]
        public string ReturnValue { get; set; }

        [JsonProperty("info")]
        public InfoModel Info { get; set; }

        [JsonProperty("readiness")]
        public string Readiness { get; set; }

        [JsonProperty("parentId")]
        public string ParentId { get; set; }

        [JsonProperty("lumigo_execution_tags_no_scrub")]
        public List<ExecutionTag> ExecutionTags { get; set; }

        public class InfoModel
        {

            [JsonProperty("tracer")]
            public TracerModel Tracer { get; set; }

            [JsonProperty("traceId")]
            public TraceIdModel TraceId { get; set; }

            [JsonProperty("logStreamName")]
            public string LogStreamName { get; set; }

            [JsonProperty("logGroupName")]
            public string LogGroupName { get; set; }

            [JsonProperty("triggeredBy")]
            public string TriggeredBy { get; set; }

            [JsonProperty("arn")]
            public string Arn { get; set; }

            [JsonProperty("httpMethod")]
            public string HttpMethod { get; set; }

            [JsonProperty("resource")]
            public string Resource { get; set; }

            [JsonProperty("api")]
            public string Api { get; set; }

            [JsonProperty("stage")]
            public string Stage { get; set; }

            [JsonProperty("messageId")]
            public string MessageId { get; set; }

            [JsonProperty("messageIds")]
            public List<string> MessageIds { get; set; }

            [JsonProperty("approxEventCreationTime")]
            public double ApproxEventCreationTime { get; set; }

            [JsonProperty("region")]
            public string Region { get; set; }
        }

        public class ExecutionTag
        {

            [JsonProperty("key")]
            public string Key { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }
        }

        public class TracerModel
        {

            [JsonProperty("version")]
            public string Version { get; set; }
        }

        public class TraceIdModel
        {
            [JsonProperty("Root")]
            public string Root { get; set; }
        }

        public class ErrorModel
        {

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }

            [JsonProperty("stacktrace")]
            public string Stacktrace { get; set; }
        }

        public enum READINESS
        {
            WARM,
            COLD
        }
    }
}
