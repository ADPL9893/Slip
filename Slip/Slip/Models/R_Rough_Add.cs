using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class R_Rough_Add
    {
        public int ID { get; set; }
        public string JangadNo { get; set; }
        public string RName { get; set; }
        public int RoughPcs { get; set; }
        public decimal RoughWeight { get; set; }
        public decimal OriginalCarate { get; set; }
        public decimal Rate { get; set; }
        public string Size { get; set; }
        public decimal FromHeight { get; set; }
        public decimal ToHeight { get; set; }
        public decimal Hours { get; set; }
        public string BoxNo { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Modified { get; set; }
        public int UserID { get; set; }
        public bool IsActive { get; set; }
        public int ReceipeID { get; set; }
        public string SizeCode { get; set; }
        public string ColorType { get; set; }
        public string FactoryCode { get; set; }
        public string GradNo { get; set; }
        public string MachineNo { get; set; }      
        public string Party { get; set; }          
        public string InvoiceNo { get; set; }      
        public DateTime? InvoiceDate { get; set; } 
        public int PurchaseByUserID { get; set; } 
    }
}