using Finix.IPDC.Infrastructure;
using System;
using System.Collections.Generic;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.DTO
{
    public class CIBVerificationHistoryDto
    {   
        public long? CIFId { get; set; }
        public string CifNo { get; set; }
        public string CIFName { get; set; }
        public long? AppId { get; set; }
        public int? CibType { get; set; }
        public List<CIB_PersonalDto> CIBPersonal { get; set; }
        public List<CIB_OrganizationalDto> CIBOrganizational { get; set; }
        
    }
}
