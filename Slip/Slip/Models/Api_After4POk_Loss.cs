using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class Api_After4POk_Loss
    {
        public string ReportDate { get; set; }
        public string Process { get; set; }
        public int? IPcs { get; set; }
        public decimal? IRPartWeight { get; set; }
        public decimal? IPolishWeight { get; set; }
        public decimal? IModel { get; set; }
        public int? RPcs { get; set; }
        public decimal? RRPartWeight { get; set; }
        public decimal? RPolishWeight { get; set; }
        public decimal? RModel { get; set; }
        public decimal? DRPartWeight { get; set; }
        public decimal? DiffRPer { get; set; }
        public decimal? DPolishWeight { get; set; }
        public decimal? DiffPPer { get; set; }
        public int? TPcs { get; set; }
        public decimal? TRPartWeight { get; set; }
        public decimal? TPolishWeight { get; set; }
    }
}
