using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    public class Application : Entity
    {
        public long? SalesLeadId { get; set; }
        [ForeignKey("SalesLeadId")]
        public virtual SalesLead SalesLead { get; set; }
        public long? BranchId { get; set; }
        [ForeignKey("BranchId")]
        public virtual Office BranchOffice { get; set; }        
        public long? RMId { get; set; }//foreign key to employee
        [ForeignKey("RMId")]
        public virtual Employee RMEmp { get; set; }
        public string RMCode { get; set; }
        public string ApplicationNo { get; set; }
        public DateTime ApplicationDate { get; set; }
        public ApplicationCustomerType CustomerType { get; set; }
        public ApplicationType? ApplicationType { get; set; }
        public virtual ICollection<ApplicationCIFs> CIFList { get; set; }
        public long? ContactPersonId { get; set; }
        public virtual CIF_Personal ContactPerson { get; set; }
        public bool UseConAddAsGrpAdd { get; set; }
        public long? ContactPersonAddressType { get; set; }
        public long? GroupAddressId { get; set; }
        [ForeignKey("GroupAddressId")]
        public virtual Address GroupAddress { get; set; }
        public string AccountTitle { get; set; }
        public string AccGroupId { get; set; }
        public long ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        public ProductType ProductType { get; set; }
        public long? LoanApplicationId { get; set; }
        [ForeignKey("LoanApplicationId")]
        public virtual LoanApplication LoanApplication { get; set; }
        public long? DepositApplicationId { get; set; }
        [ForeignKey("DepositApplicationId")]
        public virtual DepositApplication DepositApplication { get; set; }
        public virtual ICollection<AppDocChecklist> DocChecklist { get; set; }
        public int Term { get; set; }
        public ApplicationStage? ApplicationStage { get; set; }
        public string BMComment { get; set; }
        public string TLComment { get; set; }
        public long? CurrentHolding { get; set; }
        public long? CurrentHoldingEmpId { get; set; }
        [ForeignKey("CurrentHoldingEmpId")]
        public virtual Employee CurrentHoldingEmp { get; set; }
        public bool? HardCopyReceived { get; set; }
        public DateTime? HardCopyReceiveDate { get; set; }
        public long? CostCenterId { get; set; }
        [ForeignKey("CostCenterId")]
        public virtual CostCenter CostCenter { get; set; }
        public string RejectionReason { get; set; }
        public long? RejectedByEmpId { get; set; }
        [ForeignKey("RejectedByEmpId")]
        public virtual Employee RejectedByEmp { get; set; }
        public DateTime? RejectedOn { get; set; }
    }

    [Table("ApplicationCIFs")]
    public class ApplicationCIFs : Entity
    {
        public long ApplicationId { get; set; }
        [ForeignKey("ApplicationId"), InverseProperty("CIFList")]
        public virtual Application Application { get; set; }
        public long? CIF_PersonalId { get; set; }
        [ForeignKey("CIF_PersonalId")]
        public virtual CIF_Personal CIF_Personal { get; set; }
        public ApplicantRole? ApplicantRole { get; set; }
        public RelationshipWithApplicant? RelationshipWithApplicant { get; set; }
        public long? CIF_OrganizationalId { get; set; }
        [ForeignKey("CIF_OrganizationalId")]
        public virtual CIF_Organizational CIF_Organizational { get; set; }
    }
    [Table("AppDocChecklist")]
    public class AppDocChecklist : Entity
    {
        public long ApplicationId { get; set; }
        [ForeignKey("ApplicationId"), InverseProperty("DocChecklist")]
        public virtual Application Application { get; set; }
        public long ProductDocId { get; set; }
        [ForeignKey("ProductDocId")]
        public virtual DocumentSetup ProductDoc { get; set; }
        public DocumentStatus DocumentStatus { get; set; }
        public DateTime? SubmissionDeadline { get; set; }
        public bool ApprovalRequired { get; set; }
        public long? ApprovedById { get; set; }
        [ForeignKey("ApprovedById")]
        public OfficeDesignationSetting ApprovedBy { get; set; }
    }

    [Table("CostCenter")]
    public class CostCenter : Entity
    {
        public string Name { get; set; }
    }
}
