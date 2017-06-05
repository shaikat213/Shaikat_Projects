using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("ProjectImages")]
    public class ProjectImages : Entity
    {
        public long ProjectTechnicalId { get; set; }
        [ForeignKey("ProjectTechnicalId"), InverseProperty("Photos")]
        public virtual ProjectTechnicalVerification ProjectTechnical { get; set; }
        public string PhotoUrl { get; set; }
    }



    [Table("ProjectTechnicalVerification")]
    public class ProjectTechnicalVerification : Entity
    {
        public long ProjectId { get; set; }
        [ForeignKey("ProjectId"), InverseProperty("TechnicalVerifications")]
        public virtual Project Project { get; set; }
        public long? EmployeeId { get; set; }
        public ProjectApprovalAuthority? PlanApprovalAuthority { get; set; }
        public PropertyType? ProjectPropertyType { get; set; }
        public decimal? AreaOfLandPP { get; set; }
        public long? AreaOfLandPPUomId { get; set; }
        [ForeignKey("AreaOfLandPPUomId")]
        public virtual UOM AreaOfLandPPUOM { get; set; }
        public decimal? AccessRoadWidth { get; set; }
        public int? ApprovedNoOfFloors { get; set; }
        public int? ApprovedNoOfUnitsPerFloor { get; set; }
        public int? ApprovedTotalNoOfUnits { get; set; }
        public decimal? ApprovedTotalFloorArea { get; set; }
        public bool DeviatedFromApproved { get; set; }
        public int? ActualNoOfFloors { get; set; }
        public int? ActualNoOfUnitsPerFloor { get; set; }
        public int? ActualTotalNoOfUnits { get; set; }
        public decimal? ActualTotalFloorArea { get; set; }
        public decimal? TotalDeviationInFloorArea { get; set; }
        public decimal? TotalDeviationPercentage { get; set; }
        public PropertyBounds? NorthBound { get; set; }
        public PropertyBounds? SouthBound { get; set; }
        public PropertyBounds? EastBound { get; set; }
        public PropertyBounds? WestBound { get; set; }
        public decimal? ProjectCompletionPercentage { get; set; }
        public string ProjectTechnicalVerificationPath { get; set; }
        public virtual ICollection<ProjectImages> Photos { get; set; }
        public ApprovalStatus? TechnicalApprovalStatus { get; set; }
        public string Remarks { get; set; }
    }
}
