namespace JsonStructuredLogger.FileProvider
{
    public class JsonStructuredFileLoggerOptions
    {
        public const string DefaultFileNameFormat = "{date}.json";
        public const string DefaultDateFormat = "yyyy-MM-dd";
        public string Directory { get; }
        public string FileNameFormat { get; }
        public string DateFormat { get; }

        public JsonStructuredFileLoggerOptions(string directory, string fileNameFormat = null, string dateFormat = null)
        {
            Directory = directory;
            FileNameFormat = fileNameFormat ?? DefaultFileNameFormat;
            DateFormat = dateFormat ?? DefaultDateFormat;
        }
    }
}