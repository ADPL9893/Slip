using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class DailyReport
    {
        public int SrNo { get; set; }
        public string Status { get; set; }
        public string Process { get; set; }
        public string SizeCode { get; set; }
        public decimal? RPartWeight { get; set; }
        public decimal? PrdPer { get; set; }
        public decimal? PolishPrd { get; set; }
        public decimal? PolishWeight { get; set; }
        public decimal? PolishPer { get; set; }
        public int? Pcs { get; set; }
        public decimal? Rough_Ct { get; set; }
        public decimal? Polish_Ct { get; set; }
        public decimal? Cur_Polish_Ct { get; set; }
        public decimal? DiffPer { get; set; }
        public string Ideal_Ct { get; set; }
        public decimal? MumbaiSubmit { get; set; }
        public decimal? VIPULBHAI { get; set; }
    }
}