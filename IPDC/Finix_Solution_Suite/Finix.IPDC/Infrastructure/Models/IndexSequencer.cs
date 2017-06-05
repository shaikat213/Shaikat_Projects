using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("IndexSequencer")]
    public class IndexSequencer : Entity
    {
        public string CIFNumber { get; set; }
        public string ApplicationNumber { get; set; }
        public string AccGroupId { get; set; }
        public string CreditMemoNo { get; set; }
        public string CallNo { get; set; }
        public string OfferLetterNo { get; set; }
        public string DCLNo { get; set; }
        public string DMNo { get; set; }
    }
}
