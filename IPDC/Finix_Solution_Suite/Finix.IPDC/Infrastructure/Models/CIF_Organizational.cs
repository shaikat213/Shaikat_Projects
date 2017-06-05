using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("CIF_Organizational")]
    public class CIF_Organizational : Entity
    {
        public string CIFNo { get; set; }
        public string CBSCIFNo { get; set; }
        public bool? IsEnlistedCompany { get; set; }
        public string CompanyName { get; set; }
        public long? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Organization Company { get; set; }
        public CompanyLegalStatus? LegalStatus { get; set; }
        public string TradeLicenceNo { get; set; }
        public DateTime? TradeLicenceDate { get; set; }
        public string TL_IssueAuthority { get; set; }
        public string RegistrationNo { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string RegAuthority { get; set; }
        public long? RegCountryId { get; set; }
        [ForeignKey("RegCountryId")]
        public virtual Country RegCountry { get; set; }
        public string ETIN { get; set; }
        public string VATRegNo { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonDesignation { get; set; }
        public string ContactpersonPhone { get; set; }
        public string ContactPersonCellPhone { get; set; }
        public string ContactPersonEmail { get; set; }
        public string Website { get; set; }
        public long? RegAddressId { get; set; }
        [ForeignKey("RegAddressId")]
        public virtual Address RegAddress { get; set; }
        public long? OfficeAddressId { get; set; }
        [ForeignKey("OfficeAddressId")]
        public virtual Address OfficeAddress { get; set; }
        public virtual ICollection<FactoryAddress> FactoryAddress { get; set; }
        public virtual ICollection<CIF_Org_Owners> Owners { get; set; }
        public long? NumberOfEmployees { get; set; }
        public decimal? TotalAsset { get; set; }
        public decimal? TotalAssetExcLandAndBuilding { get; set; }
        public decimal? AnnualTurnover { get; set; }
        public BusinessType? BusinessType { get; set; }
        public BusinessSize? BusinessSize { get; set; }
        public CIF_Org_SectorType? SectorType { get; set; }
        public long? NBFI_1_SectorCodeId { get; set; }
        [ForeignKey("NBFI_1_SectorCodeId")]
        public virtual SectorCode NBFI1SectorCode { get; set; }
        public long? NBFI_2_3_SectorCodeId { get; set; }
        [ForeignKey("NBFI_2_3_SectorCodeId")]
        public virtual SectorCode NBFI2SectorCode { get; set; }
        public long? NBDC_SectorCodeId { get; set; }
        [ForeignKey("NBDC_SectorCodeId")]
        public virtual SectorCode NBDCSectorCode { get; set; }
        public virtual ICollection<CIB_Organizational> CIBs { get; set; }
        public DateTime? DateOfIncorporation { get; set; }
    }

    [Table("FactoryAddress")]
    public class FactoryAddress : Entity
    {
        public long? CIF_OrganizationalId { get; set; }
        [ForeignKey("CIF_OrganizationalId"), InverseProperty("FactoryAddress")]
        public virtual CIF_Organizational CIF_Organizational { get; set; }

        public long? AddressId { get; set; }
        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; }
    }

    [Table("CIF_Org_Owners")]
    public class CIF_Org_Owners : Entity
    {
        public long? CIF_OrganizationalId { get; set; }
        [ForeignKey("CIF_OrganizationalId"), InverseProperty("Owners")]
        public virtual CIF_Organizational CIF_Organizational { get; set; }
        public long? CIF_PersonalId { get; set; }
        [ForeignKey("CIF_PersonalId")]
        public virtual CIF_Personal CIF_Personal { get; set; }
        public CIF_Org_OwnersRole CIF_Org_OwnersRole { get; set; }
    }

    [Table("SectorCode")]
    public class SectorCode : Entity
    {
        public SectorCodeType SectorCodeType { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
