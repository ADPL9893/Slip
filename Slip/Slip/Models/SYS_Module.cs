using System;

namespace Slip.Models
{
    public class SYS_Module
    {
        public int ModuleID { get; set; }
        public string ModuleName { get; set; }
        public string Description { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string URL { get; set; }
        public string IconName { get; set; }
        public bool StrictlyStop { get; set; }
        public bool IsActive { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public string Remarks { get; set; }
        public int Sequence { get; set; }
        public int UserID { get; set; }
        public int? GroupID { get; set; }
        public string GroupName { get; set; }
        public string SubMenu { get; set; }
        public bool? IsVoucher { get; set; }
    }

    public class SYS_ModuleGroupModel
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; }
        public int Sequence { get; set; }
    }
}
