using ExcelDataReader.Log;
using Newtonsoft.Json;
using Slip.Models;
using Slip.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using static Slip.Models.Filter.SessionExpireFilter;

namespace Slip.Controllers
{
    [SessionExpireFilterAttribute]
    public class ReportController : BaseController
    {
        public ActionResult PredictionReport()
        {
            return View();
        }
        public JsonResult RP_Pridiction_Getdata(string RCode, string FromDate, string ToDate)
        {
            try
            {
                List<object> _list_Slip = new List<object>();

                DataSet _DropDownList = DbHelper.ExecuteDataSet("RP_Pridiction_Getdata",
                    new SqlParameter("@RCode", RCode),
                    new SqlParameter("@FromDate", FromDate),
                    new SqlParameter("@ToDate", ToDate));

                if (_DropDownList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[0].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[0].Columns[j].ToString(), _DropDownList.Tables[0].Rows[i][j].ToString());
                        }
                        _list_Slip.Add(Values);
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

                var DataFieldSummary = _DropDownList.Tables[2].AsEnumerable()
                .Select(row => new DataFields
                {
                    column = row.Field<string>("column"),
                    valueFormat = row.Field<string>("valueFormat"),
                    summaryType = row.Field<string>("summaryType"),
                    displayFormat = row.Field<string>("displayFormat")
                })
                .ToList();

                var jsonResult = Json(new
                {
                    List = _list_Slip,
                    DataField = DataField,
                    DataFieldSummary = DataFieldSummary
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
        public JsonResult RP_Prd_Summary_Export(string RCode, string FromDate, string ToDate)
        {
            try
            {
                string Data = "";
                string headername = "Lot Wise Prd. With Labour";

                DataSet _DropDownList = DbHelper.ExecuteDataSet("RP_Prd_Summary_Getdata",
                    new SqlParameter("@RCode", RCode),
                    new SqlParameter("@FromDate", FromDate),
                    new SqlParameter("@ToDate", ToDate));

                string Tital = "";

                if (RCode != null)
                {
                    Tital = RCode;
                }
                else {
                    Tital = FromDate + " To " + ToDate;
                }

                var List = _DropDownList.Tables[0].AsEnumerable().AsEnumerable()
                .Select(row => new Rp_Prd_Summary
                {
                    OnePcs = row.Field<int?>("OnePcs"),
                    TwoPcs = row.Field<int?>("TwoPcs"),
                    ThreePcs = row.Field<int?>("ThreePcs"),
                    FourPcs = row.Field<int?>("FourPcs"),
                    FivePcs = row.Field<int?>("FivePcs"),
                    SixPcs = row.Field<int?>("SixPcs"),
                    SevenPcs = row.Field<int?>("SevenPcs"),

                    OneWT = row.Field<decimal?>("OneWT"),
                    TwoWT = row.Field<decimal?>("TwoWT"),
                    ThreeWT = row.Field<decimal?>("ThreeWT"),
                    FourWT = row.Field<decimal?>("FourWT"),
                    FiveWT = row.Field<decimal?>("FiveWT"),
                    SixWT = row.Field<decimal?>("SixWT"),
                    SevenWT = row.Field<decimal?>("SevenWT"),
                })
                .ToList();

                var List1 = _DropDownList.Tables[1].AsEnumerable().AsEnumerable()
                .Select(row => new Rp_Prd_Summary
                {

                    GradNo = row.Field<string>("GradNo"),
                    Pcs = row.Field<int?>("Pcs"),
                    Weight = row.Field<decimal?>("Weight"),
                    Per = row.Field<decimal?>("Per"),
                })
                .ToList();


                var List2 = _DropDownList.Tables[2].AsEnumerable().AsEnumerable()
                .Select(row => new Rp_Prd_Summary
                {
                    SType = row.Field<string>("SType"),
                    GradNo = row.Field<string>("GradNo"),
                    Pcs = row.Field<int?>("Pcs"),
                    Weight = row.Field<decimal?>("Weight"),
                    Per = row.Field<decimal?>("Per"),
                })
                .ToList();

                var List3 = _DropDownList.Tables[3].AsEnumerable().AsEnumerable()
                .Select(row => new Rp_Prd_Summary
                {
                    SType = row.Field<string>("SType"),
                    Pcs = row.Field<int?>("Pcs"),
                    Weight = row.Field<decimal?>("Weight"),
                    Per = row.Field<decimal?>("Per"),
                })
                .ToList();

                var List4 = _DropDownList.Tables[4].AsEnumerable().AsEnumerable()
                .Select(row => new Rp_Prd_Summary
                {
                    GradNo = row.Field<string>("GradNo"),
                    Pcs = row.Field<int?>("Pcs"),
                    Weight = row.Field<decimal?>("Weight"),
                    Per = row.Field<decimal?>("Per"),
                    PerCTValue = row.Field<decimal?>("PerCTValue"),
                })
                .ToList();

                var List5 = _DropDownList.Tables[5].AsEnumerable().AsEnumerable()
                .Select(row => new Rp_Prd_Summary
                {
                    SType = row.Field<string>("SType"),
                    GradNo = row.Field<string>("GradNo"),
                    Pcs = row.Field<int?>("Pcs"),
                    Weight = row.Field<decimal?>("Weight"),
                    Per = row.Field<decimal?>("Per"),
                    PerCTValue = row.Field<decimal?>("PerCTValue"),
                })
                .ToList();

                var List6 = _DropDownList.Tables[6].AsEnumerable().AsEnumerable()
                .Select(row => new Rp_Prd_Summary
                {
                    SType = row.Field<string>("SType"),
                    Pcs = row.Field<int?>("Pcs"),
                    Weight = row.Field<decimal?>("Weight"),
                    Per = row.Field<decimal?>("Per"),
                })
                .ToList();

                var List7 = _DropDownList.Tables[7].AsEnumerable().AsEnumerable()
                .Select(row => new Rp_Prd_Summary
                {
                    SType = row.Field<string>("SType"),
                })
                .ToList();

                var List8 = _DropDownList.Tables[8].AsEnumerable().AsEnumerable()
                .Select(row => new Rp_Prd_Summary
                {
                    Shape = row.Field<string>("Shape"),
                    Pcs = row.Field<int?>("Pcs"),
                    PrdAmt = row.Field<decimal?>("PrdAmt"),
                    PrcAmt = row.Field<decimal?>("PrcAmt"),
                    Pweight = row.Field<decimal?>("Pweight"),
                    Per = Convert.ToDecimal(row.Field<string>("Per")),
                })
                .ToList();

                var List9 = _DropDownList.Tables[9].AsEnumerable().AsEnumerable()
                .Select(row => new Rp_Prd_Summary
                {
                    Shape = "TOTAL",
                    Pcs = row.Field<int?>("Pcs"),
                    PrdAmt = row.Field<decimal?>("PrdAmt"),
                    PrcAmt = row.Field<decimal?>("PrcAmt"),
                    Pweight = row.Field<decimal?>("Pweight"),
                    Per = Convert.ToDecimal(row.Field<string>("Per")),
                })
                .ToList();

                var List10 = _DropDownList.Tables[10].AsEnumerable().AsEnumerable()
                .Select(row => new Rp_Prd_Summary
                {
                    Pcs = row.Field<int?>("Pcs"),
                    RoughWeight = row.Field<decimal?>("RoughWeight"),
                    Receipe = row.Field<string>("Receipe"),
                })
                .ToList();

                var List11 = _DropDownList.Tables[11].AsEnumerable().AsEnumerable()
                .Select(row => new Rp_Prd_Summary
                {
                    SizeCode = row.Field<string>("SizeCode"),
                    MinPlateSize = row.Field<decimal?>("MinPlateSize"),
                    MaxPlateSize = row.Field<decimal?>("MaxPlateSize"),
                })
                .ToList();

                Data = ExcelExport.RPPrdSummaryExport(List, List1, List2, List3, List4, List5, List6, List7, List8, List9, List10, List11, headername, Tital);

                var jsonResult = Json(new
                {
                    Data = Data,
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
        public ActionResult HPHTReport()
        {
            return View();
        }

        public JsonResult RP_Slip_Report_Getdata(string Process, string FromDate, string ToDate, string RCode, string TableNo)
        {
            try
            {
                List<object> _list_Slip = new List<object>();

                DataSet _DropDownList = DbHelper.ExecuteDataSet("RP_Slip_Report_Getdata",
                    new SqlParameter("@Process", Process ?? ""),
                    new SqlParameter("@FromDate", FromDate ?? ""),
                    new SqlParameter("@ToDate", ToDate ?? ""),
                    new SqlParameter("@RCode", RCode ?? ""),
                    new SqlParameter("@TableNo", TableNo ?? ""));

                if (_DropDownList.Tables.Count > 0 && _DropDownList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[0].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[0].Columns[j].ToString(), _DropDownList.Tables[0].Rows[i][j].ToString());
                        }
                        _list_Slip.Add(Values);
                    }
                }

                var DataField = _DropDownList.Tables.Count > 1 ? _DropDownList.Tables[1].AsEnumerable()
               .Select(row => new DataFields
               {
                   dataField = row.Field<string>("dataField"),
                   dataType = row.Field<string>("dataType"),
                   format = row.Field<string>("format"),
                   alignment = row.Field<string>("alignment"),
                   visible = row.Field<bool>("visible"),
               })
               .ToList() : new List<DataFields>();

                var DataFieldSummary = _DropDownList.Tables.Count > 2 ? _DropDownList.Tables[2].AsEnumerable()
                .Select(row => new DataFields
                {
                    column = row.Field<string>("column"),
                    valueFormat = row.Field<string>("valueFormat"),
                    summaryType = row.Field<string>("summaryType"),
                    displayFormat = row.Field<string>("displayFormat")
                })
                .ToList() : new List<DataFields>();

                var jsonResult = Json(new
                {
                    List = _list_Slip,
                    DataField = DataField,
                    DataFieldSummary = DataFieldSummary
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                var jsonResult = Json(new
                {
                    List = new List<object>(),
                    DataField = new List<DataFields>(),
                    DataFieldSummary = new List<DataFields>()
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
        }
        public ActionResult DailyReport()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetDailyReport(string date)
        {
            var model = new DailyReportViewModel
            {
                PolishData = new List<PolishData>(),
                RoughData = new List<RoughData>(),
                VipulbhaiData = new List<VipulbhaiData>(),
                JumboWhiteData = new List<ProcessData>(),
                SmallWhiteData = new List<ProcessData>(),
                ColorData = new List<ProcessData>(),
                UnderProcessData = new List<ProcessData>(),
                TransferData = new List<TransferData>(),
                PrcWiseUnderProcessData = new List<UnderProcessDetail>(),
                ReportDate = date
            };

            try
            {
                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("API_GetData_For_Daily_Report", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Date", date);

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Result Set 1: Polish Data
                            while (reader.Read())
                            {
                                model.PolishData.Add(new PolishData
                                {
                                    ReportDate = reader["ReportDate"].ToString(),
                                    MainGroup = reader["MainGroup"].ToString(),
                                    Status = reader["Status"].ToString(),
                                    PolishWeight = reader["PolishWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishWeight"]),
                                    MumbaiSubmit = reader["MumbaiSubmit"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["MumbaiSubmit"])
                                });
                            }

                            // Result Set 2: Rough Data
                            reader.NextResult();
                            while (reader.Read())
                            {
                                model.RoughData.Add(new RoughData
                                {
                                    ReportDate = reader["ReportDate"].ToString(),
                                    MainGroup = reader["MainGroup"].ToString(),
                                    Status = reader["Status"].ToString(),
                                    RoughWeight = reader["RPartWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["RPartWeight"]),
                                });
                            }

                            // Result Set 2: Rough Data
                            reader.NextResult();
                            while (reader.Read())
                            {
                                model.VipulbhaiData.Add(new VipulbhaiData
                                {
                                    ReportDate = reader["ReportDate"].ToString(),
                                    MainGroup = reader["MainGroup"].ToString(),
                                    Status = reader["Status"].ToString(),
                                    JW = reader["JW"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["JW"]),
                                    JC = reader["JC"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["JC"]),
                                    SW = reader["SW"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["SW"]),
                                    SC = reader["SC"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["SC"]),
                                    Total = reader["Total"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["Total"]),
                                });
                            }

                            // Result Set 3: Jumbo Data
                            reader.NextResult();
                            while (reader.Read())
                            {
                                model.JumboWhiteData.Add(new ProcessData
                                {
                                    ReportDate = reader["ReportDate"].ToString(),
                                    MainGroup = reader["MainGroup"].ToString(),
                                    Process = reader["Process"].ToString(),
                                    RPartWeight = reader["RPartWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["RPartWeight"]),
                                    PolishPrd = reader["PolishPrd"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishPrd"]),
                                    PrdPer = reader["PrdPer"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PrdPer"]),
                                    PolishWeight = reader["PolishWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishWeight"]),
                                    PolishPer = reader["PolishPer"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishPer"])
                                });
                            }

                            // Result Set 4: Small Data
                            reader.NextResult();
                            while (reader.Read())
                            {
                                model.SmallWhiteData.Add(new ProcessData
                                {
                                    ReportDate = reader["ReportDate"].ToString(),
                                    MainGroup = reader["MainGroup"].ToString(),
                                    Process = reader["Process"].ToString(),
                                    RPartWeight = reader["RPartWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["RPartWeight"]),
                                    PolishPrd = reader["PolishPrd"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishPrd"]),
                                    PrdPer = reader["PrdPer"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PrdPer"]),
                                    PolishWeight = reader["PolishWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishWeight"]),
                                    PolishPer = reader["PolishPer"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishPer"])
                                });
                            }

                            // Result Set 5: Process Receive Data
                            reader.NextResult();
                            while (reader.Read())
                            {
                                model.ColorData.Add(new ProcessData
                                {
                                    ReportDate = reader["ReportDate"].ToString(),
                                    MainGroup = reader["MainGroup"].ToString(),
                                    Process = reader["Process"].ToString(),
                                    RPartWeight = reader["RPartWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["RPartWeight"]),
                                    PolishPrd = reader["PolishPrd"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishPrd"]),
                                    PrdPer = reader["PrdPer"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PrdPer"]),
                                    PolishWeight = reader["PolishWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishWeight"]),
                                    PolishPer = reader["PolishPer"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishPer"])
                                });
                            }

                            // Result Set 5: Process Receive Data
                            reader.NextResult();
                            while (reader.Read())
                            {
                                model.UnderProcessData.Add(new ProcessData
                                {
                                    ReportDate = reader["ReportDate"].ToString(),
                                    MainGroup = reader["MainGroup"].ToString(),
                                    Process = reader["Process"].ToString(),
                                    RPartWeight = reader["RPartWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["RPartWeight"]),
                                    PolishPrd = reader["PolishPrd"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishPrd"]),
                                    PrdPer = reader["PrdPer"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PrdPer"]),
                                    PolishWeight = reader["PolishWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishWeight"]),
                                    PolishPer = reader["PolishPer"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishPer"])
                                });
                            }

                            // Result Set 5: Process Receive Data
                            reader.NextResult();
                            while (reader.Read())
                            {
                                model.TransferData.Add(new TransferData
                                {
                                    ReportDate = reader["ReportDate"].ToString(),
                                    MainGroup = reader["MainGroup"].ToString(),
                                    SizeCode = reader["SizeCode"].ToString(),
                                    RPartWeight = reader["RPartWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["RPartWeight"]),
                                    PolishPrd = reader["PolishPrd"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishPrd"]),
                                    PrdPer = reader["PrdPer"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PrdPer"]),
                                    PolishWeight = reader["PolishWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishWeight"]),
                                    PolishPer = reader["PolishPer"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishPer"])
                                });
                            }

                            // Result Set 6: Under Process Detail
                            reader.NextResult();
                            while (reader.Read())
                            {
                                model.PrcWiseUnderProcessData.Add(new UnderProcessDetail
                                {
                                    ReportDate = reader["ReportDate"].ToString(),
                                    MainGroup = reader["MainGroup"].ToString(),
                                    SrNo = Convert.ToInt32(reader["SrNo"]),
                                    Process = reader["Process"].ToString(),
                                    Pcs = reader["Pcs"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["Pcs"]),
                                    RPartWeight = reader["RPartWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["RPartWeight"]),
                                    PolishWeight = reader["PolishWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishWeight"]),
                                    Cur_Polish_Ct = reader["Cur_Polish_Ct"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["Cur_Polish_Ct"]),
                                    DiffPer = reader["DiffPer"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["DiffPer"]),
                                    Ideal_Ct = reader["Ideal_Ct"].ToString()
                                });
                            }
                        }
                    }
                }

                return Json(new { success = true, data = model }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetDailyReport_Excel(string date)
        {
            List<PolishData> PolishData = new List<PolishData>();
            List<RoughData> RoughData = new List<RoughData>();
            List<VipulbhaiData> VipulbhaiData = new List<VipulbhaiData>();
            List<ProcessData> JumboWhiteData = new List<ProcessData>();
            List<ProcessData> SmallWhiteData = new List<ProcessData>();
            List<ProcessData> ColorData = new List<ProcessData>();
            List<ProcessData> UnderProcessData = new List<ProcessData>();
            List<TransferData> TransferData = new List<TransferData>();
            List<UnderProcessDetail> PrcWiseUnderProcessData = new List<UnderProcessDetail>();


            string ReportDate = date;
            try
            {
                string Data = "";
                string headername = "Process Wise Timing";
                DataSet _DropDownList = new DataSet();

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("API_GetData_For_Daily_Report", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Date", date);

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PolishData.Add(new PolishData
                                {
                                    ReportDate = reader["ReportDate"].ToString(),
                                    MainGroup = reader["MainGroup"].ToString(),
                                    Status = reader["Status"].ToString(),
                                    PolishWeight = reader["PolishWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishWeight"]),
                                    MumbaiSubmit = reader["MumbaiSubmit"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["MumbaiSubmit"])
                                });
                            }

                            // Result Set 2: Rough Data
                            reader.NextResult();
                            while (reader.Read())
                            {
                                RoughData.Add(new RoughData
                                {
                                    ReportDate = reader["ReportDate"].ToString(),
                                    MainGroup = reader["MainGroup"].ToString(),
                                    Status = reader["Status"].ToString(),
                                    RoughWeight = reader["RPartWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["RPartWeight"]),
                                });
                            }

                            // Result Set 2: Rough Data
                            reader.NextResult();
                            while (reader.Read())
                            {
                                VipulbhaiData.Add(new VipulbhaiData
                                {
                                    ReportDate = reader["ReportDate"].ToString(),
                                    MainGroup = reader["MainGroup"].ToString(),
                                    Status = reader["Status"].ToString(),
                                    JW = reader["JW"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["JW"]),
                                    JC = reader["JC"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["JC"]),
                                    SW = reader["SW"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["SW"]),
                                    SC = reader["SC"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["SC"]),
                                    Total = reader["Total"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["Total"]),
                                });
                            }

                            // Result Set 3: Jumbo Data
                            reader.NextResult();
                            while (reader.Read())
                            {
                                JumboWhiteData.Add(new ProcessData
                                {
                                    ReportDate = reader["ReportDate"].ToString(),
                                    MainGroup = reader["MainGroup"].ToString(),
                                    Process = reader["Process"].ToString(),
                                    RPartWeight = reader["RPartWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["RPartWeight"]),
                                    PolishPrd = reader["PolishPrd"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishPrd"]),
                                    PrdPer = reader["PrdPer"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PrdPer"]),
                                    PolishWeight = reader["PolishWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishWeight"]),
                                    PolishPer = reader["PolishPer"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishPer"])
                                });
                            }

                            // Result Set 4: Small Data
                            reader.NextResult();
                            while (reader.Read())
                            {
                                SmallWhiteData.Add(new ProcessData
                                {
                                    ReportDate = reader["ReportDate"].ToString(),
                                    MainGroup = reader["MainGroup"].ToString(),
                                    Process = reader["Process"].ToString(),
                                    RPartWeight = reader["RPartWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["RPartWeight"]),
                                    PolishPrd = reader["PolishPrd"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishPrd"]),
                                    PrdPer = reader["PrdPer"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PrdPer"]),
                                    PolishWeight = reader["PolishWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishWeight"]),
                                    PolishPer = reader["PolishPer"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishPer"])
                                });
                            }

                            // Result Set 5: Process Receive Data
                            reader.NextResult();
                            while (reader.Read())
                            {
                                ColorData.Add(new ProcessData
                                {
                                    ReportDate = reader["ReportDate"].ToString(),
                                    MainGroup = reader["MainGroup"].ToString(),
                                    Process = reader["Process"].ToString(),
                                    RPartWeight = reader["RPartWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["RPartWeight"]),
                                    PolishPrd = reader["PolishPrd"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishPrd"]),
                                    PrdPer = reader["PrdPer"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PrdPer"]),
                                    PolishWeight = reader["PolishWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishWeight"]),
                                    PolishPer = reader["PolishPer"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishPer"])
                                });
                            }

                            // Result Set 5: Process Receive Data
                            reader.NextResult();
                            while (reader.Read())
                            {
                                UnderProcessData.Add(new ProcessData
                                {
                                    ReportDate = reader["ReportDate"].ToString(),
                                    MainGroup = reader["MainGroup"].ToString(),
                                    Process = reader["Process"].ToString(),
                                    RPartWeight = reader["RPartWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["RPartWeight"]),
                                    PolishPrd = reader["PolishPrd"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishPrd"]),
                                    PrdPer = reader["PrdPer"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PrdPer"]),
                                    PolishWeight = reader["PolishWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishWeight"]),
                                    PolishPer = reader["PolishPer"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishPer"])
                                });
                            }

                            // Result Set 5: Process Receive Data
                            reader.NextResult();
                            while (reader.Read())
                            {
                                TransferData.Add(new TransferData
                                {
                                    ReportDate = reader["ReportDate"].ToString(),
                                    MainGroup = reader["MainGroup"].ToString(),
                                    SizeCode = reader["SizeCode"].ToString(),
                                    RPartWeight = reader["RPartWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["RPartWeight"]),
                                    PolishPrd = reader["PolishPrd"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishPrd"]),
                                    PrdPer = reader["PrdPer"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PrdPer"]),
                                    PolishWeight = reader["PolishWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishWeight"]),
                                    PolishPer = reader["PolishPer"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishPer"])
                                });
                            }

                            // Result Set 6: Under Process Detail
                            reader.NextResult();
                            while (reader.Read())
                            {
                                PrcWiseUnderProcessData.Add(new UnderProcessDetail
                                {
                                    ReportDate = reader["ReportDate"].ToString(),
                                    MainGroup = reader["MainGroup"].ToString(),
                                    SrNo = Convert.ToInt32(reader["SrNo"]),
                                    Process = reader["Process"].ToString(),
                                    Pcs = reader["Pcs"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["Pcs"]),
                                    RPartWeight = reader["RPartWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["RPartWeight"]),
                                    PolishWeight = reader["PolishWeight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PolishWeight"]),
                                    Cur_Polish_Ct = reader["Cur_Polish_Ct"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["Cur_Polish_Ct"]),
                                    DiffPer = reader["DiffPer"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["DiffPer"]),
                                    Ideal_Ct = reader["Ideal_Ct"].ToString()
                                });
                            }
                        }
                    }
                }

                Data = ExcelExport.ExportDailyStockExcelGen(PolishData, RoughData, VipulbhaiData, JumboWhiteData, SmallWhiteData, ColorData, UnderProcessData, TransferData, PrcWiseUnderProcessData, ReportDate, headername);

                var jsonResult = Json(new
                {
                    Data = Data
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
        public ActionResult ProcessWiseTimingReport()
        {
            return View();
        }
        public JsonResult GetData_For_Process_Wise_Timing_Report(string Date, string TableNo)
        {
            try
            {
                List<object> List = new List<object>();
                List<object> List1 = new List<object>();
                List<object> List2 = new List<object>();
                List<object> List3 = new List<object>();

                DataSet _DropDownList = DbHelper.ExecuteDataSet("API_GetData_For_Process_Wise_Timing_Report",
                    new SqlParameter("@Date", Date),
                    new SqlParameter("@TableNo", TableNo));

                if (_DropDownList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[0].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[0].Columns[j].ToString(), _DropDownList.Tables[0].Rows[i][j].ToString());
                        }
                        List.Add(Values);
                    }
                }
                if (_DropDownList.Tables[1].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[1].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[1].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[1].Columns[j].ToString(), _DropDownList.Tables[1].Rows[i][j].ToString());
                        }
                        List1.Add(Values);
                    }
                }

                if (_DropDownList.Tables[2].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[2].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[2].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[2].Columns[j].ToString(), _DropDownList.Tables[2].Rows[i][j].ToString());
                        }
                        List2.Add(Values);
                    }
                }

                if (_DropDownList.Tables[3].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[3].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[3].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[3].Columns[j].ToString(), _DropDownList.Tables[3].Rows[i][j].ToString());
                        }
                        List3.Add(Values);
                    }
                }

                var jsonResult = Json(new
                {
                    List = List,
                    List1 = List1,
                    List2 = List2,
                    List3 = List3,
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
        public JsonResult Get_DataList_Process_Wise_Timing_Excel(string Date, string TableNo)
        {
            try
            {
                string Data = "";
                string headername = "Process Wise Timing";

                DataSet _DropDownList = DbHelper.ExecuteDataSet("API_GetData_For_Process_Wise_Timing_Report",
                    new SqlParameter("@Date", Date),
                    new SqlParameter("@TableNo", TableNo));

                List<ProcessWiseTiming> List = new List<ProcessWiseTiming>();
                List<ProcessWiseTiming> List1 = new List<ProcessWiseTiming>();
                List<ProcessWiseTiming> List2 = new List<ProcessWiseTiming>();
                List<ProcessWiseTiming> List3 = new List<ProcessWiseTiming>();


                List = _DropDownList.Tables[0].AsEnumerable().
                Select(row => new ProcessWiseTiming
                {
                    Type = row.Field<string>("Type"),
                    TableNo = row.Field<string>("TableNo"),
                    Process = row.Field<string>("Process"),
                    FinishDays = row.Field<int>("FinishDays"),
                    Days = row.Field<int?>("Days"),
                    ProcessDay = row.Field<int?>("MaxPrcDays"),
                    TotalPcs = row.Field<int?>("TotalPcs"),
                    ONTIMEPer = row.Field<decimal?>("ONTIMEPer"),
                    OVERDAYPer = row.Field<decimal?>("OVERDAYPer"),
                }).ToList();

                List1 = _DropDownList.Tables[1].AsEnumerable().
                Select(row => new ProcessWiseTiming
                {
                    Type = row.Field<string>("Type"),
                    TableNo = row.Field<string>("TableNo"),
                    Process = row.Field<string>("Process"),
                    FinishDays = row.Field<int>("FinishDays"),
                    Days = row.Field<int?>("Days"),
                    ProcessDay = row.Field<int?>("MaxPrcDays"),
                    TotalPcs = row.Field<int?>("TotalPcs"),
                    ONTIMEPer = row.Field<decimal?>("ONTIMEPer"),
                    OVERDAYPer = row.Field<decimal?>("OVERDAYPer"),
                }).ToList();

                List2 = _DropDownList.Tables[2].AsEnumerable().
                Select(row => new ProcessWiseTiming
                {
                    Type = row.Field<string>("Type"),
                    TableNo = row.Field<string>("TableNo"),
                    Process = row.Field<string>("Process"),
                    FinishDays = row.Field<int>("FinishDays"),
                    Days = row.Field<int?>("Days"),
                    ProcessDay = row.Field<int?>("MaxPrcDays"),
                    TotalPcs = row.Field<int?>("TotalPcs"),
                    ONTIMEPer = row.Field<decimal?>("ONTIMEPer"),
                    OVERDAYPer = row.Field<decimal?>("OVERDAYPer"),
                }).ToList();

                List3 = _DropDownList.Tables[3].AsEnumerable().
                Select(row => new ProcessWiseTiming
                {
                    Type = row.Field<string>("Type"),
                    TableNo = row.Field<string>("TableNo"),
                    Process = row.Field<string>("Process"),
                    FinishDays = row.Field<int>("FinishDays"),
                    Days = row.Field<int?>("Days"),
                    ProcessDay = row.Field<int?>("MaxPrcDays"),
                    TotalPcs = row.Field<int?>("TotalPcs"),
                    ONTIMEPer = row.Field<decimal?>("ONTIMEPer"),
                    OVERDAYPer = row.Field<decimal?>("OVERDAYPer"),
                }).ToList();

                Data = ExcelExport.ExportProcessWiseTimingReport(List, List1, List2, List3, Date, TableNo, headername);

                var jsonResult = Json(new
                {
                    Data = Data
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

        public ActionResult FourPOkLotSummary()
        {
            return View();
        }
        public JsonResult GetData_For_FourPOkLotSummary_Report(string Date)
        {
            try
            {
                List<object> After4POk_Loss = new List<object>();

                DataSet _DropDownList = DbHelper.ExecuteDataSet("API_GetData_For_After4POk_Loss_Report",
                    new SqlParameter("@Date", Date));

                if (_DropDownList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[0].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[0].Columns[j].ToString(), _DropDownList.Tables[0].Rows[i][j].ToString());
                        }
                        After4POk_Loss.Add(Values);
                    }
                }

                List<object> RoughTo4P_Loss = new List<object>();

                DataSet ds = DbHelper.ExecuteDataSet("API_GetData_For_RoughTo4POk_Loss_Report",
                    new SqlParameter("@Date", Date));

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                        {
                            Values.Add(ds.Tables[0].Columns[j].ToString(), ds.Tables[0].Rows[i][j].ToString());
                        }
                        RoughTo4P_Loss.Add(Values);
                    }
                }

                var jsonResult = Json(new
                {
                    After4POk_Loss = After4POk_Loss,
                    RoughTo4P_Loss = RoughTo4P_Loss
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

        public ActionResult ProcessWiseIssueReceiveLossReport()
        {
            return View();
        }

        public JsonResult GetData_For_Process_Wise_Issue_Receive_Loss_Report(string Date)
        {
            try
            {
                List<object> List = new List<object>();

                DataSet _DropDownList = DbHelper.ExecuteDataSet("API_GetData_For_Process_Wise_Issue_Receive_Loss_Report",
                    new SqlParameter("@Date", Date));

                if (_DropDownList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[0].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[0].Columns[j].ToString(), _DropDownList.Tables[0].Rows[i][j].ToString());
                        }
                        List.Add(Values);
                    }
                }

                var jsonResult = Json(new
                {
                    List = List
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

        public ActionResult DateWiseLossJumbo()
        {
            return View();
        }
        public JsonResult Jumbo_GetData_For_Date_Wise_Loss_Summary(string Date)
        {
            try
            {
                List<object> List = new List<object>();

                DataSet _DropDownList = DbHelper.ExecuteDataSet("Jumbo_GetData_For_Date_Wise_Loss_Summary",
                    new SqlParameter("@Date", Date));

                if (_DropDownList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[0].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[0].Columns[j].ToString(), _DropDownList.Tables[0].Rows[i][j].ToString());
                        }
                        List.Add(Values);
                    }
                }

                var jsonResult = Json(new
                {
                    List = List
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

        public ActionResult CleavingReport()
        {
            return View();
        }
        [HttpPost]
        public JsonResult RP_Getdata_Cleaving_Report_Summary(string reportDate, string Action, string Status)
        {
            try
            {
                List<object> SummaryList = new List<object>();
                List<object> List = new List<object>();

                DataSet _DropDownList = DbHelper.ExecuteDataSet("RP_Getdata_Cleaving_Report_Summary",
                    new SqlParameter("@ReportDate", reportDate),
                    new SqlParameter("@Action", Action),
                    new SqlParameter("@Status", Status));
                if (_DropDownList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[0].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[0].Columns[j].ToString(), _DropDownList.Tables[0].Rows[i][j].ToString());
                        }
                        SummaryList.Add(Values);
                    }
                }
                if (_DropDownList.Tables[1].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[1].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[1].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[1].Columns[j].ToString(), _DropDownList.Tables[1].Rows[i][j].ToString());
                        }
                        List.Add(Values);
                    }
                }
                var jsonResult = Json(new
                {
                    SummaryList = SummaryList,
                    List = List,
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

        public ActionResult DSTReport()
        {
            return View();
        }
        [HttpPost]
        public JsonResult RP_Getdata_DST_Report_Summary(string reportDate, string Action, string Status)
        {
            try
            {
                List<object> List = new List<object>();

                DataSet _DropDownList = DbHelper.ExecuteDataSet("RP_Getdata_DST_Report_Summary",
                    new SqlParameter("@ReportDate", reportDate),
                    new SqlParameter("@Action", Action),
                    new SqlParameter("@Status", Status));
                if (_DropDownList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[0].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[0].Columns[j].ToString(), _DropDownList.Tables[0].Rows[i][j].ToString());
                        }
                        List.Add(Values);
                    }
                }
                var jsonResult = Json(new
                {
                    List = List,
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

        public ActionResult MFGReport()
        {
            return View();
        }
        [HttpPost]
        public JsonResult RP_Getdata_MFG_Report_Summary(string reportDate, string Action, string Status)
        {
            try
            {
                List<object> List = new List<object>();

                DataSet _DropDownList = DbHelper.ExecuteDataSet("RP_Getdata_MFG_Report_Summary",
                    new SqlParameter("@ReportDate", reportDate),
                    new SqlParameter("@Action", Action),
                    new SqlParameter("@Status", Status));
                if (_DropDownList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[0].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[0].Columns[j].ToString(), _DropDownList.Tables[0].Rows[i][j].ToString());
                        }
                        List.Add(Values);
                    }
                }
                var jsonResult = Json(new
                {
                    List = List,
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
    
    }
}