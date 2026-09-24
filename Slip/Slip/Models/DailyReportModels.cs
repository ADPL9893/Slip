using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class PolishData
    {
        public string ReportDate { get; set; }
        public string MainGroup { get; set; }
        public string Status { get; set; }
        public decimal? PolishWeight { get; set; }
        public decimal? MumbaiSubmit { get; set; }
    }

    public class RoughData
    {
        public string ReportDate { get; set; }
        public string MainGroup { get; set; }
        public string Status { get; set; }
        public decimal? RoughWeight { get; set; }
    }

    public class VipulbhaiData
    {
        public string MainGroup { get; set; }
        public string ReportDate { get; set; }
        public string Status { get; set; }
        public decimal? JW { get; set; }
        public decimal? JC { get; set; }
        public decimal? SW { get; set; }
        public decimal? SC { get; set; }
        public decimal? Total { get; set; }
    }

    public class ProcessData
    {
        public string ReportDate { get; set; }
        public string MainGroup { get; set; }
        public string Process { get; set; }
        public decimal? RPartWeight { get; set; }
        public decimal? PolishPrd { get; set; }
        public decimal? PrdPer { get; set; }
        public decimal? PolishWeight { get; set; }
        public decimal? PolishPer { get; set; }
    }
    public class TransferData
    {
        public string ReportDate { get; set; }
        public string MainGroup { get; set; }
        public string SizeCode { get; set; }
        public decimal? RPartWeight { get; set; }
        public decimal? PolishPrd { get; set; }
        public decimal? PrdPer { get; set; }
        public decimal? PolishWeight { get; set; }
        public decimal? PolishPer { get; set; }
    }

    public class UnderProcessDetail
    {
        public string ReportDate { get; set; }
        public string MainGroup { get; set; }
        public int SrNo { get; set; }
        public string Process { get; set; }
        public int? Pcs { get; set; }
        public decimal? RPartWeight { get; set; }
        public decimal? PolishWeight { get; set; }
        public decimal? Cur_Polish_Ct { get; set; }
        public decimal? DiffPer { get; set; }
        public string Ideal_Ct { get; set; }
    }

    public class DailyReportViewModel
    {
        public List<PolishData> PolishData { get; set; }
        public List<RoughData> RoughData { get; set; }
        public List<VipulbhaiData> VipulbhaiData { get; set; }
        public List<ProcessData> JumboWhiteData { get; set; }
        public List<ProcessData> SmallWhiteData { get; set; }
        public List<ProcessData> ColorData { get; set; }
        public List<ProcessData> UnderProcessData { get; set; }
        public List<TransferData> TransferData { get; set; }
        public List<UnderProcessDetail> PrcWiseUnderProcessData { get; set; }
        public string ReportDate { get; set; }
    }
}