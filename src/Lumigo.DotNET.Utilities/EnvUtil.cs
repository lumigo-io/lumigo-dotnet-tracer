using System;
using System.Collections.Generic;

namespace Lumigo.DotNET.Utilities
{
    public class EnvUtil
    {
        public string GetEnv(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }

        public void SetEnv(string key, string value)
        {
            Environment.SetEnvironmentVariable(key, value);
        }

        public Dictionary<string, object> GetAll()
        {
            var response = new Dictionary<string, object>();

            foreach (System.Collections.DictionaryEntry env in Environment.GetEnvironmentVariables())
            {
                response.Add((string)env.Key, env.Value);
            }

            return response;
        }

        public bool GetBooleanEnv(string key, bool dflt)
        {
            string value = GetEnv(key);
            return value == null ? dflt : value.ToLower() == "true";
        }

        public int GetIntegerEnv(string key, int dflt)
        {
            try
            {
                string value = GetEnv(key);
                return int.Parse(value);
            }
            catch (Exception e)
            {
                Logger.LogDebug(string.Format("No configuration to key {0} use default {1}", key, dflt));
            }
            return dflt;
        }

        public string[] GetStringArrayEnv(string key, string[] dflt)
        {
            string value = GetEnv(key);
            if (!string.IsNullOrEmpty(value))
            {
                return value.Split(',');
            }
            return dflt;
        }
    }
}
