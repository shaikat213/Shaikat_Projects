using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("FundConfirmation")]
    public class FundConfirmation : Entity
    {
        public long? ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public virtual Application Application { get; set; }
        public long? ProposalId { get; set; }
        [ForeignKey("ProposalId")]
        public virtual Proposal Proposal { get; set; }
        public virtual ICollection<FundConfirmationDetail> Fundings { get; set; }
        public bool? FundReceived { get; set; }
    }

    [Table("FundConfirmationDetail")]
    public class FundConfirmationDetail : Entity
    {
        public long? FundConfirmationId { get; set; }
        [ForeignKey("FundConfirmationId"), InverseProperty("Fundings")]
        public virtual FundConfirmation FundConfirmation { get; set; }
        public long? IPDCBankAccountId { get; set; }
        [ForeignKey("IPDCBankAccountId")]
        public virtual IPDCBankAccounts IPDCBankAccount { get; set; }
        public DateTime? CreditDate { get; set; }
        public decimal? Amount { get; set; }
    }
}
