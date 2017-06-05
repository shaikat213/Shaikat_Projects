using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("UOM")]
    public class UOM : Entity
    {
        public UomType UomType { get; set; }
        public string Name { get; set; }
    }
}
