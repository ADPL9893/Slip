using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Slip.Models
{
    public class ShapeModel
    {
        public int ShapeID { get; set; }
        public string ShapeName { get; set; }
        public List<InstituteModel> Institutes { get; set; }
    }

    public class InstituteModel
    {
        public int InstituteID { get; set; }
        public string InstituteName { get; set; }
        public List<GradeModel> Grades { get; set; }
    }

    public class GradeModel
    {
        public int GradeID { get; set; }
        public string GradeName { get; set; }
    }

}