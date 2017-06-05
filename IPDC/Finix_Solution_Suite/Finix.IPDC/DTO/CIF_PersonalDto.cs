using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.DTO
{
    public class CIF_PersonalDto
    {
        public long? Id { get; set; }
        public string CIFNo { get; set; }
        public string CBSCIFNo { get; set; }
        public long? CustomersHomeBranch { get; set; } //foreign key to ipdc branch
        public long? RMId { get; set; }//foreign key to employee
        public long? RMCode { get; set; }//foreign key to employee
        public long? SalesLeadId { get; set; }        
        public ApplicationSource? ApplicationSource { get; set; }
        public string ApplicationSourceName { get; set; }
        public long? SourceRefId { get; set; }//figure out a table for source reference
        public string Title { get; set; }
        public string Name { get; set; }
        public string ConcatName { get; set; }
        public string FathersTitle { get; set; }
        public string FathersName { get; set; }
        public string MothersTitle { get; set; }
        public string MothersName { get; set; }
        public string PhotoName { get; set; }
        public string SignaturePhotoName { get; set; }
        public Gender? Gender { get; set; }
        public string GenderName { get; set; }
        public long? NationalityId { get; set; }
        public string NationalityName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string DateOfBirthText { get; set; }
        public long? BirthCountryId { get; set; }
        public string BirthCountryName { get; set; }
        public long? BirthDistrictId { get; set; }
        public string BirthDistrictName { get; set; }
        public string BirthDistrictForeign { get; set; }
        public long? PermanentAddressId { get; set; }
        public string PermanentAddressName { get; set; }
        public long? ResidenceAddressId { get; set; }
        public string ResidenceAddressName { get; set; }
        public ContactAddress? ContactAddress { get; set; }
        public string ContactAddressName { get; set; }
        public string NIDNo { get; set; }
        public string SmartNIDNo { get; set; }
        public string PassportNo { get; set; }
        public long? PassportIssueCountryId { get; set; }
        public string PassportIssueCountryName { get; set; }
        public DateTime? PassportIssueDate { get; set; }
        public string PassportIssueDateText { get; set; }
        public string DLNo { get; set; }
        public DateTime? DLIssueDate { get; set; }
        public string DLIssueDateTxt { get; set; }
        public long? DLIssueCountryId { get; set; }
        public string DLIssueCountryName { get; set; }
        public string BirthRegNo { get; set; }
        public string CommissionarCertificateNo { get; set; }
        public DateTime? CommCertIssueDate { get; set; }
        public string CommCertIssueDateText { get; set; }
        public string ETIN { get; set; }
        public MaritalStatus? MaritalStatus { get; set; }
        public string MaritalStatusName { get; set; }
        public string SpouseTitle { get; set; }
        public string SpouseName { get; set; }
        public string SpousePhoneNo { get; set; }
        public long? SpouseProfessionId { get; set; }
        public string SpouseProfessionName { get; set; }
        public bool? SpouseCompanyEnlisted { get; set; }
        public string SpouseCompanyName { get; set; }
        public string SpouseDesignation { get; set; }
        public long? SpouseCompanyId { get; set; }
        public long? SpouseWorkAddressId { get; set; }
        public string SpouseWorkAddressName { get; set; }
        public int NumberOfDependents { get; set; }
        public EducationLevel? HighestEducationLevel { get; set; }
        public string HighestEducationLevelName { get; set; }
        public ResidenceStatus? ResidenceStatus { get; set; }
        public string ResidenceStatusName { get; set; }
        public HomeOwnership? HomeOwnership { get; set; }
        public string HomeOwnershipName { get; set; }
        public YearsCurrentResidence? YearsInCurrentResidence { get; set; }
        public string YearsInCurrentResidenceName { get; set; }
        public long? OccupationId { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus Status { get; set; }
        public List<CreditCardDto> CreditCards { get; set; }
        public List<BankAccountDto> BankAccounts { get; set; }
        public AddressDto ResidenceAddress { get; set; }
        public AddressDto PermanentAddress { get; set; }
        public AddressDto SpouseWorkAddress { get; set; }
        public string PhoneNumbers { get; set; }
        public string test { get; set; }
        public HttpPostedFileBase Photo { get; set; }
        public HttpPostedFileBase SignaturePhoto { get; set; }
        public string ProfessionName { get; set; }
    }

    public class CreditCardDto
    {
        public long? Id { get; set; }
        public long? CIFId { get; set; }
        public string CreditCardNo { get; set; }
        public string CreditCardIssuersName { get; set; }
        public DateTime? CreditCardIssueDate { get; set; }
        public string CreditCardIssueDateText { get; set; }
        public decimal? CreditCardLimit { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus Status { get; set; }

    }

    public class BankAccountDto
    {
        public long? Id { get; set; }
        public long? CIFId { get; set; }
        public string AccountNo { get; set; }
        public string RoutingNo { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus Status { get; set; }
    }

    public class CIFCardAndBanksVM
    {
        public List<BankAccountDto> BankAccountDtos { get; set; }
        public List<long> RemovedBankAccounts { get; set; }
        public List<CreditCardDto> CreditCardDtos { get; set; }
        public List<long> RemovedCreditCards { get; set; }
        public long? UserId { get; set; }
    }
}