using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class RoughTo4POk_Loss
    {
        public int SrNo { get; set; }
        public string ReportDate { get; set; }
        public string Process { get; set; }
        public decimal? PrdRPartWeight { get; set; }
        public decimal? PrdPolishWeight { get; set; }
        public decimal? PrdPer { get; set; }
        public decimal? FPRPartWeight { get; set; }
        public decimal? FPPolishWeight { get; set; }
        public decimal? FPPer { get; set; }
        public decimal? FPModel { get; set; }
        public decimal? DPolishWeight { get; set; }
        public decimal? DiffPPer { get; set; }
        public int? RE4P { get; set; }
    }
}