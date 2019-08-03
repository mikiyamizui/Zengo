using System.Data.Common;
using Zengo.Interfaces;

namespace Zengo.TextFile
{
    public static class Extensions
    {
        public static ILoggerManager<TDataAdapter> AsTextFile<TDataAdapter>(this ILoggerManager<TDataAdapter> self)
            where TDataAdapter : DbDataAdapter
        {
            var manager = self.ConfigManager;
            var config = manager.GetConfiguration<TextFileLoggerConfig>();
            var logger = new TextFileLogger<TDataAdapter>(manager.Connection, config, self.TableName, self.FilterSql);

            self.Execute(logger, config);
            self.AddLogger(logger, config);

            return self;
        }
    }
}