using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure.Models;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class FDRPSDetailDto
    {
        public long? Id { get; set; }
        public long LoanApplicationId { get; set; }
        public long FDRPrimarySecurityId { get; set; }
        //public virtual FDRPrimarySecurity FDRPrimarySecurity { get; set; }
        public string FDRAccountNo { get; set; }
        public decimal Amount { get; set; }
        public string Depositor { get; set; }
        public DateTime MaturityDate { get; set; }
        public string MaturityDateTxt { get; set; }
        public string RelationshipWithApplicant { get; set; }
        public DisbursementTo? DisbursementTo { get; set; }
        public string DisbursementToName { get; set; }
        public string InstituteName { get; set; }
        public string BranchName { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
