using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class PoApprovalDto
    {
        public long? ApplicationId { get; set; }
        public PersonType PersonType { get; set; }
        public DateTime? QuotationDate { get; set; }
        public string QuotationDateTxt { get; set; }
        public long? PoId { get; set; }

    }
}
