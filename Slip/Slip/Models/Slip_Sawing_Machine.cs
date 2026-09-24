using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class Slip_Sawing_Machine
    {
        public int ID { get; set; }
        public string TableNo { get; set; }
        public string Shift { get; set; }
        public string MachineNo { get; set; }
        public string EmpName { get; set; }
        public string StoneID { get; set; }
        public string Pie { get; set; }
        public decimal? Depth { get; set; }
        public decimal? Opening { get; set; }
        public string JangadNo { get; set; }
        public string Remarks { get; set; }
        public string ReportDate { get; set; }
        public string RCode { get; set; }
    }
}