using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace Lumigo.DotNET.Utilities.Http
{
    public class LumigoTracedHttpClient : HttpClient
    {
        private readonly HttpClient _client;
        private readonly SpansContainer spansContainer;

        public LumigoTracedHttpClient(HttpClient httpClient)
        {
            _client = httpClient;
            spansContainer = SpansContainer.GetInstance();
        }

        public new HttpRequestHeaders DefaultRequestHeaders => _client.DefaultRequestHeaders;
        public new Uri BaseAddress => _client.BaseAddress;
        public new long MaxResponseContentBufferSize => _client.MaxResponseContentBufferSize;
        public new TimeSpan Timeout => _client.Timeout;

        [HttpAspect]
        public new async Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken) => await _client.DeleteAsync(requestUri, cancellationToken);
        [HttpAspect]
        public new async Task<HttpResponseMessage> DeleteAsync(string requestUri) => await _client.DeleteAsync(requestUri);
        [HttpAspect]
        public new async Task<HttpResponseMessage> DeleteAsync(Uri requestUri) => await _client.DeleteAsync(requestUri);
        [HttpAspect]
        public new async Task<HttpResponseMessage> GetAsync(string requestUri) => await _client.GetAsync(requestUri);
        [HttpAspect]
        public new async Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption) => await _client.GetAsync(requestUri, completionOption);
        [HttpAspect]
        public new async Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken) => await _client.GetAsync(requestUri, completionOption, cancellationToken);
        [HttpAspect]
        public new async Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken) => await _client.GetAsync(requestUri, cancellationToken);
        [HttpAspect]
        public new async Task<HttpResponseMessage> GetAsync(Uri requestUri) => await _client.GetAsync(requestUri);
        [HttpAspect]
        public new async Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption) => await _client.GetAsync(requestUri, completionOption);
        [HttpAspect]
        public new async Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken) => await _client.GetAsync(requestUri, completionOption, cancellationToken);
        [HttpAspect]
        public new async Task<HttpResponseMessage> GetAsync(Uri requestUri, CancellationToken cancellationToken) => await _client.GetAsync(requestUri, cancellationToken);
        [HttpAspect]
        public new async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content) => await _client.PostAsync(requestUri, content);
        [HttpAspect]
        public new async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken) => await _client.PostAsync(requestUri, content, cancellationToken);
        [HttpAspect]
        public new async Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content) => await _client.PostAsync(requestUri, content);
        [HttpAspect]
        public new async Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken) => await _client.PostAsync(requestUri, content, cancellationToken);
        [HttpAspect]
        public new async Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content) => await _client.PutAsync(requestUri, content);
        [HttpAspect]
        public new async Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content, CancellationToken cancellationToken) => await _client.PutAsync(requestUri, content, cancellationToken);
        [HttpAspect]
        public new async Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content) => await _client.PutAsync(requestUri, content);
        [HttpAspect]
        public new async Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken) => await _client.PutAsync(requestUri, content, cancellationToken);
        [HttpAspect]
        public new async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request) => await _client.SendAsync(request);
        [HttpAspect]
        public new async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption) => await _client.SendAsync(request, completionOption);
        [HttpAspect]
        public new async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken) => await _client.SendAsync(request, completionOption, cancellationToken);
        [HttpAspect]
        public new async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => await _client.SendAsync(request, cancellationToken);
    }
}
