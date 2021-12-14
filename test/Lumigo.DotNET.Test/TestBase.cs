using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lumigo.DotNET.Test
{
    public class TestBase
    {
        public TestBase()
        {
            string settingsFilePath = Path.Combine("Properties", "launchSettings.json");
            using var file = File.OpenText(settingsFilePath);
            var reader = new JsonTextReader(file);
            var jObject = JObject.Load(reader);

            var variables = jObject
                .GetValue("profiles")
                //select a proper profile here
                .SelectMany(profiles => profiles.Children())
                .SelectMany(profile => profile.Children<JProperty>())
                .Where(prop => prop.Name == "environmentVariables")
                .SelectMany(prop => prop.Value.Children<JProperty>())
                .ToList();

            foreach (var variable in variables)
            {
                Environment.SetEnvironmentVariable(variable.Name, variable.Value.ToString());
            }
            Environment.SetEnvironmentVariable("LUMIGO_MAX_ENTRY_SIZE", null);
            Environment.SetEnvironmentVariable("LUMIGO_MAX_RESPONSE_SIZE", null);
        }
    }
}
