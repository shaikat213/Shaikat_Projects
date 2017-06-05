using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("LandOwnerProperty")]
    public class LandOwnerProperty : Entity
    {
        public long OccupationId { get; set; }
        public int? NumberOfFloorsRented { get; set; }
        public PropertyType? RentedPropertyType { get; set; }
        public decimal? RentedAreaInSqf { get; set; }
        [MaxLength(4, ErrorMessage = "Completion year cannot be more than 4 characters long.")]
        public string ConstructionCompletionYear { get; set; }
        public long? PropertyAddressId { get; set; }
        [ForeignKey("PropertyAddressId")]
        public virtual Address PropertyAddress { get; set; }

        [ForeignKey("OccupationId"), InverseProperty("LandOwnerProperties")]
        public virtual Occupation Occupation { get; set; }
    }
}
