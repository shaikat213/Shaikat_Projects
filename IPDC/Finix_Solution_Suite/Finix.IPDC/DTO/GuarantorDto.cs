using Finix.IPDC.Infrastructure;
using System;

namespace Finix.IPDC.DTO
{
    public class GuarantorDto
    {
        public long? Id { get; set; }
        public long? ApplicationId { get; set; }
        public long LoanApplicationId { get; set; }
        //public virtual LoanApplication LoanApplication { get; set; }
        public long GuarantorCifId { get; set; }
        public CIF_KeyVal GuarantorCif { get; set; }
        public decimal GuaranteeAmount { get; set; }
        public int? Age { get; set; }
        public string GuarantorName { get; set; }
        public string CIFNo { get; set; }
        public string CBSCIFNo { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
        public RelationshipWithApplicant? RelationshipWithApplicant { get; set; }
        public string RelationshipWithApplicantName { get; set; }
    }
}