using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.Infrastructure
{

    [Table("Task")]
    public class Task : Entity
    {
        public Task()
        {
            Roles = new List<Role>();
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<Menu> Menus { get; set; }
    }
}
