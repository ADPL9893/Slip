using System;

namespace Slip.Models
{
    public class MST_VoucherDetails
    {
        public int ConfigID { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public string VoucherType { get; set; }
        public string VoucherName { get; set; }
        public string NumericMethod { get; set; }
        public string Prefix { get; set; }
        public string Separator { get; set; }
        public int StartFrom { get; set; }
        public int? CurrentNo { get; set; }
        public bool IsActive { get; set; }
        public int? EntryBy { get; set; }
        public DateTime? EntryDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
