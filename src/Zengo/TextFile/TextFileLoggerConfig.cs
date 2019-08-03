using Zengo.Abstracts;

namespace Zengo.TextFile
{
    public class TextFileLoggerConfig : BaseLoggerConfig
    {
        public char HorizontalLineChar { get; set; } = '-';
        public string LineSeparatorFirst { get; set; } = "+-";
        public string LineSeparatorMiddle { get; set; } = "-+-";
        public string LineSeparatorLast { get; set; } = "-+";
        public string ValueSeparatorFirst { get; set; } = "| ";
        public string ValueSeparatorMiddle { get; set; } = " | ";
        public string ValueSeparatorLast { get; set; } = " |";
    }
}