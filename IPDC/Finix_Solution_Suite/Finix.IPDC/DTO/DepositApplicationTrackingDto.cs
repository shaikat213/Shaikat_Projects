using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class DepositApplicationTrackingDto
    {
        public long? Id { get; set; }
        public long? ApplicationId { get; set; }
        public string ApplicationNo { get; set; }
        public string AccountTitle { get; set; }
        public long? DepositApplicationId { get; set; }
        public decimal? MaturityAmount { get; set; }
        public InstrumentDeliveryStatus? InstrumentDeliveryStatus { get; set; }
        public string InstrumentDeliveryStatusName { get; set; }
        public WelcomeLetterStatus? WelcomeLetterStatus { get; set; }
        public string WelcomeLetterStatusName { get; set; }
        public DateTime? ChangeDate { get; set; }
        public string ChangeDateText { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus Status { get; set; }
    }
}
