using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.DTO
{
    public class Proposal_CIBDto
    {
        public long? Id { get; set; }
        public long? ProposalId { get; set; }
        public string CIFNo { get; set; }
        public VerificationAs ClientRole { get; set; }
        public string ClientRoleName { get; set; }
        public string Name { get; set; }
        public CIBClassificationStatus CIBStatus { get; set; }
        public string CIBStatusName { get; set; }
        public DateTime CIBDate { get; set; }
        public string CIBDateTxt { get; set; }
        public decimal TotalOutstandingAsBorrower { get; set; }
        public decimal ClassifiedAmountAsBorrower { get; set; }
        public decimal TotalEMIAsBorrower { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus Status { get; set; }
    }
}
