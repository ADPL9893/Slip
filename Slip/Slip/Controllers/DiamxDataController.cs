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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using static Slip.Models.Filter.SessionExpireFilter;


namespace Slip.Controllers
{
    public class DiamxDataController : BaseController
    {
        #region :: Available Stock ::

        public ActionResult AvailableStock()
        {
            SetModulePermissions("DiamxData", "AvailableStock");

            bool isAdmin = SessionFacade.UserSession != null && SessionFacade.UserSession.IsAdmin;
            bool canView = isAdmin || (ViewBag.CanAdd == true || ViewBag.CanEdit == true || ViewBag.CanDelete == true);
            bool canAdd = isAdmin || (ViewBag.CanAdd == true);
            bool canExport = isAdmin || (ViewBag.CanAdd == true || ViewBag.CanEdit == true);

            try
            {
                if (SessionFacade.UserSession != null && !isAdmin)
                {
                    List<Dictionary<string, object>> permissionRows = DbHelper.ExecuteReaderAsList("Get_UserModulePermission",
                        new SqlParameter("@UserID", SessionFacade.UserSession.UserID),
                        new SqlParameter("@ControllerName", "DiamxData"),
                        new SqlParameter("@ActionName", "AvailableStock"));

                    if (permissionRows.Count > 0)
                    {
                        Dictionary<string, object> permissionRow = permissionRows[0];
                        foreach (var kvp in permissionRow)
                        {
                            string col = kvp.Key;
                            if (col.Equals("View", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                            {
                                canView = Convert.ToBoolean(kvp.Value);
                            }
                            else if (col.Equals("Add", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                            {
                                canAdd = Convert.ToBoolean(kvp.Value);
                            }
                            else if (col.Equals("Export", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                            {
                                canExport = Convert.ToBoolean(kvp.Value);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
            }

            ViewBag.CanView = canView;
            ViewBag.CanAdd = canAdd;
            ViewBag.CanExport = canExport;
            ViewBag.IsAdmin = isAdmin;

            return View();
        }

        public JsonResult GetData_For_DMX_AvailableStock(string date = null)
        {
            try
            {
                DataSet _DropDownList;
                List<object> _list_Stock = new List<object>();

                if (!string.IsNullOrEmpty(date))
                {
                    _DropDownList = DbHelper.ExecuteDataSet("DMX_AvailableStock_GetAll", new SqlParameter("@CreatedDate", date));
                }
                else
                {
                    _DropDownList = DbHelper.ExecuteDataSet("DMX_AvailableStock_GetAll");
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
                        _list_Stock.Add(Values);
                    }
                }

                var jsonResult = Json(new
                {
                    List = _list_Stock
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

        [HttpPost]
        public JsonResult SyncAvailableStock()
        {
            string Message = "";
            try
            {
                // 1. Call the Diamx API (Token must be in query string for Web API routing to match the action)
                string apiToken = "40f7db5d-c49f-4887-a209-31440f39b7b6";
                string apiUrl = "https://anjalilab.diamx.net/API/StockSearch?APIToken=" + apiToken;

                string jsonBody = "{}";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = 300000; // 5 minutes timeout

                byte[] bodyBytes = Encoding.UTF8.GetBytes(jsonBody);
                request.ContentLength = bodyBytes.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bodyBytes, 0, bodyBytes.Length);
                }

                string responseJson = "";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseJson = reader.ReadToEnd();
                    }
                }

                // 2. Deserialize the API response
                DiamxApiResponse apiResponse = JsonConvert.DeserializeObject<DiamxApiResponse>(responseJson);

                if (apiResponse == null || apiResponse.ApiStatus != "Success" || apiResponse.StoneList == null || apiResponse.StoneList.Count == 0)
                {
                    Message = "No data received from Diamx API.";
                    return Json(new { Message }, JsonRequestBehavior.AllowGet);
                }

                // 3. Convert to XML and insert into database
                XmlDocument Xmldata = CommonMethods.ConvertToXml(apiResponse.StoneList);
                string xmlStr = "<DocumentElement>" + Xmldata.DocumentElement.InnerXml + "</DocumentElement>";

                Message = DbHelper.ExecuteNonQueryWithMessage("DMX_AvailableStock_Insert", 300,
                    new SqlParameter("@XML", SqlDbType.Xml) { Value = xmlStr },
                    new SqlParameter("@UserID", SessionFacade.UserSession.UserID));
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                Message = "ERROR: " + ex.Message;
            }

            return Json(new { Message }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region :: HOLD Stock ::

        public ActionResult HOLDStock()
        {
            SetModulePermissions("DiamxData", "HOLDStock");

            bool isAdmin = SessionFacade.UserSession != null && SessionFacade.UserSession.IsAdmin;
            bool canView = isAdmin || (ViewBag.CanAdd == true || ViewBag.CanEdit == true || ViewBag.CanDelete == true);
            bool canAdd = isAdmin || (ViewBag.CanAdd == true);
            bool canExport = isAdmin || (ViewBag.CanAdd == true || ViewBag.CanEdit == true);

            try
            {
                if (SessionFacade.UserSession != null && !isAdmin)
                {
                    List<Dictionary<string, object>> permissionRows = DbHelper.ExecuteReaderAsList("Get_UserModulePermission",
                        new SqlParameter("@UserID", SessionFacade.UserSession.UserID),
                        new SqlParameter("@ControllerName", "DiamxData"),
                        new SqlParameter("@ActionName", "HOLDStock"));

                    if (permissionRows.Count > 0)
                    {
                        Dictionary<string, object> permissionRow = permissionRows[0];
                        foreach (var kvp in permissionRow)
                        {
                            string col = kvp.Key;
                            if (col.Equals("View", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                            {
                                canView = Convert.ToBoolean(kvp.Value);
                            }
                            else if (col.Equals("Add", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                            {
                                canAdd = Convert.ToBoolean(kvp.Value);
                            }
                            else if (col.Equals("Export", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                            {
                                canExport = Convert.ToBoolean(kvp.Value);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
            }

            ViewBag.CanView = canView;
            ViewBag.CanAdd = canAdd;
            ViewBag.CanExport = canExport;
            ViewBag.IsAdmin = isAdmin;

            return View();
        }

        public JsonResult GetData_For_DMX_HOLDStock(string date = null)
        {
            try
            {
                DataSet _DropDownList;
                List<object> _list_Stock = new List<object>();

                if (!string.IsNullOrEmpty(date))
                {
                    _DropDownList = DbHelper.ExecuteDataSet("DMX_HOLDStock_GetAll", new SqlParameter("@CreatedDate", date));
                }
                else
                {
                    _DropDownList = DbHelper.ExecuteDataSet("DMX_HOLDStock_GetAll");
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
                        _list_Stock.Add(Values);
                    }
                }

                var jsonResult = Json(new
                {
                    List = _list_Stock
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

        [HttpPost]
        public JsonResult SyncHOLDStock()
        {
            string Message = "";
            try
            {
                // 1. Call the Diamx API using POST and payload parameter
                string apiUrl = "https://anjalilab.diamx.net/API/GetTransactionAPI";
                var requestData = new
                {
                    APIToken = "40f7db5d-c49f-4887-a209-31440f39b7b6",
                    FromDate = "",
                    ToDate = "",
                    Branch = "",
                    Type = "HOLD"
                };

                string jsonBody = JsonConvert.SerializeObject(requestData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = 300000; // 5 minutes timeout

                byte[] bodyBytes = Encoding.UTF8.GetBytes(jsonBody);
                request.ContentLength = bodyBytes.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bodyBytes, 0, bodyBytes.Length);
                }

                string responseJson = "";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseJson = reader.ReadToEnd();
                    }
                }

                // 2. Deserialize the API response
                DiamxHoldApiResponse apiResponse = JsonConvert.DeserializeObject<DiamxHoldApiResponse>(responseJson);

                if (apiResponse == null || apiResponse.ApiStatus != "Success" || apiResponse.Transaction_List == null || apiResponse.Transaction_List.Count == 0)
                {
                    Message = "No data received from Diamx API.";
                    return Json(new { Message }, JsonRequestBehavior.AllowGet);
                }

                // 3. Convert to XML and insert into database
                XmlDocument Xmldata = CommonMethods.ConvertToXml(apiResponse.Transaction_List);
                string xmlStr = "<DocumentElement>" + Xmldata.DocumentElement.InnerXml + "</DocumentElement>";

                Message = DbHelper.ExecuteNonQueryWithMessage("DMX_HOLDStock_Insert", 300,
                    new SqlParameter("@XML", SqlDbType.Xml) { Value = xmlStr },
                    new SqlParameter("@UserID", SessionFacade.UserSession.UserID));
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                Message = "ERROR: " + ex.Message;
            }

            return Json(new { Message }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region :: MEMO Stock ::

        public ActionResult MEMOStock()
        {
            SetModulePermissions("DiamxData", "MEMOStock");

            bool isAdmin = SessionFacade.UserSession != null && SessionFacade.UserSession.IsAdmin;
            bool canView = isAdmin || (ViewBag.CanAdd == true || ViewBag.CanEdit == true || ViewBag.CanDelete == true);
            bool canAdd = isAdmin || (ViewBag.CanAdd == true);
            bool canExport = isAdmin || (ViewBag.CanAdd == true || ViewBag.CanEdit == true);

            try
            {
                if (SessionFacade.UserSession != null && !isAdmin)
                {
                    List<Dictionary<string, object>> permissionRows = DbHelper.ExecuteReaderAsList("Get_UserModulePermission",
                        new SqlParameter("@UserID", SessionFacade.UserSession.UserID),
                        new SqlParameter("@ControllerName", "DiamxData"),
                        new SqlParameter("@ActionName", "MEMOStock"));

                    if (permissionRows.Count > 0)
                    {
                        Dictionary<string, object> permissionRow = permissionRows[0];
                        foreach (var kvp in permissionRow)
                        {
                            string col = kvp.Key;
                            if (col.Equals("View", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                            {
                                canView = Convert.ToBoolean(kvp.Value);
                            }
                            else if (col.Equals("Add", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                            {
                                canAdd = Convert.ToBoolean(kvp.Value);
                            }
                            else if (col.Equals("Export", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                            {
                                canExport = Convert.ToBoolean(kvp.Value);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
            }

            ViewBag.CanView = canView;
            ViewBag.CanAdd = canAdd;
            ViewBag.CanExport = canExport;
            ViewBag.IsAdmin = isAdmin;

            return View();
        }

        public JsonResult GetData_For_DMX_MEMOStock(string date = null)
        {
            try
            {
                DataSet _DropDownList;
                List<object> _list_Stock = new List<object>();

                if (!string.IsNullOrEmpty(date))
                {
                    _DropDownList = DbHelper.ExecuteDataSet("DMX_MEMOStock_GetAll", new SqlParameter("@CreatedDate", date));
                }
                else
                {
                    _DropDownList = DbHelper.ExecuteDataSet("DMX_MEMOStock_GetAll");
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
                        _list_Stock.Add(Values);
                    }
                }

                var jsonResult = Json(new
                {
                    List = _list_Stock
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

        [HttpPost]
        public JsonResult SyncMEMOStock()
        {
            string Message = "";
            try
            {
                // 1. Call the Diamx API using POST and payload parameter
                string apiUrl = "https://anjalilab.diamx.net/API/GetTransactionAPI";
                var requestData = new
                {
                    APIToken = "40f7db5d-c49f-4887-a209-31440f39b7b6",
                    FromDate = "",
                    ToDate = "",
                    Branch = "",
                    Type = "MEMO"
                };

                string jsonBody = JsonConvert.SerializeObject(requestData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = 300000; // 5 minutes timeout

                byte[] bodyBytes = Encoding.UTF8.GetBytes(jsonBody);
                request.ContentLength = bodyBytes.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bodyBytes, 0, bodyBytes.Length);
                }

                string responseJson = "";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseJson = reader.ReadToEnd();
                    }
                }

                // 2. Deserialize the API response
                DiamxMemoApiResponse apiResponse = JsonConvert.DeserializeObject<DiamxMemoApiResponse>(responseJson);

                if (apiResponse == null || apiResponse.ApiStatus != "Success" || apiResponse.Transaction_List == null || apiResponse.Transaction_List.Count == 0)
                {
                    Message = "No data received from Diamx API.";
                    return Json(new { Message }, JsonRequestBehavior.AllowGet);
                }

                // 3. Convert to XML and insert into database
                XmlDocument Xmldata = CommonMethods.ConvertToXml(apiResponse.Transaction_List);
                string xmlStr = "<DocumentElement>" + Xmldata.DocumentElement.InnerXml + "</DocumentElement>";

                Message = DbHelper.ExecuteNonQueryWithMessage("DMX_MEMOStock_Insert", 300,
                    new SqlParameter("@XML", SqlDbType.Xml) { Value = xmlStr },
                    new SqlParameter("@UserID", SessionFacade.UserSession.UserID));
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                Message = "ERROR: " + ex.Message;
            }

            return Json(new { Message }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region :: SALES Stock ::

        public ActionResult SALESStock()
        {
            SetModulePermissions("DiamxData", "SALESStock");

            bool isAdmin = SessionFacade.UserSession != null && SessionFacade.UserSession.IsAdmin;
            bool canView = isAdmin || (ViewBag.CanAdd == true || ViewBag.CanEdit == true || ViewBag.CanDelete == true);
            bool canAdd = isAdmin || (ViewBag.CanAdd == true);
            bool canExport = isAdmin || (ViewBag.CanAdd == true || ViewBag.CanEdit == true);

            try
            {
                if (SessionFacade.UserSession != null && !isAdmin)
                {
                    List<Dictionary<string, object>> permissionRows = DbHelper.ExecuteReaderAsList("Get_UserModulePermission",
                        new SqlParameter("@UserID", SessionFacade.UserSession.UserID),
                        new SqlParameter("@ControllerName", "DiamxData"),
                        new SqlParameter("@ActionName", "SALESStock"));

                    if (permissionRows.Count > 0)
                    {
                        Dictionary<string, object> permissionRow = permissionRows[0];
                        foreach (var kvp in permissionRow)
                        {
                            string col = kvp.Key;
                            if (col.Equals("View", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                            {
                                canView = Convert.ToBoolean(kvp.Value);
                            }
                            else if (col.Equals("Add", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                            {
                                canAdd = Convert.ToBoolean(kvp.Value);
                            }
                            else if (col.Equals("Export", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                            {
                                canExport = Convert.ToBoolean(kvp.Value);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
            }

            ViewBag.CanView = canView;
            ViewBag.CanAdd = canAdd;
            ViewBag.CanExport = canExport;
            ViewBag.IsAdmin = isAdmin;

            return View();
        }

        public JsonResult GetData_For_DMX_SALESStock(string date = null)
        {
            try
            {
                DataSet _DropDownList;
                List<object> _list_Stock = new List<object>();

                if (!string.IsNullOrEmpty(date))
                {
                    _DropDownList = DbHelper.ExecuteDataSet("DMX_SALESStock_GetAll", new SqlParameter("@CreatedDate", date));
                }
                else
                {
                    _DropDownList = DbHelper.ExecuteDataSet("DMX_SALESStock_GetAll");
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
                        _list_Stock.Add(Values);
                    }
                }

                var jsonResult = Json(new
                {
                    List = _list_Stock
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

        [HttpPost]
        public JsonResult SyncSALESStock(string fromDate = "", string toDate = "")
        {
            string Message = "";
            try
            {
                // 1. Call the Diamx API using POST and payload parameter
                string apiUrl = "https://anjalilab.diamx.net/API/GetTransactionAPI";
                var requestData = new
                {
                    APIToken = "40f7db5d-c49f-4887-a209-31440f39b7b6",
                    FromDate = fromDate,
                    ToDate = toDate,
                    Branch = "",
                    Type = "SALEINVOICE"
                };

                string jsonBody = JsonConvert.SerializeObject(requestData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = 300000; // 5 minutes timeout

                byte[] bodyBytes = Encoding.UTF8.GetBytes(jsonBody);
                request.ContentLength = bodyBytes.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bodyBytes, 0, bodyBytes.Length);
                }

                string responseJson = "";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseJson = reader.ReadToEnd();
                    }
                }

                // 2. Deserialize the API response
                DiamxSalesApiResponse apiResponse = JsonConvert.DeserializeObject<DiamxSalesApiResponse>(responseJson);

                if (apiResponse == null || apiResponse.ApiStatus != "Success" || apiResponse.Transaction_List == null || apiResponse.Transaction_List.Count == 0)
                {
                    Message = "No data received from Diamx API.";
                    return Json(new { Message }, JsonRequestBehavior.AllowGet);
                }

                // 3. Convert to XML and insert into database
                XmlDocument Xmldata = CommonMethods.ConvertToXml(apiResponse.Transaction_List);
                string xmlStr = "<DocumentElement>" + Xmldata.DocumentElement.InnerXml + "</DocumentElement>";

                Message = DbHelper.ExecuteNonQueryWithMessage("DMX_SALESStock_Insert", 300,
                    new SqlParameter("@XML", SqlDbType.Xml) { Value = xmlStr },
                    new SqlParameter("@UserID", SessionFacade.UserSession.UserID));
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                Message = "ERROR: " + ex.Message;
            }

            return Json(new { Message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportSalesStockSummary(string date)
        {
            try
            {
                DataSet ds;
                List<SalesSummaryReportItem> dataList = new List<SalesSummaryReportItem>();
                List<SizeMasterItem> sizeList = new List<SizeMasterItem>();

                if (!string.IsNullOrEmpty(date))
                {
                    ds = DbHelper.ExecuteDataSet("DMX_SALESStock_SummaryReport", new SqlParameter("@CreatedDate", date));
                }
                else
                {
                    ds = DbHelper.ExecuteDataSet("DMX_SALESStock_SummaryReport", new SqlParameter("@CreatedDate", DBNull.Value));
                }

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    dataList = CommonMethods.ConvertDataTable<SalesSummaryReportItem>(ds.Tables[0]);
                }
                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    sizeList = CommonMethods.ConvertDataTable<SizeMasterItem>(ds.Tables[1]);
                }

                string displayDate = date;
                try
                {
                    if (DateTime.TryParse(date, out DateTime dt))
                    {
                        displayDate = dt.ToString("dd-MM-yyyy");
                    }
                }
                catch { }

                string Path = ExcelExport.SalesStockSummaryExport(dataList, sizeList, displayDate);

                if (!string.IsNullOrEmpty(Path))
                {
                    string downloadFileName = $"SALESStockSummary_{displayDate}.xlsx";
                    return File(Path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", downloadFileName);
                }
                else
                {
                    return Content("Error generating Excel report.");
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Content("Error: " + ex.Message);
            }
        }

        public ActionResult ExportSalesStockSellerWise(string date)
        {
            try
            {
                DataSet ds;
                List<SalesSellerWiseReportItem> dataList = new List<SalesSellerWiseReportItem>();

                if (!string.IsNullOrEmpty(date))
                {
                    ds = DbHelper.ExecuteDataSet("DMX_SALESStock_SellerWiseReport", new SqlParameter("@CreatedDate", date));
                }
                else
                {
                    ds = DbHelper.ExecuteDataSet("DMX_SALESStock_SellerWiseReport", new SqlParameter("@CreatedDate", DBNull.Value));
                }

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    dataList = CommonMethods.ConvertDataTable<SalesSellerWiseReportItem>(ds.Tables[0]);
                }

                string displayDate = date;
                try
                {
                    if (DateTime.TryParse(date, out DateTime dt))
                    {
                        displayDate = dt.ToString("dd-MM-yyyy");
                    }
                }
                catch { }

                string Path = ExcelExport.SalesStockSellerWiseExport(dataList, displayDate);

                if (!string.IsNullOrEmpty(Path))
                {
                    string downloadFileName = $"SALESSellerWiseReport_{displayDate}.xlsx";
                    return File(Path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", downloadFileName);
                }
                else
                {
                    return Content("Error generating Excel report.");
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Content("Error: " + ex.Message);
            }
        }

        #endregion


        public ActionResult ExportAvailableStockSummary(string date)
        {
            try
            {
                DataSet _DropDownList = new DataSet();
                List<DmxStockSummary_Size> List1 = new List<DmxStockSummary_Size>();
                List<DmxStockSummary_Color> List2 = new List<DmxStockSummary_Color>();
                List<DmxStockSummary_Clarity> List3 = new List<DmxStockSummary_Clarity>();
                List<DmxStockSummary_Pivot> List4 = new List<DmxStockSummary_Pivot>();
                System.Data.DataTable dtReportDays = new System.Data.DataTable();
                System.Data.DataTable dtStatus = new System.Data.DataTable();

                string sqlDateStr = null;
                if (!string.IsNullOrEmpty(date) && date != "NaN-NaN-NaN")
                {
                    DateTime parsedDate;
                    if (DateTime.TryParseExact(date, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDate))
                    {
                        sqlDateStr = parsedDate.ToString("yyyy-MM-dd");
                    }
                    else if (DateTime.TryParse(date, out parsedDate))
                    {
                        sqlDateStr = parsedDate.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        sqlDateStr = date; // Fallback
                    }
                }

                if (!string.IsNullOrEmpty(sqlDateStr))
                {
                    _DropDownList = DbHelper.ExecuteDataSet("DMX_AvailableStock_SummaryReport", new SqlParameter("@CreatedDate", sqlDateStr));
                }
                else
                {
                    _DropDownList = DbHelper.ExecuteDataSet("DMX_AvailableStock_SummaryReport", new SqlParameter("@CreatedDate", DBNull.Value));
                }

                if (_DropDownList.Tables.Count >= 6)
                {
                    List1 = CommonMethods.ConvertDataTable<DmxStockSummary_Size>(_DropDownList.Tables[0]);
                    List2 = CommonMethods.ConvertDataTable<DmxStockSummary_Color>(_DropDownList.Tables[1]);
                    List3 = CommonMethods.ConvertDataTable<DmxStockSummary_Clarity>(_DropDownList.Tables[2]);
                    List4 = CommonMethods.ConvertDataTable<DmxStockSummary_Pivot>(_DropDownList.Tables[3]);
                    dtReportDays = _DropDownList.Tables[4];
                    dtStatus = _DropDownList.Tables[5];
                }
                else if (_DropDownList.Tables.Count >= 4)
                {
                    List1 = CommonMethods.ConvertDataTable<DmxStockSummary_Size>(_DropDownList.Tables[0]);
                    List2 = CommonMethods.ConvertDataTable<DmxStockSummary_Color>(_DropDownList.Tables[1]);
                    List3 = CommonMethods.ConvertDataTable<DmxStockSummary_Clarity>(_DropDownList.Tables[2]);
                    List4 = CommonMethods.ConvertDataTable<DmxStockSummary_Pivot>(_DropDownList.Tables[3]);
                }

                List<DMX_AvailableStock> rawStock = new List<DMX_AvailableStock>();
                List<SizeMasterItem> sizeList = new List<SizeMasterItem>();

                using (SqlConnection con = new SqlConnection(conn))
                {
                    con.Open();

                    // Query 1: raw stock details
                    string rawStockQuery = @"
                        SELECT Shape, Weight, Color, Clarity, Cut, Polish, Symm, SaleAmt, Lab, Location, StockStatus
                        FROM dbo.DMX_AvailableStock
                        WHERE IsActive = 1 and Color is not null";
                    
                    if (!string.IsNullOrEmpty(sqlDateStr))
                    {
                        rawStockQuery += " AND CAST(Created AS DATE) = @CreatedDate";
                    }

                    using (SqlCommand cmdRaw = new SqlCommand(rawStockQuery, con))
                    {
                        if (!string.IsNullOrEmpty(sqlDateStr))
                        {
                            cmdRaw.Parameters.AddWithValue("@CreatedDate", sqlDateStr);
                        }

                        using (SqlDataReader reader = cmdRaw.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                rawStock.Add(new DMX_AvailableStock
                                {
                                    Shape = reader["Shape"] != DBNull.Value ? Convert.ToString(reader["Shape"]) : "",
                                    Weight = reader["Weight"] != DBNull.Value ? Convert.ToString(reader["Weight"]) : "0",
                                    Color = reader["Color"] != DBNull.Value ? Convert.ToString(reader["Color"]) : "",
                                    Clarity = reader["Clarity"] != DBNull.Value ? Convert.ToString(reader["Clarity"]) : "",
                                    Cut = reader["Cut"] != DBNull.Value ? Convert.ToString(reader["Cut"]) : "",
                                    Polish = reader["Polish"] != DBNull.Value ? Convert.ToString(reader["Polish"]) : "",
                                    Symm = reader["Symm"] != DBNull.Value ? Convert.ToString(reader["Symm"]) : "",
                                    SaleAmt = reader["SaleAmt"] != DBNull.Value ? Convert.ToString(reader["SaleAmt"]) : "0",
                                    Lab = reader["Lab"] != DBNull.Value ? Convert.ToString(reader["Lab"]) : "",
                                    Location = reader["Location"] != DBNull.Value ? Convert.ToString(reader["Location"]) : "",
                                    StockStatus = reader["StockStatus"] != DBNull.Value ? Convert.ToString(reader["StockStatus"]) : ""
                                });
                            }
                        }
                    }

                    // Query 2: size master items
                    string sizeQuery = @"
                        SELECT FromSize, ToSize, Sequence
                        FROM dbo.MST_Size_DMX
                        WHERE IsActive = 1
                        ORDER BY FromSize";

                    using (SqlCommand cmdSize = new SqlCommand(sizeQuery, con))
                    {
                        using (SqlDataReader reader = cmdSize.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                sizeList.Add(new SizeMasterItem
                                {
                                    FromSize = reader["FromSize"] != DBNull.Value ? Convert.ToDecimal(reader["FromSize"]) : 0m,
                                    ToSize = reader["ToSize"] != DBNull.Value ? Convert.ToDecimal(reader["ToSize"]) : 0m,
                                    Sequence = reader["Sequence"] != DBNull.Value ? Convert.ToInt32(reader["Sequence"]) : 0
                                });
                            }
                        }
                    }

                    con.Close();
                }

                string HeaderName = "ROUND 3EX-IGI-COLOR,CLARITY,SIZE,WEIGHT,TOTAL $ PERCENTAGE WISE REPORT";
                
                string displayDate = DateTime.Now.ToString("dd-MM-yyyy");
                try
                {
                    if (!string.IsNullOrEmpty(date) && date != "NaN-NaN-NaN" && DateTime.TryParse(date, out DateTime parsedDate))
                    {
                        displayDate = parsedDate.ToString("dd-MM-yyyy");
                    }
                }
                catch { }

                string Path = ExcelExport.AvailableStockSummaryExport(List1, List2, List3, List4, HeaderName, rawStock, sizeList, displayDate, dtReportDays, dtStatus);

                if (!string.IsNullOrEmpty(Path))
                {
                    return File(Path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AvailableStockSummary.xlsx");
                }
                else
                {
                    return Content("Error generating Excel file.");
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Content("Error: " + ex.Message);
            }
        }
        public ActionResult ExportAVLStockWithSales(string date)
        {
            try
            {
                List<DmxStockAndSaleSummary> list = new List<DmxStockAndSaleSummary>();

                List<Dictionary<string, object>> avlStockWithSalesRows;
                if (!string.IsNullOrEmpty(date) && date != "NaN-NaN-NaN")
                {
                    avlStockWithSalesRows = DbHelper.ExecuteReaderAsList("DMX_StockAndSalesReport_Export", new SqlParameter("@ReportDate", date));
                }
                else
                {
                    avlStockWithSalesRows = DbHelper.ExecuteReaderAsList("DMX_StockAndSalesReport_Export", new SqlParameter("@ReportDate", DBNull.Value));
                }

                foreach (var row in avlStockWithSalesRows)
                {
                    list.Add(new DmxStockAndSaleSummary
                    {
                        Location = row["Location"] != null ? Convert.ToString(row["Location"]) : "",
                        SizeBucket = row["SizeBucket"] != null ? Convert.ToString(row["SizeBucket"]) : "",
                        Color = row["Color"] != null ? Convert.ToString(row["Color"]) : "",
                        Clarity = row["Clarity"] != null ? Convert.ToString(row["Clarity"]) : "",
                        StockPcs = row["StockPcs"] != null ? Convert.ToInt32(row["StockPcs"]) : 0,
                        SalePcs = row["SalePcs"] != null ? Convert.ToInt32(row["SalePcs"]) : 0
                    });
                }

                string displayDate = DateTime.Now.ToString("dd-MM-yyyy");
                if (!string.IsNullOrEmpty(date) && date != "NaN-NaN-NaN" && DateTime.TryParse(date, out DateTime parsedDate))
                {
                    displayDate = parsedDate.ToString("dd-MM-yyyy");
                }

                string path = ExcelExport.AVLStockWithSalesExport(list, displayDate);

                if (!string.IsNullOrEmpty(path))
                {
                    return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"AVL_Stock_Sales_{displayDate}.xlsx");
                }
                else
                {
                    return Content("Error generating Excel file.");
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Content("Error: " + ex.Message);
            }
        }

        public ActionResult ExportMemoHoldSummary(string date)
        {
            try
            {
                DataSet _DropDownList;
                List<DmxMemoHoldSummaryItem> List = new List<DmxMemoHoldSummaryItem>();

                if (!string.IsNullOrEmpty(date) && date != "NaN-NaN-NaN")
                {
                    _DropDownList = DbHelper.ExecuteDataSet("DMX_MemoHold_SummaryReport", new SqlParameter("@CreatedDate", date));
                }
                else
                {
                    _DropDownList = DbHelper.ExecuteDataSet("DMX_MemoHold_SummaryReport", new SqlParameter("@CreatedDate", DBNull.Value));
                }

                if (_DropDownList.Tables.Count > 0)
                {
                    List = CommonMethods.ConvertDataTable<DmxMemoHoldSummaryItem>(_DropDownList.Tables[0]);
                }

                string Path = ExcelExport.MemoHoldSummaryExport(List, date);

                if (!string.IsNullOrEmpty(Path))
                {
                    return File(Path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MemoHoldSummary.xlsx");
                }
                else
                {
                    return Content("Error generating Excel file.");
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Content("Error: " + ex.Message);
            }
        }
    }
}