using System;
using System.Data.Common;
using Zengo.Abstracts;

namespace Zengo.Interfaces
{
    public interface ILoggerManager<TDataAdapter> : IDisposable
        where TDataAdapter : DbDataAdapter
    {
        ConfigManager<TDataAdapter> ConfigManager { get; }

        string TableName { get; }

        string FilterSql { get; }

        void AddLogger(IZengoLogger logger, BaseLoggerConfig config);

        void Execute(IZengoLogger logger, BaseLoggerConfig config);
    }
}