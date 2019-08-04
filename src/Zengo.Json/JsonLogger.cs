using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Zengo.Interfaces;

namespace Zengo.Json
{
    internal class JsonLogger : ILogger
    {
        public IConfig Config => _config;
        private readonly JsonConfig _config;

        public JsonLogger(JsonConfig config)
        {
            _config = config;
        }

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
                var fileName = $"{table.Name}-{dateTime.ToString(_config.FileNameFormat)}.json";

                using (var file = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                using (var sw = new StreamWriter(file))
                {
                    sw.WriteLine(JsonConvert.SerializeObject(table, _config.JsonSerializerSettings));
                }
            });
        }
    }
}