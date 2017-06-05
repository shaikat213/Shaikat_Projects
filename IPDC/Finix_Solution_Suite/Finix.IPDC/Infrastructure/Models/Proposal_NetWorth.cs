using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Proposal_NetWorth")]
    public class Proposal_NetWorth : Entity
    {
        public long ProposalId { get; set; }
        [ForeignKey("ProposalId"), InverseProperty("NetWorths")]
        public virtual Proposal Proposal { get; set; }
        public string CIFNo { get; set; }
        public string Name { get; set; }
        public VerificationAs ClientRole { get; set; }
        public decimal TotalAssetOfApplicant { get; set; }
        public decimal TotalLiabilityOfApplicant { get; set; }
        public decimal NetWorthOfApplicant { get; set; }
    }
}
