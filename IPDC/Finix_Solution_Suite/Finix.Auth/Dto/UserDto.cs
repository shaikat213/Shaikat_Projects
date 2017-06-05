using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.DTO
{
    public class UserDto
    {
        public long? Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public long? EmployeeId { get; set; }
        public bool? IsActive { get; set; }
        public long? RoleId { get; set; }
        //public EmployeeDto Employee { get; set; }
        public long? CompanyProfileId { get; set; }
        public IEnumerable<UserPermissionDto> UserPermissions { get; set; }
        public IEnumerable<ModuleDto> Modules { get; set; }
        public IEnumerable<SubModuleDto> SubModules { get; set; }
        public string IMEI { get; set; }
    }
}
