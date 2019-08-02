namespace Zengo.Config
{
    public class PlaneTextLoggerConfig : LoggerConfig
    {
        public char HorizontalLineChar { get; set; } = '-';
        public string LineSeparatorFirst { get; set; } = "+-";
        public string LineSeparator { get; set; } = "-+-";
        public string LineSeparatorLast { get; set; } = "-+";
        public string ValueSeparatorFirst { get; set; } = "| ";
        public string ValueSeparator { get; set; } = " | ";
        public string ValueSeparatorLast { get; set; } = " |";
    }
}