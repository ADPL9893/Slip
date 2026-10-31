using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class MST_MasterbookDays
    {
        public int MasterbookDaysID { get; set; }
        public int StartDay { get; set; }
        public int EndDay { get; set; }
        public string ColorHex { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDelete { get; set; } = false;
        public int? UserID { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}