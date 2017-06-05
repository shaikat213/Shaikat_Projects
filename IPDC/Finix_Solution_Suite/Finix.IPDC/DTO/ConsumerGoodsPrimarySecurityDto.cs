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
    public class ConsumerGoodsPrimarySecurityDto
    {
        public long Id { get; set; }
        public long LoanApplicationId { get; set; }
        //public virtual LoanApplication LoanApplication { get; set; }
        public string Item { get; set; }
        public string Brand { get; set; }
        public string Dealer { get; set; }
        public long? DealerAddressId { get; set; }
        public string VendorDetail { get; set; }
        public AddressDto DealerAddress { get; set; }
        public long? ShowRoomId { get; set; }
        public string ShowRoomName { get; set; }
        public string VendorName { get; set; }
        public VendorType VendorType { get; set; }
        public VendorShowroomsDto Showroom { get; set; }
        public decimal? Price { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
