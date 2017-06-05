using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Proposal_Guarantor")]
    public class Proposal_Guarantor : Entity
    {
        public long ProposalId { get; set; }
        [ForeignKey("ProposalId"), InverseProperty("Guarantors")]
        public virtual Proposal Proposal { get; set; }
        public long? GuarantorCifId { get; set; }
        [ForeignKey("GuarantorCifId")]
        public virtual CIF_Personal GuarantorCIF { get; set; }
        public string Name { get; set; }
        public string ProfessionName { get; set; }
        public string CompanyName { get; set; }
        public string Designation { get; set; }
        public RelationshipWithApplicant RelationshipWithApplicant { get; set; }
        public int Age { get; set; }
        public decimal MonthlyIncome { get; set; }
    }
}
