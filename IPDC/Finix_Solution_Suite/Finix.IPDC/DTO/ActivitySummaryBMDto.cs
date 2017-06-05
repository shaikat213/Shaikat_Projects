using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class ActivitySummaryBMDto
    {
        public long? Id { get; set; }    
        public string Name { get; set; }
        public long? EmployeeId { get; set; }
        public long? ParentEmployeeId { get; set; }
        public string ParentEmployeeName { get; set; }
        public long? CallQty { get; set; }
        public long? LeadSubmittedQty { get; set; }
        public decimal? LeadSubmittedAmt { get; set; }
        public long? FileSubmittedQty { get; set; }
        public decimal? FileSubmittedAmt { get; set; }
        public long? FileDisbursedQty { get; set; }
        public decimal? FileDisbursedAmt { get; set; }
        public long? FileRejectedQty { get; set; }
        public decimal? FileRejectedAmt { get; set; }
        public List<ActivitySummaryBMDto>  Children { get; set; }

    }
}
