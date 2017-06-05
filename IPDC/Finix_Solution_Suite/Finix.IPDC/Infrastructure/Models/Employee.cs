using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Employee")]
    public class Employee : Entity
    {
        public long? PersonId { get; set; }
        [ForeignKey("PersonId")]
        public virtual Person Person { get; set; }
        public string EmpCode { get; set; }
        public string RmCode { get; set; }
        public string IMEINo { get; set; }
        public DateTime? JoiningDate { get; set; }
        public EmployeeType EmployeeType { get; set; }
        public long? CompanyProfileId { get; set; }

    }
}
