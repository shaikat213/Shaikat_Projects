using Finix.IPDC.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class ContactPointVerificationDto
    {
        public long? Id { get; set; }
        public DateTime? VerificationDate { get; set; }
        public long? VerifiedByUserId { get; set; }
        public long? VerifiedByEmpDegMapId { get; set; }
        public long? CifId { get; set; }
        public string CIFNo { get; set; }
        public string CIFName { get; set; }
        public long? ApplicationId { get; set; }
        public string ApplicationNo{ get; set; }
        public string AccountTitle { get; set; }
        public VerificationAs VerificationPersonRole { get; set; }
        public string VerificationPersonRoleName { get; set; }
        public VerificationStatus? Photo { get; set; }
        public VerificationStatus? Name { get; set; }
        public VerificationStatus? MobileNo { get; set; }
        public VerificationStatus? ResidencePhone { get; set; }
        public VerificationStatus? OfficePhone { get; set; }
        public VerificationStatus? SignatureOfPhotoId { get; set; }
        public VerificationStatus? SignatureOfApplication { get; set; }
        public VerificationStatus? PresentAddress { get; set; }
        public ResidenceStatus? ResidenceStatus { get; set; }
        public LocationFindibility? LocationOfResidence { get; set; }
        public string MontylyRentAndUtilityExp { get; set; }
        public YearsCurrentResidence? YearInPresentAddress { get; set; }
        public LivingWith? LivingWith { get; set; }
        public VerificationStatus? PermanentAddress { get; set; }
        public string PersonContacted { get; set; }
        public RelationshipWithApplicant? RelationshipWithApplicant { get; set; }
        public string VerificationDateText { get; set; }
        //occupation information
        public VerificationStatus? ProfessionDeclared { get; set; }
        public long? ProfessionAssessedId { get; set; }
        public VerificationStatus? NameOfOrganization { get; set; }
        public VerificationStatus? AddressOfOrganization { get; set; }
        public string NatureOfBusiness { get; set; }
        public string YearOfEstablishment { get; set; }
        public VerificationStatus? TradeLicence { get; set; }
        public VerificationStatus? SalaryCertificate { get; set; }
        public VerificationStatus? PaySlip { get; set; }
        public string OtherIncomeSource { get; set; }
        public decimal? OtherIncomeDeclared { get; set; }
        public decimal? OtherIncomeVerified { get; set; }
        //public decimal? OtherIncomeVariance { get; set; }
        public string PersonContactedAtOffice { get; set; }
        public string PersonContactedAtOfficeDetails { get; set; }
        public List<BankAccountCpvDto> BankAccounts { get; set; }
        public List<ReferenceCpvDto> References { get; set; }
        public string Remarks { get; set; }
        public HomeOwnership? HomeOwnership { get; set; }
        public string HomeOwnershipName { get; set; }
        public VerificationState? VerificationStatus { get; set; }
        public long? EmpId { get; set; }
        public string PhotoName { get; set; }
        public string SignaturePhotoName { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }

    public class BankAccountCpvDto
    {
        public long? Id { get; set; }
        public long? CPVId { get; set; }
        public string BankName { get; set; }
        public string AccountNo { get; set; }
        public VerificationStatus? AccountVerification { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }

    public class ReferenceCpvDto
    {
        public long? Id { get; set; }
        public long? CPVId { get; set; }
        public string Name { get; set; }
        public long? CifReferenceId { get; set; }
        public VerificationStatus? Mobile { get; set; }
        public VerificationStatus? Phone { get; set; }
        public VerificationStatus? ResidenceStatus { get; set; }
        public VerificationStatus? ProfessionalInformation { get; set; }
        public VerificationStatus? RelationshipWithApplicant { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
