using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class ProcessWiseTiming
    {
        public int SrNo { get; set; }
        public string Type { get; set; }
        public string TableNo { get; set; }
        public string Process { get; set; }
        public int FinishDays { get; set; }
        public int? Days { get; set; }
        public int? ProcessDay { get; set; }
        public int? TotalPcs { get; set; }
        public decimal? ONTIMEPer { get; set; }
        public decimal? OVERDAYPer { get; set; }
    }
}