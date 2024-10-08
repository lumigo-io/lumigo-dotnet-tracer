﻿using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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

        [Fact]
        public async void End_Should_Handle_Circular_References()
        {
            // Arrange
            var spansContainer = SpansContainer.GetInstance();
            spansContainer.Init(new Reporter(), new EmptyContext(), new EmptyEvent());

            var circularObject1 = new CircularReferenceClass();
            var circularObject2 = new CircularReferenceClass();
            circularObject1.Reference = circularObject2;
            circularObject2.Reference = circularObject1;

            // Act
            spansContainer.End(circularObject1).GetAwaiter().GetResult();

            // Assert
            // Check that serialization completed without errors and does not contain circular reference errors
            Assert.NotNull(spansContainer.GetEndSpan().ReturnValue);
            Assert.DoesNotContain("Self referencing loop", spansContainer.GetEndSpan().ReturnValue);
            Assert.Contains("Reference", spansContainer.GetEndSpan().ReturnValue);
        }

        class CircularReferenceClass
        {
            public CircularReferenceClass Reference { get; set; }
        }

        [Fact]
        public async void End_Should_Handle_NonSerializable_Properties()
        {
            // Arrange
            var spansContainer = SpansContainer.GetInstance();
            spansContainer.Init(new Reporter(), new EmptyContext(), new EmptyEvent());

            // Object with non-serializable properties
            var objectWithNonSerializableProperties = new ObjectWithIPAddress
            {
                Name = "TestObject",
                IPAddress = System.Net.IPAddress.Parse("fe80::1%1") // IPv6 address with ScopeId
            };

            // Act
            // Call End on spansContainer to test the dynamic ignore functionality
            await spansContainer.End(objectWithNonSerializableProperties);

            // Assert
            // Check that the EndSpan contains the serialized result and non-serializable properties were ignored
            var serializedResult = spansContainer.GetEndSpan().ReturnValue;

            Assert.NotNull(serializedResult);
            Assert.Contains("TestObject", serializedResult);
            Assert.Contains("IPAddress", serializedResult); // ScopeId should cause serialization to fail
        }

        // The class with non-serializable properties for testing purposes
        public class ObjectWithIPAddress
        {
            public string Name { get; set; }
            public System.Net.IPAddress IPAddress { get; set; }
        }
    }
}
