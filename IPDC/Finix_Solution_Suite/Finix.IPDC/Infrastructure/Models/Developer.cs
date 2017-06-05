using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Developer")]
    public class Developer : Entity
    {
        public DeveloperType DeveloperType { get; set; }
        public string GroupName { get; set; }
        public virtual ICollection<DeveloperMember> Members { get; set; }
        public string Website { get; set; }
        public string YearOfEstablishment { get; set; }
        public bool MemberOfRehab { get; set; }
        public string RehabMemberNo { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonDesignation { get; set; }
        public string ContactPersonPhone { get; set; }
        public string ContactPersonEmail { get; set; }
        public virtual ICollection<DeveloperDirector> Directors { get; set; }
        public int? ArchitectCount { get; set; }
        public int? EngineerCount { get; set; }
        public int? MarketingEmpCount { get; set; }
        public int? OtherEmpCount { get; set; }
        //banking information
        public BankAccountType? TypeOfAccount { get; set; }
        public string BankName { get; set; }
        public decimal? TotalLiabilityAmount { get; set; }
        //public virtual ICollection<Project> Projects { get; set; }

        public decimal? NumberOfCompleteProject { get; set; }
        public decimal? NumberOfOngoingProject { get; set; }
        public decimal? NumberOfUpcomingProject { get; set; }

        public DocumentStatus DeveloperMEM { get; set; }
        public DocumentStatus DevelopreART { get; set; }
        public DocumentStatus TradeLicence { get; set; }
        public DocumentStatus FormXII { get; set; }
        public DocumentStatus BoardResForSig { get; set; }
        public DocumentStatus ContactPersonVCard { get; set; }
        public virtual ICollection<DeveloperDocument> OtherDocuments { get; set; }
        public DeveloperEnlistmentStatus EnlistmentStatus { get; set; }
        public DeveloperCategory? ApprovalCategory { get; set; }
        public string ApprovalRemarks { get; set; }

    }
    [Table("DeveloperMember")]
    public class DeveloperMember : Entity
    {
        public long DeveloperId { get; set; }
        [ForeignKey("DeveloperId"), InverseProperty("Members")]
        public virtual Developer Developer { get; set; }
        public string Name { get; set; }
        public long? AddressId { get; set; }
        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; }
    }
    [Table("DeveloperDirector")]
    public class DeveloperDirector : Entity
    {
        public long DeveloperId { get; set; }
        [ForeignKey("DeveloperId"), InverseProperty("Directors")]
        public virtual Developer Developer { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public EducationLevel? AcademicQualification { get; set; }
        public decimal SharePercentage { get; set; }
        public string BusinessExperience { get; set; }
        public string ContactNumber { get; set; }
    }
    [Table("DeveloperDocument")]
    public class DeveloperDocument : Entity
    {
        public long DeveloperId { get; set; }
        [ForeignKey("DeveloperId"), InverseProperty("OtherDocuments")]
        public virtual Developer Developer { get; set; }
        public string Name { get; set; }
        public DocumentStatus DocumentStatus { get; set; }

    }
}
