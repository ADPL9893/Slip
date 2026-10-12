using Newtonsoft.Json;
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
    public class PrintController : BaseController
    {
        public ActionResult Slip_Sawing_MachinePrint(string IssueList)
        {
            var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TRN_PreProcess>>(IssueList);

            string xmlStrDown = "";
            if (list.Count > 0)
            {
                XmlDocument XmldataDown = CommonMethods.ConvertToXml(list);
                xmlStrDown = "<DocumentElement>" + XmldataDown.DocumentElement.InnerXml + "</DocumentElement>";
            }

            DataSet _EditData = DbHelper.ExecuteDataSet("Getdata_For_Slip_Sawing_Machine_Print",
                new SqlParameter("@XML", xmlStrDown));
            return View(_EditData);
        }

        public ActionResult FourP_Daily_SlipPrint(string IssueList)
        {
            var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TRN_PreProcess>>(IssueList);

            string xmlStrDown = "";
            if (list.Count > 0)
            {
                XmlDocument XmldataDown = CommonMethods.ConvertToXml(list);
                xmlStrDown = "<DocumentElement>" + XmldataDown.DocumentElement.InnerXml + "</DocumentElement>";
            }

            DataSet _EditData = DbHelper.ExecuteDataSet("Getdata_For_FourP_Daily_Slip_Print",
                new SqlParameter("@XML", xmlStrDown));
            return View(_EditData);
        }

        public ActionResult Scanning_STNPrint(string IssueList)
        {
            var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TRN_PreProcess>>(IssueList);

            string xmlStrDown = "";
            if (list.Count > 0)
            {
                XmlDocument XmldataDown = CommonMethods.ConvertToXml(list);
                xmlStrDown = "<DocumentElement>" + XmldataDown.DocumentElement.InnerXml + "</DocumentElement>";
            }

            DataSet _EditData = DbHelper.ExecuteDataSet("Getdata_For_Slip_Scanning_STN_Print",
                new SqlParameter("@XML", xmlStrDown));
            return View(_EditData);
        }

        public ActionResult HPHT_Summary_SlipPrint(string RCode, string FromDate, string ToDate, string Title)
        {
            DataSet _EditData = DbHelper.ExecuteDataSet("HPHT_Summary_SlipPrint",
                new SqlParameter("@RCode", JsonConvert.DeserializeObject<string>(RCode)),
                new SqlParameter("@FromDate", JsonConvert.DeserializeObject<string>(FromDate)),
                new SqlParameter("@ToDate", JsonConvert.DeserializeObject<string>(ToDate)));

            ViewBag.Titles = JsonConvert.DeserializeObject<string>(Title);
            return View(_EditData);
        }
    }
}