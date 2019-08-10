using Zengo.Csv;

namespace Zengo
{
    public static class CsvExtensions
    {
        public static IBuilder SaveAsCsv(
            this IBuilder self,
            string tableName, string filterSql = null)
        {
            self.Tables[tableName] = filterSql;
            self.Loggers.Add(new CsvLogger());
            return self;
        }
    }
}
