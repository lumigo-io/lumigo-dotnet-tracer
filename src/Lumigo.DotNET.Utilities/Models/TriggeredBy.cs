using System.Collections.Generic;

namespace Lumigo.DotNET.Utilities.Models
{
    public class TriggeredByModel
    {
        public string TriggeredBy { get; set; }
        public string Arn { get; set; }
        public string HttpMethod { get; set; }
        public string Resource { get; set; }
        public string Api { get; set; }
        public string Stage { get; set; }
        public string MessageId { get; set; }
        public List<string> MessageIds = new List<string>();
        public long ApproxEventCreationTime { get; set; } = 0;
        public string Region { get; set; }
    }
}
