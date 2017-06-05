using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.Infrastructure
{
    [Table("CompanyProfile")]
    public class CompanyProfile : Entity
    {
        public CompanyProfile()
        {

        }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Logo { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string ContactPerson { get; set; }
        //public long? CompanyGroupId { get; set; }
        //public decimal? Tax { get; set; }
        //public decimal? Vat { get; set; }
        public DateTime SystemDate { get; set; }
        //public DateTime ClosingTime { get; set; }
        public string VoucherNo { get; set; }
        public string JvNo { get; set; }
        public string CDvNo { get; set; }
        public string BDvNo { get; set; }
        public string CCvNo { get; set; }
        public string BCvNo { get; set; }
        public string VoucherName { get; set; }
        public string BillNo { get; set; }
        public string ChalanNo { get; set; }
        public string InvoiceNo { get; set; }
        public string PreProdNo { get; set; }
        public string ProdNo { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? SalesDate { get; set; }
        public DateTime? AccountsDate { get; set; }
        public DateTime FiscalYear { get; set; }
        public long? ParentId { get; set; }
        public CompanyType CompanyType { get; set; }
        [ForeignKey("ParentId")]
        public virtual CompanyProfile Parent { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
