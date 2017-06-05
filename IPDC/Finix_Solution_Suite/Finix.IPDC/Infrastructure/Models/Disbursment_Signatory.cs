using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Disbursment_Signatory")]
    public class Disbursment_Signatory : Entity
    {
        public long DMId { get; set; }
        [ForeignKey("DMId"), InverseProperty("Signatories")]
        public virtual DisbursementMemo DisbursementMemo { get; set; }
        public long SignatoryId { get; set; }
        [ForeignKey("SignatoryId")]
        public virtual Signatories Signatory { get; set; }
    }
}
