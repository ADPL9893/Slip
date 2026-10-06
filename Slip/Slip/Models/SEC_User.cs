using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class SEC_User
    {
        public int UserID { get; set; }
        public Nullable<int> CreatedByUserID { get; set; }
        public Nullable<int> EmployeeID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime Created { get; set; }
        public Nullable<System.DateTime> Modified { get; set; }
        public string Remarks { get; set; }
        public string TableNo { get; set; }
        public bool IsDashBoardShow { get; set; }
        public int RoleID { get; set; }
        public Nullable<int> BranchID { get; set; }
    }
}