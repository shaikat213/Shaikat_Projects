using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("OfficeDesignationSetting")]
    public class OfficeDesignationSetting : Entity
    {
        public string Name { get; set; }
        public long OfficeId { get; set; }
        public long DesignationId { get; set; }
        public long Sequence { get; set; }
        public long? ParentDesignationSettingId { get; set; }
        [ForeignKey("OfficeId")]
        public virtual Office Office { get; set; }
        [ForeignKey("DesignationId")]
        public virtual Designation Designation { get; set; }
        [ForeignKey("ParentDesignationSettingId")]
        public virtual OfficeDesignationSetting ParentOfficeDesignationSetting { get; set; }
    }
}
