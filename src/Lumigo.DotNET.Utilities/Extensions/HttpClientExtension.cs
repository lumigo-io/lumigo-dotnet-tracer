using System.Net.Http;
using Lumigo.DotNET.Utilities.Http;

namespace Lumigo.DotNET.Utilities.Extensions
{
    public static class HttpClientExtension
    {
        public static LumigoTracedHttpClient UseLumigo(this HttpClient httpClient)
        {
            return new LumigoTracedHttpClient(httpClient);
        }
    }
}
