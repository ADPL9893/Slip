namespace Slip.Models
{
    public class TRN_RoughInwardDetail
    {
        public int InwardDetailID { get; set; }
        public int InwardID { get; set; }
        public int SrNo { get; set; }
        public string LotNo { get; set; }
        public string GrowthRate { get; set; }
        public decimal? AvgGrowthRate { get; set; }
        public int? RecipeID { get; set; }
        public string RecipeName { get; set; }
        public int? GradeID { get; set; }
        public string GradeName { get; set; }
        public int Pcs { get; set; }
        public decimal Carat { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
        public string Remarks { get; set; }
        public string MachineNo { get; set; }
        public string Size { get; set; }
        public decimal? FromHeight { get; set; }
        public decimal? ToHeight { get; set; }
        public decimal? Hours { get; set; }
        public string BoxNo { get; set; }
        public string SizeCode { get; set; }
        public string ColorType { get; set; }
        public object IsDelete { get; internal set; }
        public bool NotAllow { get; internal set; } = false;
    }
}
