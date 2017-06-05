using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("DocumentCheckListDetail")]
    public class DocumentCheckListDetail : Entity
    {
        public long DCLId { get; set; }
        [ForeignKey("DCLId"), InverseProperty("Documents")]
        public DocumentCheckList DCL { get; set; }
        public long? DocumentSetupId { get; set; }
        [ForeignKey("DocumentSetupId")]
        public virtual DocumentSetup DocumentSetup { get; set; }
        public string Name { get; set; }
        public bool? IsRequired { get; set; }
        public DocumentStatus? DocumentStatus { get; set; }
        public DateTime? CollectionDate { get; set; }
        public string Remarks { get; set; }
    }
}
