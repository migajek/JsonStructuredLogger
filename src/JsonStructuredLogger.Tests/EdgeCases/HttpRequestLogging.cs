using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Xunit;

namespace JsonStructuredLogger.Tests.EdgeCases
{
    public class HttpRequestLogging
    {
        [Fact]
        public void Serialization_Error_Wont_Stop_Logger()
        { 
            // arrange
            var fixture = new LoggerFixture();
            var logger = fixture.CreateLogger<JsonStructuredLoggerTests>();

            // act
            logger.LogInformation("Processing {Request}", new DefaultHttpRequest(new DefaultHttpContext()));

            // assert
            Assert.Single(fixture.LogEntries);

        }
    }
}
