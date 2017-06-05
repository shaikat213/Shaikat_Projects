using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class ThanaDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string ThanaNameEng { get; set; }
        public string ThanaNameBng { get; set; }
        public string BBSCode { get; set; }
        public long DistrictId { get; set; }
    }
}
