using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using Zengo.Config;
using Zengo.Utils;

namespace Zengo.Loggers
{
    internal class PlaneTextLogger<TDataAdapter> : IZengoLogger
        where TDataAdapter : DbDataAdapter
    {
        public string Extension => ".log";

        private DbConnection _connection;
        private PlaneTextLoggerConfig _config;
        private string _tableName;
        private string _filterString;

        public PlaneTextLogger(DbConnection connection, PlaneTextLoggerConfig config, string tableName, string filterString)
        {
            _connection = connection;
            _config = config;
            _tableName = tableName;
            _filterString = filterString;
        }

        public void OnWrite(TextWriter sw,
            (string ColumnName, Type DataType, int Length)[] columns,
            (int Ordinal, object Value, bool IsDBNull)[][] rows
            )
        {
            var columnLine = FormatLine(true, columns.Select(column => column.ColumnName.Pad(false, column.Length)));
            var horizontalLine = FormatLine(false, columns.Select(column => new string(_config.HorizontalLineChar, column.Length)));

            sw.WriteLine(horizontalLine);
            sw.WriteLine(columnLine);
            sw.WriteLine(horizontalLine);

            rows.ToList().ForEach(row =>
            {
                var values = row.Select(cell =>
                {
                    var column = columns[cell.Ordinal];
                    var value = cell.Value;

                    var right = false
                        || value is byte
                        || value is sbyte
                        || value is decimal
                        || value is double
                        || value is float
                        || value is int
                        || value is uint
                        || value is long
                        || value is ulong
                        || value is short
                        || value is ushort
                        ;

                    return cell.Value.ToString().Pad(right, column.Length);
                });

                sw.WriteLine(FormatLine(true, values));
                sw.WriteLine(horizontalLine);
            });

            sw.WriteLine();
            sw.WriteLine($"({(rows.Length > 0 ? rows.Length.ToString() : "No")} record{(rows.Length > 1 ? "s" : "")})");
        }

        private string FormatLine(bool valueLine, IEnumerable<string> elements)
        {
            var first = valueLine
                ? _config.ValueSeparatorFirst
                : _config.LineSeparatorFirst;

            var middle = valueLine
                ? _config.ValueSeparator
                : _config.LineSeparator;

            var last = valueLine
                ? _config.ValueSeparatorLast
                : _config.LineSeparatorLast;

            return (first + string.Join(middle, elements) + last);
        }
    }
}