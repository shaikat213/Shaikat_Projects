using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.DTO
{
    public class PurchaseOrderDto
    {
        public long? Id { get; set; }
        public long ApplicationId { get; set; }
        public string ApplicationNo { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public long? ProposalId { get; set; }
        //public  ProposalDto Proposal { get; set; }
        public bool IsVendor { get; set; }
        public string SellersName { get; set; }
        public AddressDto SellersAddress { get; set; }
        public long? SellersAddressId { get; set; }
        public string SellersAddressString { get; set; }
        public decimal RecomendedLoanAmountFromIPDC { get; set; }
        public DateTime? QuotationDate { get; set; }
        public string QuotationDateTxt { get; set; }
        public decimal QuotationPrice { get; set; }
        public string ApplicationTitle { get; set; }
        public  List<PODocumentDto> Documents { get; set; }
        public List<long> RemovedDocuments { get; set; }
        public string CustomerName { get; set; }
        public long? CustomerAddressId { get; set; }
        public string CustomerAddressString { get; set; }
        public AddressDto CustomerAddress { get; set; }
        public string VehicleBrand { get; set; }
        public string ChassisNo { get; set; }
        public string EngineNo { get; set; }
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
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
