using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Proposal_FDR")]
    public class Proposal_FDR : Entity
    {
        public long ProposalId { get; set; }
        [ForeignKey("ProposalId"), InverseProperty("FDRs")]
        public virtual Proposal Proposal { get; set; }
        public string InstituteName { get; set; }
        public string BranchName { get; set; }
        public string FDRAccountNo { get; set; }
        public decimal Amount { get; set; }
        public decimal? Rate { get; set; }
        public string DepositorName { get; set; }
        public DateTime MaturityDate { get; set; }
    }
}
