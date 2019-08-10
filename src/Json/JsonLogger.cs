using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Zengo.Interfaces;

namespace Zengo.Json
{
    internal class JsonLogger : ILogger
    {
        public void Write(IEnumerable<ITable> before, IEnumerable<ITable> after)
        {
            WriteJson(before);
            WriteJson(after);
        }

        private void WriteJson(IEnumerable<ITable> tables)
        {
            tables.ToList().ForEach(table =>
            {
                var dateTime = tables.Min(t => t.DateTime);
                var fileName = string.Format($"{table.Name}-{Config.FileNameFormat}.json", dateTime);

                using (var file = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                using (var sw = new StreamWriter(file))
                {
                    sw.WriteLine(JsonConvert.SerializeObject(table, Config.Json));
                }
            });
        }
    }
}
