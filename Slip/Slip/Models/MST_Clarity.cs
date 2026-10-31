using System;

namespace Slip.Models
{
    public class MST_Clarity
    {
        public int ClarityID { get; set; }
        public string ClarityName { get; set; }
        public string SubClarity { get; set; }
        public int? Sequence { get; set; }
        public int? ClarityIDUserFile { get; set; }
        public string Remarks { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
    }
}

