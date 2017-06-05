using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.Infrastructure
{
    [Table("UserProxyRoleMenu")]
    public class UserProxyRoleMenu : Entity
    {
        public long UserProxyId { get; set; }
        public long RoleId { get; set; }
        public long MenuId { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
        [ForeignKey("MenuId")]
        public virtual Menu Menu { get; set; }
        [ForeignKey("UserProxyId"), InverseProperty("ProxyRoleMenus")]
        public virtual UserProxy UserProxy { get; set; }
        public virtual ICollection<UserProxyRoleMenuTask> RoleMenuTasks { get; set; }
    }
}
