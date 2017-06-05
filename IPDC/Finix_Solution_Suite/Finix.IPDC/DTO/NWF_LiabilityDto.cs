using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class NWV_LiabilityDto
    {
        public long? Id { get; set; }
        public long? NWV_NetWorthId { get; set; }
        public LoanType? LoanType { get; set; }
        public decimal? LoanAmountOrLimit { get; set; }
        public decimal? InstallmentAmount { get; set; }
        public int? Term { get; set; }//in months
        public int? RemainingTerm { get; set; }
        public decimal? OutstandingAmount { get; set; }
        public string BankOrFIName { get; set; }
        public LiabilityType? LiabilityType { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus Status { get; set; }
    }
}
