namespace JsonStructuredLogger.File
{
    public class JsonStructuredFileLoggerOptions
    {
        public string Directory { get; }
        public string FileNameFormat { get; }
        public string DateFormat { get; }

        public JsonStructuredFileLoggerOptions(string directory, string fileNameFormat = null, string dateFormat = null)
        {
            Directory = directory;
            FileNameFormat = fileNameFormat ?? "{date}.json";
            DateFormat = dateFormat ?? "yyyy-MM-dd";
        }
    }
}