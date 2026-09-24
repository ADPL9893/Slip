using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Slip.Controllers
{
    public class DashboardController : BaseController
    {
        // GET: Dashboard
        public ActionResult Dashboard()
        {
            SetModulePermissions("Dashboard", "Dashboard");
            return View();
        }
    }
}