using System;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class LoanAppColSecurityDto
    {
        public long? Id { get; set; }
        public long LoanApplicationId { get; set; }
        public long ColSecurityId { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string SecurityDescription { get; set; }
        public bool IsMandatory { get; set; } //IsChecked
        public bool? IsChecked { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
