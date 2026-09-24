using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Web;

namespace Slip.Models
{
    public class Slip_Scanning_STN
    {
        public int  ID { get; set; }
        public string  MachineNo { get; set; }
        public string EmpName { get; set; }
        public string Shift { get; set; }
        public string LotCode { get; set; }
        public string PCNo { get; set; }
        public decimal?  PacketWT { get; set; }
        public decimal?  MachineWT { get; set; }
        public string Remarks { get; set; }
        public int  LogID { get; set; }
        public string ReportDate { get; set; }
        public string RCode { get; set; }
    }
}