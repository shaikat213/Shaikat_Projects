using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Finix.IPDC.Infrastructure.Models
{
    public class DesignationRoleMapping : Finix.IPDC.Infrastructure.Entity
    { 
        public long OfficeDesignationSettingId { get; set; }
        public long RoleId { get; set; }
        
        [ForeignKey("OfficeDesignationSettingId")]
        public virtual OfficeDesignationSetting OfficeDesignationSetting { get; set; }
    }
}
