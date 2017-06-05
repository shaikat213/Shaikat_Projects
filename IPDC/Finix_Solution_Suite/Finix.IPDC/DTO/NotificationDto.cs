using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class NotificationDto
    {
        public long Id { get; set; }
        public NotificationType NotificationType { get; set; }
        public long? RefId { get; set; }
        public long? MenuId { get; set; }
        public string MenuName { get; set; }
        public string Url { get; set; }
        public string Parameters { get; set; }
        public long SubModuleId { get; set; }
        public long? UserId { get; set; }
        public string Message { get; set; }
        public NotificationStatusType NotificationStatusType { get; set; }
    }
}
