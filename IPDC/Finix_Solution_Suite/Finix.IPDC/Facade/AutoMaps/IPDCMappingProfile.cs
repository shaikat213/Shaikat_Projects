using AutoMapper;
using Finix.IPDC.DTO;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;
using Finix.IPDC.Util;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Finix.IPDC.Facade.AutoMaps
{
    public class IPDCMappingProfile : Profile
    {
        protected override void Configure()
        {
            base.Configure();
            CreateMap<Organization, OrganizationDto>()
                .ForMember(d => d.OrganizationTypeName, o => o.MapFrom(s => s.OrganizationType > 0 ? UiUtil.GetDisplayName(s.OrganizationType) : ""))
                .ForMember(d => d.PriorityName, o => o.MapFrom(s => s.Priority > 0 ? UiUtil.GetDisplayName(s.Priority) : ""));
            CreateMap<OrganizationDto, Organization>();

            CreateMap<Address, AddressDto>()
                 .ForMember(d => d.ThanaName, o => o.MapFrom(s => s.Thana.ThanaNameEng))
                 .ForMember(d => d.DistrictName, o => o.MapFrom(s => s.District.DistrictNameEng))
                 .ForMember(d => d.DivisionName, o => o.MapFrom(s => s.Division.DivisionNameEng))
                 .ForMember(d => d.CountryName, o => o.MapFrom(s => s.Country.Name));
            CreateMap<AddressDto, Address>();

            CreateMap<Designation, DesignationDto>()
                .ForMember(d => d.ParentName, o => o.MapFrom(s => s.ParentDesignation.Name));
            CreateMap<DesignationDto, Designation>();

            CreateMap<SalesLead, SalesLeadDto>()
                .ForMember(d => d.CallCategoryName, o => o.MapFrom(s => s.CallCategory > 0 ? UiUtil.GetDisplayName(s.CallCategory) : ""))
                .ForMember(d => d.GenderName, o => o.MapFrom(s => s.Gender > 0 ? UiUtil.GetDisplayName(s.Gender) : ""))
                .ForMember(d => d.AgeRangeName, o => o.MapFrom(s => s.AgeRange > 0 ? UiUtil.GetDisplayName(s.AgeRange) : ""))
                .ForMember(d => d.IncomeRangeName, o => o.MapFrom(s => s.IncomeRange > 0 ? UiUtil.GetDisplayName(s.IncomeRange) : ""))
                .ForMember(d => d.CallTypeName, o => o.MapFrom(s => s.CallType > 0 ? UiUtil.GetDisplayName(s.CallType) : ""))
                .ForMember(d => d.CallStatusName, o => o.MapFrom(s => UiUtil.GetDisplayName(s.CallStatus)))
                .ForMember(d => d.CustomerPriorityName, o => o.MapFrom(s => s.CustomerPriority > 0 ? UiUtil.GetDisplayName(s.CustomerPriority) : ""))
                .ForMember(d => d.CallModeName, o => o.MapFrom(s => s.CallMode > 0 ? UiUtil.GetDisplayName(s.CallMode) : ""))
                .ForMember(d => d.MaritalStatusName, o => o.MapFrom(s => s.MaritalStatus > 0 ? UiUtil.GetDisplayName(s.MaritalStatus) : ""))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name))
                .ForMember(d => d.LeadStatusName, o => o.MapFrom(s => s.LeadStatus != null ? UiUtil.GetDisplayName(s.LeadStatus) : ""))
                .ForMember(d => d.LeadPriorityName, o => o.MapFrom(s => s.LeadPriority > 0 ? UiUtil.GetDisplayName(s.LeadPriority) : ""))
                .ForMember(d => d.CustomerSensitivityName, o => o.MapFrom(s => s.CustomerSensitivity > 0 ? UiUtil.GetDisplayName(s.CustomerSensitivity) : ""))
                .ForMember(d => d.FollowupTimeText, o => o.MapFrom(s => s.FollowupTime != null ? ((DateTime)s.FollowupTime).ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture) : ""));

            CreateMap<SalesLeadDto, SalesLead>()
                .ForMember(d => d.CustomerAddress, o => o.Ignore());

            CreateMap<FollowupTrack, FollowupTrackDto>()
                .ForMember(d => d.FollowupTypeName, o => o.MapFrom(s => s.FollowupType > 0 ? UiUtil.GetDisplayName(s.FollowupType) : ""));
            CreateMap<FollowupTrackDto, FollowupTrack>();

            CreateMap<Division, DivisionDto>();
            CreateMap<DivisionDto, Division>();

            CreateMap<Vendor, VendorDto>();
            CreateMap<VendorDto, Vendor>();

            CreateMap<DesignationRoleMapping, DesignationRoleMappingDto>();
            CreateMap<DesignationRoleMappingDto, DesignationRoleMapping>();

            CreateMap<DPSMaturitySchedule, DPSMaturityScheduleDto>();
            CreateMap<DPSMaturityScheduleDto, DPSMaturitySchedule>();

            CreateMap<DesignationProductMapping, DesignationProductMappingDto>()
                .ForMember(d => d.OfficeDesignationSettingName, o => o.MapFrom(s => s.OfficeDesignationSetting.Name))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name));
            CreateMap<DesignationProductMappingDto, DesignationProductMapping>();

            CreateMap<Product, ProductDto>()
                .ForMember(d => d.FacilityTypeName, o => o.MapFrom(s => s.FacilityType != null ? UiUtil.GetDisplayName(s.FacilityType) : ""))
                .ForMember(d => d.ProductTypeName, o => o.MapFrom(s => s.ProductType != null ? UiUtil.GetDisplayName(s.ProductType) : ""))
                 .ForMember(d => d.DepositTypeName, o => o.MapFrom(s => s.DepositType != null ? UiUtil.GetDisplayName(s.DepositType) : ""));
            CreateMap<ProductDto, Product>()
                 .ForMember(d => d.ProductRates, o => o.Ignore())
                  .ForMember(d => d.ProductSpecialRate, o => o.Ignore())
                   .ForMember(d => d.DPSMaturitySchedule, o => o.Ignore())
                    .ForMember(d => d.DocumentSetups, o => o.Ignore())
                     .ForMember(d => d.ProductSecurity, o => o.Ignore());

            CreateMap<ProductSecurity, ProductSecurityDto>();
            CreateMap<ProductSecurityDto, ProductSecurity>();

            CreateMap<ProductRates, ProductRatesDto>();
            CreateMap<ProductRatesDto, ProductRates>();

            CreateMap<ProductSpecialRate, ProductSpecialRateDto>();
            CreateMap<ProductSpecialRateDto, ProductSpecialRate>();

            CreateMap<QuestionAnswer, QuestionAnswerDto>()
                .ForMember(d => d.QuestionText, o => o.MapFrom(s => s.Question.Questions));
            //.ForMember(d => d.SalesLeadName, o => o.MapFrom(s => s.SalesLead.Name));ProductRates
            CreateMap<QuestionAnswerDto, QuestionAnswer>();
            CreateMap<Call, SalesLead>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.Call_Id, o => o.MapFrom(s => s.Id));
            CreateMap<Call, SalesLeadDto>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.Call_Id, o => o.MapFrom(s => s.Id));

            CreateMap<CallDto, SalesLeadDto>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.Call_Id, o => o.MapFrom(s => s.Id));

            CreateMap<SalesLeadAssignment, SalesLeadAssignmentDto>()
                .ForMember(d => d.AssignedByName, o => o.MapFrom(s => s.AssignedByEmp.Person.FirstName))
                .ForMember(d => d.AssignedToName, o => o.MapFrom(s => s.AssignedToEmp.Person.FirstName))
                //.ForMember(d => d.SalesLeadName, o => o.MapFrom(s => s.SalesLead.Name))
                //.ForMember(d => d.SalesLeadAddress, o => o.MapFrom(s => s.SalesLead.AddressLine1))
                //.ForMember(d => d.SalesLeadAddress, o => o.MapFrom(s => s.SalesLead.AddressLine2))
                //.ForMember(d => d.SalesLeadAddress, o => o.MapFrom(s => s.SalesLead.AddressLine3))
                .ForMember(d => d.FollowUpTimeTxt, o => o.MapFrom(s => s.FollowUpTime.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)));
            //.ForMember(d => d.CreatedByName, o=> o.MapFrom(s=> ));
            CreateMap<SalesLeadAssignmentDto, SalesLeadAssignment>();


            CreateMap<Thana, ThanaDto>();
            CreateMap<ThanaDto, Thana>();

            CreateMap<OfficeDesignationArea, OfficeDesignationAreaDto>();
            CreateMap<OfficeDesignationAreaDto, OfficeDesignationArea>();

            CreateMap<District, DistrictDto>()
                 .ForMember(d => d.Name, o => o.MapFrom(s => s.DistrictNameEng));
            CreateMap<DistrictDto, District>();

            CreateMap<Upazila, UpzilaDto>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.UpazilaNameEng));
            CreateMap<UpzilaDto, Upazila>();

            CreateMap<Employee, EmployeeDto>()
                 .ForMember(d => d.Name, o => o.MapFrom(s => s.PersonId != null ? s.Person.FirstName + " " + s.Person.LastName : ""))
                 .ForMember(d => d.EmployeeTypeName, o => o.MapFrom(s => UiUtil.GetDisplayName(s.EmployeeType)))
                 .ForMember(d => d.JoiningDateText, o => o.MapFrom(s => s.JoiningDate != null ? ((DateTime)s.JoiningDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : ""));
            CreateMap<EmployeeDto, Employee>();

            CreateMap<OfficeDesignationSetting, OfficeDesignationSettingDto>()
                .ForMember(d => d.ParentDesignationSettingName, o => o.MapFrom(s => s.ParentOfficeDesignationSetting.Name))
                .ForMember(d => d.OfficeName, o => o.MapFrom(s => s.Office.Name))
                .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.Designation.Name));
            CreateMap<OfficeDesignationSettingDto, OfficeDesignationSetting>();

            CreateMap<EmployeeDesignationMapping, EmployeeDesignationMappingDto>()
                .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Employee.Person.FirstName + " " + s.Employee.Person.LastName))
                .ForMember(d => d.OfficeDesignationSettingName, o => o.MapFrom(s => s.OfficeDesignationSetting.Name))
                .ForMember(d => d.EmpCode, o => o.MapFrom(s => s.Employee.EmpCode));
            CreateMap<EmployeeDesignationMappingDto, EmployeeDesignationMapping>();

            CreateMap<Country, CountryDto>();
            CreateMap<CountryDto, Country>();

            CreateMap<Nationality, NationalityDto>();
            CreateMap<NationalityDto, Nationality>();

            CreateMap<Profession, ProfessionDto>();
            CreateMap<ProfessionDto, Profession>();

            CreateMap<CIF_Personal, CIF_PersonalDto>()
                .ForMember(d => d.NationalityName, o => o.MapFrom(s => s.Nationality.Name))
                //.ForMember(d => d.PhotoName, o => o.MapFrom(s => s.Photo))
                .ForMember(d => d.BirthCountryName, o => o.MapFrom(s => s.BirthCountry.Name))
                .ForMember(d => d.BirthDistrictName, o => o.MapFrom(s => s.BirthDistrict.DistrictNameEng))
                .ForMember(d => d.PassportIssueCountryName, o => o.MapFrom(s => s.PassportIssueCountry.Name))
                .ForMember(d => d.MaritalStatusName, o => o.MapFrom(s => s.MaritalStatus > 0 ? UiUtil.GetDisplayName(s.MaritalStatus) : ""))
                .ForMember(d => d.DLIssueCountryName, o => o.MapFrom(s => s.DLIssueCountry.Name))
                .ForMember(d => d.SpouseProfessionName, o => o.MapFrom(s => s.SpouseProfession.Name))
                .ForMember(d => d.GenderName, o => o.MapFrom(s => s.Gender))
                .ForMember(d => d.HighestEducationLevelName, o => o.MapFrom(s => s.HighestEducationLevel != null ? UiUtil.GetDisplayName(s.HighestEducationLevel) : ""))
                .ForMember(d => d.ResidenceStatusName, o => o.MapFrom(s => s.ResidenceStatus > 0 ? UiUtil.GetDisplayName(s.ResidenceStatus) : ""))
                .ForMember(d => d.HomeOwnershipName, o => o.MapFrom(s => s.HomeOwnership != null ? UiUtil.GetDisplayName(s.HomeOwnership) : ""))
                .ForMember(d => d.YearsInCurrentResidenceName, o => o.MapFrom(s => s.YearsInCurrentResidence != null ? UiUtil.GetDisplayName(s.YearsInCurrentResidence) : ""))
                .ForMember(d => d.DateOfBirthText, o => o.ResolveUsing(s => s.DateOfBirth != null ? ((DateTime)s.DateOfBirth).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : ""))
                .ForMember(d => d.PassportIssueDateText, o => o.ResolveUsing(s => s.PassportIssueDate != null ? ((DateTime)s.PassportIssueDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : ""))
                .ForMember(d => d.CommCertIssueDateText, o => o.ResolveUsing(s => s.CommCertIssueDate != null ? ((DateTime)s.CommCertIssueDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : ""))
                .ForMember(d => d.PermanentAddressName, o => o.MapFrom(s => s.PermanentAddressId != null ?
                            (!string.IsNullOrEmpty(s.PermanentAddress.AddressLine1) ? s.PermanentAddress.AddressLine1 : "")
                          + (!string.IsNullOrEmpty(s.PermanentAddress.AddressLine2) ? (", " + s.PermanentAddress.AddressLine2) : "")
                          + (!string.IsNullOrEmpty(s.PermanentAddress.AddressLine3) ? (", " + s.PermanentAddress.AddressLine3) : "")
                          + (s.PermanentAddress.ThanaId != null ? (", " + s.PermanentAddress.Thana.ThanaNameEng) : "")
                          + (s.PermanentAddress.DistrictId != null ? (", " + s.PermanentAddress.District.DistrictNameEng) : "")
                          + (s.PermanentAddress.DivisionId != null ? (", " + s.PermanentAddress.Division.DivisionNameEng) : "")
                          + (s.PermanentAddress.CountryId != null ? (", " + s.PermanentAddress.Country.Name) : "")
                          + (!string.IsNullOrEmpty(s.PermanentAddress.PostalCode) ? ", Postal Code # " + s.PermanentAddress.PostalCode : "")
                          + (!string.IsNullOrEmpty(s.PermanentAddress.PhoneNo) ? ", Phone # " + s.PermanentAddress.PhoneNo : "")
                          + (!string.IsNullOrEmpty(s.PermanentAddress.CellPhoneNo) ? ", Mobile # " + s.PermanentAddress.CellPhoneNo : "")
                          + (!string.IsNullOrEmpty(s.PermanentAddress.Email) ? ", Email # " + s.PermanentAddress.Email : "")
                           : ""))
                .ForMember(d => d.ResidenceAddressName, o => o.MapFrom(s => s.ResidenceAddressId != null ?
                (!string.IsNullOrEmpty(s.ResidenceAddress.AddressLine1) ? s.ResidenceAddress.AddressLine1 : "") +
                          (!string.IsNullOrEmpty(s.ResidenceAddress.AddressLine2) ? (", " + s.ResidenceAddress.AddressLine2) : "")
                          + (!string.IsNullOrEmpty(s.ResidenceAddress.AddressLine3) ? (", " + s.ResidenceAddress.AddressLine3) : "")
                          + (s.ResidenceAddress.ThanaId != null ? (", " + s.ResidenceAddress.Thana.ThanaNameEng) : "")
                          + (s.ResidenceAddress.DistrictId != null ? (", " + s.ResidenceAddress.District.DistrictNameEng) : "")
                          + (s.ResidenceAddress.DivisionId != null ? (", " + s.ResidenceAddress.Division.DivisionNameEng) : "")
                          + (s.ResidenceAddress.CountryId != null ? (", " + s.ResidenceAddress.Country.Name) : "")
                          + (!string.IsNullOrEmpty(s.ResidenceAddress.PostalCode) ? ", Postal Code # " + s.ResidenceAddress.PostalCode : "")
                          + (!string.IsNullOrEmpty(s.ResidenceAddress.PhoneNo) ? ", Phone # " + s.ResidenceAddress.PhoneNo : "")
                          + (!string.IsNullOrEmpty(s.ResidenceAddress.CellPhoneNo) ? ", Mobile # " + s.ResidenceAddress.CellPhoneNo : "")
                          + (!string.IsNullOrEmpty(s.ResidenceAddress.Email) ? ", Email # " + s.ResidenceAddress.Email : "")
                          : ""
                    ))
                    .ForMember(d => d.SpouseWorkAddressName, o => o.MapFrom(s => s.SpouseWorkAddressId != null ?
                    (!string.IsNullOrEmpty(s.SpouseWorkAddress.AddressLine1) ? s.SpouseWorkAddress.AddressLine1 : "") +
                          (!string.IsNullOrEmpty(s.SpouseWorkAddress.AddressLine2) ? (", " + s.SpouseWorkAddress.AddressLine2) : "")
                          + (!string.IsNullOrEmpty(s.SpouseWorkAddress.AddressLine3) ? (", " + s.SpouseWorkAddress.AddressLine3) : "")
                          + (s.SpouseWorkAddress.ThanaId != null ? (", " + s.SpouseWorkAddress.Thana.ThanaNameEng) : "")
                          + (s.SpouseWorkAddress.DistrictId != null ? (", " + s.SpouseWorkAddress.District.DistrictNameEng) : "")
                          + (s.SpouseWorkAddress.DivisionId != null ? (", " + s.SpouseWorkAddress.Division.DivisionNameEng) : "")
                          + (s.SpouseWorkAddress.CountryId != null ? (", " + s.SpouseWorkAddress.Country.Name) : "")
                          + (!string.IsNullOrEmpty(s.SpouseWorkAddress.PostalCode) ? ", Postal Code # " + s.SpouseWorkAddress.PostalCode : "")
                          + (!string.IsNullOrEmpty(s.SpouseWorkAddress.PhoneNo) ? ", Phone # " + s.SpouseWorkAddress.PhoneNo : "")
                          + (!string.IsNullOrEmpty(s.SpouseWorkAddress.CellPhoneNo) ? ", Mobile # " + s.SpouseWorkAddress.CellPhoneNo : "")
                          + (!string.IsNullOrEmpty(s.SpouseWorkAddress.Email) ? ", Email # " + s.SpouseWorkAddress.Email : "")
                          : ""
                    ))
                     .ForMember(d => d.BankAccounts, o => o.Ignore())
                .ForMember(d => d.Photo, o => o.Ignore())
                .ForMember(d => d.SignaturePhoto, o => o.Ignore());



            CreateMap<CIF_PersonalDto, CIF_Personal>()
                .ForMember(d => d.ResidenceAddress, o => o.Ignore())
                .ForMember(d => d.PermanentAddress, o => o.Ignore())
                .ForMember(d => d.SpouseWorkAddress, o => o.Ignore())
                .ForMember(d => d.ContactPointVerifications, o => o.Ignore())
                .ForMember(d => d.CIBs, o => o.Ignore())
                .ForMember(d => d.VisitReports, o => o.Ignore())
                .ForMember(d => d.NIDVerifications, o => o.Ignore())
                .ForMember(d => d.CreditCards, o => o.Ignore())
                .ForMember(d => d.BankAccounts, o => o.Ignore())
                .ForMember(d => d.Photo, o => o.Ignore())
                .ForMember(d => d.SignaturePhoto, o => o.Ignore());

            CreateMap<CIF_Reference, CIF_ReferenceDto>()
                .ForMember(d => d.CIFNo, o => o.MapFrom(s => s.CIF_Personal.CIFNo));
            CreateMap<CIF_ReferenceDto, CIF_Reference>()
                .ForMember(d => d.ResidenceAddress, o => o.Ignore())
                .ForMember(d => d.PermanentAddress, o => o.Ignore())
                .ForMember(d => d.OfficeAddress, o => o.Ignore());

            CreateMap<CIF_IncomeStatement, CIF_IncomeStatementDto>();
            CreateMap<CIF_IncomeStatementDto, CIF_IncomeStatement>()
                .ForMember(d => d.Verifications, o => o.Ignore())
                .ForMember(d => d.MonthlyOtherIncomesDeclared, o => o.Ignore());



            CreateMap<CIF_NetWorth, CIF_NetWorthDto>()
                .ForMember(d => d.SavingsInBank, o => o.MapFrom(s => s.SavingsInBank.Where(c => c.Status == EntityStatus.Active)))
                .ForMember(d => d.BusinessShares, o => o.MapFrom(s => s.BusinessShares.Where(c => c.Status == EntityStatus.Active)))
                .ForMember(d => d.Investments, o => o.MapFrom(s => s.Investments.Where(c => c.Status == EntityStatus.Active)))
                .ForMember(d => d.Liabilities, o => o.MapFrom(s => s.Liabilities.Where(c => c.Status == EntityStatus.Active)))
                .ForMember(d => d.Properties, o => o.MapFrom(s => s.Properties.Where(c => c.Status == EntityStatus.Active)));

            CreateMap<CIF_NetWorthDto, CIF_NetWorth>()
                .ForMember(d => d.SavingsInBank, o => o.Ignore())
                .ForMember(d => d.BusinessShares, o => o.Ignore())
                .ForMember(d => d.Investments, o => o.Ignore())
                .ForMember(d => d.Liabilities, o => o.Ignore())
                .ForMember(d => d.Properties, o => o.Ignore())
                .ForMember(d => d.NetWorthVerifications, o => o.Ignore());

            CreateMap<CIF_SavingsInBank, CIF_SavingsInBankDto>();
            CreateMap<CIF_SavingsInBankDto, CIF_SavingsInBank>();

            CreateMap<CIF_Investment, CIF_InvestmentDto>();
            CreateMap<CIF_InvestmentDto, CIF_Investment>();

            CreateMap<CIF_NW_Property, CIF_NW_PropertyDto>();
            CreateMap<CIF_NW_PropertyDto, CIF_NW_Property>();

            CreateMap<CIF_BusinessShares, CIF_BusinessSharesDto>();
            CreateMap<CIF_BusinessSharesDto, CIF_BusinessShares>();

            CreateMap<CIF_Liability, CIF_LiabilityDto>();
            CreateMap<CIF_LiabilityDto, CIF_Liability>();

            CreateMap<CreditCard, CreditCardDto>()
                .ForMember(d => d.CreditCardIssueDateText, o => o.MapFrom(s => s.CreditCardIssueDate != null ? ((DateTime)s.CreditCardIssueDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : ""));
            CreateMap<CreditCardDto, CreditCard>();

            CreateMap<Occupation, OccupationDto>()
                    .ForMember(d => d.OfficeAddressName, o => o.MapFrom(s => s.OfficeAddressId != null ?
                            (!string.IsNullOrEmpty(s.OfficeAddress.AddressLine1) ? s.OfficeAddress.AddressLine1 : "")
                          + (!string.IsNullOrEmpty(s.OfficeAddress.AddressLine2) ? (", " + s.OfficeAddress.AddressLine2) : "")
                          + (!string.IsNullOrEmpty(s.OfficeAddress.AddressLine3) ? (", " + s.OfficeAddress.AddressLine3) : "")
                          + (s.OfficeAddress.ThanaId != null ? (", " + s.OfficeAddress.Thana.ThanaNameEng) : "")
                          + (s.OfficeAddress.DistrictId != null ? (", " + s.OfficeAddress.District.DistrictNameEng) : "")
                          + (s.OfficeAddress.DivisionId != null ? (", " + s.OfficeAddress.Division.DivisionNameEng) : "")
                          + (s.OfficeAddress.CountryId != null ? (", " + s.OfficeAddress.Country.Name) : "")
                          + (!string.IsNullOrEmpty(s.OfficeAddress.PostalCode) ? ", Postal Code # " + s.OfficeAddress.PostalCode : "")
                          + (!string.IsNullOrEmpty(s.OfficeAddress.PhoneNo) ? ", Phone # " + s.OfficeAddress.PhoneNo : "")
                          + (!string.IsNullOrEmpty(s.OfficeAddress.CellPhoneNo) ? ", Mobile # " + s.OfficeAddress.CellPhoneNo : "")
                          + (!string.IsNullOrEmpty(s.OfficeAddress.Email) ? ", Email # " + s.OfficeAddress.Email : "")
                          : ""))
                      .ForMember(d => d.RelationshipWithIPDCName, o => o.MapFrom(s => s.RelationshipWithIPDC > 0 ? UiUtil.GetDisplayName(s.RelationshipWithIPDC) : ""))
                      .ForMember(d => d.RoleInBankName, o => o.MapFrom(s => s.RoleInBankOrFL > 0 ? UiUtil.GetDisplayName(s.RoleInBankOrFL) : ""));
            CreateMap<OccupationDto, Occupation>()
                .ForMember(d => d.OfficeAddress, o => o.Ignore())
                .ForMember(d => d.LandOwnerProperties, o => o.Ignore());

            CreateMap<LandOwnerProperty, LandOwnerPropertyDto>();
            CreateMap<LandOwnerPropertyDto, LandOwnerProperty>()
                .ForMember(d => d.Occupation, o => o.Ignore())
                .ForMember(d => d.PropertyAddress, o => o.Ignore());

            //AppDocChecklistDto
            CreateMap<ApplicationCIFs, ApplicationCIFsDto>()
                .ForMember(d => d.ProfessionName, o => o.MapFrom(s => s.CIF_Personal.Occupation.Profession.Name))
                .ForMember(d => d.OccupationTypeName, o => o.MapFrom(s => s.CIF_Personal.Occupation.OccupationType > 0 ? UiUtil.GetDisplayName(s.CIF_Personal.Occupation.OccupationType) : ""))
                .ForMember(d => d.ApplicantRoleName, o => o.MapFrom(s => s.ApplicantRole != null ? UiUtil.GetDisplayName(s.ApplicantRole) : ""))
                .ForMember(d => d.CIFNo, o => o.MapFrom(s => s.CIF_Personal.CIFNo))
                .ForMember(d => d.CBSCIFNo, o => o.MapFrom(s => s.CIF_Personal.CBSCIFNo))
                .ForMember(d => d.ApplicantName, o => o.MapFrom(s => s.CIF_PersonalId != null ? s.CIF_Personal.CIFNo + " - " + s.CIF_Personal.Name : s.CIF_OrganizationalId != null ? s.CIF_Organizational.CIFNo + " - " + s.CIF_Organizational.CompanyName : ""))
                .ForMember(d => d.Age, o => o.MapFrom(s => (s.CIF_Personal.DateOfBirth == null ? 0 : (DateTime.Now.Year - ((DateTime)s.CIF_Personal.DateOfBirth).Year)))).ForMember(d => d.RelationshipWithApplicantName, o => o.MapFrom(s => s.RelationshipWithApplicant > 0 ? UiUtil.GetDisplayName(s.RelationshipWithApplicant) : ""));
            CreateMap<ApplicationCIFsDto, ApplicationCIFs>()
                .ForMember(d => d.CIF_Personal, o => o.Ignore())
                .ForMember(d => d.Application, o => o.Ignore())
                .ForMember(d => d.CIF_Organizational, o => o.Ignore());


            CreateMap<DocumentSetup, DocumentSetupDto>();
            CreateMap<DocumentSetupDto, DocumentSetup>();

            CreateMap<AppDocChecklist, AppDocChecklistDto>()
                .ForMember(d => d.DocName, o => o.MapFrom(s => s.ProductDoc.Document.Name))
                .ForMember(d => d.IsChecked, o => o.MapFrom(s => true));

            CreateMap<AppDocChecklistDto, AppDocChecklist>()
                .ForMember(d => d.ProductDoc, o => o.Ignore())
                .ForMember(d => d.Application, o => o.Ignore())
                .ForMember(d => d.ApprovedBy, o => o.Ignore());

            CreateMap<Application, ApplicationDto>()
                .ForMember(d => d.ApplicationDateText, o => o.MapFrom(s => s.ApplicationDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)))
                .ForMember(d => d.ContactPersonName, o => o.MapFrom(s => s.ContactPerson != null ? s.ContactPerson.Name : ""))
                .ForMember(d => d.CustomerTypeName, o => o.MapFrom(s => UiUtil.GetDisplayName(s.CustomerType)))
                .ForMember(d => d.ApplicationTypeName, o => o.MapFrom(s => s.ApplicationType != null ? UiUtil.GetDisplayName(s.ApplicationType) : ""))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name))
                .ForMember(d => d.ProductTypeName, o => o.MapFrom(s => UiUtil.GetDisplayName(s.ProductType))) // .ForMember(d => d.ApplicationDateText, o => o.MapFrom(s => s.ApplicationDate.ToString("dd/MM/yyyy HH:mm")));
                .ForMember(d => d.ApplicationStageName, o => o.MapFrom(s => (s.ApplicationStage != null ? UiUtil.GetDisplayName(s.ApplicationStage) : "")))
                .ForMember(d => d.CostCenterName, o => o.MapFrom(s => s.CostCenterId != null ? s.CostCenter.Name : ""))
                .ForMember(d => d.BranchName, o => o.MapFrom(s => s.BranchId != null ? s.BranchOffice.Name : ""))
                .ForMember(d => d.GroupAddressName, o => o.MapFrom(s => s.GroupAddressId != null ?
                            (!string.IsNullOrEmpty(s.GroupAddress.AddressLine1) ? s.GroupAddress.AddressLine1 : "")
                          + (!string.IsNullOrEmpty(s.GroupAddress.AddressLine2) ? (", " + s.GroupAddress.AddressLine2) : "")
                          + (!string.IsNullOrEmpty(s.GroupAddress.AddressLine3) ? (", " + s.GroupAddress.AddressLine3) : "")
                          + (s.GroupAddress.ThanaId != null ? (", " + s.GroupAddress.Thana.ThanaNameEng) : "")
                          + (s.GroupAddress.DistrictId != null ? (", " + s.GroupAddress.District.DistrictNameEng) : "")
                          + (s.GroupAddress.DivisionId != null ? (", " + s.GroupAddress.Division.DivisionNameEng) : "")
                          + (s.GroupAddress.CountryId != null ? (", " + s.GroupAddress.Country.Name) : "")
                          + (s.GroupAddress.PostalCode != null ? (", " + s.GroupAddress.PostalCode) : "")
                          + (s.GroupAddress.PhoneNo != null ? (", " + s.GroupAddress.PhoneNo) : "")
                          + (s.GroupAddress.CellPhoneNo != null ? (", " + s.GroupAddress.CellPhoneNo) : "")
                          + (s.GroupAddress.Email != null ? (", " + s.GroupAddress.Email) : "") : ""))
                .ForMember(d => d.RMName, o => o.MapFrom(s => s.RMId != null ? s.RMEmp.Person.FirstName + " " + s.RMEmp.Person.LastName : ""));
            CreateMap<ApplicationDto, Application>()
                .ForMember(d => d.GroupAddress, o => o.Ignore())
                .ForMember(d => d.CIFList, o => o.Ignore())
                .ForMember(d => d.DocChecklist, o => o.Ignore());


            CreateMap<Application, ApplicationDetailDto>()
             .ForMember(d => d.ApplicationDateText, o => o.MapFrom(s => s.ApplicationDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)))
             .ForMember(d => d.CustomerTypeName, o => o.MapFrom(s => UiUtil.GetDisplayName(s.CustomerType)))
             .ForMember(d => d.ApplicationTypeName, o => o.MapFrom(s => s.ApplicationType != null ? UiUtil.GetDisplayName(s.ApplicationType) : ""))
             .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name))
             .ForMember(d => d.ProductTypeName, o => o.MapFrom(s => UiUtil.GetDisplayName(s.ProductType)))
             .ForMember(d => d.CIFList, o => o.MapFrom(s => s.CIFList.Where(c => c.Status == EntityStatus.Active)))
             .ForMember(d => d.BranchName, o => o.MapFrom(s => s.BranchId != null ? s.BranchOffice.Name : ""))
             .ForMember(d => d.RMName, o => o.MapFrom(s => s.RMId != null ? s.RMEmp.Person.Name : ""))
             .ForMember(d => d.LeadPriorityName, o => o.MapFrom(s => s.SalesLeadId != null ? s.SalesLead.LeadPriority != null ? UiUtil.GetDisplayName(s.SalesLead.LeadPriority) : "" : ""))
             .ForMember(d => d.RejectedByEmpName, o => o.MapFrom(s => s.RejectedByEmpId != null ? s.RejectedByEmp.Person.FirstName + " " + s.RejectedByEmp.Person.LastName : ""))
             //.ForMember(d => d.LoanApplication.Guarantors, o => o.(s => s.LoanApplicationId != null && s.LoanApplication.Guarantors != null ? s.LoanApplication.Guarantors.Where(g=>g.Status == EntityStatus.Active) : null))
             ;
            CreateMap<ApplicationDetailDto, Application>()
                .ForMember(d => d.GroupAddress, o => o.Ignore())
                .ForMember(d => d.CIFList, o => o.Ignore())
                .ForMember(d => d.DocChecklist, o => o.Ignore());

            CreateMap<GPSLog, GPSLogDto>();
            CreateMap<GPSLogDto, GPSLog>();

            CreateMap<Guarantor, GuarantorDto>()
                  .ForMember(d => d.GuarantorName, o => o.MapFrom(s => s.GuarantorCif.Name))
                  .ForMember(d => d.Age, o => o.MapFrom(s => (s.GuarantorCif.DateOfBirth == null ? 0 : (DateTime.Now.Year - ((DateTime)s.GuarantorCif.DateOfBirth).Year))))
                  .ForMember(d => d.CIFNo, o => o.MapFrom(s => s.GuarantorCif.CIFNo))
                  .ForMember(d => d.CBSCIFNo, o => o.MapFrom(s => s.GuarantorCif.CBSCIFNo))
                  .ForMember(d => d.RelationshipWithApplicantName, o => o.MapFrom(s => s.RelationshipWithApplicant > 0 ? UiUtil.GetDisplayName(s.RelationshipWithApplicant) : ""));
            CreateMap<GuarantorDto, Guarantor>()
                .ForMember(d => d.GuarantorCif, o => o.Ignore());
            //.ForMember(d => d.LoanApplication, o => o.Ignore());

            CreateMap<LoanAppWaiverReq, LoanAppWaiverReqDto>();
            CreateMap<LoanAppWaiverReqDto, LoanAppWaiverReq>()
                .ForMember(d => d.WaiverRequestedTo, o => o.Ignore());
            //.ForMember(d => d.LoanApplication, o => o.Ignore());

            CreateMap<LoanAppColSecurity, LoanAppColSecurityDto>()
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.LoanAppSecurity.Product.Name))
                .ForMember(d => d.SecurityDescription, o => o.MapFrom(s => s.LoanAppSecurity.SecurityDescription));
            CreateMap<LoanAppColSecurityDto, LoanAppColSecurity>()
                .ForMember(d => d.LoanAppSecurity, o => o.Ignore());
            //.ForMember(d => d.LoanApplication, o => o.Ignore());
            //CreateMap<LoanAppColSecurity, LoanAppColSecurityDto>();
            //CreateMap<LoanAppColSecurityDto, LoanAppColSecurity>()
            //     .ForMember(d => d.LoanAppSecurity, o => o.Ignore())
            //     .ForMember(d => d.LoanApplication, o => o.Ignore());

            CreateMap<VehiclePrimarySecurity, VehiclePrimarySecurityDto>()
                .ForMember(d => d.VehicleStatusName, o => o.MapFrom(s => s.VehicleStatus > 0 ? UiUtil.GetDisplayName(s.VehicleStatus) : ""))
                .ForMember(d => d.VehicleTypeName, o => o.MapFrom(s => s.VehicleType > 0 ? UiUtil.GetDisplayName(s.VehicleType) : ""))
                .ForMember(d => d.VendorTypeName, o => o.MapFrom(s => s.VendorType > 0 ? UiUtil.GetDisplayName(s.VendorType) : ""))
                .ForMember(d => d.VendorName, o => o.MapFrom(s => s.Vendor.Name))
                 .ForMember(d => d.VendorDetail, o => o.MapFrom(s => ("Vendor Name:" + s.Vendor.Name + Environment.NewLine + "Vendor Type:" + (s.VendorType > 0 ? UiUtil.GetDisplayName(s.VendorType) : ""))))
                .ForMember(d => d.VehicleDetail, o => o.MapFrom(s => ("Registration no.:" + s.RegistrationNo + Environment.NewLine + "Registration year.:" + s.RegistrationYear + Environment.NewLine + "Manufacturer:" + s.Manufacturer + Environment.NewLine + "Manufacturing year:" + s.MnufacturingYear)));
            CreateMap<VehiclePrimarySecurityDto, VehiclePrimarySecurity>()
                 .ForMember(d => d.SellersAddress, o => o.Ignore())
                 .ForMember(d => d.LoanApplication, o => o.Ignore());


            CreateMap<ConsumerGoodsPrimarySecurity, ConsumerGoodsPrimarySecurityDto>()
                 .ForMember(d => d.VendorDetail, o => o.MapFrom(s => ("Vendor Name" + s.Showroom.Vendor.Name + Environment.NewLine + "Showroom Name" + s.Showroom.Name + Environment.NewLine + "Contact No:" + s.Showroom.Vendor.ContactPersonPhone)))// +"Showroom Address" + s.Showroom.Address.AddressLine1 + " " + s.Showroom.Address.AddressLine2 + " " + s.Showroom.Address.AddressLine3 + Environment.NewLine
                 .ForMember(d => d.ShowRoomName, o => o.MapFrom(s => s.Showroom.Name))
                 .ForMember(d => d.Dealer, o => o.MapFrom(s => s.Showroom.Vendor.Name));
            CreateMap<ConsumerGoodsPrimarySecurityDto, ConsumerGoodsPrimarySecurity>()
                 .ForMember(d => d.Showroom, o => o.Ignore())
                 .ForMember(d => d.LoanApplication, o => o.Ignore())
                 .ForMember(d => d.DealerAddress, o => o.Ignore());

            //FDRPrimarySecurityDto
            CreateMap<FDRPrimarySecurity, FDRPrimarySecurityDto>();
            CreateMap<FDRPrimarySecurityDto, FDRPrimarySecurity>()
                .ForMember(d => d.FDRPSDetails, o => o.Ignore());

            //FDRPSDetailDto
            CreateMap<FDRPSDetail, FDRPSDetailDto>()
                .ForMember(d => d.MaturityDateTxt, o => o.MapFrom(s => s.MaturityDate != null ? ((DateTime)s.MaturityDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : ""));
            CreateMap<FDRPSDetailDto, FDRPSDetail>()
                 .ForMember(d => d.FDRPrimarySecurity, o => o.Ignore());
            //LPPrimarySecurityDto
            CreateMap<LPPrimarySecurity, LPPrimarySecurityDto>()
                .ForMember(d => d.FirstDisbursementExpDateText, o => o.MapFrom(s => s.FirstDisbursementExpDate != null ? ((DateTime)s.FirstDisbursementExpDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : ""))
                .ForMember(d => d.DeveloperName, o => o.MapFrom(s => s.Developer.GroupName))
                .ForMember(d => d.ProjectName, o => o.MapFrom(s => s.Project.ProjectName))
                .ForMember(d => d.ContactPerson, o => o.MapFrom(s => s.Developer.ContactPerson))
                .ForMember(d => d.ContactPersonDesignation, o => o.MapFrom(s => s.Developer.ContactPersonDesignation))
                .ForMember(d => d.ContactPersonPhone, o => o.MapFrom(s => s.Developer.ContactPersonPhone))
                .ForMember(d => d.LandedPropertyLoanTypeName, o => o.MapFrom(s => s.LandedPropertyLoanType != null ? UiUtil.GetDisplayName(s.LandedPropertyLoanType) : ""))
                .ForMember(d => d.LandTypeName, o => o.MapFrom(s => s.LandType != null ? UiUtil.GetDisplayName(s.LandType) : ""))
                .ForMember(d => d.Valuations, o => o.MapFrom(s => s.Valuations != null ? s.Valuations.Where(v => v.Status == EntityStatus.Active) : null));
            CreateMap<LPPrimarySecurityDto, LPPrimarySecurity>()
                 .ForMember(d => d.LoanApplication, o => o.Ignore())
                 .ForMember(d => d.PropertyAddress, o => o.Ignore())
                 .ForMember(d => d.PropertyAddress, o => o.Ignore())
                 .ForMember(d => d.Valuations, o => o.Ignore());


            CreateMap<IPDCBankAccounts, IPDCBankAccountsDto>();
            CreateMap<IPDCBankAccountsDto, IPDCBankAccounts>();

            CreateMap<DepositApplication, DepositApplicationDto>()
                .ForMember(d => d.RelationshipWithApplicantName, o => o.MapFrom(s => s.RelationshipWithApplicant > 0 ? UiUtil.GetDisplayName(s.RelationshipWithApplicant) : ""))
                .ForMember(d => d.ChequeDeposits, o => o.MapFrom(s => s.ChequeDeposits.Where(c => c.Status == EntityStatus.Active)))
                .ForMember(d => d.CashDeposits, o => o.MapFrom(s => s.CashDeposits.Where(c => c.Status == EntityStatus.Active)))
                .ForMember(d => d.TransferDeposits, o => o.MapFrom(s => s.TransferDeposits.Where(c => c.Status == EntityStatus.Active)))
                .ForMember(d => d.Nominees, o => o.MapFrom(s => s.Nominees.Where(n => n.Status == EntityStatus.Active)));
            CreateMap<DepositApplicationDto, DepositApplication>()
                .ForMember(d => d.ChequeDeposits, o => o.Ignore())
                .ForMember(d => d.TransferDeposits, o => o.Ignore())
                .ForMember(d => d.CashDeposits, o => o.Ignore())
                .ForMember(d => d.Nominees, o => o.Ignore())
                .ForMember(d => d.GuiardianCif, o => o.Ignore());

            CreateMap<DepAppChequeDeposit, DepAppChequeDepositDto>()
                .ForMember(d => d.DepositAccntName, o => o.MapFrom(s => s.IPDCBankAccount != null ? (s.IPDCBankAccount.AccountNo + "-" + s.IPDCBankAccount.BankName + "-" + s.IPDCBankAccount.BranchName) : ""));

            CreateMap<DepAppChequeDepositDto, DepAppChequeDeposit>();

            CreateMap<DepAppTransfer, DepAppTransferDto>()
            .ForMember(d => d.DepositAccntName, o => o.MapFrom(s => s.IPDCBankAccount != null ? (s.IPDCBankAccount.AccountNo + "-" + s.IPDCBankAccount.BankName + "-" + s.IPDCBankAccount.BranchName) : ""));
            CreateMap<DepAppTransferDto, DepAppTransfer>();

            CreateMap<DepAppCash, DepAppCashDto>()
                .ForMember(d => d.DepositAccntName, o => o.MapFrom(s => s.IPDCBankAccount != null ? (s.IPDCBankAccount.AccountNo + "-" + s.IPDCBankAccount.BankName + "-" + s.IPDCBankAccount.BranchName) : ""));

            CreateMap<DepAppCashDto, DepAppCash>();

            CreateMap<DepositNominee, DepositNomineeDto>()
                .ForMember(d => d.RelationshipWithApplicantName, o => o.MapFrom(s => s.RelationshipWithApplicant > 0 ? UiUtil.GetDisplayName(s.RelationshipWithApplicant) : ""));
            CreateMap<DepositNomineeDto, DepositNominee>()
                .ForMember(d => d.NomineeCif, o => o.Ignore())
                .ForMember(d => d.GuiardianCif, o => o.Ignore());

            CreateMap<BankAccount, BankAccountDto>();
            CreateMap<BankAccountDto, BankAccount>();

            CreateMap<CIF_Organizational, CIF_OrganizationalDto>()
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.IsEnlistedCompany == true && s.CompanyId != null ? s.Company.Name : s.CompanyName))
                .ForMember(d => d.TradeLicenceDateTxt, o => o.MapFrom(s => s.TradeLicenceDate != null ? ((DateTime)s.TradeLicenceDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : ""))
                .ForMember(d => d.RegistrationDateTxt, o => o.MapFrom(s => s.RegistrationDate != null ? ((DateTime)s.RegistrationDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : ""))
                .ForMember(d => d.BusinessTypeName, o => o.MapFrom(s => s.BusinessType != null ? UiUtil.GetDisplayName(s.BusinessType) : ""))
                .ForMember(d => d.BusinessSizeName, o => o.MapFrom(s => s.BusinessSize != null ? UiUtil.GetDisplayName(s.BusinessSize) : ""))
                .ForMember(d => d.SectorTypeName, o => o.MapFrom(s => s.SectorType != null ? UiUtil.GetDisplayName(s.SectorType) : ""))
                .ForMember(d => d.RegAddressTxt, o => o.MapFrom(s => s.RegAddressId != null ?
                            (!string.IsNullOrEmpty(s.RegAddress.AddressLine1) ? ("Address Line1 : " + s.RegAddress.AddressLine1) : "")
                          + (!string.IsNullOrEmpty(s.RegAddress.AddressLine2) ? (", Address Line2 : " + s.RegAddress.AddressLine2) : "")
                          + (!string.IsNullOrEmpty(s.RegAddress.AddressLine3) ? (", Address Line2 : " + s.RegAddress.AddressLine3) : "")
                          + (s.RegAddress.ThanaId != null ? (", Thana : " + s.RegAddress.Thana.ThanaNameEng) : "")
                          + (s.RegAddress.DistrictId != null ? (", District : " + s.RegAddress.District.DistrictNameEng) : "")
                          + (s.RegAddress.DivisionId != null ? (", Division : " + s.RegAddress.Division.DivisionNameEng) : "")
                          + (s.RegAddress.CountryId != null ? (", Country : " + s.RegAddress.Country.Name) : "")
                          + (s.RegAddress.CellPhoneNo != null ? (", Mobile : " + s.RegAddress.PhoneNo) : "")
                          + (s.RegAddress.PhoneNo != null ? (", Phone No : " + s.RegAddress.PhoneNo) : "")
                          + (s.RegAddress.CellPhoneNo != null ? (", Mobile : " + s.RegAddress.PhoneNo) : "") : ""))
                .ForMember(d => d.OfficeAddressTxt, o => o.MapFrom(s => s.OfficeAddressId != null ?
                            (!string.IsNullOrEmpty(s.OfficeAddress.AddressLine1) ? s.OfficeAddress.AddressLine1 : "")
                          + (!string.IsNullOrEmpty(s.OfficeAddress.AddressLine2) ? (", " + s.OfficeAddress.AddressLine2) : "")
                          + (!string.IsNullOrEmpty(s.OfficeAddress.AddressLine3) ? (", " + s.OfficeAddress.AddressLine3) : "")
                          + (s.OfficeAddress.ThanaId != null ? (", " + s.OfficeAddress.Thana.ThanaNameEng) : "")
                          + (s.OfficeAddress.DistrictId != null ? (", " + s.OfficeAddress.District.DistrictNameEng) : "")
                          + (s.OfficeAddress.DivisionId != null ? (", " + s.OfficeAddress.Division.DivisionNameEng) : "")
                          + (s.OfficeAddress.CountryId != null ? (", " + s.OfficeAddress.Country.Name) : "") : ""))
                .ForMember(d => d.LegalStatusName, o => o.MapFrom(s => s.LegalStatus != null ? UiUtil.GetDisplayName(s.LegalStatus) : ""));
            CreateMap<CIF_OrganizationalDto, CIF_Organizational>()
                .ForMember(d => d.OfficeAddress, o => o.Ignore())
                .ForMember(d => d.RegAddress, o => o.Ignore())
                .ForMember(d => d.Owners, o => o.Ignore())
                .ForMember(d => d.FactoryAddress, o => o.Ignore())
                .ForMember(d => d.CIBs, o => o.Ignore());

            CreateMap<SectorCode, SectorCodeDto>();
            CreateMap<SectorCodeDto, SectorCode>();

            CreateMap<FactoryAddress, FactoryAddressDto>()
                .ForMember(d => d.AddressTxt, o => o.MapFrom(s => s.AddressId != null ?
                            (!string.IsNullOrEmpty(s.Address.AddressLine1) ? s.Address.AddressLine1 : "")
                          + (!string.IsNullOrEmpty(s.Address.AddressLine2) ? (", " + s.Address.AddressLine2) : "")
                          + (!string.IsNullOrEmpty(s.Address.AddressLine3) ? (", " + s.Address.AddressLine3) : "")
                          + (s.Address.ThanaId != null ? (", " + s.Address.Thana.ThanaNameEng) : "")
                          + (s.Address.DistrictId != null ? (", " + s.Address.District.DistrictNameEng) : "")
                          + (s.Address.DivisionId != null ? (", " + s.Address.Division.DivisionNameEng) : "")
                          + (s.Address.CountryId != null ? (", " + s.Address.Country.Name) : "") : ""
                    ));

            CreateMap<FactoryAddressDto, FactoryAddress>()
                .ForMember(d => d.Address, o => o.Ignore());

            CreateMap<CIF_Org_Owners, CIF_Org_OwnersDto>();
            CreateMap<CIF_Org_OwnersDto, CIF_Org_Owners>()
                .ForMember(d => d.CIF_Personal, o => o.Ignore());
            CreateMap<VendorShowrooms, VendorShowroomsDto>();
            CreateMap<VendorShowroomsDto, VendorShowrooms>();

            CreateMap<Project, ProjectDto>();
            CreateMap<ProjectDto, Project>();

            CreateMap<Developer, DeveloperDto>();
            CreateMap<DeveloperDto, Developer>();

            CreateMap<LoanApplication, LoanApplicationDto>()
                 .ForMember(d => d.VehiclePrimarySecurity, o => o.Ignore())
                 .ForMember(d => d.ConsumerGoodsPrimarySecurity, o => o.Ignore())
                 .ForMember(d => d.LPPrimarySecurity, o => o.Ignore())
                 .ForMember(d => d.FDRPrimarySecurity, o => o.Ignore());
            CreateMap<LoanApplicationDto, LoanApplication>()
                .ForMember(d => d.LoanAppColSecurities, o => o.Ignore())
                .ForMember(d => d.OtherSecurities, o => o.Ignore())
                .ForMember(d => d.WaiverRequests, o => o.Ignore())
                .ForMember(d => d.Guarantors, o => o.Ignore());

            CreateMap<LoanOtherSecurities, LoanOtherSecuritiesDto>();
            CreateMap<LoanOtherSecuritiesDto, LoanOtherSecurities>();

            CreateMap<Call, CallDto>();
            CreateMap<CallDto, Call>()
                .ForMember(d => d.CustomerAddress, o => o.Ignore());


            //LPPrimarySecurityValuationDto

            CreateMap<LPPrimarySecurityValuation, LPPrimarySecurityValuationDto>()
             .ForMember(d => d.PropertyTypeName, o => o.MapFrom(s => s.LPPrimarySecurity.LandedPropertyLoanType != null ? UiUtil.GetDisplayName(s.LPPrimarySecurity.LandedPropertyLoanType) : "")) //LandedPropertyLoanType
             .ForMember(d => d.FlatSize, o => o.MapFrom(s => s.LPPrimarySecurity.FlatSize))
             .ForMember(d => d.DeveloperId, o => o.MapFrom(s => s.LPPrimarySecurity.Project != null ? s.LPPrimarySecurity.Project.DeveloperId : 0))
             .ForMember(d => d.DeveloperName, o => o.MapFrom(s => s.LPPrimarySecurity.Project != null ? s.LPPrimarySecurity.Project.Developer != null ? s.LPPrimarySecurity.Project.Developer.GroupName : null : ""));
            CreateMap<LPPrimarySecurityValuationDto, LPPrimarySecurityValuation>();
            //VehiclePrimarySecurityValuationDto

            CreateMap<VehiclePrimarySecurityValuation, VehiclePrimarySecurityValuationDto>()
                 .ForMember(d => d.VerificationDateText, o => o.MapFrom(s => s.VerificationDate != null ? ((DateTime)s.VerificationDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : ""));
            CreateMap<VehiclePrimarySecurityValuationDto, VehiclePrimarySecurityValuation>()
                .ForMember(d => d.VehiclePrimarySecurity, o => o.Ignore());

            CreateMap<ConsumerGoodsPrimarySecurityValuation, ConsumerGoodsPrimarySecurityValuationDto>()
               .ForMember(d => d.VerificationDateText, o => o.MapFrom(s => s.VerificationDate != null ? ((DateTime)s.VerificationDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : ""));
            CreateMap<ConsumerGoodsPrimarySecurityValuationDto, ConsumerGoodsPrimarySecurityValuation>()
                .ForMember(d => d.ConsumerGoodsPrimarySecurity, o => o.Ignore());

            CreateMap<ContactPointVerification, ContactPointVerificationDto>()
                .ForMember(d => d.VerificationDateText, o => o.MapFrom(s => s.VerificationDate != null ? (((DateTime)s.VerificationDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)) : ""))
               .ForMember(d => d.VerificationPersonRoleName, o => o.MapFrom(s => UiUtil.GetDisplayName(s.VerificationPersonRole)))
               .ForMember(d => d.ApplicationNo, o => o.MapFrom(s => s.Application.ApplicationNo));
            CreateMap<ContactPointVerificationDto, ContactPointVerification>()
                .ForMember(d => d.BankAccounts, o => o.Ignore())
                .ForMember(d => d.References, o => o.Ignore());


            CreateMap<CIB_Personal, CIB_PersonalDto>()
            .ForMember(d => d.VerificationDateTxt, o => o.MapFrom(s => s.VerificationDate != null ? ((DateTime)s.VerificationDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : ""))
            .ForMember(d => d.CIBReportFileName, o => o.MapFrom(s => s.CIBReport != null ? Path.GetFileName(s.CIBReport) : ""));
            CreateMap<CIB_PersonalDto, CIB_Personal>();

            CreateMap<BankAccountCpv, BankAccountCpvDto>();
            CreateMap<BankAccountCpvDto, BankAccountCpv>()
                .ForMember(d => d.Id, o => o.Ignore());

            CreateMap<ReferenceCpv, ReferenceCpvDto>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.CifReference.Name));
            CreateMap<ReferenceCpvDto, ReferenceCpv>()
                .ForMember(d => d.Id, o => o.Ignore());
            //IncomeVerificationDto
            CreateMap<IncomeVerification, IncomeVerificationDto>()
                  .ForMember(d => d.IncomeAssessmentDateTxt, o => o.MapFrom(s => s.IncomeAssessmentDate != null ? (((DateTime)s.IncomeAssessmentDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)) : ""))
                  .ForMember(d => d.VerificationPersonRoleName, o => o.MapFrom(s => s.VerificationPersonRole > 0 ? UiUtil.GetDisplayName(s.VerificationPersonRole) : ""))
                  .ForMember(d => d.VerificationStateName, o => o.MapFrom(s => UiUtil.GetDisplayName(s.VerificationState)))
                  .ForMember(d => d.CifNo, o => o.MapFrom(s => s.CfPersonal.CIFNo))
                  .ForMember(d => d.CifName, o => o.MapFrom(s => s.CfPersonal.Name))
                  .ForMember(d => d.ApplicationNo, o => o.MapFrom(s => s.Application.ApplicationNo));
            CreateMap<IncomeVerificationDto, IncomeVerification>()
                .ForMember(d => d.IncomeStatement, o => o.Ignore())
                .ForMember(d => d.CfPersonal, o => o.Ignore())
                .ForMember(d => d.MonthlyOtherIncomesAssessed, o => o.Ignore());

            CreateMap<NIDVerification, NIDVerificationDto>()
                .ForMember(d => d.VerificationDateText, o => o.MapFrom(s => s.VerificationDate != null ? (((DateTime)s.VerificationDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)) : ""))
                .ForMember(d => d.DateOfBirthText, o => o.MapFrom(s => s.DateOfBirth != null ? (((DateTime)s.DateOfBirth).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)) : ""))
                .ForMember(d => d.VerificationPersonRoleName, o => o.MapFrom(s => UiUtil.GetDisplayName(s.VerificationPersonRole)))
                .ForMember(d => d.VerificationStatusName, o => o.MapFrom(s => UiUtil.GetDisplayName(s.VerificationStatus)))
                .ForMember(d => d.FindingName, o => o.MapFrom(s => UiUtil.GetDisplayName(s.Finding)))
                .ForMember(d => d.ApplicationNo, o => o.MapFrom(s => s.Application.ApplicationNo));
            CreateMap<NIDVerificationDto, NIDVerification>()
                .ForMember(d => d.Application, o => o.Ignore())
                .ForMember(d => d.CIF, o => o.Ignore());

            CreateMap<ProjectTechnicalVerification, ProjectTechnicalVerificationDto>()
                   //.ForMember(d => d.AreaOfLandPPUOM, o => o.Ignore())
                   .ForMember(d => d.ProjectTechnicalVerificationFileName, o => o.MapFrom(s => s.ProjectTechnicalVerificationPath != null ? Path.GetFileName(s.ProjectTechnicalVerificationPath) : ""));
            CreateMap<ProjectTechnicalVerificationDto, ProjectTechnicalVerification>()
                .ForMember(d => d.Project, o => o.Ignore())
                .ForMember(d => d.AreaOfLandPPUOM, o => o.Ignore());

            CreateMap<ProjectLegalVerification, ProjectLegalVerificationDto>()
                .ForMember(d => d.VerificationReportFileName, o => o.MapFrom(s => s.VerificationReportPath != null ? Path.GetFileName(s.VerificationReportPath) : ""))
                .ForMember(d => d.VettingReportFileName, o => o.MapFrom(s => s.VettingReportPath != null ? Path.GetFileName(s.VettingReportPath) : ""));
            CreateMap<ProjectLegalVerificationDto, ProjectLegalVerification>()
                .ForMember(d => d.Project, o => o.Ignore())
                .ForMember(d => d.AreaOfLandTDUOM, o => o.Ignore())
                .ForMember(d => d.Owners, o => o.Ignore());

            CreateMap<ProjectPropertyOwner, ProjectPropertyOwnerDto>();
            CreateMap<ProjectPropertyOwnerDto, ProjectPropertyOwner>();


            CreateMap<CIB_Organizational, CIB_OrganizationalDto>()
            .ForMember(d => d.VerificationDateTxt, o => o.MapFrom(s => s.VerificationDate != null ? ((DateTime)s.VerificationDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : ""));
            CreateMap<CIB_OrganizationalDto, CIB_Organizational>();

            CreateMap<NetWorthVerification, NetWorthVerificationDto>()
            .ForMember(d => d.VerificationDateTxt, o => o.MapFrom(s => s.VerificationDate != null ? ((DateTime)s.VerificationDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : ""))
            .ForMember(d => d.SavingsInBank, o => o.MapFrom(s => s.SavingsInBank.Where(c => c.Status == EntityStatus.Active)))
            .ForMember(d => d.BusinessShares, o => o.MapFrom(s => s.BusinessShares.Where(c => c.Status == EntityStatus.Active)))
            .ForMember(d => d.Investments, o => o.MapFrom(s => s.Investments.Where(c => c.Status == EntityStatus.Active)))
            .ForMember(d => d.Liabilities, o => o.MapFrom(s => s.Liabilities.Where(c => c.Status == EntityStatus.Active)))
            .ForMember(d => d.Properties, o => o.MapFrom(s => s.Properties.Where(c => c.Status == EntityStatus.Active)));

            CreateMap<NetWorthVerificationDto, NetWorthVerification>()
                .ForMember(d => d.CIF_NetWorth, o => o.Ignore())
                .ForMember(d => d.SavingsInBank, o => o.Ignore())
                .ForMember(d => d.BusinessShares, o => o.Ignore())
                .ForMember(d => d.Investments, o => o.Ignore())
                .ForMember(d => d.Liabilities, o => o.Ignore())
                .ForMember(d => d.Properties, o => o.Ignore());

            CreateMap<NWV_SavingsInBank, NWV_SavingsInBankDto>();
            CreateMap<NWV_SavingsInBankDto, NWV_SavingsInBank>();

            CreateMap<NWV_Investment, NWV_InvestmentDto>();
            CreateMap<NWV_InvestmentDto, NWV_Investment>();

            CreateMap<NWV_Property, NWV_PropertyDto>();
            CreateMap<NWV_PropertyDto, NWV_Property>();

            CreateMap<NWV_BusinessShares, NWV_BusinessSharesDto>();
            CreateMap<NWV_BusinessSharesDto, NWV_BusinessShares>();

            CreateMap<NWV_Liability, NWV_LiabilityDto>();
            CreateMap<NWV_LiabilityDto, NWV_Liability>();

            //CreateMap<NetWorthVerification, CIF_NetWorth>();
            //CreateMap<CIF_NetWorth, NetWorthVerification>();
            CreateMap<CIF_NetWorthDto, NetWorthVerificationDto>()
                .ForMember(d => d.NetWorthId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.SavingsInBank, o => o.MapFrom(s => s.SavingsInBank.Where(n => n.Status == EntityStatus.Active)))
                .ForMember(d => d.Investments, o => o.MapFrom(s => s.Investments.Where(n => n.Status == EntityStatus.Active)))
                .ForMember(d => d.Properties, o => o.MapFrom(s => s.Properties.Where(n => n.Status == EntityStatus.Active)))
                .ForMember(d => d.BusinessShares, o => o.MapFrom(s => s.BusinessShares.Where(n => n.Status == EntityStatus.Active)))
                .ForMember(d => d.Liabilities, o => o.MapFrom(s => s.Liabilities.Where(n => n.Status == EntityStatus.Active)))
                .ForMember(d => d.Id, o => o.Ignore());
            CreateMap<CIF_SavingsInBankDto, NWV_SavingsInBankDto>()
                .ForMember(d => d.Id, o => o.Ignore());
            CreateMap<CIF_InvestmentDto, NWV_InvestmentDto>()
                .ForMember(d => d.Id, o => o.Ignore());
            CreateMap<CIF_NW_PropertyDto, NWV_PropertyDto>()
                .ForMember(d => d.Id, o => o.Ignore());
            CreateMap<CIF_BusinessSharesDto, NWV_BusinessSharesDto>()
                .ForMember(d => d.Id, o => o.Ignore());
            CreateMap<CIF_LiabilityDto, NWV_LiabilityDto>()
                .ForMember(d => d.Id, o => o.Ignore());

            //CIF_Org_OwnersDto
            //AppDocChecklistDto VisitReport
            CreateMap<CIF_Org_Owners, CIF_Org_OwnersDto>()
                .ForMember(d => d.ProfessionName, o => o.MapFrom(s => s.CIF_Personal.Occupation.Profession.Name))
                .ForMember(d => d.CIF_Org_OwnersRoleName, o => o.MapFrom(s => UiUtil.GetDisplayName(s.CIF_Org_OwnersRole)))
                .ForMember(d => d.CIFNo, o => o.MapFrom(s => s.CIF_Personal.CIFNo))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.CIF_Personal.Name))
                .ForMember(d => d.Age, o => o.MapFrom(s => (s.CIF_Personal.DateOfBirth == null ? 0 : (DateTime.Now.Year - ((DateTime)s.CIF_Personal.DateOfBirth).Year))));
            CreateMap<CIF_Org_OwnersDto, CIF_Org_Owners>()
                .ForMember(d => d.CIF_Personal, o => o.Ignore())
                .ForMember(d => d.CIF_Organizational, o => o.Ignore());

            CreateMap<VisitReport, VisitReportDto>()
                .ForMember(d => d.VisitTimeText, o => o.MapFrom(s => s.VisitTime != null ? ((DateTime)s.VisitTime).ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture) : ""))
                .ForMember(d => d.ApplicationNo, o => o.MapFrom(s => s.Application.ApplicationNo))
                .ForMember(d => d.VerificationPersonRoleName, o => o.MapFrom(s => UiUtil.GetDisplayName(s.VerificationPersonRole)))
                .ForMember(d => d.VisitReportFileName, o => o.MapFrom(s => s.VisitReportPath != null ? Path.GetFileName(s.VisitReportPath) : "")); ;
            CreateMap<VisitReportDto, VisitReport>();
            //CIF_AdditionalIncomeDeclared
            CreateMap<CIF_AdditionalIncomeDeclared, CIF_AdditionalIncomeDeclaredDto>();
            CreateMap<CIF_AdditionalIncomeDeclaredDto, CIF_AdditionalIncomeDeclared>();

            CreateMap<IPDCMessaging, IPDCMessagingDto>()
                .ForMember(d => d.FromEmpName,
                    o => o.MapFrom(s => s.FromEmployee.Person.FirstName + " " + s.FromEmployee.Person.LastName));
            CreateMap<IPDCMessagingDto, IPDCMessaging>();

            CreateMap<IPDCMessageListDto, IPDCMessagingDto>();
            //CreateMap<IPDCMessagingDto, IPDCMessaging>();

            CreateMap<IncomeVerificationAdditionalIncomeAssessed, IncomeVerificationAdditionalIncomeAssessedDto>();
            CreateMap<IncomeVerificationAdditionalIncomeAssessedDto, IncomeVerificationAdditionalIncomeAssessed>()
                .ForMember(d => d.AdditionalIncomeDeclared, o => o.Ignore());
            //ProposalDto
            CreateMap<Proposal, ProposalDto>()
                .ForMember(d => d.AppliedLoanTermApplication, o => o.MapFrom(s => s.Application.LoanApplicationId > 0 ? s.Application.LoanApplication.Term : (s.Application.DepositApplicationId > 0 ? s.Application.DepositApplication.Term : 0)))
                //
                .ForMember(d => d.ProjectAddressString, o => o.MapFrom(s => s.ProjectAddressId != null ?
                    s.ProjectAddress.AddressLine1
                    + ", " + s.ProjectAddress.AddressLine2
                        + ", " + s.ProjectAddress.AddressLine3
                        + ", " + s.ProjectAddress.Thana.ThanaNameEng
                        + ", " + s.ProjectAddress.District.DistrictNameEng
                        + ", " + s.ProjectAddress.Division.DivisionNameEng
                        + ", " + s.ProjectAddress.Country.Name : ""
                  ))
                  .ForMember(d => d.ExpiryDateTxt, o => o.MapFrom(s => s.ExpiryDate != null ? ((DateTime)s.ExpiryDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : ""))
                  .ForMember(d => d.FacilityTypeName, o => o.MapFrom(s => UiUtil.GetDisplayName(s.FacilityType)))
                  .ForMember(d => d.PropertyTypeName, o => o.MapFrom(s => UiUtil.GetDisplayName(s.PropertyType)))
                  .ForMember(d => d.ApprovalAuthorityLevelName, o => o.MapFrom(s => s.ApprovalAuthorityLevel > 0 ? UiUtil.GetDisplayName(s.ApprovalAuthorityLevel) : ""))
                  .ForMember(d => d.DeveloperCategoryName, o => o.MapFrom(s => s.DeveloperCategory > 0 ? UiUtil.GetDisplayName(s.DeveloperCategory) : ""))
                  .ForMember(d => d.PropertyOwnershipTypeName, o => o.MapFrom(s => s.PropertyOwnershipType > 0 ? UiUtil.GetDisplayName(s.PropertyOwnershipType) : ""))
                  .ForMember(d => d.Guarantors, o => o.MapFrom(s => s.Guarantors.Where(g => g.Status == EntityStatus.Active)));

            //.ForMember(d => d.ApplicationReceiveDate, o => o.MapFrom(s => s.Application.ApplicationDate))
            //.ForMember(d => d.BranchName, o => o.MapFrom(s => s.Application.BranchOffice.Name))
            //.ForMember(d => d.AppliedLoanAmount, o => o.MapFrom(s => s.Application.LoanApplication.LoanAmountApplied));
            CreateMap<ProposalDto, Proposal>()
                .ForMember(d => d.Application, o => o.Ignore())
                .ForMember(d => d.OtherCosts, o => o.Ignore())
                .ForMember(d => d.ClientProfiles, o => o.Ignore())
                .ForMember(d => d.Guarantors, o => o.Ignore())
                .ForMember(d => d.StressRates, o => o.Ignore())
                .ForMember(d => d.OverallAssessments, o => o.Ignore())
                .ForMember(d => d.FDRs, o => o.Ignore())
                .ForMember(d => d.Texts, o => o.Ignore())
                .ForMember(d => d.Liabilities, o => o.Ignore())
                .ForMember(d => d.CIBs, o => o.Ignore())
                .ForMember(d => d.NetWorths, o => o.Ignore())
                .ForMember(d => d.Incomes, o => o.Ignore())
                .ForMember(d => d.Signatories, o => o.Ignore())
                .ForMember(d => d.SecurityDetails, o => o.Ignore())
                .ForMember(d => d.ProposalCreditCards, o => o.Ignore())
                .ForMember(d => d.ValuationOtherCosts, o => o.Ignore())
                .ForMember(d => d.ProjectAddress, o => o.Ignore());

            CreateMap<Proposal_ClientProfile, Proposal_ClientProfileDto>()
                .ForMember(d => d.ResidenceStatusName, o => o.MapFrom(s => s.ResidenceStatus > 0 ? UiUtil.GetDisplayName(s.ResidenceStatus) : ""))
                .ForMember(d => d.ApplicantRoleName, o => o.MapFrom(s => UiUtil.GetDisplayName(s.ApplicantRole)))
                .ForMember(d => d.RelationshipWithApplicantName, o => o.MapFrom(s => s.RelationshipWithApplicant > 0 ? UiUtil.GetDisplayName(s.RelationshipWithApplicant) : ""))
                .ForMember(d => d.OfficeAddressString, o => o.MapFrom(s => s.OfficeAddressId != null ?
                    s.OfficeAddress.AddressLine1
                    + ", " + s.OfficeAddress.AddressLine2
                    + ", " + s.OfficeAddress.AddressLine3
                    + ", " + s.OfficeAddress.Thana.ThanaNameEng
                    + ", " + s.OfficeAddress.District.DistrictNameEng
                    + ", " + s.OfficeAddress.Division.DivisionNameEng
                    + ", " + s.OfficeAddress.Country.Name : ""
                  ))
                .ForMember(d => d.PermanentAddressString, o => o.MapFrom(s => s.PermanentAddressId != null ?
                    s.PermanentAddress.AddressLine1
                    + ", " + s.PermanentAddress.AddressLine2
                    + ", " + s.PermanentAddress.AddressLine3
                    + ", " + s.PermanentAddress.Thana.ThanaNameEng
                    + ", " + s.PermanentAddress.District.DistrictNameEng
                    + ", " + s.PermanentAddress.Division.DivisionNameEng
                    + ", " + s.PermanentAddress.Country.Name : ""
                  ))
                  .ForMember(d => d.PresentAddressString, o => o.MapFrom(s => s.PresentAddressId != null ?
                    s.PresentAddress.AddressLine1
                    + ", " + s.PresentAddress.AddressLine2
                    + ", " + s.PresentAddress.AddressLine3
                    + ", " + s.PresentAddress.Thana.ThanaNameEng
                    + ", " + s.PresentAddress.District.DistrictNameEng
                    + ", " + s.PresentAddress.Division.DivisionNameEng
                    + ", " + s.PresentAddress.Country.Name : ""
                  ));
            CreateMap<Proposal_ClientProfileDto, Proposal_ClientProfile>()
                .ForMember(d => d.CIFP, o => o.Ignore())
                .ForMember(d => d.CIFO, o => o.Ignore());

            CreateMap<Proposal_CIB, Proposal_CIBDto>()
                .ForMember(d => d.ClientRoleName, o => o.MapFrom(s => s.ClientRole > 0 ? UiUtil.GetDisplayName(s.ClientRole) : ""))
                .ForMember(d => d.CIBStatusName, o => o.MapFrom(s => UiUtil.GetDisplayName(s.CIBStatus)));
            CreateMap<Proposal_CIBDto, Proposal_CIB>();

            CreateMap<Proposal_FDR, Proposal_FDRDto>();
            CreateMap<Proposal_FDRDto, Proposal_FDR>();

            CreateMap<Proposal_Guarantor, Proposal_GuarantorDto>()
                .ForMember(d => d.RelationshipWithApplicantName, o => o.MapFrom(s => s.RelationshipWithApplicant > 0 ? UiUtil.GetDisplayName(s.RelationshipWithApplicant) : ""))
                ;
            //.ForMember(d => d.OccupationTypeName,
            //    o =>
            //        o.MapFrom(
            //            s =>
            //                s.CIF_Personal.Occupation.OccupationType > 0
            //                    ? UiUtil.GetDisplayName(s.CIF_Personal.Occupation.OccupationType)
            //                    : ""));
            CreateMap<Proposal_GuarantorDto, Proposal_Guarantor>()
                .ForMember(d => d.GuarantorCIF, o => o.Ignore());

            CreateMap<Proposal_Income, Proposal_IncomeDto>()
                .ForMember(d => d.ApplicantRoleName, o => o.MapFrom(s => UiUtil.GetDisplayName(s.ApplicantRole)));
            CreateMap<Proposal_IncomeDto, Proposal_Income>();

            CreateMap<Proposal_Liability, Proposal_LiabilityDto>();
            CreateMap<Proposal_LiabilityDto, Proposal_Liability>();

            CreateMap<Proposal_NetWorth, Proposal_NetWorthDto>()
                .ForMember(d => d.ClientRoleName, o => o.MapFrom(s => s.ClientRole != null ? UiUtil.GetDisplayName(s.ClientRole) : ""));
            CreateMap<Proposal_NetWorthDto, Proposal_NetWorth>();

            CreateMap<Proposal_OtherCost, Proposal_OtherCostDto>();
            CreateMap<Proposal_OtherCostDto, Proposal_OtherCost>();

            CreateMap<Proposal_OverallAssessment, Proposal_OverallAssessmentDto>()
                  .ForMember(d => d.VerificationStatusName, o => o.MapFrom(s => s.VerificationStatus > 0 ? UiUtil.GetDisplayName(s.VerificationStatus) : ""));
            CreateMap<Proposal_OverallAssessmentDto, Proposal_OverallAssessment>();

            CreateMap<Proposal_SecurityDetail, Proposal_SecurityDetailDto>();
            CreateMap<Proposal_SecurityDetailDto, Proposal_SecurityDetail>();

            CreateMap<Proposal_StressRate, Proposal_StressRateDto>();
            CreateMap<Proposal_StressRateDto, Proposal_StressRate>();

            CreateMap<Proposal_Text, Proposal_TextDto>();
            CreateMap<Proposal_TextDto, Proposal_Text>();

            CreateMap<Proposal_Signatory, Proposal_SignatoryDto>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Signatory != null ? s.Signatory.Name : ""));
            CreateMap<Proposal_SignatoryDto, Proposal_Signatory>();

            CreateMap<OfferLetter, OfferLetterDto>()
                .ForMember(d => d.OfferLetterDateTxt, o => o.MapFrom(s => s.OfferLetterDate != null ? ((DateTime)s.OfferLetterDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : ""))
                .ForMember(d => d.ApplicationDate, o => o.MapFrom(s => s.Proposal.Application.ApplicationDate))
                .ForMember(d => d.ApplicationNo, o => o.MapFrom(s => s.Proposal.ApplicationNo))
                .ForMember(d => d.ProposedLoanAmount, o => o.MapFrom(s => s.Proposal.RecomendedLoanAmountFromIPDC))
                .ForMember(d => d.VehicleName, o => o.MapFrom(s => s.Proposal.Vehicle_Name))
                .ForMember(d => d.YearModel, o => o.MapFrom(s => s.Proposal.Vehicle_ModelYear))
                .ForMember(d => d.CarPrice, o => o.MapFrom(s => s.Proposal.Vehicle_QuotedPrice))
                .ForMember(d => d.BorrowersContribution, o => o.MapFrom(s => s.Proposal.RecomendedLoanAmountFromIPDC - s.Proposal.PresentMarketValue != null ? s.Proposal.PresentMarketValue : 0))
                .ForMember(d => d.Term, o => o.MapFrom(s => s.Proposal.AppliedLoanTerm))
                .ForMember(d => d.InterestRate, o => o.MapFrom(s => s.Proposal.InterestRateOffered))
                .ForMember(d => d.EMI, o => o.MapFrom(s => s.Proposal.EMIofProposedLoan))
                .ForMember(d => d.ProcessingFeeRate, o => o.MapFrom(s => s.Proposal.ProcessingFeeAndDocChargesCardRate))
                .ForMember(d => d.ProcessingFeeAmount, o => o.MapFrom(s => s.Proposal.ProcessingFeeAndDocChargesAmount))
                .ForMember(d => d.ProcessingFeeAndDocChargesPercentage, o => o.MapFrom(s => s.Proposal.ProcessingFeeAndDocChargesPercentage))
                .ForMember(d => d.PDCBankName, o => o.MapFrom(s => s.Proposal.PDCBankName))
                .ForMember(d => d.PDCBankBranch, o => o.MapFrom(s => s.Proposal.PDCBankBranch))
                .ForMember(d => d.PDCAccountNo, o => o.MapFrom(s => s.Proposal.PDCAccountNo))
                .ForMember(d => d.PDCRoutingNo, o => o.MapFrom(s => s.Proposal.PDCRoutingNo))
                .ForMember(d => d.PDCAccountTitle, o => o.MapFrom(s => s.Proposal.PDCAccountTitle))
                .ForMember(d => d.PDCAccountType, o => o.MapFrom(s => s.Proposal.PDCAccountType))
                .ForMember(d => d.PDCAccountType, o => o.MapFrom(s => s.Proposal.PDCAccountType))
                .ForMember(d => d.PurposeName, o => o.MapFrom(s => s.Purpose > 0 ? UiUtil.GetDisplayName(s.Purpose) : ""))
                .ForMember(d => d.FacilityTypeName, o => o.MapFrom(s => UiUtil.GetDisplayName(s.FacilityType)));
            CreateMap<OfferLetterDto, OfferLetter>()
                  .ForMember(d => d.Proposal, o => o.Ignore())
                   .ForMember(d => d.OfferLetterTexts, o => o.Ignore());

            CreateMap<OfferLetterText, OfferLetterTextDto>()
                .ForMember(d => d.OfferTextTypeName, o => o.MapFrom(s => s.OfferTextType != null ? UiUtil.GetDisplayName(s.OfferTextType) : ""));
            CreateMap<OfferLetterTextDto, OfferLetterText>()
                .ForMember(d => d.OfferLetter, o => o.Ignore());

            CreateMap<FundConfirmation, FundConfirmationDto>();
            CreateMap<FundConfirmationDto, FundConfirmation>();

            CreateMap<FundConfirmationDetail, FundConfirmationDetailsDto>();
            CreateMap<FundConfirmationDetailsDto, FundConfirmationDetail>();

            CreateMap<DocumentCheckList, DocumentCheckListDto>()
                .ForMember(d => d.CustomerTypeName, o => o.MapFrom(s => s.Application != null ? UiUtil.GetDisplayName(s.Application.CustomerType) : ""))
                .ForMember(d => d.FacilityTypeName, o => o.MapFrom(s => s.FacilityType != null ? UiUtil.GetDisplayName(s.FacilityType) : ""))
                .ForMember(d => d.MaturityAmount, o => o.MapFrom(s => s.Application != null ? s.Application.DepositApplication != null ? s.Application.DepositApplication.MaturityAmount : null : 0)) /*s.Application.DepositApplication*/
                .ForMember(d => d.MaturityDate, o => o.MapFrom(s => s.Application != null ? s.Application.DepositApplication != null ? s.Application.DepositApplication.MaturityDate : null : null))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product != null ? s.Product.Name : ""))
                .ForMember(d => d.ApplicationNo, o => o.MapFrom(s => s.Application != null ? s.Application.ApplicationNo : ""))
                .ForMember(d => d.ApplicationTitle, o => o.MapFrom(s => s.Application != null ? s.Application.AccountTitle : ""))
                .ForMember(d => d.ApplicationDate, o => o.MapFrom(s => s.Application != null ? s.Application.ApplicationDate : DateTime.MinValue))
                .ForMember(d => d.CreditMemoNo, o => o.MapFrom(s => s.Proposal != null ? s.Proposal.CreditMemoNo : ""));
            CreateMap<DocumentCheckListDto, DocumentCheckList>()
                .ForMember(d => d.Documents, o => o.Ignore())
                .ForMember(d => d.Exceptions, o => o.Ignore())
                .ForMember(d => d.Application, o => o.Ignore())
                .ForMember(d => d.Proposal, o => o.Ignore())
                .ForMember(d => d.Product, o => o.Ignore())
                .ForMember(d => d.Employee, o => o.Ignore())
                .ForMember(d => d.Securities, o => o.Ignore())
                .ForMember(d => d.OfficeDesignationSetting, o => o.Ignore())
                .ForMember(d => d.Signatories, o => o.Ignore())
                ;

            CreateMap<DocumentCheckListDetail, DocumentCheckListDetailDto>()
                .ForMember(d => d.DocumentStatusName, o => o.MapFrom(s => s.DocumentStatus != null ? UiUtil.GetDisplayName(s.DocumentStatus) : ""));
            CreateMap<DocumentCheckListDetailDto, DocumentCheckListDetail>();

            CreateMap<DocumentCheckListException, DocumentCheckListExceptionDto>()
                .ForMember(d => d.ActionName, o => o.MapFrom(s => s.Action > 0 ? UiUtil.GetDisplayName(s.Action) : ""));
            CreateMap<DocumentCheckListExceptionDto, DocumentCheckListException>()
                .ForMember(d => d.DCL, o => o.Ignore());

            CreateMap<DepositApplicationTracking, DepositApplicationTrackingDto>()
                .ForMember(d => d.InstrumentDeliveryStatusName, o => o.MapFrom(s => s.InstrumentDeliveryStatus != null ? UiUtil.GetDisplayName(s.InstrumentDeliveryStatus) : ""))
                .ForMember(d => d.WelcomeLetterStatusName, o => o.MapFrom(s => s.WelcomeLetterStatus != null ? UiUtil.GetDisplayName(s.WelcomeLetterStatus) : ""));
            CreateMap<DepositApplicationTrackingDto, DepositApplicationTracking>();


            CreateMap<DocumentSecurities, DocumentSecuritiesDto>();
            CreateMap<DocumentSecuritiesDto, DocumentSecurities>()
                .ForMember(d => d.DCL, o => o.Ignore());

            CreateMap<PurchaseOrder, PurchaseOrderDto>()
                .ForMember(d => d.ApplicationNo, o => o.MapFrom(s => s.Application.ApplicationNo))
                .ForMember(d => d.ApplicationDate, o => o.MapFrom(s => s.Application.ApplicationDate))
                 .ForMember(d => d.SellersAddressString, o => o.MapFrom(s => s.SellersAddressId != null ?
                    ((string.IsNullOrEmpty(s.SellersAddress.AddressLine1) ? "" : (s.SellersAddress.AddressLine1 + ", ")) +
                    (string.IsNullOrEmpty(s.SellersAddress.AddressLine2) ? "" : (s.SellersAddress.AddressLine2 + ", ")) +
                    (string.IsNullOrEmpty(s.SellersAddress.AddressLine3) ? "" : (s.SellersAddress.AddressLine3 + ", ")) +
                    Environment.NewLine
                    + s.SellersAddress.Thana.ThanaNameEng
                    + ", " + s.SellersAddress.District.DistrictNameEng
                    + ", " + s.SellersAddress.Division.DivisionNameEng
                    + Environment.NewLine
                    + s.SellersAddress.Country.Name) : ""))
                   .ForMember(d => d.CustomerAddressString, o => o.MapFrom(s => s.CustomerAddressId != null ?
                    ((string.IsNullOrEmpty(s.CustomerAddress.AddressLine1) ? "" : (s.CustomerAddress.AddressLine1 + ", ")) +
                    (string.IsNullOrEmpty(s.CustomerAddress.AddressLine2) ? "" : (s.CustomerAddress.AddressLine2 + ", ")) +
                    (string.IsNullOrEmpty(s.CustomerAddress.AddressLine3) ? "" : (s.CustomerAddress.AddressLine3 + ", "))
                    + s.CustomerAddress.Thana.ThanaNameEng + ", " +
                    s.CustomerAddress.District.DistrictNameEng + ", " +
                    s.CustomerAddress.Division.DivisionNameEng + ", " +
                    s.CustomerAddress.Country.Name) : ""));
            CreateMap<PurchaseOrderDto, PurchaseOrder>()
                .ForMember(d => d.Application, o => o.Ignore())
                .ForMember(d => d.Proposal, o => o.Ignore())
                .ForMember(d => d.CustomerAddress, o => o.Ignore())
                .ForMember(d => d.SellersAddress, o => o.Ignore())
                .ForMember(d => d.Documents, o => o.Ignore());

            CreateMap<PODocument, PODocumentDto>();
            CreateMap<PODocumentDto, PODocument>();

            CreateMap<DisbursementMemo, DisbursementMemoDto>()
                .ForMember(d => d.ApplicationNo, o => o.MapFrom(s => s.Application != null ? s.Application.ApplicationNo : ""))
                .ForMember(d => d.CreditMemoNo, o => o.MapFrom(s => s.Proposal != null ? s.Proposal.CreditMemoNo : ""))
                .ForMember(d => d.AccountTitle, o => o.MapFrom(s => s.Application != null ? s.Application.AccountTitle : ""))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Application != null ? s.Application.Product != null ? s.Application.Product.Name : null : ""))
                .ForMember(d => d.FacilityTypeName, o => o.MapFrom(s => s.Proposal != null ? s.Proposal.FacilityType != null ? UiUtil.GetDisplayName(s.Proposal.FacilityType) : null : ""));
            CreateMap<DisbursementMemoDto, DisbursementMemo>()
                 .ForMember(d => d.Application, o => o.Ignore())
                .ForMember(d => d.Proposal, o => o.Ignore())
                .ForMember(d => d.Texts, o => o.Ignore())
                .ForMember(d => d.Signatories, o => o.Ignore());

            CreateMap<DMText, DMTextDto>();
            CreateMap<DMTextDto, DMText>()
                 .ForMember(d => d.DM, o => o.Ignore());

            CreateMap<Developer, DeveloperDto>();
            CreateMap<DeveloperDto, Developer>()
                .ForMember(d => d.Members, o => o.Ignore())
                .ForMember(d => d.Directors, o => o.Ignore())
                .ForMember(d => d.OtherDocuments, o => o.Ignore());

            CreateMap<DeveloperMember, DeveloperMemberDto>();
            CreateMap<DeveloperMemberDto, DeveloperMember>();

            CreateMap<DeveloperDirector, DeveloperDirectorDto>();
            CreateMap<DeveloperDirectorDto, DeveloperDirector>();

            CreateMap<Document, DocumentDto>();
            CreateMap<DocumentDto, Document>();

            CreateMap<DeveloperDocument, DeveloperDocumentDto>();
            CreateMap<DeveloperDocumentDto, DeveloperDocument>();

            CreateMap<Project, ProjectDto>();
            CreateMap<ProjectDto, Project>()
                .ForMember(d => d.ProjectAddress, o => o.Ignore());

            CreateMap<Vendor, VendorDto>();
            //.ForMember(d => d.Showrooms,
            //    o => o.MapFrom(s => s.Showrooms.Where(c => c.Status == EntityStatus.Active)));
            CreateMap<VendorDto, Vendor>()
                .ForMember(d => d.Showrooms, o => o.Ignore());

            CreateMap<VendorShowrooms, VendorShowroomsDto>();
            CreateMap<VendorShowroomsDto, VendorShowrooms>();

            CreateMap<CIF_Personal, CIF_KeyVal>()
                .ForMember(d => d.key, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.value, o => o.MapFrom(s => s.CIFNo + " - " + s.Name));
            CreateMap<CIF_Organizational, CIF_KeyVal>()
                .ForMember(d => d.key, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.value, o => o.MapFrom(s => s.CIFNo + " - " + s.CompanyName));

            CreateMap<LegalDocument, LegalDocumentDto>();
            CreateMap<LegalDocumentDto, LegalDocument>();

            CreateMap<LegalDocPropType, LegalDocPropTypeDto>();
            CreateMap<LegalDocPropTypeDto, LegalDocPropType>();

            CreateMap<LegalDocumentStatus, LegalDocumentStatusDto>()
                .ForMember(d => d.LegalDocumentName, o => o.MapFrom(s => s.LegalDocument.DocumentName));
            CreateMap<LegalDocumentStatusDto, LegalDocumentStatus>();

            CreateMap<LegalDocumentVerification, LegalDocumentVerificationDto>()
                .ForMember(d => d.LandTypeName, o => o.MapFrom(s => s.LandType != null ? UiUtil.GetDisplayName(s.LandType) : "")); ;
            CreateMap<LegalDocumentVerificationDto, LegalDocumentVerification>()
                .ForMember(d => d.LegalDocuments, o => o.Ignore());

            CreateMap<CostCenter, CostCenterDto>();
            CreateMap<CostCenterDto, CostCenter>();

            CreateMap<ProposalCreditCard, ProposalCreditCardDto>();
            CreateMap<ProposalCreditCardDto, ProposalCreditCard>();
            //DMDetailDto
            CreateMap<DMDetail, DMDetailDto>();
            CreateMap<DMDetailDto, DMDetail>();

            CreateMap<Proposal_Valuation_OtherCost, Proposal_Valuation_OtherCostDto>();
            CreateMap<Proposal_Valuation_OtherCostDto, Proposal_Valuation_OtherCost>();

            CreateMap<Signatories, SignatoriesDto>();
            CreateMap<SignatoriesDto, Signatories>();

            CreateMap<DCL_Signatory, DCL_SignatoryDto>()
                 .ForMember(d => d.Name, o => o.MapFrom(s => s.Signatory != null ? s.Signatory.Name : ""));
            CreateMap<DCL_SignatoryDto, DCL_Signatory>();

            CreateMap<Disbursment_Signatory, Disbursment_SignatoryDto>()
                 .ForMember(d => d.Name, o => o.MapFrom(s => s.Signatory != null ? s.Signatory.Name : ""));
            CreateMap<Disbursment_SignatoryDto, Disbursment_Signatory>();

            CreateMap<Notification, NotificationDto>();
            CreateMap<NotificationDto, Notification>();
        }
    }
}
