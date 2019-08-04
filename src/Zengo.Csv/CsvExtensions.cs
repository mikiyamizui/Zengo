using System.Data.Common;
using Zengo.Csv;
using Zengo.Interfaces;

namespace Zengo
{
    public static class CsvExtensions
    {
        public static IManager<TDataAdapter> AsCsv<TDataAdapter>(
            this IManager<TDataAdapter> self,
            string tableName, string filterSql = null, CsvConfig config = null)
            where TDataAdapter : DbDataAdapter
        {
            self.Tables[tableName] = filterSql;
            self.Loggers.Add(new CsvLogger<TDataAdapter>(config ?? new CsvConfig()));
            return self;
        }
    }
}