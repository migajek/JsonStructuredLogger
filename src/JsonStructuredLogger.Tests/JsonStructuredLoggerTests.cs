using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Xunit;

namespace JsonStructuredLogger.Tests
{
    public class JsonStructuredLoggerTests
    {
        [Fact]
        public void Contains_Simple_Properties_Passed_To_Message()
        {
            // arrange
            var fixture = new LoggerFixture();
            var logger = fixture.CreateLogger<JsonStructuredLoggerTests>();

            // act
            logger.LogInformation("Hello {Name}", "Alex");

            // assert
            Assert.Single(fixture.LogEntries);
            Assert.Equal("Hello Alex", fixture.LogEntries[0].Message);
            Assert.Equal("Alex", fixture.LogEntries[0].Properties["Name"]);
        }

        [Fact]
        public void Contains_Complex_Properties_Passed_To_Message()
        {
            // arrange
            var fixture = new LoggerFixture();
            var logger = fixture.CreateLogger<JsonStructuredLoggerTests>();

            // act
            logger.LogInformation("Hello {@Person}", new { Name = "Alex", Age = 30 });

            // assert
            Assert.Single(fixture.LogEntries);
            var logEntry = fixture.LogEntries.Single();
            Assert.Equal("Hello { Name = Alex, Age = 30 }", logEntry.Message);
            Assert.Contains("@Person", logEntry.Properties.Keys);
            dynamic person = logEntry.Properties["@Person"];
            Assert.Equal("Alex", person.Name);
            Assert.Equal(30, person.Age);
        }

        [Fact]
        public void Contains_Scope_Properties()
        {
            // arrange
            var fixture = new LoggerFixture();
            var logger = fixture.CreateLogger<JsonStructuredLoggerTests>();

            // act
            using var scope = logger.BeginScope("Context {CorrelationId}", "0xdeadbeef");
            logger.LogInformation("Hello {Name}", "Alex");

            // assert
            Assert.Single(fixture.LogEntries);
            var logEntry = fixture.LogEntries.Single();
            Assert.Equal("0xdeadbeef", logEntry.Properties["CorrelationId"]);
        }

        [Fact]
        public void Contains_Multiple_Scope_Properties()
        {
            // arrange
            var fixture = new LoggerFixture();
            var logger = fixture.CreateLogger<JsonStructuredLoggerTests>();

            // act
            using var scope = logger.BeginScope("Context {CorrelationId}", "0xdeadbeef");
            using var secondScope = logger.BeginScope(new { AuthInfo = "Token" });
            logger.LogInformation("Hello {Name}", "Alex");

            // assert
            Assert.Single(fixture.LogEntries);
            var logEntry = fixture.LogEntries.Single();
        
            Assert.Equal("0xdeadbeef", logEntry.Properties["CorrelationId"]);
            Assert.Equal("Token", logEntry.Properties["AuthInfo"]);
        }

        [Fact]
        public void Can_Handle_Complex_Scope_Object()
        {
            // arrange
            var fixture = new LoggerFixture();
            var logger = fixture.CreateLogger<JsonStructuredLoggerTests>();

            // act
            var scopeInfo = new
            {
                Int32 = 27,
                String = "Str",
                Dictionary = new Dictionary<string, object>()
                {
                    ["request"] = new
                    {
                        RequestId = Guid.NewGuid(),
                        UserId = 99
                    }
                }
            };
            using var scope = logger.BeginScope(scopeInfo);
            logger.LogInformation("foo");
            
            // assert
            Assert.Single(fixture.LogEntries);
            var entry = fixture.LogEntries.Single();

            Assert.Contains("Dictionary", entry.Properties);
        }
    }
}
