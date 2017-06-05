using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.DTO
{
    public class FundConfirmationDto
    {
        public long? Id { get; set; }
        public long? ApplicationId { get; set; }
        //public long? CurrentHolding { get; set; }
        public string ApplicationNo { get; set; }
        public string AccountTitle { get; set; }
        public string CustomerTypeName { get; set; }
        public string ProductName { get; set; }
        public long? ProposalId { get; set; }
        public List<FundConfirmationDetailsDto> Fundings { get; set; }
        public List<long> RemovedFundings { get; set; }
        public bool FundReceived { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
        public decimal? TotalDepositAmount { get; set; }
    }
}
