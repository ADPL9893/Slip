using System;

namespace Slip.Models
{
    public class MST_Color
    {
        public int ColorID { get; set; }
        public string ColorName { get; set; }
        public int? ColorTypeID { get; set; }
        public string ColorType { get; set; }
        public string Remarks { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public int? Sequence { get; set; }
        public int? UserID { get; set; }
        public bool IsActive { get; set; }
        public int? ColorIDUserFile { get; set; }
    }
}

