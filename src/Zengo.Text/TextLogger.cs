using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Zengo.Abstracts;
using Zengo.Interfaces;

namespace Zengo.Text
{
    internal class TextLogger<TDataAdapter> : ILogger
        where TDataAdapter : IDbDataAdapter
    {
        public string Extension => ".log";

        public IConfig Config { get; }
        private TextConfig TextConfig => Config as TextConfig;

        private IDbConnection _connection;
        private string _tableName;
        private string _filterSql;

        public TextLogger(IDbConnection connection, TextConfig config, string tableName, string filterSql)
        {
            Config = config;
            _connection = connection;
            _tableName = tableName;
            _filterSql = filterSql;
        }

        public void OnWrite(TextWriter sw, IEnumerable<IColumn> columns, IEnumerable<IRow> rows)
        {
            var columnLine = FormatLine(true, columns.Select(column => column.Name.Pad(false, column.Width)));
            var horizontalLine = FormatLine(false, columns.Select(column => new string(TextConfig.HorizontalLineChar, column.Width)));

            sw.WriteLine(horizontalLine);
            sw.WriteLine(columnLine);
            sw.WriteLine(horizontalLine);

            rows.ToList().ForEach(row =>
            {
                var values = row.Items.Select(item =>
                {
                    var value = item.Value;

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

                    value = this.QuoteString(value, item.IsDBNull, item.Column.DataType);
                    value = this.NullStringIfDBNull(value, item.IsDBNull);
                    value = this.EmptyStringIfEmpty(value, item.IsDBNull);

                    return item.Value.ToString().Pad(right, item.Column.Width);
                });

                sw.WriteLine(FormatLine(true, values));
                sw.WriteLine(horizontalLine);
            });

            sw.WriteLine();

            var count = rows.Count();
            sw.WriteLine($"({(count > 0 ? count.ToString() : "No")} record{(count > 1 ? "s" : "")})");
        }

        private string FormatLine(bool valueLine, IEnumerable<string> elements)
        {
            var first = valueLine
                ? TextConfig.ValueSeparatorFirst
                : TextConfig.LineSeparatorFirst;

            var middle = valueLine
                ? TextConfig.ValueSeparatorMiddle
                : TextConfig.LineSeparatorMiddle;

            var last = valueLine
                ? TextConfig.ValueSeparatorLast
                : TextConfig.LineSeparatorLast;

            return (first + string.Join(middle, elements) + last);
        }
    }
}