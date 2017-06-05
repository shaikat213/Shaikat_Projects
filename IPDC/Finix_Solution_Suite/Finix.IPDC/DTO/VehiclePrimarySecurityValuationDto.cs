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
    public class VehiclePrimarySecurityValuationDto
    {
        public long? Id { get; set; }
        public long? VehiclePrimarySecurityId { get; set; }
        public VehiclePrimarySecurityDto VehiclePrimarySecurity { get; set; }
        public DateTime? VerificationDate { get; set; }
        public long? VerifiedByUserId { get; set; }
        public long? VerifiedByEmpDegMapId { get; set; }
        public string VerificationDateText { get; set; }
        public decimal? VerifiedPrice { get; set; }
        public string VerificationMethod { get; set; }
        public string Remarks { get; set; }
        public long? EmpId { get; set; }
        public VerificationState? VerificationState { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
