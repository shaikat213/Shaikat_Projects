using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("EmployeeDesignationMapping")]
    public class EmployeeDesignationMapping : Entity
    {
        public long EmployeeId { get; set; }
        public long OfficeDesignationSettingId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("OfficeDesignationSettingId")]
        public virtual OfficeDesignationSetting OfficeDesignationSetting { get; set; }

    }
}
