using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class EMP_Employees
    {
        public int EmployeeID { get; set; }
        public int UserID { get; set; }
        public string EmployeeName { get; set; }
        public string Address { get; set; }
        public string IDProofDocName { get; set; }
        public string IDProofNumber { get; set; }
        public string IDProofPhotoPath { get; set; }
        public string PhotoPath { get; set; }
        public string Gender { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public int Age { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime Created { get; set; }
        public Nullable<System.DateTime> Modified { get; set; }
        public string Remarks { get; set; }
        public string AadharNo { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string MobileNo { get; set; }
        public string Designation { get; set; }
        public string EmployeeType { get; set; }
        public string GSTNo { get; set; }
        public string PanNo { get; set; }
        public int DepartID { get; set; }
        public int ManagerID { get; set; }
        public string TableNo { get; set; }
        public string EmpCode { get; set; }
        public string ReferanceName { get; set; }
        public string ReferanceMobile { get; set; }
        public string JobWork { get; set; }
        public int RoleID { get; set; }
        public string ReferenceName { get; set; }
        public string PhotoPath2 { get; set; }
        public Nullable<int> CompanyBranchID { get; set; }
        public Nullable<Guid> PartyID { get; set; }
    }
}