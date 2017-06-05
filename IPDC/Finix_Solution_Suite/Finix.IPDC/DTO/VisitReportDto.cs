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
    public class VisitReportDto
    {
        public long Id { get; set; }
        public long CIFId { get; set; }
        public string CIFNo { get; set; }
        public string CIFName { get; set; }
        //public CIF_PersonalDto CIF { get; set; }
        public long? ApplicationId { get; set; }
        public string ApplicationNo { get; set; }
        //public  ApplicationDto Application { get; set; }
        public VerificationAs VerificationPersonRole { get; set; }
        public string VerificationPersonRoleName { get; set; }
        public DateTime VisitTime { get; set; }
        public string VisitTimeText { get; set; }
        public long? VisitedById { get; set; }
        public EmployeeDto VisitedBy { get; set; }
        public string CompanyName { get; set; }
        public CompanyLegalStatus? TypeOfOrg { get; set; }
        public string NatureOfBusiness { get; set; }
        public long? OfficeAddressId { get; set; } 
        public  AddressDto OfficeAddress { get; set; }
        public string ContactedPersonName { get; set; }
        public string ContactedPersonDetails { get; set; }
        public string Description { get; set; }
        public string Observation { get; set; }
        public string VisitReportPath { get; set; }
        public HttpPostedFileBase VisitReportFile { get; set; }
        public string VisitReportFileName { get; set; }
        public long? EmpId { get; set; }
        public VerificationState? VerificationStatus { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
