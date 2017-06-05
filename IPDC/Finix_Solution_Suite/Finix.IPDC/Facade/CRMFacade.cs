using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Finix.IPDC.DTO;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;
using PagedList;
using System.Globalization;
using System.Net.Mime;
using System.Transactions;
using Finix.IPDC.Util;

namespace Finix.IPDC.Facade
{
    public class CRMFacade : BaseFacade
    {
        private SequencerFacade _sequencer = new SequencerFacade();
        private ApplicationFacade _application = new ApplicationFacade();
        public ProposalDto LoadProposalByAppId(long? appId, long? id)
        {
            var proposalDto = new ProposalDto();
            var proposalIcmes = new List<Proposal_IncomeDto>();
            proposalDto.Liabilities = new List<Proposal_LiabilityDto>();
            proposalDto.NetWorths = new List<Proposal_NetWorthDto>();
            proposalDto.CIBs = new List<Proposal_CIBDto>();
            proposalDto.TotalExpences = 0;
            List<ProposalCreditCardDto> propCreditCards = new List<ProposalCreditCardDto>();
            if (appId > 0 && id == null)
            {
                #region generate new data from application and verifications
                //var dataOfProposal = GenService.GetAll<Proposal>().Where(r => r.ApplicationId == appId && r.Status == EntityStatus.Active);
                //if (!dataOfProposal.Any())
                //{
                //List<long> clientList = new List<long>();
                List<Proposal_NetWorthDto> aNetWorthDto = new List<Proposal_NetWorthDto>();
                var applicationDataById = GenService.GetById<Application>((long)appId);

                if (applicationDataById != null)
                {
                    proposalDto.ApplicationReceiveDate = applicationDataById.ApplicationDate;
                    proposalDto.ApplicationNo = applicationDataById.ApplicationNo;
                    if (applicationDataById.BranchId != null)
                        proposalDto.BranchName = applicationDataById.BranchOffice.Name;
                    var crmSubmission =
                        GenService.GetAll<ApplicationLog>()
                            .Where(
                                l =>
                                    l.ApplicationId == applicationDataById.Id &&
                                    l.ToStage == ApplicationStage.SentToCRM) // Previous 'ApprovedByBM'
                            .FirstOrDefault();
                    if (crmSubmission != null)
                        proposalDto.CRMReceiveDate = crmSubmission.CreateDate;
                    if (applicationDataById.Product.ProposalProduct != null)
                        proposalDto.Product = (ProposalProduct)applicationDataById.Product.ProposalProduct;
                    proposalDto.ProposalDate = DateTime.Now;
                    proposalDto.ApplicationId = applicationDataById.Id;
                    //var authoritySignatory = GenService.GetAll<ApprovalAuthoritySignatory>().Where(p => p.MemoType == MemoType.Credit_Memo && p.Status == EntityStatus.Active).OrderByDescending(r => r.Id).FirstOrDefault();
                    //if (authoritySignatory != null)
                    //{
                    //    var signatories =
                    //        authoritySignatory.AuthorityGroup.AuthorityGroupDetails.Select(
                    //            r => new Proposal_SignatoryDto
                    //            {
                    //                ApprovalAuthoritySignatoryName = r.ApprovalAuthorityGroup.Name,
                    //                Name = r.OfficeDesignationSetting.Name,
                    //                SignatoryId = r.Id
                    //            }).ToList();
                    //    proposalDto.Signatories = signatories;
                    //    proposalDto.ApprovalAuthoritySignatoryId = authoritySignatory.Id;
                    //}
                    if (applicationDataById.Product != null)
                    {
                        if (applicationDataById.Product.ProposalProduct != null)
                            proposalDto.Product = (ProposalProduct)applicationDataById.Product.ProposalProduct;
                        proposalDto.FacilityType = applicationDataById.Product.FacilityType;
                        proposalDto.FacilityTypeName = UiUtil.GetDisplayName(applicationDataById.Product.FacilityType);
                    }
                    proposalDto.TLComment = applicationDataById.TLComment;
                    proposalDto.BMComment = applicationDataById.BMComment;

                }
                if (applicationDataById.RMId != null)
                {
                    proposalDto.RMName = GenService.GetById<Employee>((long)applicationDataById.RMId).Person.Name;
                    proposalDto.RMCode = GenService.GetById<Employee>((long)applicationDataById.RMId).RmCode;
                }
                var applicationLog =
                    GenService.GetAll<ApplicationLog>()
                        .Where(r => r.ApplicationId == appId && r.Status == EntityStatus.Active)
                        .OrderByDescending(r => r.Id).FirstOrDefault();
                if (applicationLog != null)
                {
                    var crmDate = applicationLog.CreateDate;
                    proposalDto.CRMReceiveDate = crmDate ?? DateTime.MinValue;
                }
                if (applicationDataById.CIFList != null)
                {
                    foreach (var applicationCiFse in applicationDataById.CIFList.Where(c => c.Status == EntityStatus.Active))
                    {
                        var income = GenService.GetAll<IncomeVerification>().Where(r => r.Status == EntityStatus.Active && r.CifId == applicationCiFse.CIF_PersonalId && r.ApplicationId == applicationDataById.Id).OrderByDescending(r => r.Id).FirstOrDefault();
                        var nvw = GenService.GetAll<NetWorthVerification>().Where(r => r.Status == EntityStatus.Active && r.CIF_PersonalId == applicationCiFse.CIF_PersonalId && r.ApplicationId == applicationDataById.Id).OrderByDescending(r => r.Id).FirstOrDefault();

                        if (applicationCiFse.CIF_Personal != null)
                        {
                            if (applicationCiFse.CIF_Personal.CreditCards != null)
                            {
                                var bank = applicationCiFse.CIF_Personal.CreditCards.Where(c => c.Status == EntityStatus.Active && c.CreditCardLimit > 0).Select(x => new ProposalCreditCardDto
                                {
                                    CreditCardId = x.Id,
                                    CIFId = x.CIFId,
                                    CreditCardNo = x.CreditCardNo,
                                    CreditCardIssuersName = x.CreditCardIssuersName,
                                    CreditCardIssueDate = x.CreditCardIssueDate,
                                    CreditCardLimit = x.CreditCardLimit
                                });
                                propCreditCards.AddRange(bank);
                            }
                            if (income != null)
                            {
                                var proposalIncome = new Proposal_IncomeDto();
                                if (income.MonthlySalaryAssessed > 0) //income.MonthlySalaryAssessedConsidered && 
                                {
                                    proposalIncome.CIFNo = applicationCiFse.CIF_Personal.CIFNo;
                                    proposalIncome.Name = applicationCiFse.CIF_Personal.Name;
                                    proposalIncome.ApplicantRole = (ApplicantRole)applicationCiFse.ApplicantRole;
                                    proposalIncome.ApplicantRoleName = applicationCiFse.ApplicantRole.ToString();
                                    proposalIncome.IsConsidered = income.MonthlySalaryAssessedConsidered;
                                    proposalIncome.IncomeSource = "Monthly Salary Assessed";
                                    proposalIncome.IncomeAmount = income.MonthlySalaryAssessed != null ? (decimal)income.MonthlySalaryAssessed : 0;
                                    proposalIcmes.Add(proposalIncome);
                                }
                                if (income.MonthlyInterestIncomeAssessed > 0) //income.MonthlyInterestIncomeAssessedConsidered && 
                                {
                                    proposalIncome = new Proposal_IncomeDto();
                                    proposalIncome.CIFNo = applicationCiFse.CIF_Personal.CIFNo;
                                    proposalIncome.Name = applicationCiFse.CIF_Personal.Name;
                                    proposalIncome.ApplicantRole = (ApplicantRole)applicationCiFse.ApplicantRole;
                                    proposalIncome.ApplicantRoleName = applicationCiFse.ApplicantRole.ToString();
                                    proposalIncome.IsConsidered = income.MonthlyInterestIncomeAssessedConsidered;
                                    proposalIncome.IncomeSource = "Monthly Interest Income Assessed";
                                    proposalIncome.IncomeAmount = income.MonthlyInterestIncomeAssessed != null ? (decimal)income.MonthlyInterestIncomeAssessed : 0;
                                    proposalIcmes.Add(proposalIncome);
                                }
                                if (income.MonthlyRentalIncomeAssessed > 0) //income.MonthlyRentalIncomeAssessedConsidered && 
                                {
                                    proposalIncome = new Proposal_IncomeDto();
                                    proposalIncome.CIFNo = applicationCiFse.CIF_Personal != null ? applicationCiFse.CIF_Personal.CIFNo : "";
                                    proposalIncome.Name = applicationCiFse.CIF_Personal != null ? applicationCiFse.CIF_Personal.Name : "";
                                    proposalIncome.ApplicantRole = applicationCiFse.ApplicantRole != null ? (ApplicantRole)applicationCiFse.ApplicantRole : 0;
                                    proposalIncome.ApplicantRoleName = applicationCiFse.ApplicantRole != null ? applicationCiFse.ApplicantRole.ToString() : "";
                                    proposalIncome.IsConsidered = income.MonthlyRentalIncomeAssessedConsidered;
                                    proposalIncome.IncomeSource = "Monthly Rental Income Assessed";
                                    proposalIncome.IncomeAmount = income.MonthlyRentalIncomeAssessed != null ? (decimal)income.MonthlyRentalIncomeAssessed : 0;
                                    proposalIcmes.Add(proposalIncome);
                                }
                                if (income.MonthlyBusinessIncomeAssessed > 0)//income.MonthlyBusinessIncomeAssessedConsidered &&
                                {
                                    proposalIncome = new Proposal_IncomeDto();
                                    proposalIncome.CIFNo = applicationCiFse.CIF_Personal.CIFNo;
                                    proposalIncome.Name = applicationCiFse.CIF_Personal.Name;
                                    proposalIncome.ApplicantRole = (ApplicantRole)applicationCiFse.ApplicantRole;
                                    proposalIncome.ApplicantRoleName = applicationCiFse.ApplicantRole.ToString();
                                    proposalIncome.IsConsidered = income.MonthlyBusinessIncomeAssessedConsidered;
                                    proposalIncome.IncomeSource = "Monthly Business Income Assessed";
                                    proposalIncome.IncomeAmount = income.MonthlyBusinessIncomeAssessed != null ? (decimal)income.MonthlyBusinessIncomeAssessed : 0;
                                    proposalIcmes.Add(proposalIncome);
                                }
                                //var incomeVerificaton = income.MonthlyOtherIncomesAssessed.Where(m => m.Status == EntityStatus.Active).Select(m => new Proposal_IncomeDto {
                                //    IsConsidered = m.IsConsidered
                                //}).ToList();
                                var incomeVerification = income.MonthlyOtherIncomesAssessed.Where(m => m.Status == EntityStatus.Active && m.IncomeAmount > 0).Select(m => new Proposal_IncomeDto
                                {
                                    CIFNo = applicationCiFse.CIF_Personal.Name,
                                    Name = applicationCiFse.CIF_Personal.Name,
                                    ApplicantRole = applicationCiFse.ApplicantRole != null ? (ApplicantRole)applicationCiFse.ApplicantRole : 0,
                                    ApplicantRoleName = applicationCiFse.ApplicantRole.ToString(),
                                    IsConsidered = m.IsConsidered,
                                    IncomeSource = m.SourceOfIncome,
                                    IncomeAmount = (decimal)m.IncomeAmount,
                                });
                                if (incomeVerification != null)
                                {
                                    proposalIcmes.AddRange(incomeVerification);
                                }
                            }
                            //cf in applicationDataById.CIFList
                            //                 join nwv in GenService.GetAll<NetWorthVerification>() on cf.CIF_Personal.Id equals
                            //                     nwv.CIF_PersonalId
                            //                 join
                            //var check = nvw.Liabilities.Where(r => r.Status == EntityStatus.Active);
                            if (nvw != null)
                            {
                                if (nvw.Liabilities != null)
                                {
                                    var liability = (from lib in nvw.Liabilities.Where(r => r.Status == EntityStatus.Active && r.LoanType != LoanType.Business)
                                                     select new Proposal_LiabilityDto
                                                     {
                                                         Name = applicationCiFse.CIF_Personal.Name,
                                                         FacilityType = lib.LoanType != null ? lib.LoanType.ToString() : "",
                                                         InstituteName = lib.BankOrFIName,
                                                         Limit = lib.LoanAmountOrLimit != null ? (decimal)lib.LoanAmountOrLimit : 0,
                                                         Outstanding = lib.OutstandingAmount != null ? (decimal)lib.OutstandingAmount : 0,
                                                         EMI = lib.InstallmentAmount != null ? (decimal)lib.InstallmentAmount : 0
                                                     }).ToList();
                                    proposalDto.Liabilities.AddRange(liability);
                                }
                            }
                            var aNetWorth = new Proposal_NetWorthDto();
                            aNetWorth.CIFId = applicationCiFse.CIF_PersonalId;
                            aNetWorth.CIFNo = applicationCiFse.CIF_Personal.CIFNo;
                            aNetWorth.Name = applicationCiFse.CIF_Personal.Name;
                            if (applicationCiFse.ApplicantRole != null)
                            {
                                aNetWorth.ClientRole = applicationCiFse.ApplicantRole == ApplicantRole.Primary ? VerificationAs.Applicant : VerificationAs.Co_Applicant;
                                aNetWorth.ClientRoleName = UiUtil.GetDisplayName(aNetWorth.ClientRole);
                            }
                            aNetWorth.TotalAssetOfApplicant = nvw != null && nvw.TotalAsset != null ? (decimal)nvw.TotalAsset : 0;
                            aNetWorth.TotalLiabilityOfApplicant = nvw != null && nvw.TotalLiabilities != null ? (decimal)nvw.TotalLiabilities : 0;
                            aNetWorth.NetWorthOfApplicant = nvw != null && nvw.NetWorth != null ? (decimal)nvw.NetWorth : 0;
                            proposalDto.NetWorths.Add(aNetWorth);

                            long role = applicationCiFse.ApplicantRole != null ? (long)applicationCiFse.ApplicantRole : 0;
                            var cibInfo = GenService.GetAll<CIB_Personal>()
                                    .Where(
                                        r =>
                                            r.CIF_PersonalId == applicationCiFse.CIF_PersonalId &&
                                            r.VerificationPersonRole == (VerificationAs)role)
                                    .OrderByDescending(r => r.VerificationDate).FirstOrDefault();
                            if (cibInfo != null)
                            {
                                var aCib = new Proposal_CIBDto();
                                aCib.CIFNo = applicationCiFse.CIF_Personal.CIFNo;
                                aCib.ClientRole = cibInfo.VerificationPersonRole;
                                aCib.ClientRoleName = cibInfo.VerificationPersonRole != null
                                    ? cibInfo.VerificationPersonRole.ToString()
                                    : "";
                                aCib.Name = applicationCiFse.CIF_Personal.Name;
                                if (cibInfo.CIBClassificationStatusAsBorrower != null)
                                {
                                    aCib.CIBStatus = (CIBClassificationStatus)cibInfo.CIBClassificationStatusAsBorrower;
                                }
                                aCib.CIBStatusName = aCib.CIBStatus != null ? aCib.CIBStatus.ToString() : "";
                                aCib.CIBDate = (DateTime)cibInfo.VerificationDate;
                                proposalDto.CIBs.Add(aCib);
                            }
                            if (income != null)
                            {
                                if (income.MonthlyExpenseTotalAssessed != null)
                                {
                                    proposalDto.TotalExpences = proposalDto.TotalExpences + income.MonthlyExpenseTotalAssessed;
                                }
                            }
                        }
                    }
                    if (proposalIcmes.Any())
                        proposalDto.Incomes = proposalIcmes;
                    proposalDto.ProposalCreditCards = propCreditCards;

                    var clientProfile =
                        applicationDataById.CIFList.Select(x => x.CIF_Personal != null && x.Status == EntityStatus.Active
                            ? new Proposal_ClientProfileDto
                            {
                                CIFPId = x.CIF_PersonalId,
                                ApplicantRole = x.ApplicantRole,
                                ApplicantRoleName = x.ApplicantRole != null ? UiUtil.GetDisplayName(x.ApplicantRole) : "",
                                CIFNo = x.CIF_Personal.CIFNo,
                                Name = x.CIF_Personal.Name,
                                //NID = x.CIF_Personal.NIDNo,
                                SmartNIDNo = x.CIF_Personal.SmartNIDNo,
                                PassportNo = x.CIF_Personal.PassportNo,
                                DLNo = x.CIF_Personal.DLNo,

                                //RelationshipWithApplicantName
                                DateOfBirth = x.CIF_Personal.DateOfBirth != null ? (DateTime)x.CIF_Personal.DateOfBirth : DateTime.MinValue,
                                AcademicQualification = x.CIF_Personal != null ? x.CIF_Personal.HighestEducationLevel.ToString() : "",
                                ProfessionName =
                                    x.CIF_Personal.Occupation != null ? x.CIF_Personal.Occupation.ProfessionId != null ? x.CIF_Personal.Occupation.Profession.Name : x.CIF_Personal.Occupation.OccupationType > 0 ? UiUtil.GetDisplayName(x.CIF_Personal.Occupation.OccupationType) : null : "",
                                Designation =
                                    x.CIF_Personal.Occupation != null ? x.CIF_Personal.Occupation.Designation : "",
                                OrganizationName =
                                    x.CIF_Personal.Occupation != null ? x.CIF_Personal.Occupation.OrganizationName : "",
                                //ExperienceDetails = x.CIF_Personal.Occupation != null ? x.CIF_Personal.Occupation.TotalYearOfServieExp.ToString() : "",
                                OfficeAddressId = x.CIF_Personal.Occupation != null ? x.CIF_Personal.Occupation.OfficeAddressId : 0,
                                OfficeAddress =
                                    x.CIF_Personal.Occupation != null
                                        ? Mapper.Map<AddressDto>(x.CIF_Personal.Occupation.OfficeAddress)
                                        : null,
                                PresentAddressId = x.CIF_Personal != null ? x.CIF_Personal.ResidenceAddressId : 0,
                                PresentAddress =
                                    x.CIF_Personal != null
                                        ? Mapper.Map<AddressDto>(x.CIF_Personal.ResidenceAddress)
                                        : null,
                                PermanentAddressId = x.CIF_Personal != null ? x.CIF_Personal.PermanentAddressId : 0,
                                PermanentAddress =
                                    x.CIF_Personal != null
                                        ? Mapper.Map<AddressDto>(x.CIF_Personal.PermanentAddress)
                                        : null,
                                Age = x.CIF_Personal != null && x.CIF_Personal.DateOfBirth != null ? (DateTime.Now.Year - ((DateTime)x.CIF_Personal.DateOfBirth).Year) * 12 + (DateTime.Now.Month - ((DateTime)x.CIF_Personal.DateOfBirth).Month) : 0,
                                //RelationshipWithApplicant = x.CIF_Personal != null
                                //        ? x
                                //        : null,
                            }
                            : null).ToList();
                    if (clientProfile.Any())
                    {
                        foreach (var item in clientProfile)
                        {
                            var cpv = GenService.GetAll<ContactPointVerification>().Where(r => r.Status == EntityStatus.Active && r.CifId == item.CIFPId && r.ApplicationId == applicationDataById.Id).OrderByDescending(r => r.Id).FirstOrDefault();
                            if (cpv != null)
                                item.ResidenceStatus = cpv.HomeOwnership != null ? (HomeOwnership)cpv.HomeOwnership : HomeOwnership.Others;
                            var nid = GenService.GetAll<NIDVerification>().Where(r => r.Status == EntityStatus.Active && r.CifId == item.CIFPId && r.ApplicationId == applicationDataById.Id).OrderByDescending(r => r.Id).FirstOrDefault();
                            if (nid != null)
                                item.NID = nid.NIDNo != null ? nid.NIDNo : "";
                        }
                        proposalDto.ClientProfiles = clientProfile;
                        var applicantName = clientProfile.Where(r => r.ApplicantRole == ApplicantRole.Primary).FirstOrDefault();
                        proposalDto.ApplicantName = applicantName != null ? applicantName.Name : "";
                    }
                    //clientList.AddRange(applicationDataById.CIFList.Select(x => x.CIF_PersonalId != null ? (long)x.CIF_PersonalId : 0));

                }
                if (applicationDataById.LoanApplication != null)
                {

                    proposalDto.AppliedLoanTermApplication = (int)applicationDataById.LoanApplication.Term;
                    proposalDto.InterestRateOffered = applicationDataById.LoanApplication.Rate;
                    proposalDto.ProcessingFeeAndDocChargesPercentage = applicationDataById.LoanApplication.ServiceChargeRate != null ? (decimal)applicationDataById.LoanApplication.ServiceChargeRate : 0;
                    //var amt = applicationDataById.LoanApplication.ServiceChargeAmount;
                    //proposalDto.ProcessingFeeAndDocChargesAmount = amt != null ? (decimal)amt : 0;
                    proposalDto.LoanPurpose = applicationDataById.LoanApplication != null ? applicationDataById.LoanApplication.Purpose : "";
                    proposalDto.AppliedLoanAmount = applicationDataById.LoanApplication.LoanAmountApplied;
                    proposalDto.PDCBankName = applicationDataById.LoanApplication.Bank;
                    proposalDto.PDCBankBranch = applicationDataById.LoanApplication.Branch;
                    proposalDto.PDCRoutingNo = applicationDataById.LoanApplication.RoutingNo;
                    proposalDto.PDCAccountTitle = applicationDataById.LoanApplication.PayeesName;
                    proposalDto.PDCAccountNo = applicationDataById.LoanApplication.PayeesAccountNo;
                    //proposalDto.PDCAccountType = applicationDataById.LoanApplication.Bank;
                    if (applicationDataById.LoanApplication.Guarantors != null)
                    {
                        //clientList.AddRange(applicationDataById.LoanApplication.Guarantors.Select(r => r.GuarantorCifId));
                        //var netWorthoFAppGrt =
                        //    applicationDataById.LoanApplication.Guarantors.Where(g => g.Status == EntityStatus.Active).Select(x => new Proposal_NetWorthDto
                        //    {
                        //        CIFId = x.GuarantorCif.Id,
                        //        CIFNo = x.GuarantorCif.CIFNo,
                        //        Name = x.GuarantorCif.Name,
                        //        ClientRoleName = VerificationAs.Guarantor.ToString(),
                        //        ClientRole = VerificationAs.Guarantor
                        //    }).ToList();
                        //aNetWorthDto.AddRange(netWorthoFAppGrt);
                        //var propGrt = applicationDataById.LoanApplication.Guarantors.Select(x => new Proposal_GuarantorDto
                        //{

                        //    Name = x.GuarantorCif.Name,
                        //    ProfessionName = x.GuarantorCif.Occupation.Profession.Name,
                        //    CompanyName = x.GuarantorCif.Occupation.OrganizationName,
                        //    Designation = x.GuarantorCif.Occupation.Designation,
                        //    RelationshipWithApplicant = (RelationshipWithApplicant)x.RelationshipWithApplicant,
                        //    Age = x.GuarantorCif.DateOfBirth == null ? 0 : (DateTime.Now.Year - ((DateTime)x.GuarantorCif.DateOfBirth).Year),
                        //    //MonthlyIncome= //todo -get Monthly Income Without using 
                        //});
                        foreach (var guarantor in applicationDataById.LoanApplication.Guarantors.Where(g => g.Status == EntityStatus.Active))
                        {
                            if (guarantor != null)
                            {

                                var cibInfo = GenService.GetAll<CIB_Personal>()
                                        .Where(r => r.CIF_PersonalId == guarantor.GuarantorCifId && r.VerificationPersonRole == VerificationAs.Guarantor)
                                        .OrderByDescending(r => r.VerificationDate).FirstOrDefault();
                                if (cibInfo != null)
                                {
                                    var aCib = new Proposal_CIBDto();
                                    aCib.CIFNo = guarantor.GuarantorCif != null ? guarantor.GuarantorCif.CIFNo : "";
                                    aCib.ClientRole = cibInfo.VerificationPersonRole;
                                    aCib.ClientRoleName = cibInfo.VerificationPersonRole != null
                                        ? cibInfo.VerificationPersonRole.ToString()
                                        : "";
                                    aCib.Name = guarantor.GuarantorCif != null ? guarantor.GuarantorCif.Name : "";
                                    aCib.CIBStatus = cibInfo.CIBClassificationStatusAsGuarantor != null ? (CIBClassificationStatus)cibInfo.CIBClassificationStatusAsGuarantor : 0;
                                    aCib.CIBStatusName = aCib.CIBStatus != null ? aCib.CIBStatus.ToString() : "";
                                    aCib.CIBDate = (DateTime)cibInfo.VerificationDate;
                                    proposalDto.CIBs.Add(aCib);
                                }
                            }

                        }

                        var propGrt = (from gtr in applicationDataById.LoanApplication.Guarantors.Where(g => g.Status == EntityStatus.Active)
                                       join iv in GenService.GetAll<IncomeVerification>().Where(r => r.Status == EntityStatus.Active && r.ApplicationId == applicationDataById.Id).OrderByDescending(g => g.Id) on gtr.GuarantorCifId equals iv.CifId
                                       //join incAssesed in GenService.GetAll<IncomeVerificationAdditionalIncomeAssessed>().Where(r=>r.Status== EntityStatus.Active) on iv.Id equals incAssesed.IncomeVerificationId
                                       select new Proposal_GuarantorDto
                                       {
                                           GuarantorCifId = gtr.GuarantorCif.Id,
                                           GuarantorCIF = Mapper.Map<CIF_KeyVal>(gtr.GuarantorCif),
                                           Name = gtr.GuarantorCif != null ? gtr.GuarantorCif.Name : "",
                                           ProfessionName = gtr.GuarantorCif != null ? gtr.GuarantorCif.OccupationId != null ? gtr.GuarantorCif.Occupation.ProfessionId != null ? gtr.GuarantorCif.Occupation.Profession.Name : gtr.GuarantorCif.Occupation.OccupationType > 0 ? UiUtil.GetDisplayName(gtr.GuarantorCif.Occupation.OccupationType) : null : null : "",
                                           CompanyName = gtr.GuarantorCif != null ? gtr.GuarantorCif.Occupation != null ? gtr.GuarantorCif.Occupation.OrganizationName : null : "",
                                           Designation = gtr.GuarantorCif != null ? gtr.GuarantorCif.Occupation != null ? gtr.GuarantorCif.Occupation.Designation : null : "",
                                           RelationshipWithApplicant = (RelationshipWithApplicant)gtr.RelationshipWithApplicant,
                                           Age =
                                               gtr.GuarantorCif.DateOfBirth == null
                                                   ? 0
                                                   : (DateTime.Now.Year - ((DateTime)gtr.GuarantorCif.DateOfBirth).Year),
                                           MonthlyIncome = iv != null ? (((iv.MonthlyBusinessIncomeAssessed != null ? iv.MonthlyBusinessIncomeAssessed : 0) + (iv.MonthlySalaryAssessed != null ? (decimal)iv.MonthlySalaryAssessed : 0) +
                                                         (iv.MonthlyRentalIncomeAssessed != null ? iv.MonthlyRentalIncomeAssessed : 0) + (iv.MonthlyInterestIncomeAssessed != null ? iv.MonthlyInterestIncomeAssessed : 0) +
                                                         (iv.MonthlyOtherIncomesAssessed.Where(r => r.IsConsidered == true).Sum(r => r.IncomeAmount)))) : 0
                                       }).ToList();
                        var propGrtgrp = (from off in propGrt
                                          group off by new { off.GuarantorCifId, } into grp
                                          let proposalGuarantorDto = grp.FirstOrDefault()
                                          where proposalGuarantorDto != null
                                          let firstOrDefault = proposalGuarantorDto
                                          where firstOrDefault != null
                                          select new Proposal_GuarantorDto
                                          {
                                              GuarantorCifId = grp.Key.GuarantorCifId,
                                              GuarantorCIF = grp != null ? proposalGuarantorDto.GuarantorCIF : null,
                                              Name = grp != null ? proposalGuarantorDto.Name : "",
                                              ProfessionName = grp != null ? proposalGuarantorDto.ProfessionName : "",
                                              CompanyName = grp != null ? proposalGuarantorDto.CompanyName : "",
                                              Designation = grp != null ? proposalGuarantorDto.Designation : "",
                                              RelationshipWithApplicant = grp != null ? proposalGuarantorDto.RelationshipWithApplicant : 0,
                                              Age = grp != null ? proposalGuarantorDto.Age : 0,
                                              MonthlyIncome = grp != null ? proposalGuarantorDto.MonthlyIncome : 0
                                          }).ToList();
                        proposalDto.Guarantors = propGrtgrp;
                        if (propGrtgrp.Any())
                        {
                            var networthVerification = GenService.GetAll<NetWorthVerification>().ToList().Where(r => r.Status == EntityStatus.Active && r.ApplicationId == applicationDataById.Id).OrderByDescending(r => r.Id);
                            var cifList = GenService.GetAll<CIF_Personal>().ToList();
                            var guarantorNetWorth = (from grt in propGrtgrp
                                                     join cif in cifList on grt.GuarantorCifId equals cif.Id
                                                     join nwv in networthVerification on grt.GuarantorCifId equals nwv.CIF.Id
                                                     select new Proposal_NetWorthDto
                                                     {
                                                         CIFId = cif.Id,
                                                         CIFNo = cif.CIFNo,
                                                         Name = cif.Name,
                                                         ClientRole = VerificationAs.Guarantor,
                                                         ClientRoleName = UiUtil.GetDisplayName(VerificationAs.Guarantor),
                                                         TotalAssetOfApplicant = nwv != null && nwv.TotalAsset != null ? (decimal)nwv.TotalAsset : 0,
                                                         TotalLiabilityOfApplicant = nwv != null && nwv.TotalLiabilities != null ? (decimal)nwv.TotalLiabilities : 0,
                                                         NetWorthOfApplicant = (decimal)nwv.CIF_NetWorth.NetWorth
                                                     }).ToList();
                            var distGrtNW = (from grtNw in guarantorNetWorth
                                             group grtNw by new { grtNw.CIFId, grtNw.CIFNo } into grp
                                             let proposalGuarantorDto = grp.FirstOrDefault()
                                             where proposalGuarantorDto != null
                                             let firstOrDefault = proposalGuarantorDto
                                             where firstOrDefault != null
                                             select new Proposal_NetWorthDto
                                             {
                                                 CIFNo = grp != null ? grp.Key.CIFNo : "",
                                                 Name = grp != null ? proposalGuarantorDto.Name : "",
                                                 ClientRole = grp != null ? proposalGuarantorDto.ClientRole : VerificationAs.Guarantor,
                                                 ClientRoleName = grp != null ? proposalGuarantorDto.ClientRoleName : "",
                                                 TotalAssetOfApplicant = grp != null ? proposalGuarantorDto.TotalAssetOfApplicant : 0,
                                                 TotalLiabilityOfApplicant = grp != null ? proposalGuarantorDto.TotalLiabilityOfApplicant : 0,
                                                 NetWorthOfApplicant = grp != null ? proposalGuarantorDto.NetWorthOfApplicant : 0

                                             }).ToList();
                            proposalDto.NetWorths.AddRange(distGrtNW);

                        }
                    }
                }

                //if (aNetWorthDto.Any())
                //{
                //var clientNetWorth = (from aNW in aNetWorthDto
                //                      join nwv in GenService.GetAll<NetWorthVerification>().ToList() on aNW.CIFId equals nwv.CIF.Id
                //                      select new Proposal_NetWorthDto
                //                      {
                //                          CIFNo = aNW.CIFNo,
                //                          Name = aNW.Name,
                //                          ClientRoleName = aNW.ClientRoleName,
                //                          TotalAssetOfApplicant = (decimal)nwv.CIF_NetWorth.TotalAsset,
                //                          TotalLiabilityOfApplicant = (decimal)nwv.CIF_NetWorth.TotalLiabilities,
                //                          NetWorthOfApplicant = (decimal)nwv.CIF_NetWorth.NetWorth
                //                      }).ToList();
                //proposalDto.NetWorths = clientNetWorth;

                //var cib = (from clt in aNetWorthDto
                //           join cbVer in GenService.GetAll<CIB_Personal>() on clt.CIFId equals cbVer.CIF_PersonalId
                //           select new Proposal_CIBDto
                //           {

                //               CIFNo = clt.CIFNo,
                //               ClientRole = clt.ClientRole,
                //               ClientRoleName = clt.ClientRoleName,
                //               Name = clt.CIFNo,
                //               CIBStatus =
                //                   (clt.ClientRole == VerificationAs.Applicant ||
                //                    clt.ClientRole == VerificationAs.Applicant)
                //                       ? (CIBClassificationStatus)cbVer.CIBClassificationStatusAsBorrower
                //                       : (CIBClassificationStatus)cbVer.CIBClassificationStatusAsGuarantor,
                //               CIBDate = (DateTime)cbVer.VerificationDate,
                //               TotalOutstandingAsBorrower = cbVer.TotalOutstandingAsBorrower != null ?(decimal)cbVer.TotalOutstandingAsBorrower :0,
                //               ClassifiedAmountAsBorrower = cbVer.ClassifiedAmountAsBorrower != null ? (decimal)cbVer.ClassifiedAmountAsBorrower :0,
                //               TotalEMIAsBorrower = cbVer.TotalEMIAsBorrower != null ?(decimal)cbVer.TotalEMIAsBorrower :0
                //           }).ToList();
                //proposalDto.CIBs = cib;
                //}
                if (applicationDataById.LoanApplication != null)
                {
                    var fdr =
                        GenService.GetAll<FDRPrimarySecurity>()
                            .FirstOrDefault(
                                r =>
                                    r.LoanApplicationId == applicationDataById.LoanApplication.Id &&
                                    applicationDataById.LoanApplication.LoanPrimarySecurityType ==
                                    LoanPrimarySecurityType.FDRPrimarySecurity);
                    if (fdr != null)
                    {
                        var fdrCrmData =
                            fdr.FDRPSDetails.Where(r => r.Status == EntityStatus.Active).Select(p => new Proposal_FDRDto
                            {
                                InstituteName = p.InstituteName,
                                BranchName = p.BranchName,
                                FDRAccountNo = p.FDRAccountNo,
                                Amount = p.Amount,
                                DepositorName = p.Depositor,
                                MaturityDate = p.MaturityDate
                            }).ToList();
                        proposalDto.FDRs = new List<Proposal_FDRDto>();
                        proposalDto.FDRs.AddRange(fdrCrmData);
                    }

                    var consumer =
                        GenService.GetAll<ConsumerGoodsPrimarySecurity>().FirstOrDefault(r => r.LoanApplicationId == applicationDataById.LoanApplication.Id && applicationDataById.LoanApplication.LoanPrimarySecurityType == LoanPrimarySecurityType.ConsumerGoodsPrimarySecurity);
                    var vehicle =
                        GenService.GetAll<VehiclePrimarySecurity>().FirstOrDefault(r => r.LoanApplicationId == applicationDataById.LoanApplication.Id && applicationDataById.LoanApplication.LoanPrimarySecurityType == LoanPrimarySecurityType.VehiclePrimarySecurity);
                    ////var lpSecurity =
                    ////    GenService.GetAll<LPPrimarySecurity>().FirstOrDefault(r => r.LoanApplicationId == applicationDataById.LoanApplication.Id && applicationDataById.LoanApplication.LoanPrimarySecurityType == LoanPrimarySecurityType.LPPrimarySecurity);
                    var lpSecurity = GenService.GetAll<LPPrimarySecurity>().Where(C => C.Status == EntityStatus.Active).OrderByDescending(C => C.Id).OrderByDescending(C => C.Id).FirstOrDefault(r => r.LoanApplicationId == applicationDataById.LoanApplication.Id && applicationDataById.LoanApplication.LoanPrimarySecurityType == LoanPrimarySecurityType.LPPrimarySecurity);
                    if (consumer != null)
                    {
                        //data.ConsumerGoodsPrimarySecurity = Mapper.Map<ConsumerGoodsPrimarySecurityDto>(consumer);
                        proposalDto.CG_Item = consumer.Item;
                        proposalDto.CG_Brand = consumer.Brand;
                        proposalDto.CG_DealerName = consumer.Dealer;
                        proposalDto.CG_Price = consumer.Price;
                        var consumerGoodsPrimarySecurityValuation = consumer.Valuations.Where(r => r.Status == EntityStatus.Active)
                            .OrderByDescending(r => r.Id)
                            .FirstOrDefault();
                        if (consumerGoodsPrimarySecurityValuation != null)
                            proposalDto.PresentMarketValue =
                                consumerGoodsPrimarySecurityValuation
                                    .VerifiedPrice;
                    }
                    else if (vehicle != null)
                    {
                        //data.VehiclePrimarySecurity = Mapper.Map<VehiclePrimarySecurityDto>(vehicle);
                        proposalDto.Vehicle_Name = vehicle.VehicleName;
                        proposalDto.Vehicle_ModelYear = vehicle.YearModel;
                        proposalDto.Vehicle_VendorName = vehicle.Vendor != null ? vehicle.Vendor.Name : "";
                        proposalDto.Vehicle_QuotedPrice = vehicle.Price;
                        proposalDto.CC = vehicle.CC != null ? vehicle.CC : ""; ;
                        proposalDto.Colour = vehicle.Colour != null ? vehicle.Colour : ""; ;
                        proposalDto.ChassisNo = vehicle.ChassisNo != null ? vehicle.ChassisNo : "";
                        proposalDto.EngineNo = vehicle.EngineNo != null ? vehicle.EngineNo : "";
                        proposalDto.Vehicle_VendorName = vehicle.Vendor != null ? vehicle.Vendor.Name : "";
                        var vehiclePrimarySecurityValuation = vehicle.Valuations.Where(r => r.Status == EntityStatus.Active)
                            .OrderByDescending(r => r.Id)
                            .FirstOrDefault();
                        if (vehiclePrimarySecurityValuation != null)
                            proposalDto.PresentMarketValue = vehiclePrimarySecurityValuation.VerifiedPrice;
                    }

                    else if (lpSecurity != null)
                    {
                        proposalDto.SellersName = lpSecurity.SellerName;
                        var securityValuation = GenService.GetAll<LPPrimarySecurityValuation>().Where(r => r.LPPrimarySecurityId == lpSecurity.Id && r.Status == EntityStatus.Active).OrderByDescending(r => r.Id).FirstOrDefault();
                        //return Mapper.Map<LPPrimarySecurityValuationDto>(data);
                        //var securityValuation = lpSecurity.Valuations.OrderByDescending(r => r.Id)
                        //    .FirstOrDefault(r => r.Status == EntityStatus.Active);
                        if (lpSecurity.Project != null)
                        {
                            proposalDto.ProjectAddress = Mapper.Map<AddressDto>(lpSecurity.Project.ProjectAddress);
                        }
                        if (securityValuation != null)
                        {
                            if (securityValuation != null)
                                proposalDto.PropertyType =
                                    securityValuation
                                        .ValuationType;
                            proposalDto.DevelopersName = securityValuation.LPPrimarySecurity.Developer != null ? securityValuation.LPPrimarySecurity.Developer.GroupName : "";
                            if (securityValuation.LPPrimarySecurity.Project != null)
                            {
                                proposalDto.ProjectName = securityValuation.LPPrimarySecurity.Project.ProjectName;
                                //proposalDto.ProjectAddress = Mapper.Map<AddressDto>(securityValuation.ProjectAddress);
                                if (lpSecurity.Project.LegalVerifications.Count > 0)
                                    proposalDto.TotalLandArea = lpSecurity.Project.LegalVerifications.OrderByDescending(r => r.Id)
                                    .FirstOrDefault()
                                    .AreaOfLandTD;
                                var projectLegalVerification = lpSecurity.Project.LegalVerifications.OrderByDescending(r => r.Id).FirstOrDefault(r => r.Status == EntityStatus.Active);
                                if (projectLegalVerification != null)
                                {
                                    proposalDto.PropertyOwnershipType = projectLegalVerification.LandType;
                                }

                            }

                            proposalDto.TotalLandArea = securityValuation.AreaOfLandAsPerPlan;
                            //proposalDto.FlatDetails= lpSecurity.Valuations.OrderByDescending(r => r.Id).FirstOrDefault(r => r.Status == EntityStatus.Active).flat;
                            //var primarySecurityValuation = lpSecurity.Valuations.OrderByDescending(r => r.Id).FirstOrDefault(r => r.Status == EntityStatus.Active);
                            //if (primarySecurityValuation != null)
                            proposalDto.NumberOfCarParking = securityValuation.CarParkingCount;
                            //var firstOrDefault = lpSecurity.Valuations.OrderByDescending(r => r.Id).FirstOrDefault(r => r.Status == EntityStatus.Active);
                            //if (firstOrDefault != null)
                            proposalDto.FlatSize = securityValuation.FlatSizeWithCommonSpace;
                            //var perKathaPrice = lpSecurity.Valuations.OrderByDescending(r => r.Id)
                            //        .FirstOrDefault(r => r.Status == EntityStatus.Active);
                            proposalDto.PricePerSQF = securityValuation.PerKathaPriceAsPerRAJUK != null ? (decimal)securityValuation.PerKathaPriceAsPerRAJUK : 0;
                            proposalDto.MarketPriceOfFlat = securityValuation.MarketPriceOfFlat != null ? (decimal)securityValuation.MarketPriceOfFlat : 0;
                            proposalDto.CarParkingPrice = securityValuation.CarParkingPrice != null ? (decimal)securityValuation.CarParkingPrice : 0;
                            proposalDto.EstimatedConstructionCostApproved = securityValuation.EstimatedConstructionCostPhysical != null ? securityValuation.EstimatedConstructionCostPhysical : 0;

                        }
                        if (lpSecurity.Developer != null)
                        {
                            if (lpSecurity.Developer.EnlistmentStatus == DeveloperEnlistmentStatus.Approved)
                            {
                                proposalDto.DeveloperCategory = lpSecurity.Developer.ApprovalCategory;
                            }
                        }
                        if (lpSecurity.ProjectId != null && lpSecurity.ProjectId > 0)
                        {
                            var technical = GenService.GetAll<ProjectTechnicalVerification>().Where(r => r.ProjectId == lpSecurity.ProjectId && r.Status == EntityStatus.Active).OrderByDescending(r => r.Id).FirstOrDefault();
                            if (technical != null)
                            {
                                proposalDto.TotalNumberOfFloors = technical.ActualNoOfFloors;
                            }
                        }
                        if (lpSecurity.LandedPropertyLoanType == null && lpSecurity.LandedPropertyLoanType == LandedPropertyLoanType.Flat_Purchase)
                        {
                            if (lpSecurity.FloorNo != null && lpSecurity.FlatSide != null && lpSecurity.FlatSize != null)
                            {
                                proposalDto.FlatDetails = "Floor No. :" + lpSecurity.FloorNo + " " + "Flat Side. :" +
                                                          lpSecurity.FlatSide + " " + "Flat Size. :" +
                                                          lpSecurity.FlatSize;
                            }
                            else
                            {
                                proposalDto.FlatDetails = "Flat Size,Floor No.,Flat Side Information Missing";
                            }

                        }
                        //proposalDto.TotalNumberOfSlabsCasted =lpSecurity.Valuations.FirstOrDefault().
                        //if (lpSecurity.Valuations.Count > 0)
                        //{
                        //    var lpPrimarySecurityValuation = lpSecurity.Valuations.OrderByDescending(r => r.Id)
                        //   .FirstOrDefault(r => r.Status == EntityStatus.Active);
                        //    if (lpPrimarySecurityValuation != null)
                        //    {
                        //        proposalDto.TotalLandArea =
                        //            lpPrimarySecurityValuation
                        //                .AreaOfLandAsPerPlan;
                        //        //proposalDto.FlatDetails= lpSecurity.Valuations.OrderByDescending(r => r.Id).FirstOrDefault(r => r.Status == EntityStatus.Active).flat;
                        //        //var primarySecurityValuation = lpSecurity.Valuations.OrderByDescending(r => r.Id).FirstOrDefault(r => r.Status == EntityStatus.Active);
                        //        //if (primarySecurityValuation != null)
                        //        proposalDto.NumberOfCarParking = lpPrimarySecurityValuation.CarParkingCount;
                        //        //var firstOrDefault = lpSecurity.Valuations.OrderByDescending(r => r.Id).FirstOrDefault(r => r.Status == EntityStatus.Active);
                        //        //if (firstOrDefault != null)
                        //        proposalDto.FlatSize = lpPrimarySecurityValuation.FlatSizeWithCommonSpace;
                        //        //var perKathaPrice = lpSecurity.Valuations.OrderByDescending(r => r.Id)
                        //        //        .FirstOrDefault(r => r.Status == EntityStatus.Active);
                        //        proposalDto.PricePerSQF = lpPrimarySecurityValuation.PerKathaPriceAsPerRAJUK != null ? (decimal)lpPrimarySecurityValuation.PerKathaPriceAsPerRAJUK : 0;
                        //        proposalDto.MarketPriceOfFlat = lpPrimarySecurityValuation.MarketPriceOfFlat != null ? (decimal)lpPrimarySecurityValuation.MarketPriceOfFlat : 0;
                        //        proposalDto.CarParkingPrice = lpPrimarySecurityValuation.CarParkingPrice != null ? (decimal)lpPrimarySecurityValuation.CarParkingPrice : 0;
                        //        proposalDto.EstimatedConstructionCostApproved = lpPrimarySecurityValuation.EstimatedConstructionCostPhysical != null ? lpPrimarySecurityValuation.EstimatedConstructionCostPhysical : 0;
                        //    }
                        //}
                    }

                    List<Proposal_SecurityDetailDto> securities =
                        applicationDataById.LoanApplication.LoanAppColSecurities.Select(x => new Proposal_SecurityDetailDto
                        {
                            SecurityType = ProposalSecurityDetailType.ColSecurity,
                            SecurityTypeName = ProposalSecurityDetailType.ColSecurity.ToString(),
                            Details = x.LoanAppSecurity != null ? x.LoanAppSecurity.SecurityDescription : "",
                        }).ToList();
                    List<Proposal_SecurityDetailDto> otherSecurity = applicationDataById.LoanApplication.OtherSecurities.Select(x => new Proposal_SecurityDetailDto
                    {
                        SecurityType = ProposalSecurityDetailType.ColSecurity,
                        SecurityTypeName = ProposalSecurityDetailType.OtherSecurity.ToString(),
                        Details = x.SecurityDescription
                    }).ToList();
                    securities.AddRange(otherSecurity);
                    proposalDto.SecurityDetails = securities;

                }
                // }
                #endregion
            }
            else if (id > 0)
            {
                var proposal = GenService.GetById<Proposal>((long)id);
                proposalDto = Mapper.Map<ProposalDto>(proposal);
                proposalDto.TotalCreditCard = 0;
                if (proposalDto.ProposalCreditCards != null)
                {
                    foreach (var creditCart in proposalDto.ProposalCreditCards)
                    {
                        proposalDto.TotalCreditCard += creditCart.CreditCardLimit != null
                            ? creditCart.CreditCardLimit
                            : 0;
                    }
                }
                proposalDto.TotalCreditCard = (proposalDto.TotalCreditCard != null ? proposalDto.TotalCreditCard * (decimal).05 : 0) + proposal.LiabilityTotalEMI;
                proposalDto.OtherCosts.RemoveAll(f => f.Status != EntityStatus.Active);
                proposalDto.ValuationOtherCosts.RemoveAll(f => f.Status != EntityStatus.Active);
                proposalDto.OverallAssessments.RemoveAll(f => f.Status != EntityStatus.Active);
                proposalDto.Texts.RemoveAll(f => f.Status != EntityStatus.Active);
                proposalDto.Signatories.RemoveAll(f => f.Status != EntityStatus.Active);
                proposalDto.ProposalCreditCards.RemoveAll(f => f.Status != EntityStatus.Active);
                if (proposalDto.Application != null)
                {
                    proposalDto.TLComment = proposalDto.Application.TLComment;
                    proposalDto.BMComment = proposalDto.Application.BMComment;
                }
            }
            return proposalDto;
        }

