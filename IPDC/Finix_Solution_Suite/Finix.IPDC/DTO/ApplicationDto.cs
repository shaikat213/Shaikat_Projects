using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.DTO
{
    public class ApplicationDto
    {
        public long? Id { get; set; }
        public long? SalesLeadId { get; set; }
        //public long OfficeLayerId { get; set; }
        public long? BranchId { get; set; }
        public string BranchName { get; set; }
        public long? ProposalId { get; set; }
        public long? DmId { get; set; }
        public long? DclId { get; set; }
        public long? RMId { get; set; }
        public string RMName { get; set; }
        public string ProposalNo { get; set; }  
        public ProposalFacilityType? FacilityType { get; set; }
        public string FacilityTypeName { get; set; }
        public string RMCode { get; set; }
        public string ApplicationNo { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string ApplicationDateText { get; set; }
        public ApplicationCustomerType CustomerType { get; set; }
        public ApplicationType? ApplicationType { get; set; }
        public List<ApplicationCIFsDto> CIFList { get; set; }
        public List<long> RemovedCIFList { get; set; }
        public long? ContactPersonId { get; set; }
        public string ContactPersonName { get; set; }
        public long? ContactPersonAddressType { get; set; }
        //public virtual CIF_Personal ContactPerson { get; set; }
        public bool UseConAddAsGrpAdd { get; set; }
        public long? GroupAddressId { get; set; }
        public string GroupAddressName { get; set; }
        //[ForeignKey("GroupAddressId")] PoId
        public long? PoId { get; set; }
        public AddressDto GroupAddress { get; set; }
        public string AccountTitle { get; set; }
        public string AccGroupId { get; set; }
        public long ProductId { get; set; }
        public ProductType? ProductType { get; set; }
        public long? LoanApplicationId { get; set; }
        public LoanApplicationDto LoanApplication { get; set; }
        public LoanPrimarySecurityType? LoanPrimarySecurityType { get; set; }
        public long? DepositApplicationId { get; set; }
        public string CBSAccountNo { get; set; }
        public DepositApplicationDto DepositApplication { get; set; }
        public decimal? MaturityAmount { get; set; }
        public DateTime? MaturityDate { get; set; }
        //public string MaturityDateTxt { get; set; }
        public decimal? AppliedLoanAmount { get; set; }
        public List<AppDocChecklistDto> DocChecklist { get; set; }
        //public List<long> RemovedDocCheckList { get; set; }
        public int Term { get; set; }
        public string CustomerTypeName { get; set; }
        public string ApplicationTypeName { get; set; }
        public string ProductName { get; set; }
        public string ProductTypeName { get; set; }
        public string test { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
        public ApplicationStage? ApplicationStage { get; set; }
        public string ApplicationStageName { get; set; }
        public string BMComment { get; set; }
        public string TLComment { get; set; }
        public long? CurrentHolding { get; set; }
        public long? CurrentHoldingEmpId { get; set; }
        public string CurrentHoldingName { get; set; }
        public bool? HardCopyReceived { get; set; }
        public string HardCopyReceiveDateText { get; set; }
        public DateTime? HardCopyReceiveDate { get; set; }
        public string VehicleName { get; set; }
        public string ModelYear { get; set; }
        public string DCLNo { get; set; }
        public long? CostCenterId { get; set; }
        public string CostCenterName { get; set; }
        public string RejectionReason { get; set; }
        public long? RejectedByEmpId { get; set; }
        public string RejectedByEmpName { get; set; }
        public DateTime? RejectedOn { get; set; }
        public decimal? Rate { get; set; }
        public string ApplicantName { get; set; }

    }

    public class CostCenterDto
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
    }
}
