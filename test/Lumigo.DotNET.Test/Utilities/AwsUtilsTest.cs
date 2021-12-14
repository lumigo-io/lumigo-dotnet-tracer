using Xunit;
using System;
using System.Linq;
using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.S3Events;
using Amazon.Lambda.SNSEvents;
using Amazon.Lambda.SQSEvents;
using Amazon.Lambda.LexEvents;
using Amazon.Lambda.ConfigEvents;
using Amazon.Lambda.CognitoEvents;
using Amazon.Lambda.KinesisEvents;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.CloudWatchLogsEvents;
using Amazon.Lambda.KinesisFirehoseEvents;
using Amazon.Lambda.KinesisAnalyticsEvents;
using Amazon.Lambda.CloudWatchEvents.ScheduledEvents;
using Lumigo.DotNET.Utilities;
using Lumigo.DotNET.Utilities.Models;

namespace Lumigo.DotNET.Test.Utilities
{
    public class AwsUtilsTest : TestBase
    {
        [Fact]
        public void ExtractTriggeredByFromEvent_Should_Return_Null()
        {
            //Arrange
            object evnt = null;

            //Act
            var result = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.Null(result);
        }


        [Fact]
        public void ExtractTriggeredByFromEvent_Should_Return_Right_Values_For_DynamoDBEvent()
        {
            //Arrange
            var date = DateTime.SpecifyKind(new DateTime(2021, 11, 13), DateTimeKind.Utc);
            object evnt = new DynamoDBEvent
            {
                Records = new List<DynamoDBEvent.DynamodbStreamRecord>
                {
                    new DynamoDBEvent.DynamodbStreamRecord
                    {
                        EventSourceArn="arn:partition:service:region:account-id:resource-id",
                        Dynamodb= new StreamRecord
                        {
                            ApproximateCreationDateTime=date,
                            NewImage=new Dictionary<string, AttributeValue>
                            {
                                {"key", new AttributeValue("value") }
                            }
                        },
                        EventName="INSERT"
                    }
                }
            };

            //Act
            var result = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("dynamodb", result.TriggeredBy);
            Assert.Equal(1636761600000, result.ApproxEventCreationTime);
            Assert.Equal("arn:partition:service:region:account-id:resource-id", result.Arn);
            Assert.Equal("B83774F46ED996EB1AAA594CA71E360C", result.MessageIds.First());
        }


        [Fact]
        public void ExtractTriggeredByFromEvent_Should_Return_Right_Values_For_KinesisEvent()
        {
            //Arrange
            object evnt = new KinesisEvent
            {
                Records = new List<KinesisEvent.KinesisEventRecord>
                {
                    new KinesisEvent.KinesisEventRecord
                    {
                        EventSourceARN="arn:partition:service:region:account-id:resource-id",
                        Kinesis = new KinesisEvent.Record
                        {
                            SequenceNumber="messageId"
                        }
                    }
                }
            };

            //Act
            var result = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.IsType<TriggeredByModel>(result);
            Assert.NotNull(result);
            Assert.Equal("kinesis", result.TriggeredBy);
            Assert.Equal("arn:partition:service:region:account-id:resource-id", result.Arn);
            Assert.Equal("messageId", result.MessageIds.First());
        }


        [Fact]
        public void ExtractTriggeredByFromEvent_Should_Return_Right_Values_For_KinesisFirehoseEvent()
        {
            //Arrange
            object evnt = new KinesisFirehoseEvent
            {
                DeliveryStreamArn = "arn:partition:service:region:account-id:resource-id"
            };

            //Act
            var result = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("kinesis", result.TriggeredBy);
            Assert.Equal("arn:partition:service:region:account-id:resource-id", result.Arn);
        }

        [Fact]
        public void ExtractTriggeredByFromEvent_Should_Return_Right_Values_For_KinesisAnalyticsFirehoseInputPreprocessingEvent()
        {
            //Arrange
            object evnt = new KinesisAnalyticsFirehoseInputPreprocessingEvent
            {
                StreamArn = "arn:partition:service:region:account-id:resource-id"
            };

            //Act
            var result = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("kinesis", result.TriggeredBy);
            Assert.Equal("arn:partition:service:region:account-id:resource-id", result.Arn);
        }



        [Fact]
        public void ExtractTriggeredByFromEvent_Should_Return_Right_Values_For_KinesisAnalyticsStreamsInputPreprocessingEvent()
        {
            //Arrange
            object evnt = new KinesisAnalyticsStreamsInputPreprocessingEvent
            {
                StreamArn = "arn:partition:service:region:account-id:resource-id"
            };

            //Act
            var result = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("kinesis", result.TriggeredBy);
            Assert.Equal("arn:partition:service:region:account-id:resource-id", result.Arn);
        }

