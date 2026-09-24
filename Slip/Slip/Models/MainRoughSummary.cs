using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class MainRoughSummary
    {
        public class CleavingSummary
        {
            public int? SrNo { get; set; }
            public string Status { get; set; }
            public string RDate { get; set; }
            public string SizeCode { get; set; }
            public int? Pcs { get; set; }
            public decimal? Weight { get; set; }
        }
        public class CleavingDetails
        {
            public int? SrNo { get; set; }
            public string Status { get; set; }
            public string MainRCode { get; set; }
            public string MainRDate { get; set; }
            public string MainReceipe { get; set; }
            public string MainGradNo { get; set; }
            public int? MainPcs { get; set; }
            public decimal? MainCarat { get; set; }
            public string RDate { get; set; }
            public string Kapan { get; set; }
            public string TableNo { get; set; }
            public string HeightName { get; set; }
            public string GradNo { get; set; }
            public int? RoughPcs { get; set; }
            public decimal? RoughCarat { get; set; }
            public decimal? AWeight { get; set; }
            public int? PrdCTUPPcs { get; set; }
            public decimal? PrdCTUPPolishWeight { get; set; }
            public decimal? PrdCTUPPer { get; set; }
            public decimal? PrdCTUPPrdAmt { get; set; }
            public decimal? PrdCTUPPrcAmt { get; set; }
            public int? PrdCTDNPcs { get; set; }
            public decimal? PrdCTDNPolishWeight { get; set; }
            public decimal? PrdCTDNPer { get; set; }
            public decimal? PrdCTDNPrdAmt { get; set; }
            public decimal? PrdCTDNPrcAmt { get; set; }
            public int? PrdPcs { get; set; }
            public decimal? PrdPolishWeight { get; set; }
            public decimal? PrdPer { get; set; }
            public decimal? PrdAmt { get; set; }
            public decimal? PrcAmt { get; set; }
            public int? PrdDays { get; set; }
            public int? ClvCTUPPcs { get; set; }
            public decimal? ClvCTUPPolishWeight { get; set; }
            public decimal? ClvCTUPPer { get; set; }
            public int? ClvCTDNPcs { get; set; }
            public decimal? ClvCTDNPolishWeight { get; set; }
            public decimal? ClvCTDNPer { get; set; }
            public int? ClvPcs { get; set; }
            public decimal? ClvPolishWeight { get; set; }
            public decimal? ClvPer { get; set; }
            public string ComplateDate { get; set; }
            public int? ClvDays { get; set; }
            public int? BagID { get; set; }
            public string BagNo { get; set; }
        }
        public class DSTDetails
        {
            public int? SrNo { get; set; }
            public string Status { get; set; }
            public string MainRCode { get; set; }
            public string MainRDate { get; set; }
            public string MainReceipe { get; set; }
            public string MainGradNo { get; set; }
            public int? MainPcs { get; set; }
            public decimal? MainCarat { get; set; }
            public string RDate { get; set; }
            public string Kapan { get; set; }
            public string TableNo { get; set; }
            public string HeightName { get; set; }
            public string GradNo { get; set; }
            public int? RoughPcs { get; set; }
            public decimal? RoughCarat { get; set; }
            public decimal? AWeight { get; set; }
            public int? ClvCTUPPcs { get; set; }
            public decimal? ClvCTUPPolishWeight { get; set; }
            public decimal? ClvCTUPPer { get; set; }
            public int? ClvCTDNPcs { get; set; }
            public decimal? ClvCTDNPolishWeight { get; set; }
            public decimal? ClvCTDNPer { get; set; }
            public int? ClvPcs { get; set; }
            public decimal? ClvPolishWeight { get; set; }
            public decimal? ClvPer { get; set; }
            public int? ClvDays { get; set; }
            public int? DSTCTUPPcs { get; set; }
            public decimal? DSTCTUPPolishWeight { get; set; }
            public decimal? DSTCTUPPer { get; set; }
            public int? DSTCTDNPcs { get; set; }
            public decimal? DSTCTDNPolishWeight { get; set; }
            public decimal? DSTCTDNPer { get; set; }
            public int? DSTPcs { get; set; }
            public decimal? DSTPolishWeight { get; set; }
            public decimal? DSTPer { get; set; }
            public string ComplateDate { get; set; }
            public int? DSTDays { get; set; }
            public int? BagID { get; set; }
            public string BagNo { get; set; }
        }
        public class MFGDetails
        {
            public int? SrNo { get; set; }
            public string Status { get; set; }
            public string MainRCode { get; set; }
            public string MainRDate { get; set; }
            public string MainReceipe { get; set; }
            public string MainGradNo { get; set; }
            public int? MainPcs { get; set; }
            public decimal? MainCarat { get; set; }
            public string RDate { get; set; }
            public string Kapan { get; set; }
            public string TableNo { get; set; }
            public string HeightName { get; set; }
            public string GradNo { get; set; }
            public int? RoughPcs { get; set; }
            public decimal? RoughCarat { get; set; }
            public decimal? AWeight { get; set; }
            public int? DSTCTUPPcs { get; set; }
            public decimal? DSTCTUPPolishWeight { get; set; }
            public decimal? DSTCTUPPer { get; set; }
            public int? DSTCTDNPcs { get; set; }
            public decimal? DSTCTDNPolishWeight { get; set; }
            public decimal? DSTCTDNPer { get; set; }
            public int? DSTPcs { get; set; }
            public decimal? DSTPolishWeight { get; set; }
            public decimal? DSTPer { get; set; }
            public int? DSTDays { get; set; }
            public int? MFGCTUPPcs { get; set; }
            public decimal? MFGCTUPPolishWeight { get; set; }
            public decimal? MFGCTUPPer { get; set; }
            public int? MFGCTDNPcs { get; set; }
            public decimal? MFGCTDNPolishWeight { get; set; }
            public decimal? MFGCTDNPer { get; set; }
            public int? MFGPcs { get; set; }
            public decimal? MFGPolishWeight { get; set; }
            public decimal? MFGPer { get; set; }
            public string ComplateDate { get; set; }
            public int? MFGDays { get; set; }
            public int? BagID { get; set; }
            public string BagNo { get; set; }
        }
    }
}