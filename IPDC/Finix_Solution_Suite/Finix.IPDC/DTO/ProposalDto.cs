using System;
using System.Collections.Generic;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.DTO
{
    public class ProposalDto
    {
        public long? Id { get; set; }
        public long ApplicationId { get; set; }
        public ApplicationDto Application { get; set; }
        public string AccountTitle { get; set; }
        public string CreditMemoNo { get; set; }
        public string ApplicationNo { get; set; }
        public string ApplicantName { get; set; }
        //initial info
        public DateTime? ApplicationReceiveDate { get; set; }
        public string ApplicationReceiveDateText { get; set; }
        public DateTime? CRMReceiveDate { get; set; }
        public string CRMReceiveDateText { get; set; }
        public DateTime? ProposalDate { get; set; }
        public string ProposalDateText { get; set; }
        public bool? IsApproved { get; set; }
        public string RMName { get; set; }
        public string RMCode { get; set; }
        public string BranchName { get; set; }
        public ProposalFacilityType? FacilityType { get; set; }
        public string FacilityTypeName { get; set; }
        public string ApplicationPurpose { get; set; }
        //loan related info
        public decimal AppliedLoanAmount { get; set; }
        public int? AppliedLoanTermApplication { get; set; }
        public int AppliedLoanTerm { get; set; }
        public decimal CurrentExposureWithIPDC { get; set; }
        public decimal TotalExposureWithIPDC { get; set; } // calculated value = currentExposureWithIPDC + LoanAmount
        public decimal InterestRateCard { get; set; }
        public decimal InterestRateOffered { get; set; }
        public decimal RateVariance { get; set; } //interestRateCard - InterestRateOffered
        public string LoanRemarks { get; set; }
        public decimal ProcessingFeeAndDocChargesCardRate { get; set; }
        //client profile
        public List<Proposal_ClientProfileDto> ClientProfiles { get; set; }
        //loan purpose
        public string LoanPurpose { get; set; }

        public decimal? Spread { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string ExpiryDateTxt { get; set; }
        public decimal? LoantoDepositRatio { get; set; }
        public decimal? TotalAmountForFdr { get; set; }
        public decimal? WeightedAverageRate { get; set; }

        public bool? DulyDischargedFdr { get; set; }
        public bool? LetterOfLienSetOff { get; set; }
        public bool? DemandPromissoryNote { get; set; } //Demand Promissory Note and Letter of Continuation
        public bool? SingedLoanApplication { get; set; } //Singed Loan Application form with accepted terms and conditions
        public string Imperfections { get; set; }

        public string PDCRemarks { get; set; }
        public decimal? ExistingEMI { get; set; }
        //income assessment
        public  List<Proposal_IncomeDto> Incomes { get; set; }//considered and not considered
        public decimal TotalMonthlyIncomeConsidered { get; set; }
        public decimal TotalMonthlyIncomeNotConsidered { get; set; }
        //public decimal LiabilityTotalEMI { get; set; }
        //public decimal EMIofProposedLoan { get; set; }
        public decimal FreeCash { get; set; } //TotalMonthlyIncome - EMIofExistingLoans - EMIofProposedLoan
        //public decimal DBR { get; set; }
        public string IncomeConsideredRemarks { get; set; }
        public string IncomeNotConsideredRemarks { get; set; }
        public decimal? TotalExpences { get; set; }
        //net worth
        public List<Proposal_NetWorthDto> NetWorths { get; set; }
        public decimal TotalAssetOfApplicants { get; set; }
        public decimal TotalLiabilitiesOfApplicants { get; set; }
        public decimal TotalNetWorthOfApplicants { get; set; }
        public string NetWorthsRemarks { get; set; }

        //CIB Status
        public List<Proposal_CIBDto> CIBs { get; set; }

        //Liability Details
        public List<Proposal_LiabilityDto> Liabilities { get; set; }
        public decimal TotalLimit { get; set; }
        public decimal TotalOutstanding { get; set; }
        public decimal LiabilityTotalEMI { get; set; }
        public string LiabilityRemarks { get; set; }
        public decimal? TotalCreditCard { get; set; }

        //personal guarantor information
        public List<long> RemovedGuarantors { get; set; }
        public List<Proposal_GuarantorDto> Guarantors { get; set; }
        public string GuarantorRemarks { get; set; }

        //comment on bank statement
        public string CommentOnBankStatement { get; set; }
        //large text field inclueds asset backup, Strength, Weakness, Disbursement Conditions, Exceptions
        public List<Proposal_TextDto> Texts { get; set; }
        public string AssetBackupRemarks { get; set; }


        // PDC / EFTN details
        public string PDCBankName { get; set; }
        public string PDCBankBranch { get; set; }
        public string PDCAccountNo { get; set; }
        public string PDCRoutingNo { get; set; }
        public string PDCAccountTitle { get; set; }
        public string PDCAccountType { get; set; }

        //Property/Vehicle/FDR/Consumer Goods Details
        public ProposalProduct Product { get; set; }

        //if product = vehicle
        public string Vehicle_Name { get; set; }
        public string Vehicle_ModelYear { get; set; }
        public string Vehicle_VendorName { get; set; }
        public decimal? Vehicle_QuotedPrice { get; set; }
        public string CC { get; set; }
        public string Colour { get; set; }
        public string ChassisNo { get; set; }
        public string EngineNo { get; set; }
        public decimal? PresentMarketValue { get; set; }
        //public decimal? RecomendedLoanAmountFromIPDC { get; set; } // non editable
        public decimal? LTVonTotalPresentMarketValue { get; set; } //loan amount / preset market value %
        public string Product_Remarks { get; set; }
        //if product = FDR
        public List<Proposal_FDRDto> FDRs { get; set; }
        //public decimal? LTVonTotalPresentMarketValue { get; set; } // Loan amount / fdr total amount %
        //public decimal? RecomendedLoanAmountFromIPDC { get; set; }
        public string FDR_Remarks { get; set; }

        //if product = Consumer goods
        public string CG_Item { get; set; }
        public string CG_Brand { get; set; }
        public string CG_DealerName { get; set; }//dealer or showroom
        public decimal? CG_Price { get; set; }
        //public decimal? PresentMarketValue { get; set; }
        //public decimal? RecomendedLoanAmountFromIPDC { get; set; }
        //public decimal? LTVonTotalPresentMarketValue { get; set; }
        //public string Product_Remarks { get; set; }
        
        //if product = property
        public string PropertyRemarks { get; set; }
        public LandedPropertyValuationType? PropertyType { get; set; }
        public string PropertyTypeName { get; set; }
        //if propertyType = Flat
        public string SellersName { get; set; }
        public string DevelopersName { get; set; }
        public string ProjectName { get; set; }
        public string ProjectStatus { get; set; }
        public long? ProjectAddressId { get; set; }
    
        public AddressDto ProjectAddress { get; set; }
        public string ProjectAddressString { get; set; }
        public DeveloperCategory? DeveloperCategory { get; set; }
        public string DeveloperCategoryName { get; set; }
        public int? TotalNumberOfFloors { get; set; }
        public int? TotalNumberOfSlabsCasted { get; set; }
        public decimal? TotalLandArea { get; set; }
        public string FlatDetails { get; set; }
        public int? NumberOfCarParking { get; set; }
        public decimal? FlatSize { get; set; }
        public LandType? PropertyOwnershipType { get; set; }
        public string PropertyOwnershipTypeName { get; set; }
        public bool? IsPermissionRequired { get; set; }
        public string PermissionRemarks { get; set; }

        //if propertyType = Own Construction
        public string PropertyOwnerName { get; set; }
        //public string ProjectName { get; set; }
        //public long? ProjectAddressId { get; set; }
        //[ForeignKey("ProjectAddressId")]
        //public Address ProjectAddress { get; set; }
        //public decimal? TotalLandArea { get; set; }
        public string OwnConstructionLoanPurpose { get; set; }
        //public LandType? PropertyOwnershipType { get; set; }
        //public bool? IsPermissionRequired { get; set; }
        //public string PermissionRemarks { get; set; }


        //landed property valuation
        //if product = property
        //if propertyType = Flat
        //public decimal? FlatSize { get; set; }
        public decimal? PricePerSQF { get; set; }
        public decimal? MarketPriceOfFlat { get; set; }
        public decimal? CarParkingPrice { get; set; }
        public decimal? RegistrationCost { get; set; }
        public decimal? TotalMarketValue { get; set; } // carParkingPrice + MarketValeOfFlat
        public decimal? DistressedValue { get; set; } //70% of present market value
        //public decimal? RecomendedLoanAmountFromIPDC { get; set; }
        //public decimal? LTVonTotalPresentMarketValue { get; set; }
        public string LandedPropertyValuationRemarks { get; set; }

        //if propertyType = Own Construction
        //public decimal? TotalLandArea { get; set; }
        public decimal? PerKathaPriceAsPerRAJUK { get; set; }
        public decimal? MarketValueOfLandAsPerRAJUK { get; set; }
        public string LPVConstructionDetails { get; set; }
        public decimal? EstimatedConstructionCostApproved { get; set; }
        public string TotalConstructionAmountDetails { get; set; }
        public decimal? TotalAmount { get; set; }
        //public decimal? DistressedValue { get; set; }
        //public decimal? RecomendedLoanAmountFromIPDC { get; set; }
        //public decimal? LTVonTotalPresentMarketValue { get; set; }
        //public string LandedPropertyValuationRemarks { get; set; }


        //Landed property financing plan
        //if product = property
        //if propertyType = Flat
        //public decimal? TotalMarketValue { get; set; } //it is purchase price
        //public decimal? RegistrationCost { get; set; }
        public List<Proposal_OtherCostDto> OtherCosts { get; set; }
        public List<Proposal_Valuation_OtherCostDto> ValuationOtherCosts { get; set; }
        public decimal? FlatFinanceTotalAmount { get; set; }
        public decimal? LPFPAlreadyPaid { get; set; }
        public decimal? LoanPaymentToDeveloper { get; set; }
        //public decimal? RecomendedLoanAmountFromIPDC { get; set; }

        public decimal? AdditionalEquity2bInvestedByApplicant { get; set; }
        public decimal? ConstructionCostFinancingPlan { get; set; }
        public string LPFinancingPlanRemarks { get; set; }

        //if propertyType = Own Construction
        public string LPFPConstructionDetails { get; set; }
        //public decimal? EstimatedConstructionCostApproved { get; set; }
        //public List<Proposal_OtherCosts> OtherCosts { get; set; }
        //public decimal? TotalAmount { get; set; }
        //public decimal? LPFPAlreadyPaid { get; set; }
        //public decimal? RecomendedLoanAmountFromIPDC { get; set; }
        public decimal? LoanAmount2bUtilized4Construction { get; set; }
        public decimal? NetAmountForConstruction { get; set; } //TotalAmount - Sum of Other costs
                                                               //public decimal? AdditionalEquity2bInvestedByApplicant { get; set; }
                                                               //public string LPFinancingPlanRemarks { get; set; }


        //overall assessment
        public List<Proposal_OverallAssessmentDto> OverallAssessments { get; set; }

        //facility recomendation
        public decimal? RecomendedLoanAmountFromIPDC { get; set; }
        //public ProposalFacilityType? FacilityType { get; set; }
        //public int AppliedLoanTerm { get; set; }
        //public decimal InterestRateOffered { get; set; }
        public decimal ProcessingFeeAndDocChargesPercentage { get; set; }
        public decimal ProcessingFeeAndDocChargesAmount { get; set; }
        public decimal EMIofProposedLoan { get; set; }
        public decimal DBR { get; set; }
        public decimal? RentalIncome { get; set; }
        public decimal DBRFlatPurchase { get; set; }
        public decimal? ExpectedRentPercentage { get; set; }
        //public decimal? LTVonTotalPresentMarketValue { get; set; }
        public string FacilityRecomendation { get; set; }
        public string FRRemarks { get; set; }
        //stress testing
        public AuthorityLevel? AuthorityLevel { get; set; }
        public List<Proposal_StressRateDto> StressRates { get; set; }
        public string StressRemarks { get; set; }
        public string PreparedBy { get; set; }
        public string ReviewedBy { get; set; }
        public string RecommendedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
        public List<long> RemovedAssetBackup { get; set; }
        public List<long> RemovedOtherCosts { get; set; }
        public List<long> RemovedValuationOtherCosts { get; set; }
        public List<long> RemovedOverallAssessments { get; set; }
        public List<long> RemovedSignatories { get; set; }
        public List<long> RemovedCreditCards { get; set; }
        //public List<long> Removed
        public long? ApprovalAuthoritySignatoryId { get; set; }
        public ApprovalAuthorityLevel? ApprovalAuthorityLevel { get; set; }
        public string ApprovalAuthorityLevelName { get; set; }
        public List<Proposal_SignatoryDto> Signatories { get; set; }
        public List<Proposal_SecurityDetailDto> SecurityDetails { get; set; }
        public string SecurityRemarks { get; set; }
        public string TLComment { get; set; }
        public string BMComment { get; set; }
        public List<ProposalCreditCardDto> ProposalCreditCards { get; set; }

    }
    public class Proposal_SignatoryDto
    {
        public long? Id { get; set; }
        //public string ApprovalAuthoritySignatoryName { get; set; }
        public string Name { get; set; }
        public long? ProposalId { get; set; }
        public long SignatoryId { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }

    }
}
