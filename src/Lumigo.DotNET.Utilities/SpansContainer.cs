using System;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Amazon.Runtime;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Lumigo.DotNET.Parser;
using Lumigo.DotNET.Parser.Spans;
using Lumigo.DotNET.Utilities.Models;
using Lumigo.DotNET.Utilities.Network;
using Lumigo.DotNET.Utilities.Extensions;
using static Lumigo.DotNET.Parser.Spans.Span;

namespace Lumigo.DotNET.Utilities
{
    public class SpansContainer
    {
        private readonly int MAX_STRING_SIZE = Configuration.GetInstance().MaxSpanFieldSize();
        private readonly int MAX_REQUEST_SIZE = Configuration.GetInstance().MaxRequestSize();
        private readonly int MAX_EXECUTION_TAGS = Configuration.GetInstance().MaxExecutionTags();
        private const int MAX_LAMBDA_TIME = 15 * 60 * 1000;
        private const string AWS_EXECUTION_ENV = "AWS_EXECUTION_ENV";
        private const string AWS_REGION = "AWS_REGION";
        private const string AMZN_TRACE_ID = "_X_AMZN_TRACE_ID";
        private const string FUNCTION_SPAN_TYPE = "function";
        private const string HTTP_SPAN_TYPE = "http";

        private Span BaseSpan;
        private Span StartFunctionSpan;
        private double RttDuration;
        private Span EndFunctionSpan;
        private Reporter Reporter;
        private List<HttpSpan> HttpSpans = new List<HttpSpan>();
        private EnvUtil EnvUtil = new EnvUtil();
        private List<ExecutionTag> ExecutionTags = new List<ExecutionTag>();

        private static SpansContainer OurInstance = new SpansContainer();
        JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DynamicIgnoreResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Error = (sender, args) => HandleSerializationError(sender, args)
        };

        private void HandleSerializationError(object sender, ErrorEventArgs args)
        {
            var currentError = args.ErrorContext.Error.Message;
            Logger.LogError($"Serialization error: {currentError}");

            // Ignore the error and continue serialization
            args.ErrorContext.Handled = true;
        }

        public static SpansContainer GetInstance()
        {
            return OurInstance;
        }

        public void Clear()
        {
            BaseSpan = null;
            StartFunctionSpan = null;
            RttDuration = 0;
            EndFunctionSpan = null;
            Reporter = null;
            HttpSpans = new List<HttpSpan>();
            ExecutionTags = new List<ExecutionTag>();
        }

