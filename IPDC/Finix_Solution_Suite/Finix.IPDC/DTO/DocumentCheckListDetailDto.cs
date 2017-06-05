using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.DTO
{
    public class DocumentCheckListDetailDto
    {
        public long? Id { get; set; }
        public long DCLId { get; set; }// [ForeignKey("DCLId"), InverseProperty("Documents")]
        //public DocumentCheckListDto DCL { get; set; }
        public long? DocumentSetupId { get; set; }//[ForeignKey("DocumentSetupId")]
        //public  DocumentSetupDto DocumentSetup { get; set; }
        public string Name { get; set; }
        public bool? IsRequired { get; set; }
        public DocumentStatus? DocumentStatus { get; set; }
        public string DocumentStatusName { get; set; }
        public DateTime? CollectionDate { get; set; }
        public string CollectionDateTxt { get; set; }
        public string Remarks { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
