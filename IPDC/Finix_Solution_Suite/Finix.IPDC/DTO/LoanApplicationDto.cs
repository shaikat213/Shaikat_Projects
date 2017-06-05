using System;
using System.Collections.Generic;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class LoanApplicationDto
    {
        public LoanApplicationDto()
        {
            CIFList = new List<ApplicationCIFsDto>();
        }
        public long? Application_Id { get; set; }
        public string ApplicationNo { get; set; }
        public long? Id { get; set; }
        public decimal? LoanAmountApplied { get; set; }
        public decimal? Rate { get; set; }
        public decimal? ServiceChargeRate { get; set; }
        public decimal? ServiceChargeAmount { get; set; }
        public decimal? DocumentationFee { get; set; }
        public decimal? OtherFees { get; set; }
        public string Purpose { get; set; }
        public LoanPrimarySecurityType? LoanPrimarySecurityType { get; set; }
        public List<LoanAppColSecurityDto> LoanAppColSecurities { get; set; }
        public List<long> RemovedLoanAppColSecurities { get; set; }
        public List<long> RemovedFDRPSDetails { get; set; }
        public string OtherSecurity { get; set; }
        public List<LoanOtherSecuritiesDto> OtherSecurities { get; set; }
        public List<long> RemovedOtherSecurities { get; set; }
        public DisbursementMode? DisbursementMode { get; set; }
        public string PayeesAccountNo { get; set; }
        public string PayeesName { get; set; }
        public string Bank { get; set; }
        public string Branch { get; set; }
        public string RoutingNo { get; set; }
        public LoanChequeDeliveryOptions? LoanChequeDeliveryOption { get; set; }
        public List<LoanAppWaiverReqDto> WaiverRequests { get; set; }
        public List<ApplicationCIFsDto> CIFList { get; set; }
        public List<long> RemovedWaiverRequests { get; set; }
        public string BeneficialOwner { get; set; }
        public string SourceOfFund { get; set; }
        public string SourceOfFundVerificationMethod { get; set; }
        public string SourceOfFundConsistency { get; set; }
        public RiskLevel? RiskLevel { get; set; }
        public string Comment { get; set; }
        public List<GuarantorDto> Guarantors { get; set; }
        public List<long> RemovedGuarantors { get; set; }
        public ConsumerGoodsPrimarySecurityDto ConsumerGoodsPrimarySecurity { get; set; }
        public VehiclePrimarySecurityDto VehiclePrimarySecurity { get; set; }
        public FDRPrimarySecurityDto FDRPrimarySecurity { get; set; }
        public LPPrimarySecurityDto LPPrimarySecurity { get; set; }
        public int? Term { get; set; }
        public DateTime? AccountOpenDate { get; set; }
        public string CBSAccountNo { get; set; }
        public string CBSBranchId { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
