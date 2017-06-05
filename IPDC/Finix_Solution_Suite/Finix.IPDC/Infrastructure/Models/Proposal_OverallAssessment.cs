using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Proposal_OverallAssessment")]
    public class Proposal_OverallAssessment : Entity
    {
        public long ProposalId { get; set; }
        [ForeignKey("ProposalId"), InverseProperty("OverallAssessments")]
        public virtual Proposal Proposal { get; set; }
        public string AssessmentParticulars { get; set; }
        public OverallVerificationStatus VerificationStatus { get; set; }
        public string CIFName { get; set; }
        public string DoneBy { get; set; }
        public DateTime AssessmentDate { get; set; }
        public string Remarks { get; set; }
    }
}
