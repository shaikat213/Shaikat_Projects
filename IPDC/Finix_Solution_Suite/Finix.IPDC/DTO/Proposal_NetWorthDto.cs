using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class Proposal_NetWorthDto
    {
        public long? CIFId { get; set; }
        public long? Id { get; set; }
        public long? ProposalId { get; set; }
        public string CIFNo { get; set; }
        public string Name { get; set; }
        public VerificationAs ClientRole { get; set; }
        public string ClientRoleName { get; set; }
        public decimal TotalAssetOfApplicant { get; set; }
        public decimal TotalLiabilityOfApplicant { get; set; }
        public decimal NetWorthOfApplicant { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus Status { get; set; }
    }
}
