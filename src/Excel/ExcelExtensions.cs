using Zengo.Excel;

namespace Zengo
{
    public static class ExcelExtensions
    {
        public static IBuilder SaveAsExcel(this IBuilder self, string tableName, string filterSql = null)
        {
            self.Tables[tableName] = filterSql;
            self.Loggers.Add(new ExcelLogger());

            return self;
        }
    }
}
