using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Lumigo.DotNET.Utilities.Extensions;

namespace Lumigo.DotNET.Utilities.Network
{
    public class Reporter
    {
        private HttpClient client;

        public Reporter()
        {
            client = new HttpClient
            {
                Timeout = Configuration.GetInstance().GetLumigoTimeout()
            };
        }

        public Reporter(HttpClient client)
        {
            this.client = client;
        }

        public async Task<double> ReportSpans(object span, int maxSize)
        {
            return await ReportSpans(new List<object>() { span }, maxSize, "_span");
        }

        public async Task<double> ReportSpans(List<object> spans, int maxSize, string suffix = "_end")
        {
            double time = DateTime.UtcNow.ToMilliseconds();
            var spansAsStringList = new List<string>();
            int sizeCount = 0;
            int handledSpans = 0;
            foreach (var span in spans)
            {
                if (sizeCount >= maxSize)
                {
                    Logger.LogDebug(string.Format("Dropped spans by request size: {0}", spans.Count - handledSpans));
                    break;
                }
                string spanAsString = JsonConvert.SerializeObject(span);
                if (spanAsString != null)
                {
                    spansAsStringList.Add(spanAsString);
                    sizeCount += StringUtils.GetBase64Size(spanAsString);
                }
                handledSpans++;
            }

            if (Configuration.GetInstance().IsAwsEnvironment() && spansAsStringList.Any())
            {
                string spansAsString = "[" + string.Join(",", spansAsStringList) + "]";
                Logger.LogDebug(string.Format("Reporting the spans: {0}", spansAsString));
                if (Configuration.GetInstance().IsWriteToFile())
                {
                    string fileName = StringUtils.BuildMd5Hash(spansAsString) + suffix;
                    string filePath = Path.Combine(Configuration.EXTENSION_DIR, fileName);
                    File.WriteAllText(filePath, spansAsString);
                }
                else
                {
                    var data = new StringContent(spansAsString, Encoding.UTF8, "application/json");
                    var url = Configuration.GetInstance().GetLumigoEdge();
                    HttpResponseMessage result;
                    Logger.LogDebug(string.Format("Sending spans to the edge: {0}. Timeout: {1}", url, client.Timeout));
                    try
                    {
                        result = await client.PostAsync(url, data);
                    }
                    catch (TaskCanceledException ex)
                    {
                        Logger.LogError(string.Format("Failed to PostAsync. IsCancellationRequested: {0}", ex.CancellationToken.IsCancellationRequested));
                        throw;
                    }
                    Logger.LogDebug(string.Format("Reported successfully. {0}", JsonConvert.SerializeObject(result)));
                }

                return DateTime.UtcNow.ToMilliseconds() - time;
            }
            return 0;
        }
    }
}
