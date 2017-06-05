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
    public class IncomeVerificationAdditionalIncomeAssessedDto
    {
        public long Id { get; set; }
        public long? IncomeVerificationId { get; set; }
        public long? AdditionalIncomeDeclaredId { get; set; }
        public CIF_AdditionalIncomeDeclaredDto AdditionalIncomeDeclared { get; set; }
        public bool IsConsidered { get; set; }
        public string SourceOfIncome { get; set; }
        public decimal? IncomeAmount { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
