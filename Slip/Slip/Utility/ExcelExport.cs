using OfficeOpenXml;
using OfficeOpenXml.Style;
using Slip.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.EnterpriseServices.Internal;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.UI.WebControls;
using static OfficeOpenXml.ExcelErrorValue;

namespace Slip.Utility
{
    public class ExcelExport
    {
        public static string FourP_Daily_Slip_Export(List<FourP_Pridiction> List, string HeaderName)
        {
            try
            {
                var FileExcelName = HeaderName;
                string stReturnFileName = "";
                using (ExcelPackage p = new ExcelPackage())
                {
                    #region Company Detail on Header
                    p.Workbook.Properties.Author = HeaderName;
                    p.Workbook.Properties.Title = HeaderName;

                    p.Workbook.Worksheets.Add("Details");

                    ExcelWorksheet worksheet2 = p.Workbook.Worksheets[1];
                    worksheet2.Name = "Details";

                    worksheet2.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    #endregion

                    Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#449BCA");
                    Color RowColor = System.Drawing.ColorTranslator.FromHtml("#DEEDF5");
                    Color TotalRow = System.Drawing.ColorTranslator.FromHtml("#449BCA");

                    #region Header Name Declaration and Assign Values
                    if (List != null)
                    {
                        int _row = 1;
                        worksheet2.Cells[_row, 1].Value = "ID";
                        worksheet2.Cells[_row, 2].Value = "RPartWeight";
                        worksheet2.Cells[_row, 3].Value = "PolishWeight";
                        worksheet2.Cells[_row, 4].Value = "Model";
                        worksheet2.Cells[_row, 5].Value = "Shape";
                        worksheet2.Cells[_row, 6].Value = "Shift";
                        worksheet2.Cells[_row, 7].Value = "MachineNo";
                        worksheet2.Cells[_row, 8].Value = "Remarks";
                        worksheet2.Cells[_row, 9].Value = "Date";
                        worksheet2.Cells[_row, 10].Value = "LogID";
                        worksheet2.Cells[_row, 11].Value = "MainRemarks";

                        worksheet2.Cells[1, 1, _row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet2.Cells[1, 1, _row, 11].Style.Font.Bold = true;
                        worksheet2.Cells[1, 1, _row, 11].Style.Font.Size = 12;
                        worksheet2.Cells[1, 1, _row, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet2.Cells[1, 1, _row, 11].Style.Fill.BackgroundColor.SetColor(colFromHex);

                        for (int i = 0; i < List.Count; i++)
                        {
                            _row++;

                            worksheet2.Cells[_row, 1].Value = List[i].ID;
                            worksheet2.Cells[_row, 2].Value = List[i].RPartWeight;
                            worksheet2.Cells[_row, 3].Value = List[i].PolishWeight;
                            worksheet2.Cells[_row, 4].Value = List[i].Model;
                            worksheet2.Cells[_row, 5].Value = List[i].ShapeName;
                            worksheet2.Cells[_row, 6].Value = List[i].Shift;
                            worksheet2.Cells[_row, 7].Value = List[i].Machine_No;
                            worksheet2.Cells[_row, 8].Value = List[i].Remarks;
                            worksheet2.Cells[_row, 9].Value = List[i].Created;
                            worksheet2.Cells[_row, 10].Value = List[i].LogID;
                            worksheet2.Cells[_row, 11].Value = List[i].MainRemarks;
                        }


                        worksheet2.Cells[1, 1, _row, 11].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[1, 1, _row, 11].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[1, 1, _row, 11].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[1, 1, _row, 11].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        //AutoFit
                        for (int i = 1; i <= 13; i++)
                        {
                            worksheet2.Column(i).AutoFit();
                            worksheet2.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }
                    }

                    string Folderpath = RequestHelpers.GetConfigValue("ExcelFiles");
                    string filename = HeaderName + ".xlsx";
                    if (Directory.Exists(HttpContext.Current.Server.MapPath(Folderpath)) == false)
                    {
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(Folderpath));
                    }

                    stReturnFileName = HttpContext.Current.Server.MapPath(Folderpath + "\\" + filename);
                    Byte[] bin = p.GetAsByteArray();

                    System.IO.File.WriteAllBytes(stReturnFileName, bin);
                    #endregion
                }
                return stReturnFileName;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return "";
            }
        }
        public static string RPPrdSummaryExport(List<Rp_Prd_Summary> List, List<Rp_Prd_Summary> List1, List<Rp_Prd_Summary> List2, List<Rp_Prd_Summary> List3, List<Rp_Prd_Summary> List4, List<Rp_Prd_Summary> List5, List<Rp_Prd_Summary> List6, List<Rp_Prd_Summary> List7, List<Rp_Prd_Summary> List8, List<Rp_Prd_Summary> List9, List<Rp_Prd_Summary> List10, List<Rp_Prd_Summary> List11, string HeaderName, string Tital)
        {
            try
            {
                ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");

                var FileExcelName = "Export";
                string stReturnFileName = "";
                using (ExcelPackage p = new ExcelPackage())
                {
                    #region Company Detail on Header
                    p.Workbook.Properties.Author = "Prd. With Labour";
                    p.Workbook.Properties.Title = "Prd. With Labour";

                    ExcelWorksheet worksheet2 = p.Workbook.Worksheets.Add("Details");

                    worksheet2.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    #endregion

                    Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#2B3C4E");
                    Color colTotal = System.Drawing.ColorTranslator.FromHtml("#c2d69a");
                    Color colFinalTotal = System.Drawing.ColorTranslator.FromHtml("#d99795");
                    Color colTitleBG = System.Drawing.ColorTranslator.FromHtml("#fac090");
                    Color Yellow = System.Drawing.ColorTranslator.FromHtml("Yellow");
                    Color LightGray = System.Drawing.ColorTranslator.FromHtml("#DCDCDC");



                    #region Header Name Declaration and Assign Values
                    int _row = 1;
                    int _T2row = 1;
                    int Brow = 1;
                    int T2Brow = 1;
                    decimal PrdWeightTotal = 0;

                    worksheet2.Cells[_row, 1, _row, 10].Merge = true;
                    worksheet2.Cells[_row, 1, _row, 10].Value = Tital;
                    worksheet2.Cells[_row, 1, _row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet2.Cells[_row, 1, _row, 10].Style.Font.Bold = true;
                    worksheet2.Cells[_row, 1, _row, 10].Style.Font.Size = 16;
                    worksheet2.Cells[_row, 1, _row, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet2.Cells[_row, 1, _row, 10].Style.Fill.BackgroundColor.SetColor(colTitleBG);
                    worksheet2.Cells[Brow, 1, _row, 10].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[Brow, 1, _row, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[Brow, 1, _row, 10].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[Brow, 1, _row, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    if (List10.Count > 0)
                    {
                        _row = _row + 2;
                        _T2row = _row;
                        Brow = _row;

                        worksheet2.Cells[_row, 1].Value = "PCS";
                        worksheet2.Cells[_row, 2].Value = List10[0].Pcs;

                        _row++;

                        worksheet2.Cells[_row, 1].Value = "ROUGH CT";
                        worksheet2.Cells[_row, 2].Value = List10[0].RoughWeight;

                        _row++;

                        worksheet2.Cells[_row, 1].Value = "RECEIPE";
                        worksheet2.Cells[_row, 2].Value = List10[0].Receipe;

                        worksheet2.Cells[Brow, 1, _row, 1].Style.Font.Bold = true;
                        worksheet2.Cells[Brow, 1, _row, 1].Style.Font.Size = 12;

                        worksheet2.Cells[Brow, 1, _row, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 2].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    if (List11.Count > 0)
                    {
                        T2Brow = _T2row;

                        worksheet2.Cells[_T2row, 4].Value = "SizeCode";
                        worksheet2.Cells[_T2row, 5].Value = "Min PlateSize";
                        worksheet2.Cells[_T2row, 6].Value = "Max PlateSize";
                        worksheet2.Cells[_T2row, 4, _T2row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet2.Cells[_T2row, 4, _T2row, 6].Style.Font.Bold = true;
                        worksheet2.Cells[_T2row, 4, _T2row, 6].Style.Font.Size = 12;
                        worksheet2.Cells[_T2row, 4, _T2row, 6].Style.Font.Color.SetColor(Color.White);
                        worksheet2.Cells[_T2row, 4, _T2row, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet2.Cells[_T2row, 4, _T2row, 6].Style.Fill.BackgroundColor.SetColor(colFromHex);

                        for (int i = 0; i < List11.Count; i++)
                        {
                            _T2row++;
                            worksheet2.Cells[_T2row, 4].Value = List11[i].SizeCode;
                            worksheet2.Cells[_T2row, 5].Value = List11[i].MinPlateSize;
                            worksheet2.Cells[_T2row, 6].Value = List11[i].MaxPlateSize;
                        }

                        worksheet2.Cells[T2Brow, 4, _T2row, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[T2Brow, 4, _T2row, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[T2Brow, 4, _T2row, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[T2Brow, 4, _T2row, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    if (List.Count > 0)
                    {
                        if (_row > _T2row)
                        {
                            _row = _row + 2;
                            Brow = _row;
                        }
                        else
                        {
                            _row = _T2row + 2;
                            Brow = _row;
                        }

                        worksheet2.Cells[_row, 1, _row, 7].Merge = true;
                        worksheet2.Cells[_row, 1, _row, 7].Value = "Stone MM Wise";
                        worksheet2.Cells[_row, 1, _row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet2.Cells[_row, 1, _row, 7].Style.Font.Bold = true;
                        worksheet2.Cells[_row, 1, _row, 7].Style.Font.Size = 12;
                        worksheet2.Cells[_row, 1, _row, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet2.Cells[_row, 1, _row, 7].Style.Fill.BackgroundColor.SetColor(colTitleBG);

                        _row++;

                        worksheet2.Cells[_row, 1].Value = "2.00 - 2.50";
                        worksheet2.Cells[_row, 2].Value = "2.51 - 2.99";
                        worksheet2.Cells[_row, 3].Value = "3.00 - 3.50";
                        worksheet2.Cells[_row, 4].Value = "3.51 - 3.90";
                        worksheet2.Cells[_row, 5].Value = "3.91 - 3.99";
                        worksheet2.Cells[_row, 6].Value = "4.00 - 4.19";
                        worksheet2.Cells[_row, 7].Value = "4.20 to UP";
                        worksheet2.Cells[_row, 1, _row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet2.Cells[_row, 1, _row, 7].Style.Font.Bold = true;
                        worksheet2.Cells[_row, 1, _row, 7].Style.Font.Size = 12;
                        worksheet2.Cells[_row, 1, _row, 7].Style.Font.Color.SetColor(Color.White);
                        worksheet2.Cells[_row, 1, _row, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet2.Cells[_row, 1, _row, 7].Style.Fill.BackgroundColor.SetColor(colFromHex);

                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        for (int i = 0; i < List.Count; i++)
                        {
                            _row++;
                            worksheet2.Cells[_row, 1].Value = List[i].OnePcs + " ( " + List[i].OneWT + " )";
                            worksheet2.Cells[_row, 2].Value = List[i].TwoPcs + " ( " + List[i].TwoWT + " )";
                            worksheet2.Cells[_row, 3].Value = List[i].ThreePcs + " ( " + List[i].ThreeWT + " )";
                            worksheet2.Cells[_row, 4].Value = List[i].FourPcs + " ( " + List[i].FourWT + " )";
                            worksheet2.Cells[_row, 5].Value = List[i].FivePcs + " ( " + List[i].FiveWT + " )";
                            worksheet2.Cells[_row, 6].Value = List[i].SixPcs + " ( " + List[i].SixWT + " )";
                            worksheet2.Cells[_row, 7].Value = List[i].SevenPcs + " ( " + List[i].SevenWT + " )";
                        }
                        //---------< SET BORDER >----------//

                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    _T2row = _row;
                    T2Brow = _row;

                    if (List1.Count > 0)
                    {
                        _row = _row + 2;
                        Brow = _row;

                        worksheet2.Cells[_row, 1, _row, 4].Merge = true;
                        worksheet2.Cells[_row, 1, _row, 4].Value = "ROUGH DETAIL";
                        worksheet2.Cells[_row, 1, _row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet2.Cells[_row, 1, _row, 4].Style.Font.Bold = true;
                        worksheet2.Cells[_row, 1, _row, 4].Style.Font.Size = 12;
                        worksheet2.Cells[_row, 1, _row, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet2.Cells[_row, 1, _row, 4].Style.Fill.BackgroundColor.SetColor(colTitleBG);
                        worksheet2.Cells[_row, 1, _row, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[_row, 1, _row, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[_row, 1, _row, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[_row, 1, _row, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        _row++;

                        worksheet2.Cells[_row, 1, _row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet2.Cells[_row, 1, _row, 4].Style.Font.Bold = true;
                        worksheet2.Cells[_row, 1, _row, 4].Style.Font.Size = 12;
                        worksheet2.Cells[_row, 1, _row, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet2.Cells[_row, 1, _row, 4].Style.Fill.BackgroundColor.SetColor(colTotal);



                        worksheet2.Cells[_row, 1].Value = "Grade";
                        worksheet2.Cells[_row, 2].Value = "Pcs";
                        worksheet2.Cells[_row, 3].Value = "Weight";
                        worksheet2.Cells[_row, 4].Value = "%";


                        ExcelStyle cellStyleHeaderwrks1 = worksheet2.Cells[_row, 1, _row, 4].Style;
                        cellStyleHeaderwrks1.Font.Bold = true;
                        cellStyleHeaderwrks1.Font.Color.SetColor(Color.White);
                        cellStyleHeaderwrks1.Fill.PatternType = ExcelFillStyle.Solid;
                        cellStyleHeaderwrks1.Fill.BackgroundColor.SetColor(colFromHex);

                        _row++;

                        int Pcs = 0;
                        decimal Weight = 0;
                        decimal Per = 0;
                        for (int i = 0; i < List1.Count; i++)
                        {
                            worksheet2.Cells[_row, 1].Value = List1[i].GradNo;
                            worksheet2.Cells[_row, 2].Value = List1[i].Pcs;
                            worksheet2.Cells[_row, 3].Value = List1[i].Weight;
                            worksheet2.Cells[_row, 4].Value = List1[i].Per;
                            _row++;
                            Pcs = Pcs + Convert.ToInt32(List1[i].Pcs);
                            Weight = Weight + Convert.ToDecimal(List1[i].Weight);
                            PrdWeightTotal = PrdWeightTotal + Convert.ToDecimal(List1[i].Weight);
                            Per = Per + Convert.ToDecimal(List1[i].Per);

                        }
                        worksheet2.Cells[_row, 1].Value = "TOTAL";
                        worksheet2.Cells[_row, 2].Value = Pcs;
                        worksheet2.Cells[_row, 3].Value = Weight;
                        worksheet2.Cells[_row, 4].Value = Per;

                        worksheet2.Cells[_row, 1, _row, 4].Style.Font.Bold = true;
                        worksheet2.Cells[_row, 1, _row, 4].Style.Font.Size = 12;
                        worksheet2.Cells[_row, 1, _row, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet2.Cells[_row, 1, _row, 4].Style.Fill.BackgroundColor.SetColor(colTotal);
                        //---------< SET BORDER >----------//
                        worksheet2.Cells[Brow, 1, _row, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    if (List2.Count > 0)
                    {
                        for (int k = 0; k < List7.Count; k++)
                        {
                            _row = _row + 2;
                            Brow = _row;

                            worksheet2.Cells[_row, 1, _row, 4].Merge = true;
                            worksheet2.Cells[_row, 1, _row, 4].Value = "ROUGH DETAIL (" + List7[k].SType + ")";
                            worksheet2.Cells[_row, 1, _row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet2.Cells[_row, 1, _row, 4].Style.Font.Bold = true;
                            worksheet2.Cells[_row, 1, _row, 4].Style.Font.Size = 12;
                            worksheet2.Cells[_row, 1, _row, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet2.Cells[_row, 1, _row, 4].Style.Fill.BackgroundColor.SetColor(colTitleBG);
                            worksheet2.Cells[_row, 1, _row, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet2.Cells[_row, 1, _row, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet2.Cells[_row, 1, _row, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet2.Cells[_row, 1, _row, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            _row++;

                            worksheet2.Cells[_row, 1, _row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet2.Cells[_row, 1, _row, 4].Style.Font.Bold = true;
                            worksheet2.Cells[_row, 1, _row, 4].Style.Font.Size = 12;
                            worksheet2.Cells[_row, 1, _row, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet2.Cells[_row, 1, _row, 4].Style.Fill.BackgroundColor.SetColor(colTotal);



                            worksheet2.Cells[_row, 1].Value = "Grade";
                            worksheet2.Cells[_row, 2].Value = "Pcs";
                            worksheet2.Cells[_row, 3].Value = "Weight";
                            worksheet2.Cells[_row, 4].Value = "%";


                            ExcelStyle cellStyleHeaderwrks1 = worksheet2.Cells[_row, 1, _row, 4].Style;
                            cellStyleHeaderwrks1.Font.Bold = true;
                            cellStyleHeaderwrks1.Font.Color.SetColor(Color.White);
                            cellStyleHeaderwrks1.Fill.PatternType = ExcelFillStyle.Solid;
                            cellStyleHeaderwrks1.Fill.BackgroundColor.SetColor(colFromHex);

                            _row++;

                            for (int i = 0; i < List2.Count; i++)
                            {
                                if (List2[i].SType == List7[k].SType)
                                {
                                    worksheet2.Cells[_row, 1].Value = List2[i].GradNo;
                                    worksheet2.Cells[_row, 2].Value = List2[i].Pcs;
                                    worksheet2.Cells[_row, 3].Value = List2[i].Weight;
                                    worksheet2.Cells[_row, 4].Value = List2[i].Per;
                                    _row++;
                                }
                            }
                            for (int i = 0; i < List3.Count; i++)
                            {
                                if (List3[i].SType == List7[k].SType)
                                {
                                    worksheet2.Cells[_row, 1].Value = "Total";
                                    worksheet2.Cells[_row, 2].Value = List3[i].Pcs;
                                    worksheet2.Cells[_row, 3].Value = List3[i].Weight;
                                    worksheet2.Cells[_row, 4].Value = List3[i].Per;
                                }
                            }

                            worksheet2.Cells[_row, 1, _row, 4].Style.Font.Bold = true;
                            worksheet2.Cells[_row, 1, _row, 4].Style.Font.Size = 12;
                            worksheet2.Cells[_row, 1, _row, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet2.Cells[_row, 1, _row, 4].Style.Fill.BackgroundColor.SetColor(colTotal);
                            //---------< SET BORDER >-----//
                            worksheet2.Cells[Brow, 1, _row, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet2.Cells[Brow, 1, _row, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet2.Cells[Brow, 1, _row, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet2.Cells[Brow, 1, _row, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        }
                    }

                    if (List4 != null)
                    {
                        _T2row = _T2row + 2;
                        T2Brow = _T2row;

                        worksheet2.Cells[_T2row, 6, _T2row, 10].Merge = true;
                        worksheet2.Cells[_T2row, 6, _T2row, 10].Value = "PLANNING DETAIL";
                        worksheet2.Cells[_T2row, 6, _T2row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Font.Bold = true;
                        worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Font.Size = 12;
                        worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Fill.BackgroundColor.SetColor(colTitleBG);
                        worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        _T2row++;

                        worksheet2.Cells[_T2row, 6, _T2row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Font.Bold = true;
                        worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Font.Size = 12;
                        worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Fill.BackgroundColor.SetColor(colTotal);


                        worksheet2.Cells[_T2row, 6].Value = "Grade";
                        worksheet2.Cells[_T2row, 7].Value = "Pcs";
                        worksheet2.Cells[_T2row, 8].Value = "Weight";
                        worksheet2.Cells[_T2row, 9].Value = "%";
                        worksheet2.Cells[_T2row, 10].Value = "PerCT Value";



                        ExcelStyle cellStyleHeaderwrks1 = worksheet2.Cells[_T2row, 6, _T2row, 10].Style;
                        cellStyleHeaderwrks1.Font.Bold = true;
                        cellStyleHeaderwrks1.Font.Color.SetColor(Color.White);
                        cellStyleHeaderwrks1.Fill.PatternType = ExcelFillStyle.Solid;
                        cellStyleHeaderwrks1.Fill.BackgroundColor.SetColor(colFromHex);

                        _T2row++;

                        int Pcs = 0;
                        decimal Weight = 0;
                        decimal Per = 0;
                        for (int i = 0; i < List4.Count; i++)
                        {
                            worksheet2.Cells[_T2row, 6].Value = List4[i].GradNo;
                            worksheet2.Cells[_T2row, 7].Value = List4[i].Pcs;
                            worksheet2.Cells[_T2row, 8].Value = List4[i].Weight;
                            worksheet2.Cells[_T2row, 9].Value = List4[i].Per;
                            worksheet2.Cells[_T2row, 10].Value = List4[i].PerCTValue;
                            _T2row++;
                            Pcs = Pcs + Convert.ToInt32(List4[i].Pcs);
                            Weight = Weight + Convert.ToDecimal(List4[i].Weight);
                            Per = Per + Convert.ToDecimal(List4[i].Per);

                        }
                        worksheet2.Cells[_T2row, 6].Value = "TOTAL";
                        worksheet2.Cells[_T2row, 7].Value = Pcs;
                        worksheet2.Cells[_T2row, 8].Value = Weight;
                        worksheet2.Cells[_T2row, 9].Value = PrdWeightTotal > 0 ? ((Weight / PrdWeightTotal) * 100) : 0;

                        worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Font.Bold = true;
                        worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Font.Size = 12;
                        worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Fill.BackgroundColor.SetColor(colTotal);
                        //---------< SET BORDER >----------//
                        worksheet2.Cells[T2Brow, 6, _T2row, 10].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[T2Brow, 6, _T2row, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[T2Brow, 6, _T2row, 10].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[T2Brow, 6, _T2row, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    }

                    if (List5.Count > 0)
                    {
                        for (int k = 0; k < List7.Count; k++)
                        {
                            _T2row = _T2row + 2;
                            T2Brow = _T2row;

                            worksheet2.Cells[_T2row, 6, _T2row, 10].Merge = true;
                            worksheet2.Cells[_T2row, 6, _T2row, 10].Value = "PLANNING DETAIL (" + List7[k].SType + ")";
                            worksheet2.Cells[_T2row, 6, _T2row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Font.Bold = true;
                            worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Font.Size = 12;
                            worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Fill.BackgroundColor.SetColor(colTitleBG);
                            worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            _T2row++;

                            worksheet2.Cells[_T2row, 6, _T2row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Font.Bold = true;
                            worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Font.Size = 12;
                            worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Fill.BackgroundColor.SetColor(colTotal);


                            worksheet2.Cells[_T2row, 6].Value = "Grade";
                            worksheet2.Cells[_T2row, 7].Value = "Pcs";
                            worksheet2.Cells[_T2row, 8].Value = "Weight";
                            worksheet2.Cells[_T2row, 9].Value = "%";
                            worksheet2.Cells[_T2row, 10].Value = "PerCT Value";



                            ExcelStyle cellStyleHeaderwrks1 = worksheet2.Cells[_T2row, 6, _T2row, 10].Style;
                            cellStyleHeaderwrks1.Font.Bold = true;
                            cellStyleHeaderwrks1.Font.Color.SetColor(Color.White);
                            cellStyleHeaderwrks1.Fill.PatternType = ExcelFillStyle.Solid;
                            cellStyleHeaderwrks1.Fill.BackgroundColor.SetColor(colFromHex);

                            _T2row++;

                            for (int i = 0; i < List5.Count; i++)
                            {
                                if (List5[i].SType == List7[k].SType)
                                {
                                    worksheet2.Cells[_T2row, 6].Value = List5[i].GradNo;
                                    worksheet2.Cells[_T2row, 7].Value = List5[i].Pcs;
                                    worksheet2.Cells[_T2row, 8].Value = List5[i].Weight;
                                    worksheet2.Cells[_T2row, 9].Value = List5[i].Per;
                                    worksheet2.Cells[_T2row, 10].Value = List5[i].PerCTValue;
                                    _T2row++;
                                }
                            }
                            for (int i = 0; i < List6.Count; i++)
                            {
                                if (List6[i].SType == List7[k].SType)
                                {
                                    worksheet2.Cells[_T2row, 6].Value = "Total";
                                    worksheet2.Cells[_T2row, 7].Value = List6[i].Pcs;
                                    worksheet2.Cells[_T2row, 8].Value = List6[i].Weight;
                                    worksheet2.Cells[_T2row, 9].Value = List6[i].Per;
                                    worksheet2.Cells[_T2row, 10].Style.Numberformat.Format = "0.00";
                                }
                            }

                            worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Font.Bold = true;
                            worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Font.Size = 12;
                            worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet2.Cells[_T2row, 6, _T2row, 10].Style.Fill.BackgroundColor.SetColor(colTotal);
                            //---------< SET BORDER >----------//
                            worksheet2.Cells[T2Brow, 6, _T2row, 10].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet2.Cells[T2Brow, 6, _T2row, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet2.Cells[T2Brow, 6, _T2row, 10].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet2.Cells[T2Brow, 6, _T2row, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        }
                    }

                    if (List8 != null)
                    {
                        if (_row > _T2row)
                        {
                            _row = _row + 2;
                            Brow = _row;
                        }
                        else
                        {
                            _row = _T2row + 2;
                            Brow = _row;
                        }
                        worksheet2.Cells[_row, 1, _row, 6].Merge = true;
                        worksheet2.Cells[_row, 1, _row, 6].Value = "SHAPE WISE SUMMARY";
                        worksheet2.Cells[_row, 1, _row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet2.Cells[_row, 1, _row, 6].Style.Font.Bold = true;
                        worksheet2.Cells[_row, 1, _row, 6].Style.Font.Size = 12;
                        worksheet2.Cells[_row, 1, _row, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet2.Cells[_row, 1, _row, 6].Style.Fill.BackgroundColor.SetColor(colTitleBG);
                        worksheet2.Cells[_row, 1, _row, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[_row, 1, _row, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[_row, 1, _row, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[_row, 1, _row, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        _row++;

                        worksheet2.Cells[_row, 1].Value = "Shape";
                        worksheet2.Cells[_row, 2].Value = "Pcs";
                        worksheet2.Cells[_row, 3].Value = "Polish Wt";
                        worksheet2.Cells[_row, 4].Value = "Per %";
                        worksheet2.Cells[_row, 5].Value = "Labour";
                        worksheet2.Cells[_row, 6].Value = "Prc";

                        ExcelStyle cellStyleHeaderwrks11 = worksheet2.Cells[_row, 1, _row, 6].Style;
                        cellStyleHeaderwrks11.Font.Bold = true;
                        cellStyleHeaderwrks11.Font.Size = 12;
                        cellStyleHeaderwrks11.Font.Color.SetColor(Color.White);
                        cellStyleHeaderwrks11.Fill.PatternType = ExcelFillStyle.Solid;
                        cellStyleHeaderwrks11.Fill.BackgroundColor.SetColor(colFromHex);

                        for (int i = 0; i < List8.Count; i++)
                        {
                            _row++;
                            worksheet2.Cells[_row, 1].Value = List8[i].Shape;
                            worksheet2.Cells[_row, 2].Value = List8[i].Pcs;
                            worksheet2.Cells[_row, 3].Value = List8[i].Pweight;
                            worksheet2.Cells[_row, 4].Value = List8[i].Per;
                            worksheet2.Cells[_row, 5].Value = List8[i].PrdAmt;
                            worksheet2.Cells[_row, 6].Value = List8[i].PrcAmt;
                        }

                        _row++;

                        worksheet2.Cells[_row, 1].Value = List9[0].Shape;
                        worksheet2.Cells[_row, 2].Value = List9[0].Pcs;
                        worksheet2.Cells[_row, 3].Value = List9[0].Pweight;
                        worksheet2.Cells[_row, 4].Value = List9[0].Per;
                        worksheet2.Cells[_row, 5].Value = List9[0].PrdAmt;
                        worksheet2.Cells[_row, 6].Value = List9[0].PrcAmt;

                        ExcelStyle cellStyleHeaderwrks1 = worksheet2.Cells[_row, 1, _row, 6].Style;
                        cellStyleHeaderwrks1.Font.Size = 12;
                        cellStyleHeaderwrks1.Font.Bold = true;
                        //cellStyleHeaderwrks1.Font.Color.SetColor(Color.White);
                        cellStyleHeaderwrks1.Fill.PatternType = ExcelFillStyle.Solid;
                        cellStyleHeaderwrks1.Fill.BackgroundColor.SetColor(colTotal);


                        //---------< SET BORDER >----------//
                        worksheet2.Cells[Brow, 1, _row, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    }

                    //AutoFit
                    for (int i = 1; i <= 15; i++)
                    {
                        worksheet2.Column(i).AutoFit();
                        worksheet2.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    string Folderpath = RequestHelpers.GetConfigValue("ExcelFiles");
                    string filename = "Prd Summary.xlsx";
                    if (Directory.Exists(HttpContext.Current.Server.MapPath(Folderpath)) == false)
                    {
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(Folderpath));
                    }

                    stReturnFileName = HttpContext.Current.Server.MapPath(Folderpath + "\\" + filename);
                    Byte[] bin = p.GetAsByteArray();

                    System.IO.File.WriteAllBytes(stReturnFileName, bin);
                    #endregion
                }
                return stReturnFileName;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return "";
            }
        }
        public static string ExportProcessWiseTimingReport(List<ProcessWiseTiming> List, List<ProcessWiseTiming> List1, List<ProcessWiseTiming> List2, List<ProcessWiseTiming> List3, string Date, string TableNo, string headername)
        {
            try
            {
                ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");
                var FileExcelName = "Export";
                string stReturnFileName = "";
                using (ExcelPackage p = new ExcelPackage())
                {
                    #region Company Detail on Header
                    p.Workbook.Properties.Author = "Process Wise Timing";
                    p.Workbook.Properties.Title = "Process Wise Timing";

                    #endregion

                    Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#2B3C4E");
                    Color colTotal = System.Drawing.ColorTranslator.FromHtml("#c2d69a");
                    Color colFinalTotal = System.Drawing.ColorTranslator.FromHtml("#d99795");
                    Color colTitleBG = System.Drawing.ColorTranslator.FromHtml("#fac090");
                    Color Yellow = System.Drawing.ColorTranslator.FromHtml("Yellow");
                    Color LightGray = System.Drawing.ColorTranslator.FromHtml("#DCDCDC");


                    #region Header Name Declaration and Assign Values
                    int _row = 1;
                    int Brow = 1;

                    DateTime Today = DateTime.Now;

                    if (List.Count > 0)
                    {
                        ExcelWorksheet worksheet = p.Workbook.Worksheets.Add("Jumbo White");

                        worksheet.Cells[_row, 1, _row, 6].Merge = true;
                        worksheet.Cells[_row, 1, _row, 6].Value = List[0].Type + " (" + List[0].FinishDays + " Days )";

                        _row++;

                        worksheet.Cells[_row, 1, _row, 3].Merge = true;
                        worksheet.Cells[_row, 1, _row, 3].Value = List[0].FinishDays;

                        worksheet.Cells[_row, 4, _row, 6].Merge = true;
                        worksheet.Cells[_row, 4, _row, 6].Value = List[0].TableNo;

                        _row++;

                        worksheet.Cells[_row, 1, _row, 6].Merge = true;
                        worksheet.Cells[_row, 1, _row, 6].Value = Date;

                        _row++;

                        worksheet.Cells[_row, 1].Value = "Process";
                        worksheet.Cells[_row, 2].Value = "Days";
                        worksheet.Cells[_row, 3].Value = "ProcessDay";
                        worksheet.Cells[_row, 4].Value = "Total Pcs";
                        worksheet.Cells[_row, 5].Value = "On-Time %";
                        worksheet.Cells[_row, 6].Value = "Over-Time %";

                        worksheet.Cells[Brow, 1, _row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[Brow, 1, _row, 6].Style.Font.Bold = true;
                        worksheet.Cells[Brow, 1, _row, 6].Style.Font.Size = 12;
                        worksheet.Cells[Brow, 1, _row, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[Brow, 1, _row, 6].Style.Fill.BackgroundColor.SetColor(colTitleBG);

                        decimal? OnTime;
                        decimal? OverTime;

                        for (int i = 0; i < List.Count; i++)
                        {
                            _row++;
                            worksheet.Cells[_row, 1].Value = List[i].Process;
                            worksheet.Cells[_row, 2].Value = List[i].Days;
                            worksheet.Cells[_row, 3].Value = List[i].ProcessDay;
                            worksheet.Cells[_row, 4].Value = List[i].TotalPcs;

                            OnTime = List[i].ONTIMEPer;
                            worksheet.Cells[_row, 5].Value = OnTime + "%";
                            worksheet.Cells[_row, 5].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            worksheet.Cells[_row, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml(getOnTimeStyle(OnTime)));

                            OverTime = List[i].OVERDAYPer;
                            worksheet.Cells[_row, 6].Value = OverTime + "%";
                            worksheet.Cells[_row, 6].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            worksheet.Cells[_row, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml(getOverTimeStyle(OverTime)));
                        }

                        //---------< SET BORDER >----------//
                        worksheet.Cells[Brow, 1, _row, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[Brow, 1, _row, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[Brow, 1, _row, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[Brow, 1, _row, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        //AutoFit
                        for (int i = 1; i <= 6; i++)
                        {
                            worksheet.Column(i).AutoFit();
                            worksheet.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }
                    }


                    if (List1.Count > 0)
                    {

                        ExcelWorksheet worksheet2 = p.Workbook.Worksheets.Add("Jumbo Color");

                        _row = 1;
                        Brow = 1;

                        worksheet2.Cells[_row, 1, _row, 6].Merge = true;
                        worksheet2.Cells[_row, 1, _row, 6].Value = List1[0].Type + " (" + List1[0].FinishDays + " Days )";

                        _row++;

                        worksheet2.Cells[_row, 1, _row, 3].Merge = true;
                        worksheet2.Cells[_row, 1, _row, 3].Value = List1[0].FinishDays;

                        worksheet2.Cells[_row, 4, _row, 6].Merge = true;
                        worksheet2.Cells[_row, 4, _row, 6].Value = List1[0].TableNo;

                        _row++;

                        worksheet2.Cells[_row, 1, _row, 6].Merge = true;
                        worksheet2.Cells[_row, 1, _row, 6].Value = Date;

                        _row++;

                        worksheet2.Cells[_row, 1].Value = "Process";
                        worksheet2.Cells[_row, 2].Value = "Days";
                        worksheet2.Cells[_row, 3].Value = "ProcessDay";
                        worksheet2.Cells[_row, 4].Value = "Total Pcs";
                        worksheet2.Cells[_row, 5].Value = "On-Time %";
                        worksheet2.Cells[_row, 6].Value = "Over-Time %";

                        worksheet2.Cells[Brow, 1, _row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet2.Cells[Brow, 1, _row, 6].Style.Font.Bold = true;
                        worksheet2.Cells[Brow, 1, _row, 6].Style.Font.Size = 12;
                        worksheet2.Cells[Brow, 1, _row, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet2.Cells[Brow, 1, _row, 6].Style.Fill.BackgroundColor.SetColor(colTitleBG);

                        decimal? OnTime;
                        decimal? OverTime;

                        for (int i = 0; i < List1.Count; i++)
                        {
                            _row++;
                            worksheet2.Cells[_row, 1].Value = List1[i].Process;
                            worksheet2.Cells[_row, 2].Value = List1[i].Days;
                            worksheet2.Cells[_row, 3].Value = List1[i].ProcessDay;
                            worksheet2.Cells[_row, 4].Value = List1[i].TotalPcs;
                            OnTime = List1[i].ONTIMEPer;
                            worksheet2.Cells[_row, 5].Value = OnTime + "%";
                            worksheet2.Cells[_row, 5].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            worksheet2.Cells[_row, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml(getOnTimeStyle(OnTime)));

                            OverTime = List1[i].OVERDAYPer;
                            worksheet2.Cells[_row, 6].Value = OverTime + "%";
                            worksheet2.Cells[_row, 6].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            worksheet2.Cells[_row, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml(getOverTimeStyle(OverTime)));
                        }

                        //---------< SET BORDER >----------//
                        worksheet2.Cells[Brow, 1, _row, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        //AutoFit
                        for (int i = 1; i <= 6; i++)
                        {
                            worksheet2.Column(i).AutoFit();
                            worksheet2.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }
                    }

                    if (List2.Count > 0)
                    {
                        ExcelWorksheet worksheet3 = p.Workbook.Worksheets.Add("Small White");

                        _row = 1;
                        Brow = 1;

                        worksheet3.Cells[_row, 1, _row, 6].Merge = true;
                        worksheet3.Cells[_row, 1, _row, 6].Value = List2[0].Type + " (" + List2[0].FinishDays + " Days )";

                        _row++;

                        worksheet3.Cells[_row, 1, _row, 3].Merge = true;
                        worksheet3.Cells[_row, 1, _row, 3].Value = List2[0].FinishDays;

                        worksheet3.Cells[_row, 4, _row, 6].Merge = true;
                        worksheet3.Cells[_row, 4, _row, 6].Value = List2[0].TableNo;

                        _row++;

                        worksheet3.Cells[_row, 1, _row, 6].Merge = true;
                        worksheet3.Cells[_row, 1, _row, 6].Value = Date;

                        _row++;

                        worksheet3.Cells[_row, 1].Value = "Process";
                        worksheet3.Cells[_row, 2].Value = "Days";
                        worksheet3.Cells[_row, 3].Value = "ProcessDay";
                        worksheet3.Cells[_row, 4].Value = "Total Pcs";
                        worksheet3.Cells[_row, 5].Value = "On-Time %";
                        worksheet3.Cells[_row, 6].Value = "Over-Time %";

                        worksheet3.Cells[Brow, 1, _row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet3.Cells[Brow, 1, _row, 6].Style.Font.Bold = true;
                        worksheet3.Cells[Brow, 1, _row, 6].Style.Font.Size = 12;
                        worksheet3.Cells[Brow, 1, _row, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet3.Cells[Brow, 1, _row, 6].Style.Fill.BackgroundColor.SetColor(colTitleBG);

                        decimal? OnTime;
                        decimal? OverTime;

                        for (int i = 0; i < List2.Count; i++)
                        {
                            _row++;
                            worksheet3.Cells[_row, 1].Value = List2[i].Process;
                            worksheet3.Cells[_row, 2].Value = List2[i].Days;
                            worksheet3.Cells[_row, 3].Value = List2[i].ProcessDay;
                            worksheet3.Cells[_row, 4].Value = List2[i].TotalPcs;
                            OnTime = List2[i].ONTIMEPer;
                            worksheet3.Cells[_row, 5].Value = OnTime + "%";
                            worksheet3.Cells[_row, 5].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            worksheet3.Cells[_row, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml(getOnTimeStyle(OnTime)));

                            OverTime = List2[i].OVERDAYPer;
                            worksheet3.Cells[_row, 6].Value = OverTime + "%";
                            worksheet3.Cells[_row, 6].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            worksheet3.Cells[_row, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml(getOverTimeStyle(OverTime)));
                        }

                        //---------< SET BORDER >----------//
                        worksheet3.Cells[Brow, 1, _row, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet3.Cells[Brow, 1, _row, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet3.Cells[Brow, 1, _row, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet3.Cells[Brow, 1, _row, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        //AutoFit
                        for (int i = 1; i <= 6; i++)
                        {
                            worksheet3.Column(i).AutoFit();
                            worksheet3.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }
                    }

                    if (List3.Count > 0)
                    {
                        ExcelWorksheet worksheet4 = p.Workbook.Worksheets.Add("Small Color");

                        _row = 1;
                        Brow = 1;

                        worksheet4.Cells[_row, 1, _row, 6].Merge = true;
                        worksheet4.Cells[_row, 1, _row, 6].Value = List3[0].Type + " (" + List3[0].FinishDays + " Days )";

                        _row++;

                        worksheet4.Cells[_row, 1, _row, 3].Merge = true;
                        worksheet4.Cells[_row, 1, _row, 3].Value = List3[0].FinishDays;

                        worksheet4.Cells[_row, 4, _row, 6].Merge = true;
                        worksheet4.Cells[_row, 4, _row, 6].Value = List3[0].TableNo;

                        _row++;

                        worksheet4.Cells[_row, 1, _row, 6].Merge = true;
                        worksheet4.Cells[_row, 1, _row, 6].Value = Date;

                        _row++;

                        worksheet4.Cells[_row, 1].Value = "Process";
                        worksheet4.Cells[_row, 2].Value = "Days";
                        worksheet4.Cells[_row, 3].Value = "ProcessDay";
                        worksheet4.Cells[_row, 4].Value = "Total Pcs";
                        worksheet4.Cells[_row, 5].Value = "On-Time %";
                        worksheet4.Cells[_row, 6].Value = "Over-Time %";

                        worksheet4.Cells[Brow, 1, _row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet4.Cells[Brow, 1, _row, 6].Style.Font.Bold = true;
                        worksheet4.Cells[Brow, 1, _row, 6].Style.Font.Size = 12;
                        worksheet4.Cells[Brow, 1, _row, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet4.Cells[Brow, 1, _row, 6].Style.Fill.BackgroundColor.SetColor(colTitleBG);

                        decimal? OnTime;
                        decimal? OverTime;

                        for (int i = 0; i < List3.Count; i++)
                        {
                            _row++;
                            worksheet4.Cells[_row, 1].Value = List3[i].Process;
                            worksheet4.Cells[_row, 2].Value = List3[i].Days;
                            worksheet4.Cells[_row, 3].Value = List3[i].ProcessDay;
                            worksheet4.Cells[_row, 4].Value = List3[i].TotalPcs;
                            OnTime = List3[i].ONTIMEPer;
                            worksheet4.Cells[_row, 5].Value = OnTime + "%";
                            worksheet4.Cells[_row, 5].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            worksheet4.Cells[_row, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml(getOnTimeStyle(OnTime)));

                            OverTime = List3[i].OVERDAYPer;
                            worksheet4.Cells[_row, 6].Value = OverTime + "%";
                            worksheet4.Cells[_row, 6].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            worksheet4.Cells[_row, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml(getOverTimeStyle(OverTime)));
                        }

                        //---------< SET BORDER >----------//
                        worksheet4.Cells[Brow, 1, _row, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet4.Cells[Brow, 1, _row, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet4.Cells[Brow, 1, _row, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet4.Cells[Brow, 1, _row, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        //AutoFit
                        for (int i = 1; i <= 6; i++)
                        {
                            worksheet4.Column(i).AutoFit();
                            worksheet4.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }
                    }

                    string Folderpath = RequestHelpers.GetConfigValue("ExcelFiles");
                    string filename = "ProcessWiseTiming (" + TableNo + ").xlsx";
                    if (Directory.Exists(HttpContext.Current.Server.MapPath(Folderpath)) == false)
                    {
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(Folderpath));
                    }

                    stReturnFileName = HttpContext.Current.Server.MapPath(Folderpath + "\\" + filename);
                    Byte[] bin = p.GetAsByteArray();

                    System.IO.File.WriteAllBytes(stReturnFileName, bin);
                    #endregion
                }
                return stReturnFileName;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return "";
            }
        }
        public static string getOnTimeStyle(decimal? OnTime)
        {
            string OnTimeBG = "#FFFFFF";

            if (OnTime == 0)
            {
                OnTimeBG = "#FFFFFF";
            }
            else if (OnTime > 1 && OnTime <= 20)
            {
                OnTimeBG = "#a9e394";
            }
            else if (OnTime > 20 && OnTime <= 40)
            {
                OnTimeBG = "#8acd72";
            }
            else if (OnTime > 40 && OnTime <= 60)
            {
                OnTimeBG = "#78c95b";
            }
            else if (OnTime > 60 && OnTime <= 80)
            {
                OnTimeBG = "#6dd14b";
            }
            else if (OnTime > 80)
            {
                OnTimeBG = "#48c31d";
            }

            return OnTimeBG;
        }
        public static string getOverTimeStyle(decimal? OverTime)
        {
            string OverTimeBG = "#FFFFFF";

            if (OverTime == 0)
            {
                OverTimeBG = "#FFFFFF";
            }
            else if (OverTime > 1 && OverTime <= 20)
            {
                OverTimeBG = "#f2aa84";
            }
            else if (OverTime > 20 && OverTime <= 40)
            {
                OverTimeBG = "#f59969";
            }
            else if (OverTime > 40 && OverTime <= 60)
            {
                OverTimeBG = "#f58d57";
            }
            else if (OverTime > 60 && OverTime <= 80)
            {
                OverTimeBG = "#f18046";
            }
            else if (OverTime > 80)
            {
                OverTimeBG = "#ff7128";
            }

            return OverTimeBG;
        }
        public static string ExportDailyStockExcelGen(List<PolishData> PolishData, List<RoughData> RoughData, List<VipulbhaiData> VipulbhaiData, List<ProcessData> JumboWhiteData, List<ProcessData> SmallWhiteData, List<ProcessData> ColorData, List<ProcessData> UnderProcessData, List<TransferData> TransferData, List<UnderProcessDetail> PrcWiseUnderProcessData, string ReportDate, string HeaderName)
        {
            try
            {
                ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");

                var FileExcelName = "Export";
                string stReturnFileName = "";
                using (ExcelPackage p = new ExcelPackage())
                {
                    #region Company Detail on Header
                    p.Workbook.Properties.Author = "Daily Report";
                    p.Workbook.Properties.Title = "Daily Report";

                    ExcelWorksheet worksheet2 = p.Workbook.Worksheets.Add("DailyReport");

                    #endregion

                    Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#2B3C4E");
                    Color colTotal = System.Drawing.ColorTranslator.FromHtml("#c2d69a");
                    Color colFinalTotal = System.Drawing.ColorTranslator.FromHtml("#d99795");
                    Color colTitleBG = System.Drawing.ColorTranslator.FromHtml("#fac090");
                    Color Yellow = System.Drawing.ColorTranslator.FromHtml("Yellow");
                    Color LightGray = System.Drawing.ColorTranslator.FromHtml("#DCDCDC");

                    string bgColors = "#DAEEF3"; /// "#ffffff";

                    int _row = 1;
                    int Brow = 1;
                    int _t1row = 1;
                    int t1Brow = 1;

                    worksheet2.Cells[_row, 1, _row, 8].Merge = true;
                    worksheet2.Cells[_row, 1, _row, 8].Value = ReportDate;
                    worksheet2.Cells[_row, 1, _row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet2.Cells[_row, 1, _row, 8].Style.Font.Bold = true;
                    worksheet2.Cells[_row, 1, _row, 8].Style.Font.Size = 14;
                    worksheet2.Cells[_row, 1, _row, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet2.Cells[_row, 1, _row, 8].Style.Fill.BackgroundColor.SetColor(colTitleBG);
                    worksheet2.Cells[_row, 1, _row, 8].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[_row, 1, _row, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[_row, 1, _row, 8].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[_row, 1, _row, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    _row = _row + 2;
                    Brow = _row;
                    _t1row = _row;
                    t1Brow = _row;

                    #region Polish
                    worksheet2.Cells[_row, 1].Value = "POLISH";
                    worksheet2.Cells[_row, 1, _row, 3].Merge = true;
                    worksheet2.Cells[_row, 1, _row, 3].Style.Font.Bold = true;
                    worksheet2.Cells[_row, 1, _row, 3].Style.Font.Size = 14;
                    worksheet2.Cells[_row, 1, _row, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet2.Cells[_row, 1, _row, 3].Style.Fill.BackgroundColor.SetColor(colTitleBG);

                    _row++;

                    worksheet2.Cells[_row, 1].Value = "";
                    worksheet2.Cells[_row, 2].Value = "POLISH";
                    worksheet2.Cells[_row, 3].Value = "MUMBAI SUBMIT";
                    worksheet2.Cells[_row, 1, _row, 3].Style.Font.Bold = true;
                    worksheet2.Cells[_row, 1, _row, 3].Style.Font.Size = 12;
                    worksheet2.Cells[_row, 1, _row, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet2.Cells[_row, 1, _row, 3].Style.Fill.BackgroundColor.SetColor(colTotal);

                    if (PolishData.Count > 0)
                    {
                        for (int i = 0; i < PolishData.Count; i++)
                        {
                            _row++;
                            worksheet2.Cells[_row, 1].Value = PolishData[i].Status;
                            worksheet2.Cells[_row, 2].Value = PolishData[i].PolishWeight;
                            worksheet2.Cells[_row, 3].Value = "";

                            if (PolishData[i].Status == "TOTAL")
                            {
                                worksheet2.Cells[_row, 1, _row, 3].Style.Font.Bold = true;
                            }
                        }
                    }
                    worksheet2.Cells[Brow, 1, _row, 3].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[Brow, 1, _row, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[Brow, 1, _row, 3].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[Brow, 1, _row, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    #endregion Polish

                    #region Rough
                    worksheet2.Cells[_t1row, 5].Value = "ROUGH";
                    worksheet2.Cells[_t1row, 5, _t1row, 6].Merge = true;
                    worksheet2.Cells[_t1row, 5, _t1row, 6].Style.Font.Bold = true;
                    worksheet2.Cells[_t1row, 5, _t1row, 6].Style.Font.Size = 14;
                    worksheet2.Cells[_t1row, 5, _t1row, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet2.Cells[_t1row, 5, _t1row, 6].Style.Fill.BackgroundColor.SetColor(colTitleBG);

                    _t1row++;

                    worksheet2.Cells[_t1row, 5].Value = "";
                    worksheet2.Cells[_t1row, 6].Value = "ROUGH";
                    worksheet2.Cells[_t1row, 5, _t1row, 6].Style.Font.Bold = true;
                    worksheet2.Cells[_t1row, 5, _t1row, 6].Style.Font.Size = 12;
                    worksheet2.Cells[_t1row, 5, _t1row, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet2.Cells[_t1row, 5, _t1row, 6].Style.Fill.BackgroundColor.SetColor(colTotal);

                    if (RoughData.Count > 0)
                    {
                        for (int i = 0; i < RoughData.Count; i++)
                        {
                            _t1row++;
                            worksheet2.Cells[_t1row, 5].Value = RoughData[i].Status;
                            worksheet2.Cells[_t1row, 6].Value = RoughData[i].RoughWeight;

                            if (RoughData[i].Status == "TOTAL")
                            {
                                worksheet2.Cells[_row, 5, _row, 6].Style.Font.Bold = true;
                            }
                        }
                    }
                    worksheet2.Cells[t1Brow, 5, _t1row, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[t1Brow, 5, _t1row, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[t1Brow, 5, _t1row, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[t1Brow, 5, _t1row, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    #endregion Rough

                    if (_t1row > _row)
                    {
                        _row = _t1row + 2;
                    }
                    else
                    {
                        _row = _row + 2;
                    }
                    Brow = _row;

                    #region Vipulbhai
                    worksheet2.Cells[_row, 1].Value = "VIPULBHAI";
                    worksheet2.Cells[_row, 1, _row, 6].Merge = true;
                    worksheet2.Cells[_row, 1, _row, 6].Style.Font.Bold = true;
                    worksheet2.Cells[_row, 1, _row, 6].Style.Font.Size = 14;
                    worksheet2.Cells[_row, 1, _row, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet2.Cells[_row, 1, _row, 6].Style.Fill.BackgroundColor.SetColor(colTitleBG);

                    _row++;

                    worksheet2.Cells[_row, 1].Value = "Status";
                    worksheet2.Cells[_row, 2].Value = "JW";
                    worksheet2.Cells[_row, 3].Value = "JC";
                    worksheet2.Cells[_row, 4].Value = "SW";
                    worksheet2.Cells[_row, 5].Value = "SC";
                    worksheet2.Cells[_row, 6].Value = "Total";
                    worksheet2.Cells[_row, 1, _row, 6].Style.Font.Bold = true;
                    worksheet2.Cells[_row, 1, _row, 6].Style.Font.Size = 12;
                    worksheet2.Cells[_row, 1, _row, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet2.Cells[_row, 1, _row, 6].Style.Fill.BackgroundColor.SetColor(colTotal);

                    if (VipulbhaiData.Count > 0)
                    {
                        for (int i = 0; i < VipulbhaiData.Count; i++)
                        {
                            _row++;
                            worksheet2.Cells[_row, 1].Value = VipulbhaiData[i].Status;
                            worksheet2.Cells[_row, 2].Value = VipulbhaiData[i].JW;
                            worksheet2.Cells[_row, 3].Value = VipulbhaiData[i].JC;
                            worksheet2.Cells[_row, 4].Value = VipulbhaiData[i].SW;
                            worksheet2.Cells[_row, 5].Value = VipulbhaiData[i].SC;
                            worksheet2.Cells[_row, 6].Value = VipulbhaiData[i].Total;

                            if (VipulbhaiData[i].Status == "TOTAL")
                            {
                                worksheet2.Cells[_row, 1, _row, 6].Style.Font.Bold = true;
                            }
                        }
                    }
                    worksheet2.Cells[Brow, 1, _row, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[Brow, 1, _row, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[Brow, 1, _row, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[Brow, 1, _row, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    #endregion Vipulbhai

                    #region Under Process (Jumbo) (White)
                    _row = _row + 2;
                    Brow = _row;

                    worksheet2.Cells[_row, 1, _row, 7].Merge = true;
                    worksheet2.Cells[_row, 1, _row, 7].Value = "Under Process (Jumbo) (White)";
                    worksheet2.Cells[_row, 1, _row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet2.Cells[_row, 1, _row, 7].Style.Font.Bold = true;
                    worksheet2.Cells[_row, 1, _row, 7].Style.Font.Size = 14;

                    _row++;

                    worksheet2.Cells[_row, 1, _row, 2].Merge = true;
                    worksheet2.Cells[_row, 1, _row, 2].Value = "Process";
                    worksheet2.Cells[_row, 3].Value = "RPartWeight";
                    worksheet2.Cells[_row, 4].Value = "POLISH PRD CT";
                    worksheet2.Cells[_row, 5].Value = "%";
                    worksheet2.Cells[_row, 6].Value = "POLISH CT";
                    worksheet2.Cells[_row, 7].Value = "%";
                    worksheet2.Cells[_row, 1, _row, 7].Style.Font.Bold = true;
                    worksheet2.Cells[_row, 1, _row, 7].Style.Font.Size = 12;
                    worksheet2.Cells[_row, 1, _row, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet2.Cells[_row, 1, _row, 7].Style.Fill.BackgroundColor.SetColor(colTitleBG);

                    if (JumboWhiteData.Count > 0)
                    {
                        for (int i = 0; i < JumboWhiteData.Count; i++)
                        {
                            _row++;
                            worksheet2.Cells[_row, 1, _row, 2].Merge = true;
                            worksheet2.Cells[_row, 1, _row, 2].Value = JumboWhiteData[i].Process;
                            worksheet2.Cells[_row, 3].Value = JumboWhiteData[i].RPartWeight;
                            worksheet2.Cells[_row, 4].Value = JumboWhiteData[i].PolishPrd;
                            worksheet2.Cells[_row, 5].Value = JumboWhiteData[i].PrdPer;
                            worksheet2.Cells[_row, 6].Value = JumboWhiteData[i].PolishWeight;
                            worksheet2.Cells[_row, 7].Value = JumboWhiteData[i].PolishPer;
                            if (JumboWhiteData[i].Process == "Total_Rough_Stocks")
                            {
                                worksheet2.Cells[_row, 1, _row, 7].Style.Font.Bold = true;
                                worksheet2.Cells[_row, 1, _row, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet2.Cells[_row, 1, _row, 7].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(bgColors));
                            }
                        }

                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }
                    #endregion Under Process (Jumbo) (White)

                    #region Under Process (Small) (White)
                    _row = _row + 2;
                    Brow = _row;

                    worksheet2.Cells[_row, 1, _row, 7].Merge = true;
                    worksheet2.Cells[_row, 1, _row, 7].Value = "Under Process (Small) (White)";
                    worksheet2.Cells[_row, 1, _row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet2.Cells[_row, 1, _row, 7].Style.Font.Bold = true;
                    worksheet2.Cells[_row, 1, _row, 7].Style.Font.Size = 14;

                    _row++;

                    worksheet2.Cells[_row, 1, _row, 2].Merge = true;
                    worksheet2.Cells[_row, 1, _row, 2].Value = "Process";
                    worksheet2.Cells[_row, 3].Value = "RPartWeight";
                    worksheet2.Cells[_row, 4].Value = "POLISH PRD CT";
                    worksheet2.Cells[_row, 5].Value = "%";
                    worksheet2.Cells[_row, 6].Value = "POLISH CT";
                    worksheet2.Cells[_row, 7].Value = "%";
                    worksheet2.Cells[_row, 1, _row, 7].Style.Font.Bold = true;
                    worksheet2.Cells[_row, 1, _row, 7].Style.Font.Size = 12;
                    worksheet2.Cells[_row, 1, _row, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet2.Cells[_row, 1, _row, 7].Style.Fill.BackgroundColor.SetColor(colTitleBG);

                    if (SmallWhiteData.Count > 0)
                    {
                        for (int i = 0; i < SmallWhiteData.Count; i++)
                        {
                            _row++;
                            worksheet2.Cells[_row, 1, _row, 2].Merge = true;
                            worksheet2.Cells[_row, 1, _row, 2].Value = SmallWhiteData[i].Process;
                            worksheet2.Cells[_row, 3].Value = SmallWhiteData[i].RPartWeight;
                            worksheet2.Cells[_row, 4].Value = SmallWhiteData[i].PolishPrd;
                            worksheet2.Cells[_row, 5].Value = SmallWhiteData[i].PrdPer;
                            worksheet2.Cells[_row, 6].Value = SmallWhiteData[i].PolishWeight;
                            worksheet2.Cells[_row, 7].Value = SmallWhiteData[i].PolishPer;
                            if (SmallWhiteData[i].Process == "Total_Rough_Stocks")
                            {
                                worksheet2.Cells[_row, 1, _row, 7].Style.Font.Bold = true;
                                worksheet2.Cells[_row, 1, _row, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet2.Cells[_row, 1, _row, 7].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(bgColors));
                            }
                        }

                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }
                    #endregion Under Process (Small) (White)

                    #region Under Process (Color)
                    _row = _row + 2;
                    Brow = _row;

                    worksheet2.Cells[_row, 1, _row, 7].Merge = true;
                    worksheet2.Cells[_row, 1, _row, 7].Value = "Under Process (Color)";
                    worksheet2.Cells[_row, 1, _row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet2.Cells[_row, 1, _row, 7].Style.Font.Bold = true;
                    worksheet2.Cells[_row, 1, _row, 7].Style.Font.Size = 14;

                    _row++;

                    worksheet2.Cells[_row, 1, _row, 2].Merge = true;
                    worksheet2.Cells[_row, 1, _row, 2].Value = "Process";
                    worksheet2.Cells[_row, 3].Value = "RPartWeight";
                    worksheet2.Cells[_row, 4].Value = "POLISH PRD CT";
                    worksheet2.Cells[_row, 5].Value = "%";
                    worksheet2.Cells[_row, 6].Value = "POLISH CT";
                    worksheet2.Cells[_row, 7].Value = "%";
                    worksheet2.Cells[_row, 1, _row, 7].Style.Font.Bold = true;
                    worksheet2.Cells[_row, 1, _row, 7].Style.Font.Size = 12;
                    worksheet2.Cells[_row, 1, _row, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet2.Cells[_row, 1, _row, 7].Style.Fill.BackgroundColor.SetColor(colTitleBG);

                    if (ColorData.Count > 0)
                    {
                        for (int i = 0; i < ColorData.Count; i++)
                        {
                            _row++;
                            worksheet2.Cells[_row, 1, _row, 2].Merge = true;
                            worksheet2.Cells[_row, 1, _row, 2].Value = ColorData[i].Process;
                            worksheet2.Cells[_row, 3].Value = ColorData[i].RPartWeight;
                            worksheet2.Cells[_row, 4].Value = ColorData[i].PolishPrd;
                            worksheet2.Cells[_row, 5].Value = ColorData[i].PrdPer;
                            worksheet2.Cells[_row, 6].Value = ColorData[i].PolishWeight;
                            worksheet2.Cells[_row, 7].Value = ColorData[i].PolishPer;
                            if (ColorData[i].Process == "Total_Rough_Stocks")
                            {
                                worksheet2.Cells[_row, 1, _row, 7].Style.Font.Bold = true;
                                worksheet2.Cells[_row, 1, _row, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet2.Cells[_row, 1, _row, 7].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(bgColors));
                            }
                        }

                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }
                    #endregion Under Process (Color)

                    #region Under Process 
                    _row = _row + 2;
                    Brow = _row;

                    worksheet2.Cells[_row, 1, _row, 7].Merge = true;
                    worksheet2.Cells[_row, 1, _row, 7].Value = "Under Process";
                    worksheet2.Cells[_row, 1, _row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet2.Cells[_row, 1, _row, 7].Style.Font.Bold = true;
                    worksheet2.Cells[_row, 1, _row, 7].Style.Font.Size = 14;

                    _row++;

                    worksheet2.Cells[_row, 1, _row, 2].Merge = true;
                    worksheet2.Cells[_row, 1, _row, 2].Value = "Process";
                    worksheet2.Cells[_row, 3].Value = "RPartWeight";
                    worksheet2.Cells[_row, 4].Value = "POLISH PRD CT";
                    worksheet2.Cells[_row, 5].Value = "%";
                    worksheet2.Cells[_row, 6].Value = "POLISH CT";
                    worksheet2.Cells[_row, 7].Value = "%";
                    worksheet2.Cells[_row, 1, _row, 7].Style.Font.Bold = true;
                    worksheet2.Cells[_row, 1, _row, 7].Style.Font.Size = 12;
                    worksheet2.Cells[_row, 1, _row, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet2.Cells[_row, 1, _row, 7].Style.Fill.BackgroundColor.SetColor(colTitleBG);

                    if (UnderProcessData.Count > 0)
                    {
                        for (int i = 0; i < UnderProcessData.Count; i++)
                        {
                            _row++;
                            worksheet2.Cells[_row, 1, _row, 2].Merge = true;
                            worksheet2.Cells[_row, 1, _row, 2].Value = UnderProcessData[i].Process;
                            worksheet2.Cells[_row, 3].Value = UnderProcessData[i].RPartWeight;
                            worksheet2.Cells[_row, 4].Value = UnderProcessData[i].PolishPrd;
                            worksheet2.Cells[_row, 5].Value = UnderProcessData[i].PrdPer;
                            worksheet2.Cells[_row, 6].Value = UnderProcessData[i].PolishWeight;
                            worksheet2.Cells[_row, 7].Value = UnderProcessData[i].PolishPer;
                            if (UnderProcessData[i].Process == "Total_Rough_Stocks")
                            {
                                worksheet2.Cells[_row, 1, _row, 7].Style.Font.Bold = true;
                                worksheet2.Cells[_row, 1, _row, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet2.Cells[_row, 1, _row, 7].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(bgColors));
                            }
                        }

                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }
                    #endregion Under Process

                    #region Transfer
                    _row = _row + 2;
                    Brow = _row;

                    worksheet2.Cells[_row, 1, _row, 7].Merge = true;
                    worksheet2.Cells[_row, 1, _row, 7].Value = "Transfer";
                    worksheet2.Cells[_row, 1, _row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet2.Cells[_row, 1, _row, 7].Style.Font.Bold = true;
                    worksheet2.Cells[_row, 1, _row, 7].Style.Font.Size = 14;

                    _row++;

                    worksheet2.Cells[_row, 1, _row, 2].Merge = true;
                    worksheet2.Cells[_row, 1, _row, 2].Value = "Process";
                    worksheet2.Cells[_row, 3].Value = "RPartWeight";
                    worksheet2.Cells[_row, 4].Value = "POLISH PRD CT";
                    worksheet2.Cells[_row, 5].Value = "%";
                    worksheet2.Cells[_row, 6].Value = "POLISH CT";
                    worksheet2.Cells[_row, 7].Value = "%";
                    worksheet2.Cells[_row, 1, _row, 7].Style.Font.Bold = true;
                    worksheet2.Cells[_row, 1, _row, 7].Style.Font.Size = 12;
                    worksheet2.Cells[_row, 1, _row, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet2.Cells[_row, 1, _row, 7].Style.Fill.BackgroundColor.SetColor(colTitleBG);

                    if (TransferData.Count > 0)
                    {
                        for (int i = 0; i < TransferData.Count; i++)
                        {
                            _row++;
                            worksheet2.Cells[_row, 1, _row, 2].Merge = true;
                            worksheet2.Cells[_row, 1, _row, 2].Value = TransferData[i].SizeCode;
                            worksheet2.Cells[_row, 3].Value = TransferData[i].RPartWeight;
                            worksheet2.Cells[_row, 4].Value = TransferData[i].PolishPrd;
                            worksheet2.Cells[_row, 5].Value = TransferData[i].PrdPer;
                            worksheet2.Cells[_row, 6].Value = TransferData[i].PolishWeight;
                            worksheet2.Cells[_row, 7].Value = TransferData[i].PolishPer;
                            if (TransferData[i].SizeCode == "TOTAL")
                            {
                                worksheet2.Cells[_row, 1, _row, 7].Style.Font.Bold = true;
                                worksheet2.Cells[_row, 1, _row, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet2.Cells[_row, 1, _row, 7].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(bgColors));
                            }
                        }

                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[Brow, 1, _row, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }
                    #endregion Transfer

                    #region Process Wise UnderProcess
                    _row = _row + 2;
                    Brow = _row;

                    worksheet2.Cells[_row, 1, _row, 8].Merge = true;
                    worksheet2.Cells[_row, 1, _row, 8].Value = "Process Wise UnderProcess";
                    worksheet2.Cells[_row, 1, _row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet2.Cells[_row, 1, _row, 8].Style.Font.Bold = true;
                    worksheet2.Cells[_row, 1, _row, 8].Style.Font.Size = 14;

                    _row++;

                    worksheet2.Cells[_row, 1].Value = "SrNo";
                    worksheet2.Cells[_row, 2].Value = "Process";
                    worksheet2.Cells[_row, 3].Value = "Pcs";
                    worksheet2.Cells[_row, 4].Value = "ORG Rough Ct.";
                    worksheet2.Cells[_row, 5].Value = "Polish Ct.";
                    worksheet2.Cells[_row, 6].Value = "Cur Polish Ct.";
                    worksheet2.Cells[_row, 7].Value = "DiffPer";
                    worksheet2.Cells[_row, 8].Value = "Ideal Ct.";
                    worksheet2.Cells[_row, 1, _row, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet2.Cells[_row, 1, _row, 8].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#2B3C4E"));
                    worksheet2.Cells[_row, 1, _row, 8].Style.Font.Color.SetColor(Color.White);

                    worksheet2.Cells[_row, 1, _row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet2.Cells[_row, 1, _row, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet2.Cells[_row, 1, _row, 8].Style.Font.Bold = true;
                    worksheet2.Cells[_row, 1, _row, 8].Style.Font.Size = 12;

                    int SrNo = 0;

                    if (PrcWiseUnderProcessData.Count > 0)
                    {
                        for (int i = 0; i < PrcWiseUnderProcessData.Count; i++)
                        {
                            SrNo++;
                            _row++;

                            worksheet2.Cells[_row, 1].Value = SrNo;
                            worksheet2.Cells[_row, 2].Value = PrcWiseUnderProcessData[i].Process;
                            worksheet2.Cells[_row, 3].Value = PrcWiseUnderProcessData[i].Pcs;
                            worksheet2.Cells[_row, 4].Value = PrcWiseUnderProcessData[i].RPartWeight;
                            worksheet2.Cells[_row, 5].Value = PrcWiseUnderProcessData[i].PolishWeight;
                            worksheet2.Cells[_row, 6].Value = PrcWiseUnderProcessData[i].Cur_Polish_Ct;
                            worksheet2.Cells[_row, 7].Value = PrcWiseUnderProcessData[i].DiffPer;
                            worksheet2.Cells[_row, 8].Value = PrcWiseUnderProcessData[i].Ideal_Ct;
                        }
                    }

                    worksheet2.Cells[_row - 2, 1, _row, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet2.Cells[_row - 2, 1, _row, 7].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(bgColors));
                    ExcelStyle cellStyleHeaderwrkss = worksheet2.Cells[_row - 2, 1, _row, 7].Style;
                    cellStyleHeaderwrkss.Font.Bold = true;
                    cellStyleHeaderwrkss.Font.Color.SetColor(Color.Black);

                    worksheet2.Cells[Brow, 1, _row, 8].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[Brow, 1, _row, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[Brow, 1, _row, 8].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[Brow, 1, _row, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    #endregion Process Wise UnderProcess


                    //AutoFit
                    for (int i = 1; i <= 20; i++)
                    {
                        worksheet2.Column(i).AutoFit();
                        worksheet2.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    string Folderpath = RequestHelpers.GetConfigValue("ExcelFiles");
                    string filename = "DailyReport.xlsx";
                    if (Directory.Exists(HttpContext.Current.Server.MapPath(Folderpath)) == false)
                    {
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(Folderpath));
                    }

                    stReturnFileName = HttpContext.Current.Server.MapPath(Folderpath + "\\" + filename);
                    Byte[] bin = p.GetAsByteArray();

                    System.IO.File.WriteAllBytes(stReturnFileName, bin);

                }
                return stReturnFileName;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return "";
            }
        }
        public static string ExportPricing(List<colorList> ColorList, List<clarityList> ClarityList, List<sizeList> Sizes, List<TRN_Pricing> List, string Shape, string HeaderName)
        {
            try
            {
                ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");

                var FileExcelName = HeaderName;
                string stReturnFileName = "";
                using (ExcelPackage p = new ExcelPackage())
                {
                    #region Company Detail on Header
                    p.Workbook.Properties.Author = HeaderName;
                    p.Workbook.Properties.Title = HeaderName;

                    ExcelWorksheet worksheet2 = p.Workbook.Worksheets.Add("Details");

                    worksheet2.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    #endregion

                    Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#449BCA");
                    Color RowColor = System.Drawing.ColorTranslator.FromHtml("#DEEDF5");
                    Color TotalRow = System.Drawing.ColorTranslator.FromHtml("#449BCA");

                    #region Header Name Declaration and Assign Values
                    // normalize inputs
                    var colors = ColorList ?? new List<colorList>();
                    var clarities = ClarityList ?? new List<clarityList>();
                    var sizes = Sizes ?? new List<sizeList>();
                    var predictions = List ?? new List<TRN_Pricing>();

                    int HeaderWidth = 3 + (clarities.Count * 2);

                    worksheet2.Cells[1, 1, 1, HeaderWidth].Merge = true;
                    worksheet2.Cells[1, 1, 1, HeaderWidth].Value = Shape;
                    worksheet2.Cells[1, 1, 1, HeaderWidth].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet2.Cells[1, 1, 1, HeaderWidth].Style.Font.Bold = true;
                    worksheet2.Cells[1, 1, 1, HeaderWidth].Style.Font.Size = 16;
                    worksheet2.Cells[1, 1, 1, HeaderWidth].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet2.Cells[1, 1, 1, HeaderWidth].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    worksheet2.Cells[1, 1, 1, HeaderWidth].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[1, 1, 1, HeaderWidth].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[1, 1, 1, HeaderWidth].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[1, 1, 1, HeaderWidth].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    // layout parameters
                    const int maxBlocksPerRow = 2;           // two blocks per visual row as in the provided image
                    const int horizontalSpacing = 1;         // empty columns between blocks
                    int startTopRow = 3;                     // top row to start placing blocks

                    // convert sizes to list to support indexing
                    var sizeList = sizes.ToList();

                    int totalRowsOfBlocks = (int)Math.Ceiling(sizeList.Count / (double)maxBlocksPerRow);

                    for (int blockRow = 0; blockRow < totalRowsOfBlocks; blockRow++)
                    {
                        // get sizes for this visual row (max 2)
                        var rowSizes = sizeList.Skip(blockRow * maxBlocksPerRow).Take(maxBlocksPerRow).ToList();

                        // compute block heights and place blocks left-to-right
                        int maxHeightInRow = 0;
                        for (int blockIndex = 0; blockIndex < rowSizes.Count; blockIndex++)
                        {
                            var s = rowSizes[blockIndex];
                            if (s == null) continue;

                            // determine block column start
                            int blockWidth = 1 + Math.Max(clarities.Count, 1); // "Color" column + clarity columns
                            int headerStartCol = blockIndex * (blockWidth + horizontalSpacing) + 1;
                            int headerEndCol = headerStartCol + blockWidth - 1;

                            // merged size header (top of block)
                            worksheet2.Cells[startTopRow, headerStartCol, startTopRow, headerEndCol].Merge = true;
                            worksheet2.Cells[startTopRow, headerStartCol].Value = s.Size;
                            worksheet2.Cells[startTopRow, headerStartCol].Style.Font.Bold = true;
                            worksheet2.Cells[startTopRow, headerStartCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            

                            // sub headers: Color + clarity names
                            int subHeaderRow = startTopRow + 1;
                            worksheet2.Cells[subHeaderRow, headerStartCol].Value = "Color";
                            worksheet2.Cells[subHeaderRow, headerStartCol].Style.Font.Bold = true;
                            worksheet2.Cells[subHeaderRow, headerStartCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet2.Cells[subHeaderRow, headerStartCol].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                            int colIndex = headerStartCol + 1;
                            foreach (var c in clarities)
                            {
                                worksheet2.Cells[subHeaderRow, colIndex].Value = c?.Clarity ?? "";
                                worksheet2.Cells[subHeaderRow, colIndex].Style.Font.Bold = true;
                                worksheet2.Cells[subHeaderRow, colIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet2.Cells[subHeaderRow, colIndex].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                                colIndex++;
                            }

                            // data rows: one row per color
                            int firstDataRow = startTopRow + 2;
                            int rowIndex = firstDataRow;
                            foreach (var clr in colors)
                            {
                                worksheet2.Cells[rowIndex, headerStartCol].Value = clr?.Color ?? "";
                                worksheet2.Cells[rowIndex, headerStartCol].Style.Font.Bold = true;
                                worksheet2.Cells[rowIndex, headerStartCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet2.Cells[rowIndex, headerStartCol].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                                colIndex = headerStartCol + 1;

                                foreach (var cl in clarities)
                                {
                                    // find price for this size/color/clarity
                                    var price = predictions
                                        .Where(x =>
                                            string.Equals(x.Color, clr?.Color, StringComparison.OrdinalIgnoreCase) &&
                                            string.Equals(x.Clarity, cl?.Clarity, StringComparison.OrdinalIgnoreCase) &&
                                            string.Equals(x.Size, s.Size, StringComparison.OrdinalIgnoreCase))
                                        .Select(x => x.FinalRate)
                                        .FirstOrDefault();

                                    // show empty cell for zero or missing price
                                    decimal priceDec;
                                    var show = (price == null) ? "" :
                                        (decimal.TryParse(Convert.ToString(price), out priceDec) && priceDec == 0m) ? "" : Convert.ToString(price);

                                    worksheet2.Cells[rowIndex, colIndex].Value = show;
                                    colIndex++;
                                }

                                rowIndex++;
                            }

                            // add a blank row after colors (visual spacing)
                            int blockHeight = (rowIndex - startTopRow);
                            if (blockHeight > maxHeightInRow) maxHeightInRow = blockHeight;

                            // set borders and minor styling for the block range
                            try
                            {
                                var blockRange = worksheet2.Cells[startTopRow, headerStartCol, startTopRow + blockHeight - 1, headerEndCol];
                                blockRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                blockRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                blockRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                blockRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }
                            catch
                            {
                                // ignore styling errors
                            }

                            // AutoFit columns used by this block
                            try
                            {
                                for (int c = headerStartCol; c <= headerEndCol; c++)
                                {
                                    worksheet2.Column(c).AutoFit();
                                    worksheet2.Column(c).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                }
                            }
                            catch
                            {
                                // ignore AutoFit errors
                            }
                        } // end blocks in the visual row

                        // advance top row by the tallest block in this visual row + gap
                        startTopRow += maxHeightInRow + 1;
                    } // end block rows
                    #endregion

                    string Folderpath = RequestHelpers.GetConfigValue("ExcelFiles");
                    string filename = HeaderName + ".xlsx";
                    if (Directory.Exists(HttpContext.Current.Server.MapPath(Folderpath)) == false)
                    {
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(Folderpath));
                    }

                    stReturnFileName = HttpContext.Current.Server.MapPath(Folderpath + "\\" + filename);
                    Byte[] bin = p.GetAsByteArray();

                    System.IO.File.WriteAllBytes(stReturnFileName, bin);
                }
                return stReturnFileName;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return "";
            }
        }
        public static string ExportPricingPivot(List<colorList> ColorList, List<clarityList> ClarityList, List<sizeList> Sizes, List<TRN_Pricing> List, string Shape, string HeaderName)
        {
            try
            {
                ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");

                string stReturnFileName = "";
                using (ExcelPackage p = new ExcelPackage())
                {
                    #region Company Detail on Header
                    p.Workbook.Properties.Author = HeaderName;
                    p.Workbook.Properties.Title = HeaderName;

                    ExcelWorksheet worksheet2 = p.Workbook.Worksheets.Add("Details");

                    worksheet2.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    #endregion

                    var colors = ColorList ?? new List<colorList>();
                    var clarities = ClarityList ?? new List<clarityList>();
                    var sizes = Sizes ?? new List<sizeList>();
                    var predictions = List ?? new List<TRN_Pricing>();

                    int row = 1;

                    // ===== HEADER =====
                    worksheet2.Cells[row, 1].Value = "GroupName";
                    worksheet2.Cells[row, 2].Value = "Color";

                    for (int i = 0; i < clarities.Count; i++)
                    {
                        worksheet2.Cells[row, 3 + i].Value = clarities[i].Clarity;
                    }

                    int endCol = 2 + clarities.Count;

                    worksheet2.Cells[row, 1, row, endCol].Style.Font.Bold = true;
                    worksheet2.Cells[row, 1, row, endCol].Style.Font.Color.SetColor(Color.White);
                    worksheet2.Cells[row, 1, row, endCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet2.Cells[row, 1, row, endCol].Style.Fill.BackgroundColor.SetColor(Color.Black);

                    // Add borders to Header
                    worksheet2.Cells[row, 1, row, endCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[row, 1, row, endCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[row, 1, row, endCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[row, 1, row, endCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    row++;

                    // ===== DATA ROWS =====
                    foreach (var s in sizes)
                    {
                        foreach (var clr in colors)
                        {
                            worksheet2.Cells[row, 1].Value = s.Size;
                            worksheet2.Cells[row, 2].Value = clr.Color;

                            // Light Blue background for GroupName column
                            worksheet2.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet2.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#DEEDF5"));

                            for (int i = 0; i < clarities.Count; i++)
                            {
                                var cl = clarities[i];

                                var price = predictions
                                    .Where(x =>
                                        string.Equals(x.Color, clr.Color, StringComparison.OrdinalIgnoreCase) &&
                                        string.Equals(x.Clarity, cl.Clarity, StringComparison.OrdinalIgnoreCase) &&
                                        string.Equals(x.Size, s.Size, StringComparison.OrdinalIgnoreCase))
                                    .Select(x => x.FinalRate)
                                    .FirstOrDefault();

                                decimal priceDec;
                                var show = (price == null) ? "" :
                                    (decimal.TryParse(Convert.ToString(price), out priceDec) && priceDec == 0m) ? "" : Convert.ToString(price);

                                worksheet2.Cells[row, 3 + i].Value = show;
                            }

                            // Add borders for data row
                            worksheet2.Cells[row, 1, row, endCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet2.Cells[row, 1, row, endCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet2.Cells[row, 1, row, endCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet2.Cells[row, 1, row, endCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            row++;
                        }
                    }

                    // AutoFit columns
                    for (int i = 1; i <= endCol; i++)
                    {
                        worksheet2.Column(i).AutoFit();
                        worksheet2.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    string Folderpath = RequestHelpers.GetConfigValue("ExcelFiles");
                    string filename = HeaderName + "_" + Shape + ".xlsx";
                    if (Directory.Exists(HttpContext.Current.Server.MapPath(Folderpath)) == false)
                    {
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(Folderpath));
                    }

                    stReturnFileName = HttpContext.Current.Server.MapPath(Folderpath + "\\" + filename);
                    Byte[] bin = p.GetAsByteArray();

                    System.IO.File.WriteAllBytes(stReturnFileName, bin);
                }
                return stReturnFileName;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return "";
            }
        }
        public static string ExportApprovePricing(List<clarityList> ClarityList,List<TRN_Pricing> List, string HeaderName)
        {
            try
            {
                ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");

                var FileExcelName = HeaderName;
                string stReturnFileName = "";
                using (ExcelPackage p = new ExcelPackage())
                {
                    #region Company Detail on Header
                    p.Workbook.Properties.Author = HeaderName;
                    p.Workbook.Properties.Title = HeaderName;

                    ExcelWorksheet worksheet2 = p.Workbook.Worksheets.Add("Details");

                    worksheet2.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    #endregion

                    Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#449BCA");
                    Color RowColor = System.Drawing.ColorTranslator.FromHtml("#DEEDF5");
                    Color TotalRow = System.Drawing.ColorTranslator.FromHtml("#449BCA");

                    #region Header Name Declaration and Assign Values
                    var clarityColumns = ClarityList.Select(x => x.Clarity.ToString()).ToList();

                    int row = 1;

                    // ===== HEADER =====
                    worksheet2.Cells[row, 1].Value = "GradType";
                    worksheet2.Cells[row, 2].Value = "Shape";
                    worksheet2.Cells[row, 3].Value = "Size";
                    worksheet2.Cells[row, 4].Value = "Color";

                    for (int i = 0; i < clarityColumns.Count; i++)
                        worksheet2.Cells[row, 5 + i].Value = clarityColumns[i];

                    worksheet2.Cells[row, 1, row, 4 + clarityColumns.Count].Style.Font.Bold = true;
                    worksheet2.Cells[row, 1, row, 4 + clarityColumns.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet2.Cells[row, 1, row, 4 + clarityColumns.Count].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                    row++;

                    // ===== DATA (PIVOT LOGIC) =====
                    var groupedData = List
                        .GroupBy(x => new
                        {
                            GradType = x.GradType,
                            Shape = x.Shape,
                            Size = x.Size,
                            Color = x.Color
                        });

                    foreach (var grp in groupedData)
                    {
                        worksheet2.Cells[row, 1].Value = grp.Key.GradType;
                        worksheet2.Cells[row, 2].Value = grp.Key.Shape;
                        worksheet2.Cells[row, 3].Value = grp.Key.Size;
                        worksheet2.Cells[row, 4].Value = grp.Key.Color;

                        for (int i = 0; i < clarityColumns.Count; i++)
                        {
                            string clarity = clarityColumns[i];

                            var rate = grp
                                .FirstOrDefault(x => x.Clarity == clarity)?.Rate ?? 0;

                            worksheet2.Cells[row, 5 + i].Value = rate;
                        }

                        row++;
                    }

                    for (int i = 1; i <= 20; i++)
                    {
                        worksheet2.Column(i).AutoFit();
                        worksheet2.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    #endregion

                    string Folderpath = RequestHelpers.GetConfigValue("ExcelFiles");
                    string filename = HeaderName + ".xlsx";
                    if (Directory.Exists(HttpContext.Current.Server.MapPath(Folderpath)) == false)
                    {
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(Folderpath));
                    }

                    stReturnFileName = HttpContext.Current.Server.MapPath(Folderpath + "\\" + filename);
                    Byte[] bin = p.GetAsByteArray();

                    System.IO.File.WriteAllBytes(stReturnFileName, bin);
                }
                return stReturnFileName;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return "";
            }
        }
        public static string ExportApprovePricingV2(List<colorList> ColorList, List<clarityList> ClarityList, List<sizeList> Sizes, List<TRN_Pricing> List, string Shape, string GradType,string GridType, string Date, string HeaderName)
        {
            try
            {
                ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");

                var FileExcelName = HeaderName;
                string stReturnFileName = "";
                using (ExcelPackage p = new ExcelPackage())
                {
                    #region Company Detail on Header
                    p.Workbook.Properties.Author = HeaderName;
                    p.Workbook.Properties.Title = HeaderName;

                    ExcelWorksheet worksheet2 = p.Workbook.Worksheets.Add("Details");

                    worksheet2.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    #endregion

                    Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#449BCA");
                    Color RowColor = System.Drawing.ColorTranslator.FromHtml("#DEEDF5");
                    Color TotalRow = System.Drawing.ColorTranslator.FromHtml("#449BCA");

                    #region Header Name Declaration and Assign Values
                    // normalize inputs
                    var colors = ColorList ?? new List<colorList>();
                    var clarities = ClarityList ?? new List<clarityList>();
                    var sizes = Sizes ?? new List<sizeList>();
                    var predictions = List ?? new List<TRN_Pricing>();


                    if (GridType == "Size")
                    {
                        int HeaderWidth = 3 + (clarities.Count * 2);

                        worksheet2.Cells[1, 1, 1, HeaderWidth].Merge = true;
                        worksheet2.Cells[1, 1, 1, HeaderWidth].Value = Shape + " (" + GradType + ")" + " (" + Date + ")";
                        worksheet2.Cells[1, 1, 1, HeaderWidth].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet2.Cells[1, 1, 1, HeaderWidth].Style.Font.Bold = true;
                        worksheet2.Cells[1, 1, 1, HeaderWidth].Style.Font.Size = 16;
                        worksheet2.Cells[1, 1, 1, HeaderWidth].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet2.Cells[1, 1, 1, HeaderWidth].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        worksheet2.Cells[1, 1, 1, HeaderWidth].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[1, 1, 1, HeaderWidth].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[1, 1, 1, HeaderWidth].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[1, 1, 1, HeaderWidth].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        // layout parameters
                        const int maxBlocksPerRow = 2;           // two blocks per visual row as in the provided image
                        const int horizontalSpacing = 1;         // empty columns between blocks
                        int startTopRow = 3;                     // top row to start placing blocks

                        // convert sizes to list to support indexing
                        var sizeList = sizes.ToList();

                        int totalRowsOfBlocks = (int)Math.Ceiling(sizeList.Count / (double)maxBlocksPerRow);

                        for (int blockRow = 0; blockRow < totalRowsOfBlocks; blockRow++)
                        {
                            // get sizes for this visual row (max 2)
                            var rowSizes = sizeList.Skip(blockRow * maxBlocksPerRow).Take(maxBlocksPerRow).ToList();

                            // compute block heights and place blocks left-to-right
                            int maxHeightInRow = 0;
                            for (int blockIndex = 0; blockIndex < rowSizes.Count; blockIndex++)
                            {
                                var s = rowSizes[blockIndex];
                                if (s == null) continue;

                                // determine block column start
                                int blockWidth = 1 + Math.Max(clarities.Count, 1); // "Color" column + clarity columns
                                int headerStartCol = blockIndex * (blockWidth + horizontalSpacing) + 1;
                                int headerEndCol = headerStartCol + blockWidth - 1;

                                // merged size header (top of block)
                                worksheet2.Cells[startTopRow, headerStartCol, startTopRow, headerEndCol].Merge = true;
                                worksheet2.Cells[startTopRow, headerStartCol].Value = s.Size;
                                worksheet2.Cells[startTopRow, headerStartCol].Style.Font.Bold = true;
                                worksheet2.Cells[startTopRow, headerStartCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                                // sub headers: Color + clarity names
                                int subHeaderRow = startTopRow + 1;
                                worksheet2.Cells[subHeaderRow, headerStartCol].Value = "Color";
                                worksheet2.Cells[subHeaderRow, headerStartCol].Style.Font.Bold = true;
                                worksheet2.Cells[subHeaderRow, headerStartCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet2.Cells[subHeaderRow, headerStartCol].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                                int colIndex = headerStartCol + 1;
                                foreach (var c in clarities)
                                {
                                    worksheet2.Cells[subHeaderRow, colIndex].Value = c?.Clarity ?? "";
                                    worksheet2.Cells[subHeaderRow, colIndex].Style.Font.Bold = true;
                                    worksheet2.Cells[subHeaderRow, colIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    worksheet2.Cells[subHeaderRow, colIndex].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                                    colIndex++;
                                }

                                // data rows: one row per color
                                int firstDataRow = startTopRow + 2;
                                int rowIndex = firstDataRow;
                                foreach (var clr in colors)
                                {
                                    worksheet2.Cells[rowIndex, headerStartCol].Value = clr?.Color ?? "";
                                    worksheet2.Cells[rowIndex, headerStartCol].Style.Font.Bold = true;
                                    worksheet2.Cells[rowIndex, headerStartCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    worksheet2.Cells[rowIndex, headerStartCol].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                                    colIndex = headerStartCol + 1;

                                    foreach (var cl in clarities)
                                    {
                                        // find price for this size/color/clarity
                                        var price = predictions
                                            .Where(x =>
                                                string.Equals(x.Color, clr?.Color, StringComparison.OrdinalIgnoreCase) &&
                                                string.Equals(x.Clarity, cl?.Clarity, StringComparison.OrdinalIgnoreCase) &&
                                                string.Equals(x.Size, s.Size, StringComparison.OrdinalIgnoreCase))
                                            .Select(x => x.FinalRate)
                                            .FirstOrDefault();

                                        // show empty cell for zero or missing price
                                        decimal priceDec;
                                        var show = (price == null) ? "" :
                                            (decimal.TryParse(Convert.ToString(price), out priceDec) && priceDec == 0m) ? "" : Convert.ToString(price);

                                        worksheet2.Cells[rowIndex, colIndex].Value = show;
                                        colIndex++;
                                    }

                                    rowIndex++;
                                }

                                // add a blank row after colors (visual spacing)
                                int blockHeight = (rowIndex - startTopRow);
                                if (blockHeight > maxHeightInRow) maxHeightInRow = blockHeight;

                                // set borders and minor styling for the block range
                                try
                                {
                                    var blockRange = worksheet2.Cells[startTopRow, headerStartCol, startTopRow + blockHeight - 1, headerEndCol];
                                    blockRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    blockRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    blockRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    blockRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                catch
                                {
                                    // ignore styling errors
                                }

                                // AutoFit columns used by this block
                                try
                                {
                                    for (int c = headerStartCol; c <= headerEndCol; c++)
                                    {
                                        worksheet2.Column(c).AutoFit();
                                        worksheet2.Column(c).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    }
                                }
                                catch
                                {
                                    // ignore AutoFit errors
                                }
                            } // end blocks in the visual row

                            // advance top row by the tallest block in this visual row + gap
                            startTopRow += maxHeightInRow + 1;
                        } // end block rows
                    }
                    else if (GridType == "Color")
                    {
                        int HeaderWidth = 2 + sizes.Count;

                        worksheet2.Cells[1, 1, 1, HeaderWidth].Merge = true;
                        worksheet2.Cells[1, 1, 1, HeaderWidth].Value = Shape + " (" + GradType + ")" + " (" + Date + ")";
                        worksheet2.Cells[1, 1, 1, HeaderWidth].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet2.Cells[1, 1, 1, HeaderWidth].Style.Font.Bold = true;
                        worksheet2.Cells[1, 1, 1, HeaderWidth].Style.Font.Size = 16;
                        worksheet2.Cells[1, 1, 1, HeaderWidth].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet2.Cells[1, 1, 1, HeaderWidth].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        worksheet2.Cells[1, 1, 1, HeaderWidth].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[1, 1, 1, HeaderWidth].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[1, 1, 1, HeaderWidth].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet2.Cells[1, 1, 1, HeaderWidth].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        int startTopRow = 3;
                        Color headerBgColor = ColorTranslator.FromHtml("#E8F1F9");
                        Color textColor = ColorTranslator.FromHtml("#449BCA");
                        Color borderColor = ColorTranslator.FromHtml("#D3E3F0");

                        foreach (var clr in colors)
                        {
                            if (clr == null || string.IsNullOrEmpty(clr.Color)) continue;
                            
                            // add droplet icon exactly like image if wanted, or just simple text
                            string clrText = " \U0001F4A7 " + clr.Color + " Color";
                            
                            worksheet2.Cells[startTopRow, 1, startTopRow, HeaderWidth].Merge = true;
                            worksheet2.Cells[startTopRow, 1, startTopRow, HeaderWidth].Value = clrText;
                            worksheet2.Cells[startTopRow, 1, startTopRow, HeaderWidth].Style.Font.Bold = true;
                            worksheet2.Cells[startTopRow, 1, startTopRow, HeaderWidth].Style.Font.Color.SetColor(textColor);
                            worksheet2.Cells[startTopRow, 1, startTopRow, HeaderWidth].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet2.Cells[startTopRow, 1, startTopRow, HeaderWidth].Style.Fill.BackgroundColor.SetColor(headerBgColor);
                            
                            int subHeaderRow = startTopRow + 1;
                            
                            worksheet2.Cells[subHeaderRow, 2].Value = "CLARITY";
                            worksheet2.Cells[subHeaderRow, 2].Style.Font.Bold = true;
                            worksheet2.Cells[subHeaderRow, 2].Style.Font.Color.SetColor(textColor);
                            worksheet2.Cells[subHeaderRow, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            
                            int colIndex = 3;
                            foreach (var s in sizes)
                            {
                                worksheet2.Cells[subHeaderRow, colIndex].Value = s?.Size ?? "";
                                worksheet2.Cells[subHeaderRow, colIndex].Style.Font.Bold = true;
                                worksheet2.Cells[subHeaderRow, colIndex].Style.Font.Color.SetColor(textColor);
                                worksheet2.Cells[subHeaderRow, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                colIndex++;
                            }

                            int firstDataRow = subHeaderRow + 1;
                            int lastDataRow = firstDataRow + clarities.Count - 1;

                            if (clarities.Count > 0)
                            {
                                worksheet2.Cells[firstDataRow, 1, lastDataRow, 1].Merge = true;
                                worksheet2.Cells[firstDataRow, 1, lastDataRow, 1].Value = clr.Color;
                                worksheet2.Cells[firstDataRow, 1, lastDataRow, 1].Style.Font.Bold = true;
                                worksheet2.Cells[firstDataRow, 1, lastDataRow, 1].Style.Font.Color.SetColor(textColor);
                                worksheet2.Cells[firstDataRow, 1, lastDataRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet2.Cells[firstDataRow, 1, lastDataRow, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            }

                            int rowIndex = firstDataRow;
                            foreach (var cl in clarities)
                            {
                                worksheet2.Cells[rowIndex, 2].Value = cl?.Clarity ?? "";
                                worksheet2.Cells[rowIndex, 2].Style.Font.Color.SetColor(textColor);
                                worksheet2.Cells[rowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                colIndex = 3;
                                foreach (var s in sizes)
                                {
                                    var price = predictions
                                        .Where(x =>
                                            string.Equals(x.Color, clr.Color, StringComparison.OrdinalIgnoreCase) &&
                                            string.Equals(x.Clarity, cl?.Clarity, StringComparison.OrdinalIgnoreCase) &&
                                            string.Equals(x.Size, s?.Size, StringComparison.OrdinalIgnoreCase))
                                        .Select(x => x.FinalRate)
                                        .FirstOrDefault();

                                    decimal priceDec;
                                    var show = (price == null) ? "" :
                                        (decimal.TryParse(Convert.ToString(price), out priceDec) && priceDec == 0m) ? "" : Convert.ToString(price);

                                    worksheet2.Cells[rowIndex, colIndex].Value = show;
                                    worksheet2.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet2.Cells[rowIndex, colIndex].Style.Font.Bold = true;
                                    colIndex++;
                                }
                                rowIndex++;
                            }

                            // Inner thin Borders
                            for (int r = startTopRow; r <= lastDataRow; r++)
                            {
                                for (int c = 1; c <= HeaderWidth; c++)
                                {
                                    worksheet2.Cells[r, c].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    worksheet2.Cells[r, c].Style.Border.Top.Color.SetColor(borderColor);
                                    worksheet2.Cells[r, c].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    worksheet2.Cells[r, c].Style.Border.Bottom.Color.SetColor(borderColor);
                                    worksheet2.Cells[r, c].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    worksheet2.Cells[r, c].Style.Border.Left.Color.SetColor(borderColor);
                                    worksheet2.Cells[r, c].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    worksheet2.Cells[r, c].Style.Border.Right.Color.SetColor(borderColor);
                                }
                            }

                            // Thick Borders around the block
                            worksheet2.Cells[startTopRow, 1, startTopRow, HeaderWidth].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            worksheet2.Cells[startTopRow, 1, startTopRow, HeaderWidth].Style.Border.Top.Color.SetColor(borderColor);
                            
                            worksheet2.Cells[lastDataRow, 1, lastDataRow, HeaderWidth].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            worksheet2.Cells[lastDataRow, 1, lastDataRow, HeaderWidth].Style.Border.Bottom.Color.SetColor(borderColor);
                            
                            worksheet2.Cells[startTopRow, 1, lastDataRow, 1].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            worksheet2.Cells[startTopRow, 1, lastDataRow, 1].Style.Border.Left.Color.SetColor(borderColor);
                            
                            worksheet2.Cells[startTopRow, HeaderWidth, lastDataRow, HeaderWidth].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            worksheet2.Cells[startTopRow, HeaderWidth, lastDataRow, HeaderWidth].Style.Border.Right.Color.SetColor(borderColor);
                            
                            startTopRow = lastDataRow + 2;
                        }

                        for (int c = 1; c <= HeaderWidth; c++)
                        {
                            worksheet2.Column(c).AutoFit();
                            worksheet2.Column(c).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }
                    }

                    #endregion

                    string Folderpath = RequestHelpers.GetConfigValue("ExcelFiles");
                    string filename = HeaderName + ".xlsx";
                    if (Directory.Exists(HttpContext.Current.Server.MapPath(Folderpath)) == false)
                    {
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(Folderpath));
                    }

                    stReturnFileName = HttpContext.Current.Server.MapPath(Folderpath + "\\" + filename);
                    Byte[] bin = p.GetAsByteArray();

                    System.IO.File.WriteAllBytes(stReturnFileName, bin);
                }
                return stReturnFileName;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return "";
            }
        }
        public static string ExportPriceComparisonExcel(DataSet ds, string shape, string gradType)
        {
            try
            {
                ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");

                string stReturnFileName = "";
                using (ExcelPackage p = new ExcelPackage())
                {
                    p.Workbook.Properties.Author = "ADPL";
                    p.Workbook.Properties.Title = "Price Comparison";

                    var ws = p.Workbook.Worksheets.Add("Price Comparison");
                    ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    DataTable dtColors = ds.Tables[0];
                    DataTable dtClarities = ds.Tables[1];
                    DataTable dtSizeGroups = ds.Tables[2];
                    DataTable dtData = ds.Tables[3];

                    int currentRow = 1;
                    int currentCol = 1;

                    Color headerBg = ColorTranslator.FromHtml("#D3D3D3");
                    Color colorBg = ColorTranslator.FromHtml("#F0F0F0");

                    // Loop through Size Groups and create a matrix for each
                    foreach (DataRow sgRow in dtSizeGroups.Rows)
                    {
                        string sizeGroup = sgRow["SizeGroup"].ToString();

                        // Header for Size Group
                        int startCol = currentCol;
                        int endCol = currentCol + dtClarities.Rows.Count;
                        ws.Cells[currentRow, startCol, currentRow, endCol].Merge = true;
                        ws.Cells[currentRow, startCol, currentRow, endCol].Value = sizeGroup;
                        ws.Cells[currentRow, startCol, currentRow, endCol].Style.Font.Bold = true;
                        ws.Cells[currentRow, startCol, currentRow, endCol].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        currentRow++;

                        // Horizontal Header (Clarity)
                        ws.Cells[currentRow, currentCol].Value = "Color";
                        ws.Cells[currentRow, currentCol].Style.Font.Bold = true;
                        ws.Cells[currentRow, currentCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[currentRow, currentCol].Style.Fill.BackgroundColor.SetColor(headerBg);
                        ws.Cells[currentRow, currentCol].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        int clarityColIdx = currentCol + 1;
                        foreach (DataRow clarityRow in dtClarities.Rows)
                        {
                            ws.Cells[currentRow, clarityColIdx].Value = clarityRow["Clarity"].ToString();
                            ws.Cells[currentRow, clarityColIdx].Style.Font.Bold = true;
                            ws.Cells[currentRow, clarityColIdx].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells[currentRow, clarityColIdx].Style.Fill.BackgroundColor.SetColor(headerBg);
                            ws.Cells[currentRow, clarityColIdx].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            clarityColIdx++;
                        }
                        currentRow++;

                        // Rows (Color)
                        int startRowForData = currentRow;
                        foreach (DataRow colorRow in dtColors.Rows)
                        {
                            string colorName = colorRow["Color"].ToString();
                            ws.Cells[currentRow, currentCol].Value = colorName;
                            ws.Cells[currentRow, currentCol].Style.Font.Bold = true;
                            ws.Cells[currentRow, currentCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells[currentRow, currentCol].Style.Fill.BackgroundColor.SetColor(colorBg);
                            ws.Cells[currentRow, currentCol].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            int dataColIdx = currentCol + 1;
                            foreach (DataRow clarityRow in dtClarities.Rows)
                            {
                                string clarityName = clarityRow["Clarity"].ToString();

                                // Find data for this size group, color, and clarity
                                var match = dtData.AsEnumerable().FirstOrDefault(r =>
                                    r["SizeGroup"].ToString() == sizeGroup &&
                                    r["Color"].ToString() == colorName &&
                                    r["Clarity"].ToString() == clarityName);

                                if (match != null)
                                {
                                    decimal oldPrice = match["OldPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(match["OldPrice"]);
                                    decimal newPrice = match["NewPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(match["NewPrice"]);
                                    decimal diff = newPrice - oldPrice;

                                    var cell = ws.Cells[currentRow, dataColIdx];

                                    // Ã¢Å“â€¦ Force TEXT format
                                    cell.Style.Numberformat.Format = "@";
                                    cell.Style.WrapText = true;
                                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                    // Enable rich text
                                    cell.IsRichText = true;
                                    var rt = cell.RichText;

                                    // Ã¢Å“â€¦ Format values safely
                                    string oldStr = oldPrice == 0 ? "-" : oldPrice.ToString("0.00");
                                    string newStr = oldPrice > 0 ? newPrice.ToString("0.00") : (newPrice == 0 ? "-" : newPrice.ToString("0.00"));
                                    string diffStr1 = (newPrice > oldPrice ? "" : "") + diff.ToString("0.00");
                                    string diffStr = diff == 0 ? "-" : diffStr1;

                                    // OLD PRICE
                                    var oldRt = rt.Add(oldStr + "\n");
                                    oldRt.Color = Color.Gray;

                                    // NEW PRICE
                                    var newRt = rt.Add(newStr + "\n");
                                    newRt.Bold = true;
                                    if (newPrice > oldPrice) newRt.Color = Color.Green;
                                    else if (newPrice < oldPrice) newRt.Color = Color.Red;

                                    // DIFFERENCE
                                    var diffRt = rt.Add(diffStr);
                                    if (newPrice > oldPrice) diffRt.Color = Color.Green;
                                    else if (newPrice < oldPrice) diffRt.Color = Color.Red;

                                    ws.Cells[currentRow, dataColIdx].Style.WrapText = true;
                                    ws.Cells[currentRow, dataColIdx].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                }
                                else
                                {
                                    ws.Cells[currentRow, dataColIdx].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                }
                                dataColIdx++;
                            }
                            currentRow++;
                        }

                        // Move currentCol for next matrix (with spacing)
                        currentCol = endCol + 2;
                        if (currentCol > 20)
                        {
                            currentCol = 1;
                            currentRow += 2;
                        }
                        else
                        {
                            currentRow -= (dtColors.Rows.Count + 2);
                        }
                    }

                    ws.Cells[ws.Dimension.Address].AutoFitColumns();

                    string Folderpath = ConfigurationManager.AppSettings["ExcelFiles"];
                    string filename = "PriceComparison_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    if (!Directory.Exists(HttpContext.Current.Server.MapPath(Folderpath)))
                    {
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(Folderpath));
                    }

                    stReturnFileName = HttpContext.Current.Server.MapPath(Folderpath + "\\" + filename);
                    File.WriteAllBytes(stReturnFileName, p.GetAsByteArray());
                }
                return stReturnFileName;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return "";
            }
        }
        private static void GeneratePivotSheet(ExcelPackage p, string sheetName, string headerTitle, string valueLabel, string numberFormat, List<DmxStockSummary_Pivot> List4, Func<DmxStockSummary_Pivot, decimal> valSelector, Func<DmxStockSummary_Pivot, decimal> perSelector)
        {
            ExcelWorksheet wsPivot = p.Workbook.Worksheets.Add(sheetName);
            wsPivot.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsPivot.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            Color gold = System.Drawing.ColorTranslator.FromHtml("#FFC000");
            Color lightGreen = System.Drawing.ColorTranslator.FromHtml("#A9D08E");
            Color lavender = System.Drawing.ColorTranslator.FromHtml("#E4DFEC");
            Color lightBlue = System.Drawing.ColorTranslator.FromHtml("#9BC2E6");
            Color colBlack = System.Drawing.ColorTranslator.FromHtml("#000000");
            Color colWhite = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");

            var standardClarities = new List<string> { "FL", "IF", "VVS1", "VVS2", "VS1", "VS2", "SI1", "SI2", "I1", "I2", "I3" };
            var claritiesInData = List4.Select(x => x.Clarity).Distinct().ToList();
            var sortedClarities = claritiesInData.OrderBy(c => {
                int idx = standardClarities.IndexOf(c);
                return idx == -1 ? 99 : idx;
            }).ToList();

            int pivotCols = 1 + (sortedClarities.Count * 2) + 2; 
            
            wsPivot.Cells[1, 1, 1, pivotCols].Merge = true;
            wsPivot.Cells[1, 1, 1, pivotCols].Value = headerTitle;
            wsPivot.Cells[1, 1, 1, pivotCols].Style.Font.Bold = true;
            wsPivot.Cells[1, 1, 1, pivotCols].Style.Font.Size = 14;
            wsPivot.Cells[1, 1, 1, pivotCols].Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsPivot.Cells[1, 1, 1, pivotCols].Style.Fill.BackgroundColor.SetColor(gold);

            wsPivot.Cells[2, 1].Value = DateTime.Now.ToString("dd-MM-yy");
            wsPivot.Cells[2, 1].Style.Font.Bold = true;
            int colIdx = 2;
            foreach(var clr in sortedClarities) {
                wsPivot.Cells[2, colIdx].Value = valueLabel;
                wsPivot.Cells[2, colIdx + 1].Value = "%";
                wsPivot.Cells[2, colIdx, 2, colIdx+1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsPivot.Cells[2, colIdx, 2, colIdx+1].Style.Fill.BackgroundColor.SetColor(lightGreen);
                wsPivot.Cells[2, colIdx, 2, colIdx+1].Style.Font.Bold = true;
                colIdx += 2;
            }
            wsPivot.Cells[2, colIdx].Value = valueLabel;
            wsPivot.Cells[2, colIdx + 1].Value = "%";
            wsPivot.Cells[2, colIdx, 2, colIdx+1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsPivot.Cells[2, colIdx, 2, colIdx+1].Style.Fill.BackgroundColor.SetColor(lightGreen);
            wsPivot.Cells[2, colIdx, 2, colIdx+1].Style.Font.Bold = true;

            wsPivot.Cells[3, 1].Value = "SIZE";
            wsPivot.Cells[3, 1].Style.Font.Color.SetColor(System.Drawing.Color.Gray);
            colIdx = 2;
            foreach(var clr in sortedClarities) {
                wsPivot.Cells[3, colIdx, 3, colIdx+1].Merge = true;
                wsPivot.Cells[3, colIdx].Value = clr;
                wsPivot.Cells[3, colIdx, 3, colIdx+1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsPivot.Cells[3, colIdx, 3, colIdx+1].Style.Fill.BackgroundColor.SetColor(colBlack);
                wsPivot.Cells[3, colIdx, 3, colIdx+1].Style.Font.Color.SetColor(colWhite);
                colIdx += 2;
            }
            wsPivot.Cells[3, colIdx].Value = "Total";
            wsPivot.Cells[3, colIdx+1].Value = "%";
            wsPivot.Cells[3, colIdx, 3, colIdx+1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsPivot.Cells[3, colIdx, 3, colIdx+1].Style.Fill.BackgroundColor.SetColor(colBlack);
            wsPivot.Cells[3, colIdx, 3, colIdx+1].Style.Font.Color.SetColor(colWhite);

            int rIdx = 4;
            var sizeGroups = List4.Select(x => x.SizeBucket).Distinct().ToList();
            var stdColors = new List<string> { "D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z" };

            foreach(var sg in sizeGroups) {
                var sgData = List4.Where(x => x.SizeBucket == sg).ToList();
                
                wsPivot.Cells[rIdx, 1].Value = sg;
                wsPivot.Cells[rIdx, 1].Style.Font.Bold = true;
                wsPivot.Cells[rIdx, 1, rIdx, pivotCols].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsPivot.Cells[rIdx, 1, rIdx, pivotCols].Style.Fill.BackgroundColor.SetColor(lightBlue);
                
                colIdx = 2;
                decimal totalSgVal = 0;
                decimal totalSgPer = 0;
                foreach(var clr in sortedClarities) {
                    var clrSum = sgData.Where(x => x.Clarity == clr).Sum(valSelector);
                    var clrPerSum = sgData.Where(x => x.Clarity == clr).Sum(perSelector);
                    if (clrSum > 0) {
                        wsPivot.Cells[rIdx, colIdx].Value = clrSum;
                        wsPivot.Cells[rIdx, colIdx+1].Value = clrPerSum;
                        wsPivot.Cells[rIdx, colIdx, rIdx, colIdx+1].Style.Font.Bold = true;
                    }
                    totalSgVal += clrSum;
                    totalSgPer += clrPerSum;
                    colIdx += 2;
                }
                wsPivot.Cells[rIdx, colIdx].Value = totalSgVal;
                wsPivot.Cells[rIdx, colIdx+1].Value = totalSgPer;
                wsPivot.Cells[rIdx, colIdx, rIdx, colIdx].Style.Font.Bold = true;
                
                wsPivot.Cells[rIdx, colIdx+1].Style.Fill.BackgroundColor.SetColor(colBlack);
                wsPivot.Cells[rIdx, colIdx+1].Style.Font.Color.SetColor(colWhite);
                wsPivot.Cells[rIdx, colIdx+1].Style.Font.Bold = true;
                rIdx++;

                var colorsInSg = sgData.Select(x => x.Color).Distinct()
                                       .OrderBy(c => { int idx = stdColors.IndexOf(c.ToUpper()); return idx == -1 ? 99 : idx; }).ToList();
                
                foreach(var c in colorsInSg) {
                    var colorData = sgData.Where(x => x.Color == c).ToList();
                    wsPivot.Cells[rIdx, 1].Value = c;
                    
                    colIdx = 2;
                    decimal totalColVal = 0;
                    decimal totalColPer = 0;
                    foreach(var clr in sortedClarities) {
                        var cellData = colorData.FirstOrDefault(x => x.Clarity == clr);
                        if (cellData != null) {
                            var v = valSelector(cellData);
                            var pPer = perSelector(cellData);
                            if (v > 0) {
                                wsPivot.Cells[rIdx, colIdx].Value = v;
                                wsPivot.Cells[rIdx, colIdx+1].Value = pPer;
                                totalColVal += v;
                                totalColPer += pPer;
                            }
                        }
                        colIdx += 2;
                    }
                    wsPivot.Cells[rIdx, colIdx].Value = totalColVal;
                    wsPivot.Cells[rIdx, colIdx+1].Value = totalColPer;
                    
                    wsPivot.Cells[rIdx, colIdx].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsPivot.Cells[rIdx, colIdx].Style.Fill.BackgroundColor.SetColor(lightBlue);
                    
                    wsPivot.Cells[rIdx, colIdx+1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsPivot.Cells[rIdx, colIdx+1].Style.Fill.BackgroundColor.SetColor(colWhite);
                    rIdx++;
                }
            }

            wsPivot.Cells[rIdx, 1].Value = "Total";
            wsPivot.Cells[rIdx, 1, rIdx, pivotCols].Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsPivot.Cells[rIdx, 1, rIdx, pivotCols].Style.Fill.BackgroundColor.SetColor(gold);
            wsPivot.Cells[rIdx, 1, rIdx, pivotCols].Style.Font.Bold = true;
            
            colIdx = 2;
            decimal grandVal = 0;
            decimal grandPer = 0;
            foreach(var clr in sortedClarities) {
                var clrTotal = List4.Where(x => x.Clarity == clr).Sum(valSelector);
                var clrTotalPer = List4.Where(x => x.Clarity == clr).Sum(perSelector);
                wsPivot.Cells[rIdx, colIdx].Value = clrTotal;
                wsPivot.Cells[rIdx, colIdx+1].Value = clrTotalPer;
                grandVal += clrTotal;
                grandPer += clrTotalPer;
                colIdx += 2;
            }
            wsPivot.Cells[rIdx, colIdx].Value = grandVal;
            wsPivot.Cells[rIdx, colIdx+1].Value = Math.Round(grandPer, 0); 
            
            for (int r = 4; r <= rIdx; r++) {
                for(int c=2; c <= pivotCols; c+=2) {
                    wsPivot.Cells[r, c].Style.Numberformat.Format = numberFormat;
                }
            }
            
            SetBorders(wsPivot, 1, 1, rIdx, pivotCols);
            for (int i = 1; i <= pivotCols; i++) {
                wsPivot.Column(i).AutoFit();
            }
        }

        public static string AvailableStockSummaryExport(List<DmxStockSummary_Size> List1, List<DmxStockSummary_Color> List2, List<DmxStockSummary_Clarity> List3, List<DmxStockSummary_Pivot> List4, string HeaderName, List<DMX_AvailableStock> rawStock = null, List<SizeMasterItem> sizeList = null, string displayDate = null, System.Data.DataTable dtReportDays = null, System.Data.DataTable dtStatus = null)
        {
            try
            {
                ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");
                var FileExcelName = "AvailableStockSummary";
                string stReturnFileName = "";
                using (ExcelPackage p = new ExcelPackage())
                {
                    p.Workbook.Properties.Author = "Diamx Stock";
                    p.Workbook.Properties.Title = "Available Stock Summary";

                    GeneratePivotSheet(p, "PCS WISE REPORT", "ROUND 3EX (IGI STOCK) PERCENTAGE WISE REPORT", "PCS", "0", List4, x => x.Pcs, x => x.PcsPer);
                    GeneratePivotSheet(p, "WEIGHT WISE REPORT", "ROUND 3EX (IGI STOCK) WEIGHT PERCENTAGE WISE REPORT", "Weight", "#,##0.00", List4, x => x.Weight, x => x.WeightPer);
                    GeneratePivotSheet(p, "DOLLAR WISE", "ROUND 3EX (IGI STOCK) DOLLAR PERCENTAGE WISE REPORT", "Amount", "#,##0.00", List4, x => x.TotalAmt, x => x.TotalAmtPer);

                    // === ANALYSIS SHEET (NEW) ===
                    string reportDate = string.IsNullOrEmpty(displayDate) ? DateTime.Today.ToString("dd-MM-yyyy") : displayDate;
                    GenerateAnalysisSheet(p, rawStock, sizeList, reportDate, dtReportDays, dtStatus);

                    // === STOCK SUMMARY REPORT SHEET (NEW) ===
                    GenerateStockSummaryReportSheet(p, rawStock, sizeList, reportDate);

                    string Folderpath = RequestHelpers.GetConfigValue("ExcelFiles");
                    if (string.IsNullOrEmpty(Folderpath))
                    {
                        Folderpath = "\\ExcelFiles";
                    }
                    string filename = "AvailableStockSummary_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    
                    if (HttpContext.Current != null)
                    {
                        if (Directory.Exists(HttpContext.Current.Server.MapPath(Folderpath)) == false)
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath(Folderpath));
                        }
                        stReturnFileName = HttpContext.Current.Server.MapPath(Folderpath + "\\" + filename);
                    }
                    else
                    {
                        string localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExcelFiles");
                        if (!Directory.Exists(localPath))
                        {
                            Directory.CreateDirectory(localPath);
                        }
                        stReturnFileName = Path.Combine(localPath, filename);
                    }
                    
                    AddStoneWiseDetailsSheet(p, "DMX_AvailableStock", displayDate);

                    Byte[] bin = p.GetAsByteArray();
                    System.IO.File.WriteAllBytes(stReturnFileName, bin);
                }
                return stReturnFileName;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                if (HttpContext.Current == null)
                {
                    Console.WriteLine("EXPORT ERROR: " + ex.ToString());
                }
                return "";
            }
        }

        private static void GenerateAnalysisSheet(ExcelPackage p, List<DMX_AvailableStock> rawStock, List<SizeMasterItem> sizeList, string date, System.Data.DataTable dtReportDays, System.Data.DataTable dtStatus)
        {
            if (rawStock == null || sizeList == null || rawStock.Count == 0 || sizeList.Count == 0)
                return;

            ExcelWorksheet ws = p.Workbook.Worksheets.Add("IGI Stock Wise Summary");
            ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.View.ShowGridLines = true;

            // Filter for IGI stones (using rawStock to match Single Stone Details sheet)
            var igiStones = rawStock;

            var parsed = igiStones.Select(s => {
                decimal w = 0m;
                decimal.TryParse(s.Weight, out w);
                
                decimal amt = 0m;
                decimal.TryParse(s.SaleAmt, out amt);

                string shape = s.Shape != null ? s.Shape.Trim().ToUpper() : "";
                string color = s.Color != null ? s.Color.Trim().ToUpper() : "";
                string clarity = s.Clarity != null ? s.Clarity.Trim().ToUpper() : "";
                string cut = s.Cut != null ? s.Cut.Trim().ToUpper() : "";
                string polish = s.Polish != null ? s.Polish.Trim().ToUpper() : "";
                string symm = s.Symm != null ? s.Symm.Trim().ToUpper() : "";
                string loc = s.Location != null ? s.Location.Trim().ToUpper() : "";

                bool isEX = false;
                if (shape == "ROUND")
                {
                    isEX = (cut == "ID" || cut == "EX") && polish == "EX" && symm == "EX";
                }
                else
                {
                    isEX = polish == "EX" && symm == "EX";
                }

                string st = s.StockStatus != null ? s.StockStatus.Trim().ToUpper() : "";
                string mappedStatus = st;
                if (st == "ONMEMO") mappedStatus = "MEMO";
                else if (st == "ONHOLD") mappedStatus = "HOLD";

                string mappedLoc = (loc == "NY" || loc == "NEWYORK") ? "NEWYORK" : (loc == "SURAT" ? "SURAT" : "MUMBAI");

                return new {
                    Shape = shape,
                    Weight = w,
                    Color = color,
                    Clarity = clarity,
                    IsEX = isEX,
                    SaleAmt = amt,
                    Location = mappedLoc,
                    StockStatus = mappedStatus
                };
            }).ToList();

            // Filter sizeList to only include sizes that actually exist in parsed data
            sizeList = sizeList.Where(sizeItem => 
                parsed.Any(s => s.Weight >= sizeItem.FromSize && s.Weight <= sizeItem.ToSize)
            ).ToList();

            if (sizeList.Count == 0)
                return;

            // 1. Title Row
            string dateStr = DateTime.Today.ToString("dd-MM-yyyy");
            if (!string.IsNullOrEmpty(date))
            {
                if (DateTime.TryParse(date, out DateTime parsedDt))
                {
                    dateStr = parsedDt.ToString("dd-MM-yyyy");
                }
                else
                {
                    dateStr = date;
                }
            }

            ws.Cells[2, 1, 2, 29].Merge = true;
            var titleCell = ws.Cells[2, 1];
            titleCell.Value = $"{dateStr} IGI STOCK ANALYSIS REPORT";
            ws.Row(2).Height = 40;

            var titleRange = ws.Cells[2, 1, 2, 29];
            titleRange.Style.Font.Size = 16;
            titleRange.Style.Font.Bold = true;
            titleRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            titleRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            titleRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            titleRange.Style.Fill.BackgroundColor.SetColor(Color.White);
            titleRange.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            SetBorders(ws, 2, 1, 2, 29);

            // 2. Prepare Data for Each Table
            
            // 2a. SIZE data items
            var sizeItemsList = sizeList.Select(sizeItem => {
                string label = sizeItem.FromSize.ToString("0.00") + "-" + (Math.Truncate(sizeItem.ToSize * 100) / 100).ToString("0.00");
                var matched = parsed.Where(s => s.Weight >= sizeItem.FromSize && s.Weight <= sizeItem.ToSize).ToList();
                return new {
                    Name = label,
                    Pcs = matched.Count,
                    Weight = matched.Sum(s => s.Weight),
                    TotalAmt = matched.Sum(s => s.SaleAmt)
                };
            }).ToList();

            // 2b. SHAPE data items
            var shapeItemsList = parsed
                .GroupBy(s => s.Shape)
                .Select(g => new {
                    Name = string.IsNullOrEmpty(g.Key) ? "UNKNOWN" : g.Key,
                    Pcs = g.Count(),
                    Weight = g.Sum(s => s.Weight),
                    TotalAmt = g.Sum(s => s.SaleAmt)
                })
                .OrderByDescending(x => x.TotalAmt)
                .ToList();

            // 2c. CLARITY data items
            var standardClarityOrder = new List<string> { "FL", "IF", "VVS1", "VVS2", "VS1", "VS2", "SI1", "SI2", "I1", "I2", "I3" };
            var clarityItemsList = parsed
                .GroupBy(s => s.Clarity)
                .Select(g => new {
                    Name = string.IsNullOrEmpty(g.Key) ? "UNKNOWN" : g.Key,
                    Pcs = g.Count(),
                    Weight = g.Sum(s => s.Weight),
                    TotalAmt = g.Sum(s => s.SaleAmt)
                })
                .OrderBy(x => {
                    int idx = standardClarityOrder.IndexOf(x.Name);
                    return idx >= 0 ? idx : 999;
                })
                .ToList();

            // 2d. Color data items
            var targetColors = new List<string> { "D", "E", "F", "G", "H", "I", "J" };
            var colorItemsList = parsed
                .GroupBy(s => targetColors.Contains(s.Color) ? s.Color : "COLOR STONE")
                .Select(g => new {
                    Name = g.Key,
                    Pcs = g.Count(),
                    Weight = g.Sum(s => s.Weight),
                    TotalAmt = g.Sum(s => s.SaleAmt)
                })
                .OrderBy(x => {
                    int idx = targetColors.IndexOf(x.Name);
                    return idx >= 0 ? idx : 999;
                })
                .ToList();

            // 2e. LOCATION data items
            var locationItemsList = parsed
                .GroupBy(s => s.Location)
                .Select(g => new {
                    Name = string.IsNullOrEmpty(g.Key) ? "UNKNOWN" : g.Key,
                    Pcs = g.Count(),
                    Weight = g.Sum(s => s.Weight),
                    TotalAmt = g.Sum(s => s.SaleAmt)
                })
                .OrderByDescending(x => x.TotalAmt)
                .ToList();

            // 2f. CUT data items
            var cutItemsList = parsed
                .GroupBy(s => s.IsEX ? "EX" : "VG")
                .Select(g => new {
                    Name = g.Key,
                    Pcs = g.Count(),
                    Weight = g.Sum(s => s.Weight),
                    TotalAmt = g.Sum(s => s.SaleAmt)
                })
                .OrderBy(x => x.Name == "EX" ? 0 : 1)
                .ToList();

            // 3. Draw tables
            
            // Column Group 1 (Cols 1-6): SIZE
            DrawSummaryTable(ws, 4, 1, "SIZE", sizeItemsList, x => x.Name, x => x.Pcs, x => x.Weight, x => x.TotalAmt);

            // Column Group 2 (Cols 8-13): SHAPE & CLARITY
            int shapeLastRow = DrawSummaryTable(ws, 4, 8, "SHAPE", shapeItemsList, x => x.Name, x => x.Pcs, x => x.Weight, x => x.TotalAmt);
            DrawSummaryTable(ws, shapeLastRow + 2, 8, "CLARITY", clarityItemsList, x => x.Name, x => x.Pcs, x => x.Weight, x => x.TotalAmt);

            // Column Group 3 (Cols 15-20): Color, LOCATION & CUT
            int colorLastRow = DrawSummaryTable(ws, 4, 15, "Color", colorItemsList, x => x.Name, x => x.Pcs, x => x.Weight, x => x.TotalAmt);
            int locLastRow = DrawSummaryTable(ws, colorLastRow + 2, 15, "LOCATION", locationItemsList, x => x.Name, x => x.Pcs, x => x.Weight, x => x.TotalAmt);
            DrawSummaryTable(ws, locLastRow + 2, 15, "CUT", cutItemsList, x => x.Name, x => x.Pcs, x => x.Weight, x => x.TotalAmt);

            // Set Column Widths
            ws.Column(1).Width = 12; // SIZE name
            ws.Column(2).Width = 8;  // Pcs
            ws.Column(3).Width = 10; // Weight
            ws.Column(4).Width = 12; // Total $
            ws.Column(5).Width = 10; // Per Ct $
            ws.Column(6).Width = 8;  // %
            ws.Column(7).Width = 4;  // spacer

            ws.Column(8).Width = 24; // SHAPE name
            ws.Column(9).Width = 8;  // Pcs
            ws.Column(10).Width = 10; // Weight
            ws.Column(11).Width = 12;// Total $
            ws.Column(12).Width = 10;// Per Ct $
            ws.Column(13).Width = 8; // %
            ws.Column(14).Width = 4; // spacer

            ws.Column(15).Width = 15;// Color / Location / Cut name
            ws.Column(16).Width = 8; // Pcs
            ws.Column(17).Width = 10;// Weight
            ws.Column(18).Width = 12;// Total $
            ws.Column(19).Width = 10;// Per Ct $
            ws.Column(20).Width = 8; // %
            ws.Column(21).Width = 4; // spacer

            // === NEW: REPORT DAYS & STATUS TABLES ===
            if (dtReportDays == null) dtReportDays = new System.Data.DataTable();
            if (dtStatus == null) dtStatus = new System.Data.DataTable();

            System.Drawing.Color colGreenHeader = System.Drawing.ColorTranslator.FromHtml("#A9D08E");
            System.Drawing.Color colPinkHeader = System.Drawing.ColorTranslator.FromHtml("#E4DFEC");
            System.Drawing.Color colLightBlue = System.Drawing.ColorTranslator.FromHtml("#DDEBF7");
            System.Drawing.Color colLightGreen = System.Drawing.ColorTranslator.FromHtml("#C6EFCE");
            System.Drawing.Color colBlack = System.Drawing.ColorTranslator.FromHtml("#000000");
            System.Drawing.Color colWhite = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
            System.Drawing.Color colLightGray = System.Drawing.ColorTranslator.FromHtml("#F2F2F2");

            int rDays = 4;
            int startColDays = 22;

            string colAvailable = ExcelCellAddress.GetColumnLetter(startColDays + 1); // W (23)
            string colOnHold = ExcelCellAddress.GetColumnLetter(startColDays + 2);    // X (24)
            string colOnMemo = ExcelCellAddress.GetColumnLetter(startColDays + 3);    // Y (25)
            string colTotPcs = ExcelCellAddress.GetColumnLetter(startColDays + 4);    // Z (26)
            string colTotWt = ExcelCellAddress.GetColumnLetter(startColDays + 5);     // AA (27)
            string colTotAmt = ExcelCellAddress.GetColumnLetter(startColDays + 6);    // AB (28)
            string colTotPct = ExcelCellAddress.GetColumnLetter(startColDays + 7);    // AC (29)

            // Headers row 4
            ws.Row(rDays).Height = 25;
            ws.Cells[rDays, startColDays].Value = "REPORT DAYS";
            ws.Cells[rDays, startColDays].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[rDays, startColDays].Style.Fill.BackgroundColor.SetColor(colGreenHeader);
            ws.Cells[rDays, startColDays].Style.Font.Bold = true;

            string[] headersDays = { "AVAILABLE", "ONHOLD", "ONMEMO", "Total Pcs", "Total Weight", "Total $", "%" };
            for (int h = 0; h < headersDays.Length; h++)
            {
                int colIdx = startColDays + 1 + h;
                ws.Cells[rDays, colIdx].Value = headersDays[h];
                ws.Cells[rDays, colIdx].Style.Font.Bold = true;
                ws.Cells[rDays, colIdx].Style.Fill.PatternType = ExcelFillStyle.Solid;

                if (headersDays[h] == "Total $" || headersDays[h] == "%")
                {
                    ws.Cells[rDays, colIdx].Style.Fill.BackgroundColor.SetColor(colBlack);
                    ws.Cells[rDays, colIdx].Style.Font.Color.SetColor(colWhite);
                }
                else if (headersDays[h] == "Total Pcs" || headersDays[h] == "Total Weight")
                {
                    ws.Cells[rDays, colIdx].Style.Fill.BackgroundColor.SetColor(colLightBlue);
                }
                else
                {
                    ws.Cells[rDays, colIdx].Style.Fill.BackgroundColor.SetColor(colPinkHeader);
                }
            }

            rDays++;

            // Render MUMBAI (Row 5)
            int mumbaiRow = rDays;
            ws.Row(rDays).Height = 20;
            ws.Cells[rDays, startColDays].Value = "MUMBAI";
            ws.Cells[rDays, startColDays, rDays, startColDays + 7].Style.Font.Bold = true;
            ws.Cells[rDays, startColDays, rDays, startColDays + 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[rDays, startColDays, rDays, startColDays + 7].Style.Fill.BackgroundColor.SetColor(colLightGreen);

            // Set MUMBAI formulas
            ws.Cells[rDays, startColDays + 1].Formula = $"SUM({colAvailable}6:{colAvailable}11)"; // AVAILABLE
            ws.Cells[rDays, startColDays + 2].Formula = $"SUM({colOnHold}6:{colOnHold}11)"; // ONHOLD
            ws.Cells[rDays, startColDays + 3].Formula = $"SUM({colOnMemo}6:{colOnMemo}11)"; // ONMEMO
            ws.Cells[rDays, startColDays + 4].Formula = $"SUM({colTotPcs}6:{colTotPcs}11)"; // Total Pcs
            ws.Cells[rDays, startColDays + 5].Formula = $"SUM({colTotWt}6:{colTotWt}11)"; // Total Weight
            ws.Cells[rDays, startColDays + 6].Formula = $"SUM({colTotAmt}6:{colTotAmt}11)"; // Total $
            ws.Cells[rDays, startColDays + 7].Formula = $"IF({colTotAmt}$26>0,({colTotAmt}5/{colTotAmt}$26)*100,0)"; // %

            rDays++;

            // Render MUMBAI buckets (Rows 6-11)
            string[] buckets = { "1 TO 30 Days", "31 TO 59 Days", "60 TO 99 Days", "100 TO 199 Days", "200 TO 299 Days", "300 Days+" };
            for (int b = 0; b < buckets.Length; b++)
            {
                string bucketName = buckets[b];
                ws.Row(rDays).Height = 20;
                ws.Cells[rDays, startColDays].Value = bucketName;

                // Find data row for MUMBAI and this bucket
                System.Data.DataRow[] foundRows = dtReportDays.Select("Location = 'MUMBAI' AND DayBucket = '" + bucketName + "'");
                if (foundRows.Length > 0)
                {
                    System.Data.DataRow dr = foundRows[0];
                    ws.Cells[rDays, startColDays + 1].Value = Convert.ToInt32(dr["AvailablePcs"]);
                    ws.Cells[rDays, startColDays + 2].Value = Convert.ToInt32(dr["OnHoldPcs"]);
                    ws.Cells[rDays, startColDays + 3].Value = Convert.ToInt32(dr["OnMemoPcs"]);
                    ws.Cells[rDays, startColDays + 5].Value = Convert.ToDecimal(dr["TotalWeight"]);
                    ws.Cells[rDays, startColDays + 6].Value = Convert.ToDecimal(dr["TotalAmt"]);
                }
                else
                {
                    ws.Cells[rDays, startColDays + 1].Value = 0;
                    ws.Cells[rDays, startColDays + 2].Value = 0;
                    ws.Cells[rDays, startColDays + 3].Value = 0;
                    ws.Cells[rDays, startColDays + 5].Value = 0;
                    ws.Cells[rDays, startColDays + 6].Value = 0;
                }

                // Total Pcs and % formulas
                ws.Cells[rDays, startColDays + 4].Formula = $"SUM({colAvailable}{rDays}:{colOnMemo}{rDays})";
                ws.Cells[rDays, startColDays + 7].Formula = $"IF({colTotAmt}$26>0,({colTotAmt}{rDays}/{colTotAmt}$26)*100,0)";

                rDays++;
            }

            // Render NY (Row 12)
            int nyRow = rDays;
            ws.Row(rDays).Height = 20;
            ws.Cells[rDays, startColDays].Value = "NY";
            ws.Cells[rDays, startColDays, rDays, startColDays + 7].Style.Font.Bold = true;
            ws.Cells[rDays, startColDays, rDays, startColDays + 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[rDays, startColDays, rDays, startColDays + 7].Style.Fill.BackgroundColor.SetColor(colLightGreen);

            // Set NY formulas
            ws.Cells[rDays, startColDays + 1].Formula = $"SUM({colAvailable}13:{colAvailable}18)"; // AVAILABLE
            ws.Cells[rDays, startColDays + 2].Formula = $"SUM({colOnHold}13:{colOnHold}18)"; // ONHOLD
            ws.Cells[rDays, startColDays + 3].Formula = $"SUM({colOnMemo}13:{colOnMemo}18)"; // ONMEMO
            ws.Cells[rDays, startColDays + 4].Formula = $"SUM({colTotPcs}13:{colTotPcs}18)"; // Total Pcs
            ws.Cells[rDays, startColDays + 5].Formula = $"SUM({colTotWt}13:{colTotWt}18)"; // Total Weight
            ws.Cells[rDays, startColDays + 6].Formula = $"SUM({colTotAmt}13:{colTotAmt}18)"; // Total $
            ws.Cells[rDays, startColDays + 7].Formula = $"IF({colTotAmt}$26>0,({colTotAmt}12/{colTotAmt}$26)*100,0)"; // %

            rDays++;

            // Render NY buckets (Rows 13-18)
            for (int b = 0; b < buckets.Length; b++)
            {
                string bucketName = buckets[b];
                ws.Row(rDays).Height = 20;
                ws.Cells[rDays, startColDays].Value = bucketName;

                // Find data row for NY and this bucket
                System.Data.DataRow[] foundRows = dtReportDays.Select("Location = 'NY' AND DayBucket = '" + bucketName + "'");
                if (foundRows.Length > 0)
                {
                    System.Data.DataRow dr = foundRows[0];
                    ws.Cells[rDays, startColDays + 1].Value = Convert.ToInt32(dr["AvailablePcs"]);
                    ws.Cells[rDays, startColDays + 2].Value = Convert.ToInt32(dr["OnHoldPcs"]);
                    ws.Cells[rDays, startColDays + 3].Value = Convert.ToInt32(dr["OnMemoPcs"]);
                    ws.Cells[rDays, startColDays + 5].Value = Convert.ToDecimal(dr["TotalWeight"]);
                    ws.Cells[rDays, startColDays + 6].Value = Convert.ToDecimal(dr["TotalAmt"]);
                }
                else
                {
                    ws.Cells[rDays, startColDays + 1].Value = 0;
                    ws.Cells[rDays, startColDays + 2].Value = 0;
                    ws.Cells[rDays, startColDays + 3].Value = 0;
                    ws.Cells[rDays, startColDays + 5].Value = 0;
                    ws.Cells[rDays, startColDays + 6].Value = 0;
                }

                // Total Pcs and % formulas
                ws.Cells[rDays, startColDays + 4].Formula = $"SUM({colAvailable}{rDays}:{colOnMemo}{rDays})";
                ws.Cells[rDays, startColDays + 7].Formula = $"IF({colTotAmt}$26>0,({colTotAmt}{rDays}/{colTotAmt}$26)*100,0)";

                rDays++;
            }

            // Render SURAT (Row 19)
            int suratRow = rDays;
            ws.Row(rDays).Height = 20;
            ws.Cells[rDays, startColDays].Value = "SURAT";
            ws.Cells[rDays, startColDays, rDays, startColDays + 7].Style.Font.Bold = true;
            ws.Cells[rDays, startColDays, rDays, startColDays + 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[rDays, startColDays, rDays, startColDays + 7].Style.Fill.BackgroundColor.SetColor(colLightGreen);

            // Set SURAT formulas
            ws.Cells[rDays, startColDays + 1].Formula = $"SUM({colAvailable}20:{colAvailable}25)"; // AVAILABLE
            ws.Cells[rDays, startColDays + 2].Formula = $"SUM({colOnHold}20:{colOnHold}25)"; // ONHOLD
            ws.Cells[rDays, startColDays + 3].Formula = $"SUM({colOnMemo}20:{colOnMemo}25)"; // ONMEMO
            ws.Cells[rDays, startColDays + 4].Formula = $"SUM({colTotPcs}20:{colTotPcs}25)"; // Total Pcs
            ws.Cells[rDays, startColDays + 5].Formula = $"SUM({colTotWt}20:{colTotWt}25)"; // Total Weight
            ws.Cells[rDays, startColDays + 6].Formula = $"SUM({colTotAmt}20:{colTotAmt}25)"; // Total $
            ws.Cells[rDays, startColDays + 7].Formula = $"IF({colTotAmt}$26>0,({colTotAmt}19/{colTotAmt}$26)*100,0)"; // %

            rDays++;

            // Render SURAT buckets (Rows 20-25)
            for (int b = 0; b < buckets.Length; b++)
            {
                string bucketName = buckets[b];
                ws.Row(rDays).Height = 20;
                ws.Cells[rDays, startColDays].Value = bucketName;

                // Find data row for SURAT and this bucket
                System.Data.DataRow[] foundRows = dtReportDays.Select("Location = 'SURAT' AND DayBucket = '" + bucketName + "'");
                if (foundRows.Length > 0)
                {
                    System.Data.DataRow dr = foundRows[0];
                    ws.Cells[rDays, startColDays + 1].Value = Convert.ToInt32(dr["AvailablePcs"]);
                    ws.Cells[rDays, startColDays + 2].Value = Convert.ToInt32(dr["OnHoldPcs"]);
                    ws.Cells[rDays, startColDays + 3].Value = Convert.ToInt32(dr["OnMemoPcs"]);
                    ws.Cells[rDays, startColDays + 5].Value = Convert.ToDecimal(dr["TotalWeight"]);
                    ws.Cells[rDays, startColDays + 6].Value = Convert.ToDecimal(dr["TotalAmt"]);
                }
                else
                {
                    ws.Cells[rDays, startColDays + 1].Value = 0;
                    ws.Cells[rDays, startColDays + 2].Value = 0;
                    ws.Cells[rDays, startColDays + 3].Value = 0;
                    ws.Cells[rDays, startColDays + 5].Value = 0;
                    ws.Cells[rDays, startColDays + 6].Value = 0;
                }

                // Total Pcs and % formulas
                ws.Cells[rDays, startColDays + 4].Formula = $"SUM({colAvailable}{rDays}:{colOnMemo}{rDays})";
                ws.Cells[rDays, startColDays + 7].Formula = $"IF({colTotAmt}$26>0,({colTotAmt}{rDays}/{colTotAmt}$26)*100,0)";

                rDays++;
            }

            // Render TOTAL row (Row 26)
            int totalRowDays = rDays;
            ws.Row(rDays).Height = 22;
            ws.Cells[rDays, startColDays].Value = "TOTAL";
            ws.Cells[rDays, startColDays, rDays, startColDays + 7].Style.Font.Bold = true;
            ws.Cells[rDays, startColDays, rDays, startColDays + 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[rDays, startColDays, rDays, startColDays + 7].Style.Fill.BackgroundColor.SetColor(colLightBlue);

            // Sum totals from Mumbai, NY and SURAT headers
            ws.Cells[rDays, startColDays + 1].Formula = $"{colAvailable}5+{colAvailable}12+{colAvailable}19"; // AVAILABLE Total
            ws.Cells[rDays, startColDays + 2].Formula = $"{colOnHold}5+{colOnHold}12+{colOnHold}19"; // ONHOLD Total
            ws.Cells[rDays, startColDays + 3].Formula = $"{colOnMemo}5+{colOnMemo}12+{colOnMemo}19"; // ONMEMO Total
            ws.Cells[rDays, startColDays + 4].Formula = $"{colTotPcs}5+{colTotPcs}12+{colTotPcs}19"; // Total Pcs Total
            ws.Cells[rDays, startColDays + 5].Formula = $"{colTotWt}5+{colTotWt}12+{colTotWt}19"; // Total Weight Total
            ws.Cells[rDays, startColDays + 6].Formula = $"{colTotAmt}5+{colTotAmt}12+{colTotAmt}19"; // Total $ Total
            ws.Cells[rDays, startColDays + 7].Value = 100; // % Total is 100

            // Apply number formatting and borders to REPORT DAYS table
            for (int r = 4; r <= totalRowDays; r++)
            {
                ws.Cells[r, startColDays + 1].Style.Numberformat.Format = "#,##0;(#,##0);\"\"";
                ws.Cells[r, startColDays + 2].Style.Numberformat.Format = "#,##0;(#,##0);\"\"";
                ws.Cells[r, startColDays + 3].Style.Numberformat.Format = "#,##0;(#,##0);\"\"";
                ws.Cells[r, startColDays + 4].Style.Numberformat.Format = "#,##0;(#,##0);\"\"";
                ws.Cells[r, startColDays + 5].Style.Numberformat.Format = "#,##0.00;(#,##0.00);\"\"";
                ws.Cells[r, startColDays + 6].Style.Numberformat.Format = "#,##0;(#,##0);\"\"";
                ws.Cells[r, startColDays + 7].Style.Numberformat.Format = "0.00";
            }
            SetBorders(ws, 4, startColDays, totalRowDays, startColDays + 7);

            // === DRAW STATUS TABLE ===
            int rStatus = totalRowDays + 3; // 29
            int startColStatus = 22;

            string colStatusPcs = ExcelCellAddress.GetColumnLetter(startColStatus + 2); // X (24)
            string colStatusWt = ExcelCellAddress.GetColumnLetter(startColStatus + 3);  // Y (25)
            string colStatusAmt = ExcelCellAddress.GetColumnLetter(startColStatus + 4); // Z (26)
            string colStatusPct = ExcelCellAddress.GetColumnLetter(startColStatus + 5); // AA (27)

            // Headers
            ws.Row(rStatus).Height = 25;
            ws.Cells[rStatus, startColStatus].Value = "STATUS";
            ws.Cells[rStatus, startColStatus].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[rStatus, startColStatus].Style.Fill.BackgroundColor.SetColor(colGreenHeader);
            ws.Cells[rStatus, startColStatus].Style.Font.Bold = true;

            ws.Cells[rStatus, startColStatus + 1].Value = "LOCATION";
            ws.Cells[rStatus, startColStatus + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[rStatus, startColStatus + 1].Style.Fill.BackgroundColor.SetColor(colGreenHeader);
            ws.Cells[rStatus, startColStatus + 1].Style.Font.Bold = true;

            string[] headersStatus = { "PCS", "WEIGHT", "TOTAL $", "%" };
            for (int h = 0; h < headersStatus.Length; h++)
            {
                int colIdx = startColStatus + 2 + h;
                ws.Cells[rStatus, colIdx].Value = headersStatus[h];
                ws.Cells[rStatus, colIdx].Style.Font.Bold = true;
                ws.Cells[rStatus, colIdx].Style.Fill.PatternType = ExcelFillStyle.Solid;

                if (headersStatus[h] == "TOTAL $" || headersStatus[h] == "%")
                {
                    ws.Cells[rStatus, colIdx].Style.Fill.BackgroundColor.SetColor(colBlack);
                    ws.Cells[rStatus, colIdx].Style.Font.Color.SetColor(colWhite);
                }
                else
                {
                    ws.Cells[rStatus, colIdx].Style.Fill.BackgroundColor.SetColor(colPinkHeader);
                }
            }

            rStatus++;

            // Render status categories: AVAILABLE, MEMO, HOLD, OFFLINE
            int dataStartRowStatus = rStatus;
            string[] statuses = { "AVAILABLE", "MEMO", "HOLD", "OFFLINE" };
            string[] locations = { "MUMBAI", "NEWYORK", "SURAT" };

            for (int s = 0; s < statuses.Length; s++)
            {
                string statusName = statuses[s];
                int sStartRow = rStatus;

                for (int l = 0; l < locations.Length; l++)
                {
                    string locName = locations[l];
                    ws.Row(rStatus).Height = 20;

                    // Write location
                    ws.Cells[rStatus, startColStatus + 1].Value = locName;

                    // Query value from parsed rawStock to ensure perfectly filtered data
                    var statusData = parsed.Where(x => x.StockStatus == statusName && x.Location == locName).ToList();
                    
                    if (statusData.Count > 0)
                    {
                        ws.Cells[rStatus, startColStatus + 2].Value = statusData.Count;
                        ws.Cells[rStatus, startColStatus + 3].Value = statusData.Sum(x => x.Weight);
                        ws.Cells[rStatus, startColStatus + 4].Value = statusData.Sum(x => x.SaleAmt);
                    }
                    else
                    {
                        ws.Cells[rStatus, startColStatus + 2].Value = 0;
                        ws.Cells[rStatus, startColStatus + 3].Value = 0;
                        ws.Cells[rStatus, startColStatus + 4].Value = 0;
                    }

                    // % formula: TOTAL $ / Grand Total $ * 100
                    int totalRowIndex = dataStartRowStatus + (statuses.Length * locations.Length);
                    ws.Cells[rStatus, startColStatus + 5].Formula = $"IF({colStatusAmt}${totalRowIndex}>0,({colStatusAmt}{rStatus}/{colStatusAmt}${totalRowIndex})*100,0)";

                    rStatus++;
                }

                // Merge the Status cells vertically
                ws.Cells[sStartRow, startColStatus, sStartRow + locations.Length - 1, startColStatus].Merge = true;
                ws.Cells[sStartRow, startColStatus].Value = statusName;
            }

            // Render Total row
            int totalRowStatus = rStatus;
            ws.Row(rStatus).Height = 22;
            
            ws.Cells[rStatus, startColStatus, rStatus, startColStatus + 1].Merge = true;
            ws.Cells[rStatus, startColStatus].Value = "Total";
            
            ws.Cells[rStatus, startColStatus, rStatus, startColStatus + 5].Style.Font.Bold = true;
            ws.Cells[rStatus, startColStatus, rStatus, startColStatus + 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[rStatus, startColStatus, rStatus, startColStatus + 5].Style.Fill.BackgroundColor.SetColor(colLightGray);

            // Sum formulas
            ws.Cells[rStatus, startColStatus + 2].Formula = $"SUM({colStatusPcs}{dataStartRowStatus}:{colStatusPcs}{rStatus - 1})";
            ws.Cells[rStatus, startColStatus + 3].Formula = $"SUM({colStatusWt}{dataStartRowStatus}:{colStatusWt}{rStatus - 1})";
            ws.Cells[rStatus, startColStatus + 4].Formula = $"SUM({colStatusAmt}{dataStartRowStatus}:{colStatusAmt}{rStatus - 1})";
            ws.Cells[rStatus, startColStatus + 5].Value = 100; // % Total is 100

            // Apply formats and borders
            for (int r = dataStartRowStatus - 1; r <= totalRowStatus; r++)
            {
                ws.Cells[r, startColStatus + 2].Style.Numberformat.Format = "#,##0;(#,##0);\"\"";
                ws.Cells[r, startColStatus + 3].Style.Numberformat.Format = "#,##0.00;(#,##0.00);\"\"";
                ws.Cells[r, startColStatus + 4].Style.Numberformat.Format = "#,##0;(#,##0);\"\"";
                ws.Cells[r, startColStatus + 5].Style.Numberformat.Format = "0.00";
            }
            SetBorders(ws, dataStartRowStatus - 1, startColStatus, totalRowStatus, startColStatus + 5);

            // Set Column Widths for Columns 21-29
            ws.Column(21).Width = 4;  // spacer
            ws.Column(22).Width = 20; // REPORT DAYS / STATUS
            ws.Column(23).Width = 15; // AVAILABLE / LOCATION
            ws.Column(24).Width = 12; // ONHOLD / PCS
            ws.Column(25).Width = 12; // ONMEMO / WEIGHT
            ws.Column(26).Width = 12; // Total Pcs / TOTAL $
            ws.Column(27).Width = 15; // Total Weight / %
            ws.Column(28).Width = 15; // Total $
            ws.Column(29).Width = 10; // %
        }

        private static int DrawSummaryTable<T>(
            ExcelWorksheet ws,
            int startRow,
            int startCol,
            string headerName,
            List<T> items,
            Func<T, string> nameSelector,
            Func<T, int> pcsSelector,
            Func<T, decimal> weightSelector,
            Func<T, decimal> amtSelector,
            string totalLabel = "Total")
        {
            Color colGreenHeader = System.Drawing.ColorTranslator.FromHtml("#A9D08E");
            Color colPinkHeader = System.Drawing.ColorTranslator.FromHtml("#E4DFEC");
            Color colBlack = System.Drawing.ColorTranslator.FromHtml("#000000");
            Color colWhite = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");

            // 1. Headers Row
            int r = startRow;
            ws.Row(r).Height = 25;

            // Column 1: name (e.g. SIZE, SHAPE)
            var c1 = ws.Cells[r, startCol];
            c1.Value = headerName;
            c1.Style.Font.Bold = true;
            c1.Style.Fill.PatternType = ExcelFillStyle.Solid;
            c1.Style.Fill.BackgroundColor.SetColor(colGreenHeader);

            // Column 2: Pcs
            var c2 = ws.Cells[r, startCol + 1];
            c2.Value = "Pcs";
            c2.Style.Font.Bold = true;
            c2.Style.Fill.PatternType = ExcelFillStyle.Solid;
            c2.Style.Fill.BackgroundColor.SetColor(colPinkHeader);

            // Column 3: Weight
            var c3 = ws.Cells[r, startCol + 2];
            c3.Value = "Weight";
            c3.Style.Font.Bold = true;
            c3.Style.Fill.PatternType = ExcelFillStyle.Solid;
            c3.Style.Fill.BackgroundColor.SetColor(colPinkHeader);

            // Column 4: Total $
            var c4 = ws.Cells[r, startCol + 3];
            c4.Value = (headerName == "LOCATION" || headerName == "CUT") ? "TOTAL $" : "Total $";
            c4.Style.Font.Bold = true;
            c4.Style.Font.Color.SetColor(colWhite);
            c4.Style.Fill.PatternType = ExcelFillStyle.Solid;
            c4.Style.Fill.BackgroundColor.SetColor(colBlack);

            // Column 5: Per Ct $
            var c5 = ws.Cells[r, startCol + 4];
            c5.Value = "Per Ct $";
            c5.Style.Font.Bold = true;
            c5.Style.Font.Color.SetColor(colWhite);
            c5.Style.Fill.PatternType = ExcelFillStyle.Solid;
            c5.Style.Fill.BackgroundColor.SetColor(colBlack);

            // Column 6: %
            var c6 = ws.Cells[r, startCol + 5];
            c6.Value = "%";
            c6.Style.Font.Bold = true;
            c6.Style.Font.Color.SetColor(colWhite);
            c6.Style.Fill.PatternType = ExcelFillStyle.Solid;
            c6.Style.Fill.BackgroundColor.SetColor(colBlack);

            r++;

            // 2. Data Rows
            int dataStartRow = r;
            string wtColLetter = ExcelCellAddress.GetColumnLetter(startCol + 2);
            string amtColLetter = ExcelCellAddress.GetColumnLetter(startCol + 3);
            string perCtColLetter = ExcelCellAddress.GetColumnLetter(startCol + 4);
            string pctColLetter = ExcelCellAddress.GetColumnLetter(startCol + 5);

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                ws.Row(r).Height = 20;

                ws.Cells[r, startCol].Value = nameSelector(item);
                ws.Cells[r, startCol + 1].Value = pcsSelector(item);
                ws.Cells[r, startCol + 2].Value = weightSelector(item);
                ws.Cells[r, startCol + 3].Value = Math.Round(amtSelector(item), 0);

                // Per Ct $ formula: TotalAmt / Weight
                ws.Cells[r, startCol + 4].Formula = $"IF({wtColLetter}{r}>0,{amtColLetter}{r}/{wtColLetter}{r},0)";

                // Formatting
                ws.Cells[r, startCol + 1].Style.Numberformat.Format = "#,##0;(#,##0);\"-\"";
                ws.Cells[r, startCol + 2].Style.Numberformat.Format = "#,##0.00;(#,##0.00);\"-\"";
                ws.Cells[r, startCol + 3].Style.Numberformat.Format = "#,##0;(#,##0);\"-\"";
                ws.Cells[r, startCol + 4].Style.Numberformat.Format = "#,##0.00;(#,##0.00);\"-\"";
                ws.Cells[r, startCol + 5].Style.Numberformat.Format = "0.00";

                r++;
            }

            int totalRow = r;
            ws.Row(totalRow).Height = 22;

            // 3. Total Row
            var tLabelCell = ws.Cells[totalRow, startCol];
            tLabelCell.Value = totalLabel;
            tLabelCell.Style.Font.Bold = true;
            tLabelCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            tLabelCell.Style.Fill.BackgroundColor.SetColor(colPinkHeader);

            var tPcsCell = ws.Cells[totalRow, startCol + 1];
            tPcsCell.Formula = $"SUM({ExcelCellAddress.GetColumnLetter(startCol + 1)}{dataStartRow}:{ExcelCellAddress.GetColumnLetter(startCol + 1)}{totalRow - 1})";
            tPcsCell.Style.Font.Bold = true;
            tPcsCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            tPcsCell.Style.Fill.BackgroundColor.SetColor(colPinkHeader);
            tPcsCell.Style.Numberformat.Format = "#,##0;(#,##0);\"-\"";

            var tWeightCell = ws.Cells[totalRow, startCol + 2];
            tWeightCell.Formula = $"SUM({ExcelCellAddress.GetColumnLetter(startCol + 2)}{dataStartRow}:{ExcelCellAddress.GetColumnLetter(startCol + 2)}{totalRow - 1})";
            tWeightCell.Style.Font.Bold = true;
            tWeightCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            tWeightCell.Style.Fill.BackgroundColor.SetColor(colPinkHeader);
            tWeightCell.Style.Numberformat.Format = "#,##0.00;(#,##0.00);\"-\"";

            var tAmtCell = ws.Cells[totalRow, startCol + 3];
            tAmtCell.Formula = $"SUM({ExcelCellAddress.GetColumnLetter(startCol + 3)}{dataStartRow}:{ExcelCellAddress.GetColumnLetter(startCol + 3)}{totalRow - 1})";
            tAmtCell.Style.Font.Bold = true;
            tAmtCell.Style.Font.Color.SetColor(colWhite);
            tAmtCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            tAmtCell.Style.Fill.BackgroundColor.SetColor(colBlack);
            tAmtCell.Style.Numberformat.Format = "#,##0;(#,##0);\"-\"";

            // Total Per Ct $: TotalAmt / TotalWeight
            var tPerCtCell = ws.Cells[totalRow, startCol + 4];
            tPerCtCell.Formula = $"IF({wtColLetter}{totalRow}>0,{amtColLetter}{totalRow}/{wtColLetter}{totalRow},0)";
            tPerCtCell.Style.Font.Bold = true;
            tPerCtCell.Style.Font.Color.SetColor(colWhite);
            tPerCtCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            tPerCtCell.Style.Fill.BackgroundColor.SetColor(colBlack);
            tPerCtCell.Style.Numberformat.Format = "#,##0.00;(#,##0.00);\"-\"";

            var tPctCell = ws.Cells[totalRow, startCol + 5];
            tPctCell.Value = 100;
            tPctCell.Style.Font.Bold = true;
            tPctCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            tPctCell.Style.Fill.BackgroundColor.SetColor(colPinkHeader);
            tPctCell.Style.Numberformat.Format = "0.00";

            // 4. Fill % Formulas for Data Rows
            string totalAmtCellAddress = $"{amtColLetter}{totalRow}";
            for (int rowIdx = dataStartRow; rowIdx < totalRow; rowIdx++)
            {
                ws.Cells[rowIdx, startCol + 5].Formula = $"IF({totalAmtCellAddress}>0,({amtColLetter}{rowIdx}/{totalAmtCellAddress})*100,0)";
            }

            // 5. Borders
            SetBorders(ws, startRow, startCol, totalRow, startCol + 5);

            return totalRow;
        }

        public static string MemoHoldSummaryExport(List<DmxMemoHoldSummaryItem> list, string date)
        {
            try
            {
                ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");
                
                DateTime reportDate = DateTime.Today;
                if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out DateTime parsedDate))
                {
                    reportDate = parsedDate;
                }

                using (ExcelPackage p = new ExcelPackage())
                {
                    p.Workbook.Properties.Author = "Diamx Stock";
                    p.Workbook.Properties.Title = "MEMO & HOLD Stock Summary";

                    ExcelWorksheet ws = p.Workbook.Worksheets.Add("Summary");
                    ws.View.ShowGridLines = true;
                    
                    ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    Color colDarkGray = System.Drawing.ColorTranslator.FromHtml("#404040");
                    Color colWhite = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                    Color colBlack = System.Drawing.ColorTranslator.FromHtml("#000000");
                    Color colMumbaiHeader = System.Drawing.ColorTranslator.FromHtml("#FFC000"); // Gold
                    Color colSuratHeader = System.Drawing.ColorTranslator.FromHtml("#92D050"); // Light Green
                    Color colNyHeader = System.Drawing.ColorTranslator.FromHtml("#4472C4"); // Blue
                    
                    Color colMumbaiHold = System.Drawing.ColorTranslator.FromHtml("#FFE699"); // Light Yellow
                    Color colMumbaiMemo = System.Drawing.ColorTranslator.FromHtml("#F8CBAD"); // Light Orange
                    Color colMumbaiTotal = System.Drawing.ColorTranslator.FromHtml("#F4B084"); // Medium Orange
                    
                    Color colSuratHold = System.Drawing.ColorTranslator.FromHtml("#E2EFDA"); // Very Light Green
                    Color colSuratMemo = System.Drawing.ColorTranslator.FromHtml("#C6E0B4"); // Light Green 2
                    Color colSuratTotal = System.Drawing.ColorTranslator.FromHtml("#A9D08E"); // Medium Green
                    
                    Color colNyHold = System.Drawing.ColorTranslator.FromHtml("#BDD7EE"); // Light Blue
                    Color colNyMemo = System.Drawing.ColorTranslator.FromHtml("#D9E1F2"); // Light Blue-Gray
                    Color colNyTotal = System.Drawing.ColorTranslator.FromHtml("#9BC2E6"); // Medium Blue
                    
                    Color colHeaderBG = System.Drawing.ColorTranslator.FromHtml("#D9D9D9");
                    Color colTotalRow = System.Drawing.ColorTranslator.FromHtml("#F2F2F2");
                    Color colSellerParent = System.Drawing.ColorTranslator.FromHtml("#DDEBF7"); // Light Blue for Seller Parent
                    Color colSellerHeader = System.Drawing.ColorTranslator.FromHtml("#FFFF00"); // Yellow for SELLER Header

                    // Title Header
                    ws.Cells[1, 1, 1, 26].Merge = true;
                    ws.Cells[1, 1].Value = reportDate.ToString("dd-MM-yyyy") + " (IGI) MEMO & HOLD";
                    ws.Cells[1, 1].Style.Font.Bold = true;
                    ws.Cells[1, 1].Style.Font.Size = 16;
                    ws.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(colDarkGray);
                    ws.Cells[1, 1].Style.Font.Color.SetColor(colWhite);

                    // Row 2-4: Headers
                    ws.Cells[2, 1, 4, 1].Merge = true;
                    ws.Cells[2, 1].Value = "SELLER";
                    ws.Cells[2, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[2, 1].Style.Fill.BackgroundColor.SetColor(colSellerHeader);
                    ws.Cells[2, 1].Style.Font.Bold = true;
                    
                    ws.Cells[2, 2, 2, 8].Merge = true;
                    ws.Cells[2, 2].Value = "MUMBAI";
                    ws.Cells[2, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[2, 2].Style.Fill.BackgroundColor.SetColor(colMumbaiHeader);
                    ws.Cells[2, 2].Style.Font.Bold = true;

                    ws.Cells[2, 9, 2, 15].Merge = true;
                    ws.Cells[2, 9].Value = "SURAT";
                    ws.Cells[2, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[2, 9].Style.Fill.BackgroundColor.SetColor(colSuratHeader);
                    ws.Cells[2, 9].Style.Font.Bold = true;

                    ws.Cells[2, 16, 2, 22].Merge = true;
                    ws.Cells[2, 16].Value = "NY";
                    ws.Cells[2, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[2, 16].Style.Fill.BackgroundColor.SetColor(colNyHeader);
                    ws.Cells[2, 16].Style.Font.Bold = true;
                    ws.Cells[2, 16].Style.Font.Color.SetColor(colWhite);

                    ws.Cells[2, 23, 4, 23].Merge = true;
                    ws.Cells[2, 23].Value = "Total Pcs";

                    ws.Cells[2, 24, 4, 24].Merge = true;
                    ws.Cells[2, 24].Value = "Total Weight";

                    ws.Cells[2, 25, 4, 25].Merge = true;
                    ws.Cells[2, 25].Value = "Total Total $";

                    ws.Cells[2, 26, 4, 26].Merge = true;
                    ws.Cells[2, 26].Value = "Per Ct";

                    // Row 3
                    ws.Cells[3, 2, 3, 4].Merge = true;
                    ws.Cells[3, 2].Value = "ONHOLD";
                    ws.Cells[3, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[3, 2].Style.Fill.BackgroundColor.SetColor(colMumbaiHold);
                    ws.Cells[3, 2].Style.Font.Bold = true;

                    ws.Cells[3, 5, 3, 7].Merge = true;
                    ws.Cells[3, 5].Value = "ONMEMO";
                    ws.Cells[3, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[3, 5].Style.Fill.BackgroundColor.SetColor(colMumbaiMemo);
                    ws.Cells[3, 5].Style.Font.Bold = true;

                    ws.Cells[3, 8, 4, 8].Merge = true;
                    ws.Cells[3, 8].Value = "Total $";
                    ws.Cells[3, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[3, 8].Style.Fill.BackgroundColor.SetColor(colMumbaiTotal);
                    ws.Cells[3, 8].Style.Font.Bold = true;

                    ws.Cells[3, 9, 3, 11].Merge = true;
                    ws.Cells[3, 9].Value = "ONHOLD";
                    ws.Cells[3, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[3, 9].Style.Fill.BackgroundColor.SetColor(colSuratHold);
                    ws.Cells[3, 9].Style.Font.Bold = true;

                    ws.Cells[3, 12, 3, 14].Merge = true;
                    ws.Cells[3, 12].Value = "ONMEMO";
                    ws.Cells[3, 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[3, 12].Style.Fill.BackgroundColor.SetColor(colSuratMemo);
                    ws.Cells[3, 12].Style.Font.Bold = true;

                    ws.Cells[3, 15, 4, 15].Merge = true;
                    ws.Cells[3, 15].Value = "Total $";
                    ws.Cells[3, 15].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[3, 15].Style.Fill.BackgroundColor.SetColor(colSuratTotal);
                    ws.Cells[3, 15].Style.Font.Bold = true;

                    ws.Cells[3, 16, 3, 18].Merge = true;
                    ws.Cells[3, 16].Value = "ONHOLD";
                    ws.Cells[3, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[3, 16].Style.Fill.BackgroundColor.SetColor(colNyHold);
                    ws.Cells[3, 16].Style.Font.Bold = true;

                    ws.Cells[3, 19, 3, 21].Merge = true;
                    ws.Cells[3, 19].Value = "ONMEMO";
                    ws.Cells[3, 19].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[3, 19].Style.Fill.BackgroundColor.SetColor(colNyMemo);
                    ws.Cells[3, 19].Style.Font.Bold = true;

                    ws.Cells[3, 22, 4, 22].Merge = true;
                    ws.Cells[3, 22].Value = "Total $";
                    ws.Cells[3, 22].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[3, 22].Style.Fill.BackgroundColor.SetColor(colNyTotal);
                    ws.Cells[3, 22].Style.Font.Bold = true;

                    // Row 4
                    ws.Cells[4, 2].Value = "Pcs";
                    ws.Cells[4, 3].Value = "Weight";
                    ws.Cells[4, 4].Value = "Total $";
                    
                    ws.Cells[4, 5].Value = "Pcs";
                    ws.Cells[4, 6].Value = "Weight";
                    ws.Cells[4, 7].Value = "Total $";

                    ws.Cells[4, 9].Value = "Pcs";
                    ws.Cells[4, 10].Value = "Weight";
                    ws.Cells[4, 11].Value = "Total $";
                    
                    ws.Cells[4, 12].Value = "Pcs";
                    ws.Cells[4, 13].Value = "Weight";
                    ws.Cells[4, 14].Value = "Total $";
                    
                    ws.Cells[4, 16].Value = "Pcs";
                    ws.Cells[4, 17].Value = "Weight";
                    ws.Cells[4, 18].Value = "Total $";
                    
                    ws.Cells[4, 19].Value = "Pcs";
                    ws.Cells[4, 20].Value = "Weight";
                    ws.Cells[4, 21].Value = "Total $";

                    var mergedHeaders = new[] { 23, 24, 25, 26 };
                    foreach (var col in mergedHeaders)
                    {
                        ws.Cells[2, col, 4, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[2, col, 4, col].Style.Fill.BackgroundColor.SetColor(colHeaderBG);
                        ws.Cells[2, col, 4, col].Style.Font.Bold = true;
                    }

                    for (int col = 2; col <= 7; col++)
                    {
                        ws.Cells[4, col].Style.Font.Bold = true;
                        ws.Cells[4, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[4, col].Style.Fill.BackgroundColor.SetColor(col == 2 || col == 3 || col == 4 ? colMumbaiHold : colMumbaiMemo);
                    }
                    for (int col = 9; col <= 14; col++)
                    {
                        ws.Cells[4, col].Style.Font.Bold = true;
                        ws.Cells[4, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[4, col].Style.Fill.BackgroundColor.SetColor(col == 9 || col == 10 || col == 11 ? colSuratHold : colSuratMemo);
                    }
                    for (int col = 16; col <= 21; col++)
                    {
                        ws.Cells[4, col].Style.Font.Bold = true;
                        ws.Cells[4, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[4, col].Style.Fill.BackgroundColor.SetColor(col == 16 || col == 17 || col == 18 ? colNyHold : colNyMemo);
                    }

                    string fmtPcs = "#,##0;-#,##0;\"\"";
                    string fmtWt = "#,##0.000;-#,##0.000;\"\"";
                    string fmtAmt = "#,##0;-#,##0;\"\"";
                    string fmtRate = "#,##0.00;-#,##0.00;\"\"";

                    var grouped = list
                        .Where(x => !string.IsNullOrEmpty(x.Sellar))
                        .GroupBy(x => x.Sellar)
                        .Select(g => new
                        {
                            SellerName = g.Key,
                            Dates = g.GroupBy(d => d.EntryDate.HasValue ? d.EntryDate.Value.Date : DateTime.MinValue)
                                     .Select(dg => {
                                         var dList = dg.ToList();
                                         return new
                                         {
                                             EntryDate = dg.Key,
                                             MumbaiHoldPcs = dList.Where(x => string.Equals(x.Branch, "MUMBAI", StringComparison.OrdinalIgnoreCase) && string.Equals(x.Status, "ONHOLD", StringComparison.OrdinalIgnoreCase)).Sum(x => x.PacketPcs),
                                             MumbaiHoldWt = dList.Where(x => string.Equals(x.Branch, "MUMBAI", StringComparison.OrdinalIgnoreCase) && string.Equals(x.Status, "ONHOLD", StringComparison.OrdinalIgnoreCase)).Sum(x => x.Weight),
                                             MumbaiHoldAmt = dList.Where(x => string.Equals(x.Branch, "MUMBAI", StringComparison.OrdinalIgnoreCase) && string.Equals(x.Status, "ONHOLD", StringComparison.OrdinalIgnoreCase)).Sum(x => x.SaleAmt),
                                             
                                             MumbaiMemoPcs = dList.Where(x => string.Equals(x.Branch, "MUMBAI", StringComparison.OrdinalIgnoreCase) && string.Equals(x.Status, "ONMEMO", StringComparison.OrdinalIgnoreCase)).Sum(x => x.PacketPcs),
                                             MumbaiMemoWt = dList.Where(x => string.Equals(x.Branch, "MUMBAI", StringComparison.OrdinalIgnoreCase) && string.Equals(x.Status, "ONMEMO", StringComparison.OrdinalIgnoreCase)).Sum(x => x.Weight),
                                             MumbaiMemoAmt = dList.Where(x => string.Equals(x.Branch, "MUMBAI", StringComparison.OrdinalIgnoreCase) && string.Equals(x.Status, "ONMEMO", StringComparison.OrdinalIgnoreCase)).Sum(x => x.SaleAmt),
                                             
                                             SuratHoldPcs = dList.Where(x => string.Equals(x.Branch, "SURAT", StringComparison.OrdinalIgnoreCase) && string.Equals(x.Status, "ONHOLD", StringComparison.OrdinalIgnoreCase)).Sum(x => x.PacketPcs),
                                             SuratHoldWt = dList.Where(x => string.Equals(x.Branch, "SURAT", StringComparison.OrdinalIgnoreCase) && string.Equals(x.Status, "ONHOLD", StringComparison.OrdinalIgnoreCase)).Sum(x => x.Weight),
                                             SuratHoldAmt = dList.Where(x => string.Equals(x.Branch, "SURAT", StringComparison.OrdinalIgnoreCase) && string.Equals(x.Status, "ONHOLD", StringComparison.OrdinalIgnoreCase)).Sum(x => x.SaleAmt),
                                             
                                             SuratMemoPcs = dList.Where(x => string.Equals(x.Branch, "SURAT", StringComparison.OrdinalIgnoreCase) && string.Equals(x.Status, "ONMEMO", StringComparison.OrdinalIgnoreCase)).Sum(x => x.PacketPcs),
                                             SuratMemoWt = dList.Where(x => string.Equals(x.Branch, "SURAT", StringComparison.OrdinalIgnoreCase) && string.Equals(x.Status, "ONMEMO", StringComparison.OrdinalIgnoreCase)).Sum(x => x.Weight),
                                             SuratMemoAmt = dList.Where(x => string.Equals(x.Branch, "SURAT", StringComparison.OrdinalIgnoreCase) && string.Equals(x.Status, "ONMEMO", StringComparison.OrdinalIgnoreCase)).Sum(x => x.SaleAmt),
                                             
                                             NyHoldPcs = dList.Where(x => (string.Equals(x.Branch, "NY", StringComparison.OrdinalIgnoreCase) || string.Equals(x.Branch, "NEWYORK", StringComparison.OrdinalIgnoreCase)) && string.Equals(x.Status, "ONHOLD", StringComparison.OrdinalIgnoreCase)).Sum(x => x.PacketPcs),
                                             NyHoldWt = dList.Where(x => (string.Equals(x.Branch, "NY", StringComparison.OrdinalIgnoreCase) || string.Equals(x.Branch, "NEWYORK", StringComparison.OrdinalIgnoreCase)) && string.Equals(x.Status, "ONHOLD", StringComparison.OrdinalIgnoreCase)).Sum(x => x.Weight),
                                             NyHoldAmt = dList.Where(x => (string.Equals(x.Branch, "NY", StringComparison.OrdinalIgnoreCase) || string.Equals(x.Branch, "NEWYORK", StringComparison.OrdinalIgnoreCase)) && string.Equals(x.Status, "ONHOLD", StringComparison.OrdinalIgnoreCase)).Sum(x => x.SaleAmt),
                                             
                                             NyMemoPcs = dList.Where(x => (string.Equals(x.Branch, "NY", StringComparison.OrdinalIgnoreCase) || string.Equals(x.Branch, "NEWYORK", StringComparison.OrdinalIgnoreCase)) && string.Equals(x.Status, "ONMEMO", StringComparison.OrdinalIgnoreCase)).Sum(x => x.PacketPcs),
                                             NyMemoWt = dList.Where(x => (string.Equals(x.Branch, "NY", StringComparison.OrdinalIgnoreCase) || string.Equals(x.Branch, "NEWYORK", StringComparison.OrdinalIgnoreCase)) && string.Equals(x.Status, "ONMEMO", StringComparison.OrdinalIgnoreCase)).Sum(x => x.Weight),
                                             NyMemoAmt = dList.Where(x => (string.Equals(x.Branch, "NY", StringComparison.OrdinalIgnoreCase) || string.Equals(x.Branch, "NEWYORK", StringComparison.OrdinalIgnoreCase)) && string.Equals(x.Status, "ONMEMO", StringComparison.OrdinalIgnoreCase)).Sum(x => x.SaleAmt)
                                         };
                                     })
                                     .OrderBy(dg => dg.EntryDate)
                                     .ToList()
                        })
                        .OrderBy(sg => sg.SellerName)
                        .ToList();

                    int r = 5;
                    List<int> parentRows = new List<int>();

                    foreach (var sg in grouped)
                    {
                        int parentRowIdx = r;
                        parentRows.Add(parentRowIdx);

                        ws.Cells[parentRowIdx, 1].Value = sg.SellerName;
                        
                        int childStart = parentRowIdx + 1;
                        int childEnd = parentRowIdx + sg.Dates.Count;

                        for (int col = 2; col <= 25; col++)
                        {
                            string colLetter = GetColName(col);
                            ws.Cells[parentRowIdx, col].Formula = $"=SUM({colLetter}{childStart}:{colLetter}{childEnd})";
                        }
                        
                        ws.Cells[parentRowIdx, 26].Formula = $"=IF(X{parentRowIdx}>0, Y{parentRowIdx}/X{parentRowIdx}, 0)";

                        ws.Row(parentRowIdx).Style.Font.Bold = true;
                        ws.Cells[parentRowIdx, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[parentRowIdx, 1].Style.Fill.BackgroundColor.SetColor(colSellerParent);
                        
                        for (int col = 2; col <= 26; col++)
                        {
                            ws.Cells[parentRowIdx, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells[parentRowIdx, col].Style.Fill.BackgroundColor.SetColor(colTotalRow);
                        }

                        r++;

                        foreach (var dg in sg.Dates)
                        {
                            int childRowIdx = r;
                            
                            ws.Cells[childRowIdx, 1].Value = dg.EntryDate == DateTime.MinValue ? "N/A" : dg.EntryDate.ToString("dd/MM/yyyy");

                            ws.Cells[childRowIdx, 2].Value = dg.MumbaiHoldPcs;
                            ws.Cells[childRowIdx, 3].Value = dg.MumbaiHoldWt;
                            ws.Cells[childRowIdx, 4].Value = dg.MumbaiHoldAmt;

                            ws.Cells[childRowIdx, 5].Value = dg.MumbaiMemoPcs;
                            ws.Cells[childRowIdx, 6].Value = dg.MumbaiMemoWt;
                            ws.Cells[childRowIdx, 7].Value = dg.MumbaiMemoAmt;

                            // Mumbai Total $
                            ws.Cells[childRowIdx, 8].Formula = $"=D{childRowIdx}+G{childRowIdx}";

                            ws.Cells[childRowIdx, 9].Value = dg.SuratHoldPcs;
                            ws.Cells[childRowIdx, 10].Value = dg.SuratHoldWt;
                            ws.Cells[childRowIdx, 11].Value = dg.SuratHoldAmt;

                            ws.Cells[childRowIdx, 12].Value = dg.SuratMemoPcs;
                            ws.Cells[childRowIdx, 13].Value = dg.SuratMemoWt;
                            ws.Cells[childRowIdx, 14].Value = dg.SuratMemoAmt;

                            // Surat Total $
                            ws.Cells[childRowIdx, 15].Formula = $"=K{childRowIdx}+N{childRowIdx}";

                            ws.Cells[childRowIdx, 16].Value = dg.NyHoldPcs;
                            ws.Cells[childRowIdx, 17].Value = dg.NyHoldWt;
                            ws.Cells[childRowIdx, 18].Value = dg.NyHoldAmt;

                            ws.Cells[childRowIdx, 19].Value = dg.NyMemoPcs;
                            ws.Cells[childRowIdx, 20].Value = dg.NyMemoWt;
                            ws.Cells[childRowIdx, 21].Value = dg.NyMemoAmt;

                            // NY Total $
                            ws.Cells[childRowIdx, 22].Formula = $"=R{childRowIdx}+U{childRowIdx}";

                            // Total Pcs
                            ws.Cells[childRowIdx, 23].Formula = $"=B{childRowIdx}+E{childRowIdx}+I{childRowIdx}+L{childRowIdx}+P{childRowIdx}+S{childRowIdx}";

                            // Total Weight
                            ws.Cells[childRowIdx, 24].Formula = $"=C{childRowIdx}+F{childRowIdx}+J{childRowIdx}+M{childRowIdx}+Q{childRowIdx}+T{childRowIdx}";

                            // Total Total $
                            ws.Cells[childRowIdx, 25].Formula = $"=H{childRowIdx}+O{childRowIdx}+V{childRowIdx}";

                            // Per Ct
                            ws.Cells[childRowIdx, 26].Formula = $"=IF(X{childRowIdx}>0, Y{childRowIdx}/X{childRowIdx}, 0)";

                            r++;
                        }
                    }

                    int grandTotalRowIdx = r;
                    ws.Cells[grandTotalRowIdx, 1].Value = "Total";
                    ws.Cells[grandTotalRowIdx, 1].Style.Font.Bold = true;

                    for (int col = 2; col <= 25; col++)
                    {
                        string colLetter = GetColName(col);
                        if (parentRows.Count > 0)
                        {
                            string formulaStr = "=" + string.Join("+", parentRows.Select(pRow => $"{colLetter}{pRow}"));
                            ws.Cells[grandTotalRowIdx, col].Formula = formulaStr;
                        }
                        else
                        {
                            ws.Cells[grandTotalRowIdx, col].Value = 0;
                        }
                    }

                    ws.Cells[grandTotalRowIdx, 26].Formula = $"=IF(X{grandTotalRowIdx}>0, Y{grandTotalRowIdx}/X{grandTotalRowIdx}, 0)";

                    ws.Row(grandTotalRowIdx).Style.Font.Bold = true;
                    for (int col = 1; col <= 26; col++)
                    {
                        ws.Cells[grandTotalRowIdx, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[grandTotalRowIdx, col].Style.Fill.BackgroundColor.SetColor(colHeaderBG);
                    }

                    r++;

                    int pctRowIdx = r;
                    ws.Cells[pctRowIdx, 1].Value = "%";
                    ws.Cells[pctRowIdx, 1].Style.Font.Bold = true;

                    int[] pcsColumns = new[] { 2, 5, 9, 12, 16, 19, 23 };
                    foreach (var col in pcsColumns)
                    {
                        string colLetter = GetColName(col);
                        ws.Cells[pctRowIdx, col].Formula = $"=IF(W{grandTotalRowIdx}>0, {colLetter}{grandTotalRowIdx}/W{grandTotalRowIdx}, 0)";
                        ws.Cells[pctRowIdx, col].Style.Numberformat.Format = "0.00%";
                    }

                    ws.Row(pctRowIdx).Style.Font.Bold = true;
                    for (int col = 1; col <= 26; col++)
                    {
                        ws.Cells[pctRowIdx, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[pctRowIdx, col].Style.Fill.BackgroundColor.SetColor(colTotalRow);
                    }

                    for (int row = 5; row <= pctRowIdx; row++)
                    {
                        for (int col = 2; col <= 26; col++)
                        {
                            if (row == pctRowIdx && pcsColumns.Contains(col))
                                continue;

                            if (col == 2 || col == 5 || col == 9 || col == 12 || col == 16 || col == 19 || col == 23)
                            {
                                ws.Cells[row, col].Style.Numberformat.Format = fmtPcs;
                            }
                            else if (col == 3 || col == 6 || col == 10 || col == 13 || col == 17 || col == 20 || col == 24)
                            {
                                ws.Cells[row, col].Style.Numberformat.Format = fmtWt;
                            }
                            else if (col == 26)
                            {
                                ws.Cells[row, col].Style.Numberformat.Format = fmtRate;
                            }
                            else
                            {
                                ws.Cells[row, col].Style.Numberformat.Format = fmtAmt;
                            }
                        }
                    }

                    ws.Cells[2, 1, pctRowIdx, 26].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    SetBorders(ws, 2, 1, pctRowIdx, 26);

                    for (int col = 1; col <= 26; col++)
                    {
                        ws.Column(col).AutoFit();
                    }

                    string Folderpath = RequestHelpers.GetConfigValue("ExcelFiles");
                    if (string.IsNullOrEmpty(Folderpath)) Folderpath = "~/ExcelFiles";
                    string filename = "MEMO_HOLD_StockSummary_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    string stReturnFileName = "";

                    if (HttpContext.Current != null)
                    {
                        if (Directory.Exists(HttpContext.Current.Server.MapPath(Folderpath)) == false)
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath(Folderpath));
                        }
                        stReturnFileName = HttpContext.Current.Server.MapPath(Folderpath + "\\" + filename);
                    }
                    else
                    {
                        string localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExcelFiles");
                        if (!Directory.Exists(localPath))
                        {
                            Directory.CreateDirectory(localPath);
                        }
                        stReturnFileName = Path.Combine(localPath, filename);
                    }

                    AddStoneWiseDetailsSheet(p, "DMX_MEMOStock", date);

                    Byte[] bin = p.GetAsByteArray();
                    System.IO.File.WriteAllBytes(stReturnFileName, bin);

                    return stReturnFileName;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return "";
            }
        }

        private static string GetColName(int col)
        {
            int dividend = col;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        private static void SetBorders(ExcelWorksheet ws, int startRow, int startCol, int endRow, int endCol)
        {
            var range = ws.Cells[startRow, startCol, endRow, endCol];
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }

        private class SizeBucketItem
        {
            public decimal Min { get; set; }
            public decimal Max { get; set; }
            public string DisplayLabel { get; set; }
        }

        public static string SalesStockSummaryExport(List<SalesSummaryReportItem> data, List<SizeMasterItem> sizeList, string date)
        {
            try
            {
                if (sizeList != null && data != null)
                {
                    sizeList = sizeList.Where(size => 
                        data.Any(item => item.Weight >= size.FromSize && item.Weight <= size.ToSize)
                    ).ToList();
                }

                ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");
                string filename = "SALES_StockSummary_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                string stReturnFileName = "";

                using (ExcelPackage p = new ExcelPackage())
                {
                    p.Workbook.Properties.Author = "Diamx Stock";
                    p.Workbook.Properties.Title = "Sales Stock Summary";

                    ExcelWorksheet ws = p.Workbook.Worksheets.Add("SALES Summary");
                    ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    Color gold = System.Drawing.ColorTranslator.FromHtml("#FFC000");
                    Color lightBlue = System.Drawing.ColorTranslator.FromHtml("#DDEBF7");
                    Color headerBlue = System.Drawing.ColorTranslator.FromHtml("#449BCA");

                    // 1. Title Row
                    ws.Cells[1, 1, 1, 22].Merge = true;
                    var titleCell = ws.Cells[1, 1];
                    titleCell.Style.Font.Size = 16;
                    titleCell.Style.Font.Bold = true;
                    
                    var rt1 = titleCell.RichText.Add(date + " ");
                    rt1.Color = System.Drawing.Color.Black;
                    
                    var rt2 = titleCell.RichText.Add("(AVERAGE PER CT $ SALE (IGI))");
                    rt2.Color = System.Drawing.Color.Red;
                    
                    var rt3 = titleCell.RichText.Add(" REPORT");
                    rt3.Color = System.Drawing.Color.Black;

                    // 2. Clarity Headers
                    ws.Cells[2, 2, 2, 4].Merge = true; ws.Cells[2, 2].Value = "VVS1";
                    ws.Cells[2, 5, 2, 7].Merge = true; ws.Cells[2, 5].Value = "VVS2";
                    ws.Cells[2, 8, 2, 10].Merge = true; ws.Cells[2, 8].Value = "VS1";
                    ws.Cells[2, 11, 2, 13].Merge = true; ws.Cells[2, 11].Value = "VS2";
                    ws.Cells[2, 14, 2, 16].Merge = true; ws.Cells[2, 14].Value = "SI1";

                    ws.Cells[2, 19].Value = "VVS1";
                    ws.Cells[2, 20].Value = "VVS2";
                    ws.Cells[2, 21].Value = "VS1-VS2";
                    ws.Cells[2, 22].Value = "SI1";

                    var rightHeaders = ws.Cells[2, 19, 2, 22];
                    rightHeaders.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rightHeaders.Style.Fill.BackgroundColor.SetColor(headerBlue);
                    rightHeaders.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    rightHeaders.Style.Font.Bold = true;

                    // 3. Sub-headers
                    ws.Cells[3, 1].Value = "SIZE";
                    for (int i = 0; i < 5; i++)
                    {
                        ws.Cells[3, 2 + i * 3].Value = "Weight";
                        ws.Cells[3, 3 + i * 3].Value = "Total $";
                        ws.Cells[3, 4 + i * 3].Value = "$";
                    }

                    var headerRange = ws.Cells[2, 1, 3, 22];
                    headerRange.Style.Font.Bold = true;

                    // 4. Data Rows configuration
                    var sizeBuckets = new List<SizeBucketItem>();
                    if (sizeList != null)
                    {
                        foreach (var size in sizeList)
                        {
                            decimal min = size.FromSize;
                            decimal max = size.ToSize;

                            decimal displayMax = max;
                            if ((max % 1.0m) == 0.609m)
                            {
                                displayMax = max - 0.019m;
                            }
                            decimal truncatedMax = Math.Truncate(displayMax * 100m) / 100m;
                            string displayLabel = min.ToString("0.00") + "-" + truncatedMax.ToString("0.00");

                            sizeBuckets.Add(new SizeBucketItem
                            {
                                Min = min,
                                Max = max,
                                DisplayLabel = displayLabel
                            });
                        }
                    }
                    // Standard diamond colors in order
                    var stdColorOrder = new List<string> { "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

                    // Get distinct colors from data, ordered by standard color list first, then alphabetically
                    var uniqueColors = data
                        .Where(x => !string.IsNullOrEmpty(x.Color))
                        .Select(x => x.Color.Trim().ToUpper())
                        .Distinct()
                        .OrderBy(c => {
                            int idx = stdColorOrder.IndexOf(c);
                            return idx == -1 ? 99 : idx;
                        })
                        .ThenBy(c => c)
                        .ToArray();

                    // If no colors are found in the data, default to D, E, F, G
                    string[] colors = uniqueColors.Length > 0 ? uniqueColors : new[] { "D", "E", "F", "G" };
                    string[] clarities = { "VVS1", "VVS2", "VS1", "VS2", "SI1" };

                    int currentRow = 4;
                    foreach (var bucket in sizeBuckets)
                    {
                        int sizeGroupRowIdx = currentRow;
                        ws.Cells[sizeGroupRowIdx, 1].Value = bucket.DisplayLabel;
                        ws.Cells[sizeGroupRowIdx, 1].Style.Font.Bold = true;

                        var sizeRowRange = ws.Cells[sizeGroupRowIdx, 1, sizeGroupRowIdx, 16];
                        sizeRowRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        sizeRowRange.Style.Fill.BackgroundColor.SetColor(lightBlue);
                        sizeRowRange.Style.Font.Bold = true;

                        for (int c = 0; c < 5; c++)
                        {
                            ws.Cells[sizeGroupRowIdx, 4 + c * 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells[sizeGroupRowIdx, 4 + c * 3].Style.Fill.BackgroundColor.SetColor(gold);
                        }

                        for (int c = 0; c < 5; c++)
                        {
                            int colStart = 2 + c * 3;
                            int colTotal = 3 + c * 3;
                            int colRate = 4 + c * 3;

                            string colLetterStart = GetColName(colStart);
                            string colLetterTotal = GetColName(colTotal);

                            ws.Cells[sizeGroupRowIdx, colStart].Formula = $"SUM({colLetterStart}{sizeGroupRowIdx + 1}:{colLetterStart}{sizeGroupRowIdx + colors.Length})";
                            ws.Cells[sizeGroupRowIdx, colTotal].Formula = $"SUM({colLetterTotal}{sizeGroupRowIdx + 1}:{colLetterTotal}{sizeGroupRowIdx + colors.Length})";
                            ws.Cells[sizeGroupRowIdx, colRate].Formula = $"IF({colLetterStart}{sizeGroupRowIdx}>0,{colLetterTotal}{sizeGroupRowIdx}/{colLetterStart}{sizeGroupRowIdx},\"\")";

                            ws.Cells[sizeGroupRowIdx, colStart].Style.Numberformat.Format = "#,##0.00;(#,##0.00);\"\"";
                            ws.Cells[sizeGroupRowIdx, colTotal].Style.Numberformat.Format = "#,##0;(#,##0);\"\"";
                            ws.Cells[sizeGroupRowIdx, colRate].Style.Numberformat.Format = "0;\"\"";
                        }

                        ws.Cells[sizeGroupRowIdx, 19].Formula = $"IF(B{sizeGroupRowIdx}>0,C{sizeGroupRowIdx}/B{sizeGroupRowIdx},\"\")";
                        ws.Cells[sizeGroupRowIdx, 20].Formula = $"IF(E{sizeGroupRowIdx}>0,F{sizeGroupRowIdx}/E{sizeGroupRowIdx},\"\")";
                        ws.Cells[sizeGroupRowIdx, 21].Formula = $"IF((H{sizeGroupRowIdx}+K{sizeGroupRowIdx})>0,(I{sizeGroupRowIdx}+L{sizeGroupRowIdx})/(H{sizeGroupRowIdx}+K{sizeGroupRowIdx}),\"\")";
                        ws.Cells[sizeGroupRowIdx, 22].Formula = $"IF(N{sizeGroupRowIdx}>0,O{sizeGroupRowIdx}/N{sizeGroupRowIdx},\"\")";

                        var rightSummaryCells = ws.Cells[sizeGroupRowIdx, 19, sizeGroupRowIdx, 22];
                        rightSummaryCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        rightSummaryCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black);
                        rightSummaryCells.Style.Font.Color.SetColor(System.Drawing.Color.White);
                        rightSummaryCells.Style.Font.Bold = true;
                        rightSummaryCells.Style.Numberformat.Format = "0.00;\"\"";

                        currentRow++;

                        foreach (var color in colors)
                        {
                            ws.Cells[currentRow, 1].Value = color;

                            for (int c = 0; c < 5; c++)
                            {
                                string clarity = clarities[c];
                                var items = data.Where(x => x.Color == color && x.Clarity == clarity && x.Weight >= bucket.Min && x.Weight <= bucket.Max).ToList();

                                decimal weightSum = items.Sum(x => x.Weight);
                                decimal totalAmtSum = items.Sum(x => x.Trans_Amt);

                                int colStart = 2 + c * 3;
                                int colTotal = 3 + c * 3;
                                int colRate = 4 + c * 3;

                                if (weightSum > 0)
                                {
                                    ws.Cells[currentRow, colStart].Value = weightSum;
                                    ws.Cells[currentRow, colTotal].Value = totalAmtSum;

                                    string colLetterStart = GetColName(colStart);
                                    string colLetterTotal = GetColName(colTotal);
                                    ws.Cells[currentRow, colRate].Formula = $"IF({colLetterStart}{currentRow}>0,{colLetterTotal}{currentRow}/{colLetterStart}{currentRow},\"\")";
                                }

                                ws.Cells[currentRow, colStart].Style.Numberformat.Format = "#,##0.00;(#,##0.00);\"\"";
                                ws.Cells[currentRow, colTotal].Style.Numberformat.Format = "#,##0;(#,##0);\"\"";
                                ws.Cells[currentRow, colRate].Style.Numberformat.Format = "0;\"\"";
                            }
                            currentRow++;
                        }
                    }

                    // Apply borders
                    SetBorders(ws, 2, 1, currentRow - 1, 16);
                    SetBorders(ws, 2, 19, currentRow - 1, 22);

                    // Column Widths
                    ws.Column(1).Width = 10;
                    for (int c = 0; c < 5; c++)
                    {
                        ws.Column(2 + c * 3).Width = 10;
                        ws.Column(3 + c * 3).Width = 12;
                        ws.Column(4 + c * 3).Width = 8;
                    }
                    ws.Column(17).Width = 3;
                    ws.Column(18).Width = 3;
                    ws.Column(19).Width = 10;
                    ws.Column(20).Width = 10;
                    ws.Column(21).Width = 12;
                    ws.Column(22).Width = 10;

                    // Output file
                    string Folderpath = ConfigurationManager.AppSettings["ExcelFiles"];
                    if (string.IsNullOrEmpty(Folderpath))
                    {
                        Folderpath = "\\ExcelFiles";
                    }

                    if (HttpContext.Current != null)
                    {
                        if (!Directory.Exists(HttpContext.Current.Server.MapPath(Folderpath)))
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath(Folderpath));
                        }
                        stReturnFileName = HttpContext.Current.Server.MapPath(Folderpath + "\\" + filename);
                    }
                    else
                    {
                        string localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExcelFiles");
                        if (!Directory.Exists(localPath))
                        {
                            Directory.CreateDirectory(localPath);
                        }
                        stReturnFileName = Path.Combine(localPath, filename);
                    }

                    AddStoneWiseDetailsSheet(p, "DMX_SALESStock", date);

                    File.WriteAllBytes(stReturnFileName, p.GetAsByteArray());
                }

                return stReturnFileName;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return "";
            }
        }

        public static string SalesStockSellerWiseExport(List<SalesSellerWiseReportItem> data, string date)
        {
            try
            {
                ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");
                string filename = "SALES_SellerWiseSummary_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                string stReturnFileName = "";

                using (ExcelPackage p = new ExcelPackage())
                {
                    p.Workbook.Properties.Author = "Diamx Stock";
                    p.Workbook.Properties.Title = "Sales Stock Seller Wise Report";

                    ExcelWorksheet ws = p.Workbook.Worksheets.Add("Seller Wise Sale");
                    ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.View.ShowGridLines = true;

                    Color colHeaderBg = ColorTranslator.FromHtml("#F2F2F2");
                    Color colLightBlue = ColorTranslator.FromHtml("#DDEBF7");
                    Color colPink = ColorTranslator.FromHtml("#F2DCDB");
                    Color colLightGreen = ColorTranslator.FromHtml("#E2EFDA");
                    Color colPurple = ColorTranslator.FromHtml("#E4DFEC");
                    Color colGoldTotal = ColorTranslator.FromHtml("#FFD966");
                    Color colOrangePeach = ColorTranslator.FromHtml("#FCE4D6");

                    // Location Branch BG mapping
                    Color colMumBranch = ColorTranslator.FromHtml("#E2EFDA"); // light green for Mumbai
                    Color colNYBranch = ColorTranslator.FromHtml("#DDEBF7");  // light blue for NY
                    Color colSurBranch = ColorTranslator.FromHtml("#FCE4D6"); // light peach/orange for Surat

                    string monthYear = date;
                    try
                    {
                        if (DateTime.TryParse(date, out DateTime dt))
                        {
                            monthYear = dt.ToString("MMM-yy").ToUpper();
                        }
                    }
                    catch { }

                    // 1. Title Row
                    ws.Cells[1, 1, 1, 28].Merge = true;
                    var titleCell = ws.Cells[1, 1];
                    titleCell.Value = $"{monthYear} IGI CERTIFIED SALE REPORT";
                    titleCell.Style.Font.Size = 18;
                    titleCell.Style.Font.Bold = true;
                    titleCell.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#C00000"));
                    titleCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    titleCell.Style.Fill.BackgroundColor.SetColor(Color.White);
                    ws.Row(1).Height = 40;

                    // 2. Main Headers (Row 2, 3, 4)
                    
                    // SELLER (Col A)
                    ws.Cells[2, 1, 4, 1].Merge = true;
                    ws.Cells[2, 1].Value = "SELLER";
                    ws.Cells[2, 1].Style.Font.Bold = true;
                    ws.Cells[2, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[2, 1].Style.Fill.BackgroundColor.SetColor(colHeaderBg);

                    // BRANCH (Col B)
                    ws.Cells[2, 2, 4, 2].Merge = true;
                    ws.Cells[2, 2].Value = "BRANCH";
                    ws.Cells[2, 2].Style.Font.Bold = true;
                    ws.Cells[2, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[2, 2].Style.Fill.BackgroundColor.SetColor(colHeaderBg);

                    // MUMBAI (Col C-H)
                    ws.Cells[2, 3, 2, 8].Merge = true;
                    ws.Cells[2, 3].Value = "MUMBAI";
                    ws.Cells[2, 3].Style.Font.Bold = true;
                    ws.Cells[2, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[2, 3].Style.Fill.BackgroundColor.SetColor(colLightBlue);

                    ws.Cells[3, 3, 3, 5].Merge = true;
                    ws.Cells[3, 3].Value = "Sold After Purchase";
                    ws.Cells[3, 3].Style.Font.Bold = true;
                    ws.Cells[3, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[3, 3].Style.Fill.BackgroundColor.SetColor(colPink);

                    ws.Cells[3, 6, 3, 8].Merge = true;
                    ws.Cells[3, 6].Value = "SALE";
                    ws.Cells[3, 6].Style.Font.Bold = true;
                    ws.Cells[3, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[3, 6].Style.Fill.BackgroundColor.SetColor(colLightGreen);

                    // NEWYORK (Col I-N)
                    ws.Cells[2, 9, 2, 14].Merge = true;
                    ws.Cells[2, 9].Value = "NEWYORK";
                    ws.Cells[2, 9].Style.Font.Bold = true;
                    ws.Cells[2, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[2, 9].Style.Fill.BackgroundColor.SetColor(colLightBlue);

                    ws.Cells[3, 9, 3, 11].Merge = true;
                    ws.Cells[3, 9].Value = "Sold After Purchase";
                    ws.Cells[3, 9].Style.Font.Bold = true;
                    ws.Cells[3, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[3, 9].Style.Fill.BackgroundColor.SetColor(colPink);

                    ws.Cells[3, 12, 3, 14].Merge = true;
                    ws.Cells[3, 12].Value = "SALE";
                    ws.Cells[3, 12].Style.Font.Bold = true;
                    ws.Cells[3, 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[3, 12].Style.Fill.BackgroundColor.SetColor(colLightGreen);

                    // SURAT (Col O-T)
                    ws.Cells[2, 15, 2, 20].Merge = true;
                    ws.Cells[2, 15].Value = "SURAT";
                    ws.Cells[2, 15].Style.Font.Bold = true;
                    ws.Cells[2, 15].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[2, 15].Style.Fill.BackgroundColor.SetColor(colLightBlue);

                    ws.Cells[3, 15, 3, 17].Merge = true;
                    ws.Cells[3, 15].Value = "Sold After Purchase";
                    ws.Cells[3, 15].Style.Font.Bold = true;
                    ws.Cells[3, 15].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[3, 15].Style.Fill.BackgroundColor.SetColor(colPink);

                    ws.Cells[3, 18, 3, 20].Merge = true;
                    ws.Cells[3, 18].Value = "SALE";
                    ws.Cells[3, 18].Style.Font.Bold = true;
                    ws.Cells[3, 18].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[3, 18].Style.Fill.BackgroundColor.SetColor(colLightGreen);

                    // INC TRANSFER (Col U-W)
                    ws.Cells[3, 21, 3, 23].Merge = true;
                    ws.Cells[3, 21].Value = "INC TRANSFER";
                    ws.Cells[3, 21].Style.Font.Bold = true;
                    ws.Cells[3, 21].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[3, 21].Style.Fill.BackgroundColor.SetColor(colPurple);

                    // Row 4: Sub-headers Pcs, Weight, Total $
                    string[] subHd = { "Pcs", "Weight", "Total $" };
                    for (int g = 0; g < 7; g++) // MUMBAI(2), NEWYORK(2), SURAT(2), INC TRANSFER(1) = 7 sub groups
                    {
                        Color bg = colPink;
                        if (g == 1 || g == 3 || g == 5) bg = colLightGreen;
                        if (g == 6) bg = colPurple;

                        for (int c = 0; c < 3; c++)
                        {
                            int colIdx = 3 + g * 3 + c;
                            ws.Cells[4, colIdx].Value = subHd[c];
                            ws.Cells[4, colIdx].Style.Font.Bold = true;
                            ws.Cells[4, colIdx].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells[4, colIdx].Style.Fill.BackgroundColor.SetColor(bg);
                        }
                    }

                    // Total Pcs, Total Weight, Total Total $, Per Ct, %
                    ws.Cells[2, 24, 4, 24].Merge = true;
                    ws.Cells[2, 24].Value = "Total Pcs";
                    ws.Cells[2, 24].Style.Font.Bold = true;
                    ws.Cells[2, 24].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[2, 24].Style.Fill.BackgroundColor.SetColor(colHeaderBg);

                    ws.Cells[2, 25, 4, 25].Merge = true;
                    ws.Cells[2, 25].Value = "Total Weight";
                    ws.Cells[2, 25].Style.Font.Bold = true;
                    ws.Cells[2, 25].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[2, 25].Style.Fill.BackgroundColor.SetColor(colHeaderBg);

                    ws.Cells[2, 26, 4, 26].Merge = true;
                    ws.Cells[2, 26].Value = "Total Total $";
                    ws.Cells[2, 26].Style.Font.Bold = true;
                    ws.Cells[2, 26].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[2, 26].Style.Fill.BackgroundColor.SetColor(colHeaderBg);

                    ws.Cells[2, 27, 4, 27].Merge = true;
                    ws.Cells[2, 27].Value = "Per Ct";
                    ws.Cells[2, 27].Style.Font.Bold = true;
                    ws.Cells[2, 27].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[2, 27].Style.Fill.BackgroundColor.SetColor(colHeaderBg);

                    ws.Cells[2, 28, 4, 28].Merge = true;
                    ws.Cells[2, 28].Value = "%";
                    ws.Cells[2, 28].Style.Font.Bold = true;
                    ws.Cells[2, 28].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[2, 28].Style.Fill.BackgroundColor.SetColor(colHeaderBg);

                    ws.Row(2).Height = 22;
                    ws.Row(3).Height = 22;
                    ws.Row(4).Height = 22;

                    // 3. Populate Data Rows
                    int currentRow = 5;
                    int totalRow = 5;
                    if (data != null && data.Count > 0)
                    {
                        totalRow = 5 + data.Count;
                        foreach (var item in data)
                        {
                            ws.Row(currentRow).Height = 20;

                            // Seller (Col A)
                            ws.Cells[currentRow, 1].Value = item.Seller_Name;
                            ws.Cells[currentRow, 1].Style.Font.Bold = true;
                            ws.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            // Branch (Col B)
                            ws.Cells[currentRow, 2].Value = item.Branch;
                            ws.Cells[currentRow, 2].Style.Font.Bold = true;
                            
                            // Highlight Branch based on its value
                            string br = item.Branch != null ? item.Branch.Trim().ToUpper() : "";
                            Color branchColor = colHeaderBg;
                            if (br == "MUMBAI") branchColor = colMumBranch;
                            else if (br == "NY" || br == "NEWYORK" || br == "NEW YORK") branchColor = colNYBranch;
                            else if (br == "SURAT") branchColor = colSurBranch;

                            ws.Cells[currentRow, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells[currentRow, 2].Style.Fill.BackgroundColor.SetColor(branchColor);

                            // MUMBAI SAP (Col C-E)
                            ws.Cells[currentRow, 3].Value = item.Mum_SAP_Pcs;
                            ws.Cells[currentRow, 4].Value = item.Mum_SAP_Weight;
                            ws.Cells[currentRow, 5].Value = item.Mum_SAP_Amt;

                            // MUMBAI SALE (Col F-H)
                            ws.Cells[currentRow, 6].Value = item.Mum_Sale_Pcs;
                            ws.Cells[currentRow, 7].Value = item.Mum_Sale_Weight;
                            ws.Cells[currentRow, 8].Value = item.Mum_Sale_Amt;

                            // NEWYORK SAP (Col I-K)
                            ws.Cells[currentRow, 9].Value = item.NY_SAP_Pcs;
                            ws.Cells[currentRow, 10].Value = item.NY_SAP_Weight;
                            ws.Cells[currentRow, 11].Value = item.NY_SAP_Amt;

                            // NEWYORK SALE (Col L-N)
                            ws.Cells[currentRow, 12].Value = item.NY_Sale_Pcs;
                            ws.Cells[currentRow, 13].Value = item.NY_Sale_Weight;
                            ws.Cells[currentRow, 14].Value = item.NY_Sale_Amt;

                            // SURAT SAP (Col O-Q)
                            ws.Cells[currentRow, 15].Value = item.Sur_SAP_Pcs;
                            ws.Cells[currentRow, 16].Value = item.Sur_SAP_Weight;
                            ws.Cells[currentRow, 17].Value = item.Sur_SAP_Amt;

                            // SURAT SALE (Col R-T)
                            ws.Cells[currentRow, 18].Value = item.Sur_Sale_Pcs;
                            ws.Cells[currentRow, 19].Value = item.Sur_Sale_Weight;
                            ws.Cells[currentRow, 20].Value = item.Sur_Sale_Amt;

                            // INC TRANSFER (Col U-W)
                            ws.Cells[currentRow, 21].Value = item.Inc_Transfer_Pcs;
                            ws.Cells[currentRow, 22].Value = item.Inc_Transfer_Weight;
                            ws.Cells[currentRow, 23].Value = item.Inc_Transfer_Amt;

                            // Row Formulas (Col X-AB)
                            // Total Pcs (Col X) = C+F+I+L+O+R+U
                            ws.Cells[currentRow, 24].Formula = $"C{currentRow}+F{currentRow}+I{currentRow}+L{currentRow}+O{currentRow}+R{currentRow}+U{currentRow}";
                            // Total Weight (Col Y) = D+G+J+M+P+S+V
                            ws.Cells[currentRow, 25].Formula = $"D{currentRow}+G{currentRow}+J{currentRow}+M{currentRow}+P{currentRow}+S{currentRow}+V{currentRow}";
                            // Total Total $ (Col Z) = E+H+K+N+Q+T+W
                            ws.Cells[currentRow, 26].Formula = $"E{currentRow}+H{currentRow}+K{currentRow}+N{currentRow}+Q{currentRow}+T{currentRow}+W{currentRow}";
                            // Per Ct (Col AA) = Z / Y
                            ws.Cells[currentRow, 27].Formula = $"IF(Y{currentRow}>0,Z{currentRow}/Y{currentRow},0)";
                            // % (Col AB) = Z / Z$totalRow * 100
                            ws.Cells[currentRow, 28].Formula = $"IF(Z${totalRow}>0,(Z{currentRow}/Z${totalRow})*100,0)";

                            currentRow++;
                        }
                    }

                    // 4. Grand Total Row
                    ws.Row(totalRow).Height = 22;
                    ws.Cells[totalRow, 1, totalRow, 2].Merge = true;
                    ws.Cells[totalRow, 1].Value = "TOTAL";
                    ws.Cells[totalRow, 1].Style.Font.Bold = true;

                    // Set bold and gold/yellow bg for Total row
                    for (int c = 1; c <= 28; c++)
                    {
                        ws.Cells[totalRow, c].Style.Font.Bold = true;
                        ws.Cells[totalRow, c].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[totalRow, c].Style.Fill.BackgroundColor.SetColor(colGoldTotal);
                    }

                    // Column totals formulas (C to Z)
                    string[] colLetters = {
                        "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
                    };
                    for (int c = 2; c < 26; c++) // Cols C to Z (indexes 3 to 26)
                    {
                        string let = colLetters[c];
                        ws.Cells[totalRow, c + 1].Formula = $"SUM({let}5:{let}{totalRow - 1})";
                    }
                    // Per Ct (Col AA) = Z / Y
                    ws.Cells[totalRow, 27].Formula = $"IF(Y{totalRow}>0,Z{totalRow}/Y{totalRow},0)";
                    // % (Col AB) = SUM(AB5:AB{totalRow-1})
                    ws.Cells[totalRow, 28].Formula = $"SUM(AB5:AB{totalRow - 1})";

                    // Apply number formatting to main table (rows 5 to totalRow)
                    for (int r = 5; r <= totalRow; r++)
                    {
                        // Pcs cols
                        int[] pcsCols = { 3, 6, 9, 12, 15, 18, 21, 24 };
                        foreach (int col in pcsCols)
                            ws.Cells[r, col].Style.Numberformat.Format = "#,##0;(#,##0);\"\"";

                        // Weight cols
                        int[] wtCols = { 4, 7, 10, 13, 16, 19, 22, 25 };
                        foreach (int col in wtCols)
                            ws.Cells[r, col].Style.Numberformat.Format = "#,##0.00;(#,##0.00);\"\"";

                        // Amt cols
                        int[] amtCols = { 5, 8, 11, 14, 17, 20, 23, 26 };
                        foreach (int col in amtCols)
                            ws.Cells[r, col].Style.Numberformat.Format = "#,##0;(#,##0);\"\"";

                        // Per Ct & %
                        ws.Cells[r, 27].Style.Numberformat.Format = "0.00;\"\"";
                        ws.Cells[r, 28].Style.Numberformat.Format = "0.00;\"\"";
                    }

                    // Set borders for main table
                    SetBorders(ws, 2, 1, totalRow, 28);

                    // 5. SUMMARY TABLE (at bottom)
                    int rSummary = totalRow + 3;

                    // Summary Header Row 1
                    ws.Row(rSummary).Height = 25;
                    
                    // Grand Total header (Cols A-E)
                    ws.Cells[rSummary, 1, rSummary, 5].Merge = true;
                    ws.Cells[rSummary, 1].Value = $"{monthYear} SALE SUMMARY REPORT";
                    ws.Cells[rSummary, 1].Style.Font.Bold = true;
                    ws.Cells[rSummary, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[rSummary, 1].Style.Fill.BackgroundColor.SetColor(colPink);

                    // MUMBAI header (Cols G-J)
                    ws.Cells[rSummary, 7, rSummary, 10].Merge = true;
                    ws.Cells[rSummary, 7].Value = "MUMBAI";
                    ws.Cells[rSummary, 7].Style.Font.Bold = true;
                    ws.Cells[rSummary, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[rSummary, 7].Style.Fill.BackgroundColor.SetColor(colLightGreen);

                    // NEW YORK header (Cols L-O)
                    ws.Cells[rSummary, 12, rSummary, 15].Merge = true;
                    ws.Cells[rSummary, 12].Value = "NEW YORK";
                    ws.Cells[rSummary, 12].Style.Font.Bold = true;
                    ws.Cells[rSummary, 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[rSummary, 12].Style.Fill.BackgroundColor.SetColor(colLightBlue);

                    // SURAT header (Cols Q-T)
                    ws.Cells[rSummary, 17, rSummary, 20].Merge = true;
                    ws.Cells[rSummary, 17].Value = "SURAT";
                    ws.Cells[rSummary, 17].Style.Font.Bold = true;
                    ws.Cells[rSummary, 17].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[rSummary, 17].Style.Fill.BackgroundColor.SetColor(colOrangePeach);

                    // Summary Header Row 2 (Pcs, Weight, Total $, Per Ct)
                    int rSubHd = rSummary + 1;
                    ws.Row(rSubHd).Height = 20;

                    // Grand total sub-headers
                    string[] summarySubHeaders = { "STATUS", "PCS", "WEIGHT", "TOTAL $", "PER CT" };
                    for (int c = 0; c < 5; c++)
                    {
                        var cell = ws.Cells[rSubHd, c + 1];
                        cell.Value = summarySubHeaders[c];
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(colHeaderBg);
                    }

                    // Mumbai, NY, Surat sub-headers
                    string[] locSubHeaders = { "PCS", "WEIGHT", "TOTAL $", "PER CT" };
                    int[] startCols = { 7, 12, 17 };
                    foreach (int sc in startCols)
                    {
                        for (int c = 0; c < 4; c++)
                        {
                            var cell = ws.Cells[rSubHd, sc + c];
                            cell.Value = locSubHeaders[c];
                            cell.Style.Font.Bold = true;
                            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cell.Style.Fill.BackgroundColor.SetColor(colHeaderBg);
                        }
                    }

                    // Rows for SALE, Sold After Purchase, ANJALI INC TRANSFER, TOTAL
                    int rSale = rSummary + 2;
                    int rSAP = rSummary + 3;
                    int rAIT = rSummary + 4;
                    int rTot = rSummary + 5;

                    ws.Row(rSale).Height = 20;
                    ws.Row(rSAP).Height = 20;
                    ws.Row(rAIT).Height = 20;
                    ws.Row(rTot).Height = 22;

                    // STATUS column values
                    ws.Cells[rSale, 1].Value = "SALE";
                    ws.Cells[rSale, 1].Style.Font.Bold = true;
                    ws.Cells[rSale, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[rSale, 1].Style.Fill.BackgroundColor.SetColor(colLightGreen);

                    ws.Cells[rSAP, 1].Value = "Sold After Purchase";
                    ws.Cells[rSAP, 1].Style.Font.Bold = true;
                    ws.Cells[rSAP, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[rSAP, 1].Style.Fill.BackgroundColor.SetColor(colPink);

                    ws.Cells[rAIT, 1].Value = "ANJALI INC TRANSFER";
                    ws.Cells[rAIT, 1].Style.Font.Bold = true;
                    ws.Cells[rAIT, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[rAIT, 1].Style.Fill.BackgroundColor.SetColor(colPurple);

                    ws.Cells[rTot, 1].Value = "TOTAL";

                    // MUMBAI data formulas (Cols G-J)
                    // SALE row
                    ws.Cells[rSale, 7].Formula = $"F{totalRow}"; // Mumbai SALE Pcs
                    ws.Cells[rSale, 8].Formula = $"G{totalRow}"; // Mumbai SALE Weight
                    ws.Cells[rSale, 9].Formula = $"H{totalRow}"; // Mumbai SALE Total $
                    ws.Cells[rSale, 10].Formula = $"IF(H{rSale}>0,I{rSale}/H{rSale},0)"; // Mumbai SALE Per Ct
                    // SAP row
                    ws.Cells[rSAP, 7].Formula = $"C{totalRow}"; // Mumbai SAP Pcs
                    ws.Cells[rSAP, 8].Formula = $"D{totalRow}"; // Mumbai SAP Weight
                    ws.Cells[rSAP, 9].Formula = $"E{totalRow}"; // Mumbai SAP Total $
                    ws.Cells[rSAP, 10].Formula = $"IF(H{rSAP}>0,I{rSAP}/H{rSAP},0)"; // Mumbai SAP Per Ct
                    // INC TRANSFER row (Mumbai is 0/empty)
                    ws.Cells[rAIT, 7].Value = 0;
                    ws.Cells[rAIT, 8].Value = 0;
                    ws.Cells[rAIT, 9].Value = 0;
                    ws.Cells[rAIT, 10].Value = 0;
                    // TOTAL row
                    ws.Cells[rTot, 7].Formula = $"SUM(G{rSale}:G{rAIT})";
                    ws.Cells[rTot, 8].Formula = $"SUM(H{rSale}:H{rAIT})";
                    ws.Cells[rTot, 9].Formula = $"SUM(I{rSale}:I{rAIT})";
                    ws.Cells[rTot, 10].Formula = $"IF(H{rTot}>0,I{rTot}/H{rTot},0)";

                    // NEW YORK data formulas (Cols L-O)
                    // SALE row
                    ws.Cells[rSale, 12].Formula = $"L{totalRow}"; // NY SALE Pcs
                    ws.Cells[rSale, 13].Formula = $"M{totalRow}"; // NY SALE Weight
                    ws.Cells[rSale, 14].Formula = $"N{totalRow}"; // NY SALE Total $
                    ws.Cells[rSale, 15].Formula = $"IF(M{rSale}>0,N{rSale}/M{rSale},0)";
                    // SAP row
                    ws.Cells[rSAP, 12].Formula = $"I{totalRow}"; // NY SAP Pcs
                    ws.Cells[rSAP, 13].Formula = $"J{totalRow}"; // NY SAP Weight
                    ws.Cells[rSAP, 14].Formula = $"K{totalRow}"; // NY SAP Total $
                    ws.Cells[rSAP, 15].Formula = $"IF(M{rSAP}>0,N{rSAP}/M{rSAP},0)";
                    // INC TRANSFER row (NY is loaded with Inc_Transfer_Totals)
                    ws.Cells[rAIT, 12].Formula = $"U{totalRow}"; // NY INC TRANSFER Pcs
                    ws.Cells[rAIT, 13].Formula = $"V{totalRow}"; // NY INC TRANSFER Weight
                    ws.Cells[rAIT, 14].Formula = $"W{totalRow}"; // NY INC TRANSFER Total $
                    ws.Cells[rAIT, 15].Formula = $"IF(M{rAIT}>0,N{rAIT}/M{rAIT},0)";
                    // TOTAL row
                    ws.Cells[rTot, 12].Formula = $"SUM(L{rSale}:L{rAIT})";
                    ws.Cells[rTot, 13].Formula = $"SUM(M{rSale}:M{rAIT})";
                    ws.Cells[rTot, 14].Formula = $"SUM(N{rSale}:N{rAIT})";
                    ws.Cells[rTot, 15].Formula = $"IF(M{rTot}>0,N{rTot}/M{rTot},0)";

                    // SURAT data formulas (Cols Q-T)
                    // SALE row
                    ws.Cells[rSale, 17].Formula = $"R{totalRow}"; // Surat SALE Pcs
                    ws.Cells[rSale, 18].Formula = $"S{totalRow}"; // Surat SALE Weight
                    ws.Cells[rSale, 19].Formula = $"T{totalRow}"; // Surat SALE Total $
                    ws.Cells[rSale, 20].Formula = $"IF(R{rSale}>0,S{rSale}/R{rSale},0)";
                    // SAP row
                    ws.Cells[rSAP, 17].Formula = $"O{totalRow}"; // Surat SAP Pcs
                    ws.Cells[rSAP, 18].Formula = $"P{totalRow}"; // Surat SAP Weight
                    ws.Cells[rSAP, 19].Formula = $"Q{totalRow}"; // Surat SAP Total $
                    ws.Cells[rSAP, 20].Formula = $"IF(R{rSAP}>0,S{rSAP}/R{rSAP},0)";
                    // INC TRANSFER row (Surat is 0/empty)
                    ws.Cells[rAIT, 17].Value = 0;
                    ws.Cells[rAIT, 18].Value = 0;
                    ws.Cells[rAIT, 19].Value = 0;
                    ws.Cells[rAIT, 20].Value = 0;
                    // TOTAL row
                    ws.Cells[rTot, 17].Formula = $"SUM(Q{rSale}:Q{rAIT})";
                    ws.Cells[rTot, 18].Formula = $"SUM(R{rSale}:R{rAIT})";
                    ws.Cells[rTot, 19].Formula = $"SUM(S{rSale}:S{rAIT})";
                    ws.Cells[rTot, 20].Formula = $"IF(R{rTot}>0,S{rTot}/R{rTot},0)";

                    // GRAND TOTAL columns data (Cols B-E)
                    // SALE row
                    ws.Cells[rSale, 2].Formula = $"G{rSale}+L{rSale}+Q{rSale}";
                    ws.Cells[rSale, 3].Formula = $"H{rSale}+M{rSale}+R{rSale}";
                    ws.Cells[rSale, 4].Formula = $"I{rSale}+N{rSale}+S{rSale}";
                    ws.Cells[rSale, 5].Formula = $"IF(C{rSale}>0,D{rSale}/C{rSale},0)";
                    // SAP row
                    ws.Cells[rSAP, 2].Formula = $"G{rSAP}+L{rSAP}+Q{rSAP}";
                    ws.Cells[rSAP, 3].Formula = $"H{rSAP}+M{rSAP}+R{rSAP}";
                    ws.Cells[rSAP, 4].Formula = $"I{rSAP}+N{rSAP}+S{rSAP}";
                    ws.Cells[rSAP, 5].Formula = $"IF(C{rSAP}>0,D{rSAP}/C{rSAP},0)";
                    // INC TRANSFER row
                    ws.Cells[rAIT, 2].Formula = $"G{rAIT}+L{rAIT}+Q{rAIT}";
                    ws.Cells[rAIT, 3].Formula = $"H{rAIT}+M{rAIT}+R{rAIT}";
                    ws.Cells[rAIT, 4].Formula = $"I{rAIT}+N{rAIT}+S{rAIT}";
                    ws.Cells[rAIT, 5].Formula = $"IF(C{rAIT}>0,D{rAIT}/C{rAIT},0)";
                    // TOTAL row
                    ws.Cells[rTot, 2].Formula = $"SUM(B{rSale}:B{rAIT})";
                    ws.Cells[rTot, 3].Formula = $"SUM(C{rSale}:C{rAIT})";
                    ws.Cells[rTot, 4].Formula = $"SUM(D{rSale}:D{rAIT})";
                    ws.Cells[rTot, 5].Formula = $"IF(C{rTot}>0,D{rTot}/C{rTot},0)";

                    // Bold + bg for Summary TOTAL row
                    for (int c = 1; c <= 20; c++)
                    {
                        if (c == 6 || c == 11 || c == 16) continue; // skip spacers
                        ws.Cells[rTot, c].Style.Font.Bold = true;
                        ws.Cells[rTot, c].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[rTot, c].Style.Fill.BackgroundColor.SetColor(colHeaderBg);
                    }

                    // Apply number formats to Summary Table cells
                    for (int r = rSale; r <= rTot; r++)
                    {
                        // PCS
                        ws.Cells[r, 2].Style.Numberformat.Format = "#,##0;(#,##0);\"\"";
                        ws.Cells[r, 7].Style.Numberformat.Format = "#,##0;(#,##0);\"\"";
                        ws.Cells[r, 12].Style.Numberformat.Format = "#,##0;(#,##0);\"\"";
                        ws.Cells[r, 17].Style.Numberformat.Format = "#,##0;(#,##0);\"\"";

                        // WEIGHT
                        ws.Cells[r, 3].Style.Numberformat.Format = "#,##0.00;(#,##0.00);\"\"";
                        ws.Cells[r, 8].Style.Numberformat.Format = "#,##0.00;(#,##0.00);\"\"";
                        ws.Cells[r, 13].Style.Numberformat.Format = "#,##0.00;(#,##0.00);\"\"";
                        ws.Cells[r, 18].Style.Numberformat.Format = "#,##0.00;(#,##0.00);\"\"";

                        // TOTAL $
                        ws.Cells[r, 4].Style.Numberformat.Format = "#,##0;(#,##0);\"\"";
                        ws.Cells[r, 9].Style.Numberformat.Format = "#,##0;(#,##0);\"\"";
                        ws.Cells[r, 14].Style.Numberformat.Format = "#,##0;(#,##0);\"\"";
                        ws.Cells[r, 19].Style.Numberformat.Format = "#,##0;(#,##0);\"\"";

                        // PER CT
                        ws.Cells[r, 5].Style.Numberformat.Format = "0.00;\"\"";
                        ws.Cells[r, 10].Style.Numberformat.Format = "0.00;\"\"";
                        ws.Cells[r, 15].Style.Numberformat.Format = "0.00;\"\"";
                        ws.Cells[r, 20].Style.Numberformat.Format = "0.00;\"\"";
                    }

                    // Set borders for summary table parts
                    SetBorders(ws, rSummary, 1, rTot, 5);  // Grand total section
                    SetBorders(ws, rSummary, 7, rTot, 10); // Mumbai section
                    SetBorders(ws, rSummary, 12, rTot, 15); // NY section
                    SetBorders(ws, rSummary, 17, rTot, 20); // Surat section

                    // Set Column widths
                    ws.Column(1).Width = 25; // SELLER name / STATUS
                    ws.Column(2).Width = 15; // BRANCH / PCS
                    
                    // C to W detailed columns widths
                    for (int c = 3; c <= 23; c++)
                        ws.Column(c).Width = 11;

                    // Spacer column widths (F, K, P)
                    ws.Column(6).Width = 4;
                    ws.Column(11).Width = 4;
                    ws.Column(16).Width = 4;

                    // Totals column widths (X, Y, Z, AA, AB)
                    ws.Column(24).Width = 12; // Total Pcs
                    ws.Column(25).Width = 15; // Total Weight
                    ws.Column(26).Width = 15; // Total Total $
                    ws.Column(27).Width = 12; // Per Ct
                    ws.Column(28).Width = 10; // %

                    // Output file location
                    string Folderpath = ConfigurationManager.AppSettings["ExcelFiles"];
                    if (string.IsNullOrEmpty(Folderpath))
                    {
                        Folderpath = "\\ExcelFiles";
                    }

                    if (HttpContext.Current != null)
                    {
                        if (!Directory.Exists(HttpContext.Current.Server.MapPath(Folderpath)))
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath(Folderpath));
                        }
                        stReturnFileName = HttpContext.Current.Server.MapPath(Folderpath + "\\" + filename);
                    }
                    else
                    {
                        string localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExcelFiles");
                        if (!Directory.Exists(localPath))
                        {
                            Directory.CreateDirectory(localPath);
                        }
                        stReturnFileName = Path.Combine(localPath, filename);
                    }

                    AddStoneWiseDetailsSheet(p, "DMX_SALESStock", date);

                    File.WriteAllBytes(stReturnFileName, p.GetAsByteArray());
                }

                return stReturnFileName;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return "";
            }
        }

        private class StockSummaryReportParsedStone
        {
            public decimal Weight { get; set; }
            public string Color { get; set; }
            public string Clarity { get; set; }
            public bool IsWhite { get; set; }
            public bool IsEX { get; set; }
            public decimal SaleAmt { get; set; }
            public string Location { get; set; }
        }

        private static void GenerateStockSummaryReportSheet(ExcelPackage p, List<DMX_AvailableStock> rawStock, List<SizeMasterItem> sizeList, string date)
        {
            if (rawStock == null || sizeList == null || rawStock.Count == 0 || sizeList.Count == 0)
                return;

            ExcelWorksheet ws = p.Workbook.Worksheets.Add("Stock Summary Report");
            ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.View.ShowGridLines = true;

            Color colBlack = System.Drawing.ColorTranslator.FromHtml("#000000");
            Color colWhite = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
            
            Color colMumbaiBg = System.Drawing.ColorTranslator.FromHtml("#F4B084"); // Orange/Gold
            Color colNyBg = System.Drawing.ColorTranslator.FromHtml("#5B9BD5"); // Blue/Cyan
            
            Color colGreenEX = System.Drawing.ColorTranslator.FromHtml("#70AD47"); // Green
            Color colRedVG = System.Drawing.ColorTranslator.FromHtml("#ED7D31"); // Red/Orange
            
            Color colTotalPurple = System.Drawing.ColorTranslator.FromHtml("#E4DFEC"); // light purple
            Color colTotalGreen = System.Drawing.ColorTranslator.FromHtml("#E2EFDA"); // light green
            Color colGold = System.Drawing.ColorTranslator.FromHtml("#FFC000"); // Gold

            var whiteColors = new HashSet<string> { "D", "E", "F", "G", "H", "I", "J", "K", "L", "M" };

            var igiStones = rawStock;

            var parsed = igiStones.Select(s => {
                decimal w = 0m;
                decimal.TryParse(s.Weight, out w);
                
                decimal amt = 0m;
                decimal.TryParse(s.SaleAmt, out amt);

                string shape = s.Shape != null ? s.Shape.Trim().ToUpper() : "";
                string color = s.Color != null ? s.Color.Trim().ToUpper() : "";
                string clarity = s.Clarity != null ? s.Clarity.Trim().ToUpper() : "";
                string cut = s.Cut != null ? s.Cut.Trim().ToUpper() : "";
                string polish = s.Polish != null ? s.Polish.Trim().ToUpper() : "";
                string symm = s.Symm != null ? s.Symm.Trim().ToUpper() : "";
                string loc = s.Location != null ? s.Location.Trim().ToUpper() : "";

                bool isWhite = whiteColors.Contains(color);
                bool isEX = false;
                if (shape == "ROUND")
                {
                    isEX = (cut == "ID" || cut == "EX") && polish == "EX" && symm == "EX";
                }
                else
                {
                    isEX = polish == "EX" && symm == "EX";
                }

                return new StockSummaryReportParsedStone {
                    Weight = w,
                    Color = color,
                    Clarity = clarity,
                    IsWhite = isWhite,
                    IsEX = isEX,
                    SaleAmt = amt,
                    Location = loc
                };
            }).ToList();

            // Filter sizeList to only include sizes that actually exist in parsed data
            sizeList = sizeList.Where(sizeItem => 
                parsed.Any(s => s.Weight >= sizeItem.FromSize && s.Weight <= sizeItem.ToSize)
            ).ToList();

            if (sizeList.Count == 0)
                return;

            // Row 1: Title
            ws.Cells[1, 1, 1, 23].Merge = true;
            var titleCell = ws.Cells[1, 1];
            titleCell.Value = $"{date} STOCK SUMMARY REPORT (IGI)";
            titleCell.Style.Font.Size = 16;
            titleCell.Style.Font.Bold = true;
            ws.Row(1).Height = 35;

            // Row 2: Branch headers
            ws.Cells[2, 2, 2, 7].Merge = true;
            ws.Cells[2, 2].Value = "MUMBAI";
            ws.Cells[2, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[2, 2].Style.Fill.BackgroundColor.SetColor(colMumbaiBg);
            ws.Cells[2, 2].Style.Font.Bold = true;

            ws.Cells[2, 8, 2, 13].Merge = true;
            ws.Cells[2, 8].Value = "NY";
            ws.Cells[2, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[2, 8].Style.Fill.BackgroundColor.SetColor(colNyBg);
            ws.Cells[2, 8].Style.Font.Bold = true;

            ws.Cells[2, 14, 2, 19].Merge = true;
            ws.Cells[2, 14].Value = "SURAT";
            ws.Cells[2, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[2, 14].Style.Fill.BackgroundColor.SetColor(colNyBg);
            ws.Cells[2, 14].Style.Font.Bold = true;

            // Summary headers
            ws.Cells[2, 20, 5, 20].Merge = true;
            ws.Cells[2, 20].Value = "Total Pcs";
            ws.Cells[2, 21, 5, 21].Merge = true;
            ws.Cells[2, 21].Value = "Total Weight";
            ws.Cells[2, 22, 5, 22].Merge = true;
            ws.Cells[2, 22].Value = "Total Total $";
            ws.Cells[2, 23, 5, 23].Merge = true;
            ws.Cells[2, 23].Value = "Per Ct";

            for (int col = 20; col <= 23; col++)
            {
                var cell = ws.Cells[2, col];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#F2F2F2"));
            }

            // Row 3: Stock Category Headers
            // Mumbai
            ws.Cells[3, 2, 3, 7].Merge = true;
            ws.Cells[3, 2].Value = "WHITE STOCK";
            // NY
            ws.Cells[3, 8, 3, 13].Merge = true;
            ws.Cells[3, 8].Value = "WHITE STOCK";
            // SURAT
            ws.Cells[3, 14, 3, 19].Merge = true;
            ws.Cells[3, 14].Value = "WHITE STOCK";

            for (int col = 2; col <= 19; col += 6)
            {
                ws.Cells[3, col].Style.Font.Bold = true;
                ws.Cells[3, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[3, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#F2F2F2"));
            }

            // Row 4: EX/VG Headers
            for (int col = 2; col <= 19; col += 6)
            {
                // EX
                ws.Cells[4, col, 4, col + 2].Merge = true;
                ws.Cells[4, col].Value = "EX";
                ws.Cells[4, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[4, col].Style.Fill.BackgroundColor.SetColor(colGreenEX);
                ws.Cells[4, col].Style.Font.Color.SetColor(colWhite);
                ws.Cells[4, col].Style.Font.Bold = true;

                // VG
                ws.Cells[4, col + 3, 4, col + 5].Merge = true;
                ws.Cells[4, col + 3].Value = "VG";
                ws.Cells[4, col + 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[4, col + 3].Style.Fill.BackgroundColor.SetColor(colRedVG);
                ws.Cells[4, col + 3].Style.Font.Color.SetColor(colWhite);
                ws.Cells[4, col + 3].Style.Font.Bold = true;
            }

            // Row 5: Column Subheaders & Size Merged Header
            ws.Cells[2, 1, 4, 1].Merge = true;
            ws.Cells[5, 1].Value = "SIZE";
            ws.Cells[5, 1].Style.Font.Bold = true;

            for (int col = 2; col <= 19; col += 3)
            {
                ws.Cells[5, col].Value = "Pcs";
                ws.Cells[5, col + 1].Value = "Weight";
                ws.Cells[5, col + 2].Value = "Total $";
                
                ws.Cells[5, col].Style.Font.Bold = true;
                ws.Cells[5, col + 1].Style.Font.Bold = true;
                ws.Cells[5, col + 2].Style.Font.Bold = true;
            }

            // Apply light grey background to Row 5 subheaders
            var row5Headers = ws.Cells[5, 1, 5, 19];
            row5Headers.Style.Fill.PatternType = ExcelFillStyle.Solid;
            row5Headers.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#F2F2F2"));

            int totRow = sizeList.Count + 6;
            int pctRow = sizeList.Count + 7;

            // Row 6 to sizeList.Count + 5: Populate Data
            for (int i = 0; i < sizeList.Count; i++)
            {
                var sizeItem = sizeList[i];
                int r = 6 + i;

                string label = sizeItem.FromSize.ToString("0.00") + "-" + (Math.Truncate(sizeItem.ToSize * 100) / 100).ToString("0.00");
                ws.Cells[r, 1].Value = label;
                ws.Cells[r, 1].Style.Font.Bold = true;

                // Match stones
                var matched = parsed.Where(s => s.Weight >= sizeItem.FromSize && s.Weight <= sizeItem.ToSize).ToList();

                // Mumbai groups
                var mbWE = matched.Where(s => s.Location == "MUMBAI" && s.IsWhite && s.IsEX).ToList();
                var mbWV = matched.Where(s => s.Location == "MUMBAI" && s.IsWhite && !s.IsEX).ToList();
                var mbCE = matched.Where(s => s.Location == "MUMBAI" && !s.IsWhite && s.IsEX).ToList();
                var mbCV = matched.Where(s => s.Location == "MUMBAI" && !s.IsWhite && !s.IsEX).ToList();

                // NY groups
                var nyWE = matched.Where(s => s.Location == "NY" && s.IsWhite && s.IsEX).ToList();
                var nyWV = matched.Where(s => s.Location == "NY" && s.IsWhite && !s.IsEX).ToList();
                var nyCE = matched.Where(s => s.Location == "NY" && !s.IsWhite && s.IsEX).ToList();
                var nyCV = matched.Where(s => s.Location == "NY" && !s.IsWhite && !s.IsEX).ToList();

                // Surat groups
                var suWE = matched.Where(s => s.Location == "SURAT" && s.IsWhite && s.IsEX).ToList();
                var suWV = matched.Where(s => s.Location == "SURAT" && s.IsWhite && !s.IsEX).ToList();
                var suCE = matched.Where(s => s.Location == "SURAT" && !s.IsWhite && s.IsEX).ToList();
                var suCV = matched.Where(s => s.Location == "SURAT" && !s.IsWhite && !s.IsEX).ToList();

                // Write Mumbai
                WriteGroupCells(ws, r, 2, mbWE);
                WriteGroupCells(ws, r, 5, mbWV);

                // Write NY
                WriteGroupCells(ws, r, 8, nyWE);
                WriteGroupCells(ws, r, 11, nyWV);

                // Write Surat
                WriteGroupCells(ws, r, 14, suWE);
                WriteGroupCells(ws, r, 17, suWV);

                // Write Formulas for totals
                ws.Cells[r, 20].Formula = $"SUM(B{r},E{r},H{r},K{r},N{r},Q{r})";
                ws.Cells[r, 21].Formula = $"SUM(C{r},F{r},I{r},L{r},O{r},R{r})";
                ws.Cells[r, 22].Formula = $"SUM(D{r},G{r},J{r},M{r},P{r},S{r})";
                ws.Cells[r, 23].Formula = $"IF(U{r}>0,V{r}/U{r},0)";

                // Number Formats
                ws.Cells[r, 20].Style.Numberformat.Format = "#,##0;(#,##0);\"\"";
                ws.Cells[r, 21].Style.Numberformat.Format = "#,##0.00;(#,##0.00);\"\"";
                ws.Cells[r, 22].Style.Numberformat.Format = "#,##0;(#,##0);\"\"";
                ws.Cells[r, 23].Style.Numberformat.Format = "#,##0.00;(#,##0.00);\"\"";
            }

            // Row totRow: TOTAL
            ws.Cells[totRow, 1].Value = "TOTAL";
            ws.Cells[totRow, 1].Style.Font.Bold = true;

            for (int col = 2; col <= 19; col++)
            {
                string colLetter = ExcelCellAddress.GetColumnLetter(col);
                ws.Cells[totRow, col].Formula = $"SUM({colLetter}6:{colLetter}{totRow-1})";
                ws.Cells[totRow, col].Style.Font.Bold = true;
                
                // Formatting
                if (col % 3 == 0) // Total $
                {
                    ws.Cells[totRow, col].Style.Numberformat.Format = "#,##0;(#,##0);\"\"";
                }
                else if (col % 3 == 2) // Pcs
                {
                    ws.Cells[totRow, col].Style.Numberformat.Format = "#,##0;(#,##0);\"\"";
                }
                else // Weight
                {
                    ws.Cells[totRow, col].Style.Numberformat.Format = "#,##0.00;(#,##0.00);\"\"";
                }
            }

            // Summary totals
            ws.Cells[totRow, 20].Formula = $"SUM(T6:T{totRow-1})";
            ws.Cells[totRow, 21].Formula = $"SUM(U6:U{totRow-1})";
            ws.Cells[totRow, 22].Formula = $"SUM(V6:V{totRow-1})";
            ws.Cells[totRow, 23].Formula = $"IF(U{totRow}>0,V{totRow}/U{totRow},0)";

            for (int col = 20; col <= 23; col++)
            {
                ws.Cells[totRow, col].Style.Font.Bold = true;
                if (col == 20 || col == 22)
                    ws.Cells[totRow, col].Style.Numberformat.Format = "#,##0;(#,##0);\"\"";
                else
                    ws.Cells[totRow, col].Style.Numberformat.Format = "#,##0.00;(#,##0.00);\"\"";
            }

            // Background styles for TOTAL row
            var totBtoY = ws.Cells[totRow, 1, totRow, 19];
            totBtoY.Style.Fill.PatternType = ExcelFillStyle.Solid;
            totBtoY.Style.Fill.BackgroundColor.SetColor(colTotalPurple);

            var totZtoAB = ws.Cells[totRow, 20, totRow, 22];
            totZtoAB.Style.Fill.PatternType = ExcelFillStyle.Solid;
            totZtoAB.Style.Fill.BackgroundColor.SetColor(colTotalGreen);

            ws.Cells[totRow, 23].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[totRow, 23].Style.Fill.BackgroundColor.SetColor(colGold);

            // Row pctRow: %
            ws.Cells[pctRow, 1].Value = "%";
            ws.Cells[pctRow, 1].Style.Font.Bold = true;

            for (int col = 2; col <= 19; col++)
            {
                string colLetter = ExcelCellAddress.GetColumnLetter(col);
                int typeMod = (col - 2) % 3;
                if (typeMod == 0) // Pcs % relative to T{totRow}
                {
                    ws.Cells[pctRow, col].Formula = $"IF(T{totRow}>0,{colLetter}{totRow}/T{totRow}*100,0)";
                }
                else if (typeMod == 1) // Weight % relative to U{totRow}
                {
                    ws.Cells[pctRow, col].Formula = $"IF(U{totRow}>0,{colLetter}{totRow}/U{totRow}*100,0)";
                }
                else // Total $ % relative to V{totRow}
                {
                    ws.Cells[pctRow, col].Formula = $"IF(V{totRow}>0,{colLetter}{totRow}/V{totRow}*100,0)";
                }
                ws.Cells[pctRow, col].Style.Font.Bold = true;
                ws.Cells[pctRow, col].Style.Numberformat.Format = "0.00";
            }

            // Summary % formulas
            ws.Cells[pctRow, 20].Formula = "100.00";
            ws.Cells[pctRow, 21].Formula = "100.00";
            ws.Cells[pctRow, 22].Formula = "100.00";
            ws.Cells[pctRow, 23].Value = ""; // empty for Per Ct %

            for (int col = 20; col <= 22; col++)
            {
                ws.Cells[pctRow, col].Style.Font.Bold = true;
                ws.Cells[pctRow, col].Style.Numberformat.Format = "0.00";
            }

            // Background styles for % row
            var pctAll = ws.Cells[pctRow, 1, pctRow, 23];
            pctAll.Style.Fill.PatternType = ExcelFillStyle.Solid;
            pctAll.Style.Fill.BackgroundColor.SetColor(colTotalPurple);

            // Borders
            SetBorders(ws, 2, 1, pctRow, 23);

            // Column Widths
            ws.Column(1).Width = 12; // SIZE
            for (int col = 2; col <= 19; col++)
            {
                if (col % 3 == 0) ws.Column(col).Width = 10; // Total $
                else if (col % 3 == 2) ws.Column(col).Width = 7; // Pcs
                else ws.Column(col).Width = 9; // Weight
            }
            ws.Column(20).Width = 10; // Total Pcs
            ws.Column(21).Width = 12; // Total Weight
            ws.Column(22).Width = 14; // Total Total $
            ws.Column(23).Width = 10; // Per Ct
        }

        private static void WriteGroupCells(ExcelWorksheet ws, int r, int startCol, List<StockSummaryReportParsedStone> list)
        {
            if (list.Count > 0)
            {
                ws.Cells[r, startCol].Value = list.Count;
                ws.Cells[r, startCol + 1].Value = list.Sum(s => s.Weight);
                ws.Cells[r, startCol + 2].Value = Math.Round(list.Sum(s => s.SaleAmt), 0);

                ws.Cells[r, startCol].Style.Numberformat.Format = "#,##0;(#,##0);\"\"";
                ws.Cells[r, startCol + 1].Style.Numberformat.Format = "#,##0.00;(#,##0.00);\"\"";
                ws.Cells[r, startCol + 2].Style.Numberformat.Format = "#,##0;(#,##0);\"\"";
            }
            else
            {
                ws.Cells[r, startCol].Value = null;
                ws.Cells[r, startCol + 1].Value = null;
                ws.Cells[r, startCol + 2].Value = null;
            }
        }

        private static void AddStoneWiseDetailsSheet(ExcelPackage p, string tableName, string dateStr)
        {
            try
            {
                string connString = null;
                var connSetting = ConfigurationManager.ConnectionStrings["StockDetailConnectionString"];
                if (connSetting != null)
                {
                    connString = connSetting.ConnectionString;
                }
                else
                {
                    connString = "Data Source=DESKTOP-C70BOOF;Initial Catalog=SLIP;Integrated Security=True";
                }
                string query = "";
                
                DateTime parsedDate;
                string formattedDate = null;
                if (!string.IsNullOrEmpty(dateStr) && dateStr != "NaN-NaN-NaN")
                {
                    if (DateTime.TryParseExact(dateStr, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDate))
                    {
                        formattedDate = parsedDate.ToString("yyyy-MM-dd");
                    }
                    else if (DateTime.TryParse(dateStr, out parsedDate))
                    {
                        formattedDate = parsedDate.ToString("yyyy-MM-dd");
                    }
                }

                if (tableName == "DMX_MEMOStock")
                {
                    query = @"
                        SELECT 
                            'MEMO' AS Status, ID, Stone_NO, Shape, Weight, Color, Clarity, Cut, Polish, Symm, 
                            FlrIntens, FlrColor, FColor, FCIntens, FCOverton, Lab, Lab_Report_No, Location, 
                            LiveRAP, SaleDis, SaleRate, SaleAmt, Trans_No, Trans_Date, Trans_Due_Day, 
                            Trans_Due_Date, Trans_Rap, Trans_Dis, Trans_Rate, Trans_Amt, NewArrival, 
                            StockStatus, CVD_HPHT, Country, State, City, Seller_Name, Branch, EntryBy, 
                            EntryDate, Party, PartyCountry, ExRate, PacketPcs, Created, IsActive
                        FROM dbo.DMX_MEMOStock
                        WHERE IsActive = 1" + (string.IsNullOrEmpty(formattedDate) ? "" : " AND CAST(Created AS DATE) = @CreatedDate") + @"
                        UNION ALL
                        SELECT 
                            'HOLD' AS Status, ID, Stone_NO, Shape, Weight, Color, Clarity, Cut, Polish, Symm, 
                            FlrIntens, FlrColor, FColor, FCIntens, FCOverton, Lab, Lab_Report_No, Location, 
                            LiveRAP, SaleDis, SaleRate, SaleAmt, Trans_No, Trans_Date, Trans_Due_Day, 
                            Trans_Due_Date, Trans_Rap, Trans_Dis, Trans_Rate, Trans_Amt, NewArrival, 
                            StockStatus, CVD_HPHT, Country, State, City, Seller_Name, Branch, EntryBy, 
                            EntryDate, Party, PartyCountry, ExRate, PacketPcs, Created, IsActive
                        FROM dbo.DMX_HOLDStock
                        WHERE IsActive = 1" + (string.IsNullOrEmpty(formattedDate) ? "" : " AND CAST(Created AS DATE) = @CreatedDate");
                }
                else
                {
                    query = "SELECT * FROM dbo." + tableName + " WHERE IsActive = 1";
                    if (!string.IsNullOrEmpty(formattedDate))
                    {
                        query += " AND CAST(Created AS DATE) = @CreatedDate";
                    }
                }

                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connString))
                {
                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                    {
                        if (!string.IsNullOrEmpty(formattedDate))
                        {
                            cmd.Parameters.AddWithValue("@CreatedDate", formattedDate);
                        }
                        
                        cmd.CommandText = query;
                        cmd.Connection = conn;
                        
                        using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            
                            ExcelWorksheet ws = p.Workbook.Worksheets.Add("STONE WISE DETAILS");
                            ws.View.ShowGridLines = true;
                            
                            ws.Cells["A1"].LoadFromDataTable(dt, true);
                            
                            int colCount = dt.Columns.Count;
                            if (colCount > 0)
                            {
                                var headerRange = ws.Cells[1, 1, 1, colCount];
                                headerRange.Style.Font.Bold = true;
                                headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#D9D9D9"));
                                headerRange.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                
                                if (ws.Dimension != null)
                                {
                                    ws.Cells[ws.Dimension.Address].AutoFitColumns();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                if (HttpContext.Current == null)
                {
                    Console.WriteLine("ERROR adding STONE WISE DETAILS: " + ex.ToString());
                }
            }
        }
        public static string AVLStockWithSalesExport(List<Slip.Models.DmxStockAndSaleSummary> data, string date)
        {
            try
            {
                ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");
                string filename = "AVL_Stock_Sales_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                string stReturnFileName = "";

                using (ExcelPackage p = new ExcelPackage())
                {
                    p.Workbook.Properties.Author = "Diamx Stock";
                    p.Workbook.Properties.Title = "AVL Stock with Sales";

                    ExcelWorksheet ws = p.Workbook.Worksheets.Add("AvailableStock");
                    ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.View.ShowGridLines = true;

                    int totalCols = 3 + (7 * 3) + 3; // 3 (Location, Size, Color) + 7 clarities * 3 + Total * 3 = 27 cols

                    var titleCell = ws.Cells[1, 1, 1, totalCols];
                    titleCell.Merge = true;
                    titleCell.Value = $"CURRENT AVAILABLE STOCK (IGI) + {date} SALE REPORT";
                    titleCell.Style.Font.Size = 16;
                    titleCell.Style.Font.Bold = true;
                    titleCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    titleCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FCE4D6"));
                    ws.Row(1).Height = 35;

                    string[] clarities = { "FL", "IF", "VVS1", "VVS2", "VS1", "VS2", "SI1" };
                    string[] bgColors = { "#D9D9D9", "#C6E0B4", "#BDD7EE", "#F8CBAD", "#FFE699", "#9BC2E6", "#FF99CC" };

                    ws.Cells[2, 1, 3, 1].Merge = true;
                    ws.Cells[2, 1].Value = "LOCATION";
                    ws.Cells[2, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[2, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFD966"));
                    ws.Cells[2, 1].Style.Font.Bold = true;

                    ws.Cells[2, 2, 3, 2].Merge = true;
                    ws.Cells[2, 2].Value = "SIZE";
                    ws.Cells[2, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[2, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFD966"));
                    ws.Cells[2, 2].Style.Font.Bold = true;

                    ws.Cells[2, 3, 3, 3].Merge = true;
                    ws.Cells[2, 3].Value = "COLOR";
                    ws.Cells[2, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[2, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFD966"));
                    ws.Cells[2, 3].Style.Font.Bold = true;

                    // Apply AutoFilter
                    ws.Cells[3, 1, 3, totalCols].AutoFilter = true;

                    int col = 4;
                    for (int i = 0; i < clarities.Length; i++)
                    {
                        ws.Cells[2, col, 2, col + 2].Merge = true;
                        ws.Cells[2, col].Value = clarities[i];
                        ws.Cells[2, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[2, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml(bgColors[i]));
                        ws.Cells[2, col].Style.Font.Bold = true;

                        ws.Cells[3, col].Value = "Stock";
                        ws.Cells[3, col + 1].Value = "Sale";
                        ws.Cells[3, col + 2].Value = "%";

                        ws.Cells[3, col, 3, col + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[3, col, 3, col + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFD966"));
                        ws.Cells[3, col, 3, col + 2].Style.Font.Bold = true;

                        col += 3;
                    }

                    ws.Cells[2, col].Value = "Total Stock";
                    ws.Cells[2, col + 1].Value = "Total Sale";
                    ws.Cells[2, col + 2].Value = "Total %";
                    ws.Cells[2, col, 3, col].Merge = true;
                    ws.Cells[2, col + 1, 3, col + 1].Merge = true;
                    ws.Cells[2, col + 2, 3, col + 2].Merge = true;

                    ws.Cells[2, col, 3, col + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[2, col, 3, col + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#A9D08E"));
                    ws.Cells[2, col, 3, col + 2].Style.Font.Bold = true;
                    var locGroups = data.GroupBy(d => d.Location).OrderBy(g => g.Key);
                    int r = 4;

                    foreach (var lg in locGroups)
                    {
                        // --- LOCATION TOTAL ROW ---
                        ws.Cells[r, 1].Value = lg.Key;
                        ws.Cells[r, 1, r, totalCols].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[r, 1, r, totalCols].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFF00")); // Yellow
                        ws.Cells[r, 1, r, totalCols].Style.Font.Bold = true;

                        int locCol = 4;
                        int locTotalStock = 0;
                        int locTotalSale = 0;

                        for (int i = 0; i < clarities.Length; i++)
                        {
                            int sumStock = lg.Where(x => x.Clarity == clarities[i]).Sum(x => x.StockPcs);
                            int sumSale = lg.Where(x => x.Clarity == clarities[i]).Sum(x => x.SalePcs);

                            if (sumStock > 0 || sumSale > 0)
                            {
                                if (sumStock > 0) ws.Cells[r, locCol].Value = sumStock;
                                if (sumSale > 0) ws.Cells[r, locCol + 1].Value = sumSale;

                                if (sumStock == 0 && sumSale > 0)
                                    ws.Cells[r, locCol + 2].Value = "#DIV/0!";
                                else if (sumStock > 0)
                                {
                                    ws.Cells[r, locCol + 2].Value = ((decimal)sumSale / sumStock) * 100;
                                    ws.Cells[r, locCol + 2].Style.Numberformat.Format = "0.00";
                                }
                            }
                            
                            locTotalStock += sumStock;
                            locTotalSale += sumSale;
                            locCol += 3;
                        }

                        if (locTotalStock > 0 || locTotalSale > 0)
                        {
                            if (locTotalStock > 0) ws.Cells[r, locCol].Value = locTotalStock;
                            if (locTotalSale > 0) ws.Cells[r, locCol + 1].Value = locTotalSale;
                            if (locTotalStock == 0 && locTotalSale > 0)
                                ws.Cells[r, locCol + 2].Value = "#DIV/0!";
                            else if (locTotalStock > 0)
                            {
                                ws.Cells[r, locCol + 2].Value = ((decimal)locTotalSale / locTotalStock) * 100;
                                ws.Cells[r, locCol + 2].Style.Numberformat.Format = "0.00";
                            }
                        }

                        r++;

                        // --- DATA ROWS ---
                        int startLocR = r;
                        var sizeGroups = lg.GroupBy(d => d.SizeBucket).OrderBy(g => g.Key);

                        foreach (var sg in sizeGroups)
                        {
                            int startR = r;
                            var existingColors = sg.Select(x => x.Color).Distinct().OrderBy(c => c).ToList();

                            if (existingColors.Count == 0) existingColors.Add("");

                            foreach (var c in existingColors)
                            {
                                ws.Cells[r, 3].Value = c;
                                var colorData = sg.Where(x => x.Color == c).ToList();

                                int cCol = 4;
                                int totalRowStock = 0;
                                int totalRowSale = 0;

                                for (int i = 0; i < clarities.Length; i++)
                                {
                                    var cellData = colorData.FirstOrDefault(x => x.Clarity == clarities[i]);
                                    int stock = cellData != null ? cellData.StockPcs : 0;
                                    int sale = cellData != null ? cellData.SalePcs : 0;

                                    if (stock > 0 || sale > 0)
                                    {
                                        if (stock > 0) ws.Cells[r, cCol].Value = stock;
                                        if (sale > 0) ws.Cells[r, cCol + 1].Value = sale;

                                        if (stock == 0 && sale > 0)
                                            ws.Cells[r, cCol + 2].Value = "#DIV/0!";
                                        else if (stock > 0)
                                        {
                                            ws.Cells[r, cCol + 2].Value = ((decimal)sale / stock) * 100;
                                            ws.Cells[r, cCol + 2].Style.Numberformat.Format = "0.00";
                                        }
                                    }
                                    totalRowStock += stock;
                                    totalRowSale += sale;
                                    cCol += 3;
                                }

                                if (totalRowStock > 0 || totalRowSale > 0)
                                {
                                    if (totalRowStock > 0) ws.Cells[r, cCol].Value = totalRowStock;
                                    if (totalRowSale > 0) ws.Cells[r, cCol + 1].Value = totalRowSale;
                                    if (totalRowStock == 0 && totalRowSale > 0)
                                        ws.Cells[r, cCol + 2].Value = "#DIV/0!";
                                    else if (totalRowStock > 0)
                                    {
                                        ws.Cells[r, cCol + 2].Value = ((decimal)totalRowSale / totalRowStock) * 100;
                                        ws.Cells[r, cCol + 2].Style.Numberformat.Format = "0.00";
                                    }
                                }
                                r++;
                            }

                            ws.Cells[startR, 2].Value = sg.Key;
                            if (r - 1 > startR)
                            {
                                ws.Cells[startR, 2, r - 1, 2].Merge = true;
                                ws.Cells[startR, 2, r - 1, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            }
                        }

                        // We already put Location string in the Total Row. 
                        // To make the Location filter work on the data rows, we will put the Location string in the data rows too, but in a merged cell.
                        ws.Cells[startLocR, 1].Value = lg.Key;
                        if (r - 1 > startLocR)
                        {
                            ws.Cells[startLocR, 1, r - 1, 1].Merge = true;
                            ws.Cells[startLocR, 1, r - 1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        }
                    }

                    ws.Cells[r, 1, r, 3].Merge = true;
                    ws.Cells[r, 1].Value = "TOTAL";
                    ws.Cells[r, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[r, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#A6A6A6"));
                    ws.Cells[r, 1].Style.Font.Bold = true;

                    int tCol = 4;
                    int grandTotalStock = 0;
                    int grandTotalSale = 0;
                    for (int i = 0; i < clarities.Length; i++)
                    {
                        int sumStock = data.Where(x => x.Clarity == clarities[i]).Sum(x => x.StockPcs);
                        int sumSale = data.Where(x => x.Clarity == clarities[i]).Sum(x => x.SalePcs);

                        ws.Cells[r, tCol].Value = sumStock;
                        ws.Cells[r, tCol + 1].Value = sumSale;

                        if (sumStock == 0 && sumSale > 0)
                            ws.Cells[r, tCol + 2].Value = "#DIV/0!";
                        else if (sumStock > 0)
                        {
                            ws.Cells[r, tCol + 2].Value = ((decimal)sumSale / sumStock) * 100;
                            ws.Cells[r, tCol + 2].Style.Numberformat.Format = "0.00";
                        }

                        ws.Cells[r, tCol, r, tCol + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[r, tCol, r, tCol + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml(bgColors[i]));
                        ws.Cells[r, tCol, r, tCol + 2].Style.Font.Bold = true;

                        grandTotalStock += sumStock;
                        grandTotalSale += sumSale;
                        tCol += 3;
                    }

                    ws.Cells[r, tCol].Value = grandTotalStock;
                    ws.Cells[r, tCol + 1].Value = grandTotalSale;
                    if (grandTotalStock == 0 && grandTotalSale > 0)
                        ws.Cells[r, tCol + 2].Value = "#DIV/0!";
                    else if (grandTotalStock > 0)
                    {
                        ws.Cells[r, tCol + 2].Value = ((decimal)grandTotalSale / grandTotalStock) * 100;
                        ws.Cells[r, tCol + 2].Style.Numberformat.Format = "0.00";
                    }

                    ws.Cells[r, tCol, r, tCol + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[r, tCol, r, tCol + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#A9D08E"));
                    ws.Cells[r, tCol, r, tCol + 2].Style.Font.Bold = true;

                    var modelTable = ws.Cells[2, 1, r, totalCols];
                    modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    string Folderpath = ConfigurationManager.AppSettings["ExcelFiles"];
                    if (string.IsNullOrEmpty(Folderpath)) Folderpath = "\\ExcelFiles";

                    if (HttpContext.Current != null)
                    {
                        if (!Directory.Exists(HttpContext.Current.Server.MapPath(Folderpath)))
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath(Folderpath));
                        stReturnFileName = HttpContext.Current.Server.MapPath(Folderpath + "\\" + filename);
                    }
                    else
                    {
                        string localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExcelFiles");
                        if (!Directory.Exists(localPath)) Directory.CreateDirectory(localPath);
                        stReturnFileName = Path.Combine(localPath, filename);
                    }

                    File.WriteAllBytes(stReturnFileName, p.GetAsByteArray());
                }
                return stReturnFileName;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return "";
            }
        }
    }
}


