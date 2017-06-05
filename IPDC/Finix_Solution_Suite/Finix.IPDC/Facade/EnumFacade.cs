using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.DTO;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Util;

namespace Finix.IPDC.Facade
{
    public class EnumFacade : BaseFacade
    {
        public List<EnumDto> GetApplicationCustomerTypes()
        {
            var typeList = Enum.GetValues(typeof(ApplicationCustomerType))
               .Cast<ApplicationCustomerType>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = t.ToString(),
                   //DisplayName = UiUtil.GetDisplayName(t)
               });
            return typeList.ToList();
        }
        public List<EnumDto> GetApplicationTypes()
        {
            var typeList = Enum.GetValues(typeof(ApplicationType))
               .Cast<ApplicationType>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = t.ToString(),
                   //DisplayName = UiUtil.GetDisplayName(t)
               });
            return typeList.ToList();
        }
        public List<EnumDto> GetProductTypes()
        {
            var typeList = Enum.GetValues(typeof(ProductType))
               .Cast<ProductType>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = t.ToString(),
                   //DisplayName = UiUtil.GetDisplayName(t)
               });
            return typeList.ToList();
        }
        public List<EnumDto> GetApplicantRoles()
        {
            var typeList = Enum.GetValues(typeof(ApplicantRole))
               .Cast<ApplicantRole>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = UiUtil.GetDisplayName(t)
               });
            return typeList.ToList();
        }

        public List<EnumDto> GetDocumentStatusList()
        {
            var statusList = Enum.GetValues(typeof(DocumentStatus))
               .Cast<DocumentStatus>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = UiUtil.GetDisplayName(t)
               });
            return statusList.ToList();
        }

        public List<EnumDto> GetInstrumentStateList()
        {
            var statusList = Enum.GetValues(typeof(InstrumentDispatchStatus))
               .Cast<InstrumentDispatchStatus>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = UiUtil.GetDisplayName(t)
               });
            return statusList.ToList();
        }

        public List<EnumDto> GetWelcomeLetterStatus()
        {
            var statusList = Enum.GetValues(typeof(WelcomeLetterStatus))
               .Cast<WelcomeLetterStatus>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = UiUtil.GetDisplayName(t)
               });
            return statusList.ToList();
        }

        public List<EnumDto> GetLoanPrimarySecurityTypes()
        {
            var securityType = Enum.GetValues(typeof(LoanPrimarySecurityType))
          .Cast<LoanPrimarySecurityType>()
          .Select(t => new EnumDto
          {
              Id = ((int)t),
              Name = UiUtil.GetDisplayName(t)
          });
            return securityType.ToList();
        }
        public List<EnumDto> GetDisbursementModes()
        {
            var disbursmentMode = Enum.GetValues(typeof(DisbursementMode))
          .Cast<DisbursementMode>()
          .Select(t => new EnumDto
          {
              Id = ((int)t),
              Name = UiUtil.GetDisplayName(t)
          });
            return disbursmentMode.ToList();
        }
        public List<EnumDto> GetDisbursementToEnums()
        {
            var disbursmentMode = Enum.GetValues(typeof(DisbursementTo))
          .Cast<DisbursementTo>()
          .Select(t => new EnumDto
          {
              Id = ((int)t),
              Name = UiUtil.GetDisplayName(t)
          });
            return disbursmentMode.ToList();
        }

        public List<EnumDto> GetRelationshipWithApplicant()
        {
            var disbursmentMode = Enum.GetValues(typeof(RelationshipWithApplicant))
          .Cast<RelationshipWithApplicant>()
          .Select(t => new EnumDto
          {
              Id = ((int)t),
              Name = UiUtil.GetDisplayName(t)
          });
            return disbursmentMode.ToList();
        }
        public List<EnumDto> GetLoanChequeDeliveryOptions()
        {
            var disbursmentMode = Enum.GetValues(typeof(LoanChequeDeliveryOptions))
            .Cast<LoanChequeDeliveryOptions>()
            .Select(t => new EnumDto
            {
                Id = ((int)t),
                Name = UiUtil.GetDisplayName(t)
            });
            return disbursmentMode.ToList();
        }

        public List<EnumDto> GetRiskLevels()
        {
            var riskLevel = Enum.GetValues(typeof(RiskLevel))
            .Cast<RiskLevel>()
            .Select(t => new EnumDto
            {
                Id = ((int)t),
                Name = UiUtil.GetDisplayName(t)
            });
            return riskLevel.ToList();
        }

        public List<EnumDto> GetVehicleStatus()
        {
            var status = Enum.GetValues(typeof(VehicleStatus))
            .Cast<VehicleStatus>()
            .Select(t => new EnumDto
            {
                Id = ((int)t),
                Name = UiUtil.GetDisplayName(t)
            });
            return status.ToList();
        }

        public List<EnumDto> GetVendorTypes()
        {
            var vendorType = Enum.GetValues(typeof(VendorType))
       .Cast<VendorType>()
       .Select(t => new EnumDto
       {
           Id = ((int)t),
           Name = UiUtil.GetDisplayName(t)
       });
            return vendorType.ToList();
        }

        public List<EnumDto> GetVehicleTypes()
        {
            var vehicleType = Enum.GetValues(typeof(VehicleType))
            .Cast<VehicleType>()
            .Select(t => new EnumDto
            {
                Id = ((int)t),
                Name = UiUtil.GetDisplayName(t)
            });
            return vehicleType.ToList();
        }

        public List<EnumDto> GetLandedPropertyLoanTypes()
        {
            var propType = Enum.GetValues(typeof(LandedPropertyLoanType))
           .Cast<LandedPropertyLoanType>()
           .Select(t => new EnumDto
           {
               Id = ((int)t),
               Name = UiUtil.GetDisplayName(t)
           });
            return propType.ToList();
        }

        public List<EnumDto> GetLandedPropertySellertypes()
        {
            var seller = Enum.GetValues(typeof(LandedPropertySellertype))
            .Cast<LandedPropertySellertype>()
            .Select(t => new EnumDto
            {
                Id = ((int)t),
                Name = UiUtil.GetDisplayName(t)
            });
            return seller.ToList();
        }

        public List<EnumDto> GetWaiverTypes()
        {
            var waiverType = Enum.GetValues(typeof(TypeOfWaiverReq))
           .Cast<TypeOfWaiverReq>()
           .Select(t => new EnumDto
           {
               Id = ((int)t),
               Name = UiUtil.GetDisplayName(t)
           });
            return waiverType.ToList();
        }

        public List<EnumDto> GetAllCifOrgLegalStatus()
        {
            var waiverType = Enum.GetValues(typeof(CompanyLegalStatus))
           .Cast<CompanyLegalStatus>()
           .Select(t => new EnumDto
           {
               Id = ((int)t),
               Name = UiUtil.GetDisplayName(t)
           });
            return waiverType.ToList();
        }

        public List<EnumDto> GetAllCifOrgOwnerRoles()
        {
            var waiverType = Enum.GetValues(typeof(CIF_Org_OwnersRole))
           .Cast<CIF_Org_OwnersRole>()
           .Select(t => new EnumDto
           {
               Id = ((int)t),
               Name = UiUtil.GetDisplayName(t)
           });
            return waiverType.ToList();
        }

        public List<EnumDto> GetAllCallType()
        {
            var waiverType = Enum.GetValues(typeof(CallType))
           .Cast<CallType>()
           .Select(t => new EnumDto
           {
               Id = ((int)t),
               Name = UiUtil.GetDisplayName(t)
           });
            return waiverType.ToList();
        }

        public List<EnumDto> GetAllCallTypeCC()
        {
            var waiverType = Enum.GetValues(typeof(CallType)).Cast<CallType>().Select(t => new EnumDto
            {
                Id = ((int)t),
                Name = UiUtil.GetDisplayName(t)
            });
            return waiverType.Where(x => x.Id != 1).ToList();
        }

        //public List<EnumDto> GetAllLeadStatusRM()
        //{
        //    var waiverType = Enum.GetValues(typeof(LeadStatus)).Cast<LeadStatus>().Select(t => new EnumDto
        //    {
        //        Id = ((int)t),
        //        Name = UiUtil.GetDisplayName(t)
        //    });
        //    return waiverType.Where(x => x.Id == 2 && x.Id==4).ToList();
        //}
        public List<EnumDto> GetAllCallSource()
        {
            var waiverType = Enum.GetValues(typeof(CallCategory))
           .Cast<CallCategory>()
           .Select(t => new EnumDto
           {
               Id = ((int)t),
               Name = UiUtil.GetDisplayName(t)
           });
            return waiverType.ToList();
        }

        public List<EnumDto> GetAllCallFailReason()
        {
            var waiverType = Enum.GetValues(typeof(CallFailReason))
           .Cast<CallFailReason>()
           .Select(t => new EnumDto
           {
               Id = ((int)t),
               Name = UiUtil.GetDisplayName(t)
           });
            return waiverType.ToList();
        }

        public List<EnumDto> GetAllRelationships()
        {
            var waiverType = Enum.GetValues(typeof(RelationshipWithApplicant))
           .Cast<RelationshipWithApplicant>()
           .Select(t => new EnumDto
           {
               Id = ((int)t),
               Name = UiUtil.GetDisplayName(t)
           });
            return waiverType.ToList();
        }

        //public List<EnumDto> GetAllCallStatus()
        //{
        //    var waiverType = Enum.GetValues(typeof(CallStatus))
        //   .Cast<CallStatus>()
        //   .Select(t => new EnumDto
        //   {
        //       Id = ((int)t),
        //       Name = UiUtil.GetDisplayName(t)
        //   });
        //    return waiverType.ToList();
        //}
        public List<EnumDto> GetProjectStatuses()
        {
            var projectStatus = Enum.GetValues(typeof(ProjectStatus))
         .Cast<ProjectStatus>()
         .Select(t => new EnumDto
         {
             Id = ((int)t),
             Name = UiUtil.GetDisplayName(t)
         });
            return projectStatus.ToList();
        }

        public List<EnumDto> GetVerificationAs()
        {
            var projectStatus = Enum.GetValues(typeof(VerificationAs))
         .Cast<VerificationAs>()
         .Select(t => new EnumDto
         {
             Id = ((int)t),
             Name = UiUtil.GetDisplayName(t)
         });
            return projectStatus.ToList();
        }

        public List<EnumDto> GetClassificationStatus()
        {
            var projectStatus = Enum.GetValues(typeof(CIBClassificationStatus))
         .Cast<CIBClassificationStatus>()
         .Select(t => new EnumDto
         {
             Id = ((int)t),
             Name = UiUtil.GetDisplayName(t)
         });
            return projectStatus.ToList();
        }

        public List<EnumDto> GetApplicantsFlatStatuses()
        {
            var flatStatus = Enum.GetValues(typeof(FlatStatus))
            .Cast<FlatStatus>()
            .Select(t => new EnumDto
            {
                Id = ((int)t),
                Name = UiUtil.GetDisplayName(t)
            });
            return flatStatus.ToList();
        }

        public List<EnumDto> GetVerificationStates()
        {
            var verificationStatus = Enum.GetValues(typeof(VerificationState))
            .Cast<VerificationState>()
            .Select(t => new EnumDto
            {
                Id = ((int)t),
                Name = UiUtil.GetDisplayName(t)
            });
            return verificationStatus.ToList();
        }
        public List<EnumDto> GetOverallVerificationStates()
        {
            var verificationStatus = Enum.GetValues(typeof(OverallVerificationStatus))
            .Cast<OverallVerificationStatus>()
            .Select(t => new EnumDto
            {
                Id = ((int)t),
                Name = UiUtil.GetDisplayName(t)
            });
            return verificationStatus.ToList();
        }
        public List<EnumDto> GetVerificationStatuses()
        {
            var verificationStatus = Enum.GetValues(typeof(VerificationStatus))
            .Cast<VerificationStatus>()
            .Select(t => new EnumDto
            {
                Id = ((int)t),
                Name = UiUtil.GetDisplayName(t)
            });
            return verificationStatus.ToList();
        }

        public List<EnumDto> GetValuationType()
        {
            var valuationType = Enum.GetValues(typeof(LandedPropertyValuationType))
            .Cast<LandedPropertyValuationType>()
            .Select(t => new EnumDto
            {
                Id = ((int)t),
                Name = UiUtil.GetDisplayName(t)
            });
            return valuationType.ToList();
        }
        public List<EnumDto> GetLocationFindibility()
        {
            var valuationType = Enum.GetValues(typeof(LocationFindibility))
            .Cast<LocationFindibility>()
            .Select(t => new EnumDto
            {
                Id = ((int)t),
                Name = UiUtil.GetDisplayName(t)
            });
            return valuationType.ToList();
        }
        public List<EnumDto> GetYearsCurrentResidence()
        {
            var valuationType = Enum.GetValues(typeof(YearsCurrentResidence))
            .Cast<YearsCurrentResidence>()
            .Select(t => new EnumDto
            {
                Id = ((int)t),
                Name = UiUtil.GetDisplayName(t)
            });
            return valuationType.ToList();
        }
        public List<EnumDto> GetProjectApprovalAuthority()
        {
            var approvalAuthority = Enum.GetValues(typeof(ProjectApprovalAuthority))
            .Cast<ProjectApprovalAuthority>()
            .Select(t => new EnumDto
            {
                Id = ((int)t),
                Name = UiUtil.GetDisplayName(t)
            });
            return approvalAuthority.ToList();
        }
        public List<EnumDto> GetPropertyType()
        {
            var propertyType = Enum.GetValues(typeof(PropertyType))
            .Cast<PropertyType>()
            .Select(t => new EnumDto
            {
                Id = ((int)t),
                Name = UiUtil.GetDisplayName(t)
            });
            return propertyType.ToList();
        }
        public List<EnumDto> GetPropertyBounds()
        {
            var propertyBounds = Enum.GetValues(typeof(PropertyBounds))
            .Cast<PropertyBounds>()
            .Select(t => new EnumDto
            {
                Id = ((int)t),
                Name = UiUtil.GetDisplayName(t)
            });
            return propertyBounds.ToList();
        }
        public List<EnumDto> GetApprovalStatus()
        {
            var approvalStatus = Enum.GetValues(typeof(ApprovalStatus))
            .Cast<ApprovalStatus>()
            .Select(t => new EnumDto
            {
                Id = ((int)t),
                Name = UiUtil.GetDisplayName(t)
            });
            return approvalStatus.ToList();
        }

        public List<EnumDto> GetLandTypes()
        {
            var valuationType = Enum.GetValues(typeof(LandType))
            .Cast<LandType>()
            .Select(t => new EnumDto
            {
                Id = ((int)t),
                Name = UiUtil.GetDisplayName(t)
            });
            return valuationType.ToList();
        }

        public List<EnumDto> GetProposalProduct()
        {
            var data = Enum.GetValues(typeof(ProposalProduct))
              .Cast<ProposalProduct>()
              .Select(t => new EnumDto
              {
                  Id = ((int)t),
                  Name = UiUtil.GetDisplayName(t)
              });
            return data.ToList();
        }

        public List<EnumDto> GetDeveloperCategory()
        {
            var data = Enum.GetValues(typeof(DeveloperCategory))
            .Cast<DeveloperCategory>()
            .Select(t => new EnumDto
            {
                Id = ((int)t),
                Name = UiUtil.GetDisplayName(t)
            });
            return data.ToList();
        }
        public List<EnumDto> GetFacilityType()
        {
            var data = Enum.GetValues(typeof(ProposalFacilityType))
            .Cast<ProposalFacilityType>()
            .Select(t => new EnumDto
            {
                Id = ((int)t),
                Name = UiUtil.GetDisplayName(t)
            });
            return data.ToList();
        }

        public List<EnumDto> GetPrinterFiltering()
        {
            var data = Enum.GetValues(typeof(PrinterFiltering))
       .Cast<PrinterFiltering>()
       .Select(t => new EnumDto
       {
           Id = ((int)t),
           Name = UiUtil.GetDisplayName(t)
       });
            return data.ToList();
        }

        public List<EnumDto> GetPurposes()
        {
            var data = Enum.GetValues(typeof(Purpose))
            .Cast<Purpose>()
            .Select(t => new EnumDto
            {
                Id = ((int)t),
                Name = UiUtil.GetDisplayName(t)
            });
            return data.ToList();
        }

        public List<EnumDto> GetOfferTextType()
        {
            //throw new NotImplementedException();
            var data = Enum.GetValues(typeof(OfferTextType))
           .Cast<OfferTextType>()
           .Select(t => new EnumDto
           {
               Id = ((int)t),
               Name = UiUtil.GetDisplayName(t)
           });
            return data.ToList();
        }

        public List<EnumDto> GetAgeRange()
        {
            //throw new NotImplementedException();
            var data = Enum.GetValues(typeof(AgeRange))
           .Cast<AgeRange>()
           .Select(t => new EnumDto
           {
               Id = ((int)t),
               Name = UiUtil.GetDisplayName(t)
           });
            return data.ToList();
        }
        public List<EnumDto> GetIncomeRange()
        {
            //throw new NotImplementedException();
            var data = Enum.GetValues(typeof(IncomeRange))
           .Cast<IncomeRange>()
           .Select(t => new EnumDto
           {
               Id = ((int)t),
               Name = UiUtil.GetDisplayName(t)
           });
            return data.ToList();
        }
        public List<EnumDto> GetCustomerPriorities()
        {
            //throw new NotImplementedException();
            var data = Enum.GetValues(typeof(CustomerPriority))
           .Cast<CustomerPriority>()
           .Select(t => new EnumDto
           {
               Id = ((int)t),
               Name = UiUtil.GetDisplayName(t)
           });
            return data.ToList();
        }
        public List<EnumDto> GetCallModes()
        {
            //throw new NotImplementedException();
            var data = Enum.GetValues(typeof(CallMode))
           .Cast<CallMode>()
           .Select(t => new EnumDto
           {
               Id = ((int)t),
               Name = UiUtil.GetDisplayName(t)
           });
            return data.ToList();
        }

        public List<EnumDto> GetMaritalStatuses()
        {
            //throw new NotImplementedException();
            var data = Enum.GetValues(typeof(MaritalStatus))
           .Cast<MaritalStatus>()
           .Select(t => new EnumDto
           {
               Id = ((int)t),
               Name = UiUtil.GetDisplayName(t)
           });
            return data.ToList();
        }
        public List<EnumDto> GetCustomerSensitivities()
        {
            //throw new NotImplementedException();
            var data = Enum.GetValues(typeof(CustomerSensitivity))
           .Cast<CustomerSensitivity>()
           .Select(t => new EnumDto
           {
               Id = ((int)t),
               Name = UiUtil.GetDisplayName(t)
           });
            return data.ToList();
        }

        public List<EnumDto> GetLeadPriorities()
        {
            //throw new NotImplementedException();
            var data = Enum.GetValues(typeof(LeadPriority))
           .Cast<LeadPriority>()
           .Select(t => new EnumDto
           {
               Id = ((int)t),
               Name = UiUtil.GetDisplayName(t)
           });
            return data.ToList();
        }

        public List<EnumDto> GetDeveloperEnlistmentStatuses()
        {
            //throw new NotImplementedException();
            var data = Enum.GetValues(typeof(DeveloperEnlistmentStatus))
           .Cast<DeveloperEnlistmentStatus>()
           .Select(t => new EnumDto
           {
               Id = ((int)t),
               Name = UiUtil.GetDisplayName(t)
           });
            return data.ToList();
        }

        public List<EnumDto> GetBankAccountTypes()
        {
            //throw new NotImplementedException();
            var data = Enum.GetValues(typeof(BankAccountType))
           .Cast<BankAccountType>()
           .Select(t => new EnumDto
           {
               Id = ((int)t),
               Name = UiUtil.GetDisplayName(t)
           });
            return data.ToList();
        }

        public List<EnumDto> GetDeveloperType()
        {
            //throw new NotImplementedException();
            var data = Enum.GetValues(typeof(DeveloperType))
           .Cast<DeveloperType>()
           .Select(t => new EnumDto
           {
               Id = ((int)t),
               Name = UiUtil.GetDisplayName(t)
           });
            return data.ToList();
        }

        public List<EnumDto> GetDeveloperProjectStatus()
        {
            //throw new NotImplementedException();
            var data = Enum.GetValues(typeof(DeveloperProjectStatus))
           .Cast<DeveloperProjectStatus>()
           .Select(t => new EnumDto
           {
               Id = ((int)t),
               Name = UiUtil.GetDisplayName(t)
           });
            return data.ToList();
        }
        public List<EnumDto> GetBusinessTypes()
        {
            //throw new NotImplementedException();
            var data = Enum.GetValues(typeof(BusinessType))
           .Cast<BusinessType>()
           .Select(t => new EnumDto
           {
               Id = ((int)t),
               Name = UiUtil.GetDisplayName(t)
           });
            return data.ToList();
        }
        public List<EnumDto> GetBusinessSizes()
        {
            //throw new NotImplementedException();
            var data = Enum.GetValues(typeof(BusinessSize))
           .Cast<BusinessSize>()
           .Select(t => new EnumDto
           {
               Id = ((int)t),
               Name = UiUtil.GetDisplayName(t)
           });
            return data.ToList();
        }
        public List<EnumDto> GetCIF_Org_SectorTypes()
        {
            //throw new NotImplementedException();
            var data = Enum.GetValues(typeof(CIF_Org_SectorType))
           .Cast<CIF_Org_SectorType>()
           .Select(t => new EnumDto
           {
               Id = ((int)t),
               Name = UiUtil.GetDisplayName(t)
           });
            return data.ToList();
        }
        public List<EnumDto> GetSectorCodeTypes()
        {
            //throw new NotImplementedException();
            var data = Enum.GetValues(typeof(SectorCodeType))
           .Cast<SectorCodeType>()
           .Select(t => new EnumDto
           {
               Id = ((int)t),
               Name = UiUtil.GetDisplayName(t)
           });
            return data.ToList();
        }
        public List<EnumDto> GetHomeOwnerships()
        {
            var data = Enum.GetValues(typeof(HomeOwnership))
           .Cast<HomeOwnership>()
           .Select(t => new EnumDto
           {
               Id = ((int)t),
               Name = UiUtil.GetDisplayName(t)
           });
            return data.ToList();
        }

        public List<EnumDto> GetChequeDeliveryOptions()
        {
            var data = Enum.GetValues(typeof(LoanChequeDeliveryOptions))
            .Cast<LoanChequeDeliveryOptions>()
            .Select(t => new EnumDto
            {
                Id = ((int)t),
                Name = UiUtil.GetDisplayName(t)
            });
            return data.ToList();
        }

        public List<EnumDto> GetVendorProductType()
        {
            var data = Enum.GetValues(typeof(VendorProductType))
            .Cast<VendorProductType>()
            .Select(t => new EnumDto
            {
                Id = ((int)t),
                Name = UiUtil.GetDisplayName(t)
            });
            return data.ToList();
        }
        public List<EnumDto> GetAuthorityLevel()
        {
            var data = Enum.GetValues(typeof(ApprovalAuthorityLevel))
            .Cast<ApprovalAuthorityLevel>()
            .Select(t => new EnumDto
            {
                Id = ((int)t),
                Name = UiUtil.GetDisplayName(t)
            });
            return data.ToList();
        }
        
    }
}
