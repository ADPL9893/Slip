using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class SEC_UserPrivileges
    {
        public int UserPrivilegesID { get; set; }
        public int ModuleID { get; set; }
        public int UserID { get; set; }
        public int CreatedByUserID { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime Modified { get; set; }
        public Nullable<System.DateTime> Remarks { get; set; }
        public bool View { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        public bool Export { get; set; }
        public bool Mail { get; set; }
        public bool Print { get; set; }
        public bool History { get; set; }
    }
}