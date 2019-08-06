using Zengo.Csv;

namespace Zengo
{
    public static class CsvExtensions
    {
        public static IBuilder AsCsv(
            this IBuilder self,
            string tableName, string filterSql = null, CsvConfig config = null)
        {
            self.Tables[tableName] = filterSql;
            self.Loggers.Add(new CsvLogger(config ?? new CsvConfig()));
            return self;
        }
    }
}