        [Fact]
        public void ExtractTriggeredByFromEvent_Should_Return_Right_Values_For_S3Event()
        {
            //Arrange
            object evnt = new S3Event
            {
                Records = new List<S3Event.S3EventNotificationRecord>
                {
                    new Amazon.S3.Util.S3EventNotification.S3EventNotificationRecord
                    {
                        S3 = new Amazon.S3.Util.S3EventNotification.S3Entity
                        {
                            Bucket = new Amazon.S3.Util.S3EventNotification.S3BucketEntity
                            {
                                Arn="arn:partition:service:region:account-id:resource-id"
                            }
                        }
                    }
                }
            };

            //Act
            var result = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("s3", result.TriggeredBy);
            Assert.Equal("arn:partition:service:region:account-id:resource-id", result.Arn);
        }

        [Fact]
        public void ExtractTriggeredByFromEvent_Should_Return_Right_Values_For_SNSEvent()
        {
            //Arrange
            object evnt = new SNSEvent
            {
                Records = new List<SNSEvent.SNSRecord>
                {
                    new SNSEvent.SNSRecord
                    {
                        Sns = new SNSEvent.SNSMessage
                        {
                            TopicArn ="arn:partition:service:region:account-id:resource-id",
                            MessageId="messageId"
                        }
                    }
                }
            };

            //Act
            var result = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("sns", result.TriggeredBy);
            Assert.Equal("arn:partition:service:region:account-id:resource-id", result.Arn);
            Assert.Equal("messageId", result.MessageId);
        }

        [Fact]
        public void ExtractTriggeredByFromEvent_Should_Return_Right_Values_For_SQSEvent()
        {
            //Arrange
            object evnt = new SQSEvent
            {
                Records = new List<SQSEvent.SQSMessage>
                {
                    new SQSEvent.SQSMessage
                    {
                        EventSourceArn="arn:partition:service:region:account-id:resource-id",
                        MessageId="messageId"
                    }
                }
            };

            //Act
            var result = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("sqs", result.TriggeredBy);
            Assert.Equal("arn:partition:service:region:account-id:resource-id", result.Arn);
            Assert.Equal("messageId", result.MessageId);
        }

        [Fact]
        public void ExtractTriggeredByFromEvent_Should_Return_Right_Values_For_APIGatewayProxyRequest()
        {
            //Arrange
            object evnt = new APIGatewayProxyRequest
            {
                HttpMethod = "POST",
                Resource = "/path",
                Headers = new Dictionary<string, string>
                {
                    {"Host","lumigo.com" }
                },
                RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
                {
                    Stage = "default"
                }
            };

            //Act
            var result = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("apigw", result.TriggeredBy);
            Assert.Equal("POST", result.HttpMethod);
            Assert.Equal("/path", result.Resource);
            Assert.Equal("lumigo.com", result.Api);
            Assert.Equal("default", result.Stage);
            Assert.Null(result.Arn);
        }

        [Fact]
        public void ExtractTriggeredByFromEvent_Should_Return_Right_Values_For_CloudWatchLogsEvent()
        {
            //Arrange
            object evnt = new CloudWatchLogsEvent();

            //Act
            var result = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("cloudwatch", result.TriggeredBy);
            Assert.Null(result.Arn);
        }

        [Fact]
        public void ExtractTriggeredByFromEvent_Should_Return_Right_Values_For_ScheduledEvent()
        {
            //Arrange
            object evnt = new ScheduledEvent
            {
                Resources = new List<string>
                {
                    "arn:aws:events:us-west-2:111111111111:rule/my-name"
                },
                Region = "us-west-2",
            };

            //Act
            var result = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("cloudwatch", result.TriggeredBy);
            Assert.Equal("arn:aws:events:us-west-2:111111111111:rule/my-name", result.Arn);
            Assert.Equal("us-west-2", result.Region);
            Assert.Equal("my-name", result.Resource);
        }

        [Fact]
        public void ExtractTriggeredByFromEvent_Should_Return_Right_Values_For_ConfigEvent()
        {
            //Arrange
            object evnt = new ConfigEvent();

            //Act
            var result = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("config", result.TriggeredBy);
            Assert.Null(result.Arn);
        }

        [Fact]
        public void ExtractTriggeredByFromEvent_Should_Return_Right_Values_For_LexEvent()
        {
            //Arrange
            object evnt = new LexEvent();

            //Act
            var result = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("lex", result.TriggeredBy);
            Assert.Null(result.Arn);
        }

