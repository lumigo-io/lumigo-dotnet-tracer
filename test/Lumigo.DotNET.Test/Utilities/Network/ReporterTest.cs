using Xunit;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using RichardSzalay.MockHttp;
using Lumigo.DotNET.Utilities.Network;

namespace Lumigo.DotNET.Test.Utilities
{
    public class ReporterTest : TestBase
    {
        [Fact]
        public async Task ReportSpansTest()
        {
            //Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect(HttpMethod.Post, "https://eu-central-1.lumigo-tracer-edge.golumigo.com/api/spans")
                .Respond(HttpStatusCode.OK);
            var client = mockHttp.ToHttpClient();
            var reporter = new Reporter(client);
            var spans = new List<string> { "1", "2" };

            //Act
            await reporter.ReportSpans(spans, 3);

            //Assert
            mockHttp.VerifyNoOutstandingRequest();
        }

    }
}
