using System;

namespace Slip.Models
{
    public class MST_Shape
    {
        public int ShapeID { get; set; }
        public string ShapeName { get; set; }
        public int? ShapeTypeID { get; set; }
        public string ShapeTypeName { get; set; }
        public string Remarks { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public int? Sequence { get; set; }
        public string ShapeCategory { get; set; }
        public int? UserID { get; set; }
        public int? UpdateBy { get; set; }
        public string ShapeGroup { get; set; }
        public string ShortName { get; set; }
        public int? MaxNo { get; set; }
        public DateTime? MaxNoDate { get; set; }
        public string Type { get; set; }
        public string DisplayType { get; set; }
        public string ShapePrdGroup { get; set; }
        public string ShapeFinalGroup { get; set; }
        public int? ShapeIDUserFile { get; set; }
    }
}

