using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class QRList
    {
        public string SubPridictionID { get; set; }
        public string SubStoneID { get; set; }
        public string QRName { get; set; }
        public string QRUrl { get; set; }
        public string Shape { get; set; }
        public string Size { get; set; }
        public string Height { get; set; }
        public string Receipe { get; set; }
        public int Pcs { get; set; }
        public decimal? Weight { get; set; }
        public string JangadNo { get; set; }
        public string EmpName { get; set; }
        public string MainRCode { get; set; }

    }
}