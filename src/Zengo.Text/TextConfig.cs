using Zengo.Abstracts;

namespace Zengo.Text
{
    public class TextConfig : ConfigBase
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