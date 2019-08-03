using System.Data.Common;
using Zengo.Interfaces;

namespace Zengo.CsvFile
{
    public static class Extensions
    {
        public static ILoggerManager<TDataAdapter> AsCsvFile<TDataAdapter>(this ILoggerManager<TDataAdapter> self)
            where TDataAdapter : DbDataAdapter
        {
            var manager = self.ConfigManager;
            var config = manager.GetConfiguration<CsvFileLoggerConfig>();
            var logger = new CsvFileLogger<TDataAdapter>(manager.Connection, config, self.TableName, self.FilterSql);

            self.Execute(logger, config);
            self.AddLogger(logger, config);

            return self;
        }
    }
}