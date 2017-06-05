using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("DocumentCheckListException")]
    public class DocumentCheckListException : Entity
    {
        public long DCLId { get; set; }
        [ForeignKey("DCLId"), InverseProperty("Exceptions")]
        public DocumentCheckList DCL { get; set; }
        public string Description { get; set; }
        public string Justification { get; set; }
        public DateTime? CollectionDate { get; set; }
        public DCLExceptionAction? Action { get; set; }
        public DateTime? ObtainedDate { get; set; }
    }
}
