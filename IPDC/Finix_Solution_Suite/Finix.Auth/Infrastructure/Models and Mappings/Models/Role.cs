using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.Infrastructure
{
    [Table("Role")]
    public class Role : Entity
    {
        public Role()
        {
            Tasks = new List<Task>();
            Users = new List<User>();
            Menus = new List<Menu>();

        }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Menu> Menus { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
