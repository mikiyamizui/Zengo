using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Zengo.Config;
using Zengo.Loggers;

namespace Zengo
{
    public class ZengoLogger<TDataAdapter>
        where TDataAdapter : DbDataAdapter
    {
        public IDbConnection Connection { get; }

        private IDictionary<Type, LoggerConfig> _configurations = new Dictionary<Type, LoggerConfig>();

        static ZengoLogger()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public ZengoLogger(IDbConnection connection)
        {
            Connection = connection;
        }

        public TConfiguraion GetConfiguration<TConfiguraion>()
            where TConfiguraion : LoggerConfig
        {
            var type = typeof(PlaneTextLoggerConfig);
            return _configurations.ContainsKey(type)
                ? _configurations[type] as TConfiguraion
                : Activator.CreateInstance<TConfiguraion>();
        }

        public ZengoLogger<TDataAdapter> SetConfiguration<TConfiguration>(LoggerConfig configuration)
        {
            _configurations.Add(configuration.GetType(), configuration);
            return this;
        }

        public ILoggerManager<TDataAdapter> Table(string tableName, string filterSql = null)
            => new LoggerManager<TDataAdapter>(this, tableName, filterSql);
    }
}