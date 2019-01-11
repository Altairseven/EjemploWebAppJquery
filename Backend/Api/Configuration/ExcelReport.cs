using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using DocumentFormat.OpenXml;
//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using System.IO.Packaging;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Reflection;
using NPOI.HSSF.Util;

namespace Api.Configuration {
    public class ExcelReport {

        public IWorkbook WorkBook { get; set; }
        public Object Dataset { get; set; }

        public ExcelReport() {
            this.WorkBook = new XSSFWorkbook();
        }


        public Byte[] GetExcel<T>(Dictionary<string, ExcelColumn> Columns, List<T> DataSet) {

            using (var ms = new MemoryStream()) {

                IWorkbook wb = new XSSFWorkbook();

                //ISheet sheet1 = workbook.CreateSheet("Sheet1");

                //sheet1.AddMergedRegion(new CellRangeAddress(0, 0, 0, 10));
                //var rowIndex = 0;
                //IRow row = sheet1.CreateRow(rowIndex);
                //row.Height = 30 * 80;
                //row.CreateCell(0).SetCellValue("this is content");
                //sheet1.AutoSizeColumn(0);
                //rowIndex++;

                //var sheet2 = workbook.CreateSheet("Sheet2");
                //var style1 = workbook.CreateCellStyle();
                //style1.FillForegroundColor = HSSFColor.Blue.Index2;
                //style1.FillPattern = FillPattern.SolidForeground;

                //var style2 = workbook.CreateCellStyle();
                //style2.FillForegroundColor = HSSFColor.Yellow.Index2;
                //style2.FillPattern = FillPattern.SolidForeground;

                //var cell2 = sheet2.CreateRow(0).CreateCell(0);
                //cell2.CellStyle = style1;
                //cell2.SetCellValue(0);

                //cell2 = sheet2.CreateRow(1).CreateCell(0);
                //cell2.CellStyle = style2;
                //cell2.SetCellValue(1);

                ISheet sheet1 = wb.CreateSheet("Hoja 1");
                var rowIndex = 0;
                //Creates Headers 
                IRow row = sheet1.CreateRow(rowIndex);
                rowIndex++;
                var HeaderStyle = wb.CreateCellStyle();

                HeaderStyle.FillForegroundColor = HSSFColor.PaleBlue.Index;
                HeaderStyle.FillPattern = FillPattern.SolidForeground;
                HeaderStyle.BorderBottom = BorderStyle.Medium;
                HeaderStyle.BottomBorderColor = HSSFColor.Black.Index;
                HeaderStyle.Alignment = HorizontalAlignment.Center;

                var font = wb.CreateFont();
                font.FontHeightInPoints = 11;
                font.FontName = "Calibri";
                font.Boldweight = (short)FontBoldWeight.Bold;

                var cellIndex = 0;
                foreach (var header in Columns) {
                    var cell = row.CreateCell(cellIndex);
                    cell.CellStyle = HeaderStyle;
                    cell.CellStyle.SetFont(font);
                    cell.SetCellValue(header.Value.Name);
                    cellIndex++;
                }

                foreach (var item in DataSet) {
                    row = sheet1.CreateRow(rowIndex);
                    rowIndex++;
                    cellIndex = 0;

                    var style = wb.CreateCellStyle();
                    foreach (var Property in Columns) {
                        var cell = row.CreateCell(cellIndex);
                        cellIndex++;

                        var prop = item.GetType().GetProperty(Property.Key);
                        var val = prop.GetValue(item);
                        var propstr = prop.PropertyType.ToString();
                        switch (propstr) {
                            case "System.String": {
                                style = wb.CreateCellStyle();
                                style.WrapText = false;

                                cell.SetCellValue(Convert.ToString(val));



                                cell.CellStyle = style;



                                break;
                            }
                            case "System.Int16":
                            case "System.Nullable`1[System.Int16]":
                            case "System.Int32":
                            case "System.Nullable`1[System.Int32]":
                            case "System.Int64":
                            case "System.Nullable`1[System.Int64]":
                            case "System.Decimal":
                            case "System.Nullable`1[System.Decimal]": {
                                cell.SetCellType(CellType.Numeric);
                                if (val == null) {
                                    break;
                                }

                                cell.SetCellValue(Convert.ToDouble(val));


                                IDataFormat dataFormatCustom = wb.CreateDataFormat();
                                style = wb.CreateCellStyle();
                                style.WrapText = false;
                                if (Property.Value.Type.ToLower() == "decimal") {
                                    style.DataFormat = dataFormatCustom.GetFormat("#,##0.00");
                                }
                                else {
                                    style.DataFormat = dataFormatCustom.GetFormat("0");
                                }
                                cell.CellStyle = style;
                                break;
                            }
                            case "System.DateTime":
                            case "System.Nullable`1[System.DateTime]": {
                                if (val == null) {
                                    cell.SetCellType(CellType.Blank);
                                    break;
                                }
                                DateTime fecha = Convert.ToDateTime(val);

                                IDataFormat dataFormatCustom = wb.CreateDataFormat();
                                style = wb.CreateCellStyle();

                                style.Alignment = HorizontalAlignment.Center;


                                if (Property.Value.Type.ToLower() == "time")
                                    style.DataFormat = dataFormatCustom.GetFormat("hh:mm");
                                else
                                    style.DataFormat = dataFormatCustom.GetFormat("dd/mm/yyyy");

                                style.WrapText = false;
                                cell.SetCellValue(fecha);

                                cell.CellStyle = style;

                                break;
                            }


                            default:
                                break;
                        }




                        //cell.SetCellValue();
                    }
                }

                for (int i = 0; i < Columns.Count(); i++) {
                    sheet1.SetColumnWidth(i, Columns.ElementAt(i).Value.Width);
                }









                wb.Write(ms);

                return ms.ToArray();


            }



        }

    }

    public class ExcelColumn {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Width { get; set; }
    }
}

