using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class Proposal_Valuation_OtherCostDto
    {
        public long? Id { get; set; }
        public long? ProposalId { get; set; }
        public string Details { get; set; }
        public decimal Amount { get; set; }
        public OtherCostEnum? Other { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus Status { get; set; }
    }
}
