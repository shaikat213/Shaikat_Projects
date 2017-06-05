using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("District")]
    public class District :Entity
    {
        public long DivisionId { get; set; }
        public string DistrictNameEng { get; set; }
        public string DistrictNameBng { get; set; }
        public string BBSCode { get; set; }
        [ForeignKey("DivisionId"), InverseProperty("Districts")]
        public virtual Division Division { get; set; }
        public ICollection<Thana> Thanas { get; set; }
        public ICollection<Upazila> Upazilas { get; set; }
    }
}