        public long GuarantorId(long propId)
        {
            var guarantors = GenService.GetAll<Proposal_Guarantor>().FirstOrDefault(g => g.ProposalId == propId);
            if (guarantors != null)
            {
                return guarantors.Id;
            }
            return 0;
        }

        public long GetProposalId(long AppId)
        {
            var proposal = GenService.GetAll<Proposal>().FirstOrDefault(p => p.ApplicationId == AppId);
            if (proposal != null)
            {
                return proposal.Id;
            }
            return 0;
        }
        public Address EditAddress(AddressDto dto)
        {
            var address = new Address();
            try
            {
                if (dto.Id != null)
                {
                    address = GenService.GetById<Address>((long)dto.Id);
                    dto.CreateDate = address.CreateDate;
                    dto.CreatedBy = address.CreatedBy;
                    address = Mapper.Map(dto, address);
                    GenService.Save(address);
                }
                else
                {
                    address = Mapper.Map<Address>(dto);
                    GenService.Save(address);
                }
                return address;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public ResponseDto SaveProposal(ProposalDto dto, long userId)
        {
            ResponseDto response = new ResponseDto();
            #region Edit Proposal
            if (dto.Id > 0 && dto.Id != null)
            {
                List<Proposal_ClientProfile> aClientProfile = new List<Proposal_ClientProfile>();
                List<Proposal_Income> incomeList = new List<Proposal_Income>();
                List<Proposal_NetWorth> netWorthList = new List<Proposal_NetWorth>();
                List<Proposal_CIB> cibList = new List<Proposal_CIB>();
                List<Proposal_Liability> liabilityList = new List<Proposal_Liability>();
                List<Proposal_Guarantor> guarantorList = new List<Proposal_Guarantor>();
                List<Proposal_Text> textList = new List<Proposal_Text>();
                List<Proposal_FDR> fdrList = new List<Proposal_FDR>();
                List<Proposal_OtherCost> otherCostList = new List<Proposal_OtherCost>();
                List<Proposal_Valuation_OtherCost> otherValuationCostList = new List<Proposal_Valuation_OtherCost>();
                List<Proposal_OverallAssessment> assesmentList = new List<Proposal_OverallAssessment>();
                List<Proposal_StressRate> stressRateList = new List<Proposal_StressRate>();
                List<Proposal_SecurityDetail> securityList = new List<Proposal_SecurityDetail>();
                List<Proposal_Signatory> signatoryList = new List<Proposal_Signatory>();
                List<ProposalCreditCard> creditCardList = new List<ProposalCreditCard>();
                var prev = GenService.GetById<Proposal>((long)dto.Id);
                #region ProposalCreditCard
                if (dto.ProposalCreditCards != null)
                {
                    foreach (var creditCardsDto in dto.ProposalCreditCards)
                    {
                        var creditCards = new ProposalCreditCard();
                        if (creditCardsDto.Id != null && creditCardsDto.Id > 0)
                        {
                            creditCards = GenService.GetById<ProposalCreditCard>((long)creditCardsDto.Id);
                            creditCardsDto.CreateDate = creditCards.CreateDate;
                            creditCardsDto.CreatedBy = creditCards.CreatedBy;
                            creditCardsDto.Status = creditCards.Status;
                            creditCardsDto.EditDate = DateTime.Now;
                            creditCardsDto.ProposalId = creditCards.ProposalId;
                            creditCardsDto.CreditCardId = creditCards.CreditCardId;
                            creditCards = Mapper.Map(creditCardsDto, creditCards);
                            creditCardList.Add(creditCards);
                        }
                        else
                        {
                            //text = new Proposal_Text();
                            creditCards = Mapper.Map<ProposalCreditCard>(creditCardsDto);
                            creditCards.Status = EntityStatus.Active;
                            creditCards.CreatedBy = userId;
                            creditCards.CreateDate = DateTime.Now;
                            creditCards.ProposalId = prev.Id;
                            creditCardList.Add(creditCards);
                            //GenService.Save(text);
                        }
                        //GenService.Save(income);
                    }
                }
                #endregion
                #region Project Address
                if (dto.ProjectAddress.IsChanged)
                {
                    if (dto.ProjectAddress.Id != null)
                    {
                        var address = GenService.GetById<Address>((long)dto.ProjectAddress.Id);
                        dto.ProjectAddress.CreateDate = address.CreateDate;
                        dto.ProjectAddress.CreatedBy = address.CreatedBy;
                        address = Mapper.Map(dto.ProjectAddress, address);
                        GenService.Save(address);
                        dto.ProjectAddressId = address.Id;
                    }
                    else
                    {
                        var projectAddress = Mapper.Map<Address>(dto.ProjectAddress);
                        GenService.Save(projectAddress);
                        dto.ProjectAddressId = projectAddress.Id;
                    }

                }
                else
                {
                    dto.ProjectAddressId = prev.ProjectAddressId;
                }
                #endregion
                #region Client Profile

                if (dto.ClientProfiles != null)
                {
                    foreach (var item in dto.ClientProfiles)
                    {
                        Proposal_ClientProfile client;
                        if (item.Id != null && item.Id > 0)
                        {
                            client = GenService.GetById<Proposal_ClientProfile>((long)item.Id);
                            //entity.Owners.Where(o => o.Id == item.Id).FirstOrDefault();//
                            if (item.OfficeAddress.IsChanged)
                            {
                                item.OfficeAddressId = EditAddress(item.OfficeAddress).Id;
                            }
                            else
                            {
                                item.OfficeAddressId = client.OfficeAddressId;
                            }
                            if (item.PermanentAddress.IsChanged)
                            {
                                item.PermanentAddressId = EditAddress(item.PermanentAddress).Id;
                            }
                            else
                            {
                                item.PermanentAddressId = client.PermanentAddressId;
                            }
                            if (item.PresentAddress.IsChanged)
                            {
                                item.PresentAddressId = EditAddress(item.PresentAddress).Id;
                            }
                            else
                            {
                                item.PresentAddressId = client.PresentAddressId;
                            }
                            item.ProposalId = client.ProposalId;
                            item.CreatedBy = client.CreatedBy;
                            item.CreateDate = client.CreateDate;
                            //item.Age = client.Age;
                            client = Mapper.Map(item, client);
                            client.EditDate = DateTime.Now;
                            client.EditedBy = userId;

                            //GenService.Save(client);
                            aClientProfile.Add(client);
                        }
                    }
                }

                #endregion
                #region Proposal_IncomeDto
                if (dto.Incomes != null)
                {
                    foreach (var incomeDto in dto.Incomes)
                    {
                        var income = GenService.GetById<Proposal_Income>((long)incomeDto.Id);
                        incomeDto.CreateDate = income.CreateDate;
                        incomeDto.CreatedBy = income.CreatedBy;
                        incomeDto.Status = income.Status;
                        incomeDto.EditDate = DateTime.Now;
                        incomeDto.ProposalId = income.ProposalId;
                        income = Mapper.Map(incomeDto, income);
                        incomeList.Add(income);
                        //GenService.Save(income);
                    }
                }
                #endregion
                #region Proposal_NetWorthDto
                if (dto.NetWorths != null)
                {
                    foreach (var networthDto in dto.NetWorths)
                    {
                        var netWorth = GenService.GetById<Proposal_NetWorth>((long)networthDto.Id);
                        networthDto.CreateDate = netWorth.CreateDate;
                        networthDto.CreatedBy = netWorth.CreatedBy;
                        networthDto.Status = netWorth.Status;
                        networthDto.EditDate = DateTime.Now;
                        networthDto.ProposalId = netWorth.ProposalId;
                        netWorth = Mapper.Map(networthDto, netWorth);
                        netWorthList.Add(netWorth);
                        //GenService.Save(income);
                    }
                }
                #endregion
                #region Proposal_CIBDto
                if (dto.CIBs != null)
                {
                    foreach (var cibDto in dto.CIBs)
                    {
                        var cib = GenService.GetById<Proposal_CIB>((long)cibDto.Id);
                        cibDto.CreateDate = cib.CreateDate;
                        cibDto.CreatedBy = cib.CreatedBy;
                        cibDto.Status = cib.Status;
                        cibDto.EditDate = DateTime.Now;
                        cibDto.ProposalId = cib.ProposalId;
                        cib = Mapper.Map(cibDto, cib);
                        cibList.Add(cib);
                        //GenService.Save(income);
                    }
                }
                #endregion
                #region Proposal_LiabilitiesDto
                if (dto.Liabilities != null)
                {
                    foreach (var lbtDto in dto.Liabilities)
                    {
                        var lbt = GenService.GetById<Proposal_Liability>((long)lbtDto.Id);
                        lbtDto.CreateDate = lbt.CreateDate;
                        lbtDto.CreatedBy = lbt.CreatedBy;
                        lbtDto.Status = lbt.Status;
                        lbtDto.EditDate = DateTime.Now;
                        lbtDto.ProposalId = lbt.ProposalId;
                        lbt = Mapper.Map(lbtDto, lbt);
                        liabilityList.Add(lbt);
                        //GenService.Save(income);
                    }
                }
                #endregion
                #region Proposal_GuarantorDto
                if (dto.Guarantors != null)
                {
                    foreach (var gurantorDto in dto.Guarantors)
                    {
                        var guarantor = new Proposal_Guarantor();
                        if (gurantorDto.Id != null && gurantorDto.Id > 0)
                        {
                            guarantor = GenService.GetById<Proposal_Guarantor>((long)gurantorDto.Id);
                            gurantorDto.CreateDate = guarantor.CreateDate;
                            gurantorDto.CreatedBy = guarantor.CreatedBy;
                            gurantorDto.Status = guarantor.Status;
                            gurantorDto.EditDate = DateTime.Now;
                            gurantorDto.ProposalId = guarantor.ProposalId; ;
                            guarantor = Mapper.Map(gurantorDto, guarantor);
                            guarantorList.Add(guarantor);
                        }
                        else
                        {
                            guarantor = Mapper.Map<Proposal_Guarantor>(gurantorDto);
                            guarantor.Status = EntityStatus.Active;
                            guarantor.CreatedBy = userId;
                            guarantor.CreateDate = DateTime.Now;
                            guarantor.ProposalId = prev.Id;
                            guarantorList.Add(guarantor);
                        }
                    }
                }
                if (dto.RemovedGuarantors != null)
                {
                    foreach (var item in dto.RemovedGuarantors)
                    {
                        var guarantor = GenService.GetById<Proposal_Guarantor>(item);
                        if (guarantor != null)
                        {
                            guarantor.Status = EntityStatus.Inactive;
                            guarantor.EditDate = DateTime.Now;
                            guarantor.EditedBy = userId;
                        }
                        guarantorList.Add(guarantor);
                    }
                }
                #endregion
                #region Proposal_TextDto
                if (dto.Texts != null)
                {
                    foreach (var textDto in dto.Texts)
                    {
                        var text = new Proposal_Text();
                        if (textDto.Id != null && textDto.Id > 0)
                        {
                            text = GenService.GetById<Proposal_Text>((long)textDto.Id);
                            textDto.CreateDate = text.CreateDate;
                            textDto.CreatedBy = text.CreatedBy;
                            textDto.Status = text.Status;
                            textDto.EditDate = DateTime.Now;
                            textDto.ProposalId = text.ProposalId;
                            text = Mapper.Map(textDto, text);
                            textList.Add(text);
                        }
                        else
                        {
                            //text = new Proposal_Text();
                            text = Mapper.Map<Proposal_Text>(textDto);
                            text.Status = EntityStatus.Active;
                            text.CreatedBy = userId;
                            text.CreateDate = DateTime.Now;
                            text.ProposalId = prev.Id;
                            textList.Add(text);
                            //GenService.Save(text);
                        }
                        //GenService.Save(income);
                    }
                }
                #endregion
                #region Proposal_FDRDto
                if (dto.FDRs != null)
                {
                    foreach (var fdrDto in dto.FDRs)
                    {
                        var fdr = GenService.GetById<Proposal_FDR>((long)fdrDto.Id);
                        fdrDto.CreateDate = fdr.CreateDate;
                        fdrDto.CreatedBy = fdr.CreatedBy;
                        fdrDto.Status = fdr.Status;
                        fdrDto.EditDate = DateTime.Now;
                        fdrDto.ProposalId = fdr.ProposalId;
                        fdr = Mapper.Map(fdrDto, fdr);
                        fdrList.Add(fdr);
                        //GenService.Save(income);
                    }
                }
                #endregion
                #region Proposal_OtherCostsDto
                if (dto.OtherCosts != null)
                {
                    foreach (var otherDto in dto.OtherCosts)
                    {
                        var otherCost = new Proposal_OtherCost();
                        if (otherDto.Id != null && otherDto.Id > 0)
                        {
                            otherCost = GenService.GetById<Proposal_OtherCost>((long)otherDto.Id);
                            otherDto.CreateDate = otherCost.CreateDate;
                            otherDto.CreatedBy = otherCost.CreatedBy;
                            otherDto.Status = otherCost.Status;
                            otherDto.EditDate = DateTime.Now;
                            otherDto.ProposalId = otherCost.ProposalId;
                            otherCost = Mapper.Map(otherDto, otherCost);
                            otherCostList.Add(otherCost);
                        }
                        else
                        {
                            //otherCost = new Proposal_OtherCosts();
                            otherCost = Mapper.Map<Proposal_OtherCost>(otherDto);
                            otherCost.Status = EntityStatus.Active;
                            otherCost.CreatedBy = userId;
                            otherCost.CreateDate = DateTime.Now;
                            otherCost.ProposalId = prev.Id;
                            otherCostList.Add(otherCost);
                            //GenService.Save(text);
                        }

                        //GenService.Save(income);
                    }
                }
                #endregion
                #region Proposal_OtherCostsDto
                if (dto.ValuationOtherCosts != null)
                {
                    foreach (var otherDto in dto.ValuationOtherCosts)
                    {
                        var otherCost = new Proposal_Valuation_OtherCost();
                        if (otherDto.Id != null && otherDto.Id > 0)
                        {
                            otherCost = GenService.GetById<Proposal_Valuation_OtherCost>((long)otherDto.Id);
                            otherDto.CreateDate = otherCost.CreateDate;
                            otherDto.CreatedBy = otherCost.CreatedBy;
                            otherDto.Status = otherCost.Status;
                            otherDto.EditDate = DateTime.Now;
                            otherDto.ProposalId = otherCost.ProposalId;
                            otherCost = Mapper.Map(otherDto, otherCost);
                            otherValuationCostList.Add(otherCost);
                        }
                        else
                        {
                            otherCost = Mapper.Map<Proposal_Valuation_OtherCost>(otherDto);
                            otherCost.Status = EntityStatus.Active;
                            otherCost.CreatedBy = userId;
                            otherCost.CreateDate = DateTime.Now;
                            otherCost.ProposalId = prev.Id;
                            otherValuationCostList.Add(otherCost);
                        }
                    }
                }
                #endregion
                #region Proposal_SecurityDetail

                if (dto.SecurityDetails != null)
                {
                    foreach (var securityDto in dto.SecurityDetails)
                    {
                        var security = new Proposal_SecurityDetail();
                        if (security.Id != null && security.Id > 0)
                        {
                            security = GenService.GetById<Proposal_SecurityDetail>((long)securityDto.Id);
                            securityDto.CreateDate = security.CreateDate;
                            securityDto.CreatedBy = security.CreatedBy;
                            securityDto.Status = security.Status;
                            securityDto.EditDate = DateTime.Now;
                            securityDto.ProposalId = security.ProposalId;
                            security = Mapper.Map(securityDto, security);
                            securityList.Add(security);
                        }
                        else
                        {
                            //text = new Proposal_Text();
                            security = Mapper.Map<Proposal_SecurityDetail>(securityDto);
                            security.Status = EntityStatus.Active;
                            security.CreatedBy = userId;
                            security.CreateDate = DateTime.Now;
                            security.ProposalId = prev.Id;
                            securityList.Add(security);
                            //GenService.Save(text);
                        }
                        //GenService.Save(income);
                    }
                }
                #endregion
                #region Proposal_OverallAssessmentDto
                if (dto.OverallAssessments != null)
                {
                    foreach (var assesmentsDto in dto.OverallAssessments)
                    {
                        var assesments = new Proposal_OverallAssessment();
                        if (assesmentsDto.Id != null && assesmentsDto.Id > 0)
                        {
                            assesments = GenService.GetById<Proposal_OverallAssessment>((long)assesmentsDto.Id);
                            assesmentsDto.CreateDate = assesments.CreateDate;
                            assesmentsDto.CreatedBy = assesments.CreatedBy;
                            assesmentsDto.Status = assesments.Status;
                            assesmentsDto.EditDate = DateTime.Now;
                            assesmentsDto.ProposalId = assesments.ProposalId;
                            assesments = Mapper.Map(assesmentsDto, assesments);
                            assesmentList.Add(assesments);
                        }
                        else
                        {
                            //assesments = new Proposal_OverallAssessment();
                            assesments = Mapper.Map<Proposal_OverallAssessment>(assesmentsDto);
                            assesments.Status = EntityStatus.Active;
                            assesments.CreatedBy = userId;
                            assesments.CreateDate = DateTime.Now;
                            assesments.ProposalId = prev.Id;
                            assesmentList.Add(assesments);
                            //GenService.Save(text);
                        }
                        //GenService.Save(income);
                    }
                }
                #endregion
                #region Proposal_StressRateDto
                if (dto.StressRates != null)
                {
                    foreach (var stressRateDto in dto.StressRates)
                    {
                        var stress = new Proposal_StressRate();
                        if (stressRateDto.Id != null && stressRateDto.Id > 0)
                        {
                            stress = GenService.GetById<Proposal_StressRate>((long)stressRateDto.Id);
                            stressRateDto.CreateDate = stress.CreateDate;
                            stressRateDto.CreatedBy = stress.CreatedBy;
                            stressRateDto.Status = stress.Status;
                            stressRateDto.EditDate = DateTime.Now;
                            stressRateDto.ProposalId = stress.ProposalId;
                            stress = Mapper.Map(stressRateDto, stress);
                            stressRateList.Add(stress);
                        }
                        else
                        {
                            //assesments = new Proposal_OverallAssessment();
                            stress = Mapper.Map<Proposal_StressRate>(stressRateDto);
                            stress.Status = EntityStatus.Active;
                            stress.CreatedBy = userId;
                            stress.CreateDate = DateTime.Now;
                            stress.ProposalId = prev.Id;
                            stressRateList.Add(stress);
                            //GenService.Save(text);
                        }
                    }
                }
                #endregion
                #region Removed Property
                if (dto.RemovedAssetBackup != null)
                {
                    foreach (var item in dto.RemovedAssetBackup)
                    {
                        var text = GenService.GetById<Proposal_Text>(item);
                        if (text != null)
                        {
                            text.Status = EntityStatus.Inactive;
                            text.EditDate = DateTime.Now;
                            text.EditedBy = userId;
                        }
                        GenService.Save(text);
                    }
                }
                if (dto.RemovedOtherCosts != null)
                {
                    foreach (var item in dto.RemovedOtherCosts)
                    {
                        var otherCost = GenService.GetById<Proposal_OtherCost>(item);
                        if (otherCost != null)
                        {
                            otherCost.Status = EntityStatus.Inactive;
                            otherCost.EditDate = DateTime.Now;
                            otherCost.EditedBy = userId;
                        }
                        GenService.Save(otherCost);
                    }
                }
                if (dto.RemovedValuationOtherCosts != null)
                {
                    foreach (var item in dto.RemovedValuationOtherCosts)
                    {
                        var otherCost = GenService.GetById<Proposal_Valuation_OtherCost>(item);
                        if (otherCost != null)
                        {
                            otherCost.Status = EntityStatus.Inactive;
                            otherCost.EditDate = DateTime.Now;
                            otherCost.EditedBy = userId;
                        }
                        GenService.Save(otherCost);
                    }
                }
                if (dto.RemovedOverallAssessments != null)
                {
                    foreach (var item in dto.RemovedOverallAssessments)
                    {
                        var assessment = GenService.GetById<Proposal_OverallAssessment>(item);
                        if (assessment != null)
                        {
                            assessment.Status = EntityStatus.Inactive;
                            assessment.EditDate = DateTime.Now;
                            assessment.EditedBy = userId;
                        }
                        GenService.Save(assessment);
                    }
                }
                if (dto.RemovedSignatories != null)
                {
                    foreach (var item in dto.RemovedSignatories)
                    {
                        var signatory = GenService.GetById<Proposal_Signatory>(item);
                        if (signatory != null)
                        {
                            signatory.Status = EntityStatus.Inactive;
                            signatory.EditDate = DateTime.Now;
                            signatory.EditedBy = userId;
                        }
                        GenService.Save(signatory);
                    }
                }
                if (dto.RemovedCreditCards != null)
                {
                    foreach (var item in dto.RemovedCreditCards)
                    {
                        var crdCard = GenService.GetById<ProposalCreditCard>(item);
                        if (crdCard != null)
                        {
                            crdCard.Status = EntityStatus.Inactive;
                            crdCard.EditDate = DateTime.Now;
                            crdCard.EditedBy = userId;
                        }
                        GenService.Save(crdCard);
                    }
                }
                //if(dto.removed)
                #endregion

                #region Proposal_Signatory

                if (dto.Signatories != null)
                {
                    foreach (var signatorDto in dto.Signatories)
                    {
                        var signatory = new Proposal_Signatory();
                        if (signatorDto.Id != null && signatorDto.Id > 0)
                        {
                            signatory = GenService.GetById<Proposal_Signatory>((long)signatorDto.Id);
                            signatorDto.CreateDate = signatory.CreateDate;
                            signatorDto.CreatedBy = signatory.CreatedBy;
                            signatorDto.Status = signatory.Status;
                            signatorDto.EditDate = DateTime.Now;
                            signatorDto.ProposalId = signatory.ProposalId;
                            signatory = Mapper.Map(signatorDto, signatory);
                            signatoryList.Add(signatory);
                        }
                        else
                        {
                            //assesments = new Proposal_OverallAssessment();
                            signatory = Mapper.Map<Proposal_Signatory>(signatorDto);
                            signatory.Status = EntityStatus.Active;
                            signatory.CreatedBy = userId;
                            signatory.CreateDate = DateTime.Now;
                            signatory.ProposalId = prev.Id;
                            signatoryList.Add(signatory);
                            //GenService.Save(text);
                        }
                    }
                }
                #endregion

                dto.ApplicationId = prev.ApplicationId;
                dto.CreditMemoNo = prev.CreditMemoNo;
                dto.ApplicationNo = prev.ApplicationNo;
                dto.PDCAccountNo = prev.PDCAccountNo;
                dto.CreateDate = prev.CreateDate;
                dto.CreatedBy = prev.CreatedBy;
                dto.Status = prev.Status;
                dto.EditDate = DateTime.Now;
                Mapper.Map(dto, prev);
                GenService.Save(prev);

                GenService.Save(aClientProfile);
                GenService.Save(incomeList);
                GenService.Save(netWorthList);
                GenService.Save(cibList);
                GenService.Save(liabilityList);
                GenService.Save(guarantorList);
                GenService.Save(textList);
                GenService.Save(fdrList);
                GenService.Save(otherCostList);
                GenService.Save(assesmentList);
                GenService.Save(stressRateList);
                GenService.Save(securityList);
                GenService.Save(signatoryList);
                GenService.Save(creditCardList);
                GenService.Save(otherValuationCostList);
                response.Id = prev.Id;
                response.Success = true;
                response.Message = "Proposal Edited Successfully";
            }
            #endregion
            #region Add Proposal
            else
            {
                var prev =
                    GenService.GetAll<Proposal>()
                        .Where(r => r.ApplicationId == dto.ApplicationId && r.Status == EntityStatus.Active)
                        .ToList();
                prev.ForEach(c => { c.Status = EntityStatus.Inactive; });
                GenService.Save(prev);
                var data = Mapper.Map<Proposal>(dto);
                var projectAddress = Mapper.Map<Address>(dto.ProjectAddress);
                if (projectAddress.CountryId != null)
                {
                    GenService.Save(projectAddress);
                    data.ProjectAddressId = projectAddress.Id;
                }
                if (dto.ClientProfiles != null && dto.ClientProfiles.Count > 0)
                {
                    data.ClientProfiles = new List<Proposal_ClientProfile>();
                    foreach (var clientProfile in dto.ClientProfiles)
                    {
                        var officeAddress = Mapper.Map<Address>(clientProfile.OfficeAddress);
                        if (officeAddress.CountryId != null)
                        {
                            GenService.Save(officeAddress);
                            clientProfile.OfficeAddressId = officeAddress.Id;
                        }
                        var presentAddress = Mapper.Map<Address>(clientProfile.PresentAddress);
                        if (presentAddress.CountryId != null)
                        {
                            GenService.Save(presentAddress);
                            clientProfile.PresentAddressId = presentAddress.Id;
                        }
                        var permanentAddress = Mapper.Map<Address>(clientProfile.PermanentAddress);
                        if (permanentAddress.CountryId != null)
                        {
                            GenService.Save(permanentAddress);
                            clientProfile.PermanentAddressId = permanentAddress.Id;
                        }
                        data.ClientProfiles.Add(Mapper.Map<Proposal_ClientProfile>(clientProfile));
                    }
                    //data.ClientProfile = Mapper.Map<List<Proposal_ClientProfile>>(dto.ClientProfile);
                    data.ClientProfiles = data.ClientProfiles.Select(d =>
                    {
                        d.Status = EntityStatus.Active;
                        return d;
                    }).ToList();
                }
                if (dto.Incomes != null && dto.Incomes.Count > 0)
                {
                    data.Incomes = new List<Proposal_Income>();
                    data.Incomes = Mapper.Map<List<Proposal_Income>>(dto.Incomes);
                    data.Incomes = data.Incomes.Select(d =>
                    {
                        d.Status = EntityStatus.Active;
                        return d;
                    }).ToList();
                }
                if (dto.ProposalCreditCards != null && dto.ProposalCreditCards.Count > 0)
                {
                    data.ProposalCreditCards = new List<ProposalCreditCard>();
                    data.ProposalCreditCards = Mapper.Map<List<ProposalCreditCard>>(dto.ProposalCreditCards);
                    data.ProposalCreditCards = data.ProposalCreditCards.Select(d =>
                    {
                        d.Status = EntityStatus.Active;
                        return d;
                    }).ToList();
                }
                if (dto.SecurityDetails != null && dto.SecurityDetails.Count > 0)
                {
                    data.SecurityDetails = new List<Proposal_SecurityDetail>();
                    data.SecurityDetails = Mapper.Map<List<Proposal_SecurityDetail>>(dto.SecurityDetails);
                    data.SecurityDetails = data.SecurityDetails.Select(d =>
                    {
                        d.Status = EntityStatus.Active;
                        return d;
                    }).ToList();
                }
                if (dto.NetWorths != null && dto.NetWorths.Count > 0)
                {
                    data.NetWorths = new List<Proposal_NetWorth>();
                    data.NetWorths = Mapper.Map<List<Proposal_NetWorth>>(dto.NetWorths);
                    data.NetWorths = data.NetWorths.Select(d =>
                    {
                        d.Status = EntityStatus.Active;
                        return d;
                    }).ToList();
                }
                if (dto.CIBs != null && dto.CIBs.Count > 0)
                {
                    data.CIBs = new List<Proposal_CIB>();
                    data.CIBs = Mapper.Map<List<Proposal_CIB>>(dto.CIBs);
                    data.CIBs = data.CIBs.Select(d =>
                    {
                        d.Status = EntityStatus.Active;
                        return d;
                    }).ToList();
                }
                if (dto.Liabilities != null && dto.Liabilities.Count > 0)
                {
                    data.Liabilities = new List<Proposal_Liability>();
                    data.Liabilities = Mapper.Map<List<Proposal_Liability>>(dto.Liabilities);
                    data.Liabilities = data.Liabilities.Select(d =>
                    {
                        d.Status = EntityStatus.Active;
                        return d;
                    }).ToList();
                }
                if (dto.Guarantors != null && dto.Guarantors.Count > 0)
                {
                    data.Guarantors = new List<Proposal_Guarantor>();
                    data.Guarantors = Mapper.Map<List<Proposal_Guarantor>>(dto.Guarantors);
                    data.Guarantors = data.Guarantors.Select(d =>
                    {
                        d.Status = EntityStatus.Active;
                        return d;
                    }).ToList();
                }
                if (dto.Texts != null && dto.Texts.Count > 0)
                {
                    data.Texts = new List<Proposal_Text>();
                    data.Texts = Mapper.Map<List<Proposal_Text>>(dto.Texts);
                    data.Texts = data.Texts.Select(d =>
                    {
                        d.Status = EntityStatus.Active;
                        return d;
                    }).ToList();
                }
                if (dto.FDRs != null && dto.FDRs.Count > 0)
                {
                    data.FDRs = new List<Proposal_FDR>();
                    data.FDRs = Mapper.Map<List<Proposal_FDR>>(dto.FDRs);
                    data.FDRs = data.FDRs.Select(d =>
                    {
                        d.Status = EntityStatus.Active;
                        return d;
                    }).ToList();
                }
                if (dto.OtherCosts != null && dto.OtherCosts.Count > 0)
                {
                    data.OtherCosts = new List<Proposal_OtherCost>();
                    data.OtherCosts = Mapper.Map<List<Proposal_OtherCost>>(dto.OtherCosts);
                    data.OtherCosts = data.OtherCosts.Select(d =>
                    {
                        d.Status = EntityStatus.Active;
                        return d;
                    }).ToList();
                }
                if (dto.ValuationOtherCosts != null && dto.ValuationOtherCosts.Count > 0)
                {
                    data.ValuationOtherCosts = new List<Proposal_Valuation_OtherCost>();
                    data.ValuationOtherCosts = Mapper.Map<List<Proposal_Valuation_OtherCost>>(dto.ValuationOtherCosts);
                    data.ValuationOtherCosts = data.ValuationOtherCosts.Select(d =>
                    {
                        d.Status = EntityStatus.Active;
                        return d;
                    }).ToList();
                }
                if (dto.OverallAssessments != null && dto.OverallAssessments.Count > 0)
                {
                    data.OverallAssessments = new List<Proposal_OverallAssessment>();
                    data.OverallAssessments = Mapper.Map<List<Proposal_OverallAssessment>>(dto.OverallAssessments);
                    data.OverallAssessments = data.OverallAssessments.Select(d =>
                    {
                        d.Status = EntityStatus.Active;
                        return d;
                    }).ToList();
                }
                if (dto.StressRates != null && dto.StressRates.Count > 0)
                {
                    data.StressRates = new List<Proposal_StressRate>();
                    data.StressRates = Mapper.Map<List<Proposal_StressRate>>(dto.StressRates);
                    data.StressRates = data.StressRates.Select(d =>
                    {
                        d.Status = EntityStatus.Active;
                        return d;
                    }).ToList();
                }
                if (dto.Signatories != null && dto.Signatories.Count > 0)
                {
                    data.Signatories = new List<Proposal_Signatory>();
                    data.Signatories = Mapper.Map<List<Proposal_Signatory>>(dto.Signatories);
                    data.Signatories = data.Signatories.Select(d =>
                    {
                        d.Status = EntityStatus.Active;
                        return d;
                    }).ToList();
                }
                if (dto.CRMReceiveDate == null)
                {
                    data.CRMReceiveDate = DateTime.Now;
                }
                data.CreditMemoNo = _sequencer.GetUpdatedCreditMemoNo();
                data.Status = EntityStatus.Active;
                data.CreateDate = DateTime.Now;
                GenService.Save(data);

                response.Id = data.Id;
                response.Success = true;
                response.Message = "Proposal Saved Successfully";
            }
            #endregion

            return response;
        }

        public List<ProposalDto> LoadProposalById(long ProposalId)
        {
            return Mapper.Map<List<ProposalDto>>(GenService.GetAll<Proposal>().Where(p => p.Id == ProposalId).ToList());
        }
        //public List<ApprovalAuthorityGroupDto> GetApprovalAuthorityGroup(long productId, decimal fromAmount, decimal toAmount, MemoType memoType)
        //{
        //    var result = GenService.GetAll<ApprovalAuthoritySignatory>().Where(a => a.ProductId == productId && a.AmountFrom == fromAmount && a.AmountTo == toAmount && a.MemoType == memoType).Select(a => a.AuthorityGroup).ToList();
        //    return Mapper.Map<List<ApprovalAuthorityGroupDto>>(result);
        //}

        //public List<Proposal_SignatoryDto> GetAuthority(decimal loanAmount)//, MemoType memoType
        //{
        //    var authoritySignatory = GenService.GetAll<ApprovalAuthoritySignatory>().Where(p => p.MemoType == MemoType.Credit_Memo && p.Status == EntityStatus.Active).OrderByDescending(r => r.Id).FirstOrDefault();
        //    if (authoritySignatory != null)
        //    {
        //        var signatories =
        //            authoritySignatory.AuthorityGroup.AuthorityGroupDetails.Select(
        //                r => new Proposal_SignatoryDto
        //                {
        //                    ApprovalAuthoritySignatoryName = r.ApprovalAuthorityGroup.Name,
        //                    Name = r.OfficeDesignationSetting.Name,
        //                    SignatoryId = r.Id
        //                }).ToList();
        //        return signatories;
        //    }
        //    return null;
        //}

        public ResponseDto SaveOfferLetter(OfferLetterDto dto, long userId)
        {
            ResponseDto response = new ResponseDto();
            try
            {
                if (dto.ProposalId < 0)
                {
                    response.Message = "Proposal don't exist";
                    return response;
                }
                #region Edit
                if (dto.Id > 0)
                {
                    var prev = GenService.GetById<OfferLetter>((long)dto.Id);
                    if (dto.OfferLetterTexts != null)
                    {
                        foreach (var item in dto.OfferLetterTexts)
                        {
                            OfferLetterText offerLetter;
                            if (item.Id != null && item.Id > 0)
                            {
                                offerLetter = GenService.GetById<OfferLetterText>((long)item.Id);
                                if (item.Status == null)
                                    item.Status = offerLetter.Status;
                                item.CreateDate = offerLetter.CreateDate;
                                item.CreatedBy = offerLetter.CreatedBy;
                                item.OfferLetterId = offerLetter.OfferLetterId;
                                item.EditDate = DateTime.Now;
                                item.EditedBy = userId;
                                item.Status = EntityStatus.Active;
                                Mapper.Map(item, offerLetter);
                                GenService.Save(offerLetter);

                            }
                            else
                            {
                                offerLetter = new OfferLetterText();
                                item.CreateDate = DateTime.Now;
                                item.CreatedBy = userId;
                                item.Status = EntityStatus.Active;
                                item.OfferLetterId = prev.Id;
                                offerLetter = Mapper.Map<OfferLetterText>(item);
                                GenService.Save(offerLetter);

                            }

                        }
                    }
                    if (dto.RemovedOfferLetterTexts != null)
                    {
                        foreach (var item in dto.RemovedOfferLetterTexts)
                        {
                            var text = GenService.GetById<OfferLetterText>(item);
                            if (text != null)
                            {
                                text.Status = EntityStatus.Inactive;
                                text.EditDate = DateTime.Now;
                                text.EditedBy = userId;
                            }
                            GenService.Save(text);
                        }
                    }
                    dto.CreateDate = prev.CreateDate;
                    dto.CreatedBy = prev.CreatedBy;
                    dto.Status = prev.Status;
                    dto.OfferLetterNo = prev.OfferLetterNo;
                    dto.ProposalId = prev.ProposalId;
                    dto.EditDate = DateTime.Now;
                    prev = Mapper.Map(dto, prev);
                    GenService.Save(prev);
                    N.CreateNotificationForService(NotificationType.ApplicationOfferLetterIssued, prev.Id);
                    response.Id = prev.Id;
                    response.Success = true;
                    response.Message = "Offer Letter Edited Successfully";
                }
                #endregion
                #region Add
                else
                {
                    var data = Mapper.Map<OfferLetter>(dto);
                    data.EditDate = DateTime.Now;
                    data.Status = EntityStatus.Active;
                    data.CreateDate = DateTime.Now;
                    if (dto.OfferLetterTexts != null && dto.OfferLetterTexts.Count > 0)
                    {
                        data.OfferLetterTexts = Mapper.Map<List<OfferLetterText>>(dto.OfferLetterTexts);
                        foreach (var item in data.OfferLetterTexts)
                        {
                            item.CreateDate = DateTime.Now;
                            item.CreatedBy = userId;
                            item.Status = EntityStatus.Active;
                        }
                    }
                    data.OfferLetterNo = _sequencer.GetUpdatedOfferLetterNo();
                    GenService.Save(data);
                    N.CreateNotificationForService(NotificationType.ApplicationOfferLetterIssued, data.Id);
                    response.Id = data.Id;
                    response.Success = true;
                    response.Message = "Offer Letter Saved Successfully";
                }
                #endregion

            }
            catch (Exception ex)
            {
                response.Message = "Error Message : " + ex;
            }
            return response;
        }

        public OfferLetterDto LoadOfferLetter(long? proposalId, long? id)
        {
            var offerDto = new OfferLetterDto();
            if (proposalId > 0 && (id == 0 || id == null))
            {
                offerDto.ProposalId = (long)proposalId;
                var proposal = GenService.GetById<Proposal>((long)proposalId);
                if (proposal != null)
                {
                    string[] earlySettlementAuto = { "2% on the outstanding loan amount plus VAT for full payment in advance.", "2% on the prepaid amount plus VAT for partial payment in the first year.", "1% on the prepaid amount plus VAT in case of partial payment after one year." };
                    string[] earlySettlementHome = { "2% + VAT of outstanding if settled within first 5 years.", "1%+ VAT of outstanding if settled after 5 years." };
                    string[] earlySettlementPersonal = { "An unwinding fee will be charged at 2% on the prepaid principal amount." };

                    string[] partialHome = { "Minimum pre-payment amount is BDT. 100,000.00 (One Hundred Thousand) only.", "Pre-payment is allowed any time but first pre-payment will be allowed only after completion of 06 (six) successful installments from the full disbursement." };

                    string[] docAuto = { "Demand Promissory Note and Letter of Continuation.", "Terms & Conditions duly signed by the Borrower.", "Letter of Guarantee of spouse/any other close relative of the Borrower acceptable to IPDC.", "Letter of Authorization to repossess and sale of the Vehicle.", "Quotation from vendor.", "Undertaking duly executed by the Borrower.", "60 Post Dated Cheques (PDCs) for monthly installment payment and One Security Cheque covering the entire facility", "Money receipt for payment of borrower’s participation." };
                    string[] disbursmentAuto = { "Authenticated photocopy of ‘Car Registration Certificate’, ‘Fitness Certificate’ and ‘Tax Token’ after registration of the vehicle.", "Comprehensive First Party Insurance (original/copy) from any reputed insurance company acceptable to IPDC.", "Duly signed delivery challan.", "Bill/invoice from vendor." };
                    string[] docpersonal = { "Terms and conditions duly signed by the Borrower.", "Demand promissory note and Letter of Continuation.", "Security cheque covering the entire loan amount." };
                    if (proposal.FacilityType != null)
                    {
                        offerDto.FacilityType = (ProposalFacilityType)proposal.FacilityType;
                        offerDto.FacilityTypeName = offerDto.FacilityType.ToString();
                        if (offerDto.FacilityType == ProposalFacilityType.Auto_Loan)
                        {
                            offerDto.OfferLetterTexts = earlySettlementAuto.Select(x => new OfferLetterTextDto
                            {
                                OfferTextType = OfferTextType.EarlySettlement,
                                Text = x
                            }).ToList();
                            var condition = docAuto.Select(x => new OfferLetterTextDto
                            {
                                OfferTextType = OfferTextType.Documentation,
                                PrinterFiltering = PrinterFiltering.PO,
                                Text = x
                            }).ToList();
                            offerDto.OfferLetterTexts.AddRange(condition);
                            var disbursment = disbursmentAuto.Select(x => new OfferLetterTextDto
                            {
                                OfferTextType = OfferTextType.Documentation,
                                PrinterFiltering = PrinterFiltering.Disbursment,
                                Text = x
                            }).ToList();
                            offerDto.OfferLetterTexts.AddRange(disbursment);
                        }
                        if (offerDto.FacilityType == ProposalFacilityType.Home_Loan)
                        {
                            offerDto.OfferLetterTexts = earlySettlementHome.Select(x => new OfferLetterTextDto
                            {
                                OfferTextType = OfferTextType.EarlySettlement,
                                Text = x
                            }).ToList();
                            var condition = partialHome.Select(x => new OfferLetterTextDto
                            {
                                OfferTextType = OfferTextType.PartialPayment,
                                Text = x
                            }).ToList();
                            offerDto.OfferLetterTexts.AddRange(condition);
                        }
                        if (offerDto.FacilityType == ProposalFacilityType.Personal_Loan)
                        {
                            offerDto.OfferLetterTexts = earlySettlementPersonal.Select(x => new OfferLetterTextDto
                            {
                                OfferTextType = OfferTextType.EarlySettlement,
                                Text = x
                            }).ToList();

                            var doctext = docpersonal.Select(x => new OfferLetterTextDto
                            {
                                OfferTextType = OfferTextType.Documentation,
                                PrinterFiltering = PrinterFiltering.Disbursment,
                                Text = x
                            }).ToList();
                            offerDto.OfferLetterTexts.AddRange(doctext);
                        }
                        if (proposal.FacilityType == ProposalFacilityType.RLS)
                        {
                            offerDto.ExpiryDate = proposal.ExpiryDate;
                            offerDto.TotalAmountForFdr = proposal.TotalAmountForFdr;
                            offerDto.WeightedAverageRate = proposal.WeightedAverageRate;
                            offerDto.Spread = proposal.Spread;
                            offerDto.LoantoDepositRatio = proposal.LoantoDepositRatio;
                        }
                        if (offerDto.FacilityType == ProposalFacilityType.Home_Loan)
                        {
                            var proposalText = proposal.Texts.Where(r => r.Type == ProposalTextTypes.FinalDisbursementConditions && r.Status == EntityStatus.Active).ToList();
                            offerDto.DisbursmentCondition = proposalText.Select(x => new OfferLetterTextDto
                            {
                                OfferTextType = OfferTextType.DisbursmentCondition,
                                PrinterFiltering = x.PrinterFiltering,
                                Text = x.Text
                            }).ToList();
                        }
                        if (offerDto.FacilityType == ProposalFacilityType.Auto_Loan ||
                            offerDto.FacilityType == ProposalFacilityType.Personal_Loan)
                        {
                            var proposalText = proposal.Texts.Where(r => r.Type == ProposalTextTypes.ModeOfDisbursement && r.Status == EntityStatus.Active && r.IsPrintable != null && r.IsPrintable == true).ToList();
                            offerDto.ModeOfDisbursment = proposalText.Select(x => new OfferLetterTextDto
                            {
                                OfferTextType = OfferTextType.DisbursmentMode,
                                PrinterFiltering = x.PrinterFiltering,
                                Text = x.Text
                            }).ToList();
                        }
                    }
                }
            }
            else if (id > 0)
            {
                var offerLetter = GenService.GetById<OfferLetter>((long)id);
                offerDto = Mapper.Map<OfferLetterDto>(offerLetter);
                offerDto.OfferLetterTexts.RemoveAll(f => f.Status != EntityStatus.Active);
            }
            return offerDto;
        }

        public long GetOfferLetterId(long proposalId)
        {
            if (proposalId > 0)
            {
                var OfferLetter = GenService.GetAll<OfferLetter>().FirstOrDefault(o => o.ProposalId == proposalId);
                if (OfferLetter != null)
                    return OfferLetter.Id;
            }
            return 0;
        }

        public IPagedList<ProposalDto> GetCreditMemosForApproval(int pageSize, int pageCount, string searchString, long userId)
        {
            var idList = GenService.GetAll<OfferLetter>().Where(o => o.Status == EntityStatus.Active).Select(o => o.ProposalId).ToList();
            var allApp = GenService.GetAll<Proposal>().Where(s => s.Status == EntityStatus.Active && !idList.Contains(s.Id) && s.CreatedBy == userId).Select(s => new ProposalDto()
            {
                Id = s.Id,
                ApplicationNo = s.ApplicationNo,
                AccountTitle = s.Application.AccountTitle,
                ApplicationReceiveDate = s.ApplicationReceiveDate,
                ProposalDate = s.ProposalDate,
                IsApproved = s.IsApproved
            });
            var temp = allApp.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);

            return temp;
        }

        public ResponseDto CreditMemoApproval(long ProposalId, bool ApprovalStatus, long userId)
        {
            var response = new ResponseDto();
            var proposal = GenService.GetById<Proposal>(ProposalId);
            if (proposal != null)
            {
                proposal.IsApproved = ApprovalStatus;
                proposal.EditDate = DateTime.Now;
                proposal.EditedBy = userId;

                response.Message = "Credit Memo - " + proposal.CreditMemoNo + " successfully approved.";
                response.Success = true;

                #region application log

                var app = GenService.GetById<Application>(proposal.ApplicationId);
                var log = new ApplicationLog();
                log.Activity = Activity.Submit;
                log.AppIdRef = proposal.ApplicationId;
                log.ApplicationId = proposal.ApplicationId;
                log.AppType = AppType.Application;
                log.CreateDate = DateTime.Now;
                log.CreatedBy = userId;
                log.FromUserId = userId;
                log.FromStage = app.ApplicationStage;
                log.Status = EntityStatus.Active;
                log.ToStage = ApplicationStage.SentToOperations;

                app.ApplicationStage = ApplicationStage.SentToOperations;
                app.CurrentHolding = null;
                app.CurrentHoldingEmpId = null;

                #endregion

                using (var tran = new TransactionScope())
                {
                    try
                    {
                        GenService.Save(log);
                        GenService.Save(app);
                        GenService.Save(proposal);

                        tran.Complete();
                    }
                    catch (Exception ex)
                    {
                        tran.Dispose();
                        response.Message = "Approval failed";
                    }
                }

                return response;
            }
            response.Message = "Credit memo not found.";

            return response;
        }

        public IPagedList<OfferLetterDto> GetOfferLettersForApprovalCRM(int pageSize, int pageCount, string searchString)
        {
            var allApp = GenService.GetAll<OfferLetter>().Where(s => s.Status == EntityStatus.Active && s.CRMIsApproved != true).Select(s => new OfferLetterDto()
            {
                Id = s.Id,
                ApplicationDate = s.Proposal.ApplicationReceiveDate,
                ApplicationNo = s.Proposal.ApplicationNo,
                ProposalNo = s.Proposal.CreditMemoNo,
                OfferLetterNo = s.OfferLetterNo,
                ApplicationTitle = s.Proposal.Application.AccountTitle,
                OfferLetterDate = s.OfferLetterDate,
                CRMIsApproved = s.CRMIsApproved
            });
            var temp = allApp.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);

            return temp;
        }

