using Newtonsoft.Json;
using OfficeOpenXml;
using Slip.Models;
using Slip.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using static Slip.Models.Filter.SessionExpireFilter;
using static Slip.Models.MainRoughSummary;

namespace Slip.Controllers
{
    public class SlipController : BaseController
    {
        #region :: SLIP ENTRY ::
        public ActionResult Slip_Entry()
        {
            SetModulePermissions("Slip", "Slip_Entry");
            return View();
        }
        #endregion


        public ActionResult AddReports()
        {
            return View();
        }
        public JsonResult Daily_Report_Insert_Using_Excel(HttpPostedFileBase excelFile)
        {
            ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");

            string Message = "";
            List<DailyReport> polishList = new List<DailyReport>();
            List<DailyReport> roughList = new List<DailyReport>();
            List<DailyReport> jwList = new List<DailyReport>();
            List<DailyReport> swList = new List<DailyReport>();
            List<DailyReport> colorList = new List<DailyReport>();
            List<DailyReport> underprocessList = new List<DailyReport>();
            List<DailyReport> transferList = new List<DailyReport>();
            List<DailyReport> detailList = new List<DailyReport>();
            try
            {
                if (excelFile == null || excelFile.ContentLength == 0)
                    Message = "Please upload an Excel file";

                if (Message != "Please upload an Excel file")
                {

                    string Date = "";
                    using (var package = new OfficeOpenXml.ExcelPackage(excelFile.InputStream))
                    {
                        ExcelWorksheet ws = package.Workbook.Worksheets[0];

                        int row = 1;

                        int SrNo = 1;
                        string Status = "";

                        // ------------------------------
                        // SECTION 1: Date
                        // ------------------------------
                        row = 1; // skip header row

                        Date = ws.Cells[row, 1].Text;
                        Date = Regex.Match(Date, @"\(([^)]*)\)").Groups[1].Value;

                        // ------------------------------
                        // SECTION 2: Polish
                        // ------------------------------
                        row = CommonMethods.FindRow(ws, 1, "POLISH");
                        Status = ws.Cells[row, 1].Text;
                        row += 2; // skip header row
                        SrNo = 1;
                        while (ws.Cells[row, 1].Value != null)
                        {
                            polishList.Add(new DailyReport
                            {
                                SrNo = SrNo,
                                Status = Status,
                                Process = ws.Cells[row, 1].Text,
                                PolishWeight = CommonMethods.GetDecimal(ws.Cells[row, 2])
                            });
                            row++;
                            SrNo++;
                        }

                        // ------------------------------
                        // SECTION 3: Rough
                        // ------------------------------
                        row = CommonMethods.FindRow(ws, 5, "ROUGH");
                        Status = ws.Cells[row, 5].Text;
                        row += 2; // skip header row
                        SrNo = 1;
                        while (ws.Cells[row, 1].Value != null)
                        {
                            roughList.Add(new DailyReport
                            {
                                SrNo = SrNo,
                                Status = Status,
                                Process = ws.Cells[row, 5].Text,
                                RPartWeight = CommonMethods.GetDecimal(ws.Cells[row, 6])
                            });
                            row++;
                            SrNo++;
                        }

                        // ------------------------------
                        // SECTION 4: Under Process (Jumbo) (White)
                        // ------------------------------
                        row = CommonMethods.FindRow(ws, 1, "Under Process (Jumbo) (White)");
                        Status = ws.Cells[row, 1].Text;
                        row += 2; // skip header row
                        SrNo = 1;
                        while (ws.Cells[row, 1].Value != null)
                        {
                            jwList.Add(new DailyReport
                            {
                                SrNo = SrNo,
                                Status = Status,
                                Process = ws.Cells[row, 1].Text,
                                RPartWeight = CommonMethods.GetDecimal(ws.Cells[row, 3]),
                                PolishPrd = CommonMethods.GetDecimal(ws.Cells[row, 4]),
                                PrdPer = CommonMethods.GetDecimal(ws.Cells[row, 5]),
                                PolishWeight = CommonMethods.GetDecimal(ws.Cells[row, 6]),
                                PolishPer = CommonMethods.GetDecimal(ws.Cells[row, 7])
                            });
                            row++;
                            SrNo++;
                        }

                        // ------------------------------
                        // SECTION 5: Under Process (Small) (White)
                        // ------------------------------
                        row = CommonMethods.FindRow(ws, 1, "Under Process (Small) (White)");
                        Status = ws.Cells[row, 1].Text;
                        row += 2;
                        SrNo = 1;
                        while (ws.Cells[row, 1].Value != null)
                        {
                            swList.Add(new DailyReport
                            {
                                SrNo = SrNo,
                                Status = Status,
                                Process = ws.Cells[row, 1].Text,
                                RPartWeight = CommonMethods.GetDecimal(ws.Cells[row, 3]),
                                PolishPrd = CommonMethods.GetDecimal(ws.Cells[row, 4]),
                                PrdPer = CommonMethods.GetDecimal(ws.Cells[row, 5]),
                                PolishWeight = CommonMethods.GetDecimal(ws.Cells[row, 6]),
                                PolishPer = CommonMethods.GetDecimal(ws.Cells[row, 7])
                            });
                            row++;
                            SrNo++;
                        }

                        // ------------------------------
                        // SECTION 6: Under Process (Color)
                        // ------------------------------
                        row = CommonMethods.FindRow(ws, 1, "Under Process (Color)");
                        Status = ws.Cells[row, 1].Text;
                        row += 2;
                        SrNo = 1;
                        while (ws.Cells[row, 1].Value != null)
                        {
                            colorList.Add(new DailyReport
                            {
                                SrNo = SrNo,
                                Status = Status,
                                Process = ws.Cells[row, 1].Text,
                                RPartWeight = CommonMethods.GetDecimal(ws.Cells[row, 3]),
                                PolishPrd = CommonMethods.GetDecimal(ws.Cells[row, 4]),
                                PrdPer = CommonMethods.GetDecimal(ws.Cells[row, 5]),
                                PolishWeight = CommonMethods.GetDecimal(ws.Cells[row, 6]),
                                PolishPer = CommonMethods.GetDecimal(ws.Cells[row, 7])
                            });
                            row++;
                            SrNo++;
                        }

                        // ------------------------------
                        // SECTION 7: Under Process
                        // ------------------------------
                        row = CommonMethods.FindRow(ws, 1, "Under Process");
                        Status = ws.Cells[row, 1].Text;
                        row += 2;
                        SrNo = 1;
                        while (ws.Cells[row, 1].Value != null)
                        {
                            underprocessList.Add(new DailyReport
                            {
                                SrNo = SrNo,
                                Status = Status,
                                Process = ws.Cells[row, 1].Text,
                                RPartWeight = CommonMethods.GetDecimal(ws.Cells[row, 3]),
                                PolishPrd = CommonMethods.GetDecimal(ws.Cells[row, 4]),
                                PrdPer = CommonMethods.GetDecimal(ws.Cells[row, 5]),
                                PolishWeight = CommonMethods.GetDecimal(ws.Cells[row, 6]),
                                PolishPer = CommonMethods.GetDecimal(ws.Cells[row, 7])
                            });
                            row++;
                            SrNo++;
                        }

                        // ------------------------------
                        // SECTION 8: Transfer
                        // ------------------------------
                        row = CommonMethods.FindRow(ws, 1, "Transfer");
                        Status = ws.Cells[row, 1].Text;
                        row += 2;
                        SrNo = 1;
                        while (ws.Cells[row, 1].Value != null)
                        {
                            transferList.Add(new DailyReport
                            {
                                SrNo = SrNo,
                                Status = Status,
                                Process = "Transfer",
                                SizeCode = ws.Cells[row, 1].Text,
                                RPartWeight = CommonMethods.GetDecimal(ws.Cells[row, 3]),
                                PolishPrd = CommonMethods.GetDecimal(ws.Cells[row, 4]),
                                PrdPer = CommonMethods.GetDecimal(ws.Cells[row, 5]),
                                PolishWeight = CommonMethods.GetDecimal(ws.Cells[row, 6]),
                                PolishPer = CommonMethods.GetDecimal(ws.Cells[row, 7])
                            });
                            row++;
                            SrNo++;
                        }

                        // ------------------------------
                        // SECTION 9: Process Wise UnderProcess
                        // ------------------------------
                        row = CommonMethods.FindRow(ws, 1, "Process Wise UnderProcess");
                        Status = "Process Wise UnderProcess";
                        row += 2;
                        SrNo = 1;

                        while (ws.Cells[row, 2].Value != null)
                        {
                            detailList.Add(new DailyReport
                            {
                                SrNo = SrNo,
                                Status = Status,
                                Process = ws.Cells[row, 2].Text,
                                Pcs = Convert.ToInt32(ws.Cells[row, 3].Value),
                                Rough_Ct = CommonMethods.GetDecimal(ws.Cells[row, 4]),
                                Polish_Ct = CommonMethods.GetDecimal(ws.Cells[row, 5]),
                                Cur_Polish_Ct = CommonMethods.GetDecimal(ws.Cells[row, 6]),
                                DiffPer = CommonMethods.GetDecimal(ws.Cells[row, 7]),
                                Ideal_Ct = ws.Cells[row, 8].Text
                            });
                            row++;
                            SrNo++;
                        }
                    }

                    // Now you have all parsed lists:
                    // jumboList, smallList, receiveList, detailList

                    string pxmlStr = "";
                    if (polishList.Count > 0)
                    {
                        XmlDocument pXmldata = CommonMethods.ConvertToXml(polishList);
                        pxmlStr = "<DocumentElement>" + pXmldata.DocumentElement.InnerXml + "</DocumentElement>";
                    }
                    string rxmlStr = "";
                    if (roughList.Count > 0)
                    {
                        XmlDocument rXmldata = CommonMethods.ConvertToXml(roughList);
                        rxmlStr = "<DocumentElement>" + rXmldata.DocumentElement.InnerXml + "</DocumentElement>";
                    }

                    string jwxmlStr = "";
                    if (jwList.Count > 0)
                    {
                        XmlDocument jwXmldata = CommonMethods.ConvertToXml(jwList);
                        jwxmlStr = "<DocumentElement>" + jwXmldata.DocumentElement.InnerXml + "</DocumentElement>";
                    }

                    string swxmlStr = "";
                    if (swList.Count > 0)
                    {
                        XmlDocument swXmldata = CommonMethods.ConvertToXml(swList);
                        swxmlStr = "<DocumentElement>" + swXmldata.DocumentElement.InnerXml + "</DocumentElement>";
                    }
                    string cxmlStr = "";
                    if (colorList.Count > 0)
                    {
                        XmlDocument cXmldata = CommonMethods.ConvertToXml(colorList);
                        cxmlStr = "<DocumentElement>" + cXmldata.DocumentElement.InnerXml + "</DocumentElement>";
                    }
                    string uxmlStr = "";
                    if (underprocessList.Count > 0)
                    {
                        XmlDocument uXmldata = CommonMethods.ConvertToXml(underprocessList);
                        uxmlStr = "<DocumentElement>" + uXmldata.DocumentElement.InnerXml + "</DocumentElement>";
                    }
                    string txmlStr = "";
                    if (transferList.Count > 0)
                    {
                        XmlDocument tXmldata = CommonMethods.ConvertToXml(transferList);
                        txmlStr = "<DocumentElement>" + tXmldata.DocumentElement.InnerXml + "</DocumentElement>";
                    }
                    string dxmlStr = "";
                    if (detailList.Count > 0)
                    {
                        XmlDocument dXmldata = CommonMethods.ConvertToXml(detailList);
                        dxmlStr = "<DocumentElement>" + dXmldata.DocumentElement.InnerXml + "</DocumentElement>";
                    }

                    Message = DbHelper.ExecuteNonQueryWithMessage("Daily_Report_Insert_Using_Excel",
                        new SqlParameter("@ReportDate", Date),
                        new SqlParameter("@PolishXML", pxmlStr),
                        new SqlParameter("@RoughhXML", rxmlStr),
                        new SqlParameter("@JWXML", jwxmlStr),
                        new SqlParameter("@SWXML", swxmlStr),
                        new SqlParameter("@ColorXML", cxmlStr),
                        new SqlParameter("@UnderXML", uxmlStr),
                        new SqlParameter("@TransferXML", txmlStr),
                        new SqlParameter("@DetailsXML", dxmlStr),
                        new SqlParameter("@UserID", SessionFacade.UserSession.UserID));
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                Message = "ERROR: " + ex.Message;
            }
            return Json(new { Message });
        }
        public ActionResult MumbaiSubmit()
        {
            return View();
        }
        public JsonResult Daily_RP_Rough_Polish_GetData(string Type)
        {
            try
            {
                List<object> _list_Process = new List<object>();
                List<object> EMPEmployeeList = new List<object>();

                DataSet _DropDownList = DbHelper.ExecuteDataSet("Daily_RP_Rough_Polish_GetData",
                    new SqlParameter("@Type", Type));

                if (_DropDownList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[0].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[0].Columns[j].ToString(), _DropDownList.Tables[0].Rows[i][j].ToString());
                        }
                        _list_Process.Add(Values);
                    }
                }

