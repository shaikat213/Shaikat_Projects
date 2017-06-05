using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class Proposal_GuarantorDto
    {
        public long? Id { get; set; }
        public long? ProposalId { get; set; }
        public long? GuarantorCifId { get; set; }
        public CIF_KeyVal GuarantorCIF { get; set; }
        public string Name { get; set; }
        public string ProfessionName { get; set; }
        public string CompanyName { get; set; }
        public string Designation { get; set; }
        public RelationshipWithApplicant RelationshipWithApplicant { get; set; }
        public string RelationshipWithApplicantName { get; set; }
        public int? Age { get; set; }
        public decimal? MonthlyIncome { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus Status { get; set; }
        
    }
}
