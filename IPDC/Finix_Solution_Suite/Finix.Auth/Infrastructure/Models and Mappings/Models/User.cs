using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.Infrastructure
{

    [Table("User")]
    public class User : Entity
    {
        public User()
        {
            Roles = new List<Role>();
            Menus = new List<Menu>();
        }

        public string UserName { get; set; }
        public string Password { get; set; }
        //public string IMEI { get; set; }
        public long? EmployeeId { get; set; }
        public bool IsActive { get; set; }
        //[ForeignKey("EmployeeId")]
        //public virtual Employee Employee { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<Menu> Menus { get; set; }
        public virtual ICollection<UserCompanyApplication> UserCompanyApplications { get; set; }
    }
}
