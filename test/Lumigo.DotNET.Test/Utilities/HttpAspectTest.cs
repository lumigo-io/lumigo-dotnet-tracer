using Xunit;
using System;
using Lumigo.DotNET.Utilities.Http;
using System.Net.Http;

namespace Lumigo.DotNET.Test.Utilities
{
    public class HttpAspectTest
    {
        [Fact]
        public void GetUriTest_Should_Return_Null()
        {
            //Arrange
            var args = new object[] { };
            var item = new HttpAspect();

            //Act
            var value = item.GetUri(args);

            //Assert
            Assert.Null(value);
        }


        [Fact]
        public void GetUriTest_Should_Return_Url_Of_Uri()
        {
            //Arrange
            var url = "https://www.google.com/";
            var uri = new Uri(url);
            var args = new object[] { uri };
            var item = new HttpAspect();

            //Act
            var value = item.GetUri(args);

            //Assert
            Assert.Equal(url, value);
        }


        [Fact]
        public void GetHttpContent_Should_Return_Null()
        {
            //Arrange
            var args = new object[] { };
            var item = new HttpAspect();

            //Act
            var value = item.GetHttpContent(args);

            //Assert
            Assert.Null(value);
        }


        [Fact]
        public void GetHttpContent_Should_Return_Url_OfUri()
        {
            //Arrange
            var httpContent = new StringContent("");
            var args = new object[] { httpContent };
            var item = new HttpAspect();

            //Act
            var value = item.GetHttpContent(args);

            //Assert
            Assert.Equal(httpContent, value);
        }


        [Fact]
        public void GetMethod_Should_Return_Null()
        {
            //Arrange
            var name = "SendAsync";
            var args = new object[] { };
            var item = new HttpAspect();

            //Act
            var value = item.GetMethod(name, args);

            //Assert
            Assert.Null(value);
        }


        [Fact]
        public void GetMethod_Should_Return_Url_OfUri()
        {
            //Arrange
            var name = "SendAsync";
            var request = new HttpRequestMessage();
            var args = new object[] { request };
            var item = new HttpAspect();

            //Act
            var value = item.GetMethod(name, args);

            //Assert
            Assert.Equal("GET", value);
        }
    }
}
