using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class PerformerDto
    {
        public long? Id { get; set; }
        public long? RMId { get; set; }
        public string RMName { get; set; }
        public long? BranchId { get; set; }
        public long? ProductId { get; set; }
        public string ProductName { get; set; }
        public string BranchName { get; set; }
        public int Number { get; set; }
        public int WAR { get; set; }
        public long? OfficeDesignationSettingId { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Contribution { get; set; }
        public decimal? Rate { get; set; }
    }
}
