using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.Infrastructure
{
    [Table("UserProxy")]
    public class UserProxy : Entity
    {
        public long AssignedBy { get; set; }
        public long ProxyUserId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public virtual ICollection<UserProxyRoleMenu> ProxyRoleMenus { get; set; }
    }
}
