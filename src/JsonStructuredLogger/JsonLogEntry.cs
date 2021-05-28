using System;
using System.Collections.Generic;

namespace JsonStructuredLogger
{
    public sealed class JsonLogEntry
    {
        public DateTimeOffset Timestamp { get; set; }
        public int LogLevel { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string Category { get; set; }
        public string Exception { get; set; }
        public string Message { get; set; }
        public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>(StringComparer.Ordinal);
    }
}