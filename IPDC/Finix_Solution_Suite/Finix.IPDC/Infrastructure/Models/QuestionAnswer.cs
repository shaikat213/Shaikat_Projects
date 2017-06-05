using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    public class QuestionAnswer : Entity
    {
        public  long SalesLeadId { get; set; }
        public long QuestionId { get; set; }
        public string Answer { get; set; }
        public long? QuestionedBy { get; set; }
        //todo-make foreignkey to Emp code or rm/sa code
        [ForeignKey("SalesLeadId")]
        public virtual SalesLead SalesLead { get; set; }
        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }
    }
}
