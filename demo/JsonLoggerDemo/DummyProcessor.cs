using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace JsonLoggerDemo
{
    internal class DummyProcessor
    {
        private static readonly Random Random = new Random();
        private readonly ILogger<DummyProcessor> _logger;

        public DummyProcessor(ILogger<DummyProcessor> logger)
        {
            _logger = logger;
        }

        public void ProcessOperations(User user)
        {
            using var scope = _logger.BeginScope("Processing user {UserId}", user.Id);
            var retries = Enumerable.Range(1, Random.Next(0, 5));
            foreach (var retry in retries)
            {
                _logger.LogInformation("User details {@User}. Retry {Retry}", user, retry);
                _logger.LogInformation("Response from external service {@Response}", new {ResponseId = Random.Next(0, 1024)});
            }

            var status = Random.Next(0, 2) == 1;
            _logger.LogInformation("Operation success: {Success}", status);
        }
    }
}