                var DataField = _DropDownList.Tables[1].AsEnumerable()
               .Select(row => new DataFields
               {
                   dataField = row.Field<string>("dataField"),
                   dataType = row.Field<string>("dataType"),
                   format = row.Field<string>("format"),
                   alignment = row.Field<string>("alignment"),
                   visible = row.Field<bool>("visible"),
               })
               .ToList();

                var jsonResult = Json(new
                {
                    List = _list_Process,
                    DataField = DataField
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                var jsonResult = Json(new
                {
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
        }
        public JsonResult Daily_RP_Rough_Polish_Insert_Update_Delete(List<Daily_RP_Rough_Polish> IssueList, string Action, string Type)
        {
            string Message = "";
            try
            {
                XmlDocument Xmldata = CommonMethods.ConvertToXml(IssueList);
                string xmlStr = "<DocumentElement>" + Xmldata.DocumentElement.InnerXml + "</DocumentElement>";
                // ErrorLogger.ErrorLogStr("XML Create" + xmlStr);

                Message = DbHelper.ExecuteNonQueryWithMessage("Daily_RP_Rough_Polish_Insert_Update_Delete",
                    new SqlParameter("@XML", xmlStr),
                    new SqlParameter("@Action", Action),
                    new SqlParameter("@Type", Type),
                    new SqlParameter("@UserID", SessionFacade.UserSession.UserID));
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                Message = "ERROR: " + ex.Message;
            }

            return Json(new { Message = Message }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Vipulbhai()
        {
            return View();
        }
        public JsonResult After4POk_Loss_Insert_Using_Excel(HttpPostedFileBase excelFile)
        {
            ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");

            string Message = "";
            List<After4POk_Loss> List = new List<After4POk_Loss>();
            try
            {
                if (excelFile == null || excelFile.ContentLength == 0)
                    Message = "Please upload an Excel file";

                if (Message != "Please upload an Excel file")
                {

                    string Date = "";
                    using (var package = new OfficeOpenXml.ExcelPackage(excelFile.InputStream))
                    {
                        ExcelWorksheet ws = package.Workbook.Worksheets[0];

                        int row = 1;

                        int SrNo = 1;

                        // ------------------------------
                        // SECTION 1: Date
                        // ------------------------------

                        Date = ws.Cells[row, 1].Text;

                        // ------------------------------
                        // SECTION 1: Process Loss
                        // ------------------------------
                        row = CommonMethods.FindRow(ws, 1, "Process");
                        row += 2; // skip header row
                        SrNo = 1;
                        while (ws.Cells[row, 1].Value != null)
                        {
                            List.Add(new After4POk_Loss
                            {
                                SrNo = SrNo,
                                Process = ws.Cells[row, 1].Text,
                                IPcs = CommonMethods.ToNullableInt(ws.Cells[row, 2].Value),
                                IRPartWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 3]),
                                IPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 4]),
                                IModel = CommonMethods.ToNullableDecimal(ws.Cells[row, 5]),

                                RPcs = CommonMethods.ToNullableInt(ws.Cells[row, 6].Value),
                                RRPartWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 7]),
                                RPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 8]),
                                RModel = CommonMethods.ToNullableDecimal(ws.Cells[row, 9]),

                                DRPartWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 10]),
                                DiffRPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 11]),
                                DPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 12]),
                                DiffPPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 13]),

                                TPcs = CommonMethods.ToNullableInt(ws.Cells[row, 14].Value),
                                TRPartWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 15]),
                                TPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 16]),
                            });
                            row++;
                            SrNo++;
                        }
                    }
                    // Now you have all parsed lists:
                    // jumboList, smallList, receiveList, detailList

                    string xmlStr = "";
                    if (List.Count > 0)
                    {
                        XmlDocument pXmldata = CommonMethods.ConvertToXml(List);
                        xmlStr = "<DocumentElement>" + pXmldata.DocumentElement.InnerXml + "</DocumentElement>";
                    }
                    Message = DbHelper.ExecuteNonQueryWithMessage("After4POk_Loss_Insert_Using_Excel",
                        new SqlParameter("@ReportDate", Date),
                        new SqlParameter("@XML", xmlStr),
                        new SqlParameter("@UserID", SessionFacade.UserSession.UserID));
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                Message = "ERROR: " + ex.Message;
            }
            return Json(new { Message });
        }
        public JsonResult RoughTo4POk_Loss_Insert_Using_Excel(HttpPostedFileBase excelFile)
        {
            ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");

            string Message = "";
            List<RoughTo4POk_Loss> List = new List<RoughTo4POk_Loss>();
            try
            {
                if (excelFile == null || excelFile.ContentLength == 0)
                    Message = "Please upload an Excel file";

                if (Message != "Please upload an Excel file")
                {
                    string Date = "";
                    using (var package = new OfficeOpenXml.ExcelPackage(excelFile.InputStream))
                    {
                        ExcelWorksheet ws = package.Workbook.Worksheets[0];

                        int row = 1;
                        int SrNo = 1;

                        // ------------------------------
                        // SECTION 1: Date
                        // ------------------------------

                        Date = ws.Cells[row, 1].Text;

                        // ------------------------------
                        // SECTION 1: Process Loss
                        // ------------------------------
                        row = CommonMethods.FindRow(ws, 1, "Process");
                        row += 2; // skip header row
                        SrNo = 1;
                        while (ws.Cells[row, 1].Value != null)
                        {
                            List.Add(new RoughTo4POk_Loss
                            {
                                SrNo = SrNo,
                                Process = ws.Cells[row, 1].Text,
                                PrdRPartWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 2]),
                                PrdPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 3]),
                                PrdPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 4]),

                                FPRPartWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 5]),
                                FPPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 6]),
                                FPPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 7]),
                                FPModel = CommonMethods.ToNullableDecimal(ws.Cells[row, 8]),

                                DPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 9]),
                                DiffPPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 10]),

                                RE4P = CommonMethods.ToNullableInt(ws.Cells[row, 11].Value),
                            });
                            row++;
                            SrNo++;
                        }
                    }
                    // Now you have all parsed lists:
                    // jumboList, smallList, receiveList, detailList

                    string xmlStr = "";
                    if (List.Count > 0)
                    {
                        XmlDocument pXmldata = CommonMethods.ConvertToXml(List);
                        xmlStr = "<DocumentElement>" + pXmldata.DocumentElement.InnerXml + "</DocumentElement>";
                    }
                    Message = DbHelper.ExecuteNonQueryWithMessage("RoughTo4POk_Insert_Using_Excel",
                        new SqlParameter("@ReportDate", Date),
                        new SqlParameter("@XML", xmlStr),
                        new SqlParameter("@UserID", SessionFacade.UserSession.UserID));
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                Message = "ERROR: " + ex.Message;
            }
            return Json(new { Message });
        }


        public JsonResult Process_Wise_Timing_Insert_Using_Excel(HttpPostedFileBase excelFile)
        {
            ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");

            string Message = "";
            List<ProcessWiseTiming> JumboWhiteList = new List<ProcessWiseTiming>();
            List<ProcessWiseTiming> JumboColorList = new List<ProcessWiseTiming>();
            List<ProcessWiseTiming> SmallWhiteList = new List<ProcessWiseTiming>();
            List<ProcessWiseTiming> SmallColorList = new List<ProcessWiseTiming>();
            try
            {
                if (excelFile == null || excelFile.ContentLength == 0)
                    Message = "Please upload an Excel file";

                if (Message != "Please upload an Excel file")
                {

                    string Date = "";
                    string JumboWhiteDays = "";
                    string JumboColorDays = "";
                    string SmallWhiteDays = "";
                    string SmallColorDays = "";
                    string TableNo = "";
                    using (var package = new OfficeOpenXml.ExcelPackage(excelFile.InputStream))
                    {
                        ExcelWorksheet ws = package.Workbook.Worksheets[0];
                        ExcelWorksheet ws1 = package.Workbook.Worksheets[1];
                        ExcelWorksheet ws2 = package.Workbook.Worksheets[2];
                        ExcelWorksheet ws3 = package.Workbook.Worksheets[3];

                        int row = 1;

                        int SrNo = 1;

                        // ------------------------------
                        // SECTION 1: Date & TableNo
                        // ------------------------------

                        Date = ws.Cells[3, 1].Text;
                        TableNo = ws.Cells[2, 4].Text;

                        // ------------------------------
                        // SECTION 2: Jumbo White
                        // ------------------------------
                        JumboWhiteDays = ws.Cells[2, 1].Text;

                        row = CommonMethods.FindRow(ws, 1, "Process");
                        row++; // skip header row
                        SrNo = 1;
                        if (row > 0)
                        {
                            while (ws.Cells[row, 1].Value != null)
                            {
                                JumboWhiteList.Add(new ProcessWiseTiming
                                {
                                    SrNo = SrNo,
                                    FinishDays = Convert.ToInt32(JumboWhiteDays),
                                    Process = ws.Cells[row, 1].Text,
                                    Days = CommonMethods.ToNullableInt(ws.Cells[row, 2].Value),
                                    ProcessDay = CommonMethods.ToNullableInt(ws.Cells[row, 3].Value),
                                    TotalPcs = CommonMethods.ToNullableInt(ws.Cells[row, 4].Value),
                                    ONTIMEPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 5]),
                                    OVERDAYPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 6]),
                                });
                                row++;
                                SrNo++;
                            }
                        }

                        // ------------------------------
                        // SECTION 3: Jumbo Color
                        // ------------------------------
                        JumboColorDays = ws1.Cells[2, 1].Text;

                        row = CommonMethods.FindRow(ws1, 1, "Process");
                        row++; // skip header row
                        SrNo = 1;
                        if (row > 0)
                        {
                            while (ws1.Cells[row, 1].Value != null)
                            {
                                JumboColorList.Add(new ProcessWiseTiming
                                {
                                    SrNo = SrNo,
                                    FinishDays = Convert.ToInt32(JumboColorDays),
                                    Process = ws1.Cells[row, 1].Text,
                                    Days = CommonMethods.ToNullableInt(ws1.Cells[row, 2].Value),
                                    ProcessDay = CommonMethods.ToNullableInt(ws1.Cells[row, 3].Value),
                                    TotalPcs = CommonMethods.ToNullableInt(ws1.Cells[row, 4].Value),
                                    ONTIMEPer = CommonMethods.ToNullableDecimal(ws1.Cells[row, 5]),
                                    OVERDAYPer = CommonMethods.ToNullableDecimal(ws1.Cells[row, 6]),
                                });
                                row++;
                                SrNo++;
                            }
                        }

                        // ------------------------------
                        // SECTION 4: Small White
                        // ------------------------------
                        SmallWhiteDays = ws2.Cells[2, 1].Text;

                        row = CommonMethods.FindRow(ws2, 1, "Process");
                        row++; // skip header row
                        SrNo = 1;
                        if (row > 0)
                        {
                            while (ws2.Cells[row, 1].Value != null)
                            {
                                SmallWhiteList.Add(new ProcessWiseTiming
                                {
                                    SrNo = SrNo,
                                    FinishDays = Convert.ToInt32(SmallWhiteDays),
                                    Process = ws2.Cells[row, 1].Text,
                                    ProcessDay = CommonMethods.ToNullableInt(ws2.Cells[row, 2].Value),
                                    Days = CommonMethods.ToNullableInt(ws2.Cells[row, 3].Value),
                                    TotalPcs = CommonMethods.ToNullableInt(ws2.Cells[row, 4].Value),
                                    ONTIMEPer = CommonMethods.ToNullableDecimal(ws2.Cells[row, 5]),
                                    OVERDAYPer = CommonMethods.ToNullableDecimal(ws2.Cells[row, 6]),
                                });
                                row++;
                                SrNo++;
                            }
                        }


                        // ------------------------------
                        // SECTION 5: Small Color
                        // ------------------------------
                        SmallColorDays = ws3.Cells[2, 1].Text;

                        row = CommonMethods.FindRow(ws3, 1, "Process");
                        row++; // skip header row
                        SrNo = 1;
                        if (row > 0)
                        {
                            while (ws3.Cells[row, 1].Value != null)
                            {
                                SmallColorList.Add(new ProcessWiseTiming
                                {
                                    SrNo = SrNo,
                                    FinishDays = Convert.ToInt32(SmallColorDays),
                                    Process = ws3.Cells[row, 1].Text,
                                    ProcessDay = CommonMethods.ToNullableInt(ws3.Cells[row, 2].Value),
                                    Days = CommonMethods.ToNullableInt(ws3.Cells[row, 3].Value),
                                    TotalPcs = CommonMethods.ToNullableInt(ws3.Cells[row, 4].Value),
                                    ONTIMEPer = CommonMethods.ToNullableDecimal(ws3.Cells[row, 5]),
                                    OVERDAYPer = CommonMethods.ToNullableDecimal(ws3.Cells[row, 6]),
                                });
                                row++;
                                SrNo++;
                            }
                        }
                    }

                    // Now you have all parsed lists:
                    // jumboList, smallList, receiveList, detailList

                    string JWxmlStr = "";
                    if (JumboWhiteList.Count > 0)
                    {
                        XmlDocument JXmldata = CommonMethods.ConvertToXml(JumboWhiteList);
                        JWxmlStr = "<DocumentElement>" + JXmldata.DocumentElement.InnerXml + "</DocumentElement>";
                    }
                    string JCxmlStr = "";
                    if (JumboColorList.Count > 0)
                    {
                        XmlDocument JXmldata = CommonMethods.ConvertToXml(JumboColorList);
                        JCxmlStr = "<DocumentElement>" + JXmldata.DocumentElement.InnerXml + "</DocumentElement>";
                    }
                    string SWxmlStr = "";
                    if (SmallWhiteList.Count > 0)
                    {
                        XmlDocument SXmldata = CommonMethods.ConvertToXml(SmallWhiteList);
                        SWxmlStr = "<DocumentElement>" + SXmldata.DocumentElement.InnerXml + "</DocumentElement>";
                    }
                    string SCxmlStr = "";
                    if (SmallColorList.Count > 0)
                    {
                        XmlDocument SXmldata = CommonMethods.ConvertToXml(SmallColorList);
                        SCxmlStr = "<DocumentElement>" + SXmldata.DocumentElement.InnerXml + "</DocumentElement>";
                    }
                    Message = DbHelper.ExecuteNonQueryWithMessage("Process_Wise_Timing_Insert_Using_Excel",
                        new SqlParameter("@ReportDate", Date),
                        new SqlParameter("@TableNo", TableNo),
                        new SqlParameter("@JumboWhiteXML", JWxmlStr),
                        new SqlParameter("@JumboColorXML", JCxmlStr),
                        new SqlParameter("@SmallWhiteXML", SWxmlStr),
                        new SqlParameter("@SmallColorXML", SCxmlStr),
                        new SqlParameter("@UserID", SessionFacade.UserSession.UserID));
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                Message = "ERROR: " + ex.Message;
            }
            return Json(new { Message });
        }

        public JsonResult Process_Wise_Issue_Receive_Loss_Insert_Using_Excel(HttpPostedFileBase excelFile)
        {
            ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");

            string Message = "";
            List<Process_Wise_Issue_Receive_Loss> List = new List<Process_Wise_Issue_Receive_Loss>();
            try
            {
                if (excelFile == null || excelFile.ContentLength == 0)
                    Message = "Please upload an Excel file";

                if (Message != "Please upload an Excel file")
                {

                    string Date = "";
                    using (var package = new OfficeOpenXml.ExcelPackage(excelFile.InputStream))
                    {
                        ExcelWorksheet ws = package.Workbook.Worksheets[0];

                        int row = 1;

                        int SrNo = 1;

                        // ------------------------------
                        // SECTION 1: Date
                        // ------------------------------

                        Date = ws.Cells[row, 1].Text;

                        // ------------------------------
                        // SECTION 1: Process Loss
                        // ------------------------------
                        row = CommonMethods.FindRow(ws, 1, "Process");
                        row = row + 2; // skip header row
                        SrNo = 1;
                        while (ws.Cells[row, 1].Value != null)
                        {
                            List.Add(new Process_Wise_Issue_Receive_Loss
                            {
                                SrNo = SrNo,
                                Process = ws.Cells[row, 1].Text,
                                IdealCount = ws.Cells[row, 2].Text,
                                IssuePcs = CommonMethods.ToNullableInt(ws.Cells[row, 3].Value),
                                IssueRPartWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 4]),
                                IssuePolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 5]),
                                IssueModel = CommonMethods.ToNullableDecimal(ws.Cells[row, 6]),
                                ReceivePcs = CommonMethods.ToNullableInt(ws.Cells[row, 7].Value),
                                ReceiveRPartWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 8]),
                                ReceivePolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 9]),
                                ReceiveModel = CommonMethods.ToNullableDecimal(ws.Cells[row, 10]),
                                LossRPartWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 11]),
                                LossRPartWeightPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 12]),
                                LossPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 13]),
                                LossPolishWeightPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 14]),
                            });
                            row++;
                            SrNo++;
                        }

                        string xmlStr = "";
                        if (List.Count > 0)
                        {
                            XmlDocument Xmldata = CommonMethods.ConvertToXml(List);
                            xmlStr = "<DocumentElement>" + Xmldata.DocumentElement.InnerXml + "</DocumentElement>";
                        }

                        Message = DbHelper.ExecuteNonQueryWithMessage("Process_Wise_Issue_Receive_Loss_Insert_Using_Excel",
                            new SqlParameter("@ReportDate", Date),
                            new SqlParameter("@XML", xmlStr),
                            new SqlParameter("@UserID", SessionFacade.UserSession.UserID));
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                Message = "ERROR: " + ex.Message;
            }
            return Json(new { Message });
        }

        public JsonResult Pridiction_Details_Insert_Using_Excel(HttpPostedFileBase excelFile)
        {
            ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");

            string Message = "";
            List<Pridiction> List = new List<Pridiction>();
            try
            {
                if (excelFile == null || excelFile.ContentLength == 0)
                    Message = "Please upload an Excel file";

                if (Message != "Please upload an Excel file")
                {

                    DataTable dt = new DataTable();

                    // STRING columns
                    dt.Columns.Add("GradNo", typeof(string));
                    dt.Columns.Add("ColorType", typeof(string));
                    dt.Columns.Add("MainRCode", typeof(string));
                    dt.Columns.Add("RCode", typeof(string));
                    dt.Columns.Add("LotCode", typeof(string));
                    dt.Columns.Add("SubStoneID", typeof(string));
                    dt.Columns.Add("Shape", typeof(string));
                    dt.Columns.Add("Color", typeof(string));
                    dt.Columns.Add("Clarity", typeof(string));
                    dt.Columns.Add("Cut", typeof(string));
                    dt.Columns.Add("TableName", typeof(string));
                    dt.Columns.Add("Receipe", typeof(string));
                    dt.Columns.Add("Girdle", typeof(string));
                    dt.Columns.Add("PriceListName", typeof(string));
                    dt.Columns.Add("SieveSize", typeof(string));
                    dt.Columns.Add("UserName", typeof(string));
                    dt.Columns.Add("Process", typeof(string));
                    dt.Columns.Add("SizeCode", typeof(string));
                    dt.Columns.Add("PlateSize", typeof(decimal));
                    dt.Columns.Add("TagNo", typeof(string));
                    dt.Columns.Add("StoneID", typeof(string));

                    // DECIMAL columns
                    dt.Columns.Add("Labour", typeof(decimal));
                    dt.Columns.Add("Pricing", typeof(decimal));
                    dt.Columns.Add("AWeight", typeof(decimal));
                    dt.Columns.Add("RoughWeight", typeof(decimal));
                    dt.Columns.Add("RPartWeight", typeof(decimal));
                    dt.Columns.Add("PolishWeight", typeof(decimal));
                    dt.Columns.Add("Per", typeof(decimal));
                    dt.Columns.Add("StoneMM", typeof(decimal));
                    dt.Columns.Add("Rap", typeof(decimal));
                    dt.Columns.Add("Discount", typeof(decimal));
                    dt.Columns.Add("Value", typeof(decimal));
                    dt.Columns.Add("Length", typeof(decimal));
                    dt.Columns.Add("Width", typeof(decimal));
                    dt.Columns.Add("Table", typeof(decimal));
                    dt.Columns.Add("CrownAngle", typeof(decimal));
                    dt.Columns.Add("CrownHeight", typeof(decimal));
                    dt.Columns.Add("PavilionAngle", typeof(decimal));
                    dt.Columns.Add("PavilionHeight", typeof(decimal));
                    dt.Columns.Add("TotalHeight", typeof(decimal));
                    dt.Columns.Add("Ratio", typeof(decimal));
                    dt.Columns.Add("Tilt", typeof(decimal));

                    // DATE
                    dt.Columns.Add("Dates", typeof(DateTime));

                    // INT
                    dt.Columns.Add("PridictionID", typeof(int));

                    string Date = "";
                    using (var package = new OfficeOpenXml.ExcelPackage(excelFile.InputStream))
                    {
                        ExcelWorksheet ws = package.Workbook.Worksheets[0];

                        int row = 1;

                        int SrNo = 1;

                        //row = CommonMethods.FindRow(ws, 1, "Process");
                        row = 2; // skip header row
                        while (ws.Cells[row, 1].Value != null)
                        {
                            dt.Rows.Add(
                                ws.Cells[row, 1].Text,    // GradNo
                                ws.Cells[row, 2].Text,    // ColorType
                                ws.Cells[row, 5].Text,    // MainRCode
                                ws.Cells[row, 6].Text,    // RCode
                                ws.Cells[row, 7].Text,    // LotCode
                                ws.Cells[row, 8].Text,    // SubStoneID
                                ws.Cells[row, 16].Text,   // Shape
                                ws.Cells[row, 17].Text,   // Color
                                ws.Cells[row, 18].Text,   // Clarity
                                ws.Cells[row, 19].Text,   // Cut
                                ws.Cells[row, 20].Text,   // TableName
                                ws.Cells[row, 21].Text,   // Receipe
                                ws.Cells[row, 28].Text,   // Girdle
                                ws.Cells[row, 36].Text,   // PriceListName
                                ws.Cells[row, 37].Text,   // SieveSize
                                ws.Cells[row, 38].Text,   // UserName
                                ws.Cells[row, 40].Text,   // Process
                                ws.Cells[row, 45].Text,   // SizeCode
                                CommonMethods.ToNullableDecimal(ws.Cells[row, 41]),  // PlateSize
                                ws.Cells[row, 43].Text,   // TagNo
                                ws.Cells[row, 44].Text,   // StoneID

                                CommonMethods.ToNullableDecimal(ws.Cells[row, 3]),   // Labour
                                CommonMethods.ToNullableDecimal(ws.Cells[row, 4]),   // Pricing
                                CommonMethods.ToNullableDecimal(ws.Cells[row, 9]),   // AWeight
                                CommonMethods.ToNullableDecimal(ws.Cells[row, 10]),  // RoughWeight
                                CommonMethods.ToNullableDecimal(ws.Cells[row, 12]),  // RPartWeight
                                CommonMethods.ToNullableDecimal(ws.Cells[row, 13]),  // PolishWeight
                                CommonMethods.ToNullableDecimal(ws.Cells[row, 14]),  // Per
                                CommonMethods.ToNullableDecimal(ws.Cells[row, 15]),  // StoneMM
                                CommonMethods.ToNullableDecimal(ws.Cells[row, 22]),  // Rap
                                CommonMethods.ToNullableDecimal(ws.Cells[row, 23]),  // Discount
                                CommonMethods.ToNullableDecimal(ws.Cells[row, 24]),  // Value
                                CommonMethods.ToNullableDecimal(ws.Cells[row, 25]),  // Length
                                CommonMethods.ToNullableDecimal(ws.Cells[row, 26]),  // Width
                                CommonMethods.ToNullableDecimal(ws.Cells[row, 27]),  // Table
                                CommonMethods.ToNullableDecimal(ws.Cells[row, 29]),  // CrownAngle
                                CommonMethods.ToNullableDecimal(ws.Cells[row, 30]),  // CrownHeight
                                CommonMethods.ToNullableDecimal(ws.Cells[row, 31]),  // PavilionAngle
                                CommonMethods.ToNullableDecimal(ws.Cells[row, 32]),  // PavilionHeight
                                CommonMethods.ToNullableDecimal(ws.Cells[row, 33]),  // TotalHeight
                                CommonMethods.ToNullableDecimal(ws.Cells[row, 34]),  // Ratio
                                CommonMethods.ToNullableDecimal(ws.Cells[row, 35]),  // Tilt

                                Convert.ToDateTime(ws.Cells[row, 39].Value),                // Dates
                                CommonMethods.ToNullableInt(ws.Cells[row, 42].Value)   // PridictionID
                            );   
                            row++;
                            SrNo++;
                        }

                        
                        SqlParameter tvpParam = new SqlParameter("@TVP", dt)
                        {
                            SqlDbType = SqlDbType.Structured,
                            TypeName = "dbo.Pridiction_TVP"
                        };

                        Message = DbHelper.ExecuteNonQueryWithMessage("Pridiction_Details_Insert_Using_Excel", 60,
                            tvpParam,
                            new SqlParameter("@UserID", SessionFacade.UserSession.UserID));
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                Message = "ERROR: " + ex.Message;
            }
            return Json(new { Message });
        }

        public JsonResult Sawing_Loss_Details_Insert_Using_Excel(HttpPostedFileBase excelFile)
        {
            ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");

            string Message = "";
            List<RP_Sawing_Loss> List = new List<RP_Sawing_Loss>();
            try
            {
                if (excelFile == null || excelFile.ContentLength == 0)
                    Message = "Please upload an Excel file";

                if (Message != "Please upload an Excel file")
                {

                    string Date = "";
                    using (var package = new OfficeOpenXml.ExcelPackage(excelFile.InputStream))
                    {
                        ExcelWorksheet ws = package.Workbook.Worksheets[0];

                        int row = 1;

                        int SrNo = 1;

                        // ------------------------------
                        // SECTION 1: Date
                        // ------------------------------

                        Date = ws.Cells[row, 1].Text;

                        Date = Date.Split('(')[0].Trim();

                        // ------------------------------
                        // SECTION 1: Process Loss
                        // ------------------------------
                        row = CommonMethods.FindRow(ws, 1, "SizeCode Wise Sawing Receive");
                        row = row + 2; // skip header row
                        SrNo = 1;
                        while (ws.Cells[row, 1].Value != null)
                        {
                            List.Add(new RP_Sawing_Loss
                            {
                                SrNo = SrNo,
                                TableNo = ws.Cells[row, 1].Text,
                                SizeCode = ws.Cells[row, 2].Text,
                                MainPcs = CommonMethods.ToNullableInt(ws.Cells[row, 3].Value),
                                IssueRPartWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 4]),
                                ReceiveRPartWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 5]),
                                RLossPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 6]),
                                IssuePolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 7]),
                                ReceivePolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 8]),
                                PLossPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 9]),
                            });
                            row++;
                            SrNo++;
                        }

                        string xmlStr = "";
                        if (List.Count > 0)
                        {
                            XmlDocument Xmldata = CommonMethods.ConvertToXml(List);
                            xmlStr = "<DocumentElement>" + Xmldata.DocumentElement.InnerXml + "</DocumentElement>";
                        }

                        Message = DbHelper.ExecuteNonQueryWithMessage("Sawing_Loss_Summary_Insert",
                            new SqlParameter("@ReportDate", Date),
                            new SqlParameter("@XML", xmlStr),
                            new SqlParameter("@UserID", SessionFacade.UserSession.UserID));
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                Message = "ERROR: " + ex.Message;
            }
            return Json(new { Message });
        }
        //public JsonResult TRN_Ideal_Labour_Upload(HttpPostedFileBase excelFile)
        //{
        //    ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");

        //    string Message = "";
        //    List<TRN_Labour> List = new List<TRN_Labour>();
        //    try
        //    {
        //        if (excelFile == null || excelFile.ContentLength == 0)
        //            Message = "Please upload an Excel file";

        //        if (Message != "Please upload an Excel file")
        //        {

        //            string Date = "";
        //            using (var package = new OfficeOpenXml.ExcelPackage(excelFile.InputStream))
        //            {
        //                ExcelWorksheet ws = package.Workbook.Worksheets[0];

        //                int row = 1;

        //                // SECTION 1: Process Loss
        //                // ------------------------------
        //              // row = CommonMethods.FindRow(ws, 1, "Ideal Labour");
        //                row = row + 1; // skip header row
                        
                        
        //                while (ws.Cells[row, 1].Value != null)
        //                {
        //                    List.Add(new TRN_Labour
        //                    {  
        //                        TLabourID = CommonMethods.ToNullableInt(ws.Cells[row, 1].Value),
        //                        MLabourID = CommonMethods.ToNullableInt(ws.Cells[row, 2].Value),
        //                        ProcessID = CommonMethods.ToNullableInt(ws.Cells[row, 3].Value),
        //                        Process = ws.Cells[row, 4].Text,
        //                        ProcessType = ws.Cells[row, 5].Text,
        //                        Amount = CommonMethods.ToNullableDecimal(ws.Cells[row, 6]),
        //                        Remarks = ws.Cells[row, 7].Text,
        //                        PType = ws.Cells[row, 8].Text,
        //                        LotType = ws.Cells[row, 9].Text,
        //                        ShapeType = ws.Cells[row, 10].Text,
        //                        FromSize = CommonMethods.ToNullableDecimal(ws.Cells[row, 11]),
        //                        ToSize = CommonMethods.ToNullableDecimal(ws.Cells[row, 12]),
        //                        AmtType = ws.Cells[row, 13].Text,
        //                        SizeCodeType = ws.Cells[row, 14].Text,
        //                        FancyShape = ws.Cells[row, 15].Text,

        //                    });
        //                    row++;
                           
        //                }

        //                string xmlStr = "";
        //                if (List.Count > 0)
        //                {
        //                    XmlDocument Xmldata = CommonMethods.ConvertToXml(List);
        //                    xmlStr = "<DocumentElement>" + Xmldata.DocumentElement.InnerXml + "</DocumentElement>";
        //                }

        //                SqlConnection consW = new SqlConnection(conn);
        //                SqlCommand cmdsW = new SqlCommand("TRN_Ideal_Labour_Upload", consW);
        //                cmdsW.CommandType = CommandType.StoredProcedure;
        //                cmdsW.Parameters.AddWithValue("@ReportDate", Date);
        //                cmdsW.Parameters.AddWithValue("@XML", xmlStr);
        //                cmdsW.Parameters.AddWithValue("@UserID", SessionFacade.UserSession.UserID);
        //                cmdsW.Parameters.Add("@MESSAGE", SqlDbType.VarChar, 1000);
        //                cmdsW.Parameters["@MESSAGE"].Direction = ParameterDirection.Output;
        //                consW.Open();

        //                int k = cmdsW.ExecuteNonQuery();
        //                Message = Convert.ToString(cmdsW.Parameters["@MESSAGE"].Value);

        //                consW.Close();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.ErrorLog(ex);
        //        Message = "ERROR: " + ex.Message;
        //    }
        //    return Json(new { Message });
        //}

        public JsonResult TRN_Ideal_Labour_Upload(HttpPostedFileBase excelFile)
        {
            ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");

            string Message = "";
            List<TRN_Labour_V2> List = new List<TRN_Labour_V2>();
            try
            {
                if (excelFile == null || excelFile.ContentLength == 0)
                    Message = "Please upload an Excel file";

                if (Message != "Please upload an Excel file")
                {

                    string Date = "";
                    using (var package = new OfficeOpenXml.ExcelPackage(excelFile.InputStream))
                    {
                        ExcelWorksheet ws = package.Workbook.Worksheets[0];

                        int row = 1;

                        // SECTION 1: Process Loss
                        // ------------------------------
                        // row = CommonMethods.FindRow(ws, 1, "Ideal Labour");
                        row = row + 1; // skip header row


                        while (ws.Cells[row, 1].Value != null)
                        {
                            List.Add(new TRN_Labour_V2
                            {
                                LID = CommonMethods.ToNullableInt(ws.Cells[row, 1].Value),
                                Process = ws.Cells[row, 2].Text,
                                SubProcess = ws.Cells[row, 3].Text,
                                Shape = ws.Cells[row, 4].Text,
                                FromSize = CommonMethods.ToNullableDecimal(ws.Cells[row, 5]),
                                ToSize = CommonMethods.ToNullableDecimal(ws.Cells[row, 6]),
                                Cut = ws.Cells[row, 7].Text,
                                ColorType = ws.Cells[row, 8].Text,
                                PcsWeight = ws.Cells[row, 9].Text,
                                IssueReceive = ws.Cells[row, 10].Text,
                                ManualAmt = CommonMethods.ToNullableDecimal(ws.Cells[row, 11]),
                                AutoAmt = CommonMethods.ToNullableDecimal(ws.Cells[row, 12]),
                                UseAmt = CommonMethods.ToNullableDecimal(ws.Cells[row, 13]),
                                Remarks = ws.Cells[row, 14].Text,

                            });
                            row++;

                        }

                        string xmlStr = "";
                        if (List.Count > 0)
                        {
                            XmlDocument Xmldata = CommonMethods.ConvertToXml(List);
                            xmlStr = "<DocumentElement>" + Xmldata.DocumentElement.InnerXml + "</DocumentElement>";
                        }

                        Message = DbHelper.ExecuteNonQueryWithMessage("TRN_Ideal_Labour_Upload",
                            new SqlParameter("@ReportDate", Date),
                            new SqlParameter("@XML", xmlStr),
                            new SqlParameter("@UserID", SessionFacade.UserSession.UserID));
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                Message = "ERROR: " + ex.Message;
            }
            return Json(new { Message });
        }

        public ActionResult AddReportsJumbo()
        {
            return View();
        }

        public JsonResult Jumbo_Date_Wise_Loss_Summary_Insert_Using_Excel(HttpPostedFileBase excelFile)
        {
            ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");

            string Message = "";
            List<After4POk_Loss> List = new List<After4POk_Loss>();
            try
            {
                if (excelFile == null || excelFile.ContentLength == 0)
                    Message = "Please upload an Excel file";

                if (Message != "Please upload an Excel file")
                {

                    string Date = "";
                    using (var package = new OfficeOpenXml.ExcelPackage(excelFile.InputStream))
                    {
                        ExcelWorksheet ws = package.Workbook.Worksheets[0];

                        int row = 1;

                        int SrNo = 1;

                        // ------------------------------
                        // SECTION 1: Date
                        // ------------------------------

                        Date = ws.Cells[row, 1].Text;

                        // ------------------------------
                        // SECTION 1: Process Loss
                        // ------------------------------
                        row = CommonMethods.FindRow(ws, 1, "Process");
                        row += 2; // skip header row
                        SrNo = 1;
                        while (ws.Cells[row, 1].Value != null)
                        {
                            List.Add(new After4POk_Loss
                            {
                                SrNo = SrNo,
                                Process = ws.Cells[row, 1].Text,
                                IPcs = CommonMethods.ToNullableInt(ws.Cells[row, 2].Value),
                                IRPartWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 3]),
                                IPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 4]),
                                IModel = CommonMethods.ToNullableDecimal(ws.Cells[row, 5]),

                                RPcs = CommonMethods.ToNullableInt(ws.Cells[row, 6].Value),
                                RRPartWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 7]),
                                RPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 8]),
                                RModel = CommonMethods.ToNullableDecimal(ws.Cells[row, 9]),

                                DRPartWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 10]),
                                DiffRPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 11]),
                                DPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 12]),
                                DiffPPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 13]),

                                TPcs = CommonMethods.ToNullableInt(ws.Cells[row, 14].Value),
                                TRPartWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 15]),
                                TPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 16]),
                            });
                            row++;
                            SrNo++;
                        }
                    }
                    // Now you have all parsed lists:
                    // jumboList, smallList, receiveList, detailList

                    string xmlStr = "";
                    if (List.Count > 0)
                    {
                        XmlDocument pXmldata = CommonMethods.ConvertToXml(List);
                        xmlStr = "<DocumentElement>" + pXmldata.DocumentElement.InnerXml + "</DocumentElement>";
                    }
                    Message = DbHelper.ExecuteNonQueryWithMessage("Jumbo_Date_Wise_Loss_Summary_Insert_Using_Excel",
                        new SqlParameter("@ReportDate", Date),
                        new SqlParameter("@XML", xmlStr),
                        new SqlParameter("@UserID", SessionFacade.UserSession.UserID));
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                Message = "ERROR: " + ex.Message;
            }
            return Json(new { Message });
        }

        public JsonResult Kapan_Wise_Cleaving_Report_Insert_Using_Excel(HttpPostedFileBase excelFile)
        {
            ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");

            string Message = "";
            List<CleavingSummary> List = new List<CleavingSummary>();
            List<CleavingDetails> List2 = new List<CleavingDetails>();
            try
            {
                if (excelFile == null || excelFile.ContentLength == 0)
                    Message = "Please upload an Excel file";

                if (Message != "Please upload an Excel file")
                {

                    string Date = "";
                    using (var package = new OfficeOpenXml.ExcelPackage(excelFile.InputStream))
                    {
                        ExcelWorksheet ws = package.Workbook.Worksheets[0];

                        int row = 1;

                        int SrNo = 1;

                        // ------------------------------
                        // SECTION 1: Date
                        // ------------------------------

                        Date = ws.Cells[row, 1].Text;

                        // ------------------------------
                        // SECTION 2: Cleaving Summary
                        // ------------------------------
                        row = CommonMethods.FindRow(ws, 1, "Date");
                        if (row > 0)
                        {
                            row += 1; // skip header row
                            SrNo = 1;
                            while (ws.Cells[row, 2].Text != "TOTAL")
                            {
                                List.Add(new CleavingSummary
                                {
                                    SrNo = SrNo,
                                    Status = "Cleaving Summary",
                                    RDate = ws.Cells[row, 1].Text,
                                    SizeCode = ws.Cells[row, 2].Text,
                                    Pcs = CommonMethods.ToNullableInt(ws.Cells[row, 3].Value),
                                    Weight = CommonMethods.ToNullableDecimal(ws.Cells[row, 4]),
                                });
                                row++;
                                SrNo++;
                            }
                        }

                        // ------------------------------
                        // SECTION 3: Cleaving Details (RUNNING)
                        // ------------------------------
                        row = CommonMethods.FindRow(ws, 1, "Kapan Wise Cleaving Report (RUNNING)");
                        if (row > 0)
                        {
                            row += 4; // skip header row
                            SrNo = 1;
                            while (ws.Cells[row, 1].Value != null)
                            {
                                List2.Add(new CleavingDetails
                                {
                                    SrNo = SrNo,
                                    Status = "Cleaving Details (RUNNING)",
                                    MainRDate = ws.Cells[row, 1].Text,
                                    MainRCode = ws.Cells[row, 2].Text,
                                    MainReceipe = ws.Cells[row, 3].Text,
                                    MainPcs = CommonMethods.ToNullableInt(ws.Cells[row, 4].Value),
                                    MainCarat = CommonMethods.ToNullableDecimal(ws.Cells[row, 5]),
                                    RDate = ws.Cells[row, 6].Text,
                                    Kapan = ws.Cells[row, 7].Text,
                                    TableNo = ws.Cells[row, 8].Text,
                                    HeightName = ws.Cells[row, 9].Text,
                                    GradNo = ws.Cells[row, 10].Text,
                                    RoughPcs = CommonMethods.ToNullableInt(ws.Cells[row, 11].Value),
                                    RoughCarat = CommonMethods.ToNullableDecimal(ws.Cells[row, 12]),
                                    PrdCTUPPcs = CommonMethods.ToNullableInt(ws.Cells[row, 13].Value),
                                    PrdCTUPPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 14]),
                                    PrdCTUPPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 15]),
                                    PrdCTUPPrdAmt = CommonMethods.ToNullableDecimal(ws.Cells[row, 16]),
                                    PrdCTUPPrcAmt = CommonMethods.ToNullableDecimal(ws.Cells[row, 17]),
                                    PrdCTDNPcs = CommonMethods.ToNullableInt(ws.Cells[row, 18].Value),
                                    PrdCTDNPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 19]),
                                    PrdCTDNPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 20]),
                                    PrdCTDNPrdAmt = CommonMethods.ToNullableDecimal(ws.Cells[row, 21]),
                                    PrdCTDNPrcAmt = CommonMethods.ToNullableDecimal(ws.Cells[row, 22]),
                                    PrdPcs = CommonMethods.ToNullableInt(ws.Cells[row, 23].Value),
                                    PrdPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 24]),
                                    PrdPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 25]),
                                    PrdAmt = CommonMethods.ToNullableDecimal(ws.Cells[row, 26]),
                                    PrcAmt = CommonMethods.ToNullableDecimal(ws.Cells[row, 27]),
                                    PrdDays = CommonMethods.ToNullableInt(ws.Cells[row, 28].Value),
                                    ClvCTUPPcs = CommonMethods.ToNullableInt(ws.Cells[row, 29].Value),
                                    ClvCTUPPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 30]),
                                    ClvCTUPPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 31]),
                                    ClvCTDNPcs = CommonMethods.ToNullableInt(ws.Cells[row, 32].Value),
                                    ClvCTDNPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 33]),
                                    ClvCTDNPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 34]),
                                    ClvPcs = CommonMethods.ToNullableInt(ws.Cells[row, 35].Value),
                                    ClvPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 36]),
                                    ClvPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 37]),
                                    ComplateDate = ws.Cells[row, 38].Text,
                                    ClvDays = CommonMethods.ToNullableInt(ws.Cells[row, 39].Value),
                                    BagID = CommonMethods.ToNullableInt(ws.Cells[row, 40].Value),
                                    BagNo = ws.Cells[row, 41].Text,
                                    AWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 42]),
                                });
                                row++;
                                SrNo++;
                            }
                        }
                        // ------------------------------
                        // SECTION 3: Cleaving Details (COMPLETE)
                        // ------------------------------
                        row = CommonMethods.FindRow(ws, 1, "Kapan Wise Cleaving Report (COMPLETE)");
                        if (row > 0)
                        {
                            row += 4; // skip header row
                            SrNo = 1;
                            while (ws.Cells[row, 1].Value != null)
                            {
                                List2.Add(new CleavingDetails
                                {
                                    SrNo = SrNo,
                                    Status = "Cleaving Details (COMPLETE)",
                                    MainRDate = ws.Cells[row, 1].Text,
                                    MainRCode = ws.Cells[row, 2].Text,
                                    MainReceipe = ws.Cells[row, 3].Text,
                                    MainPcs = CommonMethods.ToNullableInt(ws.Cells[row, 4].Value),
                                    MainCarat = CommonMethods.ToNullableDecimal(ws.Cells[row, 5]),
                                    RDate = ws.Cells[row, 6].Text,
                                    Kapan = ws.Cells[row, 7].Text,
                                    TableNo = ws.Cells[row, 8].Text,
                                    HeightName = ws.Cells[row, 9].Text,
                                    GradNo = ws.Cells[row, 10].Text,
                                    RoughPcs = CommonMethods.ToNullableInt(ws.Cells[row, 11].Value),
                                    RoughCarat = CommonMethods.ToNullableDecimal(ws.Cells[row, 12]),
                                    PrdCTUPPcs = CommonMethods.ToNullableInt(ws.Cells[row, 13].Value),
                                    PrdCTUPPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 14]),
                                    PrdCTUPPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 15]),
                                    PrdCTUPPrdAmt = CommonMethods.ToNullableDecimal(ws.Cells[row, 16]),
                                    PrdCTUPPrcAmt = CommonMethods.ToNullableDecimal(ws.Cells[row, 17]),
                                    PrdCTDNPcs = CommonMethods.ToNullableInt(ws.Cells[row, 18].Value),
                                    PrdCTDNPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 19]),
                                    PrdCTDNPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 20]),
                                    PrdCTDNPrdAmt = CommonMethods.ToNullableDecimal(ws.Cells[row, 21]),
                                    PrdCTDNPrcAmt = CommonMethods.ToNullableDecimal(ws.Cells[row, 22]),
                                    PrdPcs = CommonMethods.ToNullableInt(ws.Cells[row, 23].Value),
                                    PrdPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 24]),
                                    PrdPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 25]),
                                    PrdAmt = CommonMethods.ToNullableDecimal(ws.Cells[row, 26]),
                                    PrcAmt = CommonMethods.ToNullableDecimal(ws.Cells[row, 27]),
                                    PrdDays = CommonMethods.ToNullableInt(ws.Cells[row, 28].Value),
                                    ClvCTUPPcs = CommonMethods.ToNullableInt(ws.Cells[row, 29].Value),
                                    ClvCTUPPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 30]),
                                    ClvCTUPPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 31]),
                                    ClvCTDNPcs = CommonMethods.ToNullableInt(ws.Cells[row, 32].Value),
                                    ClvCTDNPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 33]),
                                    ClvCTDNPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 34]),
                                    ClvPcs = CommonMethods.ToNullableInt(ws.Cells[row, 35].Value),
                                    ClvPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 36]),
                                    ClvPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 37]),
                                    ComplateDate = ws.Cells[row, 38].Text,
                                    ClvDays = CommonMethods.ToNullableInt(ws.Cells[row, 39].Value),
                                    BagID = CommonMethods.ToNullableInt(ws.Cells[row, 40].Value),
                                    BagNo = ws.Cells[row, 41].Text,
                                    AWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 42]),
                                });
                                row++;
                                SrNo++;
                            }
                        }
                    }

                    string xmlStr = "";
                    if (List.Count > 0)
                    {
                        XmlDocument pXmldata = CommonMethods.ConvertToXml(List);
                        xmlStr = "<DocumentElement>" + pXmldata.DocumentElement.InnerXml + "</DocumentElement>";
                    }
                    string xmlDetails = "";
                    if (List2.Count > 0)
                    {
                        XmlDocument pXmldata2 = CommonMethods.ConvertToXml(List2);
                        xmlDetails = "<DocumentElement>" + pXmldata2.DocumentElement.InnerXml + "</DocumentElement>";
                    }

                    Message = DbHelper.ExecuteNonQueryWithMessage("Kapan_Wise_Cleaving_Report_Insert_Using_Excel",
                        new SqlParameter("@ReportDate", Date),
                        new SqlParameter("@XML", xmlStr),
                        new SqlParameter("@XML_Details", xmlDetails),
                        new SqlParameter("@UserID", SessionFacade.UserSession.UserID));
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                Message = "ERROR: " + ex.Message;
            }
            return Json(new { Message });
        }

        public JsonResult Kapan_Wise_DST_Report_Insert_Using_Excel(HttpPostedFileBase excelFile)
        {
            ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");

            string Message = "";
            List<DSTDetails> List = new List<DSTDetails>();
            try
            {
                if (excelFile == null || excelFile.ContentLength == 0)
                    Message = "Please upload an Excel file";

                if (Message != "Please upload an Excel file")
                {

                    string Date = "";
                    using (var package = new OfficeOpenXml.ExcelPackage(excelFile.InputStream))
                    {
                        ExcelWorksheet ws = package.Workbook.Worksheets[0];

                        int row = 1;

                        int SrNo = 1;

                        // ------------------------------
                        // SECTION 1: Date
                        // ------------------------------

                        Date = ws.Cells[row, 1].Text;

                        // ------------------------------
                        // SECTION 3: Cleaving Details (RUNNING)
                        // ------------------------------
                        row = CommonMethods.FindRow(ws, 1, "Kapan Wise DST Report (RUNNING)");
                        if(row > 0)
                        {
                            row += 4; // skip header row
                            SrNo = 1;
                            while (ws.Cells[row, 1].Value != null)
                            {
                                List.Add(new DSTDetails
                                {
                                    SrNo = SrNo,
                                    Status = "DST Details (RUNNING)",
                                    MainRDate = ws.Cells[row, 1].Text,
                                    MainRCode = ws.Cells[row, 2].Text,
                                    MainReceipe = ws.Cells[row, 3].Text,
                                    MainPcs = CommonMethods.ToNullableInt(ws.Cells[row, 4].Value),
                                    MainCarat = CommonMethods.ToNullableDecimal(ws.Cells[row, 5]),
                                    RDate = ws.Cells[row, 6].Text,
                                    Kapan = ws.Cells[row, 7].Text,
                                    TableNo = ws.Cells[row, 8].Text,
                                    HeightName = ws.Cells[row, 9].Text,
                                    GradNo = ws.Cells[row, 10].Text,
                                    RoughPcs = CommonMethods.ToNullableInt(ws.Cells[row, 11].Value),
                                    RoughCarat = CommonMethods.ToNullableDecimal(ws.Cells[row, 12]),
                                    ClvCTUPPcs = CommonMethods.ToNullableInt(ws.Cells[row, 13].Value),
                                    ClvCTUPPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 14]),
                                    ClvCTUPPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 15]),
                                    ClvCTDNPcs = CommonMethods.ToNullableInt(ws.Cells[row, 16].Value),
                                    ClvCTDNPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 17]),
                                    ClvCTDNPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 18]),
                                    ClvPcs = CommonMethods.ToNullableInt(ws.Cells[row, 19].Value),
                                    ClvPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 20]),
                                    ClvPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 21]),
                                    ClvDays = CommonMethods.ToNullableInt(ws.Cells[row, 22].Value),
                                    DSTCTUPPcs = CommonMethods.ToNullableInt(ws.Cells[row, 23].Value),
                                    DSTCTUPPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 24]),
                                    DSTCTUPPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 25]),
                                    DSTCTDNPcs = CommonMethods.ToNullableInt(ws.Cells[row, 26].Value),
                                    DSTCTDNPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 27]),
                                    DSTCTDNPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 28]),
                                    DSTPcs = CommonMethods.ToNullableInt(ws.Cells[row, 29].Value),
                                    DSTPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 30]),
                                    DSTPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 31]),
                                    ComplateDate = ws.Cells[row, 32].Text,
                                    DSTDays = CommonMethods.ToNullableInt(ws.Cells[row, 33].Value),
                                    BagID = CommonMethods.ToNullableInt(ws.Cells[row, 34].Value),
                                    BagNo = ws.Cells[row, 35].Text,
                                    AWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 36]),
                                });
                                row++;
                                SrNo++;
                            }
                        }
                        // ------------------------------
                        // SECTION 3: Cleaving Details (COMPLETE)
                        // ------------------------------
                        row = CommonMethods.FindRow(ws, 1, "Kapan Wise DST Report (COMPLETE)");
                        if(row > 0) {
                            row += 4; // skip header row
                            SrNo = 1;
                            while (ws.Cells[row, 1].Value != null)
                            {
                                List.Add(new DSTDetails
                                {
                                    SrNo = SrNo,
                                    Status = "DST Details (COMPLETE)",
                                    MainRDate = ws.Cells[row, 1].Text,
                                    MainRCode = ws.Cells[row, 2].Text,
                                    MainReceipe = ws.Cells[row, 3].Text,
                                    MainPcs = CommonMethods.ToNullableInt(ws.Cells[row, 4].Value),
                                    MainCarat = CommonMethods.ToNullableDecimal(ws.Cells[row, 5]),
                                    RDate = ws.Cells[row, 6].Text,
                                    Kapan = ws.Cells[row, 7].Text,
                                    TableNo = ws.Cells[row, 8].Text,
                                    HeightName = ws.Cells[row, 9].Text,
                                    GradNo = ws.Cells[row, 10].Text,
                                    RoughPcs = CommonMethods.ToNullableInt(ws.Cells[row, 11].Value),
                                    RoughCarat = CommonMethods.ToNullableDecimal(ws.Cells[row, 12]),
                                    ClvCTUPPcs = CommonMethods.ToNullableInt(ws.Cells[row, 13].Value),
                                    ClvCTUPPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 14]),
                                    ClvCTUPPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 15]),
                                    ClvCTDNPcs = CommonMethods.ToNullableInt(ws.Cells[row, 16].Value),
                                    ClvCTDNPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 17]),
                                    ClvCTDNPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 18]),
                                    ClvPcs = CommonMethods.ToNullableInt(ws.Cells[row, 19].Value),
                                    ClvPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 20]),
                                    ClvPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 21]),
                                    ClvDays = CommonMethods.ToNullableInt(ws.Cells[row, 22].Value),
                                    DSTCTUPPcs = CommonMethods.ToNullableInt(ws.Cells[row, 23].Value),
                                    DSTCTUPPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 24]),
                                    DSTCTUPPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 25]),
                                    DSTCTDNPcs = CommonMethods.ToNullableInt(ws.Cells[row, 26].Value),
                                    DSTCTDNPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 27]),
                                    DSTCTDNPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 28]),
                                    DSTPcs = CommonMethods.ToNullableInt(ws.Cells[row, 29].Value),
                                    DSTPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 30]),
                                    DSTPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 31]),
                                    ComplateDate = ws.Cells[row, 32].Text,
                                    DSTDays = CommonMethods.ToNullableInt(ws.Cells[row, 33].Value),
                                    BagID = CommonMethods.ToNullableInt(ws.Cells[row, 34].Value),
                                    BagNo = ws.Cells[row, 35].Text,
                                    AWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 36]),
                                });
                                row++;
                                SrNo++;
                            }
                        }
                    }

                    string xmlStr = "";
                    if (List.Count > 0)
                    {
                        XmlDocument pXmldata = CommonMethods.ConvertToXml(List);
                        xmlStr = "<DocumentElement>" + pXmldata.DocumentElement.InnerXml + "</DocumentElement>";
                    }

                    Message = DbHelper.ExecuteNonQueryWithMessage("Kapan_Wise_DST_Report_Insert_Using_Excel",
                        new SqlParameter("@ReportDate", Date),
                        new SqlParameter("@XML", xmlStr),
                        new SqlParameter("@UserID", SessionFacade.UserSession.UserID));
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                Message = "ERROR: " + ex.Message;
            }
            return Json(new { Message });
        }

        public JsonResult Kapan_Wise_MFG_Report_Insert_Using_Excel(HttpPostedFileBase excelFile)
        {
            ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");

            string Message = "";
            List<MFGDetails> List = new List<MFGDetails>();
            try
            {
                if (excelFile == null || excelFile.ContentLength == 0)
                    Message = "Please upload an Excel file";

                if (Message != "Please upload an Excel file")
                {

                    string Date = "";
                    using (var package = new OfficeOpenXml.ExcelPackage(excelFile.InputStream))
                    {
                        ExcelWorksheet ws = package.Workbook.Worksheets[0];

                        int row = 1;

                        int SrNo = 1;

                        // ------------------------------
                        // SECTION 1: Date
                        // ------------------------------

                        Date = ws.Cells[row, 1].Text;

                        // ------------------------------
                        // SECTION 3: Cleaving Details (RUNNING)
                        // ------------------------------
                        row = CommonMethods.FindRow(ws, 1, "Kapan Wise MFG Report (RUNNING)");
                        if (row > 0)
                        {
                            row += 4; // skip header row
                            SrNo = 1;
                            while (ws.Cells[row, 1].Value != null)
                            {
                                List.Add(new MFGDetails
                                {
                                    SrNo = SrNo,
                                    Status = "MFG Details (RUNNING)",
                                    MainRDate = ws.Cells[row, 1].Text,
                                    MainRCode = ws.Cells[row, 2].Text,
                                    MainReceipe = ws.Cells[row, 3].Text,
                                    MainPcs = CommonMethods.ToNullableInt(ws.Cells[row, 4].Value),
                                    MainCarat = CommonMethods.ToNullableDecimal(ws.Cells[row, 5]),
                                    RDate = ws.Cells[row, 6].Text,
                                    Kapan = ws.Cells[row, 7].Text,
                                    TableNo = ws.Cells[row, 8].Text,
                                    HeightName = ws.Cells[row, 9].Text,
                                    GradNo = ws.Cells[row, 10].Text,
                                    RoughPcs = CommonMethods.ToNullableInt(ws.Cells[row, 11].Value),
                                    RoughCarat = CommonMethods.ToNullableDecimal(ws.Cells[row, 12]),
                                    DSTCTUPPcs = CommonMethods.ToNullableInt(ws.Cells[row, 13].Value),
                                    DSTCTUPPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 14]),
                                    DSTCTUPPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 15]),
                                    DSTCTDNPcs = CommonMethods.ToNullableInt(ws.Cells[row, 16].Value),
                                    DSTCTDNPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 17]),
                                    DSTCTDNPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 18]),
                                    DSTPcs = CommonMethods.ToNullableInt(ws.Cells[row, 19].Value),
                                    DSTPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 20]),
                                    DSTPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 21]),
                                    DSTDays = CommonMethods.ToNullableInt(ws.Cells[row, 22].Value),
                                    MFGCTUPPcs = CommonMethods.ToNullableInt(ws.Cells[row, 23].Value),
                                    MFGCTUPPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 24]),
                                    MFGCTUPPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 25]),
                                    MFGCTDNPcs = CommonMethods.ToNullableInt(ws.Cells[row, 26].Value),
                                    MFGCTDNPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 27]),
                                    MFGCTDNPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 28]),
                                    MFGPcs = CommonMethods.ToNullableInt(ws.Cells[row, 29].Value),
                                    MFGPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 30]),
                                    MFGPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 31]),
                                    ComplateDate = ws.Cells[row, 32].Text,
                                    MFGDays = CommonMethods.ToNullableInt(ws.Cells[row, 33].Value),
                                    BagID = CommonMethods.ToNullableInt(ws.Cells[row, 34].Value),
                                    BagNo = ws.Cells[row, 35].Text,
                                    AWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 36]),
                                });
                                row++;
                                SrNo++;
                            }
                        }
                        // ------------------------------
                        // SECTION 3: Cleaving Details (COMPLETE)
                        // ------------------------------
                        row = CommonMethods.FindRow(ws, 1, "Kapan Wise MFG Report (COMPLETE)");
                        if (row > 0)
                        {
                            row += 4; // skip header row
                            SrNo = 1;
                            while (ws.Cells[row, 1].Value != null)
                            {
                                List.Add(new MFGDetails
                                {
                                    SrNo = SrNo,
                                    Status = "MFG Details (COMPLETE)",
                                    MainRDate = ws.Cells[row, 1].Text,
                                    MainRCode = ws.Cells[row, 2].Text,
                                    MainReceipe = ws.Cells[row, 3].Text,
                                    MainPcs = CommonMethods.ToNullableInt(ws.Cells[row, 4].Value),
                                    MainCarat = CommonMethods.ToNullableDecimal(ws.Cells[row, 5]),
                                    RDate = ws.Cells[row, 6].Text,
                                    Kapan = ws.Cells[row, 7].Text,
                                    TableNo = ws.Cells[row, 8].Text,
                                    HeightName = ws.Cells[row, 9].Text,
                                    GradNo = ws.Cells[row, 10].Text,
                                    RoughPcs = CommonMethods.ToNullableInt(ws.Cells[row, 11].Value),
                                    RoughCarat = CommonMethods.ToNullableDecimal(ws.Cells[row, 12]),
                                    DSTCTUPPcs = CommonMethods.ToNullableInt(ws.Cells[row, 13].Value),
                                    DSTCTUPPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 14]),
                                    DSTCTUPPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 15]),
                                    DSTCTDNPcs = CommonMethods.ToNullableInt(ws.Cells[row, 16].Value),
                                    DSTCTDNPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 17]),
                                    DSTCTDNPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 18]),
                                    DSTPcs = CommonMethods.ToNullableInt(ws.Cells[row, 19].Value),
                                    DSTPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 20]),
                                    DSTPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 21]),
                                    DSTDays = CommonMethods.ToNullableInt(ws.Cells[row, 22].Value),
                                    MFGCTUPPcs = CommonMethods.ToNullableInt(ws.Cells[row, 23].Value),
                                    MFGCTUPPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 24]),
                                    MFGCTUPPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 25]),
                                    MFGCTDNPcs = CommonMethods.ToNullableInt(ws.Cells[row, 26].Value),
                                    MFGCTDNPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 27]),
                                    MFGCTDNPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 28]),
                                    MFGPcs = CommonMethods.ToNullableInt(ws.Cells[row, 29].Value),
                                    MFGPolishWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 30]),
                                    MFGPer = CommonMethods.ToNullableDecimal(ws.Cells[row, 31]),
                                    ComplateDate = ws.Cells[row, 32].Text,
                                    MFGDays = CommonMethods.ToNullableInt(ws.Cells[row, 33].Value),
                                    BagID = CommonMethods.ToNullableInt(ws.Cells[row, 34].Value),
                                    BagNo = ws.Cells[row, 35].Text,
                                    AWeight = CommonMethods.ToNullableDecimal(ws.Cells[row, 36]),
                                });
                                row++;
                                SrNo++;
                            }
                        }
                    }

                    string xmlStr = "";
                    if (List.Count > 0)
                    {
                        XmlDocument pXmldata = CommonMethods.ConvertToXml(List);
                        xmlStr = "<DocumentElement>" + pXmldata.DocumentElement.InnerXml + "</DocumentElement>";
                    }

                    Message = DbHelper.ExecuteNonQueryWithMessage("Kapan_Wise_MFG_Report_Insert_Using_Excel",
                        new SqlParameter("@ReportDate", Date),
                        new SqlParameter("@XML", xmlStr),
                        new SqlParameter("@UserID", SessionFacade.UserSession.UserID));
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                Message = "ERROR: " + ex.Message;
            }
            return Json(new { Message });
        }
    }
}