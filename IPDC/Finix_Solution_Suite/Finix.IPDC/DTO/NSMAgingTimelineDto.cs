using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class NSMAgingTimelineDto
    {
        public long BranchId { get; set; }
        public string BranchName { get; set; }
        public double Call { get; set; }
        public double Lead { get; set; }
        public double DataCollection { get; set; }
        public double Sanction { get; set; }
        public double Disbursment { get; set; }
        public double Total { get; set; }
    }
}
