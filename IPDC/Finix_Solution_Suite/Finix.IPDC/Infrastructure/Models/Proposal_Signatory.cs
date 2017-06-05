using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Proposal_Signatory")]
    public class Proposal_Signatory : Entity
    {
        public long ProposalId { get; set; }
        [ForeignKey("ProposalId"), InverseProperty("Signatories")]
        public virtual Proposal Proposal { get; set; }
        public long SignatoryId { get; set; }
        [ForeignKey("SignatoryId")]
        public virtual Signatories Signatory { get; set; }
        //public virtual ApprovalAuthorityGroupDetail Signatory { get; set; }

    }
    
}
