using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.DTO
{
    public class LegalDocumentVerificationDto
    {
        public long? Id { get; set; }
        public long? ApplicationId { get; set; }
        public string ApplicationNo { get; set; }
        public string ApplicationTitle { get; set; }
        public LandType? LandType { get; set; }
        public long? ProjectId { get; set; }
        public string LandTypeName { get; set; }
        public  List<LegalDocumentStatusDto> LegalDocuments { get; set; }
        public List<long> RemovedDocuments { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus Status { get; set; }
    }
}
