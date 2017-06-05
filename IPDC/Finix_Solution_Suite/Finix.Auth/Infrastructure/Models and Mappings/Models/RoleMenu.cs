using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.Infrastructure
{
    [Table("RoleMenu")]
    public class RoleMenu : Entity
    {
        public long RoleId { get; set; }
        public long MenuId { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
        [ForeignKey("MenuId")]
        public virtual Menu Menu { get; set; }
        public virtual ICollection<RoleMenuTask> RoleMenuTasks { get; set; }
    }
}
