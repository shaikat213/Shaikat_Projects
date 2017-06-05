using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.DTO
{
    public class DivisionDto
    {
        public long Id { get; set; }
        public string DivisionNameEng { get; set; }
        public string DivisionNameBng { get; set; }
        public string BBSCode { get; set; }
        public long? CountryId { get; set; }
        public string CountryName { get; set; }
        //public virtual ICollection<District> Districts { get; set; }
    }
}
