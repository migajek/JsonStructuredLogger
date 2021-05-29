using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace JsonStructuredLogger.Infrastructure
{
    // https://www.meziantou.net/asp-net-core-json-logger.htm
    internal static class ExpressionCache
    {
        public delegate void AppendToDictionary(IDictionary<string, object> dictionary, object o);

        private static readonly ConcurrentDictionary<Type, AppendToDictionary> STypeCache = new ConcurrentDictionary<Type, AppendToDictionary>();
        private static readonly PropertyInfo DictionaryIndexerProperty = GetDictionaryIndexer();

        public static AppendToDictionary GetOrCreateAppendToDictionaryMethod(Type type)
        {
            return STypeCache.GetOrAdd(type, t => CreateAppendToDictionaryMethod(t));
        }

        private static AppendToDictionary CreateAppendToDictionaryMethod(Type type)
        {
            var dictionaryParameter = Expression.Parameter(typeof(IDictionary<string, object>), "dictionary");
            var objectParameter = Expression.Parameter(typeof(object), "o");

            var castedParameter = Expression.Convert(objectParameter, type); // cast o to the actual type

            // Create setter for each properties
            // dictionary["PropertyName"] = o.PropertyName;
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            var setters =
                from prop in properties
                where prop.CanRead
                let indexerExpression = Expression.Property(dictionaryParameter, DictionaryIndexerProperty, Expression.Constant(prop.Name))
                let getExpression = Expression.Property(castedParameter, prop.GetMethod)
                select Expression.Assign(indexerExpression, getExpression);

            var body = new List<Expression>(properties.Length + 1);
            body.Add(castedParameter);
            body.AddRange(setters);

            var lambdaExpression = Expression.Lambda<AppendToDictionary>(Expression.Block(body), dictionaryParameter, objectParameter);
            return lambdaExpression.Compile();
        }

        // Get the PropertyInfo for IDictionary<string, object>.this[string key]
        private static PropertyInfo GetDictionaryIndexer()
        {
            var properties = typeof(IDictionary<string, object>).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var indexers = from prop in properties
                           let indexParameters = prop.GetIndexParameters()
                           where indexParameters.Length == 1 && typeof(string).IsAssignableFrom(indexParameters[0].ParameterType)
                           select prop;

            return indexers.Single();
        }
    }
}