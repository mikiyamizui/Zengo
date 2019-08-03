using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Zengo.Abstracts;
using Zengo.Core;
using Zengo.Interfaces;

namespace Zengo
{
    public class ConfigManager<TDataAdapter>
        where TDataAdapter : DbDataAdapter
    {
        public IDbConnection Connection { get; }

        private IDictionary<Type, BaseLoggerConfig> _configurations = new Dictionary<Type, BaseLoggerConfig>();

        static ConfigManager()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public ConfigManager(IDbConnection connection)
        {
            Connection = connection;
        }

        public TConfiguraion GetConfiguration<TConfiguraion>()
            where TConfiguraion : BaseLoggerConfig
        {
            var type = typeof(TConfiguraion);
            return _configurations.ContainsKey(type)
                ? _configurations[type] as TConfiguraion
                : Activator.CreateInstance<TConfiguraion>();
        }

        public ConfigManager<TDataAdapter> SetConfiguration<TConfiguration>(BaseLoggerConfig configuration)
        {
            _configurations.Add(configuration.GetType(), configuration);
            return this;
        }

        public ILoggerManager<TDataAdapter> Table(string tableName, string filterSql = null)
            => new LoggerManager<TDataAdapter>(this, tableName, filterSql);
    }
}