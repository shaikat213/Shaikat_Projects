using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    public class DesignationProductMapping : Entity
    {
        public long ProductId { get; set; }
        public long OfficeDesignationSettingId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        [ForeignKey("OfficeDesignationSettingId")]
        public virtual OfficeDesignationSetting OfficeDesignationSetting { get; set; }
    }
}
