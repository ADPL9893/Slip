using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class TRN_Pricing
    {
        public int PricingID { get; set; }
        public int ShapeID { get; set; }
        public int ColorID { get; set; }
        public int SizeID { get; set; }
        public int ClarityID { get; set; }
        public int CutID { get; set; }
        public decimal Rate { get; set; }
        public string Remarks { get; set; }
        public int UserID { get; set; }
        public string SizeType { get; set; }
        public string GradType { get; set; }
        public string Shape { get; set; }
        public string Color { get; set; }
        public string Clarity { get; set; }
        public string Size { get; set; }
        public string FinalRate { get; set; }

    }
    public class TRN_Pricing_Filter
    {
        public int SizeID { get; set; }
    }
    public class colorList
    {
        public string Color { get; set; }
    }
    public class clarityList
    {
        public string Clarity { get; set; }
    }
    public class sizeList
    {
        public string Size { get; set; }
    }
}