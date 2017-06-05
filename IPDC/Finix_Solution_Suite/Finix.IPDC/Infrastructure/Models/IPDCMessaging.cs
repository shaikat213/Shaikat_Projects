using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("IPDCMessaging")]
    public class IPDCMessaging : Entity
    {
        public long? ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public virtual Application Application { get; set; }
        public long? RepliedTo { get; set; }
        [ForeignKey("RepliedTo")]
        public virtual IPDCMessaging ParentMessage { get; set; }
        public long? FromEmpId { get; set; }
        [ForeignKey("FromEmpId")]
        public virtual Employee FromEmployee { get; set; }
        public long? ToEmpId { get; set; }
        [ForeignKey("ToEmpId")]
        public virtual Employee ToEmployeee { get; set; }
        public long? FromOfficeDesignationSettingId { get; set; }
        [ForeignKey("FromOfficeDesignationSettingId")]
        public virtual OfficeDesignationSetting FromOfficeDesignationSetting { get; set; }
        public long? ToOfficeDesignationSettingId { get; set; }
        [ForeignKey("ToOfficeDesignationSettingId")]
        public virtual OfficeDesignationSetting ToOfficeDesignationSetting { get; set; }
        public string Message { get; set; }
        public string MobileMessage { get; set; }
        public bool? IsRead { get; set; }
        public bool? IsDraft { get; set; }

    }
}
