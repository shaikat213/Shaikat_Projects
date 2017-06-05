using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.Infrastructure
{
    [Table("RoleMenuTask")]
    public class RoleMenuTask : Entity
    {
        public long RoleMenuId { get; set; }
        public long TaskId { get; set; }
        public bool Read { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        [ForeignKey("TaskId")]
        public virtual Task Task { get; set; }
        [ForeignKey("RoleMenuId"), InverseProperty("RoleMenuTasks")]
        public virtual RoleMenu RoleMenu { get; set; }
    }
}
