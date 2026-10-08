using System;

namespace Slip.Models
{
    public class MST_RoughDistribution
    {
        public int DistributionID { get; set; }
        public int PreRoughID { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public string RCode { get; set; }
        public int RoughPcs { get; set; }
        public decimal RoughWeight { get; set; }
        public decimal? Rate { get; set; }
        public string GradNo { get; set; }
        public decimal? FromHeight { get; set; }
        public decimal? ToHeight { get; set; }
        public string Remarks { get; set; }
        public string Priority { get; set; } = "REGULAR";
        public bool IsActive { get; set; }
        public bool IsReceived { get; set; }
        public int UserID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
