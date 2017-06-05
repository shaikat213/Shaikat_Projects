using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Proposal_Income")]
    public class Proposal_Income : Entity
    {
        public long ProposalId { get; set; }
        [ForeignKey("ProposalId"), InverseProperty("Incomes")]
        public virtual Proposal Proposal { get; set; }
        public string CIFNo { get; set; }
        public string Name { get; set; }
        public ApplicantRole ApplicantRole { get; set; }
        public bool IsConsidered { get; set; }
        public string IncomeSource { get; set; }
        public decimal ConsideredPercentage { get; set; }
        public decimal IncomeAmount { get; set; }
        public decimal ConsideredAmount { get; set; }
        public string Remarks { get; set; }
    }
}
