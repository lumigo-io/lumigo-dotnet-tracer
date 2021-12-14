using System;
using Amazon.Lambda.SNSEvents;
using Amazon.Lambda.SQSEvents;
using Amazon.Lambda.APIGatewayEvents;

namespace Lumigo.DotNET.Parser
{
    public static class EventParserFactory
    {
        public static object ParseEvent(object evnt)
        {
            return evnt;
        }
    }
}
