using System;
using System.Net.Http;
using System.Threading.Tasks;
using AspectInjector.Broker;
using Lumigo.DotNET.Utilities.Extensions;

namespace Lumigo.DotNET.Utilities.Http
{
    [Aspect(Scope.Global)]
    [Injection(typeof(HttpAspect))]
    public class HttpAspect : Attribute
    {
        private readonly SpansContainer spansContainer;
        public HttpAspect()
        {
            spansContainer = SpansContainer.GetInstance();
        }

        [Advice(Kind.Around)]
        public object AddScope([Argument(Source.Arguments)] object[] args,
            [Argument(Source.Name)] string name,
            [Argument(Source.ReturnType)] Type returnType,
            [Argument(Source.Target)] Func<object[], object> target)
        {
            if (typeof(Task<HttpResponseMessage>) == returnType)
            {
                long startTime = DateTime.UtcNow.ToMilliseconds();
                var funcTask = (Task<HttpResponseMessage>)target.Invoke(args);
                var response = funcTask.GetAwaiter().GetResult();
                try
                {
                    var requestUri = GetUri(args);
                    var content = GetHttpContent(args);
                    var method = GetMethod(name, args);
                    spansContainer.AddHttpSpan(startTime, requestUri, method, content, response);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, e.Message);
                }
                return Task.FromResult(response);
            }
            return target.Invoke(args);
        }

        public string GetUri(object[] args)
        {
            foreach (var arg in args)
            {
                if (arg.GetType() == typeof(string))
                {
                    return (string)arg;
                }
                else if (arg.GetType() == typeof(Uri))
                {
                    return ((Uri)arg).AbsoluteUri;
                }
            }

            return null;
        }

        public HttpContent GetHttpContent(object[] args)
        {
            foreach (var arg in args)
            {
                if (typeof(HttpContent).IsAssignableFrom(arg.GetType()))
                {
                    return (HttpContent)arg;
                }
            }

            return null;
        }

        public string GetMethod(string name, object[] args)
        {
            if (name.Contains("Send"))
            {
                foreach (var arg in args)
                {
                    if (arg.GetType() == typeof(HttpRequestMessage))
                    {
                        return ((HttpRequestMessage)arg).Method?.Method;
                    }
                }
                return null;
            }
            else
            {
                return name.Replace("Async", "");
            }
        }

    }
}
