using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("ContactPointVerification")]
    public class ContactPointVerification : Entity
    {
        public DateTime? VerificationDate { get; set; }
        public long? VerifiedByUserId { get; set; }
        public long? VerifiedByEmpDegMapId { get; set; }
        public long? CifId { get; set; }
        [ForeignKey("CifId"), InverseProperty("ContactPointVerifications")]
        public virtual CIF_Personal CifPersonal { get; set; }
        public long? ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public virtual Application Application { get; set; }
        public VerificationAs VerificationPersonRole { get; set; }

        public VerificationStatus Photo { get; set; }
        public VerificationStatus Name { get; set; }
        public VerificationStatus MobileNo { get; set; }
        public VerificationStatus ResidencePhone { get; set; }
        public VerificationStatus OfficePhone { get; set; }
        public VerificationStatus SignatureOfPhotoId { get; set; }
        public VerificationStatus SignatureOfApplication { get; set; }
        public VerificationStatus PresentAddress { get; set; }
        public ResidenceStatus ResidenceStatus { get; set; }
        public LocationFindibility? LocationOfResidence { get; set; }
        public string MontylyRentAndUtilityExp { get; set; }
        public YearsCurrentResidence? YearInPresentAddress { get; set; }
        public LivingWith? LivingWith { get; set; }
        public VerificationStatus PermanentAddress { get; set; }
        public string PersonContacted { get; set; }
        public RelationshipWithApplicant? RelationshipWithApplicant { get; set; }

        //occupation information
        public VerificationStatus ProfessionDeclared { get; set; }
        public long? ProfessionAssessedId { get; set; }
        [ForeignKey("ProfessionAssessedId")]
        public virtual Profession ProfessionAssessed { get; set; }
        public VerificationStatus NameOfOrganization { get; set; }
        public VerificationStatus AddressOfOrganization { get; set; }
        public string NatureOfBusiness { get; set; }
        public string YearOfEstablishment { get; set; }
        public VerificationStatus TradeLicence { get; set; }
        public VerificationStatus SalaryCertificate { get; set; }
        public VerificationStatus PaySlip { get; set; }
        public string OtherIncomeSource { get; set; }
        public decimal? OtherIncomeDeclared { get; set; }
        public decimal? OtherIncomeVerified { get; set; }
        //public decimal? OtherIncomeVariance { get; set; }
        public string PersonContactedAtOffice { get; set; }
        public string PersonContactedAtOfficeDetails { get; set; }
        public virtual ICollection<BankAccountCpv> BankAccounts { get; set; }
        public virtual ICollection<ReferenceCpv> References { get; set; }
        public string Remarks { get; set; }
        public HomeOwnership? HomeOwnership { get; set; }
        public VerificationState VerificationStatus { get; set; }
        public long? EmpId { get; set; }
        [ForeignKey("EmpId")]
        public virtual Employee Employee { get; set; }
    }
}
