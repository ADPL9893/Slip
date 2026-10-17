using System;

namespace Slip.Models
{
    public class MST_Priority
    {
        public int PriorityID { get; set; }
        public string PriorityName { get; set; }
        public string ColorHex { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
