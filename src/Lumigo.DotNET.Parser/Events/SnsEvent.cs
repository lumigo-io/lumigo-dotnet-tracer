using System.Collections.Generic;

namespace Lumigo.DotNET.Parser.Events
{
    public class SnsEvent
    {
        public List<Record> Records;

        public class Record
        {
            public string Message;
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
