using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class OfficeDesignationSettingDto
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public long? EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string RoleName { get; set; }
        public long OfficeId { get; set; }
        public string OfficeName { get; set; }
        //public long UnitType { get; set; }
        public long DesignationId { get; set; }
        public string DesignationName { get; set; }
        public long? ParentDesignationSettingId { get; set; }
        public string ParentDesignationSettingName { get; set; }
        public int Sequence { get; set; }
        public long? ParentEmployeeId { get; set; }
        public string ParentEmployeeName { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public string IMEI { get; set; }
        
    }
}
