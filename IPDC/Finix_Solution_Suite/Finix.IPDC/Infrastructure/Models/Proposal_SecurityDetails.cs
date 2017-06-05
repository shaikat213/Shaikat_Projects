using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Proposal_SecurityDetail")]
    public class Proposal_SecurityDetail : Entity
    {
        public long ProposalId { get; set; }
        [ForeignKey("ProposalId"), InverseProperty("SecurityDetails")]
        public virtual Proposal Proposal { get; set; }
        public ProposalSecurityDetailType SecurityType { get; set; }
        public string Details { get; set; }
    }
}
