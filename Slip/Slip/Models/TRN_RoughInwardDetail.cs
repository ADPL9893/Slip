namespace Slip.Models
{
    public class TRN_RoughInwardDetail
    {
        public int InwardDetailID { get; set; }
        public int InwardID { get; set; }
        public int SrNo { get; set; }
        public string LotNo { get; set; }
        public string Grade { get; set; }
        public int Pcs { get; set; }
        public decimal Carat { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
        public string Remarks { get; set; }
    }
}
