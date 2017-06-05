using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class OfficePositionDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long OfficeLayerId { get; set; }
        public long? OfficeId { get; set; }
        public string OfficeName { get; set; }
        public long? DefaultDesignationId { get; set; }
        public string DefaultDesignationName { get; set; }
        public long PositionWeight { get; set; }
    }
}
