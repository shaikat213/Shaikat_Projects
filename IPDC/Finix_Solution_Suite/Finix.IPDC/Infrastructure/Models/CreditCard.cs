using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("CreditCard")]
    public class CreditCard : Entity
    {
        public long CIFId { get; set; }
        public string CreditCardNo { get; set; }
        public string CreditCardIssuersName { get; set; }
        public DateTime? CreditCardIssueDate { get; set; }
        public decimal? CreditCardLimit { get; set; }
        [ForeignKey("CIFId"), InverseProperty("CreditCards")]
        public virtual CIF_Personal CIF { get; set; }
    }
}
