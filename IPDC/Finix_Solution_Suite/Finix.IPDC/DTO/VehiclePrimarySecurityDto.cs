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
    public class VehiclePrimarySecurityDto
    {
        public long Id { get; set; }
        public long LoanApplicationId { get; set; }
        public VehicleStatus VehicleStatus { get; set; }
        public string VehicleStatusName { get; set; }
        public VendorType VendorType { get; set; }
        public string VendorTypeName { get; set; }
        public string VehicleDetail { get; set; }
        public string VendorDetail { get; set; }
        public string SellersName { get; set; }
        public long? SellersAddressId { get; set; }
        public AddressDto SellersAddress { get; set; }
        public string SellersPhone { get; set; }
        public long? VendorId { get; set; }
        public string VendorName { get; set; }
        public VehicleType VehicleType { get; set; }
        public string VehicleTypeName { get; set; }
        public string VehicleName { get; set; }
        public string Manufacturer { get; set; }
        public string MnufacturingYear { get; set; }
        public string YearModel { get; set; }
        public string RegistrationYear { get; set; }
        public string RegistrationNo { get; set; }
        public string CC { get; set; }
        public string Colour { get; set; }
        public string ChassisNo { get; set; }
        public string EngineNo { get; set; }
        public decimal? Price { get; set; }
        public VerificationState? VerificationId { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
