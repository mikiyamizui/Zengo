using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Zengo.Interfaces;

namespace Zengo.Csv
{
    internal class CsvLogger : ILogger
    {
        public IConfig Config => _config;
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
                var fileName = $"{table.Name}-{dateTime.ToString(_config.FileNameFormat)}.csv";

                using (var file = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                using (var sw = new StreamWriter(file))
                {
                    var columns = table.Columns;

                    table.Rows.ToList().ForEach(row =>
                    {
                        sw.WriteLine(string.Join(",", row.Items.Select((item, index)
                            => this.QuoteString(item.Value, item.IsDBNull, columns[index].DataType, force: true).ToString()
                            )));
                    });
                }
            });
        }
    }
}