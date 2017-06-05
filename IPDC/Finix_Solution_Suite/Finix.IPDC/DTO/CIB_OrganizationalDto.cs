using Finix.IPDC.Infrastructure;
using System;

namespace Finix.IPDC.DTO
{
    public class CIB_OrganizationalDto
    {
        public long? Id { get; set; }
        public long? CIF_OrgId { get; set; }
        public string CifOrgNo { get; set; }
        public string CifOrgName { get; set; }
        public DateTime? VerificationDate { get; set; }
        public string VerificationDateTxt { get; set; }
        public long? VerifiedByUserId { get; set; }
        public long? VerifiedByEmpDegMapId { get; set; }
        public long? ApplicationId { get; set; }
        public string ApplicationNo { get; set; }
        public string AccountTitle { get; set; }
        public VerificationAs? VerificationPersonRole { get; set; }
        public string VerificationPersonRoleName { get; set; }
        public int? NoOfLivingContactsAsBorrower { get; set; }
        public decimal? TotalOutstandingAsBorrower { get; set; }
        public decimal? ClassifiedAmountAsBorrower { get; set; }
        public decimal? TotalEMIAsBorrower { get; set; }
        public CIBClassificationStatus? CIBClassificationStatusAsBorrower { get; set; }
        public string CIBClassificationStatusAsBorrowerName { get; set; }
        public int? NoOfLivingContactsAsGuarantor { get; set; }
        public decimal? TotalOutstandingAsGuarantor { get; set; }
        public decimal? ClassifiedAmountAsGuarantor { get; set; }
        public decimal? TotalEMIAsGuarantor { get; set; }
        public CIBClassificationStatus? CIBClassificationStatusAsGuarantor { get; set; }
        public string CIBClassificationStatusAsGuarantorName { get; set; }
        public CIBClassificationStatus? CIBClassificationStatusOfDirectors { get; set; }
        public string CIBClassificationStatusOfDirectorsName { get; set; }
        public CIBClassificationStatus? CIBClassificationStatusOfDirectorsConcern { get; set; }
        public string CIBClassificationStatusOfDirectorsConcernName { get; set; }
        public long? EmpId { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
    }
}
