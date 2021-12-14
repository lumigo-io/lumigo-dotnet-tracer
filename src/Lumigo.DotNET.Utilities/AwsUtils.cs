using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Amazon.Lambda.S3Events;
using Amazon.Lambda.SNSEvents;
using Amazon.Lambda.LexEvents;
using Amazon.Lambda.SQSEvents;
using Amazon.Lambda.ConfigEvents;
using Amazon.Lambda.CognitoEvents;
using Amazon.Lambda.KinesisEvents;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.CloudWatchLogsEvents;
using Amazon.Lambda.KinesisFirehoseEvents;
using Amazon.Lambda.KinesisAnalyticsEvents;
using Amazon.Lambda.CloudWatchEvents.ScheduledEvents;
using Lumigo.DotNET.Utilities.Extensions;
using Lumigo.DotNET.Utilities.Models;

namespace Lumigo.DotNET.Utilities
{
    public static class AwsUtils
    {
        public static string COLD_START_KEY = "LUMIGO_COLD_START_KEY";
        public static readonly int MAXIMAL_NUMBER_OF_MESSAGE_IDS = 50;
        public static readonly string TRIGGERED_BY_FALLBACK = "No recognized trigger";

        private static EnvUtil envUtil = new EnvUtil();

        public static TriggeredByModel ExtractTriggeredByFromEvent(object evnt)
        {
            TriggeredByModel triggeredBy = new TriggeredByModel();
            try
            {
                Logger.LogDebug(string.Format("Trying to find triggered by to evnt from class {0}", evnt != null ? evnt.GetType().Name : string.Empty));

                if (evnt == null)
                {
                    return null;
                }
                else if (evnt is DynamoDBEvent dynamoDBEvent)
                {
                    triggeredBy.TriggeredBy = "dynamodb";
                    if (dynamoDBEvent.Records != null && dynamoDBEvent.Records.Any())
                    {
                        var firstRecord = dynamoDBEvent.Records.First();
                        triggeredBy.Arn = firstRecord.EventSourceArn;
                        if (firstRecord.Dynamodb != null && firstRecord.Dynamodb.ApproximateCreationDateTime != null)
                        {
                            triggeredBy.ApproxEventCreationTime = firstRecord.Dynamodb.ApproximateCreationDateTime.ToMilliseconds();
                        }
                        List<string> messageIds = dynamoDBEvent.Records
                            .Select(x => ExtractMessageIdFromDynamodbRecord(x))
                            .Where(x => x != null)
                            .Take(MAXIMAL_NUMBER_OF_MESSAGE_IDS)
                            .ToList();

                        if (messageIds.Any())
                            triggeredBy.MessageIds = messageIds;
                    }
                }
                else if (evnt is KinesisEvent kinesisEvent)
                {
                    triggeredBy.TriggeredBy = "kinesis";
                    if (kinesisEvent.Records != null && kinesisEvent.Records.Any())
                    {
                        var records = kinesisEvent.Records.ToList();
                        triggeredBy.Arn = records.First().EventSourceARN;
                        List<string> messageIds = records.Select(x => x.Kinesis.SequenceNumber).ToList();
                        triggeredBy.MessageId = records.First().Kinesis.SequenceNumber;
                        triggeredBy.MessageIds = messageIds;
                    }
                }
                else if (evnt is KinesisFirehoseEvent kinesisFirehoseEvent)
                {
                    triggeredBy.TriggeredBy = "kinesis";
                    triggeredBy.Arn = kinesisFirehoseEvent.DeliveryStreamArn;
                }
                else if (evnt is KinesisAnalyticsFirehoseInputPreprocessingEvent preprocessingEvent)
                {
                    triggeredBy.TriggeredBy = "kinesis";
                    triggeredBy.Arn = preprocessingEvent.StreamArn;
                }
                else if (evnt is KinesisAnalyticsStreamsInputPreprocessingEvent inputPreprocessingEvent)
                {
                    triggeredBy.TriggeredBy = "kinesis";
                    triggeredBy.Arn = inputPreprocessingEvent.StreamArn;
                }
                else if (evnt is S3Event s3Event)
                {
                    triggeredBy.TriggeredBy = "s3";
                    if (s3Event.Records != null && s3Event.Records.Any())
                    {
                        triggeredBy.Arn = s3Event.Records.First().S3.Bucket.Arn;
                    }
                }
                else if (evnt is SNSEvent snsEvent)
                {
                    triggeredBy.TriggeredBy = "sns";
                    if (snsEvent.Records != null && snsEvent.Records.Any())
                    {
                        triggeredBy.Arn = snsEvent.Records.First().Sns.TopicArn;
                        triggeredBy.MessageId = snsEvent.Records.First().Sns.MessageId;
                    }
                }
                else if (evnt is SQSEvent sqsEvent)
                {
                    triggeredBy.TriggeredBy = "sqs";
                    if (sqsEvent.Records != null && sqsEvent.Records.Any())
                    {
                        triggeredBy.Arn = sqsEvent.Records.First().EventSourceArn;
                        triggeredBy.MessageId = sqsEvent.Records.First().MessageId;
                    }
                }
                else if (evnt is APIGatewayProxyRequest apiGatewayProxyRequest)
                {
                    triggeredBy.TriggeredBy = "apigw";
                    triggeredBy.HttpMethod = apiGatewayProxyRequest.HttpMethod;
                    triggeredBy.Resource = apiGatewayProxyRequest.Resource;
                    if (apiGatewayProxyRequest.Headers != null && apiGatewayProxyRequest.Headers.Any())
                    {
                        if (apiGatewayProxyRequest.Headers.TryGetValue("Host", out string header))
                            triggeredBy.Api = header;
                        else
                            triggeredBy.Api = "unknown.unknown.unknown";
                    }
                    if (apiGatewayProxyRequest.RequestContext != null && apiGatewayProxyRequest.RequestContext.Stage != null)
                    {
                        triggeredBy.Stage = apiGatewayProxyRequest.RequestContext.Stage;
                    }
                }
                else if (evnt is CloudWatchLogsEvent)
                {
                    triggeredBy.TriggeredBy = "cloudwatch";
                }
                else if (evnt is ScheduledEvent scheduledEvent)
                {
                    triggeredBy.TriggeredBy = "cloudwatch";
                    triggeredBy.Region = scheduledEvent.Region;
                    if (scheduledEvent.Resources != null && scheduledEvent.Resources.Any())
                    {
                        triggeredBy.Arn = scheduledEvent.Resources.First();
                        var arnParts = triggeredBy.Arn.Split('/');
                        if (arnParts.Length > 1)
                        {
                            triggeredBy.Resource = arnParts[1];
                        }
                    }
                }
                else if (evnt is ConfigEvent)
                {
                    triggeredBy.TriggeredBy = "config";
                }
                else if (evnt is LexEvent)
                {
                    triggeredBy.TriggeredBy = "lex";
                }
                else if (evnt is CognitoEvent)
                {
                    triggeredBy.TriggeredBy = "cognito";
                }
                else
                {
                    // Failed to find relevant triggered by
                    triggeredBy.TriggeredBy = TRIGGERED_BY_FALLBACK;
                    return triggeredBy;
                }

                // Found triggered by
                return triggeredBy;

            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to extract triggerBy data");
                triggeredBy.TriggeredBy = TRIGGERED_BY_FALLBACK;
                return triggeredBy;
            }
        }

