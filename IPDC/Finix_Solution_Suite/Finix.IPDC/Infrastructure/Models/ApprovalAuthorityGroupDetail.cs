using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("ApprovalAuthorityGroupDetail")]
    public class ApprovalAuthorityGroupDetail : Entity
    {
        public long ApprovalAuthorityGroupId { get; set; }
        [ForeignKey("ApprovalAuthorityGroupId"), InverseProperty("AuthorityGroupDetails")]
        public virtual ApprovalAuthorityGroup ApprovalAuthorityGroup { get; set; }
        public long DesignationSettingId { get; set; }
        [ForeignKey("DesignationSettingId")]
        public virtual OfficeDesignationSetting OfficeDesignationSetting { get; set; }

    }
}
