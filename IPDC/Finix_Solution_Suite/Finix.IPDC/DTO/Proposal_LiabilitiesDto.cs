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
    public class Proposal_LiabilityDto
    {
        public long? Id { get; set; }
        public long? ProposalId { get; set; }
        public string Name { get; set; }
        public string FacilityType { get; set; }
        public string InstituteName { get; set; }
        public decimal Limit { get; set; }
        public decimal Outstanding { get; set; }
        public decimal EMI { get; set; }
        public string PaymentRecord { get; set; }
        public DateTime? StartingDate { get; set; }
        public string StartingDateTxt { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string ExpiryDateTxt { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus Status { get; set; }
    }
}
