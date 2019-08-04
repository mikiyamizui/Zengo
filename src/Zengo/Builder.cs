using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Zengo.Core;
using Zengo.Interfaces;

namespace Zengo
{
    public class Builder : IBuilder
    {
        public IDictionary<string, string> Tables { get; } = new Dictionary<string, string>();

        public IList<ILogger> Loggers { get; } = new List<ILogger>();

        private readonly IDbConnection _connection;

        internal Builder(IDbConnection connection)
        {
            _connection = connection;
        }

        public IDisposable ToDisposable()
        {
            return new Disposable(this);
        }

        public IEnumerable<ITable> CollectTables()
        {
            return Tables.Select(item =>
            {
                var tableName = item.Key;
                var filterSql = item.Value;
                var sql = $@"select * from {tableName} {filterSql ?? ""}".Trim();

                using (var command = _connection.CreateCommand())
                {
                    command.CommandText = sql;

                    using (var reader = command.ExecuteReader())
                    {
                        var columns = new List<Column>();
                        var rows = new List<Row>();

                        while (reader.Read())
                        {
                            var items = Enumerable.Range(0, reader.FieldCount).Select(ordinal =>
                            {
                                var columnName = reader.GetName(ordinal);
                                var value = reader.GetValue(ordinal);
                                var dataType = reader.GetFieldType(ordinal);
                                var isDBNull = reader.IsDBNull(ordinal);

                                if (columns.Count <= ordinal)
                                    columns.Add(new Column(ordinal, columnName, dataType));
                                var column = columns[ordinal];

                                return new Item(column, value, isDBNull);
                            }).ToList();

                            rows.Add(new Row(items));
                        }

                        return new Table(tableName, sql, columns, rows, string.IsNullOrEmpty(filterSql));
                    }
                }
            }).ToList();
        }
    }
}