using NPOI.SS.UserModel;

using SMO.AppCode.Class;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SMO.Helper
{
    public static class ExcelHelperBP
    {
        public static void InsertBodyTable(ref IWorkbook workbook,
            ref ISheet sheet,
            IList<IList<ExcelCellMeta>> metaTBody,
            int NUM_CELL,
            bool ignoreFirstColumn)
        {
            ReportUtilities.CreateRow(ref sheet, 0, NUM_CELL);

            ICellStyle styleCellDetail = workbook.CreateCellStyle();
            styleCellDetail.CloneStyleFrom(sheet.GetRow(8).Cells[0].CellStyle);
            styleCellDetail.WrapText = true;

            ICellStyle styleCellNumber = workbook.CreateCellStyle();
            styleCellNumber.CloneStyleFrom(styleCellDetail);
            styleCellNumber.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");

            ICellStyle styleCellBold = workbook.CreateCellStyle();
            styleCellBold.CloneStyleFrom(sheet.GetRow(7).Cells[0].CellStyle);
            styleCellBold.WrapText = true;
            var fontBold = workbook.CreateFont();
            fontBold.IsBold = true;
            fontBold.FontHeightInPoints = 11;
            fontBold.FontName = "Times New Roman";

            ICellStyle styleCellLastDetail = workbook.CreateCellStyle();
            styleCellLastDetail.CloneStyleFrom(sheet.GetRow(9).Cells[0].CellStyle);
            styleCellLastDetail.WrapText = true;
            styleCellLastDetail.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");

            var numRowCur = 8;
            foreach (var row in metaTBody)
            {
                var tempIgnoreColumn = ignoreFirstColumn;
                var columns = 0;
                ReportUtilities.CopyRow(ref sheet, 9, numRowCur);
                IRow rowCur = ReportUtilities.CreateRow(ref sheet, numRowCur, NUM_CELL);
                foreach (var cell in row)
                {
                    if (tempIgnoreColumn)
                    {
                        tempIgnoreColumn = false;
                        continue;
                    }
                    rowCur.Height = -1;

                    if (columns < NUM_CELL - 16 || columns == NUM_CELL - 1)
                    {
                        rowCur.Cells[columns].CellStyle = styleCellDetail;
                        var lstTexts = Regex.Split(cell.Content, "\t+");
                        if (lstTexts.Length > 2)
                        {
                            // display many line
                            var level = Regex.Matches(cell.Content, "\t").Count / lstTexts.Length;
                            var spacesLevel = string.Empty;
                            for (int i = 0; i <= level; i++)
                            {
                                spacesLevel += "    ";
                            }
                            var cellText = string.Empty;
                            for (int i = 0; i < lstTexts.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(lstTexts[i]))
                                {
                                    cellText += spacesLevel + lstTexts[i] + (i < lstTexts.Length - 1 ? Environment.NewLine : string.Empty);
                                }
                            }

                            rowCur.Cells[columns].SetCellValue(cellText);
                        }
                        else
                        {
                            rowCur.Cells[columns].SetCellValue(cell.Content.Replace("\t", "    "));
                        }
                    }
                    else
                    {
                        var lstTexts = Regex.Split(cell.Content, "\t+");
                        if (lstTexts.Length > 2 || !double.TryParse(lstTexts.Last().Replace(".", "").Replace(",", "."), out _))
                        {
                            rowCur.Cells[columns].CellStyle = styleCellDetail;
                            // display many line
                            var cellText = string.Empty;
                            for (int i = 0; i < lstTexts.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(lstTexts[i]))
                                {
                                    cellText += lstTexts[i] + (i < lstTexts.Length - 1 ? Environment.NewLine : string.Empty);
                                }
                            }

                            rowCur.Cells[columns].SetCellValue(cellText);
                        }
                        else
                        {
                            rowCur.Cells[columns].SetCellValue(UtilsCore.StringToDouble(cell.Content.Trim().Replace(".", "").Replace(",", ".")));
                            rowCur.Cells[columns].CellStyle = styleCellNumber;
                        }
                    }
                    columns++;
                }
                numRowCur++;
            }

            //Xóa dòng thừa cuối cùng khi tạo các dòng cho detail
            IRow rowLastDetail = ReportUtilities.CreateRow(ref sheet, numRowCur, NUM_CELL);
            ReportUtilities.DeleteRow(ref sheet, rowLastDetail);

            foreach (var cell in sheet.GetRow(numRowCur - 1).Cells)
            {
                cell.CellStyle = styleCellLastDetail;
            }

            rowLastDetail = ReportUtilities.CreateRow(ref sheet, numRowCur, NUM_CELL);
            ReportUtilities.DeleteRow(ref sheet, rowLastDetail);

            // set size for culumn
            for (int i = 1; i < NUM_CELL; i++)
            {
                sheet.AutoSizeColumn(i);
            }
        }

        public static void InsertHeaderTable(ref IWorkbook workbook,
            ref ISheet sheet,
            IList<IList<ExcelCellMeta>> metaTHeader,
            int NUM_CELL,
            bool ignoreFirstColumn)
        {
            ICellStyle styleCellHeader = workbook.CreateCellStyle();
            styleCellHeader.CloneStyleFrom(sheet.GetRow(7).Cells[0].CellStyle);
            styleCellHeader.WrapText = true;

            var numRowCur = 7;
            foreach (var row in metaTHeader)
            {
                var tempIgnoreColumn = ignoreFirstColumn;
                var columns = 0;
                //ReportUtilities.CopyRow(ref sheet, 8, numRowCur);
                IRow rowCur = ReportUtilities.CreateRow(ref sheet, numRowCur, NUM_CELL);
                foreach (var cell in row)
                {
                    if (tempIgnoreColumn)
                    {
                        tempIgnoreColumn = false;
                        continue;
                    }
                    rowCur.Height = -1;
                    rowCur.Cells[columns].CellStyle = styleCellHeader;
                    rowCur.Cells[columns].SetCellValue(cell.Content);
                    columns++;
                }
                numRowCur++;
            }

            //Xóa dòng thừa cuối cùng khi tạo các dòng cho detail
            //IRow rowLastDetail = ReportUtilities.CreateRow(ref sheet, numRowCur, NUM_CELL);
            //ReportUtilities.DeleteRow(ref sheet, rowLastDetail);

        }

        public static void InitHeaderFile(ref ISheet sheet, int year, string centerName, int? version, int NUM_CELL, string templateName, string unit, string name, decimal exchangeRate)
        {
            var rowHeader1 = ReportUtilities.CreateRow(ref sheet, 1, NUM_CELL);
            ReportUtilities.CreateCell(ref rowHeader1, NUM_CELL);
            rowHeader1.Cells[0].SetCellValue(rowHeader1.Cells[0].StringCellValue + $" {centerName}");
            rowHeader1.Cells[2].SetCellValue(name);

            var rowHeader2 = ReportUtilities.CreateRow(ref sheet, 2, NUM_CELL);
            ReportUtilities.CreateCell(ref rowHeader2, NUM_CELL);
            rowHeader2.Cells[0].SetCellValue(rowHeader2.Cells[0].StringCellValue + $" {year}");

            if (version > 0)
            {
                var rowHeader3 = ReportUtilities.CreateRow(ref sheet, 3, NUM_CELL);
                ReportUtilities.CreateCell(ref rowHeader3, NUM_CELL);
                rowHeader3.Cells[0].SetCellValue(rowHeader3.Cells[0].StringCellValue + $" {version}");
            }

            var rowHeader4 = ReportUtilities.CreateRow(ref sheet, 4, NUM_CELL);
            ReportUtilities.CreateCell(ref rowHeader4, NUM_CELL);

            rowHeader4.Cells[0].SetCellValue(templateName);

            var rowHeader5 = ReportUtilities.CreateRow(ref sheet, 5, NUM_CELL);
            ReportUtilities.CreateCell(ref rowHeader5, NUM_CELL);
            if (unit == "VND")
            {
                // hiển thị đơn vị đồng, nghìn đồng, triệu đồng
                if (exchangeRate == 1)
                {
                    rowHeader5.Cells[0].SetCellValue($"Đơn vị ({unit}): Đồng");
                }
                else if (exchangeRate == 1000)
                {
                    rowHeader5.Cells[0].SetCellValue($"Đơn vị ({unit}): Nghìn đồng");
                }
                else if (exchangeRate == 1000000)
                {
                    rowHeader5.Cells[0].SetCellValue($"Đơn vị ({unit}): Triệu đồng");
                }
                else
                {
                    rowHeader5.Cells[0].SetCellValue($"Đơn vị ({unit}): {exchangeRate.ToStringVnWithoutDecimal()}");
                }
            }
            else
            {
                rowHeader5.Cells[0].SetCellValue($"Đơn vị ({unit}): {exchangeRate.ToStringVnWithoutDecimal(true)}");
            }

        }

    }
}
