using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Zengo.Interfaces;

namespace Zengo.Csv
{
    internal class CsvLogger : ILogger
    {
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
                var fileName = string.Format($"{table.Name}-{Config.FileNameFormat}.csv", dateTime);

                using (var file = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                using (var sw = new StreamWriter(file, Config.Csv.Encoding))
                using (var csv = new CsvWriter(sw))
                {
                    var columns = table.Columns;

                    if (Config.Csv.OutputColumnLine)
                    {
                        columns.ToList().ForEach(column => csv.WriteField(column.Name, Config.Csv.ForceQuote));
                        csv.NextRecord();
                    }

                    table.Rows.ToList().ForEach(row =>
                    {
                        row.Items.ToList().ForEach(item => csv.WriteField(item.Value.ToString(), Config.Csv.ForceQuote));
                        csv.NextRecord();
                    });
                }
            });
        }
    }
}
