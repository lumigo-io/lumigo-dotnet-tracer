using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Lumigo.DotNET.Utilities;
using Lumigo.DotNET.Instrumentation;
using Lumigo.DotNET.Utilities.Network;

namespace Lumigo.DotNET
{
    public class LumigoRequestHandler
    {
        private readonly Reporter reporter;
        private readonly SpansContainer spansContainer;

        public LumigoRequestHandler()
        {
            try
            {
                LumigoBootstrap.Bootstrap();
                reporter = new Reporter();
                spansContainer = SpansContainer.GetInstance();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to init LumigoRequestHandler");
            }
        }

        public void Handle<TEvent>(TEvent input, ILambdaContext context, Action handlerFunc)
        {
            if (Configuration.IsKillingSwitchActivated())
            {
                return;
            }
            try
            {
                Logger.LogDebug("Start Lumigo tracer");
                try
                {
                    spansContainer.Init(reporter, context, input);
                    spansContainer.Start().GetAwaiter().GetResult();
                    Logger.LogDebug("Finish sending start message and instrumentation");
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Failed to init span container in sync handler without response");
                }

                handlerFunc();

                try
                {
                    spansContainer.End().GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Failed to create end span when customer's lambda finished successfully");
                }

                return;
            }
            catch (Exception e)
            {
                try
                {
                    spansContainer.EndWithException(e).GetAwaiter().GetResult();
                    Logger.LogDebug(string.Format("Customer lambda had exception {0}", e.Message));
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to create end span when customer's lambda had exception");
                }
                throw;
            }
        }

        public TResponse Handle<TEvent, TResponse>(TEvent input, ILambdaContext context, Func<TResponse> handlerFunc)
        {
            if (Configuration.IsKillingSwitchActivated())
            {
                return handlerFunc();
            }
            TResponse response = default;
            try
            {
                Logger.LogDebug("Start Lumigo tracer");
                try
                {
                    spansContainer.Init(reporter, context, input);
                    spansContainer.Start().GetAwaiter().GetResult();
                    Logger.LogDebug("Finish sending start message and instrumentation");
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Failed to init span container in sync handler with response");
                }

                response = handlerFunc();

                try
                {
                    spansContainer.End(response).GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Failed to create end span");
                }

                return response;
            }
            catch (Exception e)
            {
                try
                {
                    spansContainer.EndWithException(e).GetAwaiter().GetResult();
                    Logger.LogDebug(string.Format("Customer lambda had exception {0}", e.Message));
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to create end span");
                }
                throw;
            }
        }

        public async Task<TResponse> Handle<TEvent, TResponse>(TEvent input, ILambdaContext context, Func<Task<TResponse>> handlerFunc)
        {
            if (Configuration.IsKillingSwitchActivated())
            {
                return await handlerFunc();
            }

            TResponse response = default;
            try
            {
                Logger.LogDebug("Start Lumigo tracer");
                try
                {
                    spansContainer.Init(reporter, context, input);
                    await spansContainer.Start();
                    Logger.LogDebug("Finish sending start message and instrumentation");
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Failed to init span container in async handler with response");
                }

                response = await handlerFunc();

                try
                {
                    Logger.LogDebug("End span");
                    spansContainer.End(response).GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Failed to create end span");
                }

                return response;
            }
            catch (Exception e)
            {
                try
                {
                    Logger.LogDebug(string.Format("Customer lambda had exception {0}", e.Message));
                    spansContainer.EndWithException(e).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to create end span with Exception");
                }
                throw;
            }
        }

        public void AddExecutionTag(String key, String value)
        {
            SpansContainer.GetInstance().AddExecutionTag(key, value);
        }
    }
}
