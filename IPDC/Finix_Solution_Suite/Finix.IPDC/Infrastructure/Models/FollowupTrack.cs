using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    public class FollowupTrack : Entity
    {
        public long SalesLeadId { get; set; }
        public DateTime? CurrentFollowUp { get; set; }
        public DateTime? NextFollowUp { get; set; }
        public DateTime? CallTime { get; set; }
        public FollowupType? FollowupType { get; set; }
        public string Remarks { get; set; }
        public long SubmittedBy { get; set; }
        //todo- submitted by will be changed to CC/RM/SA/TL code or id
        [ForeignKey("SalesLeadId")]
        public virtual SalesLead SalesLead { get; set; }
    }
}
