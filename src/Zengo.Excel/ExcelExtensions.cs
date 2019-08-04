using System.Data.Common;
using Zengo.Excel;
using Zengo.Interfaces;

namespace Zengo
{
    public static class ExcelExtensions
    {
        public static IManager<TDataAdapter> AsExcel<TDataAdapter>(
            this IManager<TDataAdapter> self,
            string tableName, string filterSql = null, ExcelConfig config = null)
            where TDataAdapter : DbDataAdapter
        {
            self.Tables[tableName] = filterSql;
            self.Loggers.Add(new ExcelLogger<TDataAdapter>(config ?? new ExcelConfig()));

            return self;
        }
    }
}