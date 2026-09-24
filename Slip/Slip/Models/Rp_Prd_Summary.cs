using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class Rp_Prd_Summary
    {
        public int? OnePcs { get; set; }
        public int? TwoPcs { get; set; }
        public int? ThreePcs { get; set; }
        public int? FourPcs { get; set; }
        public int? FivePcs { get; set; }
        public int? SixPcs { get; set; }
        public int? SevenPcs { get; set; }

        public decimal? OneWT { get; set; }
        public decimal? TwoWT { get; set; }
        public decimal? ThreeWT { get; set; }
        public decimal? FourWT { get; set; }
        public decimal? FiveWT { get; set; }
        public decimal? SixWT { get; set; }
        public decimal? SevenWT { get; set; }

        public string SType { get; set; }
        public string GradNo { get; set; }
        public int? Pcs { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Per { get; set; }
        public decimal? PerCTValue { get; set; }
        public string Shape { get; set; }
        public decimal? PrdAmt { get; set; }
        public decimal? PrcAmt { get; set; }
        public decimal? Pweight { get; set; }
        public decimal? RoughWeight { get; set; }
        public string Receipe { get; set; }
        public string SizeCode { get; set; }
        public decimal? MinPlateSize { get; set; }
        public decimal? MaxPlateSize { get; set; }
    }
}