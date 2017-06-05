using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    public class LegalDocumentVerification : Entity
    {
        public LandType? LandType { get; set; }
        public long? ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
        public virtual ICollection<LegalDocumentStatus> LegalDocuments { get; set; }
    }
}
