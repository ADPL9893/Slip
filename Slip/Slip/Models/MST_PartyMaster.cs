using System;

namespace Slip.Models
{
    public class MST_PartyMaster
    {
        public Guid PartyId { get; set; }
        public string PartyCode { get; set; }
        public string PartyName { get; set; }
        public string ContactPerson { get; set; }
        public bool IsOutSide { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string CountryName { get; set; }
        public string Pincode { get; set; }
        public string GSTNo { get; set; }
        public string PANNo { get; set; }
        public bool IsActive { get; set; }
        public string Remarks { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
