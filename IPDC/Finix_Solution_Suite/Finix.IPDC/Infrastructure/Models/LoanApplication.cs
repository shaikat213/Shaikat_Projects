using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("LoanApplication")]
    public class LoanApplication : Entity
    {
        
        public decimal LoanAmountApplied { get; set; }
        public decimal Rate { get; set; }
        public decimal? ServiceChargeRate { get; set; }
        public decimal? ServiceChargeAmount { get; set; }
        public decimal? DocumentationFee { get; set; }
        public decimal? OtherFees { get; set; }
        public string Purpose { get; set; }
        //todo - primary security
        public LoanPrimarySecurityType? LoanPrimarySecurityType { get; set; }
        public long? RefId { get; set; }
        


        //todo - collateral security
        public virtual ICollection<LoanAppColSecurity> LoanAppColSecurities { get; set; }
        public virtual ICollection<LoanOtherSecurities> OtherSecurities { get; set; }
        public string OtherSecurity { get; set; }
        public DisbursementMode DisbursementMode { get; set; }
        public string PayeesAccountNo { get; set; }
        public string PayeesName { get; set; }
        public string Bank { get; set; }
        public string Branch { get; set; }
        public string RoutingNo { get; set; }
        public LoanChequeDeliveryOptions? LoanChequeDeliveryOption { get; set; }
        public virtual ICollection<LoanAppWaiverReq> WaiverRequests { get; set; }

        //aml compliance
        public string BeneficialOwner { get; set; }
        public string SourceOfFund { get; set; }
        public string SourceOfFundVerificationMethod { get; set; }
        public string SourceOfFundConsistency { get; set; }
        public RiskLevel RiskLevel { get; set; }
        public string Comment { get; set; }

        public virtual ICollection<Guarantor> Guarantors { get; set; }
        public int? Term { get; set; }

        //Operations
        public DateTime? AccountOpenDate { get; set; }
        public string CBSAccountNo { get; set; }
        [MaxLength(4)]
        public string CBSBranchId { get; set; }
    }
    [Table("LoanAppWaiverReq")]
    public class LoanAppWaiverReq : Entity
    {
        public long LoanApplicationId { get; set; }
        [ForeignKey("LoanApplicationId"), InverseProperty("WaiverRequests")]
        public virtual LoanApplication LoanApplication { get; set; }
        public TypeOfWaiverReq WaiverType { get; set; }
        public long WaiverRequestedToId { get; set; }
        [ForeignKey("WaiverRequestedToId")]
        public virtual OfficeDesignationSetting WaiverRequestedTo { get; set; }
    }
    [Table("Guarantor")]
    public class Guarantor : Entity
    {
        public long LoanApplicationId { get; set; }
        [ForeignKey("LoanApplicationId"), InverseProperty("Guarantors")]
        public virtual LoanApplication LoanApplication { get; set; }
        public long GuarantorCifId { get; set; }
        [ForeignKey("GuarantorCifId")]
        public virtual CIF_Personal GuarantorCif { get; set; }
        public decimal GuaranteeAmount { get; set; }
        public RelationshipWithApplicant? RelationshipWithApplicant { get; set; }
    }
    [Table("LoanAppColSecurity")]
    public class LoanAppColSecurity : Entity
    {
        public long LoanApplicationId { get; set; }
        [ForeignKey("LoanApplicationId"), InverseProperty("LoanAppColSecurities")]
        public virtual LoanApplication LoanApplication { get; set; }
        public long ColSecurityId { get; set; }
        [ForeignKey("ColSecurityId")]
        public virtual ProductSecurity LoanAppSecurity { get; set; }
    }

    [Table("LoanOtherSecurities")]
    public class LoanOtherSecurities : Entity
    {
        public long LoanApplicationId { get; set; }
        [ForeignKey("LoanApplicationId"), InverseProperty("OtherSecurities")]
        public virtual LoanApplication LoanApplication { get; set; }
        public string SecurityDescription { get; set; }
    }

}
