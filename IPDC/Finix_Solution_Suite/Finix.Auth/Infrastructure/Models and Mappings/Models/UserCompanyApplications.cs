using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Finix.Auth.Infrastructure
{
    [Table("UserCompanyApplication")]
    public class UserCompanyApplication : Entity
    {
        public long UserId { get; set; }
        public long CompanyId { get; set; }
        public long ApplicationId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("CompanyId")]
        public virtual CompanyProfile CompanyProfile { get; set; }
        [ForeignKey("ApplicationId")]
        public virtual Application Application { get; set; }
    }
}