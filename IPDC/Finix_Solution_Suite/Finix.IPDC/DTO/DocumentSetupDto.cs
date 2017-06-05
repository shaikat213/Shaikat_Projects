using System;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.DTO
{
    public class DocumentSetupDto
    {
        public long? Id { get; set; }
        public long ProductId { get; set; }
        //[ForeignKey("ProductId"), InverseProperty("DocumentSetups")]
        //public virtual Product Product { get; set; }
        public string DocName { get; set; }
        public bool IsMandatory { get; set; }
        //public ApplicationCustomerType AppicableFor { get; set; }
        public DocCollectionStage DocCollectionStage { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }

        //public long ProductId { get; set; }
        public long DocId { get; set; }
        //public bool IsMandatory { get; set; }
        public ApplicationCustomerType CustomerType { get; set; }
        public CompanyLegalStatus? CompanyLegalStatus { get; set; }
        //public DocCollectionStage? DocCollectionStage { get; set; }




    }
}
