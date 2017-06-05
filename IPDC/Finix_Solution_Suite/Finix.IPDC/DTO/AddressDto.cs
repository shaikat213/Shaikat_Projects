using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class AddressDto
    {
        public long? Id { get; set; }
        public long? ThanaId { get; set; }
        public string ThanaName { get; set; }
        public long? DistrictId { get; set; }
        public string DistrictName { get; set; }
        public long? DivisionId { get; set; }
        public string DivisionName { get; set; }
        public long? CountryId { get; set; }
        public string CountryName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNo { get; set; }
        public string CellPhoneNo { get; set; }
        public string Email { get; set; }
        public bool IsChanged { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
