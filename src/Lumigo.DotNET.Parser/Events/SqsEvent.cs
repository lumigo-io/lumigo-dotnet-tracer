using System.Collections.Generic;

namespace Lumigo.DotNET.Parser.Events
{
    public class SqsEvent
    {
        public List<Record> Records;

        public class Record
        {
            public string Body;
            public Dictionary<string, MessageAttribute> MessageAttributes;
            public string MessageId;
        }

        public class MessageAttribute
        {
            public string Type;
            public string Value;
        }
    }
}
