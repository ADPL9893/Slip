using System;
using System.Collections.Generic;

namespace Slip.Models
{
    public class TRN_RoughInward
    {
        public int InwardID { get; set; }
        public string ChallanNo { get; set; }
        public DateTime ChallanDate { get; set; }
        public string ChallanDateStr { get; set; }
        public int? FactoryCodeID { get; set; }
        public string FactoryCodeName { get; set; }
        public string SourceType { get; set; }
        public int? FromBranchID { get; set; }
        public string FromBranchName { get; set; }
        public Guid? FromPartyID { get; set; }
        public string PartyName { get; set; }
        public int ToBranchID { get; set; }
        public string ToBranchName { get; set; }
        public int PurposeID { get; set; }
        public string PurposeName { get; set; }
        public string SizeCode { get; set; }
        public string ColorType { get; set; }
        public string Remarks { get; set; }
        public int TotalPcs { get; set; }
        public decimal TotalCarat { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public List<TRN_RoughInwardDetail> Details { get; set; }
    }
}
