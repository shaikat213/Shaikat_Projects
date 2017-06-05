using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class OfficeDesignationAreaDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? OfficeDesignationSettingId { get; set; }
        public UpozilaOrThana? UpozilaOrThana { get; set; }
        public long? RefId { get; set; }
    }
}
