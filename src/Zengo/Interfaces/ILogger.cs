using System;
using System.Collections.Generic;

namespace Zengo.Interfaces
{
    public interface ILogger
    {
        IConfig Config { get; }

        void Write(IEnumerable<ITable> before, IEnumerable<ITable> after);
    }

    public static class ILoggerExtensions
    {
        public static object QuoteString<TLogger>(this TLogger logger,
            object value, bool isDBNull, Type dataType, bool force = false)
            where TLogger : ILogger
            => force || (!isDBNull && typeof(string) == dataType)
            ? logger.Config.QuoteString
            + (value?.ToString().Replace(logger.Config.QuoteString, logger.Config.QuoteString + logger.Config.QuoteString))
            + logger.Config.QuoteString
            : value;

        public static object NullStringIfDBNull<TLogger>(this TLogger logger, object value, bool isDBNull)
            where TLogger : ILogger
            => isDBNull && string.IsNullOrEmpty(value?.ToString())
            ? logger.Config.NullString
            : $"'{value}'";

        public static object EmptyStringIfEmpty<TLogger>(this TLogger logger, object value, bool isDBNull)
            where TLogger : ILogger
            => string.IsNullOrEmpty(value.ToString()) && !isDBNull
            ? logger.Config.EmptyString
            : value;
    }
}