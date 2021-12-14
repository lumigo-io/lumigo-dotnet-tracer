using System;
using System.Net.Http;
using Amazon.Lambda.Core;
using Lumigo.DotNET.Utilities;
using Lumigo.DotNET.Utilities.Extensions;
using Lumigo.DotNET.Utilities.Network;
using Xunit;

namespace Lumigo.DotNET.Test.Utilities
{
    public class SpansContainerTest : TestBase
    {
        class EmptyEvent { }
        class EmptyContext : ILambdaContext
        {
            public string AwsRequestId => "111";

            public IClientContext ClientContext => throw new NotImplementedException();

            public string FunctionName => "my-func";

            public string FunctionVersion => throw new NotImplementedException();

            public ICognitoIdentity Identity => throw new NotImplementedException();

            public string InvokedFunctionArn => "my-invoked-function-arn";

            public ILambdaLogger Logger => throw new NotImplementedException();

            public string LogGroupName => "my-log-group-name";

            public string LogStreamName => "my-log-stream-name";

            public int MemoryLimitInMB => 1024;

            public TimeSpan RemainingTime => new TimeSpan();
        }

        [Fact]
        public void AddHttpSpan_Http_Call_With_Big_Body_Should_Reduce_Size()
        {
            //Arrange
            var spansContainer = SpansContainer.GetInstance();
            spansContainer.Init(new Reporter(), new EmptyContext(), new EmptyEvent());
            var maxSpanFieldSize = Configuration.GetInstance().MaxSpanFieldSize();

            long startTime = DateTime.UtcNow.ToMilliseconds();
            var body = new String('a', maxSpanFieldSize + 1000);
            var requestContent = new StringContent(body);
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
            response.Content = new StringContent(body);

            //Act
            spansContainer.AddHttpSpan(startTime, "http://not-exist", "Get", requestContent, response);

            //Assert
            Assert.Equal(maxSpanFieldSize, spansContainer.GetHttpSpans()[0].Info.HttpInfo.Request.Body.Length);
            Assert.Equal(maxSpanFieldSize, spansContainer.GetHttpSpans()[0].Info.HttpInfo.Response.Body.Length);
        }

        [Fact]
        public void AddHttpSpan_Many_Http_Calls_Should_Limit_Amount_Saved()
        {
            //Arrange
            var spansContainer = SpansContainer.GetInstance();
            spansContainer.Init(new Reporter(), new EmptyContext(), new EmptyEvent());
            int numberOfHttpCalls = 1000;

            long startTime = DateTime.UtcNow.ToMilliseconds();
            var content = new StringContent("a");
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
            response.Content = new StringContent("a");

            //Act
            for (int i = 0; i < numberOfHttpCalls; i++)
            {
                spansContainer.AddHttpSpan(startTime, "http://not-exist", "Get", content, response);
            }

            //Assert
            Assert.Equal(901, spansContainer.GetHttpSpans().Count);
        }
    }
}
