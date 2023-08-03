using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using SMO.AppCode.Class;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SMO.Helper
{
    public static class ExcelHelper
    {
        public static ExcelMeta GetExcelMeta(string html)
        {
            var excelMeta = new ExcelMeta();
            try
            {
                XElement xElement = XElement.Parse(html);
                // Lấy thông tin từ thead
                foreach (var bodyElement in xElement.Elements("thead"))
                {
                    foreach (var trElement in bodyElement.Elements("tr"))
                    {
                        var lstMeta = new List<ExcelCellMeta>();
                        foreach (var tdElement in trElement.Elements("th"))
                        {
                            var meta = new ExcelCellMeta
                            {
                                Content = tdElement.Value,
                                StyleName = tdElement.Attribute("data-xls-class")?.Value,
                                ColumnIndex = UtilsCore.StringToInt(tdElement.Attribute("data-xls-col-index")?.Value),
                                ColSpan = UtilsCore.StringToInt(tdElement.Attribute("colspan")?.Value),
                                RowSpan = UtilsCore.StringToInt(tdElement.Attribute("rowspan")?.Value)
                            };
                            lstMeta.Add(meta);
                        }
                        excelMeta.MetaTHead.Add(lstMeta);
                    }
                }

                // Lấy thông tin từ tbody
                foreach (var bodyElement in xElement.Elements("tbody"))
                {
                    foreach (var trElement in bodyElement.Elements("tr"))
                    {
                        var lstMeta = new List<ExcelCellMeta>();
                        foreach (var tdElement in trElement.Elements("td"))
                        {
                            var meta = new ExcelCellMeta
                            {
                                Content = tdElement.Value,
                                StyleName = tdElement.Attribute("data-xls-class")?.Value,
                                ColumnIndex = UtilsCore.StringToInt(tdElement.Attribute("data-xls-col-index")?.Value),
                                ColSpan = UtilsCore.StringToInt(tdElement.Attribute("colspan")?.Value),
                                RowSpan = UtilsCore.StringToInt(tdElement.Attribute("rowspan")?.Value)
                            };
                            lstMeta.Add(meta);
                        }
                        if (trElement.Attribute("data-xls-row")?.Value == "header")
                        {
                            excelMeta.MetaTHead.Add(lstMeta);
                        }
                        else
                        {
                            excelMeta.MetaTBody.Add(lstMeta);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }

            return excelMeta;
        }

        internal static bool DownloadFile(ref MemoryStream outFileStream, string path, string html, IList<int> ignoreColumns, int startRow = 0)
        {
            try
            {
                var data = GetExcelMeta(html);
                IWorkbook _workbook;
                ISheet sheet;
                if (!string.IsNullOrEmpty(path))
                {
                    FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    _workbook = new XSSFWorkbook(fs);
                    fs.Close();
                    sheet = _workbook.GetSheetAt(0);
                }
                else
                {
                    _workbook = new XSSFWorkbook();
                    sheet = _workbook.CreateSheet("Sheet 1"); //Creating New Excel Sheet object
                }
                var NUM_CELL = data.MetaTHead.FirstOrDefault().Sum(x => x.ColSpan <= 0 ? 1 : x.ColSpan) - ignoreColumns.Count;

                var headerStyle = _workbook.CreateCellStyle(); //Formatting
                var headerFont = _workbook.CreateFont();
                headerFont.IsBold = true;
                headerFont.FontHeightInPoints = 12.5;
                headerStyle.SetFont(headerFont);

                headerStyle.BorderBottom = BorderStyle.Thin;
                headerStyle.BorderLeft = BorderStyle.Thin;
                headerStyle.BorderRight = BorderStyle.Thin;
                headerStyle.BorderTop = BorderStyle.Thin;

                var bodyStyle = _workbook.CreateCellStyle(); //Formatting
                var bodyFont = _workbook.CreateFont();
                bodyFont.IsBold = false;
                bodyFont.FontHeightInPoints = 11;
                bodyStyle.SetFont(bodyFont);

                bodyStyle.BorderLeft = BorderStyle.Thin;
                bodyStyle.BorderRight = BorderStyle.Thin;
                bodyStyle.BorderBottom = BorderStyle.Dotted;

                var lastBodyStyle = _workbook.CreateCellStyle();
                bodyStyle.SetFont(bodyFont);

                lastBodyStyle.BorderLeft = BorderStyle.Thin;
                lastBodyStyle.BorderRight = BorderStyle.Thin;
                lastBodyStyle.BorderBottom = BorderStyle.Thin;

                var ignoreColumnHeader = new List<int>(ignoreColumns);
                // write header
                foreach (var h in data.MetaTHead)
                {
                    //ReportUtilities.CopyRow(ref sheet, 1, startRow);
                    IRow rowCur = ReportUtilities.CreateRow(ref sheet, startRow, NUM_CELL);
                    rowCur.Height = -1;
                    int i = 0;
                    foreach (var cell in h)
                    {
                        if (ignoreColumnHeader.Contains(i))
                        {
                            ignoreColumnHeader.Remove(i);
                            continue;
                        }
                        rowCur.Cells[i].CellStyle = headerStyle;
                        rowCur.Cells[i++].SetCellValue(cell.Content);
                    }
                }

                startRow = startRow == 0 ? data.MetaTHead.Count : startRow;
                // write body
                foreach (var d in data.MetaTBody)
                {
                    //ReportUtilities.CopyRow(ref sheet, 7, startRow++);
                    IRow rowCur = ReportUtilities.CreateRow(ref sheet, startRow++, NUM_CELL);
                    rowCur.Height = -1;
                    int i = 0;
                    var ignoreColumnBody = new List<int>(ignoreColumns);
                    foreach (var cell in d)
                    {
                        if (ignoreColumnBody.Contains(i))
                        {
                            ignoreColumnBody.Remove(i);
                            continue;
                        }
                        rowCur.Cells[i].CellStyle = bodyStyle;
                        rowCur.Cells[i++].SetCellValue(cell.Content);
                    }
                }

                // set style for last row
                var lastRow = sheet.GetRow(sheet.LastRowNum);
                foreach (var cell in lastRow.Cells)
                {
                    cell.CellStyle = lastBodyStyle;
                }

                // set size for culumn
                for (int i = 0; i < NUM_CELL; i++)
                {
                    sheet.AutoSizeColumn(i);
                }

                _workbook.Write(outFileStream);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
