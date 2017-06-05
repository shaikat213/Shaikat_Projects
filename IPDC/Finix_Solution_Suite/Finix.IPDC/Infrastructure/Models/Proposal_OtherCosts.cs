using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Proposal_OtherCost")]
    public class Proposal_OtherCost : Entity
    {
        public long ProposalId { get; set; }
        [ForeignKey("ProposalId"), InverseProperty("OtherCosts")]
        public virtual Proposal Proposal { get; set; }
        public string Details { get; set; }
        public decimal Amount { get; set; }
    }
}