        public IPagedList<OfferLetterDto> GetOfferLettersForApprovalOPS(int pageSize, int pageCount, string searchString)
        {
            var allApp = GenService.GetAll<OfferLetter>().Where(s => s.Status == EntityStatus.Active && s.CRMIsApproved == true && (s.CUSIsApproved != true || s.OPSIsApproved != true)).Select(s => new OfferLetterDto()
            {
                Id = s.Id,
                ApplicationDate = s.Proposal.ApplicationReceiveDate,
                ApplicationNo = s.Proposal.ApplicationNo,
                ProposalNo = s.Proposal.CreditMemoNo,
                OfferLetterNo = s.OfferLetterNo,
                ApplicationTitle = s.Proposal.Application.AccountTitle,
                OfferLetterDate = s.OfferLetterDate,
                CUSIsApproved = s.CUSIsApproved,
                OPSIsApproved = s.OPSIsApproved
            });
            var temp = allApp.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);

            return temp;
        }
        public ResponseDto OfferLetterApproval(long OfferLetterId, bool ApprovalStatus, long userId, int ApprovalType)
        {
            var response = new ResponseDto();
            var offerLetter = GenService.GetById<OfferLetter>(OfferLetterId);
            string approvedBy = "";
            if (offerLetter != null)
            {
                if (ApprovalType == 1)
                {
                    offerLetter.CRMIsApproved = ApprovalStatus;
                    offerLetter.CRMApprovalDate = DateTime.Now;
                    offerLetter.EditDate = offerLetter.CRMApprovalDate;
                    approvedBy = " by CRM";
                }
                else if (ApprovalType == 2)
                {
                    offerLetter.OPSIsApproved = ApprovalStatus;
                    offerLetter.OPSApprovalDate = DateTime.Now;
                    offerLetter.EditDate = offerLetter.OPSApprovalDate;
                    approvedBy = " by Operations";
                }
                else if (ApprovalType == 3)
                {
                    offerLetter.CUSIsApproved = ApprovalStatus;
                    offerLetter.CUSApprovalDate = DateTime.Now;
                    offerLetter.EditDate = offerLetter.CUSApprovalDate;
                    approvedBy = " by Customer";
                }

                offerLetter.EditedBy = userId;
                GenService.Save(offerLetter);
                response.Message = "Offer Letter - " + offerLetter.OfferLetterNo + " successfully approved" + approvedBy + ".";
                response.Success = true;
                return response;
            }
            response.Message = "Offer Letter not found.";

            return response;
        }
        public OfferLetterDto AutoLoanOfferLetterReport(long offerLetterId, long? proposalId)
        {
            if (offerLetterId > 0)
            {
                var offerLetter = GenService.GetById<OfferLetter>(offerLetterId);
                var data = Mapper.Map<OfferLetterDto>(offerLetter);
                data.ProcessingFeeRate = offerLetter.Proposal != null
                    ? offerLetter.Proposal.ProcessingFeeAndDocChargesPercentage > 0 ? offerLetter.Proposal.ProcessingFeeAndDocChargesPercentage : 0 : 0;
                data.ProcessingFeeAmount = offerLetter.Proposal != null
                  ? offerLetter.Proposal.ProcessingFeeAndDocChargesAmount > 0 ? offerLetter.Proposal.ProcessingFeeAndDocChargesAmount : 0 : 0;
                if (data.ProcessingFeeRate > 0 && data.ProcessingFeeAmount == 0)
                {
                    data.ProcessingFeeAmount = ((data.ProposedLoanAmount * data.ProcessingFeeRate) / 100) * (decimal)(1.15);
                }
                if (data.ProcessingFeeAmount > 0 && data.ProcessingFeeRate == 0)
                {
                    data.ProcessingFeeRate = (data.ProcessingFeeAmount / data.ProposedLoanAmount) / 100;
                    data.ProcessingFeeAmount = data.ProcessingFeeAmount * (decimal)(1.15);
                }
                data.BorrowersContribution = data.CarPrice - data.ProposedLoanAmount;
                if (offerLetter.Proposal.Application.LoanApplication.LoanPrimarySecurityType == LoanPrimarySecurityType.VehiclePrimarySecurity)
                {
                    if (offerLetter.Proposal.Application.LoanApplication.RefId > 0)
                    {
                        var vehicle = GenService.GetById<VehiclePrimarySecurity>((long)offerLetter.Proposal.Application.LoanApplication.RefId);
                        if (vehicle != null)
                        {
                            data.VehicleName = vehicle.VehicleName;
                            data.CC = vehicle.CC;
                            data.Colour = vehicle.Colour;
                            data.ChassisNo = vehicle.ChassisNo;
                            data.EngineNo = vehicle.EngineNo;

                        }
                    }
                }
                if (offerLetter.Proposal != null && offerLetter.FacilityType == ProposalFacilityType.Personal_Loan)
                {
                    data.PurposeName = offerLetter.Proposal.LoanPurpose;
                }
                if (offerLetter.Proposal != null && offerLetter.FacilityType == ProposalFacilityType.RLS)
                {
                    data.ExpiryDate = offerLetter.Proposal.ExpiryDate;
                    data.TotalAmountForFdr = offerLetter.Proposal.TotalAmountForFdr;
                    data.WeightedAverageRate = offerLetter.Proposal.WeightedAverageRate;
                    data.LoantoDepositRatio = offerLetter.Proposal.LoantoDepositRatio;
                }
                Address presentAddress = new Address();
                if (offerLetter.Proposal.Application.ContactPersonAddressType == 1)//1=presemt, 2= permanent,3=work, 4=group address
                {
                    presentAddress = offerLetter.Proposal.Application.CIFList.FirstOrDefault(
                      r => r.ApplicantRole == ApplicantRole.Primary).CIF_Personal.ResidenceAddress;
                }
                else if (offerLetter.Proposal.Application.ContactPersonAddressType == 2)
                {
                    presentAddress = offerLetter.Proposal.Application.CIFList.FirstOrDefault(
                      r => r.ApplicantRole == ApplicantRole.Primary).CIF_Personal.PermanentAddress;
                }
                else if (offerLetter.Proposal.Application.ContactPersonAddressType == 3)
                {
                    presentAddress = offerLetter.Proposal.Application.CIFList.FirstOrDefault(
                      r => r.ApplicantRole == ApplicantRole.Primary).CIF_Personal.Occupation.OfficeAddress;
                }
                else if (offerLetter.Proposal.Application.ContactPersonAddressType == 3)
                {
                    presentAddress = offerLetter.Proposal.Application.GroupAddress;
                }
                data.PresentAddressText = presentAddress != null ?
                    ((!string.IsNullOrEmpty(presentAddress.AddressLine1) ? presentAddress.AddressLine1 + ", " : "") +
                    (!string.IsNullOrEmpty(presentAddress.AddressLine2) ? presentAddress.AddressLine2 + ", " : "") +
                    (!string.IsNullOrEmpty(presentAddress.AddressLine3) ? presentAddress.AddressLine3 + ", " : "") + Environment.NewLine +
                    presentAddress.Thana.ThanaNameEng + ", " +
                    presentAddress.District.DistrictNameEng + Environment.NewLine +
                    presentAddress.Division.DivisionNameEng + ", " + presentAddress.Country.Name) : "";

                data.AccountTitle = offerLetter.Proposal.Application.AccountTitle;
                data.OfferLetterTexts.RemoveAll(f => f.Status != EntityStatus.Active);
                //data.ModeOfDisbursment 
                //data.DisbursmentCondition 
                return data;
            }
            else
            {
                return null;
            }
        }