        public void Init(Reporter reporter, ILambdaContext context, object evnt)
        {
            try
            {
                Logger.LogDebug("ENVIRONMENT VARIABLES: " +
                                JsonConvert.SerializeObject(Environment.GetEnvironmentVariables()));
                Logger.LogDebug("CONTEXT: " + JsonConvert.SerializeObject(context));
                Logger.LogDebug("EVENT: " + JsonConvert.SerializeObject(evnt));
                Logger.LogDebug("REPORTER: " + JsonConvert.SerializeObject(reporter));
            } catch (Exception e)
            {
                Logger.LogError(e, "Failed to log init data");
            }

            this.Clear();
            this.Reporter = reporter;
            var awsTracerId = EnvUtil.GetEnv(AMZN_TRACE_ID);
            TriggeredByModel triggeredBy = AwsUtils.ExtractTriggeredByFromEvent(evnt);
            long startTime = DateTime.UtcNow.ToMilliseconds();
            Logger.LogDebug("Start Init Span");
            this.BaseSpan = new Span
            {
                Token = Configuration.GetInstance().GetLumigoToken(),
                Id = context.AwsRequestId,
                Started = startTime,
                Name = context.FunctionName,
                Runtime = EnvUtil.GetEnv(AWS_EXECUTION_ENV),
                Region = EnvUtil.GetEnv(AWS_REGION),
                MemoryAllocated = context.MemoryLimitInMB.ToString(),
                RequestId = context.AwsRequestId,
                Account = AwsUtils.ExtractAwsAccountFromArn(context.InvokedFunctionArn),
                MaxFinishTime = startTime + (long)(context.RemainingTime.TotalMilliseconds > 0 ? context.RemainingTime.TotalMilliseconds : MAX_LAMBDA_TIME),
                TransactionId = AwsUtils.ExtractAwsTraceTransactionId(awsTracerId),
                Info = new Span.InfoModel
                {
                    Tracer = new Span.TracerModel
                    {
                        Version = Configuration.GetInstance().GetLumigoTracerVersion()
                    },
                    TraceId = new Span.TraceIdModel
                    {
                        Root = AwsUtils.ExtractAwsTraceRoot(awsTracerId)
                    },
                    TriggeredBy = triggeredBy?.TriggeredBy,
                    Api = triggeredBy?.Api,
                    Arn = triggeredBy?.Arn,
                    HttpMethod = triggeredBy?.HttpMethod,
                    Resource = triggeredBy?.Resource,
                    Stage = triggeredBy?.Stage,
                    Region = triggeredBy?.Region,
                    MessageId = triggeredBy?.MessageId,
                    MessageIds = triggeredBy?.MessageIds,
                    ApproxEventCreationTime = triggeredBy == null ? 0 : triggeredBy.ApproxEventCreationTime,
                    LogGroupName = context.LogGroupName,
                    LogStreamName = context.LogStreamName
                },
                Type = FUNCTION_SPAN_TYPE,
                Readiness = AwsUtils.GetFunctionReadiness(),
                Envs = Configuration.GetInstance().IsLumigoVerboseMode() ? JsonConvert.SerializeObject(EnvUtil.GetAll()) : null,
                Event = Configuration.GetInstance().IsLumigoVerboseMode() ? JsonConvert.SerializeObject(EventParserFactory.ParseEvent(evnt), jsonSerializerSettings) : null

            };
            Logger.LogDebug("Finish Init Span");
        }

        public async Task Start()
        {
            try
            {
                this.BaseSpan.Id += "_started";
                this.BaseSpan.Ended = this.BaseSpan.Started;
                StartFunctionSpan = BaseSpan;
                RttDuration = await Reporter.ReportSpans(PrepareToSend(StartFunctionSpan, false), MAX_REQUEST_SIZE);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to send start span");
                throw;
            }
        }

        public async Task End(object response)
        {
            Logger.LogDebug(response.ToString());
            BaseSpan.Id = BaseSpan.Id.Replace("_started", "");
            BaseSpan.ReturnValue = Configuration.GetInstance().IsLumigoVerboseMode() ? JsonConvert.SerializeObject(response, jsonSerializerSettings) : null;
            await End(BaseSpan);
        }
        public async Task End()
        {
            BaseSpan.Id = BaseSpan.Id.Replace("_started", "");
            BaseSpan.ReturnValue = null;
            await End(BaseSpan);
        }

        private async Task End(Span endFunctionSpan)
        {
            endFunctionSpan.Id = endFunctionSpan.Id.Replace("_started", "");
            endFunctionSpan.ReporterRTT = RttDuration;
            endFunctionSpan.Ended = DateTime.UtcNow.ToMilliseconds();
            endFunctionSpan.Id = BaseSpan.Id;
            endFunctionSpan.ExecutionTags = this.ExecutionTags;
            EndFunctionSpan = endFunctionSpan;
            await Reporter.ReportSpans(PrepareToSend(GetAllCollectedSpans(), EndFunctionSpan.Error != null), MAX_REQUEST_SIZE);
        }

        public async Task EndWithException(Exception e)
        {
            BaseSpan.Error = new Span.ErrorModel
            {
                Message = e.Message,
                Type = e.GetType().ToString(),
                Stacktrace = e.StackTrace
            };
            await End(BaseSpan);
        }

        public bool IsBaseSpanInitiated()
        {
            return BaseSpan != null;
        }


