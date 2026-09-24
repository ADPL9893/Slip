using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class SEC_UserPermission
    {
        public int UserPrivilegesID { get; set; }
        public int ModuleID { get; set; }
        public int UserID { get; set; }
        public string Password { get; set; }
        public int CreatedByUserID { get; set; }
        public System.DateTime Created { get; set; }
        public Nullable<System.DateTime> Modified { get; set; }
        public string Remarks { get; set; }
        public bool IsInsert { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsChange { get; set; }
        public bool IsDelete { get; set; }
    }
}