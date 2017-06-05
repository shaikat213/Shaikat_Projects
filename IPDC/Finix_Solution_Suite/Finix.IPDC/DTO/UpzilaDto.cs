using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class UpzilaDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string UpazilaNameEng { get; set; }
        public string UpazilaNameBng { get; set; }
        public string BBSCode { get; set; }
        public long DistrictId { get; set; }
    }
}
