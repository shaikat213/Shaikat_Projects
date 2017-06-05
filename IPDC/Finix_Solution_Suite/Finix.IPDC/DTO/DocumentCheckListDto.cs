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
    public class DocumentCheckListDto
    {
        public long? Id { get; set; }
        public string DCLNo { get; set; }
        public DateTime? DCLDate { get; set; }
        public string DCLDateTxt { get; set; }
        public long ApplicationId { get; set; }  //[ForeignKey("ApplicationId")]
        public string ApplicationNo { get; set; }
        public string OfferLetterNo { get; set; }
        public string CreditMemoNo { get; set; }
        public string AccountTitle { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public string ApplicationDateText { get; set; }
        public DateTime? MaturityDate { get; set; }
        //public  ApplicationDto Application { get; set; }
        public long? ProposalId { get; set; }//[ForeignKey("ProposalId")]
        //public ProposalDto Proposal { get; set; }
        public string ApplicationTitle { get; set; }
        public string CustomerTypeName { get; set; }
        public ProposalFacilityType? FacilityType { get; set; }
        public string FacilityTypeName { get; set; }
        //public DepositType? DepositType { get; set; }
        //public string DepositTypeName { get; set; }
        public long? ProductId { get; set; }//[ForeignKey("ProductId")]
        public ProductType? ProductTypeId { get; set; }
        public string ProductName { get; set; }
        public decimal? MaturityAmount { get; set; }
        //public ProductDto Product { get; set; }
        public int? Term { get; set; }
        public int? ExceptionCount { get; set; }
        public bool? IsApproved { get; set; }
        public  List<DocumentCheckListDetailDto> Documents { get; set; }
        public  List<DocumentCheckListExceptionDto> Exceptions { get; set; }
        public List<DocumentSecuritiesDto> Securities { get; set; }
        public List<DCL_SignatoryDto> Signatories { get; set; }
        public List<long> RemovedExceptions { get; set; }
        public List<long> RemovedDocuments { get; set; }
        public List<long> RemovedSecurities { get; set; }
        public List<long> RemovedSignatories { get; set; }
        public long? ApprovedByDegId { get; set; } //[ForeignKey("ApprovedByDegId")] public  OfficeDesignationSettingDto OfficeDesignationSetting { get; set; }
        public long? ApprovedByEmpId { get; set; }//[ForeignKey("ApprovedByEmpId")]
        //public  EmployeeDto Employee { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string PreparedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
