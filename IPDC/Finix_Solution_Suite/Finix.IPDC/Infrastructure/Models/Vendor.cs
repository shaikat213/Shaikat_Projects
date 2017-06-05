using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Vendor")]
    public class Vendor : Entity
    {
        public string Name { get; set; }
        public VendorProductType VendorProductType { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonPhone { get; set; }
        public string ContactPersonEmail { get; set; }
        public string Website { get; set; }
        public long? VendorAddressId { get; set; }
        [ForeignKey("VendorAddressId")]
        public virtual Address VendorAddress { get; set; }
        public virtual ICollection<VendorShowrooms> Showrooms { get; set; }
    }

    [Table("VendorShowrooms")]
    public class VendorShowrooms : Entity
    {
        public long VendorId { get; set; }
        [ForeignKey("VendorId"), InverseProperty("Showrooms")]
        public virtual Vendor Vendor { get; set; }
        public string Name { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonPhone { get; set; }
        public string ContactPersonEmail { get; set; }
        public long? AddressId { get; set; }
        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; }
    }
}
