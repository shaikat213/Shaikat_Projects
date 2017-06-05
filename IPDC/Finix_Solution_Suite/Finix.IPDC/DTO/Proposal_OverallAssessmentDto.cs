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
    public class Proposal_OverallAssessmentDto
    {
        public long? Id { get; set; }
        public long? ProposalId { get; set; }
        public string AssessmentParticulars { get; set; }
        public OverallVerificationStatus VerificationStatus { get; set; }
        public string VerificationStatusName { get; set; }
        public string CIFName { get; set; }
        public string DoneBy { get; set; }
        public DateTime? AssessmentDate { get; set; }
        public string AssessmentDateTxt { get; set; }
        public string Remarks { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
