using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;



namespace Finix.IPDC.DTO
{
    public class LegalDocPropTypeDto 
    {
        public long? Id { get; set; }
        public long LegalDocumentId { get; set; }
        public string LegalDocumentName { get; set; }
        public LandType LandType { get; set; }
        public string LandTypeName { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus Status { get; set; }
    }
}
