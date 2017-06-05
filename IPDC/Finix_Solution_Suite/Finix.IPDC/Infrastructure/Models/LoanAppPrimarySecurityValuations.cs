using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("VehiclePrimarySecurityValuation")]
    public class VehiclePrimarySecurityValuation : Entity
    {
        public long VehiclePrimarySecurityId { get; set; }
        [ForeignKey("VehiclePrimarySecurityId"), InverseProperty("Valuations")]
        public virtual VehiclePrimarySecurity VehiclePrimarySecurity { get; set; }
        public DateTime? VerificationDate { get; set; }
        public long? VerifiedByUserId { get; set; }
        public long? VerifiedByEmpDegMapId { get; set; }
        public decimal? VerifiedPrice { get; set; }
        public string VerificationMethod { get; set; }
        public string Remarks { get; set; }
        public long? EmpId { get; set; }
        [ForeignKey("EmpId")]
        public virtual Employee Employee { get; set; }
        public VerificationState VerificationState { get; set; }
    }

    [Table("ConsumerGoodsPrimarySecurityValuation")]
    public class ConsumerGoodsPrimarySecurityValuation : Entity
    {
        public long ConsumerGoodsPrimarySecurityId { get; set; }
        [ForeignKey("ConsumerGoodsPrimarySecurityId"), InverseProperty("Valuations")]
        public virtual ConsumerGoodsPrimarySecurity ConsumerGoodsPrimarySecurity { get; set; }
        public DateTime? VerificationDate { get; set; }
        public long? VerifiedByUserId { get; set; }
        public long? VerifiedByEmpDegMapId { get; set; }
        public decimal? VerifiedPrice { get; set; }
        public string VerificationMethod { get; set; }
        public string Remarks { get; set; }
        public long? EmpId { get; set; }
        [ForeignKey("EmpId")]
        public virtual Employee Employee { get; set; }
        public VerificationState VerificationState { get; set; }
    }

    [Table("LPPrimarySecurityValuation")]
    public class LPPrimarySecurityValuation : Entity
    {
        public long LPPrimarySecurityId { get; set; }
        [ForeignKey("LPPrimarySecurityId"), InverseProperty("Valuations")]
        public virtual LPPrimarySecurity LPPrimarySecurity { get; set; }
        public DateTime? VerificationDate { get; set; }
        public long? VerifiedByUserId { get; set; }
        public long? VerifiedByEmpDegMapId { get; set; }
        public string Owner { get; set; }
        public string CurrentWorkingStage { get; set; }
        public int? CompletedFloors { get; set; }
        public int? ProposedFloors { get; set; }
        public decimal? EstimatedConstructionCost { get; set; }
        public decimal? FlatSize { get; set; }
        public int? FloorNo { get; set; }
        public string FlatSide { get; set; }
        public string FlatNo { get; set; }
        public string BuildingNo { get; set; }
        public long? ProjectAddressId { get; set; }
        [ForeignKey("ProjectAddressId")]
        public virtual Address ProjectAddress { get; set; }
        public long? ConstructionTypeId { get; set; }
        [ForeignKey("ConstructionTypeId")]
        public virtual ConstructionType ConstructionType { get; set; }
        public LandedPropertyLoanType? LoanType { get; set; }
        public decimal? PropornateLand { get; set; }
        public decimal? PresentMarketRateOfLand { get; set; }//puer katha
        public decimal? PresentMarketValueOfLand { get; set; }
        public decimal? FlatSizeWithCommonSpace { get; set; }
        public decimal? FlatSizeWithoutCommonSpace { get; set; }
        public decimal? PerSFTMarketPrice { get; set; }
        public decimal? MarketPriceOfFlat { get; set; } // = FlatSizeWithCommonSpace * PerSFTMarketPrice
        public int? CarParkingCount { get; set; }
        public decimal? CarParkingPrice { get; set; }
        public decimal? TotalMarketValue { get; set; }// = MarketPriceOfFlat + CarParkingPrice
        public decimal? DistressPercentage { get; set; }
        public decimal? DistressValue { get; set; } // = DistressPercentage * TotalMarketValue
        public ProjectStatus? ProjectStatus { get; set; }
        public decimal? TotalWIPofFullProject { get; set; }
        public FlatStatus? ApplicantsFlatStatus { get; set; }
        public decimal? TotalWIPofFlat { get; set; }
        public decimal? AreaOfLandAsPerPlan { get; set; } //fetch from project database
        public decimal? AreaOfLandAsPerClient { get; set; }
        public decimal? PerKathaPriceAsPerRAJUK { get; set; }
        public decimal? PerKathaPriceAsPerClient { get; set; }
        public decimal? MarketValueOfLandAsPerRAJUK { get; set; } // = AreaOfLandAsPerPlan * PerKathaPriceAsPerRAJUK
        public decimal? MarketValueOfLandAsPerClient { get; set; }// = AreaOfLandAsPerClient * PerKathaPriceAsPerClient
        public decimal? BuildUpAreaPerFloorApproved { get; set; }
        public decimal? TotalBuildUpAreaApproved { get; set; }
        public decimal? EstimatedConstructionCostPerSFTApproved { get; set; }
        public decimal? EstimatedConstructionCostApproved { get; set; } // = TotalBuildUpAreaApproved * EstimatedConstructionCostPerSFTApproved
        public decimal? LandValueAndEstimatedConstructionCostApproved { get; set; } // = MarketValueOfLandAsPerRAJUK  + EstimatedConstructionCostApproved
        public decimal? BuildUpAreaPerFloorPhysical { get; set; }
        public decimal? TotalBuildUpAreaPhysical { get; set; }
        public decimal? EstimatedConstructionCostPerSFTPhysical { get; set; }
        public decimal? EstimatedConstructionCostPhysical { get; set; } // = TotalBuildUpAreaPhysical * EstimatedConstructionCostPerSFTPhysical
        public decimal? LandValueAndEstimatedConstructionCostPhysical { get; set; } // = MarketValueOfLandAsPerClient  + EstimatedConstructionCostPhysical
        public LandedPropertySellertype? SellerType { get; set; }
        public LandedPropertyValuationType? ValuationType { get; set; }
        public string Remarks { get; set; }
        public long? EmpId { get; set; }
        [ForeignKey("EmpId")]
        public virtual Employee Employee { get; set; }
        public VerificationState VerificationState { get; set; }
    }
}
