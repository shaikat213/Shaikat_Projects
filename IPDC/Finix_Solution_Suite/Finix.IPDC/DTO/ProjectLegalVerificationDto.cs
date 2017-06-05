using Finix.IPDC.Infrastructure;
using System;
using System.Collections.Generic;
using System.Web;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.DTO
{
    public class ProjectLegalVerificationDto
    {
        public long? Id { get; set; }
        public long? ProjectId { get; set; }
        //public ProjectDto Project { get; set; }
        public long? EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public LandType? LandType { get; set; }
        public string LandTypeName { get; set; }
        public decimal? AreaOfLandTD { get; set; }
        public long? AreaOfLandTDUomId { get; set; }
        //public UOMDto AreaOfLandPPUOM { get; set; }
        public bool? IsEncumbered { get; set; }
        public DocumentStatus? JointVenturedAgreement { get; set; }
        public string JointVenturedAgreementName { get; set; }
        public DocumentStatus? POA { get; set; }
        public string POAName { get; set; }
        public string ScheduleOfProperty { get; set; }
        public List<ProjectPropertyOwnerDto> Owners { get; set; }
        public List<long> RemovedOwners { get; set; }
        public string VerificationReport { get; set; }
        public HttpPostedFileBase VerificationReportFile { get; set; }
        public string VerificationReportFileName { get; set; }
        public long? VerifiedByEmpId { get; set; }
        public long? VerifiedByOffDegId { get; set; }
        public string VettingReport { get; set; }
        public HttpPostedFileBase VettingReportFile { get; set; }
        public string VettingReportFileName { get; set; }
        public string VettedBy { get; set; }
        public string LegalStageComment { get; set; }
        public ApprovalStatus? LegalApprovalStatus { get; set; }
        public string LegalApprovalStatusName { get; set; }
        public string VerificationReportPath { get; set; }
        public string VettingReportPath { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }

    public class ProjectPropertyOwnerDto
    {
        public long? Id { get; set; }
        public long ProjectLegalId { get; set; }
        public string Name { get; set; }
        public string TitleDeedNo { get; set; }
        public DateTime? TitleDeedDate { get; set; }
        public string TitleDeedDateTxt { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