        private object PrepareToSend(object span, bool hasError)
        {
            return ReduceSpanSize(span, hasError);
        }

        private List<object> PrepareToSend(List<object> spans, bool hasError)
        {
            foreach (var span in spans)
            {
                ReduceSpanSize(span, hasError);
            }
            return spans;
        }

        public Span GetEndSpan()
        {
            return EndFunctionSpan;
        }

        public List<HttpSpan> GetHttpSpans()
        {
            return HttpSpans;
        }

        public object ReduceSpanSize(object span, bool hasError)
        {
            int maxFieldSize = hasError ? Configuration.GetInstance().MaxSpanFieldSizeWhenError() : Configuration.GetInstance().MaxSpanFieldSize();
            if (span is Span functionSpan)
            {
                functionSpan.Envs = StringUtils.GetMaxSizeString(functionSpan.Envs, Configuration.GetInstance().MaxSpanFieldSize());
                functionSpan.ReturnValue = StringUtils.GetMaxSizeString(functionSpan.ReturnValue, maxFieldSize);
                functionSpan.Event = StringUtils.GetMaxSizeString(functionSpan.Event, maxFieldSize);
            }
            else if (span is HttpSpan httpSpan)
            {
                if (httpSpan.Info.HttpInfo.Request != null)
                {
                    httpSpan.Info.HttpInfo.Request.Headers = StringUtils.GetMaxSizeString(httpSpan.Info.HttpInfo.Request.Headers, maxFieldSize);
                    httpSpan.Info.HttpInfo.Request.Body = StringUtils.GetMaxSizeString(httpSpan.Info.HttpInfo.Request.Body, maxFieldSize);
                }
                if (httpSpan.Info.HttpInfo.Response != null)
                {
                    httpSpan.Info.HttpInfo.Response.Headers = StringUtils.GetMaxSizeString(httpSpan.Info.HttpInfo.Response.Headers, maxFieldSize);
                    httpSpan.Info.HttpInfo.Response.Body = StringUtils.GetMaxSizeString(httpSpan.Info.HttpInfo.Response.Body, maxFieldSize);
                }
            }
            else
            {
                Logger.LogDebug("Unknown span type for ReduceSpanSize");
            }
            return span;
        }

        public List<object> GetAllCollectedSpans()
        {
            var spans = new List<object>();
            spans.AddRange(HttpSpans);
            spans.Add(EndFunctionSpan);
            return spans;
        }

        public void AddExecutionTag(string key, string value)
        {
            if (this.ExecutionTags.Count() < this.MAX_EXECUTION_TAGS)
            {
                this.ExecutionTags.Add(new ExecutionTag() { Key = key, Value = value });
            }
        }

        private HttpSpan CreateBaseHttpSpan(long startTime) => new HttpSpan
        {
            Id = Guid.NewGuid().ToString(),
            Started = startTime,
            Ended = DateTime.UtcNow.ToMilliseconds(),
            TransactionId = BaseSpan.TransactionId,
            Account = BaseSpan.Account,
            Region = BaseSpan.Region,
            Token = BaseSpan.Token,
            Type = HTTP_SPAN_TYPE,
            ParentId = BaseSpan.RequestId,
            Info = new HttpSpan.InfoModel()
            {
                Tracer = new HttpSpan.TracerModel
                {
                    Version = BaseSpan.Info.Tracer.Version
                },
                TraceId = new HttpSpan.TraceIdModel
                {
                    Root = BaseSpan.Info.TraceId.Root
                }
            }
        };

