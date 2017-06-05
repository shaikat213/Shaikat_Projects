using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Upazila")]
    public class Upazila : Entity
    {
        public string UpazilaNameEng { get; set; }
        public string UpazilaNameBng { get; set; }
        public string BBSCode { get; set; }
        public long DistrictId { get; set; }
        [ForeignKey("DistrictId"), InverseProperty("Upazilas")]
        public virtual District District { get; set; }
    }
}
