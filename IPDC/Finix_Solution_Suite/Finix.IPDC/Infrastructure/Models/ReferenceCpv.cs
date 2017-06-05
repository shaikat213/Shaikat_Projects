using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("ReferenceCpv")]
    public class ReferenceCpv : Entity
    {
        public long CPVId { get; set; }
        [ForeignKey("CPVId"), InverseProperty("References")]
        public virtual ContactPointVerification CPV { get; set; }
        public long CifReferenceId { get; set; }
        [ForeignKey("CifReferenceId")]
        public virtual CIF_Reference CifReference { get; set; }
        public VerificationStatus Mobile { get; set; }
        public VerificationStatus Phone { get; set; }
        public VerificationStatus ResidenceStatus { get; set; }
        public VerificationStatus ProfessionalInformation { get; set; }
        public VerificationStatus RelationshipWithApplicant { get; set; }
    }
}
