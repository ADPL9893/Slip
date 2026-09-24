using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class TRN_Labour
    {
        public int? TLabourID { get; set; }
        public int? MLabourID { get; set; }
        public int? ProcessID { get; set; }
        public string Process { get; set; }
        public string ProcessType { get; set; }
        public decimal? Amount { get; set; }
        public bool IsActive { get; set; }
        public string Remarks { get; set; }

        public string PType { get; set; }
        public string LotType { get; set; }
        public string ShapeType { get; set; }
        public decimal? FromSize { get; set; }
        public decimal? ToSize { get; set; }
        public string AmtType { get; set; }
        //public string WeightType { get; set; }
       
        //public string RpartPolish { get; set; }
        //public string PolishRange { get; set; }
        //public string TreatedColor { get; set; }
        //public string Cut { get; set; }
        //public string LabourType { get; set; }
        public string SizeCodeType { get; set; }
        public string FancyShape { get; set; }
    }


    public class TRN_Labour_V2
    {
        public int? LID { get; set; }
        public string Process { get; set; }
        public string SubProcess { get; set; }
        public string Shape { get; set; }
        public decimal? FromSize { get; set; }
        public decimal? ToSize { get; set; }
        public string Cut { get; set; }
        public string ColorType { get; set; }
        public string PcsWeight { get; set; }
        public string IssueReceive { get; set; }
        public decimal? ManualAmt { get; set; }
        public decimal? AutoAmt { get; set; }
        public decimal? UseAmt { get; set; }
        public string Remarks { get; set; }
    }

}