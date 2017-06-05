using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("CIB_Personal")]
    public class CIB_Personal : Entity
    {
        public long CIF_PersonalId { get; set; }
        [ForeignKey("CIF_PersonalId"), InverseProperty("CIBs")]
        public virtual CIF_Personal CIF { get; set; }
        public string CIBReport { get; set; }
        public DateTime? VerificationDate { get; set; }
        public long? VerifiedByUserId { get; set; }
        public long? VerifiedByEmpDegMapId { get; set; }
        public long? ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public virtual Application Application { get; set; }
        public VerificationAs VerificationPersonRole { get; set; }
        public int? NoOfLivingContactsAsBorrower { get; set; }
        public decimal? TotalOutstandingAsBorrower { get; set; }
        public decimal? ClassifiedAmountAsBorrower { get; set; }
        public decimal? TotalEMIAsBorrower { get; set; }
        public CIBClassificationStatus? CIBClassificationStatusAsBorrower { get; set; }
        public int? NoOfLivingContactsAsGuarantor { get; set; }
        public decimal? TotalOutstandingAsGuarantor { get; set; }
        public decimal? ClassifiedAmountAsGuarantor { get; set; }
        public decimal? TotalEMIAsGuarantor { get; set; }
        public CIBClassificationStatus? CIBClassificationStatusAsGuarantor { get; set; }
        public decimal? ExposureInBusiness { get; set; }
        public CIBClassificationStatus? CIBClassificationStatusOfBusiness { get; set; }
        public long? EmpId { get; set; }
        [ForeignKey("EmpId")]
        public virtual Employee Employee { get; set; }
    }
}
