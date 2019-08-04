using Zengo.Core;

namespace Zengo.Csv
{
    public class CsvConfig : Config
    {
        public bool OutputColumnLine { get; set; } = false;
        public bool ForceQuoteValues { get; set; } = false;
    }
}