        public long GetProposalByAppId(long? AppId)
        {
            long proposalId = 0;
            var proposal =
                GenService
                    .GetAll<Proposal>()
                    .FirstOrDefault(e => e.ApplicationId == AppId && e.Status == EntityStatus.Active);
            if (proposal != null)
            {
                proposalId = (long)proposal.Id;
            }

            if (proposalId > 0)
                return proposalId;
            return 0;
        }

        public ResponseDto SaveProposalApproval(ProposalDto dto, long userId)
        {
            var entity = new Proposal();
            ResponseDto response = new ResponseDto();
            ApplicationLog log = new ApplicationLog();
            if (dto != null && dto.Id > 0)
            {
                if (dto.Id != null)
                    entity = GenService.GetById<Proposal>((long)dto.Id);
                using (var tran = new TransactionScope())
                {

                    try
                    {
                        if (entity.IsApproved == null)
                        {
                            entity.IsApproved = true;
                            var application = GenService.GetById<Application>(entity.ApplicationId);
                            log.FromStage = application.ApplicationStage;
                            log.Activity = Activity.Submit;
                            log.AppIdRef = application.Id;
                            log.ApplicationId = application.Id;
                            log.AppType = AppType.Application;
                            log.CreateDate = DateTime.Now;
                            log.CreatedBy = userId;
                            log.FromUserId = userId;
                            log.ToUserId = userId;
                            log.Status = EntityStatus.Active;
                            log.ToStage = ApplicationStage.SentToOperations;
                            application.ApplicationStage = ApplicationStage.SentToOperations;
                            application.CurrentHolding = null;
                            application.CurrentHoldingEmpId = null;

                            GenService.Save(entity);
                            GenService.Save(application);
                            GenService.Save(log);
                            N.CreateNotificationForService(NotificationType.ApplicationCreditMemoApproved, (long)entity.Id);
                            N.CreateNotificationForService(NotificationType.ApplicationAvailableForGeneratingOfferLetter, (long)entity.Id);
                            tran.Complete();
                            //GenService.SaveChanges();
                            response.Id = entity.Id;
                        }
                    }
                    catch (Exception ex)
                    {
                        tran.Dispose();
                        response.Id = entity.Id;
                        response.Message = "Proposal Approved Failed";
                        return response;
                    }
                    response.Success = true;
                    response.Message = "Proposal Approved Successfully";
                }
            }
            return response;
        }
        public IPagedList<OfferLetterDto> ProposalList(int pageSize, int pageCount, string searchString)
        {
            var proposal = GenService.GetAll<Proposal>().Where(r => r.Status == EntityStatus.Active && r.IsApproved == true).ToList();
            var offerLetters =
                GenService.GetAll<OfferLetter>()
                    .Where(o => !(o.Status == EntityStatus.Active && o.CRMIsApproved == true && o.CUSIsApproved == true && o.OPSIsApproved == true) && o.Status == EntityStatus.Active);
            List<long> proposalIds = GenService.GetAll<OfferLetter>()
                    .Where(o => o.Status == EntityStatus.Active && o.CRMIsApproved == true && o.CUSIsApproved == true && o.OPSIsApproved == true).Select(o => o.ProposalId)
                    .ToList();
            var offerLetter =
                (
                //from po in proposal
                from po in proposal
                join off in offerLetters on po.Id equals off.ProposalId into extra
                from ext in extra.DefaultIfEmpty()
                select new OfferLetterDto
                {
                    ProposalId = po.Id,
                    ApplicationNo = po.ApplicationNo,
                    ApplicationId = po.ApplicationId,
                    ProposalNo = po.CreditMemoNo,
                    AccountTitle = po.Application != null ? po.Application.AccountTitle : "",
                    OfferLetterNo = ext != null ? ext.OfferLetterNo : "",
                    Id = ext != null ? ext.Id : 0,
                    Status = ext != null ? ext.Status : 0,
                }).OrderByDescending(r => r.Id).ToList();
            var result = (from off in offerLetter
                          group off by new { off.ApplicationNo, off.ProposalNo, off.ProposalId, off.ApplicationId }
                into grp
                          let firstOrDefault = grp.FirstOrDefault()
                          where firstOrDefault != null
                          select new OfferLetterDto
                          {
                              ApplicationId = grp.Key.ApplicationId,
                              ProposalId = grp.Key.ProposalId,
                              ApplicationNo = grp.Key.ApplicationNo,
                              ProposalNo = grp.Key.ProposalNo,
                              AccountTitle = grp.FirstOrDefault().AccountTitle,
                              Id = firstOrDefault.Id
                          }).ToList();

            result.RemoveAll(p => proposalIds.Contains(p.ProposalId));
            if (!string.IsNullOrEmpty(searchString))
                result = result.Where(a => a.ProposalNo.ToLower().Contains(searchString.ToLower())).ToList();
            var temp = result.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }

