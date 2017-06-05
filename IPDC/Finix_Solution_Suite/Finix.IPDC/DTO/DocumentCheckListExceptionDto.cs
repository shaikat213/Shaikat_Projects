using System;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class DocumentCheckListExceptionDto
    {
        public long? Id { get; set; }
        public long DCLId { get; set; }//[ForeignKey("DCLId"), InverseProperty("Exceptions")]
       // public DocumentCheckListDto DCL { get; set; }
        public string Description { get; set; }
        public string Justification { get; set; }
        public DateTime? CollectionDate { get; set; }
        public string CollectionDateTxt { get; set; }
        public DateTime? ObtainedDate { get; set; }
        public string ObtainedDateTxt { get; set; }
        public DCLExceptionAction? Action { get; set; }
        public string ActionName { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
