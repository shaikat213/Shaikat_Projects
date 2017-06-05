using System;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class AppDocChecklistDto
    {
        public long? Id { get; set; }
        public long ApplicationId { get; set; }
        public long ProductDocId { get; set; }
        public DocumentSetupDto ProductDoc { get; set; }
        public DocumentStatus DocumentStatus { get; set; }
        public DateTime? SubmissionDeadline { get; set; }
        public string SubmissionDeadlineText { get; set; }
        public bool ApprovalRequired { get; set; }
        public long? ApprovedById { get; set; }
        public long ProductId { get; set; }
        public ProductDto Product { get; set; }
        public string DocName { get; set; }
        public string ProductName { get; set; }
        public bool? IsChecked { get; set; }
        public bool IsMandatory { get; set; }
        public ApplicationCustomerType AppicableFor { get; set; }
        public DocCollectionStage DocCollectionStage { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
