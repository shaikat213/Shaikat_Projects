using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class ApprovalRatioDto
    {
        public decimal CallConverSionSuccessfulFt { get; set; }
        public decimal CallConverSionUnSuccessfulFt { get; set; }
        public decimal CallConverSionRateFt { get; set; }
        public decimal LeadConverSionSuccessfulFt { get; set; }
        public decimal LeadConverSionUnSuccessfulFt { get; set; }
        public decimal LeadConverSionRateFt { get; set; }
        public decimal FileApprovedFt { get; set; }
        public decimal FileDisapprovedFt { get; set; }
        public decimal ApplicationApprovalRateFt { get; set; }

        public decimal CallConverSionSuccessfulSt { get; set; }
        public decimal CallConverSionUnSuccessfulSt { get; set; }
        public decimal CallConverSionRateSt { get; set; }
        public decimal LeadConverSionSuccessfulSt { get; set; }
        public decimal LeadConverSionUnSuccessfulSt { get; set; }
        public decimal LeadConverSionRateSt { get; set; }
        public decimal FileApprovedSt { get; set; }
        public decimal FileDisapprovedSt { get; set; }
        public decimal ApplicationApprovalRateSt { get; set; }

        public decimal CallConversationGrowth { get; set; }
        public decimal LeadConversationGrowth { get; set; }
        public decimal ApplicationApprovalGrowth { get; set; }
    }
}
