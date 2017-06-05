using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class QuestionAnswerDto 
    {
        public long SalesLeadId { get; set; }
        public string SalesLeadName { get; set; }
        public long QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string Answer { get; set; }
        public long? QuestionedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus Status { get; set; }
    }
}
