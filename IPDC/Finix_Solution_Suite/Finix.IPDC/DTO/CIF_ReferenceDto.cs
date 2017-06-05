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
    
    public class CIF_ReferenceDto
    {
        public long? Id { get; set; }
        public string CIFNo { get; set; }
        public long? CIF_PersonalId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public bool? EnlistedOrganization { get; set; }
        public string OrganizationName { get; set; }
        public long? OrganizationId { get; set; }        
        public OrganizationDto Organization { get; set; }
        public long? OfficeAddressId { get; set; }        
        public AddressDto OfficeAddress { get; set; }
        public string RelationshipWithApplicant { get; set; }
        public long? ResidenceAddressId { get; set; }        
        public AddressDto ResidenceAddress { get; set; }
        public long? PermanentAddressId { get; set; }        
        public AddressDto PermanentAddress { get; set; }        
        //public CIF_PersonalDto CIF_Personal { get; set; }
        public bool IsRemoved { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }

    }
}
