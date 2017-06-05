using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    public class OfferLetterText : Entity
    {
        public long OfferLetterId { get; set; }
        [ForeignKey("OfferLetterId")]
        public virtual OfferLetter OfferLetter { get; set; }
        public string Text { get; set; }
        public OfferTextType? OfferTextType { get; set; }
        public PrinterFiltering? PrinterFiltering { get; set; }
    }
}
