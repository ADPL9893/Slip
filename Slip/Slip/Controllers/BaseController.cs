using Slip.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Slip.Models.Filter.SessionExpireFilter;

namespace Slip.Controllers
{
    [SessionExpireFilterAttribute]
    public class BaseController : Controller
    {
        protected string conn
        {
            get
            {
                return ConfigurationManager
                       .ConnectionStrings["StockDetailConnectionString"]
                       .ConnectionString;
            }
        }

        protected void SetModulePermissions(string controllerName, string actionName)
        {
            bool canAdd = false;
            bool canEdit = false;
            bool canDelete = false;
            bool canExport = false;

            try
            {
                if (SessionFacade.UserSession != null)
                {
                    if (SessionFacade.UserSession.IsAdmin)
                    {
                        canAdd = true;
                        canEdit = true;
                        canDelete = true;
                        canExport = true;
                    }
                    else
                    {
                        using (SqlConnection con = new SqlConnection(conn))
                        {
                            using (SqlCommand cmd = new SqlCommand("Get_UserModulePermission", con))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@UserID", SessionFacade.UserSession.UserID);
                                cmd.Parameters.AddWithValue("@ControllerName", controllerName);
                                cmd.Parameters.AddWithValue("@ActionName", actionName);

                                con.Open();
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        canAdd = reader["Add"] != DBNull.Value && Convert.ToBoolean(reader["Add"]);
                                        canEdit = reader["Edit"] != DBNull.Value && Convert.ToBoolean(reader["Edit"]);
                                        canDelete = reader["Delete"] != DBNull.Value && Convert.ToBoolean(reader["Delete"]);
                                        try
                                        {
                                            canExport = reader["Export"] != DBNull.Value && Convert.ToBoolean(reader["Export"]);
                                        }
                                        catch
                                        {
                                            canExport = canAdd;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
            }

            ViewBag.CanAdd = canAdd;
            ViewBag.CanEdit = canEdit;
            ViewBag.CanDelete = canDelete;
        }
    }
}