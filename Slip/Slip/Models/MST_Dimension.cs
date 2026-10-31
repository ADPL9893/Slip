using System;

namespace Slip.Models
{
    public class MST_Dimension
    {
        public int ID { get; set; }
        public string DimensionName { get; set; }
        public int? OrderBy { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}

