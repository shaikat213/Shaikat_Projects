using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    public class LegalDocumentStatus : Entity
    {
        public long LegalDocumentVerificationId { get; set; }
        public long LegalDocumentId { get; set; }
        [ForeignKey("LegalDocumentId")]
        public virtual LegalDocument LegalDocument { get; set; }
        public VerificationStatus VerificationStatus { get; set; }

        [ForeignKey("LegalDocumentVerificationId"), InverseProperty("LegalDocuments")]
        public virtual LegalDocumentVerification LegalDocumentVerification { get; set; }
    }
}
