using Finix.IPDC.Infrastructure;
using System;

namespace Finix.IPDC.DTO
{
    public class CallDto
    {
        public long? Id { get; set; }
        public string CallId { get; set; }
        public bool? IsOrganization { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonDesignation { get; set; }
        public CallCategory? CallCategory { get; set; }
        public string CallCategoryName { get; set; }
        public string CallSourceName { get; set; }
        public string CallSourceText { get; set; }
        public string CustomerName { get; set; }
        public string CustomerOrganization { get; set; }
        public string CustomerDesignation { get; set; }
        public Gender? Gender { get; set; }
        public string GenderName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string DateOfBirthText { get; set; }
        public AgeRange? AgeRange { get; set; }
        public long? ProfessionId { get; set; }
        public string ProfessionName { get; set; }
        public IncomeRange? IncomeRange { get; set; }
        public string OtherProfession { get; set; }
        public string CustomerPhone { get; set; }
        public long? CustomerAddressId { get; set; }
        public AddressDto CustomerAddress { get; set; }
        public long? ProductId { get; set; }
        //public ProductDto Product { get; set; }
        public decimal? Amount { get; set; }
        public CallType? CallType { get; set; }
        public string CallTypeName { get; set; }
        public CallStatus? CallStatus { get; set; }
        public string CallStatusName { get; set; }
        public long? ReferredTo { get; set; }
        //public OfficeDesignationSettingDto ReferredToDeg { get; set; }
        public long? AssignedTo { get; set; }
        //public OfficeDesignationSettingDto AssignedToDeg { get; set; }
        public string AssignedToName { get; set; }
        public CallFailReason? CallFailReason { get; set; }
        public string CallFailReasonName { get; set; }
        public string Remarks { get; set; }
        public string FailedRemarks { get; set; }
        public DateTime? FollowUpCallTime { get; set; }
        public string FollowUpCallTimeText { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public string test { get; set; }
        public string NatureOfBusiness { get; set; }
        public CustomerPriority? CustomerPriority { get; set; }
        public string CustomerPriorityName { get; set; }
        public CallMode? CallMode { get; set; }
        public string CallCreatorName { get; set; }
        public MaritalStatus? MaritalStatus { get; set; }
        public bool? ConvertedToLead { get; set; }
        public long? UserId { get; set; }
        public string ApiKey { get; set; }
    }
}
