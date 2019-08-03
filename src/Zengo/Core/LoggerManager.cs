using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using Zengo.Abstracts;
using Zengo.Interfaces;

namespace Zengo.Core
{
    public class LoggerManager<TDataAdapter> : ILoggerManager<TDataAdapter>
        where TDataAdapter : DbDataAdapter
    {
        public ConfigManager<TDataAdapter> ConfigManager { get; }
        public string TableName { get; }
        public string FilterSql { get; }

        private List<(IZengoLogger Logger, BaseLoggerConfig Config)> _loggers = new List<(IZengoLogger, BaseLoggerConfig)>();

        public LoggerManager(ConfigManager<TDataAdapter> zengo, string tableName, string filterSql)
        {
            ConfigManager = zengo;
            TableName = tableName;
            FilterSql = filterSql;
        }

        public void Dispose()
            => _loggers.ForEach(item => Execute(item.Logger, item.Config));

        public void AddLogger(IZengoLogger logger, BaseLoggerConfig config)
            => _loggers.Add((logger, config));

        public void Execute(IZengoLogger logger, BaseLoggerConfig config)
        {
            var fileName = $"{TableName}{DateTime.Now.ToString(config.FileNameFormat)}{logger.Extension}";
            using (var file = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var sw = new StreamWriter(file, Encoding.UTF8))
            {
                try
                {
                    var sql = $@"select * from {TableName} {FilterSql ?? ""}".Trim();

                    var table = new DataTable();
                    using (var adapter = Activator.CreateInstance(typeof(TDataAdapter), sql, ConfigManager.Connection) as TDataAdapter)
                    {
                        adapter.Fill(table);
                    }

                    var rows = table.Rows
                        .Cast<DataRow>()
                        .Select(row => Enumerable.Range(0, row.ItemArray.Length)
                            .Select(ordinal => (Ordinal: ordinal, Value: row.ItemArray[ordinal], IsDBNull: row.IsNull(ordinal)))
                            .ToArray())
                        .ToArray();

                    var columns = table.Columns
                        .Cast<DataColumn>()
                        .Select((column, ordinal) =>
                        {
                            var length = column.ColumnName.Length;

                            if (rows.Any())
                            {
                                length = Math.Max(length, rows.Select(row => row[ordinal].Value.ToString()?.Length ?? 0).Max());
                            }

                            return (column.ColumnName, column.DataType, Length: length);
                        })
                        .ToArray();

                    logger.OnWrite(sw, columns, rows);
                }
                catch (Exception e)
                {
                    sw.WriteLine(e);
                }

                file.Flush();
            }
        }
    }
}