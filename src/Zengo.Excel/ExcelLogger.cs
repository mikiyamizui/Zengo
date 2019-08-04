using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Zengo.Interfaces;

namespace Zengo.Excel
{
    internal class ExcelLogger<TDataAdapter> : ILogger
    {
        public IConfig Config => _config;
        private readonly ExcelConfig _config;

        public ExcelLogger(ExcelConfig config)
        {
            _config = config;
        }

        public void Write(IEnumerable<ITable> tables1, IEnumerable<ITable> tables2)
        {
            using (var wb = new XLWorkbook())
            {
                ApplyStyleConfig(wb.Style, _config.DefaultStyle);

                var tables = tables1.Zip(tables2, (table1, table2) => (table1, table2));

                tables.ToList().ForEach(_ =>
                {
                    var (table1, table2) = _;

                    var ws = wb.AddWorksheet(table1.Name);

                    var pos1 = ws.Cell(2, 2);
                    var range1 = InsertTable(ws, table1, pos1);

                    var pos2 = range1.LastRowUsed().FirstCellUsed().CellBelow(2);
                    var range2 = InsertTable(ws, table2, pos2);

                    var range = range1.Cells().Zip(range2.Cells(), (c1, c2) => (c1, c2));

                    foreach (var (cell1, cell2) in range)
                    {
                        if (cell1.Value.ToString() != cell2.Value.ToString())
                        {
                            ApplyStyleConfig(cell2.Style, _config.ChangedValueStyle);
                        }
                    }

                    ws.FirstColumn().Width = 10;

                    ws.ColumnsUsed().AdjustToContents();
                    //ws.RowsUsed().AdjustToContents();

                    var maxWidth = ws.ColumnsUsed().Max(r => r.Width);
                    var maxHeight = ws.RowsUsed().Max(c => c.Height);

                    ws.Columns().Width = 5;
                    ws.Rows().Height = 5;

                    ws.ColumnsUsed().Width = maxWidth * 1.1;
                    ws.RowsUsed().Height = maxHeight * 1.1;
                });

                var dateTime = tables1.Min(table => table.DateTime);
                var fileName = $"{dateTime.ToString(_config.FileNameFormat)}.xlsx";

                wb.SaveAs(fileName);
            }
        }

        private IXLRange InsertTable(IXLWorksheet ws, ITable table, IXLCell startCell)
        {
            var dateTimeHeaderRange = ws.Range(startCell, startCell.CellRight(table.Columns.Count - 1));
            var dateTimeHeaderStyle = dateTimeHeaderRange.Merge().SetValue(table.DateTime.ToString(_config.DateTimeFormat)).Style;
            dateTimeHeaderStyle.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            ApplyStyleConfig(dateTimeHeaderStyle, _config.DateTimeHeaderStyle);

            var sqlHeaderStartCell = dateTimeHeaderRange.FirstColumn().LastCellUsed().CellBelow();
            var sqlHeaderRange = ws.Range(sqlHeaderStartCell, sqlHeaderStartCell.CellRight(table.Columns.Count - 1));
            var sqlHeaderStyle = sqlHeaderRange.Merge().SetValue(table.Sql).Style;
            sqlHeaderStyle.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            ApplyStyleConfig(sqlHeaderStyle, _config.SqlHeaderStyle);

            var columns = table.Columns.Select(column => column.Name).ToArray();
            var columnHeaderStartCell = sqlHeaderRange.FirstColumn().LastCellUsed().CellBelow();
            var columnHeaderRange = columnHeaderStartCell.InsertData(new[] { columns });
            var columnHeaderStyle = columnHeaderRange.Style;
            columnHeaderStyle.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            ApplyStyleConfig(columnHeaderStyle, _config.ColumnHeaderStyle);

            var data = table.Rows.Select(row => row.Items.Select(item => item.Value).ToArray());
            var dataStartCell = columnHeaderRange.FirstColumn().LastCell().CellBelow();
            var dataRange = dataStartCell.InsertData(data);

            var columnRowNumber = columnHeaderRange.FirstRow().RowNumber();

            var oddLineDataStyle = dataRange.Rows(r => (r.RowNumber() - columnRowNumber) % 2 == 1).Style;
            oddLineDataStyle.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            ApplyStyleConfig(oddLineDataStyle, _config.OddLineStyle);

            var evenLineDataStyle = dataRange.Rows(r => (r.RowNumber() - columnRowNumber) % 2 == 0).Style;
            evenLineDataStyle.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            ApplyStyleConfig(evenLineDataStyle, _config.EvenLineStyle);

            var tableRange = ws.Range(columnHeaderStartCell, dataRange.LastCell());
            tableRange.CreateTable();
            tableRange.Style.Border.OutsideBorder = _config.TableOutlineBorderStyle;

            if (_config.AutoSortByMostLeftColumn)
            {
                dataRange.Sort();
            }

            return dataRange;
        }

        private void ApplyStyleConfig(IXLStyle style, ExcelStyleConfig config)
        {
            if (!string.IsNullOrEmpty(config.BackgroundColor))
                style.Fill.BackgroundColor = XLColor.FromHtml(config.BackgroundColor);

            if (!string.IsNullOrEmpty(config.FontColor))
                style.Font.FontColor = XLColor.FromHtml(config.FontColor);

            if (!string.IsNullOrEmpty(config.FontName))
                style.Font.FontName = config.FontName;

            if (config.FontSize != null)
                style.Font.FontSize = config.FontSize.Value;

            if (config.Bold != null)
                style.Font.Bold = config.Bold.Value;

            if (config.Italic != null)
                style.Font.Italic = config.Italic.Value;

            ApplyBorderConfig(style.Border, config.Border);
        }

        private void ApplyBorderConfig(IXLBorder border, ExcelBorderConfig config)
        {
            if (config.Top != null)
                border.TopBorder = config.Top.Value;

            if (config.Left != null)
                border.LeftBorder = config.Left.Value;

            if (config.Bottom != null)
                border.BottomBorder = config.Bottom.Value;

            if (config.Right != null)
                border.RightBorder = config.Right.Value;
        }
    }
}