using System;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class LoanAppWaiverReqDto
    {
        public long? Id { get; set; }
        public long LoanApplicationId { get; set; }
        public TypeOfWaiverReq WaiverType { get; set; }
        public long WaiverRequestedToId { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
