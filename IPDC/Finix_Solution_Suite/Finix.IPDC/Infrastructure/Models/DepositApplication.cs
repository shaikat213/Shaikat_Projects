using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("DepositApplication")]
    public class DepositApplication : Entity
    {
        public ModeOfDeposit? ModeOfDeposit { get; set; }
        public virtual ICollection<DepAppChequeDeposit> ChequeDeposits { get; set; }
        public virtual ICollection<DepAppTransfer> TransferDeposits { get; set; }
        public virtual ICollection<DepAppCash> CashDeposits { get; set; }
        public decimal TotalDepositAmount { get; set; }
        public ModeOfOperations ModeOfOperation { get; set; }
        public string SpecialInstructions { get; set; }
        public DepositClass DepositClass { get; set; }
        
        public decimal? CardRate { get; set; }
        public decimal? OfferRate { get; set; }
        public decimal? RateVariance { get; set; }
        public long? ApprovedBy { get; set; }
        [ForeignKey("ApprovedBy")]
        public virtual OfficeDesignationSetting ApprovedByDeg { get; set; }
        public DepositAccRenewalOpts RenewalOpt { get; set; }
        public DepositWithdrawalMode WithdrawalMode { get; set; }
        public string CIFAccountTitle { get; set; }
        public string CIFBankAccNo { get; set; }
        public string CIFRoutingNo { get; set; }
        public string CIFBankBranch { get; set; }
        public string CIFBank { get; set; }
        public decimal? InitialDeposit { get; set; }
        public decimal? InstallmentSize { get; set; }
        public decimal? MaturityAmount { get; set; }
        public string SourceOfFund { get; set; }
        public virtual ICollection<DepositNominee> Nominees { get; set; }
        public long? GuiardianCifId { get; set; }
        [ForeignKey("GuiardianCifId")]
        public virtual CIF_Personal GuiardianCif { get; set; }
        public RelationshipWithApplicant? RelationshipWithApplicant { get; set; }


        public string BenificialOwner { get; set; }
        public string SourceOfFundDetail { get; set; }
        public string SourceOfFundVerificationMethod { get; set; }
        public string SourceOfFundConsistency { get; set; }
        public RiskLevel? RiskLevel { get; set; }
        public string Remarks { get; set; }
        

        //checking part
        public bool? FundRealization { get; set; }
        public DateTime? FundRealizationDate { get; set; }
        public ApplicationStatus? ApplicationStatus { get; set; }
        public long? TaskAssignedToId { get; set; }
        [ForeignKey("TaskAssignedToId")]
        public virtual OfficeDesignationSetting TaskAssignedTo { get; set; }
        public SanctionCheck? SanctionCheck { get; set; }
        public string SanctionRemarks { get; set; }

        //Operations
        public DateTime? AccountOpenDate { get; set; }
        public DateTime? MaturityDate { get; set; }//accountOpenDate + Term
        public string CBSAccountNo { get; set; }
        [MaxLength(4)]
        public string CBSBranchId { get; set; }
        public string InstrumentNo { get; set; }
        public InstrumentDispatchStatus? InstrumentDispatchStatus { get; set; }
        public DateTime? InstrumentDate { get; set; }
        public bool? FundReceived { get; set; }
        public string ProofOfDeposit { get; set; }
        public WelcomeLetterStatus? WelcomeLetterStatus { get; set; }
        public int? Term { get; set; }


    }
    

    [Table("DepAppChequeDeposit")]
    public class DepAppChequeDeposit : Entity
    {
        public long DepositApplicationId { get; set; }
        [ForeignKey("DepositApplicationId"), InverseProperty("ChequeDeposits")]
        public virtual DepositApplication DepositApplication { get; set; }
        public string ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }
        public string ChequeBank { get; set; }
        public long? DepositedTo { get; set; }
        [ForeignKey("DepositedTo")]
        public virtual IPDCBankAccounts IPDCBankAccount { get; set; }
        public DateTime? DepositDate { get; set; }
        public decimal? DepositAmount { get; set; }
    }
    [Table("DepAppTransfer")]
    public class DepAppTransfer : Entity
    {
        public long DepositApplicationId { get; set; }
        [ForeignKey("DepositApplicationId"), InverseProperty("TransferDeposits")]
        public virtual DepositApplication DepositApplication { get; set; }
        public DateTime? TransferDate { get; set; }
        public string SourceBank { get; set; }
        public long? DepositedTo { get; set; }
        [ForeignKey("DepositedTo")]
        public virtual IPDCBankAccounts IPDCBankAccount { get; set; }
        public decimal? DepositAmount { get; set; }

    }
    [Table("DepAppCash")]
    public class DepAppCash : Entity
    {
        public long DepositApplicationId { get; set; }
        [ForeignKey("DepositApplicationId"), InverseProperty("CashDeposits")]
        public virtual DepositApplication DepositApplication { get; set; }
        public long? DepositedTo { get; set; }
        [ForeignKey("DepositedTo")]
        public virtual IPDCBankAccounts IPDCBankAccount { get; set; }
        public string DepositorName { get; set; }
        public string DepositorPhone { get; set; }

        public DateTime? DepositDate { get; set; }
        public decimal? DepositAmount { get; set; }
    }
    [Table("DepositNominee")]
    public class DepositNominee : Entity
    {
        public long DepositApplicationId { get; set; }
        [ForeignKey("DepositApplicationId"), InverseProperty("Nominees")]
        public virtual DepositApplication DepositApplication { get; set; }
        public long NomineeCifId { get; set; }
        [ForeignKey("NomineeCifId")]
        public virtual CIF_Personal NomineeCif { get; set; }
        public RelationshipWithApplicant? RelationshipWithApplicant { get; set; }

        public decimal PercentageShare { get; set; }
        public long? GuiardianCifId { get; set; }
        [ForeignKey("GuiardianCifId")]
        public virtual CIF_Personal GuiardianCif { get; set; }
        public RelationshipWithApplicant? GuidRelationWithNom { get; set; }

    }
    
}
