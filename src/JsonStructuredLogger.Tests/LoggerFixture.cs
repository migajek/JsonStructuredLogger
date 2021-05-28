using Microsoft.Extensions.Logging;
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
}
