using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.DTO
{
    public class OfferLetterDto
    {
        public long? Id { get; set; }
        public long ProposalId { get; set; }
        public string ProposalNo { get; set; }
        public long? ApplicationId { get; set; }
        public string ApplicationNo { get; set; }
        public string ApplicationTitle { get; set; }
        public int? BankAccount { get; set; }
        public ProposalDto Proposal { get; set; }
        public ProposalFacilityType FacilityType { get; set; }
        public string FacilityTypeName { get; set; }      
        public string OfferLetterNo { get; set; }
        public string DCLNo { get; set; }
        public DateTime OfferLetterDate { get; set; }
        public string OfferLetterDateTxt { get; set; }
        public decimal PenalInterest { get; set; }
        public List<OfferLetterTextDto> OfferLetterTexts { get; set; }
        public decimal LoanAdvance { get; set; }
        public int AcceptancePeriod { get; set; }
        public Purpose Purpose { get; set; }
        public string PurposeName { get; set; }
        public decimal CibAndProcessingFee { get; set; }
        public AddressDto PresentAddress { get; set; }
        public string PresentAddressText { get; set; }
        public DateTime ApplicationDate { get; set; }
        public decimal ProposedLoanAmount { get; set; }
        public string VehicleName { get; set; }
        public int YearModel { get; set; }
        public string CC { get; set; }
        public string Colour { get; set; }
        public string ChassisNo { get; set; }
        public string EngineNo { get; set; }
        public decimal CarPrice { get; set; }
        public decimal  BorrowersContribution { get; set; }
        public int Term { get; set; }
        public decimal InterestRate { get; set; }
        public decimal EMI { get; set; }
        public decimal ProcessingFeeRate { get; set; }
        public decimal ProcessingFeeAmount { get; set; }
        public decimal ProcessingFeeAndDocChargesPercentage { get; set; }
        public List<OfferLetterTextDto> ModeOfDisbursment { get; set; }
        public List<OfferLetterTextDto> DisbursmentCondition { get; set; }
        public string PDCBankName { get; set; }
        public string PDCBankBranch { get; set; }
        public string PDCAccountNo { get; set; }
        public string PDCRoutingNo { get; set; }
        public string PDCAccountTitle { get; set; }
        public string AccountTitle { get; set; }
        public string PDCAccountType { get; set; }
        public List<long> RemovedOfferLetterTexts { get; set; }
        public bool? CRMIsApproved { get; set; }
        public bool? OPSIsApproved { get; set; }
        public bool? CUSIsApproved { get; set; }
        public decimal? TotalAmountForFdr { get; set; }
        public decimal? WeightedAverageRate { get; set; }
        public decimal? Spread { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal? LoantoDepositRatio { get; set; }
        public string FDR_Remarks { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
