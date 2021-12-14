using Xunit;
using System;
using System.Collections.Generic;
using Lumigo.DotNET.Utilities;

namespace Lumigo.DotNET.Test.Utilities
{
    public class EnvUtilTest : TestBase
    {
        readonly EnvUtil envUtil = new EnvUtil();


        [Fact]
        public void GetEnv_Should_Return_Correct_Value()
        {
            //Arrange
            Environment.SetEnvironmentVariable("GetEnv_Key", "GetEnv_Value");

            //Act
            var value = envUtil.GetEnv("GetEnv_Key");

            //Assert
            Assert.Equal("GetEnv_Value", value);
        }


        [Fact]
        public void GetAll_Should_Return_Correct_Value()
        {
            //Arrange
            Environment.SetEnvironmentVariable("GetAll1_Key", "GetAll1_Value");
            Environment.SetEnvironmentVariable("GetAll2_Key", "GetAll2_Value");
            Environment.SetEnvironmentVariable("GetAll3_Key", "GetAll3_Value");

            //Act
            var value = envUtil.GetAll();

            //Assert
            Assert.Contains("GetAll1_Key", value.Keys);
            Assert.Contains("GetAll2_Key", value.Keys);
            Assert.Contains("GetAll3_Key", value.Keys);
            Assert.Equal("GetAll1_Value", value.GetValueOrDefault("GetAll1_Key"));
            Assert.Equal("GetAll2_Value", value.GetValueOrDefault("GetAll2_Key"));
            Assert.Equal("GetAll3_Value", value.GetValueOrDefault("GetAll3_Key"));
        }

        [Fact]
        public void GetBooleanEnv_Should_Return_Correct_Value()
        {
            //Arrange
            Environment.SetEnvironmentVariable("GetBooleanEnv_Key", "true");

            //Act
            var value = envUtil.GetBooleanEnv("GetBooleanEnv_Key", false);

            //Assert
            Assert.True(value);
        }

        [Fact]
        public void GetBooleanEnv_Should_Return_Default_Value()
        {
            //Arrange
            Environment.SetEnvironmentVariable("GetBooleanEnv_Key", "truefalse");

            //Act
            var value = envUtil.GetBooleanEnv("GetBooleanEnv_Key", false);

            //Assert
            Assert.False(value);
        }

        [Fact]
        public void GetIntegerEnv_Should_Return_Correct_Value()
        {
            //Arrange
            Environment.SetEnvironmentVariable("GetIntegerEnv_Key", "1");

            //Act
            var value = envUtil.GetIntegerEnv("GetIntegerEnv_Key", 2);

            //Assert
            Assert.Equal(1, value);
        }

        [Fact]
        public void GetIntegerEnv_Should_Return_Default_Value()
        {
            //Arrange
            Environment.SetEnvironmentVariable("GetIntegerEnv_Key", "-");

            //Act
            var value = envUtil.GetIntegerEnv("GetIntegerEnv_Key", 2);

            //Assert
            Assert.Equal(2, value);
        }

        [Fact]
        public void GetStringArrayEnv_Should_Return_Correct_Value()
        {
            //Arrange
            Environment.SetEnvironmentVariable("GetStringArrayEnv_Key", "1,2,3");

            //Act
            var value = envUtil.GetStringArrayEnv("GetStringArrayEnv_Key", new string[] { "1", "2" });

            //Assert
            Assert.Equal(new string[] { "1", "2", "3" }, value);
        }

        [Fact]
        public void GetStringArrayEnv_Should_Return_Default_Value()
        {
            //Arrange
            Environment.SetEnvironmentVariable("GetStringArrayEnv_Key", null);

            //Act
            var value = envUtil.GetStringArrayEnv("GetStringArrayEnv_Key", new string[] { "1", "2" });

            //Assert
            Assert.Equal(new string[] { "1", "2" }, value);
        }
    }
}