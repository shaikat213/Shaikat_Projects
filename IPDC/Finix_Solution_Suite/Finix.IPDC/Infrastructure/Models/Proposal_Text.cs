using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Proposal_Text")]
    public class Proposal_Text : Entity
    {
        public long ProposalId { get; set; }
        [ForeignKey("ProposalId"), InverseProperty("Texts")]
        public virtual Proposal Proposal { get; set; }
        public ProposalTextTypes Type { get; set; }
        public string Text { get; set; }
        public bool? IsPrintable { get; set; }
        public PrinterFiltering? PrinterFiltering { get; set; }
    }
}
