using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("DepositApplicationTracking")]
    public class DepositApplicationTracking : Entity
    {
        public long ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public virtual Application Application { get; set; }
        public long DepositApplicationId { get; set; }
        [ForeignKey("DepositApplicationId")]
        public virtual DepositApplication DepositApplication { get; set; }
        public InstrumentDeliveryStatus? InstrumentDeliveryStatus { get; set; }
        public WelcomeLetterStatus? WelcomeLetterStatus { get; set; }
        public DateTime? ChangeDate { get; set; }

    }
}
