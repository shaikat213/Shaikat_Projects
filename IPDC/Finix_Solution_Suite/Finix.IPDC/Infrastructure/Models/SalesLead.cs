using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    public class SalesLead : Entity
    {

        public long? Call_Id { get; set; }
        [ForeignKey("Call_Id")]
        public virtual Call Call { get; set; }
        public bool? IsOrganization { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonDesignation { get; set; }
        public CallCategory CallCategory { get; set; }
        public string CallSourceText { get; set; }
        public string CustomerName { get; set; }
        public string CustomerOrganization { get; set; }
        public string CustomerDesignation { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public AgeRange? AgeRange { get; set; }
        public IncomeRange? IncomeRange { get; set; }
        public string NatureOfBusiness { get; set; }
        public long? ProfessionId { get; set; }
        [ForeignKey("ProfessionId")]
        public virtual Profession Profession { get; set; }
        public string OtherProfession { get; set; }
        public string CustomerPhone { get; set; }
        public long? CustomerAddressId { get; set; }
        [ForeignKey("CustomerAddressId")]
        public virtual Address CustomerAddress { get; set; }
        public long? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        public decimal? Amount { get; set; }
        public CallType CallType { get; set; }
        public CallStatus CallStatus { get; set; }
        public long? ReferredTo { get; set; }
        [ForeignKey("ReferredTo")]
        public virtual OfficeDesignationSetting ReferredToDeg { get; set; }
        public long? AssignedTo { get; set; }
        [ForeignKey("AssignedTo")]
        public virtual OfficeDesignationSetting AssignedToDeg { get; set; }
        public string Remarks { get; set; }
        public CustomerPriority? CustomerPriority { get; set; }
        public CallMode? CallMode { get; set; }
        public string CallCreatorName { get; set; }
        public MaritalStatus? MaritalStatus { get; set; }
        public LeadStatus? LeadStatus { get; set; }
        public DateTime? FollowupTime { get; set; }
        public CustomerSensitivity? CustomerSensitivity { get; set; }
        public CallFailReason? CallFailReason { get; set; }
        public string FailedRemarks { get; set; }
        public LeadPriority? LeadPriority { get; set; }
        public long? ProspectiveByEmpId { get; set; }
        [ForeignKey("ProspectiveByEmpId")]
        public virtual Employee ProspectiveByEmp { get; set; }
    }
}
