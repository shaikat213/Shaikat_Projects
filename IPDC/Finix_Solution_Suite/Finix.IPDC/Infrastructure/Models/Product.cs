using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    public class Product : Entity
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Prefix { get; set; }
        public ProductType? ProductType { get; set; }
        public DepositType? DepositType { get; set; }
        public int MinTerm { get; set; }
        public int MaxTerm { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public virtual ICollection<ProductRates> ProductRates { get; set; }
        public virtual ICollection<ProductSpecialRate> ProductSpecialRate { get; set; }
        public virtual ICollection<DPSMaturitySchedule> DPSMaturitySchedule { get; set; }
        public virtual ICollection<DocumentSetup> DocumentSetups { get; set; }

        //for loan products
        public decimal? ApplicationFee { get; set; }
        public decimal? MaxProcessingFeeRate { get; set; }
        public decimal? MaxProcessingFeeAmount { get; set; }
        public decimal? MaxDocFeeRate { get; set; }
        public decimal? MaxDocFeeAmount { get; set; }
        public decimal? MinCIBCharge { get; set; }
        public virtual ICollection<ProductSecurity> ProductSecurity { get; set; }
        public bool JointAccountAllowed { get; set; }

        //for deposit produt
        public bool FlexAmountAllowed { get; set; }
        public ProposalProduct? ProposalProduct { get; set; }
        public ProposalFacilityType? FacilityType { get; set; }

    }
    [Table("ProductRates")]
    public class ProductRates : Entity
    {
        public long ProductId { get; set; }
        [ForeignKey("ProductId"), InverseProperty("ProductRates")]
        public virtual Product Product { get; set; }
        public DateTime EffectiveDate { get; set; }
        public decimal CardRate { get; set; }
        public decimal PositiveVariance { get; set; }
        public decimal NegativeVariance { get; set; }
    }
    [Table("ProductSpecialRate")]
    public class ProductSpecialRate : Entity
    {
        public long ProductId { get; set; }
        [ForeignKey("ProductId"), InverseProperty("ProductSpecialRate")]
        public virtual Product Product { get; set; }

        public DateTime EffectiveDate { get; set; }
        public decimal Deviation { get; set; }
        public long AuthorizedBy { get; set; }
        [ForeignKey("AuthorizedBy")]
        public virtual OfficeDesignationSetting OfficeDesignationSetting { get; set; }
    }

    [Table("DPSMaturitySchedule")]
    public class DPSMaturitySchedule : Entity
    {
        public long ProductId { get; set; }
        [ForeignKey("ProductId"), InverseProperty("DPSMaturitySchedule")]
        public virtual Product Product { get; set; }
        public DateTime EffectiveDate { get; set; }
        public decimal InitialDeposit { get; set; }
        public decimal InstallmentAmount { get; set; }
        public int Term { get; set; }
        public decimal MaturityAmount { get; set; }

    }
    [Table("DocumentSetup")]
    public class DocumentSetup : Entity
    {
        public long ProductId { get; set; }
        [ForeignKey("ProductId"), InverseProperty("DocumentSetups")]
        public virtual Product Product { get; set; }
        public long DocId { get; set; }
        [ForeignKey("DocId")]
        public virtual Document Document { get; set; }
        public bool IsMandatory { get; set; }
        public ApplicationCustomerType CustomerType { get; set; }
        public CompanyLegalStatus? CompanyLegalStatus { get; set; }
        public DocCollectionStage? DocCollectionStage { get; set; }
    }
    [Table("ProductSecurity")]
    public class ProductSecurity : Entity
    {
        public long ProductId { get; set; }
        [ForeignKey("ProductId"), InverseProperty("ProductSecurity")]
        public virtual Product Product { get; set; }

        public string SecurityDescription { get; set; }
        public bool IsMandatory { get; set; }
    }
}
