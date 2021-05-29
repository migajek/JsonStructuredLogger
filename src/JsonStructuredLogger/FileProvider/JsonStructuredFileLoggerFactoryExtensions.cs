using Microsoft.Extensions.Logging;

namespace JsonStructuredLogger.FileProvider
{
    public static class JsonStructuredFileLoggerFactoryExtensions
    {
        public static ILoggingBuilder AddJsonFile(this ILoggingBuilder builder, string directory, string fileNameFormat = null,
            string dateFormat = null)
        {
            return builder.AddProvider(
                new JsonStructuredFileLoggerProvider(
                    new JsonStructuredFileLoggerOptions(directory, fileNameFormat, dateFormat)));
        }
    }
}
