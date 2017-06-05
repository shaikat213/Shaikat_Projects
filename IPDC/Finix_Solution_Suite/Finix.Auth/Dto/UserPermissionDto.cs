using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.DTO
{
    public class UserPermissionDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long ModuleId { get; set; }
        public long SubModuleId { get; set; }
        public long MenuId { get; set; }
        public long MenuActionId { get; set; }
    }
}
