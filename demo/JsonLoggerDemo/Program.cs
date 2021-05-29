using System;
using System.Linq;
using System.Threading.Tasks;
using JsonStructuredLogger.FileProvider;
using JsonStructuredLogger.Reader;
using Microsoft.Extensions.Logging;

namespace JsonLoggerDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var loggerFactory = LoggerFactory.Create(cfg => { cfg.AddJsonFile("./", "log.json"); });

            var commandHandler = new ProcessCommandHandler(loggerFactory.CreateLogger<ProcessCommandHandler>(),
                new DummyProcessor(loggerFactory.CreateLogger<DummyProcessor>()));

            foreach (var _ in Enumerable.Range(0, 3))
            {
                commandHandler.Process(Guid.NewGuid().ToString("N").Substring(1, 4));
            }

            var reader = new LogReader();
            var entries = reader.ReadEntries("./log.json");

            var failedRequests = await entries
                .Where(x => x.QueryProperty<bool?>("Success") == false)
                .Select(x => x.QueryProperty("RequestId"))
                .Distinct()
                .ToArrayAsync();

            foreach (var failedRequest in failedRequests)
            {
                Console.WriteLine($"Operation {failedRequest} failed");
            }

            var usersWithTopRetries = await entries
                .Where(x => x.QueryProperty<int?>("Retry").HasValue)
                .Select(x => new
                {
                    Retry = x.QueryProperty<int>("Retry"), 
                    User = $"{x.QueryProperty("@User.FirstName")} {x.QueryProperty("@User.LastName")}"
                        
                })
                .GroupBy(x => x.User)
                .SelectAwait(async x => new {User = x.Key, Retries = await x.MaxAsync(y => y.Retry)})
                .ToArrayAsync();

            foreach (var attempt in usersWithTopRetries)
            {
                Console.WriteLine($"Got {attempt.Retries} retries for {attempt.User}");
            }
        }
    }
}
