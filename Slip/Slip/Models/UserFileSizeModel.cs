using System;

namespace Slip.Models
{
    public class UserFileSizeModel
    {
        public int SizeID { get; set; }
        public string SizeName { get; set; }
        public int? UserFileSizeID { get; set; }
        public int? PrcSizeID { get; set; }
        public decimal FromSize { get; set; }
        public decimal ToSize { get; set; }
        public string Shape { get; set; }
        public string Cut { get; set; }
        public string Type { get; set; }
        public string LabourType { get; set; }
        public bool IsActive { get; set; }
        public string Remarks { get; set; }
        public int UserID { get; set; }
        public DateTime? EntryDate { get; set; }
    }
}
