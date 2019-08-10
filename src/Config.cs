using Zengo.Csv;
using Zengo.Excel;
using Zengo.Json;

namespace Zengo
{
    public static class Config
    {
        public static string FileNameFormat { get; set; } = "{0:yyyyMMdd-HHmmss-ffff}";

        public static string NullString { get; set; } = "(NULL)";

        public static CsvConfig Csv { get; } = new CsvConfig();
        public static ExcelConfig Excel { get; } = new ExcelConfig();
        public static JsonConfig Json { get; } = new JsonConfig();
    }
}