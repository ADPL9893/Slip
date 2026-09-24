using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Slip.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public FileResult DownloadExcel(string filepath)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(filepath));
        }
    }
}