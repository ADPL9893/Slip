using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class Mst_Department
    {
        public Guid ID { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; } // Display purpose (Join with MST_CompanyBranch)
        public string DepartmentName { get; set; }
        public string ShortName { get; set; }
        public string Floor { get; set; }
        public string Location { get; set; }
        public bool IsClv { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}