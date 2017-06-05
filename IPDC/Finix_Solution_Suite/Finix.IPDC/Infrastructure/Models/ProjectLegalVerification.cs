using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("ProjectLegalVerification")]
    public class ProjectLegalVerification : Entity
    {
        public long ProjectId { get; set; }
        [ForeignKey("ProjectId"), InverseProperty("LegalVerifications")]
        public virtual Project Project { get; set; }
        public long? EmployeeId { get; set; }
        public LandType? LandType { get; set; }
        public decimal? AreaOfLandTD { get; set; }
        public long? AreaOfLandTDUomId { get; set; }
        [ForeignKey("AreaOfLandTDUomId")]
        public virtual UOM AreaOfLandTDUOM { get; set; }
        public bool? IsEncumbered { get; set; }
        public DocumentStatus? JointVenturedAgreement { get; set; }
        public DocumentStatus? POA { get; set; }
        public string ScheduleOfProperty { get; set; }
        public virtual ICollection<ProjectPropertyOwner> Owners { get; set; }
        public string VerificationReport { get; set; }
        public string VerificationReportPath { get; set; }

        public long? VerifiedByEmpId { get; set; }
        [ForeignKey("VerifiedByEmpId")]
        public virtual Employee VerifiedByEmp { get; set; }
        public long? VerifiedByOffDegId { get; set; }
        [ForeignKey("VerifiedByOffDegId")]
        public virtual OfficeDesignationSetting VerifiedByOffDeg { get; set; }

        public string VettingReport { get; set; }
        public string VettingReportPath { get; set; }
        //public string VettingReportUrl {get;set;}
        public string VettedBy { get; set; }
        public string LegalStageComment { get; set; }
        public ApprovalStatus? LegalApprovalStatus { get; set; }
    }

    
}
