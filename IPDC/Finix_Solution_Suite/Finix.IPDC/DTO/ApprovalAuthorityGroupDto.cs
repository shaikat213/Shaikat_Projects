using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class ApprovalAuthorityGroupDto
    {
        public string Name { get; set; }
        public List<ApprovalAuthorityGroupDetailDto> AuthorityGroupDetails { get; set; }
    }

    public class ApprovalAuthorityGroupDetailDto
    {
        public long ApprovalAuthorityGroupId { get; set; }
        public long DesignationSettingId { get; set; }
    }
}
