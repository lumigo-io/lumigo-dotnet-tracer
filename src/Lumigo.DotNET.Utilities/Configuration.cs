using System;

namespace Lumigo.DotNET.Utilities
{
    public class Configuration
    {
        private static readonly string EDGE_PREFIX = "https://";
        private static readonly string EDGE_SUFFIX = "/api/spans";
        private static readonly string EDGE_DEFAULT_URL = "{0}.lumigo-tracer-edge.golumigo.com";

        public static readonly string REGION_KEY = "AWS_REGION";
        public static readonly string DEBUG_KEY = "LUMIGO_DEBUG";
        public static readonly string TOKEN_KEY = "LUMIGO_TRACER_TOKEN";
        public static readonly string LUMIGO_VERBOSE = "LUMIGO_VERBOSE";
        public static readonly string EXTENSION_DIR = "/tmp/lumigo-spans";
        public static readonly string TRACER_HOST_KEY = "LUMIGO_TRACER_HOST";
        public static readonly string LUMIGO_KILL_SWITCH = "LUMIGO_SWITCH_OFF";
        public static readonly string REPORTER_TIMEOUT = "LUMIGO_REPORTER_TIMEOUT";
        public static readonly string LUMIGO_MAX_ENTRY_SIZE = "LUMIGO_MAX_ENTRY_SIZE";
        public static readonly string LUMIGO_INSTRUMENTATION = "LUMIGO_INSTRUMENTATION";
        public static readonly string LUMIGO_MAX_RESPONSE_SIZE = "LUMIGO_MAX_RESPONSE_SIZE";
        public static readonly string LUMIGO_MAX_EXECUTION_TAGS = "LUMIGO_MAX_EXECUTION_TAGS";
        public static readonly string LUMIGO_USE_TRACER_EXTENSION = "LUMIGO_USE_TRACER_EXTENSION";

        private static Configuration instance;

        private readonly EnvUtil envUtil = new EnvUtil();

        public static Configuration GetInstance()
        {
            if (instance == null)
            {
                instance = new Configuration();
            }
            return instance;
        }

        public int MaxSpanFieldSize()
        {
            return envUtil.GetIntegerEnv(LUMIGO_MAX_ENTRY_SIZE, 1024);
        }

        public int MaxRequestSize()
        {
            return envUtil.GetIntegerEnv(LUMIGO_MAX_RESPONSE_SIZE, 1024 * 900);
        }

        public int MaxExecutionTags()
        {
            return envUtil.GetIntegerEnv(LUMIGO_MAX_EXECUTION_TAGS, 50);
        }

        public string GetLumigoToken()
        {
            return envUtil.GetEnv(TOKEN_KEY);
        }

        public string GetLumigoTracerVersion()
        {
            return "1.0.38";  // Being updated from .bumpversion.cfg
        }

        public bool DebugMode
        {
            get { return envUtil.GetBooleanEnv(DEBUG_KEY, false); }

        }

        public bool IsWriteToFile()
        {
            return envUtil.GetBooleanEnv(LUMIGO_USE_TRACER_EXTENSION, false);
        }

        public bool IsLumigoVerboseMode()
        {
            return envUtil.GetBooleanEnv(LUMIGO_VERBOSE, true);
        }

        public TimeSpan GetLumigoTimeout()
        {
            try
            {
                string timeout = envUtil.GetEnv(REPORTER_TIMEOUT);
                if (timeout != null)
                {
                    return TimeSpan.FromMilliseconds(long.Parse(timeout));
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "LUMIGO_REPORTER_TIMEOUT isn't a timestamp, using default");
            }
            return TimeSpan.FromMilliseconds(700);
        }

        public bool IsAwsEnvironment()
        {
            return envUtil.GetEnv("LAMBDA_RUNTIME_DIR") != null;
        }

        public string GetLumigoEdge()
        {
            string url = envUtil.GetEnv(TRACER_HOST_KEY);

            if (url == null)
            {
                url = string.Format(EDGE_DEFAULT_URL, envUtil.GetEnv(REGION_KEY));
            }
            return EDGE_PREFIX + url + EDGE_SUFFIX;
        }

        public int MaxSpanFieldSizeWhenError()
        {
            return MaxSpanFieldSize() * 10;
        }

        public static bool IsKillingSwitchActivated()
        {
            try
            {
                return GetInstance().envUtil.GetBooleanEnv(LUMIGO_KILL_SWITCH, false);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to get killing switch");
                return true;
            }
        }
    }
}
