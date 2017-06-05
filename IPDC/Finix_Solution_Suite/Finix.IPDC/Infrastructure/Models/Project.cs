using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Project")]
    public class Project : Entity
    {
        public long DeveloperId { get; set; }
        [ForeignKey("DeveloperId")]
        public virtual Developer Developer { get; set; }
        public string ProjectName { get; set; }
        public decimal? Area { get; set; }
        public int? TotalSaleableUnits { get; set; }
        public int? TotalSoldUnits { get; set; }
        public long ProjectAddressId { get; set; }
        [ForeignKey("ProjectAddressId")]
        public virtual Address ProjectAddress { get; set; }
        public DateTime? HandoverDate { get; set; }
        public DateTime? AsOfDate { get; set; }
        public DeveloperProjectStatus? DeveloperProjectStatus { get; set; }
        public string ConstructionStage { get; set; }
        //fields filled to be by legal 
        public virtual ICollection<ProjectLegalVerification> LegalVerifications { get; set; }
        //fields filled to be by Technical
        public virtual ICollection<ProjectTechnicalVerification> TechnicalVerifications { get; set; }
        
    }

    [Table("ProjectPropertyOwner")]
    public class ProjectPropertyOwner : Entity
    {
        public long ProjectLegalId { get; set; }
        [ForeignKey("ProjectLegalId"), InverseProperty("Owners")]
        public virtual ProjectLegalVerification ProjectLegalVerification { get; set; }
        public string Name { get; set; }
        public string TitleDeedNo { get; set; }
        public DateTime TitleDeedDate { get; set; }
    }

    
}
