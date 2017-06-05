using Finix.IPDC.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class CBSInfoDto
    {
        public CBSInfoDto()
        {
            CIFs = new List<CIF_PersonalDto>();
        }
        public long? ApplicationId { get; set; }
        public List<CIF_PersonalDto> CIFs { get; set; } 
        public DateTime? AccountOpenDate { get; set; }
        public string AccountOpenDateTxt { get; set; }
        public  string InstrumentNo { get; set; }
        public DateTime? InstrumentDate { get; set; }
        public string InstrumentDateText { get; set; }
        public string CBSAccountNo { get; set; }
        public string CBSBranchId { get; set; }
        public DateTime? MaturityDate { get; set; }
        public string MaturityDateTxt { get; set; }
        public InstrumentDispatchStatus? InstrumentDispatchStatus { get; set; }
        public decimal? MaturityAmount { get; set; }
    }
}
