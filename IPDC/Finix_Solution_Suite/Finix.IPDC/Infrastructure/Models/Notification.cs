using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Notification")]
    public class Notification : Entity
    {
        public NotificationType NotificationType { get;set; }
        public long? RefId { get; set; }
        public long? MenuId { get; set; }
        public string MenuName { get; set; }
        public string Url { get; set; }
        public string Parameters { get; set; }
        public long? UserId { get; set; }
        public NotificationStatusType NotificationStatusType { get; set; }
        public string Message { get; set; }
    }
}
