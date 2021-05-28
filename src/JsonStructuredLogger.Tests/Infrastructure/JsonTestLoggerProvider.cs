using System.Collections.Generic;
using JsonStructuredLogger;

public class JsonTestLoggerProvider : JsonStructuredLoggerProvider
{
    private List<JsonLogEntry> _logEntries = new List<JsonLogEntry>();
    public IReadOnlyCollection<JsonLogEntry> LogEntries { get; }
    public JsonTestLoggerProvider()
    {
        LogEntries = _logEntries.AsReadOnly();
    }

    protected override void WriteEntry(JsonLogEntry entry, string serialized)
    {
        _logEntries.Add(entry);
    }
}