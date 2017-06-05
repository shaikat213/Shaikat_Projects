using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Finix.IPDC.DTO;
using Finix.IPDC.Facade;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Util;

namespace Finix.UI.Areas.IPDC.Controllers
{
    public class ApplicationController : BaseController
    {
        private readonly ApplicationFacade _applicationFacade = new ApplicationFacade();
        private readonly EnumFacade _enumFacade = new EnumFacade();
        private readonly ProductFacade _productFacade = new ProductFacade();
        private readonly DeveloperFacade _developerFacade = new DeveloperFacade();

        #region Common Application
        [HttpGet]
        public ActionResult Application()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveApplication(ApplicationDto dto)
        {
            try
            {
                if (!string.IsNullOrEmpty(dto.ApplicationDateText))
                {
                    DateTime assessmentDate;
                    var fromConverted = DateTime.TryParseExact(dto.ApplicationDateText, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out assessmentDate);
                    if (fromConverted)
                        dto.ApplicationDate = assessmentDate;
                }
                if (dto.DocChecklist != null && dto.DocChecklist.Count > 0)
                {
                    var temp = dto.DocChecklist.Where(d => !string.IsNullOrEmpty(d.SubmissionDeadlineText)).ToList();
                    if (temp != null && temp.Count > 0)
                    {
                        foreach (var item in temp)
                        {
                            DateTime submissionDate;
                            var conversion = DateTime.TryParseExact(item.SubmissionDeadlineText, "dd/MM/yyyy",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out submissionDate);
                            if (conversion)
                                item.SubmissionDeadline = submissionDate;
                        }
                    }
                }
                
                var result = _applicationFacade.SaveApplication(dto, SessionHelper.UserProfile.UserId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        //Cacel Application
        public FileResult Download()
        {
            //return File(imageName, System.Net.Mime.MediaTypeNames.Application.Octet);
            //byte[] fileBytes = new byte[] { };
            string fileName = BizConstants.CardRateChart;
            return File(fileName, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(fileName));
        }
        public JsonResult CloseApplication(long Id, string RejectionReason, ApplicationStage? toApplicationStage)
        {
            var response = _applicationFacade.CancelApplication(Id, RejectionReason, toApplicationStage, SessionHelper.UserProfile.UserId);
            return Json(response, JsonRequestBehavior.DenyGet);
        }

        public JsonResult ApplicationSendToRM(long ApplicaitonId)
        {
            var response = _applicationFacade.ApplicationSendToRM(ApplicaitonId, SessionHelper.UserProfile.UserId);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult LoadApplication(long AppId)
        {
            var result = _applicationFacade.LoadApplication(AppId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult LoadApplicationByAppId(long AppId)
        {
            var depApp = _applicationFacade.LoadApplication(AppId);
            return Json(depApp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ApplicationUneditable()
        {
            return View();
        }
        
        #endregion

        #region Deposit Application
        public ActionResult DepositApplication()
        {
            return View();
        }
        [HttpPost]
        public JsonResult SaveDepositApplication(DepositApplicationDto dto)
        {
            try
            {
                DateTime chequeDate = DateTime.Now;
                DateTime chqDepositDate = DateTime.Now;
                DateTime transferDepositDate = DateTime.Now;
                DateTime cashDepositDate = DateTime.Now;

                if (dto.ChequeDeposits != null)
                {
                    foreach (var item in dto.ChequeDeposits)
                    {
                        var FromConvertedChqDate = DateTime.TryParseExact(item.ChequeDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out chequeDate);
                        var FromConvertedDepositDate = DateTime.TryParseExact(item.DepositDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out chqDepositDate);
                        if (FromConvertedChqDate)
                            item.ChequeDate = chequeDate;
                        if (FromConvertedDepositDate)
                            item.ChequeDate = chqDepositDate;
                    }
                }

                if (dto.TransferDeposits != null)
                {
                    foreach (var item in dto.TransferDeposits)
                    {
                        var FromConvertedTransDate = DateTime.TryParseExact(item.TransferDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out transferDepositDate);
                        if (FromConvertedTransDate)
                            item.TransferDate = transferDepositDate;
                    }
                }

                if (dto.CashDeposits != null)
                {
                    foreach (var item in dto.CashDeposits)
                    {
                        var FromConvertedTransDate = DateTime.TryParseExact(item.CashDepositDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out cashDepositDate);
                        if (FromConvertedTransDate)
                            item.CashDepositDate = cashDepositDate;
                    }
                }

                DateTime fundRealizationDate = DateTime.Now;
                var FromConvertedFundRealizationDate = DateTime.TryParseExact(dto.FundRealizationDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fundRealizationDate);
                if (FromConvertedFundRealizationDate)
                    dto.FundRealizationDate = fundRealizationDate;

                DateTime accountOpenDate = DateTime.Now;
                var FromConvertedAccountOpenDateDate = DateTime.TryParseExact(dto.AccountOpenDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out accountOpenDate);
                if (FromConvertedAccountOpenDateDate)
                    dto.AccountOpenDate = accountOpenDate;

                DateTime marturityDate = DateTime.Now;
                var FromConvertedMaturityDate = DateTime.TryParseExact(dto.MaturityDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out marturityDate);
                if (FromConvertedMaturityDate)
                    dto.MaturityDate = marturityDate;

                var userId = SessionHelper.UserProfile.UserId;
                var result = _applicationFacade.SaveDepositApplication(dto, userId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.DenyGet);
            }
        }
        [HttpGet]
        public JsonResult LoadDepositApplicationByAppId(long AppId)
        {
            var depApp = _applicationFacade.LoadDepositAppByAppId(AppId);
            return Json(depApp, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Loan Application

        public ActionResult LoanApplication()
        {
            return View();
        }
        [HttpPost]
        public JsonResult SaveLoanApplication(LoanApplicationDto dto)
        {
            try
            {
                if (dto.FDRPrimarySecurity != null)
                {
                    if (dto.FDRPrimarySecurity.FDRPSDetails != null)
                    {
                        var fdrDeatils = new List<FDRPSDetailDto>();
                        foreach (var fdrpsDetailDto in dto.FDRPrimarySecurity.FDRPSDetails)
                        {
                            DateTime mtDate = DateTime.Now;
                            var FromConverted = DateTime.TryParseExact(fdrpsDetailDto.MaturityDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out mtDate);
                            if (FromConverted)
                                fdrpsDetailDto.MaturityDate = mtDate;
                            fdrDeatils.Add(fdrpsDetailDto);
                        }
                        dto.FDRPrimarySecurity.FDRPSDetails = fdrDeatils;
                    }
                }
                if (dto.LPPrimarySecurity.FirstDisbursementExpDateText != null)
                {
                    DateTime disbursementExpDate = DateTime.Now;
                    var FromConverted = DateTime.TryParseExact(dto.LPPrimarySecurity.FirstDisbursementExpDateText, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out disbursementExpDate);
                    if (FromConverted)
                        dto.LPPrimarySecurity.FirstDisbursementExpDate = disbursementExpDate;
                }
                if (dto.LoanAppColSecurities != null)
                {
                    dto.LoanAppColSecurities = dto.LoanAppColSecurities.Where(r => r.IsChecked == true).ToList();
                }
                var result = _applicationFacade.SaveLoanApplication(dto, SessionHelper.UserProfile.UserId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult LoadLoanApplicationByAppId(long AppId)
        {
            var loanApp = _applicationFacade.LoadLoanAppByAppId(AppId);
            return Json(loanApp, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Enum loading functions

        public JsonResult GetModeOfDeposit()
        {
            var typeList = Enum.GetValues(typeof(ModeOfDeposit))
               .Cast<ModeOfDeposit>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = UiUtil.GetDisplayName(t)
               });
            //var typeList = UiUtil.EnumToKeyVal<HomeOwnership>();

            return Json(typeList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetModeOfOperations()
        {
            var typeList = Enum.GetValues(typeof(ModeOfOperations))
               .Cast<ModeOfOperations>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = UiUtil.GetDisplayName(t)
               });
            //var typeList = UiUtil.EnumToKeyVal<HomeOwnership>();

            return Json(typeList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDepositClasses()
        {
            var typeList = Enum.GetValues(typeof(DepositClass))
               .Cast<DepositClass>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = UiUtil.GetDisplayName(t)
               });
            //var typeList = UiUtil.EnumToKeyVal<HomeOwnership>();

            return Json(typeList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRenewalOptions()
        {
            var typeList = Enum.GetValues(typeof(DepositAccRenewalOpts))
               .Cast<DepositAccRenewalOpts>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = UiUtil.GetDisplayName(t)
               });
            //var typeList = UiUtil.EnumToKeyVal<HomeOwnership>();

            return Json(typeList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetWithdrawalModes()
        {
            var typeList = Enum.GetValues(typeof(DepositWithdrawalMode))
               .Cast<DepositWithdrawalMode>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = UiUtil.GetDisplayName(t)
               });
            //var typeList = UiUtil.EnumToKeyVal<HomeOwnership>();

            return Json(typeList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllApplicationStatus()
        {
            var typeList = Enum.GetValues(typeof(ApplicationStatus))
               .Cast<ApplicationStatus>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = UiUtil.GetDisplayName(t)
               });
            //var typeList = UiUtil.EnumToKeyVal<HomeOwnership>();

            return Json(typeList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSectionCheckList()
        {
            var typeList = Enum.GetValues(typeof(SanctionCheck))
               .Cast<SanctionCheck>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = UiUtil.GetDisplayName(t)
               });
            //var typeList = UiUtil.EnumToKeyVal<HomeOwnership>();

            return Json(typeList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDispatchStatusList()
        {
            var typeList = Enum.GetValues(typeof(InstrumentDispatchStatus))
               .Cast<InstrumentDispatchStatus>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = UiUtil.GetDisplayName(t)
               });
            //var typeList = UiUtil.EnumToKeyVal<HomeOwnership>();

            return Json(typeList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetWelcomeLeterStatusList()
        {
            var typeList = Enum.GetValues(typeof(WelcomeLetterStatus))
               .Cast<WelcomeLetterStatus>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = UiUtil.GetDisplayName(t)
               });
            //var typeList = UiUtil.EnumToKeyVal<HomeOwnership>();

            return Json(typeList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApplicationCustomerTypes()
        {
            var applicationCustomerTypes = _enumFacade.GetApplicationCustomerTypes();
            return Json(applicationCustomerTypes, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApplicationTypes()
        {
            var applicationTypes = _enumFacade.GetApplicationTypes();
            return Json(applicationTypes, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductTypes()
        {
            var productTypes = _enumFacade.GetProductTypes();
            return Json(productTypes, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApplicantRoles()
        {
            var applicationLoans = _enumFacade.GetApplicantRoles();
            return Json(applicationLoans, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDocumentStatusList()
        {
            var statusList = _enumFacade.GetDocumentStatusList();
            return Json(statusList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLoanPrimarySecurityTypes()
        {
            var loanPrimarySecurityType = _enumFacade.GetLoanPrimarySecurityTypes();
            return Json(loanPrimarySecurityType, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDisbursementModes()
        {
            var disbursmentMode = _enumFacade.GetDisbursementModes();
            return Json(disbursmentMode, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLoanChequeDeliveryOptions()
        {
            var loanChequeDeliveryOptions = _enumFacade.GetLoanChequeDeliveryOptions();
            return Json(loanChequeDeliveryOptions, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRiskLevels()
        {
            var riskLevels = _enumFacade.GetRiskLevels();
            return Json(riskLevels, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVehicleStatus()
        {
            var vehicleStatus = _enumFacade.GetVehicleStatus();
            return Json(vehicleStatus, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVendorTypes()
        {
            var vendorTypes = _enumFacade.GetVendorTypes();
            return Json(vendorTypes, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVehicleTypes()
        {
            var vehicleTypes = _enumFacade.GetVehicleTypes();
            return Json(vehicleTypes, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLandedPropertyLoanTypes()
        {
            var landProperty = _enumFacade.GetLandedPropertyLoanTypes();
            return Json(landProperty, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLandedPropertySellertypes()
        {
            var sellerType = _enumFacade.GetLandedPropertySellertypes();
            return Json(sellerType, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetWaiverTypes()
        {
            var waiverType = _enumFacade.GetWaiverTypes();
            return Json(waiverType, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Search Application

        public ActionResult SearchApplication(string sortOrder, string currentFilter, string searchString, int page = 1)
        {
            ViewBag.CurrentSort = sortOrder;
            if (!string.IsNullOrEmpty(searchString))
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var temp = _applicationFacade.GetApplicationPagedList(20, page, searchString);
            return View(temp);
            //return View();
        }

        public ActionResult SearchApplicationByUser(string sortOrder, string currentFilter, string searchString, int page = 1)
        {
            ViewBag.CurrentSort = sortOrder;
            if (!string.IsNullOrEmpty(searchString))
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var temp = _applicationFacade.GetApplicationPagedListByUser(20, page, searchString,SessionHelper.UserProfile.UserId);
            return View(temp);
            //return View();
        }



        public ActionResult CancelApplication(string sortOrder, string currentFilter, string searchString, int page = 1)
        {
            ViewBag.CurrentSort = sortOrder;
            if (!string.IsNullOrEmpty(searchString))
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var temp = _applicationFacade.GetApplicationListForCancel(20, page, searchString);
            return View(temp);
            //return View();
        }
        #endregion

        #region Submit application

        [HttpPost]
        public JsonResult SubmitApplicationToBm(long ApplicationId, string Comment, bool IsTL = false)
        {
            var response = _applicationFacade.SubmitApplicationToBm(ApplicationId, SessionHelper.UserProfile.UserId, Comment, IsTL);
            return Json(response, JsonRequestBehavior.DenyGet);
        }
        public ActionResult ApplicationApprovalTL(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _applicationFacade.GetApplicationApprovalTLPagedList(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }

        public ActionResult ApplicationApprovalBM(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _applicationFacade.GetApplicationApprovalTLPagedList(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }

        [HttpPost]
        public JsonResult SubmitApplicationToCRM(long ApplicationId, string Comment)
        {
            var result = _applicationFacade.SubmitApplicationToCRM(ApplicationId, SessionHelper.UserProfile.UserId, Comment);
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region CRM Application
        public ActionResult ApplicationDetails()
        {
            return View();
        }
        public ActionResult ApplicationDetailsSOD()
        {
            return View();
        }
        public ActionResult CRMApplications(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _applicationFacade.GetCRMApplicationsPagedList(20, page, searchString);
            return View(model);
        }

        public ActionResult CreditAnalystApplications(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _applicationFacade.CreditAnalystApplications(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }
        public ActionResult CRMSODApplications(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _applicationFacade.GetCRMApplicationsOfSODPagedList(20, page, searchString);
            return View(model);
        }
        public ActionResult CreditAnalystApplicationsSOD(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _applicationFacade.CreditAnalystApplications(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }
        public ActionResult ApplicationCifs()
        {
            return View();
        }
        #endregion

        #region Rejected Applications
        public ActionResult RejectedApplications(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _applicationFacade.GetRejectedApplicationsPagedList(20, page, searchString);
            return View(model);
        }
        public ActionResult RejectedApplicationsRM(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _applicationFacade.GetRejectedApplicationsRMPagedList(SessionHelper.UserProfile.UserId, 20, page, searchString);
            return View(model);
        }
        public ActionResult RejectedApplicationsByUserId(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _applicationFacade.RejectedApplicationsByUserId(20, page, searchString,SessionHelper.UserProfile.UserId);
            return View(model);
        }
        #endregion

        #region Data functions
        public JsonResult GetIPDCBankAccounts()
        {
            var ipdcBankAccount = _applicationFacade.GetIPDCBankAccount();
            return Json(ipdcBankAccount, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllIpdcBankAccntiWithName()
        {
            var ipdcBankAccount = _applicationFacade.GetAllIpdcBankAccntiWithName();
            return Json(ipdcBankAccount, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllProducts()
        {
            var products = _productFacade.GetAllProducts();
            return Json(products, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductByType(ProductType typeId)
        {
            var products = _productFacade.GetProductByType(typeId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllDocCheckList(long prodId, bool? IsIndividual, long? CifOrgId)
        {
            var docCheckList = _productFacade.GetAllDocCheckList(prodId, IsIndividual, CifOrgId);
            return Json(docCheckList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLoanAppColSecurities(long? appId)
        {
            var sellerType = _applicationFacade.GetLoanAppColSecurities(appId);
            return Json(sellerType, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllDevelopers()
        {
            var waiverType = _developerFacade.GetAllDevelopers();
            return Json(waiverType, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProjectsByDevelopers(long id)
        {
            var waiverType = _developerFacade.GetProjectsByDevelopers(id);
            return Json(waiverType, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRelationshipOptions()
        {
            var result = _enumFacade.GetAllRelationships();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadApplicationByAppIdForCRM(long AppId)
        {
            var depApp = _applicationFacade.LoadApplicationByAppIdForCRM(AppId,SessionHelper.UserProfile.UserId);
            return Json(depApp, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetAllApplications()
        {
            var applications = _applicationFacade.GetAllApplications();
            return Json(applications, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPrimaryApplicantAge(long AppId)
        {
            var age = _applicationFacade.GetPrimaryApplicantAge(AppId);
            return Json(age, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApplicantYoungestAge(long AppId)
        {
            var age = _applicationFacade.GetApplicantYoungestAge(AppId);
            return Json(age, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDisbursementToList()
        {
            var result = _enumFacade.GetDisbursementToEnums();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRelationshipWithApplicant()
        {
            var result = _enumFacade.GetRelationshipWithApplicant();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCostCenters()
        {
            var result = _applicationFacade.GetAllCostCenters();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        public JsonResult GetProjectAddress(long projectId)
        {
            var result = _applicationFacade.GetProjectAddress(projectId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}