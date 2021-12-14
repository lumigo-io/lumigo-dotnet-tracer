using System;
namespace Lumigo.DotNET.Utilities
{
    public class SafeUtils
    {
        public static void SafeExecute(Action callback, string message)
        {
            try
            {
                callback();
            }
            catch (Exception e)
            {
                Logger.LogError(e, message);
            }
        }
    }
}