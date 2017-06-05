using Finix.IPDC.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class DPSMaturityScheduleDto
    {
        public long? Id { get; set; }
        public long ProductId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string EffectiveDateTxt { get; set; }
        public decimal InitialDeposit { get; set; }
        public decimal InstallmentAmount { get; set; }
        public int Term { get; set; }
        public decimal MaturityAmount { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
