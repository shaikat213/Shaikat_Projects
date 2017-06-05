using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;


namespace Finix.IPDC.DTO
{
    public class LegalDocumentStatusDto 
    {
        public long? Id { get; set; }       
        public long LegalDocumentVerificationId { get; set; }
        public long LegalDocumentId { get; set; }
        public string LegalDocumentName { get; set; }
        //public LegalDocumentDto LegalDocument { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus Status { get; set; }

    }
}