        public IPagedList<OfferLetterDto> ProposalListIdWise(int pageSize, int pageCount, string searchString)//, long UserId)
        {
            var proposal = GenService.GetAll<Proposal>().Where(r => r.Status == EntityStatus.Active && r.IsApproved == true).ToList();//&& r.Application.CurrentHolding == UserId).ToList();
            var offerLetters =
                GenService.GetAll<OfferLetter>()
                    .Where(o => !(o.Status == EntityStatus.Active && o.CRMIsApproved != null && o.CRMIsApproved == true && o.CUSIsApproved == true && o.OPSIsApproved == true))
                    .ToList();
            List<long> proposalIds = GenService.GetAll<OfferLetter>()
                    .Where(o => o.Status == EntityStatus.Active && o.CRMIsApproved != null && o.CRMIsApproved == true && o.CUSIsApproved == true && o.OPSIsApproved == true).Select(o => o.ProposalId)
                    .ToList();
            var offerLetter =
                (
                //from po in proposal
                from po in proposal
                join off in offerLetters on
                     po.Id equals off.ProposalId into extra
                from ext in extra.DefaultIfEmpty()
                select new OfferLetterDto
                {
                    ProposalId = po.Id,
                    ApplicationNo = po.ApplicationNo,
                    ApplicationId = po.ApplicationId,
                    ProposalNo = po.CreditMemoNo,
                    AccountTitle = po.Application != null ? po.Application.AccountTitle : "",
                    OfferLetterNo = ext != null ? ext.OfferLetterNo : "",
                    Id = ext != null ? ext.Id : 0,
                    Status = ext != null ? ext.Status : 0,
                }).OrderByDescending(r => r.Id).ToList();
            var result = (from off in offerLetter
                          group off by new { off.ApplicationNo, off.ProposalNo, off.ProposalId, off.ApplicationId }
                into grp
                          let firstOrDefault = grp.FirstOrDefault()
                          where firstOrDefault != null
                          select new OfferLetterDto
                          {
                              ApplicationId = grp.Key.ApplicationId,
                              ProposalId = grp.Key.ProposalId,
                              ApplicationNo = grp.Key.ApplicationNo,
                              ProposalNo = grp.Key.ProposalNo,
                              AccountTitle = grp.FirstOrDefault().AccountTitle,
                              Id = firstOrDefault.Id
                          }).ToList(); //.Where(r => r.Id <= 0)
            //foreach (var offerLetterDto in result)
            //{
            //    var check = GenService.GetAll<OfferLetter>().Where(r => r.ProposalId == offerLetterDto.ProposalId && r.Status == EntityStatus.Active).ToList();
            //    if (check.Count > 0)
            //    {
            //        proposalIds.Add(offerLetterDto.ProposalId);
            //    }
            //}
            result.RemoveAll(p => proposalIds.Contains(p.ProposalId));
            if (!string.IsNullOrEmpty(searchString))
                result = result.Where(a => a.ProposalNo.ToLower().Contains(searchString.ToLower())).ToList();
            var temp = result.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }
        public ResponseDto SaveOfferLetterApproval(OfferLetterApprovalDto dto, long userId)
        {
            ResponseDto response = new ResponseDto();

            if (dto.Id != null && dto.OfferLetterType != OfferLetterType.Self_Assaignment)
            {
                var offer = GenService.GetById<OfferLetter>((long)dto.Id);

                if (dto.OfferLetterType == OfferLetterType.CRM)
                {
                    offer.CRMApprovalDate = dto.QuotationDate;
                    offer.CRMIsApproved = true;
                }
                if (dto.OfferLetterType == OfferLetterType.Operation)
                {
                    offer.OPSApprovalDate = dto.QuotationDate;
                    offer.OPSIsApproved = true;
                }
                if (dto.OfferLetterType == OfferLetterType.Customer)
                {
                    offer.CUSApprovalDate = dto.QuotationDate;
                    offer.CUSIsApproved = true;
                }

                if (offer != null)
                {
                    GenService.Save(offer);

                    response.Success = true;
                    response.Message = "Approval Succeed";
                }
            }
            else
            {
                response.Message = "Please Save Offer Letter First";
                return response;
            }

            return response;
        }
        public ProposalDto GetProposalObjByAppId(long AppId)
        {
            if (AppId > 0)
            {
                long proposalId = GetProposalByAppId(AppId);
                if (proposalId > 0)
                {
                    var proposal = GenService.GetById<Proposal>(proposalId);
                    var data = Mapper.Map<ProposalDto>(proposal);
                    return data;
                }
            }
            return null;

        }
        public List<SignatoriesDto> GetAllSignatories()
        {
            var data = GenService.GetAll<Signatories>().Where(r => r.Status == EntityStatus.Active).ToList();
            return Mapper.Map<List<SignatoriesDto>>(data);
        }

        public ResponseDto RejectProposal(long id)
        {
            var responce = new ResponseDto();
            var proposal = GenService.GetById<Proposal>(id);
            proposal.Status = EntityStatus.Inactive;
            GenService.Save(proposal, false);
            GenService.SaveChanges();
            responce.Success = true;
            responce.Message = "Proposal Rejected Successfully";
            return responce;
        }

        public ResponseDto DiscardCreditMemo(long id)
        {
            ResponseDto response = new ResponseDto();
            if (id > 0)
            {
                var proposal = GenService.GetById<Proposal>(id);
                proposal.Status = EntityStatus.Inactive;
                try
                {
                    GenService.Save(proposal);
                    if (proposal.ApplicationId > 0)
                    {
                        response.Id = proposal.ApplicationId;
                        response.Success = true;
                        response.Message = "Proposal Discarded Successfully.";
                    }
                }
                catch (Exception ex)
                {
                    response.Message = "Failed To Discard Proposal.";
                }
               
            }
            return response;
        }
    }
}
