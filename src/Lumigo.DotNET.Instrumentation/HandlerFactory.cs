using System;
using System.Collections.Generic;
using Lumigo.DotNET.Instrumentation.Handlers;
using Lumigo.DotNET.Instrumentation.Handlers.S3;
using Lumigo.DotNET.Instrumentation.Handlers.SNS;
using Lumigo.DotNET.Instrumentation.Handlers.SQS;
using Lumigo.DotNET.Instrumentation.Handlers.Empty;
using Lumigo.DotNET.Instrumentation.Handlers.Lambda;
using Lumigo.DotNET.Instrumentation.Handlers.Cognito;
using Lumigo.DotNET.Instrumentation.Handlers.DynamoDB;
using Lumigo.DotNET.Instrumentation.Handlers.Cloudwatch;
using Lumigo.DotNET.Instrumentation.Handlers.SimpleSystemsManagement;

namespace Lumigo.DotNET.Instrumentation
{
    public class HandlerFactory : BaseFactory<Type, IServiceHandler>
    {
        public HandlerFactory(string serviceName) : base(new EmptyHandler(serviceName = "empty")) { }

        protected override Dictionary<Type, Func<IServiceHandler>> Operations => new Dictionary<Type, Func<IServiceHandler>>
        {
            { typeof(Amazon.S3.AmazonS3Client), () => new SimpleServiceHandler<S3OperationsFactory>() },
            { typeof(Amazon.CognitoIdentityProvider.AmazonCognitoIdentityProviderClient), () => new CognitoServiceHandler() },
            { typeof(Amazon.Extensions.CognitoAuthentication.CognitoUser), () => new CognitoServiceHandler() },
            { typeof(Amazon.Lambda.AmazonLambdaClient), () => new LambdaServiceHandler() },
            { typeof(Amazon.SimpleNotificationService.AmazonSimpleNotificationServiceClient), () => new SNSServiceHandler() },
            { typeof(Amazon.SQS.AmazonSQSClient), () => new SQSServiceHandler() },
            { typeof(Amazon.SimpleSystemsManagement.AmazonSimpleSystemsManagementClient), () => new SimpleSystemsManagementServiceHandler() },
            { typeof(Amazon.CloudWatch.AmazonCloudWatchClient), () => new CloudwatchServiceHandler() },
            { typeof(Amazon.CloudWatchEvents.AmazonCloudWatchEventsClient), () => new CloudwatchEventsServiceHandler() },
            { typeof(Amazon.CloudWatchLogs.AmazonCloudWatchLogsClient), () => new CloudwatchLogsServiceHandler() },
            { typeof(Amazon.DynamoDBv2.AmazonDynamoDBClient), () => new DynamoDBServiceHandler() },
        };
    }
}
