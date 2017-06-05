using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Proposal_ClientProfile")]
    public class Proposal_ClientProfile : Entity
    {
        public long ProposalId { get; set; }
        [ForeignKey("ProposalId"), InverseProperty("ClientProfiles")]
        public virtual Proposal Proposal { get; set; }
        public ApplicantRole ApplicantRole { get; set; }
        public long? CIFPId { get; set; }
        [ForeignKey("CIFPId")]
        public virtual CIF_Personal CIFP { get; set; }
        public long? CIFOId { get; set; }
        [ForeignKey("CIFOId")]
        public virtual CIF_Organizational CIFO { get; set; }
        public string CIFNo { get; set; }
        public string Name { get; set; }
        public string NID { get; set; }
        public int? Age { get; set; }
        public string SmartNIDNo { get; set; }
        public string PassportNo { get; set; }
        public string DLNo { get; set; }
        public RelationshipWithApplicant? RelationshipWithApplicant { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string AcademicQualification { get; set; }
        public string ProfessionName { get; set; }
        public string Designation { get; set; }
        public string OrganizationName { get; set; }
        public string ExperienceDetails { get; set; }
        public long? OfficeAddressId { get; set; }
        [ForeignKey("OfficeAddressId")]
        public virtual Address OfficeAddress { get; set; }
        public long? PresentAddressId { get; set; }
        [ForeignKey("PresentAddressId")]
        public virtual Address PresentAddress { get; set; }
        public long? PermanentAddressId { get; set; }
        [ForeignKey("PermanentAddressId")]
        public virtual Address PermanentAddress { get; set; }
        public HomeOwnership ResidenceStatus { get; set; }

    }
}
