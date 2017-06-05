using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("ApprovalAuthoritySignatory")]
    public class ApprovalAuthoritySignatory : Entity
    {
        public long GroupId { get; set; }
        [ForeignKey("GroupId")]
        public virtual ApprovalAuthorityGroup AuthorityGroup { get; set; }
        public long ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        public decimal? AmountFrom { get; set; }
        public decimal? AmountTo { get; set; }
        public MemoType MemoType { get; set; }
    }

    
}
