using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    public class Organization : Entity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public long PhoneNumber { get; set; }
        public string ContactPerson { get; set; }
        public OrganizationType OrganizationType { get; set; }
        public string Comment { get; set; }
        public Priority Priority { get; set; }
    }
}
