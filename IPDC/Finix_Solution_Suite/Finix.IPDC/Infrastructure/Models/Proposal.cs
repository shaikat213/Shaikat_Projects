using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Proposal")]
    public class Proposal : Entity
    {
        public long ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public virtual Application Application { get; set; }
        //initial info
        public string CreditMemoNo { get; set; }
        public DateTime ApplicationReceiveDate { get; set; }
        public string ApplicationNo { get; set; }
        public DateTime? CRMReceiveDate { get; set; }
        public DateTime? ProposalDate { get; set; }//works as approval date
        public bool? IsApproved { get; set; }
        public string RMName { get; set; }
        public string RMCode { get; set; }
        public string BranchName { get; set; }
        public ProposalFacilityType? FacilityType { get; set; }
        //loan related info
        public decimal AppliedLoanAmount { get; set; }
        public int AppliedLoanTerm { get; set; }
        public decimal CurrentExposureWithIPDC { get; set; }
        public decimal TotalExposureWithIPDC { get; set; } // calculated value = currentExposureWithIPDC + LoanAmount
        public decimal InterestRateCard { get; set; }
        public decimal InterestRateOffered { get; set; }
        public decimal RateVariance { get; set; } //interestRateCard - InterestRateOffered
        public string LoanRemarks { get; set; }
        public decimal ProcessingFeeAndDocChargesCardRate { get; set; }
        //client profile
        public virtual ICollection<Proposal_ClientProfile> ClientProfiles { get; set; }
        //loan purpose
        public string LoanPurpose { get; set; }


        public decimal? Spread { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal? LoantoDepositRatio { get; set; }


        //income assessment
        public virtual ICollection<Proposal_Income> Incomes { get; set; }//considered and not considered
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
        public virtual ICollection<Proposal_NetWorth> NetWorths { get; set; }
        public decimal TotalAssetOfApplicants { get; set; }
        public decimal TotalLiabilitiesOfApplicants { get; set; }
        public decimal TotalNetWorthOfApplicants { get; set; }
        public string NetWorthsRemarks { get; set; }

        //CIB Status
        public virtual ICollection<Proposal_CIB> CIBs { get; set; }

        //Liability Details
        public virtual ICollection<Proposal_Liability> Liabilities { get; set; }
        public decimal TotalLimit { get; set; }
        public decimal TotalOutstanding { get; set; }
        public decimal LiabilityTotalEMI { get; set; }
        public string LiabilityRemarks { get; set; }


        //personal guarantor information
        public virtual ICollection<Proposal_Guarantor> Guarantors { get; set; }
        public string GuarantorRemarks { get; set; }

        //comment on bank statement
        public string CommentOnBankStatement { get; set; }
        //large text field inclueds asset backup, Strength, Weakness, Disbursement Conditions, Exceptions
        public virtual ICollection<Proposal_Text> Texts { get; set; }
        public string AssetBackupRemarks { get; set; }


        // PDC / EFTN details
        public string PDCBankName { get; set; }
        public string PDCBankBranch { get; set; }
        public string PDCAccountNo { get; set; }
        public string PDCRoutingNo { get; set; }
        public string PDCAccountTitle { get; set; }
        public string PDCAccountType { get; set; }
        public string PDCRemarks { get; set; }
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
        public decimal? TotalAmountForFdr { get; set; }
        public decimal? WeightedAverageRate{ get; set; }

        //if product = FDR

       //Documentation//
        public bool? DulyDischargedFdr { get; set; }
        public bool? LetterOfLienSetOff { get; set; }
        public bool? DemandPromissoryNote{ get; set; } //Demand Promissory Note and Letter of Continuation
        public bool? SingedLoanApplication { get; set; } //Singed Loan Application form with accepted terms and conditions
        public string Imperfections { get; set; }
        public virtual ICollection<Proposal_FDR> FDRs { get; set; }
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
        //if propertyType = Flat
        public string SellersName { get; set; }
        public string DevelopersName { get; set; }
        public string ProjectName { get; set; }
        public string ProjectStatus { get; set; }
        public long? ProjectAddressId { get; set; }
        [ForeignKey("ProjectAddressId")]
        public virtual Address ProjectAddress { get; set; }
        public DeveloperCategory? DeveloperCategory { get; set; }
        public int? TotalNumberOfFloors { get; set; }
        public int? TotalNumberOfSlabsCasted { get; set; }
        public decimal? TotalLandArea { get; set; }
        public string FlatDetails { get; set; }
        public int? NumberOfCarParking { get; set; }
        public decimal? FlatSize { get; set; }
        public LandType? PropertyOwnershipType { get; set; }
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
        public virtual ICollection<Proposal_OtherCost> OtherCosts { get; set; }
        public virtual ICollection<Proposal_Valuation_OtherCost> ValuationOtherCosts { get; set; }
        public decimal? FlatFinanceTotalAmount { get; set; }
        public decimal? LPFPAlreadyPaid { get; set; }
        //public decimal? RecomendedLoanAmountFromIPDC { get; set; }
        public decimal? AdditionalEquity2bInvestedByApplicant { get; set; }
        public decimal? LoanPaymentToDeveloper { get; set; }
        public decimal? ConstructionCostFinancingPlan { get; set; }
        public string LPFinancingPlanRemarks { get; set; }

        //if propertyType = Own Construction
        public string LPFPConstructionDetails { get; set; }
        //public decimal? EstimatedConstructionCostApproved { get; set; }
        //public virtual ICollection<Proposal_OtherCosts> OtherCosts { get; set; }
        //public decimal? TotalAmount { get; set; }
        //public decimal? LPFPAlreadyPaid { get; set; }
        //public decimal? RecomendedLoanAmountFromIPDC { get; set; }
        public decimal? NetAmountForConstruction { get; set; } //TotalAmount - Sum of Other costs
        public decimal? LoanAmount2bUtilized4Construction { get; set; }
        //public decimal? AdditionalEquity2bInvestedByApplicant { get; set; }
        //public string LPFinancingPlanRemarks { get; set; }

        //Security details
        public virtual ICollection<Proposal_SecurityDetail> SecurityDetails { get; set; }
        public string SecurityRemarks { get; set; }

        //overall assessment
        public virtual ICollection<Proposal_OverallAssessment> OverallAssessments { get; set; }

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
        public decimal? ExpectedRentPercentage { get; set; }
        public decimal DBRFlatPurchase { get; set; }
        //public decimal? LTVonTotalPresentMarketValue { get; set; }
        public string FacilityRecomendation { get; set; }
        public string FRRemarks { get; set; }
        public decimal? ExistingEMI { get; set; }
        //stress testing
        public virtual ICollection<Proposal_StressRate> StressRates { get; set; }
        public string StressRemarks { get; set; }
        public string PreparedBy { get; set; }
        public string ReviewedBy { get; set; }
        public string RecommendedBy { get; set; }
        //public AuthorityLevel? AuthorityLevel { get; set; }
        public ApprovalAuthorityLevel? ApprovalAuthorityLevel { get; set; }
        public virtual ICollection<Proposal_Signatory> Signatories { get; set; }
        public virtual ICollection<ProposalCreditCard> ProposalCreditCards { get; set; }
        public virtual ICollection<ProposalAmendment> Amendments { get; set; }

    }
    
}
