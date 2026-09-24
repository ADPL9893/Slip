using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class FourP_Pridiction
    {
        public int ID { get; set; }
        public decimal RPartWeight { get; set; }
        public decimal PolishWeight { get; set; }
        public string ShapeName { get; set; }
        public string ShapeType { get; set; }
        public string SizeCode { get; set; }
        public string Shift { get; set; }
        public string Machine_No { get; set; }
        public string MainRemarks { get; set; }
        public string Remarks { get; set; }
        public int ADPID { get; set; }
        public int LogID { get; set; }
        public decimal Model { get; set; }
        public string Created { get; set; }
    }
}