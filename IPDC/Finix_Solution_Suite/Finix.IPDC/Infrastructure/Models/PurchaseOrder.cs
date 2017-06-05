using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("PurchaseOrder")]
    public class PurchaseOrder : Entity
    {
        public long ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public virtual Application Application { get; set; }
        public long? ProposalId { get; set; }
        [ForeignKey("ProposalId")]
        public virtual Proposal Proposal { get; set; }
        public bool IsVendor { get; set; }
        public string SellersName { get; set; }
        public long? SellersAddressId { get; set; }
        [ForeignKey("SellersAddressId")]
        public virtual Address SellersAddress { get; set; }
        public decimal RecomendedLoanAmountFromIPDC { get; set; }
        public DateTime? QuotationDate { get; set; }
        public decimal QuotationPrice { get; set; }
        public string ApplicationTitle { get; set; }
        public virtual ICollection<PODocument> Documents { get; set; }
        public string CustomerName { get; set; }
        public long? CustomerAddressId { get; set; }
        [ForeignKey("CustomerAddressId")]
        public virtual Address CustomerAddress { get; set; }
        public string VehicleBrand { get; set; }
        public string ChassisNo { get; set; }
        public string EngineNo { get; set; }
        [MaxLength(4)]
        public string ManufacturingYear { get; set; }
        public string Colour { get; set; }
        public string CC { get; set; }
        public int Valitity { get; set; }
        public bool? VendorApproved { get; set; }
        public DateTime? VendorApprovalDate { get; set; }
        public bool? CustomerApproved { get; set; }
        public DateTime? CustomerApprovalDate { get; set; }
        public bool? POApproval { get; set; }
        public DateTime? POApprovalDate { get; set; }
    }

    [Table("PODocument")]
    public class PODocument : Entity
    {
        public long POId { get; set; }
        [ForeignKey("POId"), InverseProperty("Documents")]
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public string Name { get; set; }
    }
}
