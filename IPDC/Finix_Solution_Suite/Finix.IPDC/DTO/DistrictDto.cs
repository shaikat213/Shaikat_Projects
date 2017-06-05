using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class DistrictDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long DivisionId { get; set; }
        public string DistrictNameEng { get; set; }
        public string DistrictNameBng { get; set; }
        public string BBSCode { get; set; }
    }
}
