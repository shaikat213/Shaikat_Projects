using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class NWV_BusinessSharesDto
    {
        public long? Id { get; set; }
        public long? NWV_NetWorthId { get; set; }
        public BusinessShareType? BusinessShareType { get; set; }
        public decimal ValueOfOwnership { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus Status { get; set; }

    }
}
