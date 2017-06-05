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

    public class DepositApplicationDto
    {
        public long? Id { get; set; }
        public long? Application_Id { get; set; }
        //public string ModeOfDepositId { get; set; }
        public ModeOfDeposit? ModeOfDeposit { get; set; }        
        public List<DepAppChequeDepositDto> ChequeDeposits { get; set; }        
        public List<long> RemovedChequeDeposits { get; set; }
        public List<DepAppTransferDto> TransferDeposits { get; set; }        
        public List<long> RemovedTransferDeposits { get; set; }
        public List<DepAppCashDto> CashDeposits { get; set; }
        public List<long> RemovedCashDeposits { get; set; }
        public decimal TotalDepositAmount { get; set; }
        //public string ModeOfOperationName { get; set; }
        public ModeOfOperations? ModeOfOperation { get; set; }
        public string SpecialInstructions { get; set; }
        //public long? DepositClassName { get; set; }
        public DepositClass? DepositClass { get; set; }
        public decimal? CardRate { get; set; }
        public decimal? OfferRate { get; set; }
        public decimal? RateVariance { get; set; }
        public long? ApprovedBy { get; set; }                
        public OfficeDesignationSettingDto ApprovedByDeg { get; set; }
        //public string RenewalOptName { get; set; }        
        public DepositAccRenewalOpts? RenewalOpt { get; set; }
        //public string WithdrawalModeName { get; set; }
        public DepositWithdrawalMode? WithdrawalMode { get; set; }
        public string CIFAccountTitle { get; set; }
        public string CIFBankAccNo { get; set; }
        public string CIFRoutingNo { get; set; }
        public string CIFBankBranch { get; set; }
        public string CIFBank { get; set; }
        public decimal? InitialDeposit { get; set; }
        public decimal? InstallmentSize { get; set; }
        public decimal? MaturityAmount { get; set; }
        public string SourceOfFund { get; set; }

        //Nominees
        public List<DepositNomineeDto> Nominees { get; set; }
        public List<long> RemovedNominees { get; set; }
        public long? GuiardianCifId { get; set; }
        public CIF_KeyVal GuiardianCif { get; set; }
        //public CIF_PersonalDto GuiardianCif { get; set; }
        public RelationshipWithApplicant? RelationshipWithApplicant { get; set; }
        public string RelationshipWithApplicantName { get; set; }
        public string BenificialOwner { get; set; }
        public string SourceOfFundDetail { get; set; }
        public string SourceOfFundVerificationMethod { get; set; }
        public string SourceOfFundConsistency { get; set; }
        //public string RiskLevelName { get; set; }        
        public RiskLevel? RiskLevel { get; set; }
        public string Remarks { get; set; }

        //checking part
        public bool? FundRealization { get; set; }
        public DateTime? FundRealizationDate { get; set; }
        public string FundRealizationDateTxt { get; set; }
        //public string ApplicationStatusName { get; set; }
        public ApplicationStatus? ApplicationStatus { get; set; }
        public long? TaskAssignedToId { get; set; }        
        public OfficeDesignationSettingDto TaskAssignedTo { get; set; }
        //public string SanctionCheckName { get; set; }
        public SanctionCheck? SanctionCheck { get; set; }
        public string SanctionRemarks { get; set; }

        //Operations
        public DateTime? AccountOpenDate { get; set; }
        public string AccountOpenDateTxt { get; set; }
        public DateTime? MaturityDate { get; set; }//accountOpenDate + Term
        public string MaturityDateTxt { get; set; }
        public string CBSAccountNo { get; set; }
        public string CBSBranchId { get; set; }
        public string InstrumentNo { get; set; }
        //public string InstrumentDispatchStatusName { get; set; }
        public InstrumentDispatchStatus? InstrumentDispatchStatus { get; set; }
        public DateTime? InstrumentDate { get; set; }
        public string InstrumentDateText { get; set; }
        public bool? FundReceived { get; set; }
        public string ProofOfDeposit { get; set; }
        //public string WelcomeLetterStatusName { get; set; }
        public WelcomeLetterStatus? WelcomeLetterStatus { get; set; }
        public int? Term { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
    public class DepAppChequeDepositDto
    {
        public long? Id { get; set; }
        public long? DepositApplicationId { get; set; }        
        public string ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }
        public string ChequeDateTxt { get; set; }
        public string ChequeBank { get; set; }
        public long? DepositedTo { get; set; }
        public string DepositAccntName { get; set; }
        public IPDCBankAccountsDto IPDCBankAccount { get; set; }
        public DateTime? DepositDate { get; set; }
        public string DepositDateTxt { get; set; }
        public decimal? DepositAmount { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
    public class DepAppTransferDto
    {
        public long? Id { get; set; }
        public long? DepositApplicationId { get; set; }
        public DateTime? TransferDate { get; set; }
        public string TransferDateTxt { get; set; }
        public string SourceBank { get; set; }
        public long? DepositedTo { get; set; }
        public string DepositAccntName { get; set; }
        public IPDCBankAccountsDto IPDCBankAccount { get; set; }
        public decimal? DepositAmount { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }

    }
    public class DepAppCashDto
    {
        public long? Id { get; set; }
        public long? DepositApplicationId { get; set; }
        public long? DepositedTo { get; set; }
        public string DepositAccntName { get; set; }
        public IPDCBankAccountsDto IPDCBankAccount { get; set; }
        public string DepositorName { get; set; }
        public string DepositorPhone { get; set; }
        public DateTime? CashDepositDate { get; set; }
        public string CashDepositDateTxt { get; set; }
        public decimal? DepositAmount { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
    public class DepositNomineeDto
    {
        public long? Id { get; set; }
        public long? DepositApplicationId { get; set; }
        public long NomineeCifId { get; set; }
        public CIF_KeyVal NomineeCif { get; set; }
        public RelationshipWithApplicant? RelationshipWithApplicant { get; set; }
        public string RelationshipWithApplicantName { get; set; }
        public decimal PercentageShare { get; set; }
        public long? GuiardianCifId { get; set; }
        public CIF_KeyVal GuiardianCif { get; set; }
        public RelationshipWithApplicant? GuidRelationWithNom { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
