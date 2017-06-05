using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    public class SalesLeadAssignment : Entity
    {
        public long? AssignedByEmpId { get; set; }
        [ForeignKey("AssignedByEmpId")]
        public virtual Employee AssignedByEmp { get; set; }
        public long? AssignedToEmpId { get; set; }
        [ForeignKey("AssignedToEmpId")]
        public virtual Employee AssignedToEmp { get; set; }
        public long? AssignedByOffDegId { get; set; }
        [ForeignKey("AssignedByOffDegId")]
        public virtual OfficeDesignationSetting AssignedByOffDeg { get; set; }
        public long? AssignedToOffDegId { get; set; }
        [ForeignKey("AssignedToOffDegId")]
        public virtual  OfficeDesignationSetting AssignedToOffDeg { get; set; }
        public DateTime FollowUpTime { get; set; }
        public long SalesLeadId { get; set; }
        [ForeignKey("SalesLeadId")]
        public virtual SalesLead SalesLead { get; set; }
    }
}
