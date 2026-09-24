using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class MST_Role
    {
        public int RoleID { get; set; }
        public int UserID { get; set; }
        public string RoleName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime Modified { get; set; }
        public int CreatedByUserID { get; set; }
        public string CreatedByUserName { get; set; }
        public int RolePrivilegesID { get; set; }
        public int ModuleID { get; set; }
    }
}