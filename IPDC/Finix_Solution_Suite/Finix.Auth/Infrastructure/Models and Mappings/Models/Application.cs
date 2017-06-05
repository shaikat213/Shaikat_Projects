using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.Infrastructure
{
    [Table("Application")]
    public class Application : Entity
    {
        public Application()
        {
            this.Modules = new List<Module>();
        }
        public string Name { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Module> Modules { get; set; }
    }
}
