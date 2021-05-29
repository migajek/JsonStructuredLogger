using Newtonsoft.Json.Linq;

namespace JsonStructuredLogger.Reader
{
    public static class LogEntryQueryExtensions
    {
        public static T QueryProperty<T>(this JsonLogEntry entry, string query)
        {
            var jObj = entry is JsonLogEntryResponse entryResponse
                ? entryResponse.PropertiesParsed
                : JObject.FromObject(entry.Properties);
            var result = jObj.SelectToken(query);
            if (result == null)
            {
                return default;
            }
            return result.ToObject<T>();
        }

        public static string QueryProperty(this JsonLogEntry entry, string query) =>
            QueryProperty<string>(entry, query);
    }
}