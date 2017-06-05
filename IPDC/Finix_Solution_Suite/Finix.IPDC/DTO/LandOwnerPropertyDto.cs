using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.DTO
{
    public class LandOwnerPropertyDto
    {
        public long Id { get; set; }
        public long OccupationId { get; set; }
        public int? NumberOfFloorsRented { get; set; }
        public PropertyType? RentedPropertyType { get; set; }
        public decimal? RentedAreaInSqf { get; set; }
        public string ConstructionCompletionYear { get; set; }
        public long? PropertyAddressId { get; set; }
        public AddressDto PropertyAddress { get; set; }
        //public OccupationDto Occupation { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
