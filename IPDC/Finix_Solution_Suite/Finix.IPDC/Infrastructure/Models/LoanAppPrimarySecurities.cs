using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{

    [Table("VehiclePrimarySecurity")]
    public class VehiclePrimarySecurity : Entity
    {
        public long LoanApplicationId { get; set; }
        [ForeignKey("LoanApplicationId")]
        public virtual LoanApplication LoanApplication { get; set; }
        public VehicleStatus VehicleStatus { get; set; }
        public VendorType VendorType { get; set; }
        public string SellersName { get; set; }
        public long? SellersAddressId { get; set; }
        public virtual Address SellersAddress { get; set; }
        public string SellersPhone { get; set; }
        public long? VendorId { get; set; }
        [ForeignKey("VendorId")]
        public virtual Vendor Vendor { get; set; }
        public VehicleType VehicleType { get; set; }
        public string VehicleName { get; set; }
        public string Manufacturer { get; set; }
        [MaxLength(4)]
        public string MnufacturingYear { get; set; }
        public string YearModel { get; set; }
        [MaxLength(4)]
        public string RegistrationYear { get; set; }
        public string RegistrationNo { get; set; }
        [MaxLength(4)]
        public string CC { get; set; }
        public string Colour { get; set; }
        public string ChassisNo { get; set; }
        public string EngineNo { get; set; }
        public decimal? Price { get; set; }

        public virtual ICollection<VehiclePrimarySecurityValuation> Valuations { get; set; }

    }

    [Table("ConsumerGoodsPrimarySecurity")]
    public class ConsumerGoodsPrimarySecurity : Entity
    {
        public long LoanApplicationId { get; set; }
        [ForeignKey("LoanApplicationId")]
        public virtual LoanApplication LoanApplication { get; set; }
        public string Item { get; set; }
        public string Brand { get; set; }
        public string Dealer { get; set; }
        public long? DealerAddressId { get; set; }
        [ForeignKey("DealerAddressId")]
        public virtual Address DealerAddress { get; set; }
        public long? ShowRoomId { get; set; }
        [ForeignKey("ShowRoomId")]
        public virtual VendorShowrooms Showroom { get; set; }
        public decimal? Price { get; set; }
        public virtual ICollection<ConsumerGoodsPrimarySecurityValuation> Valuations { get; set; }

    }

    [Table("FDRPrimarySecurity")]
    public class FDRPrimarySecurity : Entity
    {
        public long LoanApplicationId { get; set; }
        [ForeignKey("LoanApplicationId")]
        public virtual LoanApplication LoanApplication { get; set; }
        public virtual ICollection<FDRPSDetail> FDRPSDetails { get; set; }
        //public list DisbursementTo {get;set;}
    }
    [Table("FDRPSDetail")]
    public class FDRPSDetail : Entity
    {
        public long FDRPrimarySecurityId { get; set; }
        [ForeignKey("FDRPrimarySecurityId"), InverseProperty("FDRPSDetails")]
        public virtual FDRPrimarySecurity FDRPrimarySecurity { get; set; }
        public string FDRAccountNo { get; set; }
        public decimal Amount { get; set; }
        public string Depositor { get; set; }
        public DateTime MaturityDate { get; set; }
        public string RelationshipWithApplicant { get; set; }
        public DisbursementTo? DisbursementTo { get; set; }
        public string InstituteName { get; set; }
        public string BranchName { get; set; }

    }
    [Table("LPPrimarySecurity")]
    public class LPPrimarySecurity : Entity
    {
        public long LoanApplicationId { get; set; }
        [ForeignKey("LoanApplicationId")]
        public virtual LoanApplication LoanApplication { get; set; }
        public LandedPropertyLoanType? LandedPropertyLoanType { get; set; }
        public decimal? TotalCost { get; set; }
        public decimal? AmountPaid { get; set; }
        public decimal? RemainingClientContribution { get; set; }//total cost - amound paid - loan amount applied
        public string SourceOfRemainingFund { get; set; }
        public DateTime? FirstDisbursementExpDate { get; set; }
        public LandedPropertySellertype LandedPropertySellertype { get; set; }
        public LandType? LandType { get; set; }
        public string SellerName { get; set; }
        public string SellerPhone { get; set; }
        public long? PropertyAddressId { get; set; }
        [ForeignKey("PropertyAddressId")]
        public virtual Address PropertyAddress { get; set; }
        public long? DeveloperId { get; set; }
        [ForeignKey("DeveloperId")]
        public virtual Developer Developer { get; set; }
        public long? ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
        public decimal? FlatSize { get; set; }
        public int? FloorNo { get; set; }
        public string FlatSide { get; set; }


        //takeover
        public string TakeoverFrom { get; set; }
        public decimal? PrevCompanyApprovedLoan { get; set; }
        public decimal? PrevCompanyOutstandingLoan { get; set; }
        public decimal? TopUpLoan { get; set; }
        public decimal? CurrentInterestRate { get; set; }

        //home equity
        public string Owner { get; set; }
        public decimal? TotalPropertyValue { get; set; }

        //others
        public string CurrentWorkingStage { get; set; }
        public int? CompletedFloors { get; set; }
        public int? ProposedFloors { get; set; }
        public decimal? EstimatedConstructionCost { get; set; }
        public virtual ICollection<LPPrimarySecurityValuation> Valuations { get; set; }
    }
}
