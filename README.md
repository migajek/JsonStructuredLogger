# JsonStructuredLogger

Provides a way to persists `Microsoft.Extensions.Logging` log messages as structured JSON entries, storing all properties passed to log message along with scope (state) properties.

Minimal usage example

```c#
using var loggerFactory = LoggerFactory.Create(cfg => { cfg.AddJsonFile("./", "app_{date}.json"); });
var logger = loggerFactory.CreateLogger<Program>();

logger.LogInformation("Hello {Who}", "World");
using (var scope = logger.BeginScope(new {CorrelationId = "0xdeadbeef"}))
{
    logger.LogInformation("Processing {RequestId}", 10);
}
logger.LogInformation("Done");
```

produces a file which, along with the log event details, contains the full properties object (left only Message and Properties for brevity):
```json
{"Message":"Hello World","Properties":{"Who":"World","{OriginalFormat}":"Hello {Who}"}}
{"Message":"Processing 10","Properties":{"RequestId":10,"{OriginalFormat}":"Processing {RequestId}","CorrelationId":"0xdeadbeef"}}
{"Message":"Done","Properties":{"{OriginalFormat}":"Done"}}
```

full file:
```json
{"Timestamp":"2021-05-29T13:12:55.990651+00:00","LogLevel":2,"EventId":0,"EventName":null,"Category":"JsonLoggerDemo.Program","Exception":null,"Message":"Hello World","Properties":{"Who":"World","{OriginalFormat}":"Hello {Who}"}}
{"Timestamp":"2021-05-29T13:12:56.129659+00:00","LogLevel":2,"EventId":0,"EventName":null,"Category":"JsonLoggerDemo.Program","Exception":null,"Message":"Processing 10","Properties":{"RequestId":10,"{OriginalFormat}":"Processing {RequestId}","CorrelationId":"0xdeadbeef"}}
{"Timestamp":"2021-05-29T13:12:56.1658444+00:00","LogLevel":2,"EventId":0,"EventName":null,"Category":"JsonLoggerDemo.Program","Exception":null,"Message":"Done","Properties":{"{OriginalFormat}":"Done"}}
```

