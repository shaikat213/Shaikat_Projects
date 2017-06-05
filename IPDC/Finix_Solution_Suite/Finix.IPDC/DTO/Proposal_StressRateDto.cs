using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class Proposal_StressRateDto
    {
        public long? Id { get; set; }
        public long ProposalId { get; set; }
        public decimal InterestRate { get; set; }
        public decimal EMI { get; set; }
        public decimal Increase { get; set; }
        public decimal DBR { get; set; }
        public decimal DBRFlatPurchase { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus Status { get; set; }
    }
}
