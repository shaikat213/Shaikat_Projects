using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class IPDCMessagingDto
    {
        public long? Id { get; set; }
        public long? ApplicationId { get; set; }
        public string ApplicationNo { get; set; }
        public string AccountTitle { get; set; }
        //public MessagingOffice? MessagingOffice { get; set; }
        public long? RepliedTo { get; set; }
        public long? FromEmpId { get; set; }
        public string FromEmpName { get; set; }
        public long? ToEmpId { get; set; }
        public string ToEmpName { get; set; }
        public long? FromOfficeDesignationSettingId { get; set; }
        public string FromDesignationName { get; set; }
        public long? ToOfficeDesignationSettingId { get; set; }
        public string ToDesignationName { get; set; }
        public string Message { get; set; }
        public int? MessageType { get; set; }
        public string MobileMessage { get; set; }
        public bool? IsRead { get; set; }
        public string Read { get; set; }
        public bool? IsDraft { get; set; }
        public string Drafted { get; set; }
        //public int? MessageType { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public long? UserId { get; set; }
        public string ApiKey { get; set; }
    }
}
