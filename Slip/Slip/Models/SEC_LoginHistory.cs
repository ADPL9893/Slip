using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class SEC_LoginHistory
    {
        public int LoginHistoryID { get; set; }
        public int UserID { get; set; }
        public System.DateTime LoginTime { get; set; }
        public System.DateTime LogoutTime { get; set; }
        public System.DateTime Created { get; set; }
        public string Remarls { get; set; }
        public Nullable<System.DateTime> Modified { get; set; }
    }
}