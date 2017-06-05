using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class OfficeUnitDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int UnitType { get; set; }
        public string UnitTypeName { get; set; }
        public long? ParentId { get; set; }
        public string ParentName { get; set; }
        public bool? Checked { get; set; }
    }
}
