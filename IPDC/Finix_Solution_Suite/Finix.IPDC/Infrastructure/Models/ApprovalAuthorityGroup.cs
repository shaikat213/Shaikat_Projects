using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("ApprovalAuthorityGroup")]
    public class ApprovalAuthorityGroup : Entity
    {
        public string Name { get; set; }
        public virtual ICollection<ApprovalAuthorityGroupDetail> AuthorityGroupDetails { get; set; }
    }

    
    
}
