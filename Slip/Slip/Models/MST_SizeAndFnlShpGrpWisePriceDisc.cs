namespace Slip.Models
{
    public class MST_SizeAndFnlShpGrpWisePriceDisc
    {
        public int MstSizeID { get; set; }
        public int MStFinalShapGroupID { get; set; }
        public string FinalShapGroup { get; set; }
        public decimal? DiscountPer { get; set; }
    }
}