        [Fact]
        public void ExtractTriggeredByFromEvent_Should_Return_Right_Values_For_CognitoEvent()
        {
            //Arrange
            object evnt = new CognitoEvent();

            //Act
            var result = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("cognito", result.TriggeredBy);
            Assert.Null(result.Arn);
        }



        [Fact]
        public void ExtractTriggeredByFromEvent_Should_Return_Right_Values_For_Object()
        {
            //Arrange
            object evnt = string.Empty;

            //Act
            var result = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(AwsUtils.TRIGGERED_BY_FALLBACK, result.TriggeredBy);
            Assert.Null(result.Arn);
        }

        [Fact]
        public void ExtractMessageIdFromDynamodbRecord_Should_Return_Null_For_DynamodbStreamRecord()
        {
            //Arrange
            var record = new DynamoDBEvent.DynamodbStreamRecord();

            //Act
            var result = AwsUtils.ExtractMessageIdFromDynamodbRecord(record);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public void ExtractMessageIdFromDynamodbRecord_Should_Return_Right_Insert_Event_For_DynamodbStreamRecord()
        {
            //Arrange
            var record = new DynamoDBEvent.DynamodbStreamRecord
            {
                EventName = "INSERT",
                Dynamodb = new StreamRecord
                {
                    NewImage = new Dictionary<string, AttributeValue>
                        {
                            {"key", new AttributeValue("value") }
                        }
                }
            };

            //Act
            var result = AwsUtils.ExtractMessageIdFromDynamodbRecord(record);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("B83774F46ED996EB1AAA594CA71E360C", result);
        }

        [InlineData("MODIFY")]
        [InlineData("REMOVE")]
        [Theory]
        public void ExtractMessageIdFromDynamodbRecord_Should_Return_Right_Modify_or_Remove_Event_For_DynamodbStreamRecord(string eventName)
        {
            //Arrange
            var record = new DynamoDBEvent.DynamodbStreamRecord
            {
                EventName = eventName,
                Dynamodb = new StreamRecord
                {
                    Keys = new Dictionary<string, AttributeValue>
                        {
                            {"key", new AttributeValue("value") }
                        }
                }
            };

            //Act
            var result = AwsUtils.ExtractMessageIdFromDynamodbRecord(record);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("B83774F46ED996EB1AAA594CA71E360C", result);
        }

        [InlineData(null)]
        [InlineData("")]
        [InlineData("arn:partition:service:region:account-id")]
        [Theory]
        public void ExtractAwsAccountFromArn_Should_Return_Null(string arn)
        {
            //Act
            var result = AwsUtils.ExtractAwsAccountFromArn(arn);

            //Assert
            Assert.Null(result);
        }


        [InlineData("arn:partition:service:region:account-id:resource-id")]
        [InlineData("arn:aws:lambda:us-west-2:account-id:function:tracer-test:my-alias")]
        [Theory]
        public void ExtractAwsAccountFromArn_Should_Return_AWS_Account_Id(string arn)
        {
            //Act
            var result = AwsUtils.ExtractAwsAccountFromArn(arn);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("account-id", result);
        }

        [InlineData(null)]
        [InlineData("")]
        [InlineData("RandomValue")]
        [Theory]
        public void ExtractAwsTraceRoot_Should_Return_Null(string amznTraceId)
        {
            //Act
            var result = AwsUtils.ExtractAwsTraceRoot(amznTraceId);

            //Assert
            Assert.Null(result);
        }

        [InlineData("Root=1-5759e988-bd862e3fe1be46a994272793;Sampled=1")]
        [InlineData("Root=1-5759e988-bd862e3fe1be46a994272793;Parent=53995c3f42cd8ad8;Sampled=1")]
        [Theory]
        public void ExtractAwsTraceRoot_Should_Return_Right_Root_Value(string amznTraceId)
        {
            //Act
            var result = AwsUtils.ExtractAwsTraceRoot(amznTraceId);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("1-5759e988-bd862e3fe1be46a994272793", result);
        }


        [InlineData(null)]
        [InlineData("")]
        [InlineData("RandomValue")]
        [InlineData("Root=5759e988-bd862e3fe1be46a994272793;Sampled=1")]
        [Theory]
        public void ExtractAwsTraceTransactionId_Should_Return_Null(string amznTraceId)
        {
            //Act
            var result = AwsUtils.ExtractAwsTraceTransactionId(amznTraceId);

            //Assert
            Assert.Null(result);
        }

        [InlineData("Root=1-5759e988-bd862e3fe1be46a994272793;Sampled=1")]
        [InlineData("Root=1-5759e988-bd862e3fe1be46a994272793;Parent=53995c3f42cd8ad8;Sampled=1")]
        [Theory]
        public void ExtractAwsTraceTransactionId_Should_Return_Right_Root_Value(string amznTraceId)
        {
            //Act
            var result = AwsUtils.ExtractAwsTraceTransactionId(amznTraceId);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("bd862e3fe1be46a994272793", result);
        }


        [InlineData(null)]
        [InlineData("")]
        [Theory]
        public void ExtractAwsTraceSuffix_Should_Return_Null(string amznTraceId)
        {
            //Act
            var result = AwsUtils.ExtractAwsTraceSuffix(amznTraceId);

            //Assert
            Assert.Null(result);
        }

        [InlineData("Root=1-5759e988-bd862e3fe1be46a994272793;Sampled=1", ";Sampled=1")]
        [InlineData("Root=1-5759e988-bd862e3fe1be46a994272793;Parent=53995c3f42cd8ad8;Sampled=1", ";Parent=53995c3f42cd8ad8;Sampled=1")]
        [Theory]
        public void ExtractAwsTraceSuffix_Should_Return_Right_Root_Value(string amznTraceId, string expectedResult)
        {
            //Act
            var result = AwsUtils.ExtractAwsTraceSuffix(amznTraceId);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void GetFunctionReadiness_Should_Return_Cold()
        {
            //Arrange
            Environment.SetEnvironmentVariable("LUMIGO_COLD_START_KEY", null);

            //Act
            var result = AwsUtils.GetFunctionReadiness();

            //Assert
            Assert.NotNull(result);
            Assert.Equal("COLD", result);
        }

        [Fact]
        public void GetFunctionReadiness_Should_Return_Warm()
        {
            //Arrange
            Environment.SetEnvironmentVariable("LUMIGO_COLD_START_KEY", "true");

            //Act
            var result = AwsUtils.GetFunctionReadiness();

            //Assert
            Assert.NotNull(result);
            Assert.Equal("WARM", result);
        }

        [Fact]
        public void Returns_With_Correct_Data_When_Event_Is_DynamoDBEvent()
        {
            //Arrange
            var evnt = new DynamoDBEvent
            {
                Records = new List<DynamoDBEvent.DynamodbStreamRecord>
                {
                    new DynamoDBEvent.DynamodbStreamRecord
                    {
                        EventSourceArn="arn"
                    }
                }
            };

            //Act
            var response = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.IsType<TriggeredByModel>(response);
            Assert.Equal("dynamodb", response.TriggeredBy);
            Assert.Equal("arn", response.Arn);
        }

        [Fact]
        public void Returns_With_Correct_Data_When_Event_Is_KinesisFirehoseEvent()
        {
            //Arrange
            var evnt = new KinesisFirehoseEvent
            {
                DeliveryStreamArn = "arn"
            };

            //Act
            var response = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.IsType<TriggeredByModel>(response);
            Assert.Equal("kinesis", response.TriggeredBy);
            Assert.Equal("arn", response.Arn);
        }

        [Fact]
        public void Returns_With_Correct_Data_When_Event_Is_KinesisAnalyticsFirehoseInputPreprocessingEvent()
        {
            //Arrange
            var evnt = new KinesisAnalyticsFirehoseInputPreprocessingEvent
            {
                StreamArn = "arn"
            };

            //Act
            var response = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.IsType<TriggeredByModel>(response);
            Assert.Equal("kinesis", response.TriggeredBy);
            Assert.Equal("arn", response.Arn);
        }

        [Fact]
        public void Returns_With_Correct_Data_When_Event_Is_KinesisAnalyticsStreamsInputPreprocessingEvent()
        {
            //Arrange
            var evnt = new KinesisAnalyticsStreamsInputPreprocessingEvent
            {
                StreamArn = "arn"
            };

            //Act
            var response = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.IsType<TriggeredByModel>(response);
            Assert.Equal("kinesis", response.TriggeredBy);
            Assert.Equal("arn", response.Arn);
        }

        [Fact]
        public void Returns_With_Correct_Data_When_Event_Is_S3Event()
        {
            //Arrange
            var evnt = new S3Event
            {
                Records = new List<Amazon.S3.Util.S3EventNotification.S3EventNotificationRecord>
                {
                    new Amazon.S3.Util.S3EventNotification.S3EventNotificationRecord
                    {
                        S3 = new Amazon.S3.Util.S3EventNotification.S3Entity
                        {
                            Bucket = new Amazon.S3.Util.S3EventNotification.S3BucketEntity
                            {
                                Arn="arn"
                            }
                        }
                    }
                }
            };

            //Act
            var response = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.IsType<TriggeredByModel>(response);
            Assert.Equal("s3", response.TriggeredBy);
            Assert.Equal("arn", response.Arn);
        }

        [Fact]
        public void Returns_With_Correct_Data_When_Event_Is_SNSEvent()
        {
            //Arrange
            var evnt = new SNSEvent
            {
                Records = new List<SNSEvent.SNSRecord>
                {
                    new SNSEvent.SNSRecord
                    {
                        Sns = new SNSEvent.SNSMessage
                        {
                            TopicArn="arn",
                            MessageId="125"
                        }
                    }
                }
            };

            //Act
            var response = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.IsType<TriggeredByModel>(response);
            Assert.Equal("sns", response.TriggeredBy);
            Assert.Equal("arn", response.Arn);
            Assert.Equal("125", response.MessageId);
        }

        [Fact]
        public void Returns_With_Correct_Data_When_Event_Is_SQSEvent()
        {
            //Arrange
            var evnt = new SQSEvent
            {
                Records = new List<SQSEvent.SQSMessage>
                {
                    new SQSEvent.SQSMessage
                    {
                        EventSourceArn="arn",
                        MessageId="123"
                    }
                }
            };

            //Act
            var response = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.IsType<TriggeredByModel>(response);
            Assert.Equal("sqs", response.TriggeredBy);
            Assert.Equal("arn", response.Arn);
            Assert.Equal("123", response.MessageId);
        }

        [Fact]
        public void Returns_With_Correct_Data_When_Event_Is_APIGatewayProxyRequest()
        {
            //Arrange
            var evnt = new APIGatewayProxyRequest
            {
                HttpMethod = "POST",
                Resource = "Resource",
                Headers = new Dictionary<string, string>
                {
                    {"Host","lumigo.com" }
                },
                RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
                {
                    Stage = "stage"
                }
            };

            //Act
            var response = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.IsType<TriggeredByModel>(response);
            Assert.Equal("apigw", response.TriggeredBy);
            Assert.Equal("POST", response.HttpMethod);
            Assert.Equal("Resource", response.Resource);
            Assert.Equal("lumigo.com", response.Api);
            Assert.Equal("stage", response.Stage);
        }

        [Fact]
        public void Returns_With_Correct_Data_When_Event_Is_CloudWatchLogsEvent()
        {
            //Arrange
            var evnt = new CloudWatchLogsEvent();

            //Act
            var response = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.IsType<TriggeredByModel>(response);
            Assert.Equal("cloudwatch", response.TriggeredBy);
        }

        [Fact]
        public void Returns_With_Correct_Data_When_Event_Is_ScheduledEvent()
        {
            //Arrange
            var evnt = new ScheduledEvent
            {
                Resources = new List<string>
                {
                    "arn"
                }
            };

            //Act
            var response = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.IsType<TriggeredByModel>(response);
            Assert.Equal("cloudwatch", response.TriggeredBy);
            Assert.Equal("arn", response.Arn);
        }


        [Fact]
        public void Returns_With_Correct_Data_When_Event_Is_ConfigEvent()
        {
            //Arrange
            var evnt = new ConfigEvent();

            //Act
            var response = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.IsType<TriggeredByModel>(response);
            Assert.Equal("config", response.TriggeredBy);
        }

        [Fact]
        public void Returns_With_Correct_Data_When_Event_Is_LexEvent()
        {
            //Arrange
            var evnt = new LexEvent();

            //Act
            var response = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.IsType<TriggeredByModel>(response);
            Assert.Equal("lex", response.TriggeredBy);
        }

        [Fact]
        public void Returns_With_Correct_Data_When_Event_Is_CognitoEvent()
        {
            //Arrange
            var evnt = new CognitoEvent();

            //Act
            var response = AwsUtils.ExtractTriggeredByFromEvent(evnt);

            //Assert
            Assert.IsType<TriggeredByModel>(response);
            Assert.Equal("cognito", response.TriggeredBy);
        }

        [Fact]
        public void Returns_With_Correct_Data_When_Event_Is_Another_Object()
        {
            //Act
            var response = AwsUtils.ExtractTriggeredByFromEvent(string.Empty);

            //Assert
            Assert.IsType<TriggeredByModel>(response);
            Assert.Equal("No recognized trigger", response.TriggeredBy);
        }
    }
}
