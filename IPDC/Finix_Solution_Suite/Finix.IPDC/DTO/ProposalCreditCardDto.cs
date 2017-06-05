using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.DTO
{
    public class ProposalCreditCardDto
    {
        public long? Id { get; set; }
        public long? CreditCardId { get; set; }
        public long ProposalId { get; set; }
        public long? CIFId { get; set; }
        public string CreditCardNo { get; set; }
        public string CreditCardIssuersName { get; set; }
        public DateTime? CreditCardIssueDate { get; set; }
        public string CreditCardIssueDateTxt { get; set; }
        public decimal? CreditCardLimit { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus Status { get; set; }
        //public virtual CIF_Personal CIF { get; set; }
        //public virtual Proposal Proposal { get; set; }
    }
}
