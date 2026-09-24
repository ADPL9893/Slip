using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class Api_Response
    {
        public string Message { get; set; }
        public List<object> List { get; set; }
    }
}