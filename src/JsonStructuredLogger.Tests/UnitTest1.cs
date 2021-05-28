using System;
using Xunit;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace JsonStructuredLogger
{

    internal class LoggerFixture
    {
        private readonly JsonTestLoggerProvider _provider = new JsonTestLoggerProvider();
        public ILoggerFactory Factory { get; }

        public LoggerFixture()
        {
            Factory = LoggerFactory.Create(cfg =>
            {
                cfg.AddProvider(_provider);
            });
        }

        public ILogger<T> CreateLogger<T>() => Factory.CreateLogger<T>();

        public JsonLogEntry[] LogEntries => _provider.LogEntries.ToArray();
    }

    public class UnitTest1
    {
        [Fact]
        public void Contains_Simple_Properties_Passed_To_Message()
        {
            // arrange
            var fixture = new LoggerFixture();
            var logger = fixture.CreateLogger<UnitTest1>();

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
            var logger = fixture.CreateLogger<UnitTest1>();

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
            var logger = fixture.CreateLogger<UnitTest1>();

            // act
            using var scope = logger.BeginScope("Context {CorrelationId}", "0xdeadbeef");
            logger.LogInformation("Hello {Name}", "Alex");

            // assert
            Assert.Single(fixture.LogEntries);
            var logEntry = fixture.LogEntries.Single();
            Console.WriteLine(String.Join(", ", logEntry.Properties.Keys));
            Assert.Equal("0xdeadbeef", logEntry.Properties["CorrelationId"]);
        }

        [Fact]
        public void Contains_Multiple_Scope_Properties()
        {
            // arrange
            var fixture = new LoggerFixture();
            var logger = fixture.CreateLogger<UnitTest1>();

            // act
            using var scope = logger.BeginScope("Context {CorrelationId}", "0xdeadbeef");
            using var secondScope = logger.BeginScope(new {AuthInfo = "Token"});
            logger.LogInformation("Hello {Name}", "Alex");

            // assert
            Assert.Single(fixture.LogEntries);
            var logEntry = fixture.LogEntries.Single();
            Console.WriteLine(String.Join(", ", logEntry.Properties.Keys));
            Assert.Equal("0xdeadbeef", logEntry.Properties["CorrelationId"]);
            Assert.Equal("Token", logEntry.Properties["AuthInfo"]);
        }
    }
}
