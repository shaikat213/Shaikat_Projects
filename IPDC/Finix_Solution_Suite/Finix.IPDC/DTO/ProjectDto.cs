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
    public class ProjectDto
    {
        public long? Id { get; set; }
        public long? DeveloperId { get; set; }
        public string DeveloperName { get; set; }
        public string ProjectName { get; set; }
        public decimal? Area { get; set; }
        public int? TotalSaleableUnits { get; set; }
        public int? TotalSoldUnits { get; set; }
        public long? ProjectAddressId { get; set; }
        public AddressDto ProjectAddress { get; set; }
        public DateTime? HandoverDate { get; set; }
        public string HandoverDateText { get; set; }
        public DateTime? AsOfDate { get; set; }
        public string AsOfDateText { get; set; }
        public DeveloperProjectStatus? DeveloperProjectStatus { get; set; }
        public string DeveloperProjectStatusName { get; set; }
        public string ConstructionStage { get; set; }
        //fields filled to be by legal 
        public List<ProjectLegalVerificationDto> LegalVerifications { get; set; }
        public List<long> RemoveLegalVerifications { get; set; }
        //fields filled to be by Technical
        public List<ProjectTechnicalVerificationDto> TechnicalVerifications { get; set; }
        public List<long> RemoveLegalTechnicalVerifications { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
    }

    public class ProjectImagesDto
    {
        public long? Id { get; set; }
        public long? ProjectTechnicalId { get; set; }
        //public ProjectTechnicalVerificationDto ProjectTechnical { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
    }
}
