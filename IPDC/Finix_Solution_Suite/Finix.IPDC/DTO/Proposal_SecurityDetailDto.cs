using Finix.IPDC.Infrastructure;
using System;

namespace Finix.IPDC.DTO
{
    public class Proposal_SecurityDetailDto
    {
        public long? Id { get; set; }
        public long? ProposalId { get; set; }
        public ProposalSecurityDetailType SecurityType { get; set; }
        public string SecurityTypeName { get; set; }
        public string Details { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus Status { get; set; }
    }
}
