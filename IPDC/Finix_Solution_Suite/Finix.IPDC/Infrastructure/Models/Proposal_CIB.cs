using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Proposal_CIB")]
    public class Proposal_CIB : Entity
    {
        public long ProposalId { get; set; }
        [ForeignKey("ProposalId"), InverseProperty("CIBs")]
        public virtual Proposal Proposal { get; set; }
        public string CIFNo { get; set; }
        public VerificationAs ClientRole { get; set; }
        public string Name { get; set; }
        public CIBClassificationStatus CIBStatus { get; set; }
        public DateTime CIBDate { get; set; }
        public decimal TotalOutstandingAsBorrower { get; set; }
        public decimal ClassifiedAmountAsBorrower { get; set; }
        public decimal TotalEMIAsBorrower { get; set; }
    }
}
