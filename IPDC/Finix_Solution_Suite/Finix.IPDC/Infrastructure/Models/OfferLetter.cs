using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    public class OfferLetter : Entity
    {
        public long ProposalId { get; set; }
        [ForeignKey("ProposalId")]
        public virtual Proposal Proposal { get; set; }
        public ProposalFacilityType FacilityType { get; set; }       
        public string OfferLetterNo { get; set; }
        public DateTime OfferLetterDate { get; set; }
        public decimal PenalInterest { get; set; }
        public int? BankAccount { get; set; }
        public virtual ICollection<OfferLetterText> OfferLetterTexts { get; set; }
        public decimal LoanAdvance { get; set; }
        public int AcceptancePeriod { get; set; }
        public Purpose Purpose { get; set; }
        public decimal CibAndProcessingFee { get; set; }
        public bool? CRMIsApproved { get; set; }
        public DateTime? CRMApprovalDate { get; set; }
        public bool? OPSIsApproved { get; set; }
        public DateTime? OPSApprovalDate { get; set; }
        public bool? CUSIsApproved { get; set; }
        public DateTime? CUSApprovalDate { get; set; }
        public virtual ICollection<OfferLetterAmendment> Amendments { get; set; }
    }
}
