using Microsoft.SqlServer.Server;
using Slip.Models;
using Slip.Models.Filter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Services.Description;

namespace Slip.API
{
    public class APIController : ApiController
    {
        string conn = ConfigurationManager.ConnectionStrings["StockDetailConnectionString"].ConnectionString;

        [Route("api/API/API_TRN_Process_Timing_Getdata")]
        [BasicAuthentication]//Default
        [HttpGet]
        public IHttpActionResult API_TRN_Process_Timing_Getdata(string FromDate, string ToDate)
        {
            try
            {
                DateTime fromDt, toDt;

                if (string.IsNullOrEmpty(FromDate) && string.IsNullOrEmpty(ToDate))
                {
                    return Ok(new { Message = "Date required!" });
                }

                // --- 1. If FromDate is entered → ToDate must be entered ---
                if (!string.IsNullOrEmpty(FromDate) && string.IsNullOrEmpty(ToDate))
                {
                    return Ok(new { Message = "ToDate is required when FromDate is entered!" });
                }

                // --- 2. If ToDate is entered → FromDate must be entered ---
                if (string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    return Ok(new { Message = "FromDate is required when ToDate is entered!" });
                }

                // --- 3. Convert dates if both entered ---
                if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    // Try parse FromDate
                    if (!DateTime.TryParse(FromDate, out fromDt))
                    {
                        return Ok(new { Message = "Invalid FromDate format!" });
                    }

                    // Try parse ToDate
                    if (!DateTime.TryParse(ToDate, out toDt))
                    {
                        return Ok(new { Message = "Invalid ToDate format!" });
                    }

                    // --- 4. Check that FromDate <= ToDate ---
                    if (fromDt > toDt)
                    {
                        return Ok(new { Message = "FromDate cannot be greater than ToDate!" });
                    }
                }

                List<object> _list_Country = new List<object>();
                DataSet _DropDownList = new DataSet();

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("API_TRN_Process_Timing_Getdata", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FromDate", FromDate);
                        cmd.Parameters.AddWithValue("@ToDate", ToDate);
                        con.Open();
                        //   ErrorLogger.ErrorLogStr("1 Connection Open");
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(_DropDownList);
                        cmd.Dispose();
                    }
                    con.Close();
                    //  ErrorLogger.ErrorLogStr("2 Connection Close");
                }

