using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using CsvHelper;
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
                using (var sw = new StreamWriter(file, _config.Encoding))
                using (var csv = new CsvWriter(sw))
                {
                    var columns = table.Columns;

                    if (_config.OutputColumnLine)
                    {
                        columns.ToList().ForEach(column => csv.WriteField(column.Name, _config.ForceQuote));
                        csv.NextRecord();
                    }

                    table.Rows.ToList().ForEach(row =>
                    {
                        row.Items.ToList().ForEach(item => csv.WriteField(item.Value.ToString(), _config.ForceQuote));
                        csv.NextRecord();
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
