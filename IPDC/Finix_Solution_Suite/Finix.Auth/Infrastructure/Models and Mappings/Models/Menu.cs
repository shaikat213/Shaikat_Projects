using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.Infrastructure
{
    [Table("Menu")]
    public class Menu : Entity
    {
        public Menu()
        {
            this.Roles = new List<Role>();
            this.Users = new List<User>();
        }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public int Sl { get; set; }
        public string Url { get; set; }
        public string HeadingText { get; set; }
        public string NoteHtml { get; set; }
        public long SubModuleId { get; set; }

        [ForeignKey("SubModuleId")]
        public virtual SubModule SubModule { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
