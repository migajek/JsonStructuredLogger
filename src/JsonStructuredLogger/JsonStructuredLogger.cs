using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using JsonStructuredLogger.Infrastructure;
using Microsoft.Extensions.Logging;

namespace JsonStructuredLogger
{
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

            _entryWriter(message, JsonSerializer.Serialize(message));
        }

        private void FillExternalScopeProperties(IDictionary<string, object> dictionary)
        {
            _scopeProvider.ForEachScope((scope, state) => FillStateProperties(state, scope), dictionary);
        }

        private void FillStateProperties(IDictionary<string, object> dictionary, object scope)
        {
            if (scope == null)
                return;

            // The scope can be defined using BeginScope or LogXXX methods.
            // - logger.BeginScope(new { Author = "meziantou" })
            // - logger.LogInformation("Hello {Author}", "meziaantou")
            // Using LogXXX, an object of type FormattedLogValues is created. This type is internal but it implements IReadOnlyList, so we can use it.
            // https://github.com/aspnet/Extensions/blob/cc9a033c6a8a4470984a4cc8395e42b887c07c2e/src/Logging/Logging.Abstractions/src/FormattedLogValues.cs
            if (scope is IReadOnlyList<KeyValuePair<string, object>> formattedLogValues)
            {
                if (formattedLogValues.Count > 0)
                {
                    foreach (var value in formattedLogValues)
                    {
                        // MethodInfo is set by ASP.NET Core when reaching a controller. This type cannot be serialized using JSON.NET, but I don't need it.
                        if (value.Value is MethodInfo methodInfo)
                        {
                            dictionary[value.Key] = methodInfo.Name;
                            continue;
                        }

                        dictionary[value.Key] = value.Value;
                    }
                }
            }
            else
            {
                var appendToDictionaryMethod = ExpressionCache.GetOrCreateAppendToDictionaryMethod(scope.GetType());
                appendToDictionaryMethod(dictionary, scope);
            }
        }
    }


}