        public static string ExtractMessageIdFromDynamodbRecord(DynamoDBEvent.DynamodbStreamRecord record)
        {
            if (record.EventName == null) return null;
            if (record.EventName == "INSERT")
            {
                return StringUtils.DynamoDBItemToHash(record.Dynamodb.NewImage);
            }
            else if (record.EventName == "MODIFY" || record.EventName == "REMOVE")
            {
                return StringUtils.DynamoDBItemToHash(record.Dynamodb.Keys);
            }
            return null;
        }

        public static string ExtractAwsAccountFromArn(string arn)
        {
            if (string.IsNullOrEmpty(arn))
                return null;

            string[] arnParts = arn.Split(':');
            if (arnParts.Length < 6)
            {
                return null;
            }
            return arnParts[4];
        }

        public static string ExtractAwsTraceRoot(string amznTraceId)
        {
            if (string.IsNullOrEmpty(amznTraceId))
                return null;

            Regex regex = new Regex("([^;]+)=([^;]*)");
            var matches = regex.Matches(amznTraceId);
            foreach (Match match in matches)
            {
                if (match.Groups[1].Value == "Root")
                    return match.Groups[2].Value;
            }
            return null;
        }

        public static string ExtractAwsTraceTransactionId(string amznTraceId)
        {
            string root = ExtractAwsTraceRoot(amznTraceId);
            if (root == null)
            {
                return null;
            }
            string[] rootParts = root.Split('-');
            if (rootParts.Length < 3)
            {
                return null;
            }
            return rootParts[2];
        }

        public static string ExtractAwsTraceSuffix(string amznTraceId)
        {
            if (string.IsNullOrEmpty(amznTraceId))
                return null;

            if (!amznTraceId.Contains(";"))
            {
                return amznTraceId;
            }
            return amznTraceId.Substring(amznTraceId.IndexOf(";"));
        }

        public static string GetFunctionReadiness()
        {
            if (envUtil.GetEnv(COLD_START_KEY) != null)
            {
                return "WARM";
            }
            else
            {
                envUtil.SetEnv(COLD_START_KEY, "false");
                return "COLD";
            }
        }

    }
}
