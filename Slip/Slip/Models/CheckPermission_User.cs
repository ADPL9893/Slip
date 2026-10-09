using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class CheckPermission_User
    {
        public int ModuleID { get; set; }
        public string ModuleName { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public Nullable<int> GroupID { get; set; }
        public string GroupName { get; set; }
        public string IconName { get; set; }
        public string Description { get; set; }
        public string SubMenu { get; set; }

        public int  Sequence { get; set; }
    }
}