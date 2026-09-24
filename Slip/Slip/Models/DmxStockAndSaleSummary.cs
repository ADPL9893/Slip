using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class DmxStockAndSaleSummary
    {
        public string Location { get; set; }
        public string SizeBucket { get; set; }
        public string Color { get; set; }
        public string Clarity { get; set; }
        public int StockPcs { get; set; }
        public int SalePcs { get; set; }
    }
}
