using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("NIDVerification")]
    public class NIDVerification : Entity
    {
        public long CifId { get; set; }
        [ForeignKey("CifId"), InverseProperty("NIDVerifications")]
        public virtual CIF_Personal CIF { get; set; }
        public long? ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public virtual Application Application { get; set; }
        public VerificationAs VerificationPersonRole { get; set; }
        public DateTime? VerificationDate { get; set; }
        public long? VerifiedByUserId { get; set; }
        public long? VerifiedByEmpDegMapId { get; set; }
        public string Name { get; set; }
        public string NIDNo { get; set; }
        public DateTime DateOfBirth { get; set; }
        public FindingStatus Finding { get; set; }
        public string Remarks { get; set; }
        public string NidFileUploadPath { get; set; }
        public VerificationState VerificationStatus { get; set; }
        public long? EmpId { get; set; }
        [ForeignKey("EmpId")]
        public virtual Employee Employee { get; set; }
    }
}
