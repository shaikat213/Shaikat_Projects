using Finix.IPDC.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class ProductRatesDto
    {
        public long? Id { get; set; }
        public long ProductId { get; set; }
        //public virtual Product Product { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string EffectiveDateTxt { get; set; }
        public decimal CardRate { get; set; }
        public decimal PositiveVariance { get; set; }
        public decimal NegativeVariance { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
