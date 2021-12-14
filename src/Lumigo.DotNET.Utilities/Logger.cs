using System;

namespace Lumigo.DotNET.Utilities
{
    public static class Logger
    {
        public static void LogError(Exception ex, string message)
        {
            if (Configuration.GetInstance().DebugMode)
            {
                Console.Error.WriteLine("#LUMIGO# - Error - {0}, {1}", message, ex.ToString());
            }
        }

        public static void LogError(string message)
        {
            if (Configuration.GetInstance().DebugMode)
            {
                Console.Error.WriteLine("#LUMIGO# - Error - {0}", message);
            }
        }

        public static void LogDebug(string message)
        {
            if (Configuration.GetInstance().DebugMode)
            {
                Console.Error.WriteLine("#LUMIGO# - Debug - {0}", message);
            }
        }
    }
}
