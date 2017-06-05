using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.Infrastructure
{
    [Table("SubModule")]
    public class SubModule : Entity
    {
        public SubModule()
        {
            this.Menus = new List<Menu>();
        }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public int Sl { get; set; }
        public long ModuleId { get; set; }

        [ForeignKey("ModuleId")]
        public virtual Module Module { get; set; }
        public virtual ICollection<Menu> Menus { get; set; }
    }
}
