using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class OrganogramDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public long DesignationId { get; set; }
        public string DesignationName { get; set; }
        public long OfficeLayerId { get; set; }
        public long OfficeId { get; set; }
        public string OfficeName { get; set; }
        public long UnitType { get; set; }
        public long OfficeUnitId { get; set; }
        public string OfficeUnitName { get; set; }
        public long PositionId { get; set; }
        public string PositionName { get; set; }
        public long? ParentId { get; set; }
        public string ParentName { get; set; }
    }
}
