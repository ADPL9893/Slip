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
using System.Web.Script.Serialization;
using System.Xml;

namespace Slip.Controllers
{
    public class LoginController : Controller
    {
        string conn = ConfigurationManager.ConnectionStrings["StockDetailConnectionString"].ConnectionString;
        private const string RememberMeCookieName = "ADPL_RememberMe";

        // GET: Login
        public ActionResult Index()
        {
            if (SessionFacade.UserSession != null)
            {
                return RedirectToAction("Index", "Home");
            }

            HttpCookie rememberCookie = Request.Cookies[RememberMeCookieName];
            if (rememberCookie != null && !string.IsNullOrWhiteSpace(rememberCookie.Value))
            {
                try
                {
                    string decrypted = CommonMethods.Decrypt(rememberCookie.Value, true);
                    Dictionary<string, string> data = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(decrypted);
                    ViewBag.RememberMe = true;
                    ViewBag.RememberedUsername = data.ContainsKey("U") ? data["U"] : "";
                    ViewBag.RememberedPassword = data.ContainsKey("P") ? data["P"] : "";
                }
                catch
                {
                    ClearRememberMeCookie();
                }
            }

            return View();
        }
        public JsonResult LoginUser(string username, string password, bool rememberMe = false)
        {
            string Message = "";
            try
            {
                // ErrorLogger.ErrorLogStr("1_conn_ " + conn);

                if (SessionFacade.UserSession == null)
                {
                    DataSet dataset = DbHelper.ExecuteDataSet("Check_Login",
                        new SqlParameter("@username", SqlDbType.VarChar) { Value = username },
                        new SqlParameter("@password", SqlDbType.VarChar) { Value = password });
                    //  ErrorLogger.ErrorLogStr("3" + dataset.Tables[0].Rows.Count);
                    if (dataset.Tables[0].Rows.Count > 0)
                    {
                        SEC_User _SEC_User = new SEC_User();
                        _SEC_User.UserID = Convert.ToInt32(dataset.Tables[0].Rows[0][0].ToString());
                        _SEC_User.UserName = dataset.Tables[0].Rows[0][1].ToString();
                        _SEC_User.Password = dataset.Tables[0].Rows[0][2].ToString();
                        _SEC_User.EmployeeID = Convert.ToInt32(dataset.Tables[0].Rows[0][3].ToString());
                        _SEC_User.IsAdmin = Convert.ToBoolean(dataset.Tables[0].Rows[0][4].ToString());
                        _SEC_User.IsActive = Convert.ToBoolean(dataset.Tables[0].Rows[0][5].ToString());
                        _SEC_User.TableNo = dataset.Tables[0].Rows[0][6].ToString();
                        _SEC_User.IsDashBoardShow = Convert.ToBoolean(string.IsNullOrWhiteSpace(dataset.Tables[0].Rows[0][7].ToString()) ? "false": dataset.Tables[0].Rows[0][7].ToString());
                        _SEC_User.RoleID = Convert.ToInt32(dataset.Tables[0].Rows[0][8].ToString());
                        SessionFacade.UserSession = _SEC_User;

                        #region Login History
                        SEC_LoginHistory _TermsNew = new SEC_LoginHistory();
                        _TermsNew.UserID = SessionFacade.UserSession.UserID;
                        _TermsNew.LoginTime = DateTime.Now;
                        _TermsNew.Remarls = "LOGIN";


                        XmlDocument Xmldata = CommonMethods.ConvertToXml(_TermsNew);
                        string xmlStr = "<DocumentElement><SEC_LoginHistory>" + Xmldata.DocumentElement.InnerXml + "</SEC_LoginHistory></DocumentElement>";

                        int i = DbHelper.ExecuteNonQuery("SEC_LoginHistory_Insert_Update_Delete",
                            new SqlParameter("@XML", xmlStr),
                            new SqlParameter("@ACTION", "INSERT"));
                        //  ErrorLogger.ErrorLogStr("4" + SessionFacade.UserSession.UserID);
                        #endregion
                        if (SessionFacade.UserSession.UserID == 0)
                        {
                            SessionFacade.UserSession.UserID = 0;
                            SessionFacade.UserSession.IsAdmin = false;
                        }

                        DataSet datasetpermissions = DbHelper.ExecuteDataSet("Get_CheckPermission_User",
                            new SqlParameter("@UserID", SqlDbType.Int) { Value = SessionFacade.UserSession.UserID },
                            new SqlParameter("@RoleID", SqlDbType.Int) { Value = SessionFacade.UserSession.RoleID },
                            new SqlParameter("@IsAdmin", SqlDbType.Float) { Value = SessionFacade.UserSession.IsAdmin });

                        List<CheckPermission_User> _CheckPermission_UserList = new List<CheckPermission_User>();

                        _CheckPermission_UserList = datasetpermissions.Tables[0].AsEnumerable()
                        .Select(dataRow => new CheckPermission_User
                        {
                            ModuleID = dataRow.Field<Int32>("ModuleID"),
                            ModuleName = dataRow.Field<string>("ModuleName"),
                            Action = dataRow.Field<string>("Action"),
                            Controller = dataRow.Field<string>("Controller"),
                            GroupID = dataRow.Field<Int32>("GroupID"),
                            GroupName = dataRow.Field<string>("GroupName"),
                            IconName = dataRow.Field<string>("IconName"),
                            Description = dataRow.Field<string>("Description"),
                            SubMenu = dataRow.Field<string>("SubMenu"),
                            Sequence = dataRow.Field<int>("Sequence")
                        }).ToList();

                        SessionFacade.FormPermissions = _CheckPermission_UserList;
                        List<CheckPermission_User> _CheckPermission_UserListGroup = new List<CheckPermission_User>();

                        _CheckPermission_UserListGroup = datasetpermissions.Tables[1].AsEnumerable()
                        .Select(dataRow => new CheckPermission_User
                        {
                            GroupID = dataRow.Field<Int32>("GroupID"),
                            GroupName = dataRow.Field<string>("GroupName"),
                            SubMenu = dataRow.Field<string>("SubMenu"),
                            Sequence = dataRow.Field<int>("Sequence")
                        }).ToList();

                        SessionFacade.FormPermissionsGroup = _CheckPermission_UserListGroup;

                        if (rememberMe)
                        {
                            SetRememberMeCookie(username, password);
                        }
                        else
                        {
                            ClearRememberMeCookie();
                        }

                        int No = 1;
                        int UserID = SessionFacade.UserSession.UserID;

                        return Json(new
                        {
                            Status = No,
                            UserID = UserID
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        int No = 2;
                        return Json(new
                        {
                            Status = No,
                            UserID = 0
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    int No = 3;
                    return Json(new
                    {
                        Status = No,
                        UserID = 0
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                int No = 0;
                return Json(new
                {
                    Status = No,
                    UserID = 0
                }, JsonRequestBehavior.AllowGet);


                throw ex;
            }
            finally
            {

            }
        }
        public string LoginUserForPassChange(string username, string password)
        {
            string Message = "";
            try
            {
                if (SessionFacade.UserSession != null)
                {
                    DataSet dataset = DbHelper.ExecuteDataSet("Check_Login",
                        new SqlParameter("@username", SqlDbType.VarChar) { Value = username },
                        new SqlParameter("@password", SqlDbType.VarChar) { Value = password });

                    if (dataset.Tables[0].Rows.Count > 0)
                    {

                        SEC_User _SEC_User = new SEC_User();
                        _SEC_User.UserID = Convert.ToInt32(dataset.Tables[0].Rows[0][0].ToString());
                        _SEC_User.UserName = dataset.Tables[0].Rows[0][1].ToString();
                        _SEC_User.Password = dataset.Tables[0].Rows[0][2].ToString();
                        _SEC_User.EmployeeID = Convert.ToInt32(dataset.Tables[0].Rows[0][3].ToString());
                        _SEC_User.IsAdmin = Convert.ToBoolean(dataset.Tables[0].Rows[0][4].ToString());
                        _SEC_User.IsActive = Convert.ToBoolean(dataset.Tables[0].Rows[0][5].ToString());

                        SessionFacade.UserSession = _SEC_User;

                        #region Login History
                        SEC_LoginHistory _TermsNew = new SEC_LoginHistory();
                        _TermsNew.UserID = SessionFacade.UserSession.UserID;
                        _TermsNew.LoginTime = DateTime.Now;
                        _TermsNew.Remarls = "LOGIN";

                        XmlDocument Xmldata = CommonMethods.ConvertToXml(_TermsNew);
                        string xmlStr = "<DocumentElement><SEC_LoginHistory>" + Xmldata.DocumentElement.InnerXml + "</SEC_LoginHistory></DocumentElement>";

                        int i = DbHelper.ExecuteNonQuery("SEC_LoginHistory_Insert_Update_Delete",
                            new SqlParameter("@XML", xmlStr),
                            new SqlParameter("@ACTION", "INSERT"));

                        #endregion
                        if (SessionFacade.UserSession.UserID == 0)
                        {
                            SessionFacade.UserSession.UserID = 0;
                            SessionFacade.UserSession.IsAdmin = false;
                        }

                        DataSet datasetpermissions = DbHelper.ExecuteDataSet("Get_CheckPermission_User",
                            new SqlParameter("@UserID", SqlDbType.Int) { Value = SessionFacade.UserSession.UserID },
                            new SqlParameter("@IsAdmin", SqlDbType.Float) { Value = SessionFacade.UserSession.IsAdmin });

                        List<CheckPermission_User> _CheckPermission_UserList = new List<CheckPermission_User>();

                        _CheckPermission_UserList = datasetpermissions.Tables[0].AsEnumerable()
                                        .Select(dataRow => new CheckPermission_User
                                        {
                                            ModuleID = dataRow.Field<Int32>("ModuleID"),
                                            ModuleName = dataRow.Field<string>("ModuleName"),
                                            Action = dataRow.Field<string>("Action"),
                                            Controller = dataRow.Field<string>("Controller"),
                                            GroupID = dataRow.Field<Int32>("GroupID"),
                                            GroupName = dataRow.Field<string>("GroupName"),
                                            IconName = dataRow.Field<string>("IconName"),
                                            Description = dataRow.Field<string>("Description"),
                                        }).ToList();

                        SessionFacade.FormPermissions = _CheckPermission_UserList;

                        return "1";
                    }
                    else
                    {
                        return "2";
                    }
                }
                else
                {
                    return "3";
                }

            }
            catch (Exception ex)
            {
                return "0";
                ErrorLogger.ErrorLog(ex);
                throw ex;
            }
            finally
            {

            }
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }

        private void SetRememberMeCookie(string username, string password)
        {
            string payload = new JavaScriptSerializer().Serialize(new Dictionary<string, string>
            {
                { "U", username },
                { "P", password }
            });
            string encrypted = CommonMethods.Encrypt(payload, true);

            HttpCookie cookie = new HttpCookie(RememberMeCookieName, encrypted)
            {
                Expires = DateTime.Now.AddDays(30),
                HttpOnly = true,
                Secure = Request.IsSecureConnection
            };
            Response.Cookies.Add(cookie);
        }

        private void ClearRememberMeCookie()
        {
            if (Request.Cookies[RememberMeCookieName] != null)
            {
                HttpCookie cookie = new HttpCookie(RememberMeCookieName, "")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(cookie);
            }
        }
    }
}