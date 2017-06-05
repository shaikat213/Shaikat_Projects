using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    public class ProposalCreditCard : Entity
    {
        public long? CreditCardId { get; set; }
        public long ProposalId { get; set; }
        public long? CIFId { get; set; }
        public string CreditCardNo { get; set; }
        public string CreditCardIssuersName { get; set; }
        public DateTime? CreditCardIssueDate { get; set; }
        public decimal? CreditCardLimit { get; set; }
        [ForeignKey("CIFId")]
        public virtual CIF_Personal CIF { get; set; }
        [ForeignKey("CreditCardId")]
        public virtual CreditCard CreditCard { get; set; }
        [ForeignKey("ProposalId"), InverseProperty("ProposalCreditCards")]
        public virtual Proposal Proposal { get; set; }
    }
}
