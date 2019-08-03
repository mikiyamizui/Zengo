using System;
using System.Data;
using System.IO;
using System.Linq;
using Zengo.Abstracts;

namespace Zengo.CsvFile
{
    internal class CsvFileLogger<TDataAdapter> : BaseLogger<CsvFileLoggerConfig>
    {
        public override string Extension => ".csv";

        public CsvFileLogger(IDbConnection connection, CsvFileLoggerConfig configuraion, string tableName, string filterString)
            : base(connection, configuraion, tableName, filterString)
        {
        }

        public override void OnWrite(TextWriter sw,
            (string ColumnName, Type DataType, int Length)[] columns,
            (int Ordinal, object Value, bool IsDBNull)[][] rows)
        {
            rows.ToList().ForEach(row =>
            {
                sw.WriteLine(string.Join(",", row.Select(cell =>
                {
                    var column = columns[cell.Ordinal];
                    var value = cell.Value;

                    value = QuoteString(value, cell.IsDBNull, column.DataType, force: true);

                    return value.ToString();
                })));
            });
        }
    }
}