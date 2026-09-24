using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class SEC_RolePrivileges
    {
        public int RolePrivilegesID { get; set; }
        public int ModuleID { get; set; }
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public int CreatedByUserID { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime Modified { get; set; }
        public string Remarks { get; set; }
    }
}