using ClosedXML.Excel;
using Zengo.Core;

namespace Zengo.Excel
{
    public class ExcelConfig : Config
    {
        public bool AutoSortByFirstColumn { get; set; } = false;

        public bool GroupTableRows { get; set; } = true;

        public string DateTimeHeaderStringFormat { get; set; } = "yyyy-MM-dd HH:mm:ss.ffff";

        public XLBorderStyleValues TableOutlineBorderStyle { get; set; } = XLBorderStyleValues.Thin;

        public string UnchangedTabColor { get; set; } = null;

        public string ChangedTabColor { get; set; } = "#f96";

        public string ChangedWorksheetNamePrefix { get; set; } = null;

        public string ChangedWorksheetNameSuffix { get; set; } = "(changed)";

        public bool ProtectSheet { get; set; } = true;

        public string ProtectPassword { get; set; } = null;

        public ExcelStyleConfig DefaultStyle { get; }
            = new ExcelStyleConfig
            {
                BackgroundColor = "#fff",
                FontColor = "#050505",
                FontName = "MS Gothic",
                FontSize = 10.5,
                Bold = false,
                Italic = false,
                Border =
                {
                    Outline = XLBorderStyleValues.None,
                }
            };

        public ExcelStyleConfig DateTimeHeaderStyle { get; }
            = new ExcelStyleConfig
            {
                FontSize = 12,
                Bold = true,
            };

        public ExcelStyleConfig SqlHeaderStyle { get; }
            = new ExcelStyleConfig()
            {
                BackgroundColor = "#fedcba",
                Border =
                {
                    Outline = XLBorderStyleValues.Thin,
                },
            };

        public ExcelStyleConfig ColumnHeaderStyle { get; }
            = new ExcelStyleConfig()
            {
                BackgroundColor = "#69f",
                Border =
                {
                    Outline = XLBorderStyleValues.Thin,
                },
            };

        public ExcelStyleConfig OddLineStyle { get; }
            = new ExcelStyleConfig()
            {
                BackgroundColor = "#f0f0ff",
                Border =
                {
                    Left = XLBorderStyleValues.Thin,
                    Right = XLBorderStyleValues.Thin,
                },
            };

        public ExcelStyleConfig EvenLineStyle { get; }
            = new ExcelStyleConfig()
            {
                BackgroundColor = "#e0e0ff",
                Border =
                {
                    Left = XLBorderStyleValues.Thin,
                    Right = XLBorderStyleValues.Thin,
                },
            };

        public ExcelStyleConfig ChangedValueStyle { get; }
            = new ExcelStyleConfig()
            {
                BackgroundColor = "#ffeecc",
                FontColor = "#900",
                Bold = true,
            };
    }
}