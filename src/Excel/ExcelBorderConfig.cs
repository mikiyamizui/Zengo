using ClosedXML.Excel;

namespace Zengo
{
    public class ExcelBorderConfig
    {
        public XLBorderStyleValues? Top { get; set; }
        public XLBorderStyleValues? Left { get; set; }
        public XLBorderStyleValues? Bottom { get; set; }
        public XLBorderStyleValues? Right { get; set; }
        public XLColor Color { get; set; }

        public XLBorderStyleValues Outline
        {
            set
            {
                Top = value;
                Left = value;
                Bottom = value;
                Right = value;
            }
        }
    }
}