using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    public class OfficeDesignationArea : Entity
    {
        public string Name { get; set; }
        public long? OfficeDesignationSettingId { get; set; }
        public UpozilaOrThana? UpozilaOrThana { get; set; }
        public long? RefId { get; set; }
    }
}
