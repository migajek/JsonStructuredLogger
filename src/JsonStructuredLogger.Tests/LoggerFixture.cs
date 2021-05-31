using System.Linq;
using JsonStructuredLogger.Tests.Infrastructure;
using Microsoft.Extensions.Logging;

namespace JsonStructuredLogger.Tests
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
}
