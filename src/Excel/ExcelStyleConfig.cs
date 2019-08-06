namespace Zengo
{
    public class ExcelStyleConfig
    {
        public string BackgroundColor { get; set; } = null;
        public string FontColor { get; set; } = null;
        public string FontName { get; set; } = null;
        public double? FontSize { get; set; } = null;
        public bool? Bold { get; set; } = null;
        public bool? Italic { get; set; } = null;
        public ExcelBorderConfig Border { get; internal set; } = new ExcelBorderConfig();
    }
}