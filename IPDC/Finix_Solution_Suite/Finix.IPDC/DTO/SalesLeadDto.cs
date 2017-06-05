using Finix.IPDC.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class SalesLeadDto
    {
        public long Id { get; set; }
        public long? ApplicationId { get; set; }
        public long? Call_Id { get; set; }
        public bool? IsOrganization { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonDesignation { get; set; }
        public CallCategory? CallCategory { get; set; }
        public string CallCategoryName { get; set; }
        public string CallSourceText { get; set; }
        public string CustomerName { get; set; }
        public string CustomerOrganization { get; set; }
        public string CustomerDesignation { get; set; }
        public Gender? Gender { get; set; }
        public string GenderName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public AgeRange? AgeRange { get; set; }
        public string AgeRangeName { get; set; }
        public IncomeRange? IncomeRange { get; set; }
        public string IncomeRangeName { get; set; }
        public string NatureOfBusiness { get; set; }
        public long? ProfessionId { get; set; }
        public string OtherProfession { get; set; }
        public string CustomerPhone { get; set; }
        public long? CustomerAddressId { get; set; }
        public AddressDto CustomerAddress { get; set; }
        public long? ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal? Amount { get; set; }
        public CallType? CallType { get; set; }
        public string CallTypeName { get; set; }
        public CallStatus? CallStatus { get; set; }
        public string CallStatusName { get; set; }
        public long? ReferredTo { get; set; }
        public long? AssignedTo { get; set; }
        public string Remarks { get; set; }
        public CustomerPriority? CustomerPriority { get; set; }
        public string CustomerPriorityName { get; set; }
        public CallMode? CallMode { get; set; }
        public string CallModeName { get; set; }
        public string CallCreatorName { get; set; }
        public MaritalStatus? MaritalStatus { get; set; }
        public string MaritalStatusName { get; set; }
        public LeadStatus? LeadStatus { get; set; }
        public string LeadStatusName { get; set; }
        public DateTime? FollowupTime { get; set; }
        public string FollowupTimeText { get; set; }
        public CustomerSensitivity? CustomerSensitivity { get; set; }
        public string CustomerSensitivityName { get; set; }
        public long? EditedBy { get; set; }
        public long? CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? EditDate { get; set; }
        public EntityStatus? Status { get; set; }
        public CallFailReason? CallFailReason { get; set; }
        public string CallFailReasonName { get; set; }
        public string FailedRemarks { get; set; }
        public LeadPriority? LeadPriority { get; set; }
        public string LeadPriorityName { get; set; }
        public string ApiKey { get; set; }
        public long? UserId { get; set; }
    }
}
