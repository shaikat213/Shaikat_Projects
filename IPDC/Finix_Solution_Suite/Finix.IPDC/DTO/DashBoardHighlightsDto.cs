using System;
using System.Collections.Generic;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class DashBoardHighlightsDto
    {
        #region Call
        public long Call_HomeLoan { get; set; }
        public long Call_HomeLoanToDay { get; set; }
        public long Call_HomeLoanLastDay { get; set; }
        public decimal TodayHomeLoanAmount { get; set; }
        public decimal LastdayHomeLoanAmount { get; set; }
        public long Call_PersonalLoan { get; set; }
        public long Call_PersonalLoanToDay { get; set; }
        public long Call_PersonalLoanLastDay { get; set; }
        public decimal TodayPersonalLoanAmount { get; set; }
        public decimal LastdayPersonalLoanAmount { get; set; }
        public long Call_AutoLoan { get; set; }
        public long Call_AutoLoanToDay { get; set; }
        public long Call_AutoLoanLastDay { get; set; }
        public decimal TodayAutoLoanAmount { get; set; }
        public decimal LastdayAutoLoanAmount { get; set; }
        public long Call_FixedDeposit { get; set; }
        public long Call_FixedDepositToDay { get; set; }
        public long Call_FixedDepositLastDay { get; set; }
        public decimal TodayFixedDepositAmount { get; set; }
        public decimal LastdayFixedDepositAmount { get; set; }
        public long Call_RecurrentDeposit { get; set; }
        public long Call_RecurrentDepositToDay { get; set; }
        public long Call_RecurrentDepositLastDay { get; set; }
        public decimal TodayRecurrentDepositAmount { get; set; }
        public decimal LastdayRecurrentDepositAmount { get; set; }
        public long Call_Undefined { get; set; }
        public long Call_UndefinedToDay { get; set; }
        public long Call_UndefinedLastDay { get; set; }
        public decimal TodayUndefinedAmount { get; set; }
        public decimal LastdayUndefinedAmount { get; set; }
        public long Call_Liability { get; set; }

        #endregion Call

        #region Lead

        public long Lead_HomeLoan { get; set; }
        public long Lead_HomeLoanToDay { get; set; }
        public long Lead_HomeLoanLastDay { get; set; }
        public decimal TodayLeadHomeLoanAmount { get; set; }
        public decimal LastdayLeadHomeLoanAmount { get; set; }
        public long Lead_PersonalLoan { get; set; }
        public long Lead_PersonalLoanToDay { get; set; }
        public long Lead_PersonalLoanLastDay { get; set; }
        public decimal TodayLeadPersonalLoanAmount { get; set; }
        public decimal LastdayLeadPersonalLoanAmount { get; set; }
        public long Lead_AutoLoan { get; set; }
        public long Lead_AutoLoanToDay { get; set; }
        public long Lead_AutoLoanLastDay { get; set; }
        public decimal TodayLeadAutoLoanAmount { get; set; }
        public decimal LastdayLeadAutoLoanAmount { get; set; }
        public long Lead_FixedDeposit { get; set; }
        public long Lead_FixedDepositToDay { get; set; }
        public long Lead_FixedDepositLastDay { get; set; }
        public decimal TodayLeadFixedDepositAmount { get; set; }
        public decimal LastdayLeadFixedDepositAmount { get; set; }
        public long Lead_RecurrentDeposit { get; set; }
        public long Lead_RecurrentDepositToDay { get; set; }
        public long Lead_RecurrentDepositLastDay { get; set; }
        public decimal TodayLeadRecurrentDepositAmount { get; set; }
        public decimal LastdayLeadRecurrentDepositAmount { get; set; }
        public long Lead_Liability { get; set; }
        #endregion Lead

        #region File Submited Apps
        public long Application_HomeLoan { get; set; }
        public long Application_HomeLoanToDay { get; set; }
        public long Application_HomeLoanLastDay { get; set; }
        public decimal TodayApplicationHomeLoanAmount { get; set; }
        public decimal LastdayApplicationHomeLoanAmount { get; set; }
        public long Application_PersonalLoan { get; set; }
        public long Application_PersonalLoanToDay { get; set; }
        public long Application_PersonalLoanLastDay { get; set; }
        public decimal TodayApplicationPersonalLoanAmount { get; set; }
        public decimal LastdayApplicationPersonalLoanAmount { get; set; }
        public long Application_AutoLoan { get; set; }
        public long Application_AutoLoanToDay { get; set; }
        public long Application_AutoLoanLastDay { get; set; }
        public decimal TodayApplicationAutoLoanAmount { get; set; }
        public decimal LastdayApplicationAutoLoanAmount { get; set; }
        public long Application_FixedDeposit { get; set; }
        public long Application_FixedDepositToDay { get; set; }
        public long Application_FixedDepositLastDay { get; set; }
        public decimal TodayApplicationFixedDepositAmount { get; set; }
        public decimal LastdayApplicationFixedDepositAmount { get; set; }
        public long Application_RecurrentDeposit { get; set; }
        public long Application_RecurrentDepositToDay { get; set; }
        public long Application_RecurrentDepositLastDay { get; set; }
        public decimal TodayApplicationRecurrentDepositAmount { get; set; }
        public decimal LastdayApplicationRecurrentDepositAmount { get; set; }
        public long Application_Liability { get; set; }
        #endregion File Submited Apps

        #region File Approved Apps
        public long Approved_HomeLoan { get; set; }
        public long Approved_HomeLoanToDay { get; set; }
        public long Approved_HomeLoanLastDay { get; set; }
        public long Approved_AutoLoan { get; set; }
        public long Approved_AutoLoanToDay { get; set; }
        public long Approved_AutoLoanLastDay { get; set; }
        public long Approved_PersonalLoan { get; set; }
        public long Approved_PersonalLoanToDay { get; set; }
        public long Approved_PersonalLoanLastDay { get; set; }
        public decimal Approved_HomeLoanAmount { get; set; }
        public decimal TodayApprovedApplicationHomeLoanAmount { get; set; }
        public decimal LastdayApprovedApplicationHomeLoanAmount { get; set; }
        public decimal Approved_AutoLoanAmount { get; set; }
        public decimal TodayApprovedApplicationAutoLoanAmount { get; set; }
        public decimal LastdayApprovedApplicationAutoLoanAmount { get; set; }
        public decimal Approved_PersonalLoanAmount { get; set; }
        public decimal TodayApprovedApplicationPersonalLoanAmount { get; set; }
        public decimal LastdayApprovedApplicationPersonalLoanAmount { get; set; }
        #endregion File Approved Apps

        #region File DisApproved Apps
        public long Disapproved_HomeLoan { get; set; }
        public long DisApproved_HomeLoanToDay { get; set; }
        public long DisApproved_HomeLoanLastDay { get; set; }
        public long Disapproved_AutoLoan { get; set; }
        public long DisApproved_AutoLoanToDay { get; set; }
        public long DisApproved_AutoLoanLastDay { get; set; }
        public long Disapproved_PersonalLoan { get; set; }
        public long DisApproved_PersonalLoanToDay { get; set; }
        public long DisApproved_PersonalLoanLastDay { get; set; }
        public decimal Disapproved_HomeLoanAmount { get; set; }
        public decimal TodayDisApprovedApplicationHomeLoanAmount { get; set; }
        public decimal LastdayDisApprovedApplicationHomeLoanAmount { get; set; }
        public decimal Disapproved_PersonalLoanAmount { get; set; }
        public decimal TodayDisApprovedApplicationAutoLoanAmount { get; set; }
        public decimal LastdayDisApprovedApplicationAutoLoanAmount { get; set; }
        public decimal Disapproved_AutoLoanAmount { get; set; }
        public decimal TodayDisApprovedApplicationPersonalLoanAmount { get; set; }
        public decimal LastdayDisApprovedApplicationPersonalLoanAmount { get; set; }
        #endregion File DisApproved Apps

        #region File Disbursed/Rec Apps
        public long Disbursed_HomeLoanToDay { get; set; }
        public long Disbursed_HomeLoanLastDay { get; set; }
        public long Disbursed_AutoLoanToDay { get; set; }
        public long Disbursed_AutoLoanLastDay { get; set; }
        public long Disbursed_PersonalLoanToDay { get; set; }
        public long Disbursed_PersonalLoanLastDay { get; set; }
        public long Received_FixedToDay { get; set; }
        public long Received_FixedLastDay { get; set; }
        public long Received_RecurrentToDay { get; set; }
        public long Received_RecurrentLastDay { get; set; }
        public decimal Disbursed_HomeLoanAmount { get; set; }
        public decimal Disbursed_HomeLoanAmountToday { get; set; }
        public decimal Disbursed_HomeLoanAmountLastday { get; set; }
        public decimal Disbursed_AutoLoanAmount { get; set; }
        public decimal Disbursed_AutoLoanAmountToday { get; set; }
        public decimal Disbursed_AutoLoanAmountLastday { get; set; }
        public decimal Disbursed_PersonalLoanAmount { get; set; }
        public decimal Disbursed_PersonalLoanAmountToday { get; set; }
        public decimal Disbursed_PersonalLoanAmountLastday { get; set; }
        public decimal ReceivedDepositAmount { get; set; }
        public decimal FundReceived_FixedAmountToday { get; set; }
        public decimal FundReceived_FixedAmountLastday { get; set; }
        public decimal FundReceived_RecurrentAmountToday { get; set; }
        public decimal FundReceived_RecurrentAmountLastday { get; set; }

        public long Disbursed_HomeLoan { get; set; }
        public long Disbursed_AutoLoan { get; set; }
        public long Disbursed_PersonalLoan { get; set; }
        public long Received_FixedDeposit { get; set; }
        public long Received_RecurrentDeposit { get; set; }

        public decimal Disbursed_HomeLoanAmountMTD { get; set; }
        public decimal HomeLoanWarMTD { get; set; }
        public decimal Disbursed_AutoLoanAmountMTD { get; set; }
        public decimal AutoLoanWarMTD { get; set; }
        public decimal Disbursed_PersonalLoanAmountMTD { get; set; }
        public decimal PersonalLoanWarMTD { get; set; }
        public long Disbursed_HomeLoanMTD { get; set; }
        public long Disbursed_AutoLoanMTD { get; set; }
        public long Disbursed_PersonalLoanMTD { get; set; }


        public decimal FundReceived_FixedAmountMTD { get; set; }
        public decimal FixedDepositWarMTD { get; set; }
        public decimal FundReceived_RecurrentAmountMTD { get; set; }
        public decimal ReccurentDepositWarMTD { get; set; }
        public long Received_FixedMTD { get; set; }
        public long Received_RecurrentMTD { get; set; }

        public decimal Disbursed_HomeLoanAmountLMTD { get; set; }
        public decimal HomeLoanWarLMTD { get; set; }
        public decimal Disbursed_AutoLoanAmountLMTD { get; set; }
        public decimal AutoLoanWarLMTD { get; set; }
        public decimal Disbursed_PersonalLoanAmountLMTD { get; set; }
        public decimal PersonalLoanWarLMTD { get; set; }
        public long Disbursed_HomeLoanLMTD { get; set; }
        public long Disbursed_AutoLoanLMTD { get; set; }
        public long Disbursed_PersonalLoanLMTD { get; set; }


        public decimal FundReceived_FixedAmountLMTD { get; set; }
        public decimal FixedDepositWarLMTD { get; set; }
        public decimal FundReceived_RecurrentAmountLMTD { get; set; }
        public decimal ReccurentDepositWarLMTD { get; set; }
        public long Received_FixedLMTD { get; set; }
        public long Received_RecurrentLMTD { get; set; }
        


        #endregion File Disbursed/Rec Apps

        public string RMHomeLoan1 { get; set; }
        public decimal RMHomeLoan1Amount { get; set; }
        public int RMHomeLoan1Count { get; set; }
        public string RMHomeLoan2 { get; set; }
        public decimal RMHomeLoan2Amount { get; set; }
        public int RMHomeLoan2Count { get; set; }
        public string RMPersonalLoan { get; set; }
        public decimal RMPersonalLoanAmount { get; set; }
        public int RMPersonalLoanCount { get; set; }
        public string RMAutoLoan { get; set; }
        public decimal RMAutoLoanAmount { get; set; }
        public int RMAutoLoanCount { get; set; }
        public string RMLiability1 { get; set; }
        public decimal RMLiability1Amount { get; set; }
        public int RMLiability1Count { get; set; }
        public string RMLiability2 { get; set; }
        public decimal RMLiability2Amount { get; set; }
        public int RMLiability2Count { get; set; }
        public long LiabilityCount { get; set; }
        public long AssetCount { get; set; }
        public long? AssetBranchId { get; set; }
        public List<BusinessContributionUnitDto> BusinessConAsset { get; set; }
        public string AssetBranchName { get; set; }
        public long? LiabilityBranchId { get; set; }
        public List<BusinessContributionUnitDto> BusinessConLiability { get; set; }
        public string LiabilityBranchName { get; set; }
        public DateTime FromDate { get; set; }
        public string FromDateText { get; set; }
        public DateTime ToDate { get; set; }
        public string ToDateText { get; set; }
        public List<ResidenceBreakdownDto> ResidenceBreakdownCall { get; set; }
        public List<ResidenceBreakdownDto> ResidenceBreakdownProfession { get; set; }
        public List<ResidenceBreakdownDto> ResidenceBreakdownGender { get; set; }
        public List<ResidenceBreakdownDto> ResidenceBreakdownIncome { get; set; }
        public List<ResidenceBreakdownDto> ResidenceBreakdownApproved { get; set; }
        public List<ResidenceBreakdownDto> ResidenceBreakdownDisbursed { get; set; }

        public List<ProductivityMatrixDataUnit> ChartData { get; set; }
    }
    public class BusinessContributionUnitDto
    {
        public long? BranchId { get; set; }
        public string BranchName { get; set; }
        public ProductType? ProductType { get; set; }
        public decimal? Amount { get; set; }
        public long Count { get; set; }
    }

    public class ResidenceBreakdownDto
    {
        public Stages Stage { get; set; }
        public string StageName { get; set; }
        public Gender? Gender { get; set; }
        public string GenderName { get; set; }
        public AgeRange? AgeRange { get; set; }
        public string AgeRangeName { get; set; }
        public IncomeRange? IncomeRange { get; set; }
        public string IncomeRangeName { get; set; }
        public MaritalStatus? MaritalStatus { get; set; }
        public string MaritalStatusName { get; set; }
        public long? DivisionId { get; set; }
        public string DivisionName { get; set; }
        public long? DistrictId { get; set; }
        public string DistrictName { get; set; }
        public long? ThanaId { get; set; }
        public string ThanaName { get; set; }
        public long? ProfessionId { get; set; }
        public string ProfessionName { get; set; }
        public ProductSelection? ProductSelection { get; set; }
        public ProposalFacilityType? FacilityType { get; set; }
        public DepositType? DepositType { get; set; }
        public decimal? Amount { get; set; }
        public long Count { get; set; }
    }
    public class ProductivityMatrixDataUnit
    {
        public string Month { get; set; }
        public string Year { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Value { get; set; }
        public decimal? CompareValue { get; set; }
    }
}
