using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Proposal_StressRate")]
    public class Proposal_StressRate : Entity
    {
        public long ProposalId { get; set; }
        [ForeignKey("ProposalId"), InverseProperty("StressRates")]
        public virtual Proposal Proposal { get; set; }
        public decimal InterestRate { get; set; }
        public decimal EMI { get; set; }
        public decimal Increase { get; set; }
        public decimal DBR { get; set; }
        public decimal DBRFlatPurchase { get; set; }
    }
}
