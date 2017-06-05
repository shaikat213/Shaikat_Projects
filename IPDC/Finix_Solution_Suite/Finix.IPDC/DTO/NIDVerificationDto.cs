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
    public class NIDVerificationDto
    {
        public long Id { get; set; }
        public long CifId { get; set; }
        public  CIF_PersonalDto CIF { get; set; }
        public string CIFNo { get; set; }
        public long? ApplicationId { get; set; }
        public  ApplicationDto Application { get; set; }
        public string ApplicationNo { get; set; }
        public VerificationAs VerificationPersonRole { get; set; }
        public string VerificationPersonRoleName { get; set; }
        public DateTime? VerificationDate { get; set; }
        public long? VerifiedByUserId { get; set; }
        public long? VerifiedByEmpDegMapId { get; set; }
        public string Name { get; set; }
        public string NIDNo { get; set; }
        public string VerificationDateText { get; set; }
        public string DateOfBirthText { get; set; }
        public DateTime DateOfBirth { get; set; }
        public FindingStatus Finding { get; set; }
        public string FindingName { get; set; }
        public string Remarks { get; set; }
        public string NidFileUploadPath { get; set; }
        public VerificationState VerificationStatus { get; set; }
        public string VerificationStatusName { get; set; }
        public long? EmpId { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
