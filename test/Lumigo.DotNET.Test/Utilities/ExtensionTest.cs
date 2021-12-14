using Xunit;
using System.Net.Http;
using System.Threading.Tasks;
using Lumigo.DotNET.Utilities.Extensions;
using System;
using Lumigo.DotNET.Utilities.Http;
using System.IO;

namespace Lumigo.DotNET.Test.Utilities
{
    public class ExtensionTest
    {
        [Fact]
        public void DateTimeToMilliseconds_ToMilliseconds_Should_Return_Correct_Value()
        {
            //Arrange
            var datetime = DateTime.SpecifyKind(new DateTime(2021, 10, 21), DateTimeKind.Utc);

            //Act
            var milsec = datetime.ToMilliseconds();

            //Assert
            Assert.Equal(1634774400000, milsec);
        }

        [Fact]
        public void LumigoTracedHttpClient_Should_Return_Correct_Type()
        {
            //Act
            var httpClient = new HttpClient().UseLumigo();

            //Assert
            Assert.IsType<LumigoTracedHttpClient>(httpClient);
        }
    }
}
