using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Xunit;

namespace JsonStructuredLogger.Tests.EdgeCases
{
    public class ScopeTests
    {
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

        [Fact]
        public void Can_Handle_Indexed_Scope_Objects()
        {
            // arrange
            var fixture = new LoggerFixture();
            var logger = fixture.CreateLogger<JsonStructuredLoggerTests>();

            // act
            using var scope = logger.BeginScope(new IndexedObj<string>());
            using var scope2 = logger.BeginScope(new IndexedObj<int>());
            logger.LogInformation("foo");

            // assert
            Assert.Single(fixture.LogEntries);
        }

        [Fact]
        public void Can_Handle_Dictionary_Scope()
        {
            // arrange
            var fixture = new LoggerFixture();
            var logger = fixture.CreateLogger<JsonStructuredLoggerTests>();

            // act
            using var scope = logger.BeginScope(new Dictionary<string,object>()
            {
                ["str"] = "string",
                ["int"] = 15
            });
            
            logger.LogInformation("foo");

            // assert
            Assert.Single(fixture.LogEntries);
            Assert.Contains("str", fixture.LogEntries.Single().Properties.Keys);
        }

        private class IndexedObj<T>
        {
            public string this[T index] => "foo";
            public string OtherProp => "Hello";
        }
    }
}
