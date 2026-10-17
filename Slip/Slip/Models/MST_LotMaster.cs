using System;

namespace Slip.Models
{
    public class MST_LotMaster
    {
        public int LotID { get; set; }
        public int RoughDistributionID { get; set; }
        public string RCode { get; set; }
        public string LotCode { get; set; }
        public int DistributionPartNo { get; set; }

        public int Lot_Pcs { get; set; }
        public decimal Lot_Carat { get; set; }
        public decimal? ValuePerCT { get; set; }

        public int? FactoryCodeID { get; set; }
        public string FactoryCode { get; set; }
        public string ColorType { get; set; }
        public int? SizeCodeID { get; set; }
        public string SizeCodeType { get; set; }
        public int? GradeID { get; set; }
        public string Grade { get; set; }

        public string LotType { get; set; }
        public string OGName { get; set; }
        public string MachineNo { get; set; }
        public decimal? Hours { get; set; }
        public decimal? Height { get; set; }
        public string Size { get; set; }
        public string Receipe { get; set; }
        public string BoxNo { get; set; }
        public decimal? RoughRate { get; set; }
        public string LotPurpose { get; set; }
        public string LotPriority { get; set; }
        public string GrowthRate { get; set; }
        public string CompareNo { get; set; }
        public decimal? NewHeight { get; set; }

        public int? CakePcs { get; set; }
        public decimal? CakeWeight { get; set; }
        public int? RejPcs { get; set; }
        public decimal? RejCarat { get; set; }
        public int? OGPcs { get; set; }
        public decimal? OGCarat { get; set; }

        public Guid? DepartmentID { get; set; }
        public int? HeadManagerID { get; set; }

        public string Remarks { get; set; }
        public bool IsActive { get; set; } = true;

        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? Modified { get; set; }
    }
}
