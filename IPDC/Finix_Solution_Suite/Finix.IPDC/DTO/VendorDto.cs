using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.DTO
{
    public class VendorDto
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public VendorProductType VendorProductType { get; set; }
        public string VendorProductTypeName { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonPhone { get; set; }
        public string ContactPersonEmail { get; set; }
        public string Website { get; set; }
        public long? VendorAddressId { get; set; }
        public AddressDto VendorAddress { get; set; }
        public List<VendorShowroomsDto> Showrooms { get; set; }
        public List<long> RemovedShowrooms { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }

    }
}
