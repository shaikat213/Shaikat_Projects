using Finix.IPDC.Infrastructure;
using System;
using System.Collections.Generic;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.DTO
{
    public class NetWorthVerificationHistoryDto
    {   
        public long? CIFId { get; set; }
        public string CifNo { get; set; }
        public string CIFName { get; set; }
        public long? AppId { get; set; }
        public long? NetWorthId { get; set; }
        public List<NetWorthVerificationDto> NetWorthVerification { get; set; }
        
    }
}
