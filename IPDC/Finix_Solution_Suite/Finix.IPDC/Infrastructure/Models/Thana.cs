using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Thana")]
    public class Thana : Entity
    {
        public string ThanaNameEng { get; set; }
        public string ThanaNameBng { get; set; }
        public string BBSCode { get; set; }
        public long DistrictId { get; set; }
        [ForeignKey("DistrictId"), InverseProperty("Thanas")]
        public virtual District District { get; set; }
    }
}
