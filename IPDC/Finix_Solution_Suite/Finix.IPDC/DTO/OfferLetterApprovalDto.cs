using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class OfferLetterApprovalDto
    {
        public long? Id { get; set; }
        public bool? IsApproved { get; set; }
        public long? ProposalId { get; set; }
        public long? ApplicationId { get; set; }
        public string ApplicationNo { get; set; }
        public DateTime? QuotationDate { get; set; }
        public string QuotationDateTxt { get; set; }
        public OfferLetterType? OfferLetterType { get; set; }
    }
}
