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
            bool canView = false;
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
                        canView = true;

                    }
                    else
                    {
                        List<Dictionary<string, object>> permRows = DbHelper.ExecuteReaderAsList("Get_UserModulePermission",
                            new SqlParameter("@UserID", SessionFacade.UserSession.UserID),
                            new SqlParameter("@ControllerName", controllerName),
                            new SqlParameter("@ActionName", actionName));

                        if (permRows.Count > 0)
                        {
                            var row = permRows[0];
                            canAdd = row["Add"] != null && Convert.ToBoolean(row["Add"]);
                            canEdit = row["Edit"] != null && Convert.ToBoolean(row["Edit"]);
                            canDelete = row["Delete"] != null && Convert.ToBoolean(row["Delete"]);
                            canExport = row.ContainsKey("Export") && row["Export"] != null ? Convert.ToBoolean(row["Export"]) : canAdd;
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
            ViewBag.CanView = canView;
            ViewBag.canExport = canExport;
        }
    }
}