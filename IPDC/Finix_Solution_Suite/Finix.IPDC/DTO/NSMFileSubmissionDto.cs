using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class NSMFileSubmissionDto
    {
        public long BranchId { get; set; }
        public string BranchName { get; set; }
        public int FirstNoOfFiles { get; set; }
        public decimal FirstAmount { get; set; }
        public decimal FirstWAR { get; set; }
        public decimal FirstContribution { get; set; }
        public int SecondNoOfFiles { get; set; }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      
        public decimal SecondAmount { get; set; }
        public decimal SecondWAR { get; set; }
        public decimal SecondContribution { get; set; }
        public int GrowthNoOfFiles { get; set; }
        public decimal GrowthAmount { get; set; }
        public decimal GrowthWAR { get; set; }
        public decimal GrowthContribution { get; set; }
        //public List<PerformerDto> FirstTimelineData { get; set; }
        //public List<PerformerDto> SecondTimelineData { get; set; }
        //public List<PerformerDto> GrowthData { get; set; }
    }
}
