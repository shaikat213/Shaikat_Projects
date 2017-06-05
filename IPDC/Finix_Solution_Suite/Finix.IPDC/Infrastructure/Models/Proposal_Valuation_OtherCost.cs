using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Proposal_Valuation_OtherCost")]
    public class Proposal_Valuation_OtherCost : Entity
    {
        public long ProposalId { get; set; }
        [ForeignKey("ProposalId"), InverseProperty("ValuationOtherCosts")]
        public virtual Proposal Proposal { get; set; }
        public string Details { get; set; }
        public decimal Amount { get; set; }
    }
}
