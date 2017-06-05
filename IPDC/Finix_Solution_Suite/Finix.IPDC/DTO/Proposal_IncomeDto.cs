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
    public class Proposal_IncomeDto
    {
        public long? Id { get; set; }
        public long? ProposalId { get; set; }
        public string CIFNo { get; set; }
        public string Name { get; set; }
        public ApplicantRole ApplicantRole { get; set; }
        public string ApplicantRoleName { get; set; }
        public bool IsConsidered { get; set; }
        public string IncomeSource { get; set; }
        public decimal IncomeAmount { get; set; }
        public decimal ConsideredPercentage { get; set; }
        public decimal ConsideredAmount { get; set; }
        public string Remarks { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus Status { get; set; }
    }
}
