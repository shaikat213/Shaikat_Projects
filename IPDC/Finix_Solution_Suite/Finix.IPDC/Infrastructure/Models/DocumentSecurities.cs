using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    public class DocumentSecurities : Entity
    {
        public long DCLId { get; set; }
        [ForeignKey("DCLId"), InverseProperty("Securities")]
        public DocumentCheckList DCL { get; set; }
        public string SecurityDescription { get; set; }
        public string Value { get; set; }
    }
}
