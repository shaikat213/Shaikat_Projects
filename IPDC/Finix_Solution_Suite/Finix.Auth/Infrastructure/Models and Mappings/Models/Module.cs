using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.Infrastructure
{
    [Table("Module")]
    public class Module : Entity
    {
        public Module()
        {
            this.SubModules = new List<SubModule>();
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public int Sl { get; set; }
        //public long? ApplicationId { get; set; }
        public virtual ICollection<SubModule> SubModules { get; set; }
        [ForeignKey("ApplicationId")]
        public virtual ICollection<Application> Applications { get; set; }
    }
}
