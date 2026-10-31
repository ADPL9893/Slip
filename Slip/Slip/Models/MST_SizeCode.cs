using System;

namespace Slip.Models
{
    public class MST_SizeCode
    {
        public int ID { get; set; }
        public string SizeName { get; set; }
        public string ShortName { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}

