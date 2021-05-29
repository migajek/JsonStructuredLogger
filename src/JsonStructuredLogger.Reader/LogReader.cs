using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JsonStructuredLogger.Reader
{
    public class LogReader
    {
        private async IAsyncEnumerable<string> ReadAsync(string fileName)
        {
            using var reader = System.IO.File.OpenText(fileName);
            while (!reader.EndOfStream)
            {
                yield return await reader.ReadLineAsync();
            }
        }

        public IAsyncEnumerable<JsonLogEntry> ReadEntries(string fileName)
        {
            return ReadAsync(fileName).Select(x => JsonConvert.DeserializeObject<JsonLogEntryResponse>(x));
        }

        public async Task<IReadOnlyCollection<JsonLogEntry>> ReadAll(string fileName)
        {
            return await ReadEntries(fileName).ToArrayAsync();
        }
    }
}
