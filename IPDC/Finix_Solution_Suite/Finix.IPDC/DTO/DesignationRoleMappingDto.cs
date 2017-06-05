using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class DesignationRoleMappingDto
    {
        public long Id { get; set; }
        public long OfficeDesignationSettingId { get; set; }
        public string OfficeDesignationSettingName { get; set; }
        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus Status { get; set; }
    }
}
