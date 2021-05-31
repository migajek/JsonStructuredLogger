using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using JsonStructuredLogger.Infrastructure;
using Microsoft.Extensions.Logging;

namespace JsonStructuredLogger
{

    // https://www.meziantou.net/asp-net-core-json-logger.htm
    internal class JsonStructuredLogger : ILogger
    {
        public delegate void EntryWriter(JsonLogEntry entry, string serialized);

        private readonly EntryWriter _entryWriter;
        private readonly string _categoryName;
        private readonly IExternalScopeProvider _scopeProvider;

        public JsonStructuredLogger(EntryWriter writer, string categoryName, IExternalScopeProvider scopeProvider)
        {
            _entryWriter = writer;
            _categoryName = categoryName;
            _scopeProvider = scopeProvider;
        }

        public IDisposable BeginScope<TState>(TState state) => _scopeProvider?.Push(state) ?? NullDisposable.Instance;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter is null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }


            var message = new JsonLogEntry
            {
                Timestamp = DateTime.UtcNow,
                LogLevel = (int)logLevel,
                EventId = eventId.Id,
                EventName = eventId.Name,
                Category = _categoryName,
                Exception = exception?.ToString(),
                Message = formatter(state, exception),
            };

            FillStateProperties(message.Properties, state);
            FillExternalScopeProperties(message.Properties);

            _entryWriter(message, Serialize(message));
        }

        private string Serialize(JsonLogEntry message)
        {
            try
            {
                return JsonSerializer.Serialize(message);
            }
            catch (Exception ex)
            {
                message.Properties.Clear();
                message.Properties["__serialization_exception"] = ex.Message;
                return JsonSerializer.Serialize(message);
            }
        }

        private void FillExternalScopeProperties(IDictionary<string, object> dictionary)
        {
            _scopeProvider.ForEachScope((scope, state) => FillStateProperties(state, scope), dictionary);
        }

        private void FillStateProperties(IDictionary<string, object> dictionary, object scope)
        {
            switch (scope)
            {
                case null:
                    return;

                case IReadOnlyList<KeyValuePair<string, object>> formattedLogValues:
                {
                    foreach (var value in formattedLogValues)
                    {
                        if (value.Value is MethodInfo methodInfo)
                        {
                            dictionary[value.Key] = methodInfo.Name;
                        }
                        else
                        {
                            dictionary[value.Key] = value.Value;
                        }
                    }

                    break;
                }

                default:
                {
                    var appendToDictionaryMethod = ExpressionCache.GetOrCreateAppendToDictionaryMethod(scope.GetType());
                    appendToDictionaryMethod(dictionary, scope);
                    break;
                }
            }
        }
    }


}