using Xunit;
using System;
using BetterPrivateObject;
using Amazon.Lambda.Core;
using Lumigo.DotNET.Instrumentation;
using Lumigo.DotNET.Utilities;
using Newtonsoft.Json;

namespace Lumigo.DotNET.Test
{
    class TestRequestHandler : LumigoRequestHandler
    {
        public TestRequestHandler()
        {
            LumigoBootstrap.Bootstrap();
        }

        public string FunctionHandler(string input, ILambdaContext context)
        {
            return Handle(input, context, () =>
                {
                    if (input == "{\"data\":\"bad\"") {
                        throw new ArgumentException("FAIL!");
                    } else {
                        return "Hello world!";
                    }
                }
            );
        }
    }

    public class LumigoRequestHandlerTest : TestBase
    {
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
        public void ExceptionPropagatingFromHandler_Should_Mark_Span_Erroneous()
        {
            var handler = new TestRequestHandler();
            try {
                handler.FunctionHandler("{\"data\":\"bad\"", new EmptyContext());

                throw new Exception("Should not have gotten here");
            } catch (ArgumentException) {
                dynamic po = new PrivateObject<LumigoRequestHandler>();
                SpansContainer spansContainer = po.spansContainer;

                // Assert.Equal(2, spansContainer.GetAllCollectedSpans().Count);

                var spans = po.spansContainer.GetAllCollectedSpans();
                Console.WriteLine(JsonConvert.SerializeObject(spans[0]));
                Console.WriteLine();
                Console.WriteLine(JsonConvert.SerializeObject(spans[1]));
            }
        }
    }
}