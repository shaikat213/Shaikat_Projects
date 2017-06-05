using Finix.IPDC.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class OrganizationDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public long PhoneNumber { get; set; }
        public string ContactPerson { get; set; }
        public OrganizationType OrganizationType { get; set; }
        public string OrganizationTypeName { get; set; }
        public string Comment { get; set; }
        public Priority Priority { get; set; }
        public string PriorityName { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus Status { get; set; }
    }
}
