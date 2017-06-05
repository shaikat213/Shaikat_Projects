using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class Proposal_FDRDto
    {
        public long? Id { get; set; }
        public long? ProposalId { get; set; }
        public string InstituteName { get; set; }
        public string BranchName { get; set; }
        public string FDRAccountNo { get; set; }
        public decimal Amount { get; set; }
        public decimal? Rate { get; set; }
        public string DepositorName { get; set; }
        public DateTime? MaturityDate { get; set; }
        public string MaturityDateTxt { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus Status { get; set; }
    }
}
