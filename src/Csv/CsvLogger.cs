using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Zengo.Interfaces;

namespace Zengo.Csv
{
    internal class CsvLogger : ILogger
    {
        private readonly CsvConfig _config;

        public CsvLogger(CsvConfig config)
        {
            _config = config;
        }

        public void Write(IEnumerable<ITable> before, IEnumerable<ITable> after)
        {
            WriteCsv(before);
            WriteCsv(after);
        }

        private void WriteCsv(IEnumerable<ITable> tables)
        {
            tables.ToList().ForEach(table =>
            {
                var dateTime = tables.Min(t => t.DateTime);
                var fileName = string.Format($"{table.Name}-{_config.FileNameFormat}.csv", dateTime);

                using (var file = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                using (var sw = new StreamWriter(file))
                {
                    var columns = table.Columns;

                    if (_config.OutputColumnLine)
                    {
                        sw.WriteLine(string.Join(",", columns.Select(column => Convert(column.Name))));
                    }

                    table.Rows.ToList().ForEach(row =>
                    {
                        sw.WriteLine(string.Join(",", row.Items.Select(item => Convert(item.Value, item.IsDBNull))));
                    });
                }
            });
        }

        private string Convert(object value, bool isDBNull = false)
            => isDBNull
            ? _config.NullString
            : "\"" + value?.ToString().Replace("\"", "\"\"") + "\"";
    }
}