using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("ApplicationLog")]
    public class ApplicationLog : Entity
    {
        public long ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public virtual Application Application { get; set; }
        public long FromUserId { get; set; }
        public long ToUserId { get; set; }
        public ApplicationStage? FromStage { get; set; }
        public ApplicationStage ToStage { get; set; }
        public Activity Activity { get; set; }
        public AppType AppType { get; set; }
        public long? AppIdRef { get; set; }
    }
}
