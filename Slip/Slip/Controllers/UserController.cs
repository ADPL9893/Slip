using Slip.Models;
using Slip.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using static Slip.Models.Filter.SessionExpireFilter;

namespace Slip.Controllers
{
    public class UserController : BaseController
    {
        // GET: User
        public ActionResult Index(int id)
        {
            ViewBag.UserID = id;
            return View();
        }
        public ActionResult UserDetail()
        {
            return View();
        }
        public ActionResult ChangePassword()
        {
            return View();
        }
        public JsonResult Get_SYS_ModuleDataList()
        {
            try
            {
                List<object> _list_SYS_Module = new List<object>();
                List<object> _list_SYS_Module_Group = new List<object>();
                List<object> SYSModuleList = new List<object>();
                DataSet _DropDownList = DbHelper.ExecuteDataSet("Get_SYS_ModuleDataList");
                if (_DropDownList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[0].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[0].Columns[j].ToString(), _DropDownList.Tables[0].Rows[i][j].ToString());
                        }
                        _list_SYS_Module.Add(Values);
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
                        _list_SYS_Module_Group.Add(Values);
                    }
                }
                var jsonResult = Json(new
                {

                    SYSModuleList = _list_SYS_Module,
                    SYSModuleGroupList = _list_SYS_Module_Group

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
        public JsonResult SEC_UserPrivileges_Getdata()
        {
            try
            {
                List<object> _list_SYS_Module = new List<object>();

                List<object> SYSModuleList = new List<object>();

                DataSet _DropDownList = DbHelper.ExecuteDataSet("SEC_UserPrivileges_Getdata");

                if (_DropDownList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[0].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[0].Columns[j].ToString(), _DropDownList.Tables[0].Rows[i][j].ToString());
                        }
                        _list_SYS_Module.Add(Values);
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
                    @fixed = row.Field<bool>("fixed"),
                })
                .ToList();

                var jsonResult = Json(new
                {
                    SYSModuleList = _list_SYS_Module,
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
        public JsonResult Get_SEC_UserPrivileges_Edit(int UserID)
        {
            List<object> _SEC_User_List = new List<object>();
            List<object> _SEC_UserPrivileges_List = new List<object>(); try
            {
                List<object> _list_SYS_Module = new List<object>();
                List<object> SYSModuleList = new List<object>();
                DataSet _PartyEditData = DbHelper.ExecuteDataSet("Get_SEC_UserPrivileges_Edit",
                    new SqlParameter("@UserID", SqlDbType.Int) { Value = UserID });

                for (int i = 0; i < _PartyEditData.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, string> Values = new Dictionary<string, string>();
                    for (int j = 0; j < _PartyEditData.Tables[0].Columns.Count; j++)
                    {
                        Values.Add(_PartyEditData.Tables[0].Columns[j].ToString(), _PartyEditData.Tables[0].Rows[i][j].ToString());
                    }
                    _SEC_User_List.Add(Values);
                }
                for (int i = 0; i < _PartyEditData.Tables[1].Rows.Count; i++)
                {
                    Dictionary<string, string> Values = new Dictionary<string, string>();
                    for (int j = 0; j < _PartyEditData.Tables[1].Columns.Count; j++)
                    {
                        Values.Add(_PartyEditData.Tables[1].Columns[j].ToString(), _PartyEditData.Tables[1].Rows[i][j].ToString());
                    }
                    _SEC_UserPrivileges_List.Add(Values);
                }
                var jsonResult = Json(new
                {
                    List = _SEC_User_List,
                    SEC_UserPrivileges = _SEC_UserPrivileges_List

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
                return jsonResult;
            }
        }
        public string SEC_UserPrivileges_Insert_Update_Delete(SEC_User model, List<SEC_UserPrivileges> ImportTerms, string Action)
        {
            string Message = "";
            try
            {
                model.CreatedByUserID = SessionFacade.UserSession.UserID;
                #region Import Terms
                List<SEC_UserPrivileges> _TermsNew = new List<SEC_UserPrivileges>();
                if (ImportTerms != null)
                {
                    for (int j = 0; j < ImportTerms.Count; j++)
                    {
                        //if (ImportTerms[i].OrderedQuantity > 0)
                        ImportTerms[j].CreatedByUserID = Convert.ToInt32(SessionFacade.UserSession.UserID);
                        _TermsNew.Add(ImportTerms[j]);
                    }
                }
                #endregion

                #region Creating XMl
                string xml = "";
                string XML_Detail = "";
                XmlDocument Xmldata = CommonMethods.ConvertToXml(model);
                xml = "<DocumentElement><SEC_User>" + Xmldata.DocumentElement.InnerXml + "</SEC_User></DocumentElement>";

                if (_TermsNew.Count > 0)
                {
                    XmlDocument Xmldata_Terms = CommonMethods.ConvertToXml(_TermsNew);
                    XML_Detail = "<DocumentElement>" + Xmldata_Terms.DocumentElement.InnerXml + "</DocumentElement>";
                }
                #endregion
                //  ErrorLogger.ErrorLogStr("XML Create" + xml);
                //   ErrorLogger.ErrorLogStr("XML_Detail Create" + XML_Detail);
                Message = DbHelper.ExecuteNonQueryWithMessage("SEC_UserPrivileges_Insert_Update_Delete",
                    new SqlParameter("@XML", xml),
                    new SqlParameter("@XML_Detail", XML_Detail),
                    new SqlParameter("@ACTION", "UPDATE"));
                //  ErrorLogger.ErrorLogStr("Last" + Message);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return "ERROR : " + ex.InnerException.Message;
            }
            return Message;

        }
        public string ChangeThePasswordForUser(string UserName, string Password, string Action)
        {
            string Message = "";
            try
            {
                Message = DbHelper.ExecuteNonQueryWithMessage("ChangeThePasswordForUser",
                    new SqlParameter("@UserName", UserName),
                    new SqlParameter("@Password", Password),
                    new SqlParameter("@UserID", SessionFacade.UserSession.UserID),
                    new SqlParameter("@ACTION", Action));
                //   ErrorLogger.ErrorLogStr("Last" + Message);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return "ERROR : " + ex.InnerException.Message;
            }
            return Message;

        }
        public JsonResult Get_UserList()
        {

            try
            {
                List<object> _list_User = new List<object>();
                List<object> _list_Process = new List<object>();
                List<object> _list_Table = new List<object>();

                List<object> UserList = new List<object>();
                List<object> ProcessList = new List<object>();

                DataSet _DropDownList = DbHelper.ExecuteDataSet("Get_UserList");
                if (_DropDownList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[0].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[0].Columns[j].ToString(), _DropDownList.Tables[0].Rows[i][j].ToString());
                        }
                        _list_User.Add(Values);
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
                        _list_Process.Add(Values);
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
                        _list_Table.Add(Values);
                    }
                }
                var jsonResult = Json(new
                {
                    UserList = _list_User,
                    ProcessList = _list_Process,
                    TableList = _list_Table

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
        public ActionResult UserPermission(int? id, int? UserID)
        {
            int uid = id ?? UserID ?? 0;
            ViewBag.UserID = uid;
            string targetUserName = "";
            if (uid > 0)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(conn))
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT UserName FROM SEC_User WHERE UserID = @UserID", con))
                        {
                            cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = uid;
                            con.Open();
                            object obj = cmd.ExecuteScalar();
                            if (obj != null && obj != DBNull.Value)
                            {
                                targetUserName = obj.ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.ErrorLog(ex);
                }
            }
            ViewBag.TargetUserName = targetUserName;
            return View();
        }

        #region User Permission New Isolated Methods

        public JsonResult Get_PermissionGridData(int UserID)
        {
            try
            {
                List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
                var rows = DbHelper.ExecuteReaderAsList("SEC_UserPermission_Manage",
                    new SqlParameter("@Action", SqlDbType.VarChar, 50) { Value = "GET_PERMISSIONS" },
                    new SqlParameter("@TargetUserID", SqlDbType.Int) { Value = UserID });

                foreach (var row in rows)
                {
                    var dict = new Dictionary<string, object>();
                    dict["GroupName"] = row["GroupName"] != null ? row["GroupName"].ToString() : "General";
                    dict["ModuleID"] = row["ModuleID"] != null ? Convert.ToInt32(row["ModuleID"]) : 0;
                    dict["ModuleName"] = row["ModuleName"] != null ? row["ModuleName"].ToString() : "";
                    dict["SelectAll"] = row["SelectAll"] != null && (Convert.ToBoolean(row["SelectAll"]) || Convert.ToString(row["SelectAll"]) == "1" || Convert.ToString(row["SelectAll"]).ToLower() == "true");
                    dict["View"] = row["View"] != null && (Convert.ToBoolean(row["View"]) || Convert.ToString(row["View"]) == "1" || Convert.ToString(row["View"]).ToLower() == "true");
                    dict["Add"] = row["Add"] != null && (Convert.ToBoolean(row["Add"]) || Convert.ToString(row["Add"]) == "1" || Convert.ToString(row["Add"]).ToLower() == "true");
                    dict["Edit"] = row["Edit"] != null && (Convert.ToBoolean(row["Edit"]) || Convert.ToString(row["Edit"]) == "1" || Convert.ToString(row["Edit"]).ToLower() == "true");
                    dict["Delete"] = row["Delete"] != null && (Convert.ToBoolean(row["Delete"]) || Convert.ToString(row["Delete"]) == "1" || Convert.ToString(row["Delete"]).ToLower() == "true");
                    dict["Export"] = row["Export"] != null && (Convert.ToBoolean(row["Export"]) || Convert.ToString(row["Export"]) == "1" || Convert.ToString(row["Export"]).ToLower() == "true");
                    dict["Mail"] = row["Mail"] != null && (Convert.ToBoolean(row["Mail"]) || Convert.ToString(row["Mail"]) == "1" || Convert.ToString(row["Mail"]).ToLower() == "true");
                    dict["Print"] = row["Print"] != null && (Convert.ToBoolean(row["Print"]) || Convert.ToString(row["Print"]) == "1" || Convert.ToString(row["Print"]).ToLower() == "true");
                    dict["History"] = row["History"] != null && (Convert.ToBoolean(row["History"]) || Convert.ToString(row["History"]) == "1" || Convert.ToString(row["History"]).ToLower() == "true");
                    list.Add(dict);
                }

                var jsonResult = Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                var jsonResult = Json(new { success = false, message = ex.Message, data = new List<object>() }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
        }

        public JsonResult Get_SameAs_UserList(int UserID)
        {
            try
            {
                List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
                var rows = DbHelper.ExecuteReaderAsList("SEC_UserPermission_Manage",
                    new SqlParameter("@Action", SqlDbType.VarChar, 50) { Value = "GET_USERS" },
                    new SqlParameter("@TargetUserID", SqlDbType.Int) { Value = UserID });

                foreach (var row in rows)
                {
                    var dict = new Dictionary<string, object>();
                    dict["ID"] = row["ID"] != null ? Convert.ToInt32(row["ID"]) : 0;
                    dict["UserName"] = row["UserName"] != null ? row["UserName"].ToString() : "";
                    list.Add(dict);
                }

                var jsonResult = Json(new { success = true, list = list }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                var jsonResult = Json(new { success = false, message = ex.Message, list = new List<object>() }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
        }

        [HttpPost]
        public JsonResult CopyPermissions(int SourceUserID, int TargetUserID)
        {
            try
            {
                int createdBy = SessionFacade.UserSession != null ? SessionFacade.UserSession.UserID : 0;

                string message = DbHelper.ExecuteNonQueryWithMessage("SEC_UserPermission_Manage",
                    new SqlParameter("@Action", SqlDbType.VarChar, 50) { Value = "COPY_PERMISSIONS" },
                    new SqlParameter("@SourceUserID", SqlDbType.Int) { Value = SourceUserID },
                    new SqlParameter("@TargetUserID", SqlDbType.Int) { Value = TargetUserID },
                    new SqlParameter("@CreatedBy", SqlDbType.Int) { Value = createdBy });

                bool isSuccess = !string.IsNullOrEmpty(message) && !message.StartsWith("ERROR", StringComparison.OrdinalIgnoreCase);
                return Json(new { success = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Save_UserPermissions(int TargetUserID, List<SEC_UserPrivileges> permissions)
        {
            string message = "";
            try
            {
                if (TargetUserID <= 0)
                {
                    return Json(new { success = false, message = "Invalid Target User." });
                }

                int createdBy = SessionFacade.UserSession != null ? SessionFacade.UserSession.UserID : 0;

                List<SEC_UserPrivileges> _termsNew = new List<SEC_UserPrivileges>();
                if (permissions != null)
                {
                    foreach (var item in permissions)
                    {
                        item.UserID = TargetUserID;
                        item.CreatedByUserID = createdBy;
                        _termsNew.Add(item);
                    }
                }

                string xmlDetail = "";
                if (_termsNew.Count > 0)
                {
                    XmlDocument xmlDataDetails = CommonMethods.ConvertToXml(_termsNew);
                    xmlDetail = "<DocumentElement>" + xmlDataDetails.DocumentElement.InnerXml + "</DocumentElement>";
                }

                message = DbHelper.ExecuteNonQueryWithMessage("SEC_UserPrivileges_Insert_Update_Delete",
                    new SqlParameter("@XML", "<DocumentElement></DocumentElement>"),
                    new SqlParameter("@XML_Detail", xmlDetail),
                    new SqlParameter("@ACTION", "UPDATE"));

                bool isSuccess = !string.IsNullOrEmpty(message) && !message.StartsWith("ERROR", StringComparison.OrdinalIgnoreCase);
                return Json(new { success = isSuccess, message = message });
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = "ERROR: " + ex.Message });
            }
        }

        //[HttpPost]
        //public JsonResult Save_UserPermissions(int TargetUserID, List<SEC_UserPrivileges> permissions)
        //{
        //    try
        //    {
        //        if (TargetUserID <= 0)
        //        {
        //            return Json(new { success = false, message = "Invalid Target User." });
        //        }

        //        int createdBy = SessionFacade.UserSession != null ? SessionFacade.UserSession.UserID : 0;

        //        using (SqlConnection con = new SqlConnection(conn))
        //        {
        //            con.Open();
        //            using (SqlTransaction tran = con.BeginTransaction())
        //            {
        //                try
        //                {
        //                    using (SqlCommand delCmd = new SqlCommand("DELETE FROM SEC_UserPrivileges WHERE UserID = @TargetUserID", con, tran))
        //                    {
        //                        delCmd.Parameters.Add("@TargetUserID", SqlDbType.Int).Value = TargetUserID;
        //                        delCmd.ExecuteNonQuery();
        //                    }

        //                    if (permissions != null && permissions.Count > 0)
        //                    {
        //                        string insertSql = @"INSERT INTO SEC_UserPrivileges 
        //                            (ModuleID, UserID, CreatedByUserID, Created, Modified, [View], [Add], [Edit], [Delete], [Export], [Mail], [Print], [History])
        //                            VALUES 
        //                            (@ModuleID, @UserID, @CreatedBy, GETDATE(), GETDATE(), @View, @Add, @Edit, @Delete, @Export, @Mail, @Print, @History)";

        //                        foreach (var item in permissions)
        //                        {
        //                            using (SqlCommand insCmd = new SqlCommand(insertSql, con, tran))
        //                            {
        //                                insCmd.Parameters.Add("@ModuleID", SqlDbType.Int).Value = item.ModuleID;
        //                                insCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = TargetUserID;
        //                                insCmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = createdBy;
        //                                insCmd.Parameters.Add("@View", SqlDbType.Bit).Value = item.View;
        //                                insCmd.Parameters.Add("@Add", SqlDbType.Bit).Value = item.Add;
        //                                insCmd.Parameters.Add("@Edit", SqlDbType.Bit).Value = item.Edit;
        //                                insCmd.Parameters.Add("@Delete", SqlDbType.Bit).Value = item.Delete;
        //                                insCmd.Parameters.Add("@Export", SqlDbType.Bit).Value = item.Export;
        //                                insCmd.Parameters.Add("@Mail", SqlDbType.Bit).Value = item.Mail;
        //                                insCmd.Parameters.Add("@Print", SqlDbType.Bit).Value = item.Print;
        //                                insCmd.Parameters.Add("@History", SqlDbType.Bit).Value = item.History;
        //                                insCmd.ExecuteNonQuery();
        //                            }
        //                        }
        //                    }

        //                    tran.Commit();
        //                    return Json(new { success = true, message = "Permissions saved successfully!" });
        //                }
        //                catch (Exception ex)
        //                {
        //                    tran.Rollback();
        //                    ErrorLogger.ErrorLog(ex);
        //                    return Json(new { success = false, message = "ERROR: " + ex.Message });
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.ErrorLog(ex);
        //        return Json(new { success = false, message = "ERROR: " + ex.Message });
        //    }
        //}

        #endregion
        public JsonResult Get_SEC_UserPermission_Edit(int UserID)
        {
            List<object> _SEC_User_List = new List<object>();
            List<object> _SEC_UserPrivileges_List = new List<object>(); try
            {
                List<object> _list_SYS_Module = new List<object>();
                List<object> SYSModuleList = new List<object>();
                DataSet _PartyEditData = DbHelper.ExecuteDataSet("Get_SEC_UserPermission_Edit",
                    new SqlParameter("@UserID", SqlDbType.Int) { Value = UserID });

                for (int i = 0; i < _PartyEditData.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, string> Values = new Dictionary<string, string>();
                    for (int j = 0; j < _PartyEditData.Tables[0].Columns.Count; j++)
                    {
                        Values.Add(_PartyEditData.Tables[0].Columns[j].ToString(), _PartyEditData.Tables[0].Rows[i][j].ToString());
                    }
                    _SEC_User_List.Add(Values);
                }
                for (int i = 0; i < _PartyEditData.Tables[1].Rows.Count; i++)
                {
                    Dictionary<string, string> Values = new Dictionary<string, string>();
                    for (int j = 0; j < _PartyEditData.Tables[1].Columns.Count; j++)
                    {
                        Values.Add(_PartyEditData.Tables[1].Columns[j].ToString(), _PartyEditData.Tables[1].Rows[i][j].ToString());
                    }
                    _SEC_UserPrivileges_List.Add(Values);
                }
                var jsonResult = Json(new
                {
                    List = _SEC_User_List,
                    SEC_UserPrivileges = _SEC_UserPrivileges_List

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
                return jsonResult;
            }
        }

        public string SEC_UserPermission_Insert_Update_Delete(SEC_User model, List<SEC_UserPermission> ImportTerms, string Action)
        {
            string Message = "";
            try
            {
                model.CreatedByUserID = SessionFacade.UserSession.UserID;
                #region Import Terms
                List<SEC_UserPermission> _TermsNew = new List<SEC_UserPermission>();
                if (ImportTerms != null)
                {
                    for (int j = 0; j < ImportTerms.Count; j++)
                    {
                        //if (ImportTerms[i].OrderedQuantity > 0)
                        ImportTerms[j].CreatedByUserID = Convert.ToInt32(SessionFacade.UserSession.UserID);
                        _TermsNew.Add(ImportTerms[j]);
                    }
                }
                #endregion

                #region Creating XMl
                string xml = "";
                string XML_Detail = "";
                XmlDocument Xmldata = CommonMethods.ConvertToXml(model);
                xml = "<DocumentElement><SEC_User>" + Xmldata.DocumentElement.InnerXml + "</SEC_User></DocumentElement>";

                if (_TermsNew.Count > 0)
                {
                    XmlDocument Xmldata_Terms = CommonMethods.ConvertToXml(_TermsNew);
                    XML_Detail = "<DocumentElement>" + Xmldata_Terms.DocumentElement.InnerXml + "</DocumentElement>";
                }
                #endregion
                //  ErrorLogger.ErrorLogStr("XML Create" + xml);
                //   ErrorLogger.ErrorLogStr("XML_Detail Create" + XML_Detail);
                Message = DbHelper.ExecuteNonQueryWithMessage("SEC_UserPermission_Insert_Update_Delete",
                    new SqlParameter("@XML", xml),
                    new SqlParameter("@XML_Detail", XML_Detail),
                    new SqlParameter("@ACTION", Action));
                //  ErrorLogger.ErrorLogStr("Last" + Message);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return "ERROR : " + ex.InnerException.Message;
            }
            return Message;

        }

        public ActionResult UserPermissionDetails()
        {
            return View();
        }

        public JsonResult Getdata_User_Permission()
        {
            try
            {
                List<object> _list = new List<object>();
                DataSet _DropDownList = DbHelper.ExecuteDataSet("Getdata_User_Permission");
                if (_DropDownList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[0].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[0].Columns[j].ToString(), _DropDownList.Tables[0].Rows[i][j].ToString());
                        }
                        _list.Add(Values);
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
                    @fixed = row.Field<bool>("fixed"),
                })
                .ToList();

                var jsonResult = Json(new
                {
                    List = _list,
                    DataField = DataField,
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

        public ActionResult UserRole()
        {
            return View();
        }
        public ActionResult UserRoleUpdate(int id)
        {
            ViewBag.UserID = id;
            return View();
        }
        public JsonResult Getdata_Role()
        {
            try
            {
                List<Dictionary<string, object>> _list = new List<Dictionary<string, object>>();
                //cmd.Parameters.AddWithValue("@ShapeID", ShapeID);
                DataSet ds = DbHelper.ExecuteDataSet("Getdata_Role");
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (DataColumn col in ds.Tables[0].Columns)
                        {
                            dict[col.ColumnName] = row[col];
                        }
                        _list.Add(dict);
                    }
                }
                var jsonResult = Json(new
                {
                    RoleList = _list,
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
        public JsonResult Get_SEC_RolePrivileges_Edit(int RoleID)
        {
            List<object> _SEC_Role_List = new List<object>();
            List<object> _SEC_RolePrivileges_List = new List<object>();
            try
            {
                List<object> _list_SYS_Module = new List<object>();
                List<object> SYSModuleList = new List<object>();

                DataSet _RoleEditData = DbHelper.ExecuteDataSet("Get_SEC_RolePrivileges_Edit",
                    new SqlParameter("@RoleID", SqlDbType.Int) { Value = RoleID });

                for (int i = 0; i < _RoleEditData.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, string> Values = new Dictionary<string, string>();
                    for (int j = 0; j < _RoleEditData.Tables[0].Columns.Count; j++)
                    {
                        Values.Add(_RoleEditData.Tables[0].Columns[j].ToString(), _RoleEditData.Tables[0].Rows[i][j].ToString());
                    }
                    _SEC_Role_List.Add(Values);
                }
                for (int i = 0; i < _RoleEditData.Tables[1].Rows.Count; i++)
                {
                    Dictionary<string, string> Values = new Dictionary<string, string>();
                    for (int j = 0; j < _RoleEditData.Tables[1].Columns.Count; j++)
                    {
                        Values.Add(_RoleEditData.Tables[1].Columns[j].ToString(), _RoleEditData.Tables[1].Rows[i][j].ToString());
                    }
                    _SEC_RolePrivileges_List.Add(Values);
                }
                var jsonResult = Json(new
                {
                    List = _SEC_Role_List,
                    SEC_RolePrivileges = _SEC_RolePrivileges_List
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
                return jsonResult;
            }
        }
        public string MST_Role_Insert_Update_Delete(MST_Role model, List<SEC_RolePrivileges> ImportTerms, string Action)
        {
            string Message = "";
            try
            {
                model.CreatedByUserID = SessionFacade.UserSession.UserID;
                #region Import Terms
                List<SEC_RolePrivileges> _TermsNew = new List<SEC_RolePrivileges>();

                if (ImportTerms != null)
                {
                    foreach (var item in ImportTerms)
                    {
                        if (item != null)
                        {
                            item.CreatedByUserID = Convert.ToInt32(SessionFacade.UserSession.UserID);
                            _TermsNew.Add(item);
                        }
                    }
                }
                #endregion

                #region Creating XMl
                string xml = "";
                string XML_Detail = "";
                XmlDocument Xmldata = CommonMethods.ConvertToXml(model);
                xml = "<DocumentElement><MST_Role>" + Xmldata.DocumentElement.InnerXml + "</MST_Role></DocumentElement>";

                if (_TermsNew.Count > 0)
                {
                    XmlDocument Xmldata_Terms = CommonMethods.ConvertToXml(_TermsNew);
                    XML_Detail = "<DocumentElement>" + Xmldata_Terms.DocumentElement.InnerXml + "</DocumentElement>";
                }
                #endregion
                //  ErrorLogger.ErrorLogStr("XML Create" + xml);
                //   ErrorLogger.ErrorLogStr("XML_Detail Create" + XML_Detail);
                Message = DbHelper.ExecuteNonQueryWithMessage("MST_Role_Insert_Update_Delete",
                    new SqlParameter("@XML", xml),
                    new SqlParameter("@XML_Detail", XML_Detail),
                    new SqlParameter("@ACTION", Action));
                ErrorLogger.ErrorLogStr("SQL Message: " + Message); // Optional

            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                return "ERROR : " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return Message;

        }
    }
}