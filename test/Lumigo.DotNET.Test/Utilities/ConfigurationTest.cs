using Lumigo.DotNET.Utilities;
using System;
using Xunit;

namespace Lumigo.DotNET.Test.Utilities
{
    public class ConfigurationTest : TestBase
    {
        [Fact]
        public void GetInstance_Should_Not_Be_Null()
        {
            Assert.NotNull(Configuration.GetInstance());
        }

        [Fact]
        public void MaxSpanFieldSize_Should_Return_Correct_Value()
        {
            //Arrange
            Environment.SetEnvironmentVariable(Configuration.LUMIGO_MAX_ENTRY_SIZE, "2048");

            //Act
            var value = Configuration.GetInstance().MaxSpanFieldSize();

            //Assert
            Assert.Equal(2048, value);
        }


        [Fact]
        public void MaxSpanFieldSize_Should_Return_Default_Value()
        {
            //Arrange
            Environment.SetEnvironmentVariable(Configuration.LUMIGO_MAX_ENTRY_SIZE, null);

            //Act
            var value = Configuration.GetInstance().MaxSpanFieldSize();

            //Assert
            Assert.Equal(1024, value);
        }


        [Fact]
        public void MaxRequestSize_Should_Return_Correct_Value()
        {
            //Arrange
            Environment.SetEnvironmentVariable(Configuration.LUMIGO_MAX_RESPONSE_SIZE, "2048");

            //Act
            var value = Configuration.GetInstance().MaxRequestSize();

            //Assert
            Assert.Equal(2048, value);
        }


        [Fact]
        public void MaxRequestSize_Should_Return_Default_Value()
        {
            //Arrange
            Environment.SetEnvironmentVariable(Configuration.LUMIGO_MAX_RESPONSE_SIZE, null);

            //Act
            var value = Configuration.GetInstance().MaxRequestSize();

            //Assert
            Assert.Equal(1024 * 900, value);
        }


        [Fact]
        public void GetLumigoToken_Should_Return_Correct_Value()
        {
            //Arrange
            var token = Guid.NewGuid().ToString();
            Environment.SetEnvironmentVariable(Configuration.TOKEN_KEY, token);

            //Act
            var value = Configuration.GetInstance().GetLumigoToken();

            //Assert
            Assert.Equal(token, value);
        }

        [Fact]
        public void GetLumigoTracerVersion_Should_Return_Correct_Value()
        {
            //Act
            var value = Configuration.GetInstance().GetLumigoTracerVersion();
            var splitedValue = value.Split('.');

            //Assert
            Assert.Equal(3, splitedValue.Length);
            Assert.True(int.TryParse(splitedValue[0], out int p0));
            Assert.True(p0 > -1);
            Assert.True(int.TryParse(splitedValue[1], out int p1));
            Assert.True(p1 > -1);
            Assert.True(int.TryParse(splitedValue[2], out int p2));
            Assert.True(p2 > -1);
        }


        [Fact]
        public void DebugMode_Should_Return_Correct_Value()
        {
            //Arrange
            Environment.SetEnvironmentVariable(Configuration.DEBUG_KEY, "true");

            //Act
            var value = Configuration.GetInstance().DebugMode;

            //Assert
            Assert.True(value);
        }


        [Fact]
        public void DebugMode_Should_Return_Default_Value()
        {
            //Arrange
            Environment.SetEnvironmentVariable(Configuration.DEBUG_KEY, null);

            //Act
            var value = Configuration.GetInstance().DebugMode;

            //Assert
            Assert.False(value);
        }


        [Fact]
        public void IsWriteToFile_Should_Return_Correct_Value()
        {
            //Arrange
            Environment.SetEnvironmentVariable(Configuration.LUMIGO_USE_TRACER_EXTENSION, "true");

            //Act
            var value = Configuration.GetInstance().IsWriteToFile();

            //Assert
            Assert.True(value);
        }


        [Fact]
        public void IsWriteToFile_Should_Return_Default_Value()
        {
            //Arrange
            Environment.SetEnvironmentVariable(Configuration.LUMIGO_USE_TRACER_EXTENSION, null);

            //Act
            var value = Configuration.GetInstance().IsWriteToFile();

            //Assert
            Assert.False(value);
        }

        [Fact]
        public void IsLumigoVerboseMode_Should_Return_Correct_Value()
        {
            //Arrange
            Environment.SetEnvironmentVariable(Configuration.LUMIGO_VERBOSE, "false");

            //Act
            var value = Configuration.GetInstance().IsLumigoVerboseMode();

            //Assert
            Assert.False(value);
        }


