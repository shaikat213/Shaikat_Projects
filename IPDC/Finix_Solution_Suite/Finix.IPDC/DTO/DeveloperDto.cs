using System;
using System.Collections.Generic;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class DeveloperDto
    {
        public long? Id { get; set; }
        public DeveloperType? DeveloperType { get; set; }
        public string DeveloperTypeName { get; set; }
        public string GroupName { get; set; }
        public List<DeveloperMemberDto> Members { get; set; }
        public List<long> RemovedMembers { get; set; }
        public string Website { get; set; }
        public string YearOfEstablishment { get; set; }
        public bool? MemberOfRehab { get; set; }
        public string RehabMemberNo { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonDesignation { get; set; }
        public string ContactPersonPhone { get; set; }
        public string ContactPersonEmail { get; set; }
        public List<DeveloperDirectorDto> Directors { get; set; }
        public List<long> RemovedDirectors { get; set; }
        public int? ArchitectCount { get; set; }
        public int? EngineerCount { get; set; }
        public int? MarketingEmpCount { get; set; }
        public int? OtherEmpCount { get; set; }
        //banking information
        public BankAccountType? TypeOfAccount { get; set; }
        public string TypeOfAccountName { get; set; }
        public string BankName { get; set; }
        public decimal? TotalLiabilityAmount { get; set; }
        public decimal? NumberOfCompleteProject { get; set; }
        public decimal? NumberOfOngoingProject { get; set; }
        public decimal? NumberOfUpcomingProject { get; set; }
        public DocumentStatus? DeveloperMEM { get; set; }
        public string DeveloperMEMName { get; set; }
        public DocumentStatus? DevelopreART { get; set; }
        public string DeveloperArtName { get; set; }
        public DocumentStatus? TradeLicence { get; set; }
        public string TradeLicenceName { get; set; }
        public DocumentStatus? FormXII { get; set; }
        public string FormXIIName { get; set; }
        public DocumentStatus? BoardResForSig { get; set; }
        public string BoardResForSigName { get; set; }
        public DocumentStatus? ContactPersonVCard { get; set; }
        public string ContactPersonVCardName { get; set; }
        public List<DeveloperDocumentDto> OtherDocuments { get; set; }
        public List<long> RemovedOtherDocuments { get; set; }
        public DeveloperEnlistmentStatus? EnlistmentStatus { get; set; }
        public string EnlistmentStatusName { get; set; }
        public DeveloperCategory? ApprovalCategory { get; set; }
        public string ApprovalCategoryName { get; set; }
        public string ApprovalRemarks { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
    }

    public class DeveloperMemberDto
    {
        public long? Id { get; set; }
        public long? DeveloperId { get; set; }
        //public DeveloperDto Developer { get; set; }
        public string Name { get; set; }
        public long? AddressId { get; set; }
        public AddressDto Address { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
    
    public class DeveloperDirectorDto
    {
        public long? Id { get; set; }
        public long? DeveloperId { get; set; }
        //public DeveloperDto Developer { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public EducationLevel? AcademicQualification { get; set; }
        public string AcademicQualificationName { get; set; }
        public decimal? SharePercentage { get; set; }
        public string BusinessExperience { get; set; }
        public string ContactNumber { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
    }
    
    public class DeveloperDocumentDto
    {
        public long? Id { get; set; }
        public long? DeveloperId { get; set; }
        //public DeveloperDto Developer { get; set; }
        public string Name { get; set; }
        public DocumentStatus? DocumentStatus { get; set; }
        public string DocumentStatusName { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
    }
}
