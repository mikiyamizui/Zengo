using System.Collections.Generic;
using System.Data;
using System.Linq;
using ClosedXML.Excel;
using Zengo.Interfaces;

namespace Zengo.Excel
{
    internal class ExcelLogger : ILogger
    {
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

                    var dateTime1 = table1.DateTime.ToString(_config.DateTimeHeaderStringFormat);
                    var dateTime2 = table2.DateTime.ToString(_config.DateTimeHeaderStringFormat);

                    var ws = wb.AddWorksheet(table1.Name);

                    var pos1 = ws.Cell(2, 2);
                    var range1 = InsertTable(ws, table1, pos1);
                    var zip1 = range1.RowsUsed().Zip(table1.Rows, (xlRow, zeRow) => (xlRow, zeRow))
                        .SelectMany(x => x.xlRow.CellsUsed().Zip(x.zeRow.Items, (cell, item) => (cell, item)));

                    var pos2 = range1.LastRowUsed().FirstCellUsed().CellBelow(2);
                    var range2 = InsertTable(ws, table2, pos2);
                    var zip2 = range2.RowsUsed().Zip(table2.Rows, (xlRow, zeRow) => (xlRow, zeRow))
                        .SelectMany(x => x.xlRow.CellsUsed().Zip(x.zeRow.Items, (cell, item) => (cell, item)));

                    var changed = false;
                    zip1.Zip(zip2, (z1, z2) => (z1, z2)).ToList().ForEach(z =>
                        {
                            var cell1 = z.z1.cell;
                            var cell2 = z.z2.cell;
                            var item1 = z.z1.item;
                            var item2 = z.z2.item;

                            if (item1.Value.ToString() != item2.Value.ToString())
                            {
                                changed = true;
                                ApplyStyleConfig(cell2.Style, _config.ChangedValueStyle);

                                var columnName = item2.Column.Name;
                                var value1 = Convert(item1.Value, item1.IsDBNull).ToString();
                                var value2 = Convert(item2.Value, item2.IsDBNull).ToString();

                                cell2.Comment
                                    .AddText($"{table1.Name}.{columnName}").AddNewLine()
                                    .AddText($"{value1} -> {value2}").AddNewLine()
                                    ;
                            }
                        });

                    ws.FirstColumn().Width = 10;

                    ws.ColumnsUsed().AdjustToContents();
                    //ws.RowsUsed().AdjustToContents();

                    var maxWidth = ws.ColumnsUsed().Max(r => r.Width);
                    var maxHeight = ws.RowsUsed().Max(c => c.Height);

                    ws.Columns().Width = 5;
                    ws.Rows().Height = 5;

                    ws.ColumnsUsed().Width = maxWidth * 1.1;
                    ws.RowsUsed().Height = maxHeight * 1.1;

                    if (changed)
                    {
                        if (!string.IsNullOrEmpty(_config.ChangedTabColor))
                            ws.SetTabColor(XLColor.FromHtml(_config.ChangedTabColor));

                        if (!string.IsNullOrEmpty(_config.ChangedWorksheetNamePrefix))
                            ws.Name = $"{_config.ChangedWorksheetNamePrefix} {ws.Name}";

                        if (!string.IsNullOrEmpty(_config.ChangedWorksheetNameSuffix))
                            ws.Name = $"{ws.Name} {_config.ChangedWorksheetNameSuffix}";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(_config.UnchangedTabColor))
                            ws.SetTabColor(XLColor.FromHtml(_config.UnchangedTabColor));
                    }

                    if (_config.ProtectSheet)
                    {
                        (!string.IsNullOrEmpty(_config.ProtectPassword)
                            ? ws.Protect(_config.ProtectPassword)
                            : ws.Protect())
                            .SetInsertColumns()
                            .SetDeleteColumns()
                            .SetInsertRows()
                            .SetDeleteRows()
                            ;
                    }
                });

                var dateTime = tables1.Min(table => table.DateTime);
                var fileName = string.Format($"{_config.FileNameFormat}.xlsx", dateTime);

                wb.SaveAs(fileName);
            }
        }

        private IXLRange InsertTable(IXLWorksheet ws, ITable table, IXLCell startCell)
        {
            var dateTimeHeaderRange = ws.Range(startCell, startCell.CellRight(table.Columns.Count - 1));
            var dateTimeHeaderStyle = dateTimeHeaderRange.Merge().SetValue(table.DateTime.ToString(_config.DateTimeHeaderStringFormat)).Style;
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

            if (_config.AutoSortByFirstColumn)
            {
                dataRange.Sort();
            }

            if (_config.GroupTableRows)
            {
                ws.Rows(tableRange.FirstRowUsed().RowNumber(), tableRange.LastRowUsed().RowNumber())
                    .Group();
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

        private object Convert(object value, bool isDBNull)
            => isDBNull ? _config.NullString : $"'{value}'";
    }
}