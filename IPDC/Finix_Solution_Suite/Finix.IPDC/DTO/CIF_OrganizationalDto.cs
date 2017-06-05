using Finix.IPDC.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class CIF_OrganizationalDto
    {
        public CIF_OrganizationalDto()
        {
            //this.FactoryAddress = new List<FactoryAddressDto>();
            //this.RegAddress = new AddressDto();
            //this.OfficeAddress = new AddressDto();
            //this.Owners = new List<CIF_Org_OwnersDto>();
        }
        public long? Id { get; set; }
        public string CIFNo { get; set; }
        public string CBSCIFNo { get; set; }
        public string CifName { get; set; }
        public bool? IsEnlistedCompany { get; set; }
        public string CompanyName { get; set; }
        public long? CompanyId { get; set; }
        public CompanyLegalStatus? LegalStatus { get; set; }
        public string LegalStatusName { get; set; }
        public string TradeLicenceNo { get; set; }
        public DateTime? TradeLicenceDate { get; set; }
        public string TradeLicenceDateTxt { get; set; }
        public string TL_IssueAuthority { get; set; }
        public string RegistrationNo { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string RegistrationDateTxt { get; set; }
        public string RegAuthority { get; set; }
        public long? RegCountryId { get; set; }
        public string RegCountryName { get; set; }
        public string ETIN { get; set; }
        public string VATRegNo { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonDesignation { get; set; }
        public string ContactpersonPhone { get; set; }
        public string ContactPersonCellPhone { get; set; }
        public string ContactPersonEmail { get; set; }
        public string Website { get; set; }
        public long? RegAddressId { get; set; }
        public string RegAddressTxt { get; set; }
        public AddressDto RegAddress { get; set; }
        public long? OfficeAddressId { get; set; }
        public string OfficeAddressTxt { get; set; }
        public AddressDto OfficeAddress { get; set; }
        public List<FactoryAddressDto> FactoryAddress { get; set; }
        public List<long> RemovedFactoryAddress { get; set; }
        public List<CIF_Org_OwnersDto> Owners { get; set; }
        public List<long> RemovedOwners { get; set; }
        public long? NumberOfEmployees { get; set; }
        public decimal? TotalAsset { get; set; }
        public decimal? TotalAssetExcLandAndBuilding { get; set; }
        public decimal? AnnualTurnover { get; set; }
        public BusinessType? BusinessType { get; set; }
        public string BusinessTypeName { get; set; }
        public BusinessSize? BusinessSize { get; set; }
        public string BusinessSizeName { get; set; }
        public CIF_Org_SectorType? SectorType { get; set; }
        public string SectorTypeName { get; set; }
        public long? NBFI_1_SectorCodeId { get; set; }
        public SectorCodeDto NBFI1SectorCode { get; set; }
        public long? NBFI_2_3_SectorCodeId { get; set; }
        public SectorCodeDto NBFI2SectorCode { get; set; }
        public long? NBDC_SectorCodeId { get; set; }
        public SectorCodeDto NBDCSectorCode { get; set; }
        public DateTime? DateOfIncorporation { get; set; }
        public string DateOfIncorporationTxt { get; set; }
        public string test { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }

    public class FactoryAddressDto
    {
        public FactoryAddressDto()
        {
            //this.Address = new AddressDto();
        }
        public long? Id { get; set; }
        public long? CIF_OrganizationalId { get; set; }
        public long? AddressId { get; set; }
        public string AddressTxt { get; set; }
        public AddressDto Address { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }

    }

    public class CIF_Org_OwnersDto
    {
        public CIF_Org_OwnersDto()
        {
            //this.CIF_Personal = new CIF_PersonalDto();
        }
        public long? Id { get; set; }
        public long? ApplicationId { get; set; }
        public long? CIF_OrganizationalId { get; set; }
        public long? CIF_PersonalId { get; set; }
        public string CIFNo { get; set; }
        public CIF_KeyVal CIF_Personal { get; set; }
        public string Name { get; set; }
        public CIF_Org_OwnersRole? CIF_Org_OwnersRole { get; set; }
        public string CIF_Org_OwnersRoleName { get; set; }
        public int? Age { get; set; }
        public string ProfessionName { get; set; }
        public decimal MonthlyIncome { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
    public class SectorCodeDto
    {
        public long? Id { get; set; }
        public SectorCodeType? SectorCodeType { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
