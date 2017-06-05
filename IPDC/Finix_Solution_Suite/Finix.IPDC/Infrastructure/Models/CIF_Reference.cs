using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("CIF_Reference")]
    public class CIF_Reference : Entity
    {
        public long CIF_PersonalId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public bool? EnlistedOrganization { get; set; }
        public string OrganizationName { get; set; }
        public long? OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }
        public long? OfficeAddressId { get; set; }
        [ForeignKey("OfficeAddressId")]
        public virtual Address OfficeAddress { get; set; }
        public string RelationshipWithApplicant { get; set; }
        public long? ResidenceAddressId { get; set; }
        [ForeignKey("ResidenceAddressId")]
        public virtual Address ResidenceAddress { get; set; }
        public long? PermanentAddressId { get; set; }
        [ForeignKey("PermanentAddressId")]
        public virtual Address PermanentAddress { get; set; }
        [ForeignKey("CIF_PersonalId"), InverseProperty("References")]
        public virtual CIF_Personal CIF_Personal { get; set; }
    }
}
