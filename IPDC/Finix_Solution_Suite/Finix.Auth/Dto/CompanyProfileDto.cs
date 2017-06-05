using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.Auth.Infrastructure;

namespace Finix.Auth.DTO
{
    public class CompanyProfileDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }
        public byte[] Logo { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string ContactPerson { get; set; }
        public int? CompanyBusinessTypeId { get; set; }
        public int? CompanyGroupId { get; set; }
        public int EntryBy { get; set; }
        public DateTime? EntryDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public decimal? Tax { get; set; }
        public decimal? Vat { get; set; }
        public DateTime SystemDate { get; set; }
        public DateTime ClosingTime { get; set; }
        public string RvVoucherNo { get; set; }
        public string PvVoucherNo { get; set; }
        public string VoucherNo { get; set; }
        public string VoucherName { get; set; }
        public string BillNo { get; set; }
        public string InvoiceNo { get; set; }
        public string DVNo { get; set; }
        public string CVNo { get; set; }
        public string JVNo { get; set; }
        public string ProdNo { get; set; }
        public CompanyType CompanyType { get; set; }
        public string CompanyTypeName { get; set; }
        public long? ParentId { get; set; }
        public string ParentName { get; set; }

    }

    
}
