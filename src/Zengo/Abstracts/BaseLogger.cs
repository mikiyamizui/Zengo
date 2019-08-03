using System;
using System.Data;
using System.IO;
using Zengo.Interfaces;

namespace Zengo.Abstracts
{
    internal abstract class BaseLogger<TConfiguraion> : IZengoLogger
        where TConfiguraion : BaseLoggerConfig
    {
        public abstract string Extension { get; }

        protected IDbConnection Connection { get; }
        protected TConfiguraion Configuraion { get; }
        protected string TableName { get; }
        protected string FilterString { get; }

        public BaseLogger(IDbConnection connection, TConfiguraion configuraion, string tableName, string filterString)
        {
            Connection = connection;
            Configuraion = configuraion;
            TableName = tableName;
            FilterString = filterString;
        }

        public abstract void OnWrite(TextWriter sw,
            (string ColumnName, Type DataType, int Length)[] columns,
            (int Ordinal, object Value, bool IsDBNull)[][] rows);

        protected object QuoteString(object value, bool isDBNull, Type dataType, bool force = false)
            => force || (!isDBNull && typeof(string) == dataType)
            ? Configuraion.QuoteString
            + (value?.ToString().Replace(Configuraion.QuoteString, Configuraion.QuoteString + Configuraion.QuoteString))
            + Configuraion.QuoteString
            : value;

        protected object NullStringIfDBNull(object value, bool isDBNull)
            => isDBNull && string.IsNullOrEmpty(value?.ToString())
            ? Configuraion.NullString
            : value;

        protected object EmptyStringIfEmpty(object value, bool isDBNull)
            => string.IsNullOrEmpty(value.ToString()) && !isDBNull
            ? Configuraion.EmptyString
            : value;
    }
}