using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Proposal_Liability")]
    public class Proposal_Liability : Entity
    {
        public long ProposalId { get; set; }
        [ForeignKey("ProposalId"), InverseProperty("Liabilities")]
        public virtual Proposal Proposal { get; set; }
        public string Name { get; set; }
        public string FacilityType { get; set; }
        public string InstituteName { get; set; }
        public decimal Limit { get; set; }
        public decimal Outstanding { get; set; }
        public decimal EMI { get; set; }
        public string PaymentRecord { get; set; }
        public DateTime? StartingDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

    }
}
