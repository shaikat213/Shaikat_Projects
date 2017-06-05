using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.DTO
{
    public class UserResourceDto
    {
        public long UserId { get; internal set; }
        public bool IsAdmin { get; internal set; }
        public List<ModuleDto> Modules { get; set; }
        public List<SubModuleDto> SubModules { get; set; }
        public List<MenuDto> Menus { get; set; }
        public List<RoleDto> Roles { get; set; }
        public List<TaskDto> Tasks { get; set; }
        public List<UserCompanyApplicationDto> UserCompanyApplications { get; set; }
        public List<CompanyProfileDto> CurrentlyAccessibleCompanies { get; set; }
        public long? SelectedCompanyId { get; set; }
        //public List<ApplicationDto> Applications { get; set; }
        public long? SelectedApplicationId { get; set; }

        public string UserName { get; set; }
        public long? EmployeeId { get; set; }
        public long? EmpDegMappingId { get; set; }
        public string ApiKey { get; set; }
    }

}
