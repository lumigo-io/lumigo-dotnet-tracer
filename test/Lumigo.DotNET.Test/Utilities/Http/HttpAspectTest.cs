using Moq;
using Xunit;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Lumigo.DotNET.Utilities.Http;

namespace Lumigo.DotNET.Test.Utilities.Http
{
    public class HttpAspectTest : TestBase
    {
        [Fact]
        public void AddScope_Should_Run_Func()
        {
            //Arrange
            var aspect = new HttpAspect();
            var args = new object[]
            {
                new Uri("http://www.google.com"),
                new StringContent("value"),
                new HttpRequestMessage(HttpMethod.Get, "http://www.google.com")
            };
            var func = new Mock<Func<object[], HttpResponseMessage>>();
            func.Setup(x => x.Invoke(It.IsAny<object[]>())).Returns(new HttpResponseMessage(System.Net.HttpStatusCode.NotFound));

            //Act
            var value = aspect.AddScope(args, "PostAsync", typeof(HttpResponseMessage), func.Object);

            //Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, ((HttpResponseMessage)value).StatusCode);
        }

        [Fact]
        public void GetUri_Should_Return_Correct_Value_From_Uri_Type()
        {
            //Arrange
            var aspect = new HttpAspect();
            var args = new object[] { new Uri("http://www.google.com") };

            //Act
            var value = aspect.GetUri(args);

            //Assert
            Assert.Equal("http://www.google.com/", value);
        }

        [Fact]
        public void GetUri_Should_Return_Correct_Value_From_String_Type()
        {
            //Arrange
            var aspect = new HttpAspect();
            var args = new object[] { "http://www.google.com" };

            //Act
            var value = aspect.GetUri(args);

            //Assert
            Assert.Equal("http://www.google.com", value);
        }

        [Fact]
        public void GetUri_Should_Return_Null()
        {
            //Arrange
            var aspect = new HttpAspect();
            var args = new object[] { 1 };

            //Act
            var value = aspect.GetUri(args);

            //Assert
            Assert.Null(value);
        }

        [Fact]
        public async Task GetHttpContent_Should_Return_Correct_Value()
        {
            //Arrange
            var aspect = new HttpAspect();
            var args = new object[] { new StringContent("value") };

            //Act
            var value = aspect.GetHttpContent(args);

            //Assert
            Assert.IsType<StringContent>(value);
            Assert.Equal("value", await value.ReadAsStringAsync());
        }

        [Fact]
        public void GetHttpContent_Should_Return_Null()
        {
            //Arrange
            var aspect = new HttpAspect();
            var args = new object[] { 1 };

            //Act
            var value = aspect.GetHttpContent(args);

            //Assert
            Assert.Null(value);
        }

        [Fact]
        public void GetMethod_Should_Return_Correct_Value()
        {
            //Arrange
            var aspect = new HttpAspect();
            var args = new object[] { new HttpRequestMessage(HttpMethod.Get, "http://www.google.com") };

            //Act
            var value = aspect.GetMethod("Send", args);

            //Assert
            Assert.Equal("GET", value);
        }

        [Fact]
        public void GetMethod_Should_Return_Null()
        {
            //Arrange
            var aspect = new HttpAspect();
            var args = new object[] { 1 };

            //Act
            var value = aspect.GetMethod("Send", args);

            //Assert
            Assert.Null(value);
        }

        [Fact]
        public void GetMethod_Should_Return_As_Async_Is_Removed()
        {
            //Arrange
            var aspect = new HttpAspect();
            var args = new object[] { 1 };

            //Act
            var value = aspect.GetMethod("GetAsync", args);

            //Assert
            Assert.Equal("Get", value);
        }


    }
}
