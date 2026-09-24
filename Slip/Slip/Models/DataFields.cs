using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class DataFields
    {
        public string dataField { get; set; }
        public string dataType { get; set; }
        public string format { get; set; }
        public bool visible { get; set; }
        public bool @fixed { get; set; }
        public string alignment { get; set; }
        public string column { get; set; }
        public string summaryType { get; set; }
        public string valueFormat { get; set; }
        public bool allowEditing { get; set; }
        public string displayFormat { get; set; }
        public string sortOrder { get; set; }
        public string caption { get; set; }
    }
}