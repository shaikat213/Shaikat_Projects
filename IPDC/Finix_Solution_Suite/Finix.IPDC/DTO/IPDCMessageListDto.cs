using Finix.IPDC.Infrastructure;
using System;
using System.Collections.Generic;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.DTO
{
    public class IPDCMessageListDto
    {   
        public long? Id { get; set; }
        public long? ApplicationId { get; set; }
        public string ApplicationNo { get; set; }
        public string AccountTitle { get; set; }
        public long? RepliedTo { get; set; }
        public long? FromEmpId { get; set; }
        public string FromEmpName { get; set; }
        public long? ToEmpId { get; set; }
        public string ToEmpName { get; set; }
        public long? FromOfficeDesignationSettingId { get; set; }
        public string FromDesignationName { get; set; }
        public long? ToOfficeDesignationSettingId { get; set; }
        public string ToDesignationName { get; set; }
        public int? MessageType { get; set; }
        public List<IPDCMessagingDto> IpdcMessaging { get; set; }
        
    }
}
