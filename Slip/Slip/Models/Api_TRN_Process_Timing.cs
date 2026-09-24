using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class Api_TRN_Process_Timing
    {
        public string ProcessName { get; set; }
        public int TotalPcs { get; set; }
        public decimal OnTimePer { get; set; }
        public decimal OffTimePer { get; set; }
        public string RoughType { get; set; }
    }
}