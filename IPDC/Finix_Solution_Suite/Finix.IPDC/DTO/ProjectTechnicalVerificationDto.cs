using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.DTO
{
    public class ProjectTechnicalVerificationDto
    {
        public long? Id { get; set; }
        public long? ProjectId { get; set; }
        //public ProjectDto Project { get; set; }
        public long? EmployeeId { get; set; }
        public ProjectApprovalAuthority? PlanApprovalAuthority { get; set; }
        public string PlanApprovalAuthorityName { get; set; }
        public PropertyType? ProjectPropertyType { get; set; }
        public string ProjectPropertyTypeName { get; set; }
        public decimal? AreaOfLandPP { get; set; }
        public long? AreaOfLandPPUomId { get; set; }
        //public UOMDto AreaOfLandPPUOM { get; set; }
        public decimal? AccessRoadWidth { get; set; }
        public int? ApprovedNoOfFloors { get; set; }
        public int? ApprovedNoOfUnitsPerFloor { get; set; }
        public int? ApprovedTotalNoOfUnits { get; set; }
        public decimal? ApprovedTotalFloorArea { get; set; }
        public bool? DeviatedFromApproved { get; set; }
        public int? ActualNoOfFloors { get; set; }
        public int? ActualNoOfUnitsPerFloor { get; set; }
        public int? ActualTotalNoOfUnits { get; set; }
        public decimal? ActualTotalFloorArea { get; set; }
        public decimal? TotalDeviationInFloorArea { get; set; }
        public decimal? TotalDeviationPercentage { get; set; }
        public PropertyBounds? NorthBound { get; set; }
        public string NorthBoundName { get; set; }
        public PropertyBounds? SouthBound { get; set; }
        public string SouthBoundName { get; set; }
        public PropertyBounds? EastBound { get; set; }
        public string EastBoundName { get; set; }
        public PropertyBounds? WestBound { get; set; }
        public string WestBoundName { get; set; }
        public decimal? ProjectCompletionPercentage { get; set; }
        public HttpPostedFileBase ProjectTechnicalVerificationFile { get; set; }
        public string ProjectTechnicalVerificationFileName { get; set; }
        public string ProjectTechnicalVerificationPath { get; set; }
        public List<ProjectImagesDto> Photos { get; set; }
        public List<long> RemovePhotos { get; set; }
        public ApprovalStatus? TechnicalApprovalStatus { get; set; }
        public string TechnicalApprovalStatusName { get; set; }
        public string Remarks { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
