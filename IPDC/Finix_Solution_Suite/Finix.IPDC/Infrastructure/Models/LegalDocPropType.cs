using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    public class LegalDocPropType : Entity
    {
        public long LegalDocumentId { get; set; }
        public LandType LandType { get; set; }
        [ForeignKey("LegalDocumentId")]
        public virtual LegalDocument LegalDocument { get; set; }
    }
}
