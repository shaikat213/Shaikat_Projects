using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Division")]
    public class Division : Entity
    {
        public long? CountryId { get; set; }
        public string DivisionNameEng { get; set; }
        public string DivisionNameBng { get; set; }
        public string BBSCode { get; set; }
        public virtual ICollection<District> Districts { get; set; }
        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; }
    }
}
