using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class OfferLetterTextDto
    {
        public long? Id { get; set; }
        public long OfferLetterId { get; set; }
        public string Text { get; set; }
        public OfferTextType? OfferTextType { get; set; }
        public string OfferTextTypeName { get; set; }
        public PrinterFiltering? PrinterFiltering { get; set; }
        public string PrinterFilteringName { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
