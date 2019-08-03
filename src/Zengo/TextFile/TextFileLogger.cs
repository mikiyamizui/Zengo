using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Zengo.Abstracts;
using Zengo.Utils;

namespace Zengo.TextFile
{
    internal class TextFileLogger<TDataAdapter> : BaseLogger<TextFileLoggerConfig>
        where TDataAdapter : IDbDataAdapter
    {
        public override string Extension => ".log";

        public TextFileLogger(IDbConnection connection, TextFileLoggerConfig configuraion, string tableName, string filterString)
            : base(connection, configuraion, tableName, filterString)
        {
        }

        public override void OnWrite(TextWriter sw,
            (string ColumnName, Type DataType, int Length)[] columns,
            (int Ordinal, object Value, bool IsDBNull)[][] rows
            )
        {
            var columnLine = FormatLine(true, columns.Select(column => column.ColumnName.Pad(false, column.Length)));
            var horizontalLine = FormatLine(false, columns.Select(column => new string(Configuraion.HorizontalLineChar, column.Length)));

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

                    value = QuoteString(value, cell.IsDBNull, column.DataType);
                    value = NullStringIfDBNull(value, cell.IsDBNull);
                    value = EmptyStringIfEmpty(value, cell.IsDBNull);

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
                ? Configuraion.ValueSeparatorFirst
                : Configuraion.LineSeparatorFirst;

            var middle = valueLine
                ? Configuraion.ValueSeparatorMiddle
                : Configuraion.LineSeparatorMiddle;

            var last = valueLine
                ? Configuraion.ValueSeparatorLast
                : Configuraion.LineSeparatorLast;

            return (first + string.Join(middle, elements) + last);
        }
    }
}