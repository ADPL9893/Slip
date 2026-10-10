using System;

namespace Slip.Models
{
    public class MST_CompanyBranch
    {
        public int BranchID { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public int BranchTypeID { get; set; }
        public string BranchTypeName { get; set; }
        public string GSTIN { get; set; }
        public string StateCode { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ContactPerson { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
