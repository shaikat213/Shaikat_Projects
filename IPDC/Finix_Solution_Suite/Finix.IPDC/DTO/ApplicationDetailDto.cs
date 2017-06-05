using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.DTO
{
    public class ApplicationDetailDto
    {
        public ApplicationDetailDto()
        {
            References = new List<CIF_ReferenceDto>();
        }
        public long? Id { get; set; }
        public long? SalesLeadId { get; set; }
        public long? OfficeLayerId { get; set; }
        public long? OfferLetterId { get; set; }
        public long? BranchId { get; set; }
        public string BranchName { get; set; }
        public long? DmId { get; set; }
        public long? RMId { get; set; }//foreign key to employee
        public string RMName { get; set; }
        public string RMCode { get; set; }
        public string ApplicationNo { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public string ApplicationDateText { get; set; }
        public ApplicationCustomerType? CustomerType { get; set; }
        public ApplicationType? ApplicationType { get; set; }
        public List<ApplicationCIFsDto> CIFList { get; set; }
        public List<long> RemovedCIFList { get; set; }
        public List<CIF_Org_OwnersDto> OwnerList { get; set; }
        public long? ContactPersonId { get; set; }
        public bool? UseConAddAsGrpAdd { get; set; }
        public long? GroupAddressId { get; set; }
        public AddressDto GroupAddress { get; set; }
        public string AccountTitle { get; set; }
        public string AccGroupId { get; set; }
        public ProductDto Product { get; set; }
        public long? ProductId { get; set; }
        public string ProductName { get; set; }
        public ProductType? ProductType { get; set; }
        public long? LoanApplicationId { get; set; }
        public LoanApplicationDto LoanApplication { get; set; }
        public long? DepositApplicationId { get; set; }
        public DepositApplicationDto DepositApplication { get; set; }
        public List<AppDocChecklistDto> DocChecklist { get; set; }
        public  List<CIF_ReferenceDto> References { get; set; }
        //public List<long> RemovedDocCheckList { get; set; }
        public bool? IsSelfSubmitted { get; set; }
        public long? ProposalId { get; set; }
        public int Term { get; set; }
        public string CustomerTypeName { get; set; }
        public string ApplicationTypeName { get; set; }
        public string ProductTypeName { get; set; }
        public string test { get; set; }
        public long? ProductLegalCreatorEmpId { get; set; }
        public string ProductLegalCreatorEmpName { get; set; }
        public LeadPriority? LeadPriority { get; set; }
        public string LeadPriorityName { get; set; }
        public long? DclId { get; set; }
        public string RejectionReason { get; set; }
        public long? RejectedByEmpId { get; set; }
        public string RejectedByEmpName { get; set; }
        public DateTime? RejectedOn { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
