using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class NSMAgingDto
    {
        public long BranchId { get; set; }
        public string BranchName { get; set; }
        public decimal FirstCall { get; set; }
        public decimal FirstLead { get; set; }
        public decimal FirstDataCollection { get; set; }
        public decimal FirstSanction { get; set; }
        public decimal FirstDisbursment { get; set; }
        public decimal FirstTotal { get; set; }
        public decimal ScdCall { get; set; }
        public decimal ScdLead { get; set; }
        public decimal ScdDataCollection { get; set; }
        public decimal ScdSanction { get; set; }
        public decimal ScdDisbursment { get; set; }
        public decimal ScdTotal { get; set; }
        public decimal GrthCall { get; set; }
        public decimal GrthLead { get; set; }
        public decimal GrthDataCollection { get; set; }
        public decimal GrthSanction { get; set; }
        public decimal GrthDisbursment { get; set; }
        public decimal GrthTotal { get; set; }
    }
}
