using System;

namespace Slip.Models
{
    public class MST_Cut
    {
        public int CutID { get; set; }
        public string CutName { get; set; }
        public string CutGroup { get; set; }
        public int? Sequence { get; set; }
        public string Remarks { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
    }
}

