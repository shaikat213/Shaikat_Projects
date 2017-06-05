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
    public class FundConfirmationDetailsDto
    {
        public long? Id { get; set; }
        public long? FundConfirmationId { get; set; }
        public long? IPDCBankAccountId { get; set; }
        public DateTime? CreditDate { get; set; }
        public string CreditDateText { get; set; }
        public decimal Amount { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
