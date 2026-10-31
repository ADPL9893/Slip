using System;

namespace Slip.Models
{
    public class MST_FactoryCode
    {
        public int ID { get; set; }

        // Retain FactoryCodeID for backward compatibility
        public int FactoryCodeID
        {
            get { return ID; }
            set { ID = value; }
        }

        public string FactoryCode { get; set; }
        public int? OrderBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
