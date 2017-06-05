using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Designation")]
    public class Designation : Entity
    {
        public string Name { get; set; }
        public long? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public virtual Designation ParentDesignation { get; set; }
    }
}
