using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class OfficeDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long OfficeLayerId { get; set; }
        public string OfficeLayerName { get; set; }
        public long? ParentId { get; set; }
        public string ParentName { get; set; }
    }
}
