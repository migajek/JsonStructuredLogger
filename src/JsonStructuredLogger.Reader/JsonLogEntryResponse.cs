using Newtonsoft.Json.Linq;

namespace JsonStructuredLogger.Reader
{
    internal class JsonLogEntryResponse: JsonLogEntry
    {
        private JObject _properties;

        public JObject PropertiesParsed
        {
            get
            {
                if (_properties == null)
                {
                    _properties = JObject.FromObject(Properties);
                }

                return _properties;
            }
        }
    }
}
