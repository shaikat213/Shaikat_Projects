using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Finix.Auth.DTO
{
    public class UserCompanyApplicationDto
    {
        public long? UserId { get; set; }
        public string UserName { get; set; }
        public long? CompanyProfileId { get; set; }
        public string CompanyProfileName { get; set; }
        public List<ApplicationDto> Applications { get; set; }
    }
}