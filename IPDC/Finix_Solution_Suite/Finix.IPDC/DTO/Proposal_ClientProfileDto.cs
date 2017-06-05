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
    public class Proposal_ClientProfileDto
    {
        public long? Id { get; set; }
        public long? CIFPId { get; set; }
        public CIF_KeyVal CIFP { get; set; }
        public long? CIFOId { get; set; }
        public CIF_KeyVal CIFO { get; set; }
        public long? ProposalId { get; set; }
        public ApplicantRole? ApplicantRole { get; set; }
        public string ApplicantRoleName { get; set; }
        public string CIFNo { get; set; }
        public string Name { get; set; }
        public string NID { get; set; }
        public int? Age { get; set; }
        public string SmartNIDNo { get; set; }
        public string PassportNo { get; set; }
        public string DLNo { get; set; }
        public RelationshipWithApplicant RelationshipWithApplicant { get; set; }
        public string RelationshipWithApplicantName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string DateOfBirthTxt { get; set; }
        public string AcademicQualification { get; set; }
        public string ProfessionName { get; set; }
        public string Designation { get; set; }
        public string OrganizationName { get; set; }
        public string ExperienceDetails { get; set; }

        public long? OfficeAddressId { get; set; }
        public  AddressDto OfficeAddress { get; set; }
        public string OfficeAddressString { get; set; }
        public long? PresentAddressId { get; set; }
        public  AddressDto PresentAddress { get; set; }
        public string PresentAddressString { get; set; }
        public long? PermanentAddressId { get; set; }
        public  AddressDto PermanentAddress { get; set; }
        public string PermanentAddressString { get; set; }
        public HomeOwnership ResidenceStatus { get; set; }
        public string ResidenceStatusName { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
        
    }
}
