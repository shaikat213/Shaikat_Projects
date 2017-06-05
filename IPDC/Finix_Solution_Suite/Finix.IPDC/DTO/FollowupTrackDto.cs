using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class FollowupTrackDto
    {
        public long Id { get; set; }
        public long SalesLeadId { get; set; }
        public string SalesLeadName { get; set; }
        public DateTime? CurrentFollowUp { get; set; }
        public string CurrentFollowUpTxt { get; set; }
        public DateTime? NextFollowUp { get; set; }
        public string NextFollowUpTxt { get; set; }
        public DateTime? CallTime { get; set; }
        public string CallTimeTxt { get; set; }
        public string Remarks { get; set; }
        public long SubmittedBy { get; set; }
        public FollowupType? FollowupType { get; set; }
        public string FollowupTypeName { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public string EditedByName { get; set; }
        public EntityStatus? Status { get; set; }
        public string ApiKey { get; set; }
        public long? UserId { get; set; }
    }
}
