using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    public class QuestionAssignment : Entity
    {
        public long QuestionId { get; set; }
        public long ProductId { get; set; }
        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
