using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("OfficeLayer")]
    public class OfficeLayer : Entity
    {
        public OfficeLayer()
        {
            Offices = new List<Office>();
        }
        public string Name { get; set; }
        public int Level { get; set; }
        public virtual ICollection<Office> Offices { get; set; }
    }
}