        public void AddHttpSpan(IExecutionContext executionContext, object result = null, long startTime = 0)
        {
            if (IsBaseSpanInitiated())
            {
                if (startTime == 0)
                    startTime = DateTime.UtcNow.ToMilliseconds();
                HttpSpan httpSpan = CreateBaseHttpSpan(startTime);

                httpSpan.Info.HttpInfo = new HttpSpan.HttpInfoModel
                {
                    Host = executionContext?.RequestContext?.Request?.Endpoint?.Host,
                    Request = new HttpSpan.HttpDataModel
                    {
                        Headers = JsonConvert.SerializeObject(executionContext?.RequestContext?.Request?.Headers),
                        Uri = executionContext?.RequestContext?.Request?.Endpoint?.AbsoluteUri,
                        Method = executionContext?.RequestContext?.Request?.HttpMethod,
                        Body = executionContext?.RequestContext?.Request?.Content == null ? null : Encoding.UTF8.GetString(executionContext.RequestContext.Request.Content, 0, executionContext.RequestContext.Request.Content.Length)

                    },
                    Response = new HttpSpan.HttpDataModel
                    {
                        StatusCode = executionContext?.ResponseContext?.HttpResponse?.StatusCode == null ? 0 : (int)executionContext?.ResponseContext?.HttpResponse?.StatusCode,
                        Method = executionContext?.RequestContext?.Request?.HttpMethod,
                        Uri = executionContext?.RequestContext?.Request?.Endpoint?.AbsoluteUri,
                        Body = result != null ? JsonConvert.SerializeObject(result, jsonSerializerSettings) : null
                    }
                };
                HttpSpans.Add(httpSpan);
            }
            else
            {
                Logger.LogDebug("Couldn't add HttpSpan because BaseSpan isn't initiated. probably this request happened before the handler is called");
            }
            
        }

        public void AddHttpSpan(long startTime, string requestUri, string method, HttpContent request, HttpResponseMessage response)
        {
            if (IsBaseSpanInitiated())
            {
                if (!CanAddHttpSpan())
                {
                    return;
                }

                HttpSpan httpSpan = CreateBaseHttpSpan(startTime);
                httpSpan.Info.HttpInfo = new HttpSpan.HttpInfoModel
                {
                    Host = requestUri,
                    Request = new HttpSpan.HttpDataModel
                    {
                        Headers = request == null
                            ? ExtractHeaders(
                                new HttpClient().DefaultRequestHeaders.ToDictionary(a => a.Key,
                                    a => string.Join(";", a.Value)))
                            : ExtractHeaders(request.Headers.ToDictionary(a => a.Key, a => string.Join(";", a.Value))),
                        Uri = requestUri,
                        Method = method,
                        Body = request == null ? null : ExtractBodyFromRequest(request),
                    },
                    Response = new HttpSpan.HttpDataModel
                    {
                        Headers = ExtractHeaders(response.Headers.ToDictionary(a => a.Key,
                            a => string.Join(";", a.Value))),
                        Body = ExtractBodyFromResponse(response),
                        StatusCode = (int)response.StatusCode
                    }
                };
                ReduceSpanSize(httpSpan, false);
                HttpSpans.Add(httpSpan);
            }
            else
            {
                Logger.LogDebug("Couldn't add HttpSpan because BaseSpan isn't initiated. probably this request happened before the handler is called");
            }
        }

        private static string ExtractHeaders(Dictionary<string, string> headers)
        {
            return JsonConvert.SerializeObject(headers);
        }

        private bool CanAddHttpSpan()
        {
            int HttpSpansLen = HttpSpans.Count();
            if (HttpSpansLen * MAX_STRING_SIZE > MAX_REQUEST_SIZE)
            {
                Logger.LogDebug(string.Format("Reched max http spans. HttpSpansLen: {0}. MAX_STRING_SIZE: {1}. MAX_REQUEST_SIZE: {2}", HttpSpansLen, MAX_STRING_SIZE, MAX_REQUEST_SIZE));
                return false;
            }
            return true;
        }

        private static string ExtractBodyFromRequest(HttpContent request)
        {
            return request.ReadAsStringAsync().GetAwaiter().GetResult();
        }
        private static string ExtractBodyFromResponse(HttpResponseMessage request)
        {
            return request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }


    }

}
