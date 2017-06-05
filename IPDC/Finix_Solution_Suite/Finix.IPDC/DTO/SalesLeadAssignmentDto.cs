using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class SalesLeadAssignmentDto
    {
        public long? AssignedByOffDegId { get; set; }
        public long? AssignedToOffDegId { get; set; }
        public long? AssignedByEmpId { get; set; }
        public string AssignedByName { get; set; }
        public long? AssignedToEmpId { get; set; }
        public string AssignedToName { get; set; }
        public DateTime? FollowUpTime { get; set; }
        public string FollowUpTimeTxt { get; set; }
        public long? SalesLeadId { get; set; }
        public string SalesLeadName { get; set; }
        public string SalesLeadAddress { get; set; }
    }
}
