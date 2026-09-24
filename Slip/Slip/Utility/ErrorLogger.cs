using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Slip.Utility
{
    public class ErrorLogger
    {
        public static void CreateLogFiles()
        {

        }
        public static void ErrorLogStr(string ex)
        {
            string sLogFormat;
            string sErrorTime;
            Random rnd = new Random();
            sLogFormat = rnd.Next(1, 1000).ToString() + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + DateTime.Now.ToString("hhmmss");
            sErrorTime = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString();
            //StreamWriter sw = File.CreateText(System.Web.HttpContext.Current.Request.MapPath("~/Logs/") + Guid.NewGuid().ToString() + ".txt");
            StreamWriter sw = File.CreateText(System.Web.Hosting.HostingEnvironment.MapPath("~/Logs/") + Guid.NewGuid().ToString() + ".txt");
            sw.WriteLine("Time: " + sErrorTime);
            sw.WriteLine("--------------------------------------------------------------------------------------------");
            sw.WriteLine("Message: " + ex);
            sw.Flush();
            sw.Close();
            rnd = null;
        }
        public static void ErrorLog(Exception ex)
        {
            try
            {
                string sLogFormat;
                string sErrorTime;
                Random rnd = new Random();
                sLogFormat = rnd.Next(1, 1000).ToString() + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + DateTime.Now.ToString("hhmmss");
                sErrorTime = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString();
                //StreamWriter sw = File.CreateText(System.Web.HttpContext.Current.Request.MapPath("~/Logs/") + sLogFormat + ".txt");
                StreamWriter sw = File.CreateText(System.Web.Hosting.HostingEnvironment.MapPath("~/Logs/") + sLogFormat + ".txt");
                sw.WriteLine("Time: " + sErrorTime);
                sw.WriteLine("--------------------------------------------------------------------------------------------");
                sw.WriteLine(ex.Message);
                sw.WriteLine("--------------------------------------------------------------------------------------------");
                sw.WriteLine(ex.StackTrace);
                sw.Flush();
                sw.Close();
                rnd = null;
            }
            catch (Exception)
            {


            }



        }


        public static void ErrorLog(string message)
        {
            string sLogFormat;
            string sErrorTime;
            Random rnd = new Random();
            sLogFormat = rnd.Next(1, 1000).ToString() + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + DateTime.Now.ToString("hhmmss");
            sErrorTime = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString();
            //StreamWriter sw = File.CreateText(System.Web.HttpContext.Current.Request.MapPath("~/Logs/") + sLogFormat + ".txt");
            StreamWriter sw = File.CreateText(System.Web.Hosting.HostingEnvironment.MapPath("~/Logs/") + sLogFormat + ".txt");
            sw.WriteLine("Message: " + message);
            sw.Flush();
            sw.Close();
            rnd = null;
        }
    }
}