using System.Collections.Generic;

namespace Lumigo.DotNET.Parser.Events
{
    public class APIGWEvent
    {
        public string Path { get; set; }
        public string Resource { get; set; }
        public string HttpMethod { get; set; }
        public Dictionary<string, string> QueryStringParameters { get; set; }
        public Dictionary<string, string> PathParameters { get; set; }
        public string Body { get; set; }
        public Dictionary<string, object> Authorizer { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public Dictionary<string, string> StageVariables { get; set; }
    }
}
