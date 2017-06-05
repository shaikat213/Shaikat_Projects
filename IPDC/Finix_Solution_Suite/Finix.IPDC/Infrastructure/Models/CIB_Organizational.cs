using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("CIB_Organizational")]
    public class CIB_Organizational : Entity
    {
        public long CIF_OrgId { get; set; }
        [ForeignKey("CIF_OrgId"), InverseProperty("CIBs")]
        public virtual CIF_Organizational CIF { get; set; }
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
        public CIBClassificationStatus? CIBClassificationStatusOfDirectors { get; set; }
        public CIBClassificationStatus? CIBClassificationStatusOfDirectorsConcern { get; set; }
        public long? EmpId { get; set; }
        [ForeignKey("EmpId")]
        public virtual Employee Employee { get; set; }
    }
}
