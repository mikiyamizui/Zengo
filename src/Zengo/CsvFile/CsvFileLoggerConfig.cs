using Zengo.Abstracts;

namespace Zengo.CsvFile
{
    public class CsvFileLoggerConfig : BaseLoggerConfig
    {
        public bool OutputColumnLine { get; } = false;
        public bool ForceQuoteValues { get; } = false;
    }
}