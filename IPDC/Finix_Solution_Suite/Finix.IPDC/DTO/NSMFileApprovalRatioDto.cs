using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class NSMFileApprovalRatioDto
    {
        public long BranchId { get; set; }
        public string BranchName { get; set; }
        public decimal FirstCallConverSionSuccessful { get; set; }
        public decimal FirstCallConverSionUnSuccessful { get; set; }
        public decimal FirstCallConverSionRate { get; set; }
        public decimal FirstLeadConverSionSuccessful { get; set; }
        public decimal FirstLeadConverSionUnSuccessful { get; set; }
        public decimal FirstLeadConverSionRate { get; set; }
        public decimal FirstFileApproved { get; set; }
        public decimal FirstFileDisapproved { get; set; }
        public decimal FirstApplicationApprovalRate { get; set; }
        public decimal ScdCallConverSionSuccessful { get; set; }
        public decimal ScdCallConverSionUnSuccessful { get; set; }
        public decimal ScdCallConverSionRate { get; set; }
        public decimal ScdLeadConverSionSuccessful { get; set; }
        public decimal ScdLeadConverSionUnSuccessful { get; set; }
        public decimal ScdLeadConverSionRate { get; set; }
        public decimal ScdFileApproved { get; set; }
        public decimal ScdFileDisapproved { get; set; }
        public decimal ScdApplicationApprovalRate { get; set; }

        public decimal GrowthCallConversationRate { get; set; }
        public decimal GrowthLeadConversationRate { get; set; }
        public decimal GrowthAppApprovalConversationRate { get; set; }
    }
}
