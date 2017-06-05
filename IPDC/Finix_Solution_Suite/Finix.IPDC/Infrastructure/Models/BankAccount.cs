using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("BankAccount")]
    public class BankAccount : Entity
    {
        public long CIFId { get; set; }
        public string AccountNo { get; set; }
        public string RoutingNo { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        [ForeignKey("CIFId"), InverseProperty("BankAccounts")]
        public virtual CIF_Personal CIF { get; set; }
    }
}
