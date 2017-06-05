using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("ProposalAmendment")]
    public class ProposalAmendment : Entity
    {
        public long ProposalId { get; set; }
        [ForeignKey("ProposalId"), InverseProperty("Amendments")]
        public virtual Proposal Proposal { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public virtual ICollection<ProposalAmendment_ModeOfDisbursement> ModesOfDisbursement { get; set; }
        public virtual ICollection<ProposalAmendment_DisbursementCondition> DisbursementConditions { get; set; }
        public string PresentIssue { get; set; }
        public string ProposedAmendment { get; set; }
        public string Recomendation { get; set; }
        public virtual ICollection<ProposalAmendment_Signatory> Signatories { get; set; }
    }
    [Table("ProposalAmendment_ModeOfDisbursement")]
    public class ProposalAmendment_ModeOfDisbursement : Entity
    {
        public long ProposalAmendmentId { get; set; }
        [ForeignKey("ProposalAmendmentId"), InverseProperty("ModesOfDisbursement")]
        public virtual ProposalAmendment Proposal { get; set; }
        public string Text { get; set; }
    }
    [Table("ProposalAmendment_DisbursementCondition")]
    public class ProposalAmendment_DisbursementCondition : Entity
    {
        public long ProposalAmendmentId { get; set; }
        [ForeignKey("ProposalAmendmentId"), InverseProperty("DisbursementConditions")]
        public virtual ProposalAmendment Proposal { get; set; }
        public string Text { get; set; }
    }


    [Table("ProposalAmendment_Signatory")]
    public class ProposalAmendment_Signatory : Entity
    {
        public long ProposalAmendmentId { get; set; }
        [ForeignKey("ProposalAmendmentId"), InverseProperty("Signatories")]
        public virtual ProposalAmendment Proposal { get; set; }
        public long SignatoryId { get; set; }
        [ForeignKey("SignatoryId")]
        public virtual Signatories Signatory { get; set; }

    }
}
