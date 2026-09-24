using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class TRN_PreProcess
    {
        public int PreProcessID { get; set; }
        public int PacketID { get; set; }
        public System.DateTime Created { get; set; }
        public string Remarks { get; set; }
        public int UserID { get; set; }
        public bool IsProcess { get; set; }
        public int IssueByUserID { get; set; }
        public int ProcessID { get; set; }
        public int ToProcessID { get; set; }
        public string StoneID { get; set; }
        public int LotID { get; set; }
        public int LogID { get; set; }
    }
}