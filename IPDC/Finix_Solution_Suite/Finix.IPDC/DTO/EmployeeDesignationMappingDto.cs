using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class EmployeeDesignationMappingDto
    {
        public long? Id { get; set; }
        public long? EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmpCode { get; set; }
        public long? OfficeDesignationSettingId { get; set; }
        public string OfficeDesignationSettingName { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
    }
}
