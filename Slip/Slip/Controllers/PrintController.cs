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
            DataSet _EditData = new DataSet();
            var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TRN_PreProcess>>(IssueList);

            string xmlStrDown = "";
            if (list.Count > 0)
            {
                XmlDocument XmldataDown = CommonMethods.ConvertToXml(list);
                xmlStrDown = "<DocumentElement>" + XmldataDown.DocumentElement.InnerXml + "</DocumentElement>";
            }

            using (SqlConnection con = new SqlConnection(conn))
            {
                using (SqlCommand cmd = new SqlCommand("Getdata_For_Slip_Sawing_Machine_Print", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@XML", xmlStrDown);
                    con.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(_EditData);
                    cmd.Dispose();
                }
                con.Close();
            }
            return View(_EditData);
        }

        public ActionResult FourP_Daily_SlipPrint(string IssueList)
        {
            DataSet _EditData = new DataSet();
            var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TRN_PreProcess>>(IssueList);

            string xmlStrDown = "";
            if (list.Count > 0)
            {
                XmlDocument XmldataDown = CommonMethods.ConvertToXml(list);
                xmlStrDown = "<DocumentElement>" + XmldataDown.DocumentElement.InnerXml + "</DocumentElement>";
            }

            using (SqlConnection con = new SqlConnection(conn))
            {
                using (SqlCommand cmd = new SqlCommand("Getdata_For_FourP_Daily_Slip_Print", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@XML", xmlStrDown);
                    con.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(_EditData);
                    cmd.Dispose();
                }
                con.Close();
            }
            return View(_EditData);
        }

        public ActionResult Scanning_STNPrint(string IssueList)
        {
            DataSet _EditData = new DataSet();
            var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TRN_PreProcess>>(IssueList);

            string xmlStrDown = "";
            if (list.Count > 0)
            {
                XmlDocument XmldataDown = CommonMethods.ConvertToXml(list);
                xmlStrDown = "<DocumentElement>" + XmldataDown.DocumentElement.InnerXml + "</DocumentElement>";
            }

            using (SqlConnection con = new SqlConnection(conn))
            {
                using (SqlCommand cmd = new SqlCommand("Getdata_For_Slip_Scanning_STN_Print", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@XML", xmlStrDown);
                    con.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(_EditData);
                    cmd.Dispose();
                }
                con.Close();
            }
            return View(_EditData);
        }

        public ActionResult HPHT_Summary_SlipPrint(string RCode, string FromDate, string ToDate, string Title)
        {
            DataSet _EditData = new DataSet();

            using (SqlConnection con = new SqlConnection(conn))
            {
                using (SqlCommand cmd = new SqlCommand("HPHT_Summary_SlipPrint", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RCode", JsonConvert.DeserializeObject<string>(RCode));
                    cmd.Parameters.AddWithValue("@FromDate", JsonConvert.DeserializeObject<string>(FromDate));
                    cmd.Parameters.AddWithValue("@ToDate", JsonConvert.DeserializeObject<string>(ToDate));
                    con.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(_EditData);
                    cmd.Dispose();
                }
                con.Close();
            }

            ViewBag.Titles = JsonConvert.DeserializeObject<string>(Title);
            return View(_EditData);
        }
    }
}