using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("CIF_Personal")]
    public class CIF_Personal : Entity
    {
        public string CIFNo { get; set; }
        public string CBSCIFNo { get; set; }
        public long? CustomersHomeBranch { get; set; } //foreign key to ipdc branch
        public long? RMId { get; set; }//foreign key to employee
        public string RMCode { get; set; }
        public long? SalesLeadId { get; set; }
        public ApplicationSource? ApplicationSource { get; set; }
        public long? SourceRefId { get; set; }//figure out a table for source reference
        public string Title { get; set; }
        
        public string Name { get; set; }
        public string Photo { get; set; }
        public string FathersTitle { get; set; }
        public string FathersName { get; set; }
        public string MothersTitle { get; set; }
        public string MothersName { get; set; }
        public Gender Gender { get; set; }
        public long? NationalityId { get; set; }

        [ForeignKey("NationalityId")]
        public virtual Nationality Nationality { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public long? BirthCountryId { get; set; }
        [ForeignKey("BirthCountryId")]
        public virtual Country BirthCountry { get; set; }
        public long? BirthDistrictId { get; set; }
        [ForeignKey("BirthDistrictId")]
        public virtual District BirthDistrict { get; set; }
        public string BirthDistrictForeign { get; set; }
        public long? PermanentAddressId { get; set; }
        [ForeignKey("PermanentAddressId")]
        public virtual Address PermanentAddress { get; set; }
        public long? ResidenceAddressId { get; set; }
        [ForeignKey("ResidenceAddressId")]
        public virtual Address ResidenceAddress { get; set; }
        public  ContactAddress? ContactAddress { get; set; }
        public string NIDNo { get; set; }
        public string SmartNIDNo { get; set; }
        public string PassportNo { get; set; }
        public long? PassportIssueCountryId { get; set; }
        [ForeignKey("PassportIssueCountryId")]
        public virtual Country PassportIssueCountry { get; set; }
        public DateTime? PassportIssueDate { get; set; }
        public string DLNo { get; set; }
        public DateTime? DLIssueDate { get; set; }
        public long? DLIssueCountryId { get; set; }
        [ForeignKey("DLIssueCountryId")]
        public virtual Country DLIssueCountry { get; set; }
        public string BirthRegNo { get; set; }
        public string CommissionarCertificateNo { get; set; }
        public DateTime? CommCertIssueDate { get; set; }
        public string ETIN { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public string SpouseTitle { get; set; }
        public string SpouseName { get; set; }
        public string SpousePhoneNo { get; set; }
        public long? SpouseProfessionId { get; set; }
        [ForeignKey("SpouseProfessionId")]
        public virtual Profession SpouseProfession { get; set; }
        //public bool? SpouseCompanyEnlisted { get; set; }
        //public string SpouseCompanyName { get; set; }
        public string SpouseDesignation { get; set; }
        public bool? SpouseCompanyEnlisted { get; set; }
        public string SpouseCompanyName { get; set; }
        public long? SpouseCompanyId { get; set; }
        [ForeignKey("SpouseCompanyId")]
        public virtual Organization SpouseCompany { get; set; }
        public long? SpouseWorkAddressId { get; set; }
        [ForeignKey("SpouseWorkAddressId")]
        public virtual Address SpouseWorkAddress { get; set; }

        public int NumberOfDependents { get; set; }
        public EducationLevel? HighestEducationLevel { get; set; }
        public ResidenceStatus ResidenceStatus { get; set; }
        public HomeOwnership? HomeOwnership { get; set; }
        public YearsCurrentResidence? YearsInCurrentResidence { get; set; }

        public virtual ICollection<CreditCard> CreditCards { get; set; }
        public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public virtual ICollection<CIF_Reference> References { get; set; }
        //only one will be active at a time
        public virtual ICollection<CIF_IncomeStatement> IncomeStatements { get; set; }
        public virtual ICollection<CIF_NetWorth> NetWorths { get; set; }
        public long? CIF_RPT_CodesId { get; set; }
        [ForeignKey("CIF_RPT_CodesId")]
        public virtual CIF_RPT_Codes CIF_RPT_Codes { get; set; }

        public long? OccupationId { get; set; }
        [ForeignKey("OccupationId")]
        public virtual Occupation Occupation { get; set; }

        public virtual ICollection<ContactPointVerification> ContactPointVerifications { get; set; }
        public virtual ICollection<CIB_Personal> CIBs { get; set; }
        public virtual ICollection<VisitReport> VisitReports { get; set; }
        public virtual ICollection<NIDVerification> NIDVerifications { get; set; }
        //public virtual ICollection<IncomeVerification> IncomeVerifications { get; set; }
        public string SignaturePhoto { get; set; }

    }

    [Table("CIF_RPT_Codes")]
    public class CIF_RPT_Codes : Entity
    {
        public string CIBSectorType { get; set; }
        public string CIBSectorCode { get; set; }
        public string NBDCCode { get; set; }
        public string NBFISectorType { get; set; }
        public string NBFISectorCode { get; set; }
        public string OptionalCode1 { get; set; }
        public string OptionalCode2 { get; set; }
        public string OptionalCode3 { get; set; }
        public string OptionalCode4 { get; set; }

    }
}
