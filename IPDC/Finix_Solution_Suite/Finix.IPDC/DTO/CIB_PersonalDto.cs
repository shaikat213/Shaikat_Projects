using Finix.IPDC.Infrastructure;
using System;
using System.Web;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.DTO
{
    public class CIB_PersonalDto
    {
        public long? Id { get; set; }
        public long? CIF_PersonalId { get; set; }
        public string CIFNo { get; set; }
        public string CIFName { get; set; }
        public string CIFFathersName { get; set; }
        public string CIFMothersName { get; set; }
        public decimal? AppliedAmount { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string BirthDistrictName { get; set; }
        public string NIDNo { get; set; }
        public string PassportNo { get; set; }
        public string DLNo { get; set; }
        public string PermanentAddress { get; set; }
        public DateTime? VerificationDate { get; set; }
        public  string VerificationDateTxt { get; set; }
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
        public decimal? ExposureInBusiness { get; set; }
        public CIBClassificationStatus? CIBClassificationStatusOfBusiness { get; set; }
        public string CIBClassificationStatusOfBusinessName { get; set; }
        public long? EmpId { get; set; }
        public HttpPostedFileBase CIBReportFile { get; set; }
        public string CIBReport { get; set; }
        public string CIBReportFileName { get; set; }
        //public string PassportNo { get; set; }
        public string BirthRegNo { get; set; }
        public string SmartNIDNo { get; set; }
        public long? PassportIssueCountryId { get; set; }
        public string PassportIssueCountryName { get; set; }
        public DateTime? PassportIssueDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
    }
}
