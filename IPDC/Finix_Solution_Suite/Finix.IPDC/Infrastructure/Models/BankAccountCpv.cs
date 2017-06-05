using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("BankAccountCpv")]
    public class BankAccountCpv : Entity
    {
        public long CPVId { get; set; }
        [ForeignKey("CPVId"), InverseProperty("BankAccounts")]
        public virtual ContactPointVerification CPV { get; set; }
        public string BankName { get; set; }
        public string AccountNo { get; set; }
        public VerificationStatus AccountVerification { get; set; }
    }
}
