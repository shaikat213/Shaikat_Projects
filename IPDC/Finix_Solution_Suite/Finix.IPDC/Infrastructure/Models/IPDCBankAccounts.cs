using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("IPDCBankAccounts")]
    public class IPDCBankAccounts : Entity
    {
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string AccountNo { get; set; }
        public string RoutingNo { get; set; }
        public string CBS_GL_No { get; set; }
    }
}
