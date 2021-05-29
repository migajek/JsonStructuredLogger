using Microsoft.Extensions.Logging;

namespace JsonLoggerDemo
{
    internal class ProcessCommandHandler
    {
        private  readonly ILogger<ProcessCommandHandler> _logger;
        private readonly DummyProcessor _processor;
        
        public ProcessCommandHandler(ILogger<ProcessCommandHandler> logger, DummyProcessor processor)
        {
            _logger = logger;
            _processor = processor;
        }

        public void Process(string requestId)
        {
            using var scope = _logger.BeginScope(new {RequestId = requestId});

            var users = new User[]
            {
                "Alex W",
                "John Doe",
                "Mike F"
            };

            foreach (var user in users)
            {
                _processor.ProcessOperations(user);
            }
        }
    }
}