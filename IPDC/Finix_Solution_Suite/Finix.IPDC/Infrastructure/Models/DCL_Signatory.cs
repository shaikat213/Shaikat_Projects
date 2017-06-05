using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("DCL_Signatory")]
    public class DCL_Signatory : Entity
    {
        public long DCLId { get; set; }
        [ForeignKey("DCLId"), InverseProperty("Signatories")]
        public virtual DocumentCheckList DocumentCheckList { get; set; }
        public long SignatoryId { get; set; }
        [ForeignKey("SignatoryId")]
        public virtual Signatories Signatory { get; set; }
    }
}
