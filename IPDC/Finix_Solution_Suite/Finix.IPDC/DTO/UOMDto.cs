using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class UOMDto
    {
        public long Id { get; set; }    
        public UomType UomType { get; set; }
        public string Name { get; set; }
    }
}
