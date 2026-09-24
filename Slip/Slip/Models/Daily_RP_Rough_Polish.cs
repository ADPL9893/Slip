using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class Daily_RP_Rough_Polish
    {
        public int ID { get; set; }
        public string ReportDate { get; set; }
        public string SizeCode { get; set; }
        public string Status { get; set; }
        public decimal MumbaiSubmit { get; set; }
        public decimal RoughWeight { get; set; }
        public decimal VipulBhai { get; set; }
        public int LogID { get; set; }
    }
}