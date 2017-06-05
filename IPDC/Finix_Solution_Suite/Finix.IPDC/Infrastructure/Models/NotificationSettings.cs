using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("NotificationSettings")]
    public class NotificationSettings : Entity
    {
        public NotificationType NotificationType { get; set; }
        public long? FunctionalDesignationId { get; set; }
        [ForeignKey("FunctionalDesignationId")]
        public virtual OfficeDesignationSetting FunctionalDesignation { get; set; }
        public long? EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
        public long? RoleId { get; set; }
        public ProposalFacilityType? ProposalFacilityType { get; set; }
    }
}
