using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace JsonStructuredLogger
{
    public abstract class JsonStructuredLoggerProvider : ILoggerProvider
    {
        private readonly LoggerExternalScopeProvider _scopeProvider = new LoggerExternalScopeProvider();
        private readonly ConcurrentDictionary<string, JsonStructuredLogger> _loggers =
            new ConcurrentDictionary<string, JsonStructuredLogger>(StringComparer.Ordinal);

        protected abstract void WriteEntry(JsonLogEntry entry, string serialized);
        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, category => new JsonStructuredLogger(WriteEntry, category, _scopeProvider));
        }

        public void Dispose()
        {
        }
    }
}