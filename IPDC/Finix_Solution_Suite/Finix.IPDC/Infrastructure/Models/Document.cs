using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Document")]
    public class Document : Entity
    {
        public DocumentType DocumentType { get; set; }
        public string Name { get; set; }
    }
}
