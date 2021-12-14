using Newtonsoft.Json;
using System.Collections.Generic;

namespace Lumigo.DotNET.Parser.Spans
{
    public class HttpSpan
    {
        [JsonProperty("started")]
        public long Started { get; set; }

        [JsonProperty("ended")]
        public long Ended { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("tegion")]
        public string Region { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("info")]
        public InfoModel Info { get; set; }

        [JsonProperty("parentId")]
        public string ParentId { get; set; }


        public class InfoModel
        {
            public InfoModel()
            {

            }

            [JsonProperty("tracer")]
            public TracerModel Tracer { get; set; }

            [JsonProperty("traceId")]
            public TraceIdModel TraceId { get; set; }

            [JsonProperty("httpInfo")]
            public HttpInfoModel HttpInfo { get; set; }

            public InfoModel(TracerModel tracer, TraceIdModel traceId, HttpInfoModel httpInfo)
            {
                this.Tracer = tracer;
                this.TraceId = traceId;
                this.HttpInfo = httpInfo;
            }
        }

        public class TraceIdModel
        {

            [JsonProperty("Root")]
            public string Root { get; set; }
        }

        public class TracerModel
        {

            [JsonProperty("version")]
            public string Version { get; set; }
        }

        public class HttpInfoModel
        {

            [JsonProperty("host")]
            public string Host { get; set; }

            [JsonProperty("request")]
            public HttpDataModel Request { get; set; }

            [JsonProperty("response")]
            public HttpDataModel Response { get; set; }
        }

        public class HttpDataModel
        {

            [JsonProperty("headers")]
            public string Headers { get; set; }

            [JsonProperty("body")]
            public string Body { get; set; }

            [JsonProperty("uri")]
            public string Uri { get; set; }

            [JsonProperty("statusCode")]
            public int StatusCode { get; set; }

            [JsonProperty("method")]
            public string Method { get; set; }

            [JsonProperty("response")]
            public HttpDataModel Response { get; set; }
        }
    }
}
