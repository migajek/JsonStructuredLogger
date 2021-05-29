using System;
using System.IO;

namespace JsonStructuredLogger.FileProvider
{
    public class JsonStructuredFileLoggerProvider : JsonStructuredLoggerProvider
    {
        private readonly JsonStructuredFileLoggerOptions _options;

        public JsonStructuredFileLoggerProvider(JsonStructuredFileLoggerOptions options)
        {
            _options = options;
        }

        protected override void WriteEntry(JsonLogEntry entry, string serialized)
        {
            var fileName = _options.FileNameFormat.Replace("{date}", DateTime.UtcNow.ToString(_options.DateFormat));
            var fullName = Path.Combine(_options.Directory, fileName);
            
            System.IO.File.AppendAllLines(fullName, new []{ serialized });
        }
    }
}