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
    public class ApplicationCIFsDto
    {
        public long? Id { get; set; }
        public long ApplicationId { get; set; }
        public string ApplicationNo { get; set; }
        public string AccountTitle { get; set; }
        public long? CIF_PersonalId { get; set; }
        public CIF_KeyVal CIF_Personal { get; set; }
        public string CIFNo { get; set; }
        public string CIFName { get; set; }
        public string CBSCIFNo { get; set; }
        public ApplicantRole? ApplicantRole { get; set; }
        public string ApplicantRoleName { get; set; }
        public RelationshipWithApplicant? RelationshipWithApplicant { get; set; }
        public string RelationshipWithApplicantName { get; set; }
        public string ApplicantName { get; set; }
        public long? CIF_OrganizationalId { get; set; }
        public int? Age { get; set; }
        public string ProfessionName { get; set; }
        public decimal MonthlyIncome { get; set; }
        public long? RMId { get; set; }//foreign key to employee
        public string RMCode { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
        public string OccupationTypeName { get; set; }
    }

    public class CIF_KeyVal
    {
        public long? Id { get; set; }
        public string key { get; set; }
        public string value { get; set; }
    }
}
