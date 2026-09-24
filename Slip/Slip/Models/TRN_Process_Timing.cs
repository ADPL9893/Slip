using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class TRN_Process_Timing
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int ProcessID { get; set; }
        public string ProcessName { get; set; }
        public int TotalPcs { get; set; }
        public decimal OnTimePer { get; set; }
        public decimal OffTimePer { get; set; }
        public string RoughType { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public int IsActive { get; set; }

    }
}