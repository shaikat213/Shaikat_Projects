using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.DTO
{
    public class RolePermissionDto
    {
        public long? AssignedBy { get; set; }
        public long? ProxyUserId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public List<string> Ids { get; set; }
        public long RoleId { get; set; }
    }
}
