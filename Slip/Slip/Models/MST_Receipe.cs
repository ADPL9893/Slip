using System;

namespace Slip.Models
{
    public class MST_Receipe
    {
        public int ReceipeID { get; set; }
        public string Receipe { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public int? UserID { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

    }
}