                var List = _DropDownList.Tables[0].AsEnumerable().AsEnumerable()
                .Select(row => new Api_TRN_Process_Timing
                {
                    ProcessName = row.Field<string>("ProcessName"),
                    TotalPcs = row.Field<int>("TotalPcs"),
                    OnTimePer = row.Field<decimal>("OnTimePer"),
                    OffTimePer = row.Field<decimal>("OffTimePer"),
                    RoughType = row.Field<string>("RoughType"),
                })
                .ToList();
                // ErrorLogger.ErrorLogStr("3 List" + List);
                var Data = new { Message = "Data received successfully!", Process_Timing = List };
                return Ok(Data);
            }
            catch (Exception ex)
            {
                // You can log here: ErrorLogger.ErrorLogStr(ex.Message);
                return Ok(new
                {
                    Message = "An error occurred!",
                    Error = ex.Message
                });
            }
        }

        [Route("api/API/API_GetData_For_Daily_Report")]
        [BasicAuthentication]//Default
        [HttpGet]
        public IHttpActionResult API_GetData_For_Daily_Report(string Date)
        {
            try
            {
                DateTime Dt;

                if (string.IsNullOrEmpty(Date))
                {
                    return Ok(new { Message = "Date required!" });
                }

                if (!string.IsNullOrEmpty(Date))
                {
                    // Try parse FromDate
                    if (!DateTime.TryParse(Date, out Dt))
                    {
                        return Ok(new { Message = "Invalid Date format!" });
                    }
                }

                List<object> _list_Country = new List<object>();
                DataSet _DropDownList = new DataSet();

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("API_GetData_For_Daily_Report", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Date", Date);
                        con.Open();
                        //   ErrorLogger.ErrorLogStr("1 Connection Open");
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(_DropDownList);
                        cmd.Dispose();
                    }
                    con.Close();
                    //  ErrorLogger.ErrorLogStr("2 Connection Close");
                }

                var List = _DropDownList.Tables[0].AsEnumerable().AsEnumerable()
                .Select(row => new PolishData
                {
                    MainGroup = row.Field<string>("MainGroup"),
                    ReportDate = row.Field<string>("ReportDate"),
                    Status = row.Field<string>("Status"),
                    PolishWeight = row.Field<decimal?>("PolishWeight"),
                    MumbaiSubmit = row.Field<decimal?>("MumbaiSubmit"),
                })
                .ToList();

                var List1 = _DropDownList.Tables[1].AsEnumerable().AsEnumerable()
                .Select(row => new RoughData
                {
                    MainGroup = row.Field<string>("MainGroup"),
                    ReportDate = row.Field<string>("ReportDate"),
                    Status = row.Field<string>("Status"),
                    RoughWeight = row.Field<decimal?>("RPartWeight"),
                })
                .ToList();

                var List2 = _DropDownList.Tables[2].AsEnumerable().AsEnumerable()
                .Select(row => new VipulbhaiData
                {
                    MainGroup = row.Field<string>("MainGroup"),
                    ReportDate = row.Field<string>("ReportDate"),
                    Status = row.Field<string>("Status"),
                    JW = row.Field<decimal?>("JW"),
                    JC = row.Field<decimal?>("JC"),
                    SW = row.Field<decimal?>("SW"),
                    SC = row.Field<decimal?>("SC"),
                    Total = row.Field<decimal?>("Total"),
                })
                .ToList();

                var List3 = _DropDownList.Tables[3].AsEnumerable().AsEnumerable()
                .Select(row => new ProcessData
                {
                    MainGroup = row.Field<string>("MainGroup"),
                    ReportDate = row.Field<string>("ReportDate"),
                    Process = row.Field<string>("Process"),
                    RPartWeight = row.Field<decimal?>("RPartWeight"),
                    PrdPer = row.Field<decimal?>("PrdPer"),
                    PolishPrd = row.Field<decimal?>("PolishPrd"),
                    PolishWeight = row.Field<decimal?>("PolishWeight"),
                    PolishPer = row.Field<decimal?>("PolishPer"),
                })
                .ToList();

                //var List4 = _DropDownList.Tables[4].AsEnumerable().AsEnumerable()
                //.Select(row => new ProcessData
                //{
                //    MainGroup = row.Field<string>("MainGroup"),
                //    ReportDate = row.Field<string>("ReportDate"),
                //    Process = row.Field<string>("Process"),
                //    RPartWeight = row.Field<decimal?>("RPartWeight"),
                //    PrdPer = row.Field<decimal?>("PrdPer"),
                //    PolishPrd = row.Field<decimal?>("PolishPrd"),
                //    PolishWeight = row.Field<decimal?>("PolishWeight"),
                //    PolishPer = row.Field<decimal?>("PolishPer"),
                //})
                //.ToList();

                //var List5 = _DropDownList.Tables[5].AsEnumerable().AsEnumerable()
                //.Select(row => new ProcessData
                //{
                //    MainGroup = row.Field<string>("MainGroup"),
                //    ReportDate = row.Field<string>("ReportDate"),
                //    Process = row.Field<string>("Process"),
                //    RPartWeight = row.Field<decimal?>("RPartWeight"),
                //    PrdPer = row.Field<decimal?>("PrdPer"),
                //    PolishPrd = row.Field<decimal?>("PolishPrd"),
                //    PolishWeight = row.Field<decimal?>("PolishWeight"),
                //    PolishPer = row.Field<decimal?>("PolishPer"),
                //})
                //.ToList();

                //var List6 = _DropDownList.Tables[6].AsEnumerable().AsEnumerable()
                //.Select(row => new ProcessData
                //{
                //    ReportDate = row.Field<string>("ReportDate"),
                //    Process = row.Field<string>("Process"),
                //    RPartWeight = row.Field<decimal?>("RPartWeight"),
                //    PrdPer = row.Field<decimal?>("PrdPer"),
                //    PolishPrd = row.Field<decimal?>("PolishPrd"),
                //    PolishWeight = row.Field<decimal?>("PolishWeight"),
                //    PolishPer = row.Field<decimal?>("PolishPer"),
                //})
                //.ToList();

                //var List7 = _DropDownList.Tables[7].AsEnumerable().AsEnumerable()
                //.Select(row => new TransferData
                //{
                //    MainGroup = row.Field<string>("MainGroup"),
                //    ReportDate = row.Field<string>("ReportDate"),
                //    SizeCode = row.Field<string>("SizeCode"),
                //    RPartWeight = row.Field<decimal?>("RPartWeight"),
                //    PrdPer = row.Field<decimal?>("PrdPer"),
                //    PolishPrd = row.Field<decimal?>("PolishPrd"),
                //    PolishWeight = row.Field<decimal?>("PolishWeight"),
                //    PolishPer = row.Field<decimal?>("PolishPer"),
                //})
                //.ToList();

                //var List8 = _DropDownList.Tables[8].AsEnumerable().AsEnumerable()
                //.Select(row => new UnderProcessDetail
                //{
                //    MainGroup = row.Field<string>("MainGroup"),
                //    ReportDate = row.Field<string>("ReportDate"),
                //    SrNo = row.Field<int>("SrNo"),
                //    Process = row.Field<string>("Process"),
                //    Pcs = row.Field<int?>("Pcs"),
                //    Rough_Ct = row.Field<decimal?>("RPartWeight"),
                //    Polish_Ct = row.Field<decimal?>("PolishWeight"),
                //    Cur_Polish_Ct = row.Field<decimal?>("Cur_Polish_Ct"),
                //    DiffPer = row.Field<decimal?>("DiffPer"),
                //    Ideal_Ct = row.Field<string>("Ideal_Ct"),
                //})
                //.ToList();

                // ErrorLogger.ErrorLogStr("3 List" + List);
                var Data = new { Message = "Data received successfully!", List, List1, List2, List3/*, List4, List5, List6, List7, List8*/ };
                return Ok(Data);
            }
            catch (Exception ex)
            {
                // You can log here: ErrorLogger.ErrorLogStr(ex.Message);
                return Ok(new
                {
                    Message = "An error occurred!",
                    Error = ex.Message
                });
            }
        }

        [Route("api/API/API_GetData_For_After4POk_Loss_Report")]
        [BasicAuthentication]//Default
        [HttpGet]
        public IHttpActionResult API_GetData_For_After4POk_Loss_Report(string Date)
        {
            try
            {
                DateTime Dt;

                if (string.IsNullOrEmpty(Date))
                {
                    return Ok(new { Message = "Date required!" });
                }

                if (!string.IsNullOrEmpty(Date))
                {
                    // Try parse FromDate
                    if (!DateTime.TryParse(Date, out Dt))
                    {
                        return Ok(new { Message = "Invalid Date format!" });
                    }
                }

                List<object> _list_Country = new List<object>();
                DataSet _DropDownList = new DataSet();

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("API_GetData_For_After4POk_Loss_Report", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Date", Date);
                        con.Open();
                        //   ErrorLogger.ErrorLogStr("1 Connection Open");
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(_DropDownList);
                        cmd.Dispose();
                    }
                    con.Close();
                    //  ErrorLogger.ErrorLogStr("2 Connection Close");
                }

                var After4POk_Loss = _DropDownList.Tables[0].AsEnumerable().AsEnumerable()
                .Select(row => new Api_After4POk_Loss
                {
                    ReportDate = row.Field<string>("ReportDate"),
                    Process = row.Field<string>("Process"),
                    IPcs = row.Field<int?>("IPcs"),
                    IRPartWeight = row.Field<decimal?>("IRPartWeight"),
                    IPolishWeight = row.Field<decimal?>("IPolishWeight"),
                    IModel = row.Field<decimal?>("IModel"),
                    RPcs = row.Field<int?>("RPcs"),
                    RRPartWeight = row.Field<decimal?>("RRPartWeight"),
                    RPolishWeight = row.Field<decimal?>("RPolishWeight"),
                    RModel = row.Field<decimal?>("RModel"),
                    DRPartWeight = row.Field<decimal?>("DRPartWeight"),
                    DiffRPer = row.Field<decimal?>("DiffRPer"),
                    DPolishWeight = row.Field<decimal?>("DPolishWeight"),
                    DiffPPer = row.Field<decimal?>("DiffPPer"),
                    TPcs = row.Field<int?>("TPcs"),
                    TRPartWeight = row.Field<decimal?>("TRPartWeight"),
                    TPolishWeight = row.Field<decimal?>("TPolishWeight"),
                })
                .ToList();

                // ErrorLogger.ErrorLogStr("3 List" + List);
                var Data = new { Message = "Data received successfully!", After4POk_Loss };
                return Ok(Data);
            }
            catch (Exception ex)
            {
                // You can log here: ErrorLogger.ErrorLogStr(ex.Message);
                return Ok(new
                {
                    Message = "An error occurred!",
                    Error = ex.Message
                });
            }
        }

        [Route("api/API/API_GetData_For_RoughTo4POk_Loss_Report")]
        [BasicAuthentication]//Default
        [HttpGet]
        public IHttpActionResult API_GetData_For_RoughTo4POk_Loss_Report(string Date)
        {
            try
            {
                DateTime Dt;

                if (string.IsNullOrEmpty(Date))
                {
                    return Ok(new { Message = "Date required!" });
                }

                if (!string.IsNullOrEmpty(Date))
                {
                    // Try parse FromDate
                    if (!DateTime.TryParse(Date, out Dt))
                    {
                        return Ok(new { Message = "Invalid Date format!" });
                    }
                }

                List<object> _list_Country = new List<object>();
                DataSet _DropDownList = new DataSet();

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("API_GetData_For_RoughTo4POk_Loss_Report", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Date", Date);
                        con.Open();
                        //   ErrorLogger.ErrorLogStr("1 Connection Open");
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(_DropDownList);
                        cmd.Dispose();
                    }
                    con.Close();
                    //  ErrorLogger.ErrorLogStr("2 Connection Close");
                }

                var After4POk_Loss = _DropDownList.Tables[0].AsEnumerable().AsEnumerable()
                .Select(row => new Api_RoughTo4POk_Loss
                {
                    ReportDate = row.Field<string>("ReportDate"),
                    Process = row.Field<string>("Process"),
                    PrdRPartWeight = row.Field<decimal?>("PrdRPartWeight"),
                    PrdPolishWeight = row.Field<decimal?>("PrdPolishWeight"),
                    PrdPer = row.Field<decimal?>("PrdPer"),
                    FPRPartWeight = row.Field<decimal?>("FPRPartWeight"),
                    FPPolishWeight = row.Field<decimal?>("FPPolishWeight"),
                    FPPer = row.Field<decimal?>("FPPer"),
                    FPModel = row.Field<decimal?>("FPModel"),
                    DPolishWeight = row.Field<decimal?>("DPolishWeight"),
                    DiffPPer = row.Field<decimal?>("DiffPPer"),
                    RE4P = row.Field<int?>("RE4P"),
                })
                .ToList();

                // ErrorLogger.ErrorLogStr("3 List" + List);
                var Data = new { Message = "Data received successfully!", After4POk_Loss };
                return Ok(Data);
            }
            catch (Exception ex)
            {
                // You can log here: ErrorLogger.ErrorLogStr(ex.Message);
                return Ok(new
                {
                    Message = "An error occurred!",
                    Error = ex.Message
                });
            }
        }

        [Route("api/API/API_Slip_GetData_For_Filter")]
        [BasicAuthentication]//Default
        [HttpGet]
        public IHttpActionResult API_Slip_GetData_For_Filter()
        {
            try
            {
                List<object> ProcessList = new List<object>();
                List<object> RCodeList = new List<object>();
                List<object> TableList = new List<object>();
                DataSet _DropDownList = new DataSet();

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("API_Slip_GetData_For_Filter", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        //   ErrorLogger.ErrorLogStr("1 Connection Open");
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(_DropDownList);
                        cmd.Dispose();
                    }
                    con.Close();
                    //  ErrorLogger.ErrorLogStr("2 Connection Close");
                }

                if (_DropDownList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[0].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[0].Columns[j].ToString(), _DropDownList.Tables[0].Rows[i][j].ToString());
                        }
                        ProcessList.Add(Values);
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
                        RCodeList.Add(Values);
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
                        TableList.Add(Values);
                    }
                }

                // ErrorLogger.ErrorLogStr("3 List" + List);
                var Data = new { Message = "Data received successfully!", ProcessList, RCodeList, TableList };
                return Ok(Data);
            }
            catch (Exception ex)
            {
                // You can log here: ErrorLogger.ErrorLogStr(ex.Message);
                return Ok(new
                {
                    Message = "An error occurred!",
                    Error = ex.Message
                });
            }
        }

        [Route("api/API/API_Slip_Report_Getdata")]
        [BasicAuthentication]//Default
        [HttpGet]
        public IHttpActionResult API_Slip_Report_Getdata(string Process, string FromDate, string ToDate)
        {
            try
            {
                var Data = new Api_Response
                {
                    Message = "",
                    List = new List<object>()
                };

                if (string.IsNullOrEmpty(Process))
                {
                    Data.Message = "Process Required!";
                    return Ok(Data);
                }
                if (string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    Data.Message = "FromDate is required when ToDate is entered!";
                    return Ok(Data);
                }

                if (!string.IsNullOrEmpty(FromDate) && string.IsNullOrEmpty(ToDate))
                {
                    Data.Message = "ToDate is required when FromDate is entered!";
                    return Ok(Data);
                }

                DateTime fromDt, toDt;

                if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    // Try parse FromDate
                    if (!DateTime.TryParse(FromDate, out fromDt))
                    {
                        Data.Message = "Invalid FromDate format!";
                        return Ok(Data);
                    }

                    // Try parse ToDate
                    if (!DateTime.TryParse(ToDate, out toDt))
                    {
                        Data.Message = "Invalid ToDate format!";
                        return Ok(Data);
                    }

                    // --- 4. Check that FromDate <= ToDate ---
                    if (fromDt > toDt)
                    {
                        Data.Message = "FromDate cannot be greater than ToDate!";
                        return Ok(Data);
                    }
                }

                List<object> _list_Slip = new List<object>();
                DataSet _DropDownList = new DataSet();

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("API_Slip_Report_Getdata", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Process", Process);
                        cmd.Parameters.AddWithValue("@FromDate", FromDate);
                        cmd.Parameters.AddWithValue("@ToDate", ToDate);
                        con.Open();
                        //   ErrorLogger.ErrorLogStr("1 Connection Open");
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(_DropDownList);
                        cmd.Dispose();
                    }
                    con.Close();
                    //  ErrorLogger.ErrorLogStr("2 Connection Close");
                }

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
                Data.Message = "Data received successfully!";
                Data.List = _list_Slip;
                //var Data = new { List = _list_Slip };
                // ErrorLogger.ErrorLogStr("3 List" + List);
                return Ok(Data);
            }
            catch (Exception ex)
            {
                // You can log here: ErrorLogger.ErrorLogStr(ex.Message);
                return Ok(new
                {
                    Message = "An error occurred!",
                    Error = ex.Message
                });
            }
        }

        [Route("api/API/API_GetData_For_Process_Wise_Timing_Report")]
        [BasicAuthentication]//Default
        [HttpGet]
        public IHttpActionResult API_GetData_For_Process_Wise_Timing_Report(string Date)
        {
            try
            {
                DateTime Dt;

                if (string.IsNullOrEmpty(Date))
                {
                    return Ok(new { Message = "Date required!" });
                }

                if (!string.IsNullOrEmpty(Date))
                {
                    // Try parse FromDate
                    if (!DateTime.TryParse(Date, out Dt))
                    {
                        return Ok(new { Message = "Invalid Date format!" });
                    }
                }

                List<object> JumboWhiteList = new List<object>();
                DataSet _DropDownList = new DataSet();

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("API_GetData_For_Process_Wise_Timing_Report", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Date", Date);
                        cmd.Parameters.AddWithValue("@TableNo", 11);
                        con.Open();
                        //   ErrorLogger.ErrorLogStr("1 Connection Open");
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(_DropDownList);
                        cmd.Dispose();
                    }
                    con.Close();
                    //  ErrorLogger.ErrorLogStr("2 Connection Close");
                }

                if (_DropDownList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[0].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[0].Columns[j].ToString(), _DropDownList.Tables[0].Rows[i][j].ToString());
                        }
                        JumboWhiteList.Add(Values);
                    }
                }

                // ErrorLogger.ErrorLogStr("3 List" + List);
                var Data = new { Message = "Data received successfully!", JumboWhiteList };
                return Ok(Data);
            }
            catch (Exception ex)
            {
                // You can log here: ErrorLogger.ErrorLogStr(ex.Message);
                return Ok(new
                {
                    Message = "An error occurred!",
                    Error = ex.Message
                });
            }
        }

        [Route("api/API/API_GetData_For_Process_Wise_Issue_Receive_Loss_Report")]
        [BasicAuthentication]//Default
        [HttpGet]
        public IHttpActionResult API_GetData_For_Process_Wise_Issue_Receive_Loss_Report(string Date)
        {
            try
            {
                DateTime Dt;

                if (string.IsNullOrEmpty(Date))
                {
                    return Ok(new { Message = "Date required!" });
                }

                if (!string.IsNullOrEmpty(Date))
                {
                    // Try parse FromDate
                    if (!DateTime.TryParse(Date, out Dt))
                    {
                        return Ok(new { Message = "Invalid Date format!" });
                    }
                }

                List<object> List = new List<object>();
                DataSet _DropDownList = new DataSet();

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("API_GetData_For_Process_Wise_Issue_Receive_Loss_Report", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Date", Date);
                        con.Open();
                        //   ErrorLogger.ErrorLogStr("1 Connection Open");
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(_DropDownList);
                        cmd.Dispose();
                    }
                    con.Close();
                    //  ErrorLogger.ErrorLogStr("2 Connection Close");
                }

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

                // ErrorLogger.ErrorLogStr("3 List" + List);
                var Data = new { Message = "Data received successfully!", List };
                return Ok(Data);
            }
            catch (Exception ex)
            {
                // You can log here: ErrorLogger.ErrorLogStr(ex.Message);
                return Ok(new
                {
                    Message = "An error occurred!",
                    Error = ex.Message
                });
            }
        }
       
        [Route("api/API/API_RP_Prd_Summary_Export")]
        [BasicAuthentication]//Default
        [HttpGet]
        public IHttpActionResult API_RP_Prd_Summary_Export(string RCode, string FromDate, string ToDate)
        {
            try
            {
                DateTime Dt;

                string Error = "";

                if (string.IsNullOrEmpty(RCode) && string.IsNullOrEmpty(FromDate) && string.IsNullOrEmpty(ToDate))
                {
                    Error = "Please Enter RCode";
                    return Ok(Error);
                }

                if (!string.IsNullOrEmpty(RCode))
                {
                    if (!string.IsNullOrEmpty(FromDate) || !string.IsNullOrEmpty(ToDate))
                    {
                        Error = "You Can Enter Only RCode OR Date";
                        return Ok(Error);
                    }
                }

                DateTime fromDt, toDt;

                if (!string.IsNullOrEmpty(FromDate) || !string.IsNullOrEmpty(ToDate))
                {
                    if (!string.IsNullOrEmpty(FromDate) && string.IsNullOrEmpty(ToDate))
                    {
                        Error = "Please Enter ToDate";
                        return Ok(Error);
                    }
                    if (string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                    {
                        Error = "Please Enter FromDate";
                        return Ok(Error);
                    }
                    // Try parse FromDate
                    if (!DateTime.TryParse(FromDate, out fromDt))
                    {
                        Error = "Invalid FromDate format!";
                        return Ok(Error);
                    }

                    // Try parse ToDate
                    if (!DateTime.TryParse(ToDate, out toDt))
                    {
                        Error = "Invalid ToDate format!";
                        return Ok(Error);
                    }

                    // --- 4. Check that FromDate <= ToDate ---
                    if (fromDt > toDt)
                    {
                        Error = "FromDate cannot be greater than ToDate!";
                        return Ok(Error);
                    }

                    if (!string.IsNullOrEmpty(RCode))
                    {
                        Error = "You Can Enter Only RCode OR Date";
                        return Ok(Error);
                    }
                }                                               

                List<object> List = new List<object>();
                List<object> List1 = new List<object>();
                List<object> List2 = new List<object>();
                List<object> List3 = new List<object>();
                List<object> List4 = new List<object>();
                List<object> List5 = new List<object>();
                List<object> List6 = new List<object>();
                List<object> List7 = new List<object>();
                List<object> List8 = new List<object>();
                List<object> List9 = new List<object>();
                List<object> List10 = new List<object>();
                List<object> List11 = new List<object>();
                DataSet _DropDownList = new DataSet();

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("RP_Prd_Summary_Getdata", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RCode", RCode);
                        cmd.Parameters.AddWithValue("@FromDate", FromDate);
                        cmd.Parameters.AddWithValue("@ToDate", ToDate);
                        con.Open();
                        //   ErrorLogger.ErrorLogStr("1 Connection Open");
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(_DropDownList);
                        cmd.Dispose();
                    }
                    con.Close();
                    //  ErrorLogger.ErrorLogStr("2 Connection Close");
                }

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

                if (_DropDownList.Tables[4].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[4].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[4].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[4].Columns[j].ToString(), _DropDownList.Tables[4].Rows[i][j].ToString());
                        }
                        List4.Add(Values);
                    }
                }

                if (_DropDownList.Tables[5].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[5].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[5].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[5].Columns[j].ToString(), _DropDownList.Tables[5].Rows[i][j].ToString());
                        }
                        List5.Add(Values);
                    }
                }

                if (_DropDownList.Tables[6].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[6].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[6].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[6].Columns[j].ToString(), _DropDownList.Tables[6].Rows[i][j].ToString());
                        }
                        List6.Add(Values);
                    }
                }

                if (_DropDownList.Tables[7].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[7].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[7].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[7].Columns[j].ToString(), _DropDownList.Tables[7].Rows[i][j].ToString());
                        }
                        List7.Add(Values);
                    }
                }

                if (_DropDownList.Tables[8].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[8].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[8].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[8].Columns[j].ToString(), _DropDownList.Tables[8].Rows[i][j].ToString());
                        }
                        List8.Add(Values);
                    }
                }

                if (_DropDownList.Tables[9].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[9].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[9].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[9].Columns[j].ToString(), _DropDownList.Tables[9].Rows[i][j].ToString());
                        }
                        List9.Add(Values);
                    }
                }

                if (_DropDownList.Tables[10].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[10].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[10].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[10].Columns[j].ToString(), _DropDownList.Tables[10].Rows[i][j].ToString());
                        }
                        List10.Add(Values);
                    }
                }

                if (_DropDownList.Tables[11].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[11].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[11].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[11].Columns[j].ToString(), _DropDownList.Tables[11].Rows[i][j].ToString());
                        }
                        List11.Add(Values);
                    }
                }

                // ErrorLogger.ErrorLogStr("3 List" + List);
                var Data = new { Message = "Data received successfully!", List, List1, List2, List3, List4, List5, List6, List7, List8, List9, List10, List11 };
                return Ok(Data);
            }
            catch (Exception ex)
            {
                // You can log here: ErrorLogger.ErrorLogStr(ex.Message);
                return Ok(new
                {
                    Message = "An error occurred!",
                    Error = ex.Message
                });
            }
        }

        [Route("api/API/API_Slip_Scanning_STN_DailySlip")]
        [BasicAuthentication]//Default
        [HttpGet]
        public IHttpActionResult API_Slip_Scanning_STN_DailySlip(string FromDate, string ToDate)
        {
            try
            {
                var Data = new Api_Response
                {
                    Message = "",
                    List = new List<object>()
                };

                if (string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    Data.Message = "FromDate is required when ToDate is entered!";
                    return Ok(Data);
                }

                if (!string.IsNullOrEmpty(FromDate) && string.IsNullOrEmpty(ToDate))
                {
                    Data.Message = "ToDate is required when FromDate is entered!";
                    return Ok(Data);
                }

                DateTime fromDt, toDt;

                if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    // Try parse FromDate
                    if (!DateTime.TryParse(FromDate, out fromDt))
                    {
                        Data.Message = "Invalid FromDate format!";
                        return Ok(Data);
                    }

                    // Try parse ToDate
                    if (!DateTime.TryParse(ToDate, out toDt))
                    {
                        Data.Message = "Invalid ToDate format!";
                        return Ok(Data);
                    }

                    // --- 4. Check that FromDate <= ToDate ---
                    if (fromDt > toDt)
                    {
                        Data.Message = "FromDate cannot be greater than ToDate!";
                        return Ok(Data);
                    }
                }

                List<object> _list_Slip = new List<object>();
                DataSet _DropDownList = new DataSet();

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("API_Slip_Scanning_STN_DailySlip", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FromDate", FromDate);
                        cmd.Parameters.AddWithValue("@ToDate", ToDate);
                        con.Open();
                        //   ErrorLogger.ErrorLogStr("1 Connection Open");
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(_DropDownList);
                        cmd.Dispose();
                    }
                    con.Close();
                    //  ErrorLogger.ErrorLogStr("2 Connection Close");
                }

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
                Data.Message = "Data received successfully!";
                Data.List = _list_Slip;
                //var Data = new { List = _list_Slip };
                // ErrorLogger.ErrorLogStr("3 List" + List);
                return Ok(Data);
            }
            catch (Exception ex)
            {
                // You can log here: ErrorLogger.ErrorLogStr(ex.Message);
                return Ok(new
                {
                    Message = "An error occurred!",
                    Error = ex.Message
                });
            }
        }

        [Route("api/API/API_Slip_Sawing_Machine_DailySlip")]
        [BasicAuthentication]//Default
        [HttpGet]
        public IHttpActionResult API_Slip_Sawing_Machine_DailySlip(string FromDate, string ToDate)
        {
            try
            {
                var Data = new Api_Response
                {
                    Message = "",
                    List = new List<object>()
                };

                if (string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    Data.Message = "FromDate is required when ToDate is entered!";
                    return Ok(Data);
                }

                if (!string.IsNullOrEmpty(FromDate) && string.IsNullOrEmpty(ToDate))
                {
                    Data.Message = "ToDate is required when FromDate is entered!";
                    return Ok(Data);
                }

                DateTime fromDt, toDt;

                if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    // Try parse FromDate
                    if (!DateTime.TryParse(FromDate, out fromDt))
                    {
                        Data.Message = "Invalid FromDate format!";
                        return Ok(Data);
                    }

                    // Try parse ToDate
                    if (!DateTime.TryParse(ToDate, out toDt))
                    {
                        Data.Message = "Invalid ToDate format!";
                        return Ok(Data);
                    }

                    // --- 4. Check that FromDate <= ToDate ---
                    if (fromDt > toDt)
                    {
                        Data.Message = "FromDate cannot be greater than ToDate!";
                        return Ok(Data);
                    }
                }

                List<object> _list_Slip = new List<object>();
                DataSet _DropDownList = new DataSet();

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("API_Slip_Sawing_Machine_DailySlip", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FromDate", FromDate);
                        cmd.Parameters.AddWithValue("@ToDate", ToDate);
                        con.Open();
                        //   ErrorLogger.ErrorLogStr("1 Connection Open");
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(_DropDownList);
                        cmd.Dispose();
                    }
                    con.Close();
                    //  ErrorLogger.ErrorLogStr("2 Connection Close");
                }

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
                Data.Message = "Data received successfully!";
                Data.List = _list_Slip;
                //var Data = new { List = _list_Slip };
                // ErrorLogger.ErrorLogStr("3 List" + List);
                return Ok(Data);
            }
            catch (Exception ex)
            {
                // You can log here: ErrorLogger.ErrorLogStr(ex.Message);
                return Ok(new
                {
                    Message = "An error occurred!",
                    Error = ex.Message
                });
            }
        }

        [Route("api/API/API_FourP_Getdata_DailySlip")]
        [BasicAuthentication]//Default
        [HttpGet]
        public IHttpActionResult API_FourP_Getdata_DailySlip(string FromDate, string ToDate)
        {
            try
            {
                var Data = new Api_Response
                {
                    Message = "",
                    List = new List<object>()
                };

                if (string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    Data.Message = "FromDate is required when ToDate is entered!";
                    return Ok(Data);
                }

                if (!string.IsNullOrEmpty(FromDate) && string.IsNullOrEmpty(ToDate))
                {
                    Data.Message = "ToDate is required when FromDate is entered!";
                    return Ok(Data);
                }

                DateTime fromDt, toDt;

                if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    // Try parse FromDate
                    if (!DateTime.TryParse(FromDate, out fromDt))
                    {
                        Data.Message = "Invalid FromDate format!";
                        return Ok(Data);
                    }

                    // Try parse ToDate
                    if (!DateTime.TryParse(ToDate, out toDt))
                    {
                        Data.Message = "Invalid ToDate format!";
                        return Ok(Data);
                    }

                    // --- 4. Check that FromDate <= ToDate ---
                    if (fromDt > toDt)
                    {
                        Data.Message = "FromDate cannot be greater than ToDate!";
                        return Ok(Data);
                    }
                }

                List<object> _list_Slip = new List<object>();
                DataSet _DropDownList = new DataSet();

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("API_FourP_Getdata_DailySlip", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FromDate", FromDate);
                        cmd.Parameters.AddWithValue("@ToDate", ToDate);
                        con.Open();
                        //   ErrorLogger.ErrorLogStr("1 Connection Open");
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(_DropDownList);
                        cmd.Dispose();
                    }
                    con.Close();
                    //  ErrorLogger.ErrorLogStr("2 Connection Close");
                }

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
                Data.Message = "Data received successfully!";
                Data.List = _list_Slip;
                //var Data = new { List = _list_Slip };
                // ErrorLogger.ErrorLogStr("3 List" + List);
                return Ok(Data);
            }
            catch (Exception ex)
            {
                // You can log here: ErrorLogger.ErrorLogStr(ex.Message);
                return Ok(new
                {
                    Message = "An error occurred!",
                    Error = ex.Message
                });
            }
        }

        [Route("api/API/API_Sawing_Loss")]
        [BasicAuthentication]//Default
        [HttpGet]
        public IHttpActionResult API_Sawing_Loss(string FromDate, string ToDate)
        {
            try
            {
                var Data = new Api_Response
                {
                    Message = "",
                    List = new List<object>()
                };

                if (string.IsNullOrEmpty(FromDate) && string.IsNullOrEmpty(ToDate))
                {
                    Data.Message = "Date is required!";
                    return Ok(Data);
                }

                if (string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    Data.Message = "FromDate is required when ToDate is entered!";
                    return Ok(Data);
                }

                if (!string.IsNullOrEmpty(FromDate) && string.IsNullOrEmpty(ToDate))
                {
                    Data.Message = "ToDate is required when FromDate is entered!";
                    return Ok(Data);
                }

                DateTime fromDt, toDt;

                if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    // Try parse FromDate
                    if (!DateTime.TryParse(FromDate, out fromDt))
                    {
                        Data.Message = "Invalid FromDate format!";
                        return Ok(Data);
                    }

                    // Try parse ToDate
                    if (!DateTime.TryParse(ToDate, out toDt))
                    {
                        Data.Message = "Invalid ToDate format!";
                        return Ok(Data);
                    }

                    // --- 4. Check that FromDate <= ToDate ---
                    if (fromDt > toDt)
                    {
                        Data.Message = "FromDate cannot be greater than ToDate!";
                        return Ok(Data);
                    }
                }

                List<object> _list_Slip = new List<object>();
                DataSet _DropDownList = new DataSet();

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("API_Sawing_Loss", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FromDate", FromDate);
                        cmd.Parameters.AddWithValue("@ToDate", ToDate);
                        con.Open();
                        //   ErrorLogger.ErrorLogStr("1 Connection Open");
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(_DropDownList);
                        cmd.Dispose();
                    }
                    con.Close();
                    //  ErrorLogger.ErrorLogStr("2 Connection Close");
                }

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
                Data.Message = "Data received successfully!";
                Data.List = _list_Slip;
                //var Data = new { List = _list_Slip };
                // ErrorLogger.ErrorLogStr("3 List" + List);
                return Ok(Data);
            }
            catch (Exception ex)
            {
                // You can log here: ErrorLogger.ErrorLogStr(ex.Message);
                return Ok(new
                {
                    Message = "An error occurred!",
                    Error = ex.Message
                });
            }
        }
    }
}
