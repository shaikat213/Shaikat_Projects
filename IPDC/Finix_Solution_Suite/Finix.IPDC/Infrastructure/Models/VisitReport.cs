using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    public class VisitReport : Entity
    {
        public long CIFId { get; set; }
        [ForeignKey("CIFId"), InverseProperty("VisitReports")]
        public virtual CIF_Personal CIF { get; set; }
        public long? ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public virtual Application Application { get; set; }
        public VerificationAs VerificationPersonRole { get; set; }
        public DateTime VisitTime { get; set; }
        public long? VisitedById { get; set; }//employee id
        public virtual Employee VisitedBy { get; set; }
        public string CompanyName { get; set; }//preload from cif and editable
        public CompanyLegalStatus? TypeOfOrg { get; set; }//preload from cif -> occupation -> legal status
        public string NatureOfBusiness { get; set; }
        public long? OfficeAddressId { get; set; } // fetch from cif occupation ** on change insert new address
        [ForeignKey("OfficeAddressId")]
        public virtual Address OfficeAddress { get; set; } 
        public string ContactedPersonName { get; set; }
        public string ContactedPersonDetails { get; set; }
        public string Description { get; set; }
        public string Observation { get; set; }
        public string VisitReportPath { get; set; }
        public long? EmpId { get; set; }
        [ForeignKey("EmpId")]
        public virtual Employee Employee { get; set; }
        public VerificationState? VerificationStatus { get; set; }
    }
}
