using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Office")]
    public class Office : Entity
    {
        public Office()
        {
            SubOffices = new List<Office>();
        }
        public string Name { get; set; }
        public long? ParentId { get; set; }
        public long OfficeLayerId { get; set; }

        [ForeignKey("OfficeLayerId")]
        public virtual OfficeLayer OfficeLayer { get; set; }

        [ForeignKey("ParentId")]
        public virtual Office ParentOffice { get; set; }
        public virtual ICollection<Office> SubOffices { get; set; }
    }
}
