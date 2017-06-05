using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class ApprovalRatioTimelineWiseDto
    {
        public long BranchId { get; set; }
        public string BranchName { get; set; }
        public decimal CallConverSionSuccessful { get; set; }
        public decimal CallConverSionUnSuccessful { get; set; }
        public decimal CallConverSionRate { get; set; }
        public decimal LeadConverSionSuccessful { get; set; }
        public decimal LeadConverSionUnSuccessful { get; set; }
        public decimal LeadConverSionRate { get; set; }
        public decimal FileApproved { get; set; }
        public decimal FileDisapproved { get; set; }
        public decimal ApplicationApprovalRate { get; set; }
        //public decimal CallConverSionRate { get; set; }
        //public decimal LeadConverSionRate { get; set; }
        //public decimal ApplicationApprovalRate { get; set; }

    }
}