        [Fact]
        public void IsLumigoVerboseMode_Should_Return_Default_Value()
        {
            //Arrange
            Environment.SetEnvironmentVariable(Configuration.LUMIGO_VERBOSE, null);

            //Act
            var value = Configuration.GetInstance().IsLumigoVerboseMode();

            //Assert
            Assert.True(value);
        }


        [Fact]
        public void GetLumigoTimeout_Should_Return_Correct_Value()
        {
            //Arrange
            Environment.SetEnvironmentVariable(Configuration.REPORTER_TIMEOUT, "500");

            //Act
            var value = Configuration.GetInstance().GetLumigoTimeout();

            //Assert
            Assert.Equal(TimeSpan.FromMilliseconds(500), value);
        }


        [Fact]
        public void GetLumigoTimeout_Should_Return_Default_Value()
        {
            //Arrange
            Environment.SetEnvironmentVariable(Configuration.REPORTER_TIMEOUT, null);

            //Act
            var value = Configuration.GetInstance().GetLumigoTimeout();

            //Assert
            Assert.Equal(TimeSpan.FromMilliseconds(700), value);
        }


        [Fact]
        public void GetLumigoTimeout_Should_Return_Default_Value_When_Not_Number()
        {
            //Arrange
            Environment.SetEnvironmentVariable(Configuration.REPORTER_TIMEOUT, "aaa");

            //Act
            var value = Configuration.GetInstance().GetLumigoTimeout();

            //Assert
            Assert.Equal(TimeSpan.FromMilliseconds(700), value);
        }



        [Fact]
        public void IsAwsEnvironment_Should_Return_True()
        {
            //Arrange
            Environment.SetEnvironmentVariable("LAMBDA_RUNTIME_DIR", "X");

            //Act
            var value = Configuration.GetInstance().IsAwsEnvironment();

            //Assert
            Assert.True(value);
        }


        [Fact]
        public void IsAwsEnvironment_Should_Return_False()
        {
            //Arrange
            Environment.SetEnvironmentVariable("LAMBDA_RUNTIME_DIR", null);

            //Act
            var value = Configuration.GetInstance().IsAwsEnvironment();

            //Assert
            Assert.False(value);
        }


        [Fact]
        public void GetLumigoEdge_Should_Return_Tracer_Host()
        {
            //Arrange
            Environment.SetEnvironmentVariable(Configuration.TRACER_HOST_KEY, "TRACER_HOST_KEY_VALUE");

            //Act
            var value = Configuration.GetInstance().GetLumigoEdge();

            //Assert
            Assert.Equal("https://TRACER_HOST_KEY_VALUE/api/spans", value);
        }


        [Fact]
        public void GetLumigoEdge_Should_Return_Default()
        {
            //Arrange
            Environment.SetEnvironmentVariable(Configuration.TRACER_HOST_KEY, null);
            Environment.SetEnvironmentVariable(Configuration.REGION_KEY, "eu-central-1");

            //Act
            var value = Configuration.GetInstance().GetLumigoEdge();

            //Assert
            Assert.Equal("https://eu-central-1.lumigo-tracer-edge.golumigo.com/api/spans", value);
        }



        [Fact]
        public void MaxSpanFieldSizeWhenError_Should_Return_Correct_Value()
        {
            //Arrange
            Environment.SetEnvironmentVariable(Configuration.LUMIGO_MAX_ENTRY_SIZE, "2048");

            //Act
            var value = Configuration.GetInstance().MaxSpanFieldSizeWhenError();

            //Assert
            Assert.Equal(2048 * 10, value);
        }


        [Fact]
        public void MaxSpanFieldSizeWhenError_Should_Return_Default_Value()
        {
            //Arrange
            Environment.SetEnvironmentVariable(Configuration.LUMIGO_MAX_ENTRY_SIZE, null);

            //Act
            var value = Configuration.GetInstance().MaxSpanFieldSizeWhenError();

            //Assert
            Assert.Equal(1024 * 10, value);
        }

        [Fact]
        public void IsKillingSwitchActivated_Should_Return_Correct_Value()
        {
            //Arrange
            Environment.SetEnvironmentVariable(Configuration.LUMIGO_KILL_SWITCH, "true");

            //Act
            var value = Configuration.IsKillingSwitchActivated();

            //Assert
            Assert.True(value);
        }


        [Fact]
        public void IsKillingSwitchActivated_Should_Return_Default_Value()
        {
            //Arrange
            Environment.SetEnvironmentVariable(Configuration.LUMIGO_KILL_SWITCH, null);

            //Act
            var value = Configuration.IsKillingSwitchActivated();

            //Assert
            Assert.False(value);
        }
    }
}