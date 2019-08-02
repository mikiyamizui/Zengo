using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using Zengo.Config;

namespace Zengo.Loggers
{
    internal class LoggerManager<TDataAdapter> : ILoggerManager<TDataAdapter>
        where TDataAdapter : DbDataAdapter
    {
        private ZengoLogger<TDataAdapter> _zengo;
        private List<(IZengoLogger Logger, LoggerConfig Config)> _loggers = new List<(IZengoLogger, LoggerConfig)>();
        private string _tableName;
        private string _filterSql;

        public LoggerManager(ZengoLogger<TDataAdapter> zengo, string tableName, string filterSql)
        {
            _zengo = zengo;
            _tableName = tableName;
            _filterSql = filterSql;
        }

        public ILoggerManager<TDataAdapter> AsPlaneText()
        {
            var config = _zengo.GetConfiguration<PlaneTextLoggerConfig>();
            var logger = new PlaneTextLogger<TDataAdapter>(_zengo.Connection, config, _tableName, _filterSql);

            Execute(logger, config);

            _loggers.Add((new PlaneTextLogger<TDataAdapter>(_zengo.Connection, config, _tableName, _filterSql), config));
            return this;
        }

        public void Dispose()
        {
            _loggers.ForEach(item => Execute(item.Logger, item.Config));
        }

        private void Execute(IZengoLogger logger, LoggerConfig config)
        {
            var fileName = $"{_tableName}{DateTime.Now.ToString(config.FileName)}{logger.Extension}";
            using (var file = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var sw = new StreamWriter(file, Encoding.UTF8))
            {
                try
                {
                    var sql = $@"select * from {_tableName} {_filterSql ?? ""}".Trim();

                    sw.WriteLine(sql);
                    sw.WriteLine();

                    var table = new DataTable();
                    using (var adapter = Activator.CreateInstance(typeof(TDataAdapter), sql, _zengo.Connection) as TDataAdapter)
                    {
                        adapter.Fill(table);
                    }

                    var rows = table.Rows
                        .Cast<DataRow>()
                        .Select(row => Enumerable.Range(0, row.ItemArray.Length)
                            .Select(ordinal =>
                            {
                                var value = row.ItemArray[ordinal];
                                var isDBNull = row.IsNull(ordinal);

                                if (!isDBNull && typeof(string) == table.Columns[ordinal].DataType)
                                {
                                    value = $"'{value}'";
                                }

                                if (string.IsNullOrEmpty(value.ToString()))
                                {
                                    value = isDBNull ? config.Null : config.Empty;
                                }

                                return (
                                    Ordinal: ordinal,
                                    Value: value,
                                    IsDBNull: isDBNull
                                );
                            }).ToArray())
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