using Slip.App_Start;
using Slip.Controllers;
using Slip.Models;
using Slip.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Slip
{
    public class MvcApplication : System.Web.HttpApplication
    {
        // Make it static so it isn't garbage collected
        private static Timer _dailyTimer;
        private static Timer timer;
        protected void Application_Start()
        {
            DevExtremeBundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ScheduleDailyTask();
        }

        private void ScheduleDailyTask()
        {
            TimeZoneInfo istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

            DateTime utcNow = DateTime.UtcNow;
            DateTime istNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, istZone);

            // Set today's 7 PM IST
            DateTime nextRunIst = new DateTime(
                istNow.Year,
                istNow.Month,
                istNow.Day,
                19, 5, 0);


            string date = nextRunIst.ToString();

            ErrorLogger.ErrorLogStr("Time - " + date + "  Now - " + istNow);

            // If already past 7 PM today, schedule for tomorrow
            if (istNow > nextRunIst)
            {
                nextRunIst = nextRunIst.AddDays(1);
            }

            // Convert IST time back to UTC
            DateTime nextRunUtc = TimeZoneInfo.ConvertTimeToUtc(nextRunIst, istZone);

            TimeSpan timeToGo = nextRunUtc - utcNow;

            timer = new Timer(x =>
            {
                ExecuteInsertMethod(null);
                ScheduleDailyTask(); // Reschedule next day
            }, null, timeToGo, Timeout.InfiniteTimeSpan);



            //DateTime now = DateTime.Now;
            //DateTime today8PM = new DateTime(now.Year, now.Month, now.Day, 5, 35, 0); // 20:00 = 8 PM
            //                                                                          // If it's already past 8 PM today, schedule it for 8 PM tomorrow
            //string date = today8PM.ToString();

            //ErrorLogger.ErrorLogStr("Time - " + date + "  Now - " + now);

            //if (now > today8PM)
            //{
            //    today8PM = today8PM.AddDays(1);
            //}
            //double tickTime = (today8PM - now).TotalMilliseconds;
            //// The timer will wait 'tickTime' milliseconds, then fire the method, and repeat every 24 hours
            //_dailyTimer = new Timer(ExecuteInsertMethod, null, (int)Math.Max(tickTime, 0), (int)TimeSpan.FromHours(24).TotalMilliseconds);

        }
        private void ExecuteInsertMethod(object state)
        {
            try
            {
                // 1. Create an instance of the controller (assuming the method is not static)
                PricingController controller = new PricingController();

                // 2. Pass null or a new empty list/array instead of []
                // Example: new List<ApprovePricingModel>() or whatever type your method expects
                controller.TRN_Approve_Pricing_Insert(null, "", "Save");
            }
            catch (Exception ex)
            {
                // IMPORTANT: Because this runs in the background, you won't see this error 
                // on a webpage if it fails. You MUST log this exception to a text file
                // or database table so you know if your 8 PM job is failing.
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

    }
}
