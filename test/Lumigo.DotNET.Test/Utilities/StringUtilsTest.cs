using Xunit;
using System.IO;
using System.Collections.Generic;
using Lumigo.DotNET.Utilities;
using Amazon.DynamoDBv2.Model;

namespace Lumigo.DotNET.Test.Utilities
{
    public class StringUtilsTest : TestBase
    {
        [Fact]
        public void GetMaxSizeString_Should_Return_Same_Size_String()
        {
            //Arrange
            var str = "new string";

            //Act
            var value = StringUtils.GetMaxSizeString(str, str.Length + 1);

            //Assert
            Assert.Equal(str.Length, value.Length);
        }

        [Fact]
        public void GetMaxSizeString_Should_Return_Smaller_Size_String()
        {
            //Arrange
            var str = "new string";

            //Act
            var value = StringUtils.GetMaxSizeString(str, str.Length - 1);

            //Assert
            Assert.Equal(str.Length - 1, value.Length);
        }

        [Fact]
        public void RandomStringAndNumbers_Should_Return_Defined_Size_String()
        {
            //Act
            var value = StringUtils.RandomStringAndNumbers(5);

            //Assert
            Assert.Equal(5, value.Length);
        }

        [Fact]
        public void BuildMd5Hash_Should_Return_Valid_Hash()
        {
            //Arrange
            var str = "new string";

            //Act
            var value = StringUtils.BuildMd5Hash(str);

            //Assert
            Assert.Equal("B200A3ADBE85FE848B920DC35D5A69B2", value);
        }

        [Fact]
        public void DynamoDBItemToHash_Should_Return_Valid_Hash()
        {
            //Arrange
            var item = new Dictionary<string, AttributeValue> { { "key", new AttributeValue("value") } };

            //Act
            var value = StringUtils.DynamoDBItemToHash(item);

            //Assert
            Assert.Equal("B83774F46ED996EB1AAA594CA71E360C", value);
        }

        [Fact]
        public void GetBase64Size_Should_Return_Correct_Size()
        {
            //Arrange
            var str = "new string";

            //Act
            var value = StringUtils.GetBase64Size(str);

            //Assert
            Assert.Equal(16, value);
        }

        [Fact]
        public void StreamToString_Should_Return_Correct_String()
        {
            //Arrange
            var str = "new string";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;

            //Act
            var value = StringUtils.StreamToString(stream);

            //Assert
            Assert.Equal(str, value);
        }
    }
}
