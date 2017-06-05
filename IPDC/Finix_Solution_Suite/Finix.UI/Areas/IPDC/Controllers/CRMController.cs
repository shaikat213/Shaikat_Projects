using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Finix.IPDC.DTO;
using Finix.IPDC.Facade;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Util;
using Microsoft.Reporting.WebForms;
using System.IO;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.UI.Areas.IPDC.Controllers
{
    public class CRMController : BaseController
    {
        private readonly ApplicationFacade _applicationFacade = new ApplicationFacade();
        private readonly VerificationFacade _verification = new VerificationFacade();
        private readonly CRMFacade _crmFacade = new CRMFacade();
        private readonly EnumFacade _enumFacade = new EnumFacade();
        // GET: IPDC/CRM
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult LoadApplicationByAppIdForCRM(long AppId)
        {
            var depApp = _applicationFacade.LoadApplicationByAppIdForCRM(AppId,SessionHelper.UserProfile.UserId);
            return Json(depApp, JsonRequestBehavior.AllowGet);
        }

        //[HttpGet]
        //public JsonResult LoadApplicationByAppId(long AppId)
        //{
        //    //var depApp =             
        //    var depApp = _applicationFacade.LoadApplication(AppId);
        //    return Json(depApp, JsonRequestBehavior.AllowGet);
        //}
        [HttpGet]
        public JsonResult GetCIFVerificationHistory(long? ApplicationId, long CIFId, int CifType = 1)
        {
            if (CifType == 1)
            {
                var result = _verification.GetCIFPVerificationHistory(ApplicationId, CIFId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            if (CifType == 2)
            {

                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult LoadProposalByAppId(long? AppId, long? Id)
        {
            var depApp = _crmFacade.LoadProposalByAppId(AppId, Id);
            return Json(depApp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Proposal()
        {
            return View();
        }
        public JsonResult GetValuationType()
        {
            var data = _enumFacade.GetValuationType();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLandTypes()
        {
            var data = _enumFacade.GetLandTypes();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProposalProduct()
        {
            var data = _enumFacade.GetProposalProduct();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDeveloperCategory()
        {
            var data = _enumFacade.GetDeveloperCategory();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetApprovalAuthorityGroup(long productId, decimal fromAmount, decimal toAmount)
        //{
        //    var data = _crmFacade.GetApprovalAuthorityGroup(productId, fromAmount, toAmount, MemoType.Credit_Memo);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetRelationshipWithApplicant()
        {
            var data = _enumFacade.GetRelationshipWithApplicant();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPrinterFiltering()
        {
            var data = _enumFacade.GetPrinterFiltering();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFacilityType()
        {
            var data = _enumFacade.GetFacilityType();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPurposes()
        {
            var data = _enumFacade.GetPurposes();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //GetOfferTextType GetPrinterFiltering

        public JsonResult GetOfferTextType()
        {
            var data = _enumFacade.GetOfferTextType();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveProposal(ProposalDto dto)
        {
            try
            {
                DateTime receiveDate = DateTime.Now;
                var FromConverted = DateTime.TryParseExact(dto.ApplicationReceiveDateText, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out receiveDate);
                if (FromConverted)
                    dto.ApplicationReceiveDate = receiveDate;
                var crmConverted = DateTime.TryParseExact(dto.CRMReceiveDateText, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out receiveDate);
                if (crmConverted)
                    dto.CRMReceiveDate = receiveDate;
                var proposalConverted = DateTime.TryParseExact(dto.ProposalDateText, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out receiveDate);
                if (proposalConverted)
                    dto.ProposalDate = receiveDate;
                var expiryDateConverted = DateTime.TryParseExact(dto.ExpiryDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out receiveDate);
                if (expiryDateConverted)
                    dto.ExpiryDate = receiveDate;
                if (dto.FDRs != null)
                {
                    var fdrDeatils = new List<Proposal_FDRDto>();
                    foreach (var fdrpsDetailDto in dto.FDRs)
                    {
                        DateTime mtDate = DateTime.Now;
                        FromConverted = DateTime.TryParseExact(fdrpsDetailDto.MaturityDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out mtDate);
                        if (FromConverted)
                            fdrpsDetailDto.MaturityDate = mtDate;
                        fdrDeatils.Add(fdrpsDetailDto);
                    }
                    dto.FDRs = fdrDeatils;
                }
                if (dto.ClientProfiles != null)
                {
                    DateTime date = DateTime.Now;
                    foreach (var item in dto.ClientProfiles)
                    {
                        var DateOfBirth = DateTime.TryParseExact(item.DateOfBirthTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);

                        if (DateOfBirth)
                            item.DateOfBirth = date;
                    }
                }
                if (dto.ProposalCreditCards != null)
                {
                    DateTime date = DateTime.Now;
                    foreach (var item in dto.ProposalCreditCards)
                    {
                        var DateOfBirth = DateTime.TryParseExact(item.CreditCardIssueDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);

                        if (DateOfBirth)
                            item.CreditCardIssueDate = date;
                    }
                }
                if (dto.CIBs != null)
                {
                    DateTime date = DateTime.Now;
                    foreach (var item in dto.CIBs)
                    {
                        var cibDate = DateTime.TryParseExact(item.CIBDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);

                        if (cibDate)
                            item.CIBDate = date;
                    }
                }
                if (dto.Liabilities != null)
                {
                    DateTime startDate = DateTime.Now;
                    DateTime date = DateTime.Now;
                    foreach (var item in dto.Liabilities)
                    {
                        var expDate = DateTime.TryParseExact(item.ExpiryDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                        var strtDate = DateTime.TryParseExact(item.StartingDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate);
                        if (strtDate && startDate > DateTime.MinValue)
                            item.StartingDate = startDate;
                        if (expDate && date > DateTime.MinValue)
                            item.ExpiryDate = date;
                    }
                }
                if (dto.OverallAssessments != null)
                {
                    DateTime date = DateTime.Now;
                    foreach (var item in dto.OverallAssessments)
                    {
                        var assessmentDate = DateTime.TryParseExact(item.AssessmentDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                        //var FromConvertedDepositDate = DateTime.TryParseExact(item.DepositDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out chqDepositDate);
                        //if (FromConvertedChqDate)
                        //    item.ChequeDate = chequeDate;
                        if (assessmentDate)
                            item.AssessmentDate = date;
                    }
                }
                if (dto.FDRs != null)
                {
                    DateTime date = DateTime.Now;
                    foreach (var item in dto.FDRs)
                    {
                        var assessmentDate = DateTime.TryParseExact(item.MaturityDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                        //var FromConvertedDepositDate = DateTime.TryParseExact(item.DepositDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out chqDepositDate);
                        //if (FromConvertedChqDate)
                        //    item.ChequeDate = chequeDate;
                        if (assessmentDate)
                            item.MaturityDate = date;
                    }
                }
                var result = _crmFacade.SaveProposal(dto, SessionHelper.UserProfile.UserId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetVerificationStates()
        {
            var data = _enumFacade.GetOverallVerificationStates();
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CreditMemoReport(string reportTypeId, long ProposalId)
        {
            var proposalDto = _crmFacade.LoadProposalByAppId(null, ProposalId);//_crmFacade.LoadProposalById(ProposalId);
            List<ProposalDto> proposalDummyList = new List<ProposalDto>();
            //List<GuarantorDto> guarantorList = new List<GuarantorDto>();
            proposalDummyList.Add(proposalDto);
            //var app = _applicationFacade.LoadApplicationByAppId(proposalDto.app)
            string CustomRefNo = "IPDC/CM/";
            LocalReport lr = new LocalReport();
            string path = "";
            if (proposalDto.FacilityType == ProposalFacilityType.Home_Loan && proposalDto.PropertyType == LandedPropertyValuationType.Flat_Purchase)
            {
                CustomRefNo += "HL/";
                path = Path.Combine(Server.MapPath("~/Areas/IPDC/Reports"), "CreditMemo.rdlc");
            }
            else if (proposalDto.FacilityType == ProposalFacilityType.Home_Loan && proposalDto.PropertyType == LandedPropertyValuationType.Self_Construction)
            {
                CustomRefNo += "HL/";
                path = Path.Combine(Server.MapPath("~/Areas/IPDC/Reports"), "CreditMemoSelfConstruction.rdlc");
            }
            else if (proposalDto.FacilityType == ProposalFacilityType.Auto_Loan)
            {
                CustomRefNo += "AL/";
                path = Path.Combine(Server.MapPath("~/Areas/IPDC/Reports"), "CreditMemoAutoLoan.rdlc");
            }
            else if (proposalDto.FacilityType == ProposalFacilityType.Personal_Loan)
            {
                CustomRefNo += "PL/";
                path = Path.Combine(Server.MapPath("~/Areas/IPDC/Reports"), "CreditMemoPersonalLoan.rdlc");
            }

            CustomRefNo += proposalDto.ProposalDate.Value.Year + "/" + proposalDto.ApplicationNo;
            
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("Proposal");
            }
            //IEnumerable<ProposalDto> proposalList;// = new IEnumerable<ProposalDto>();
            //proposalList = new List<ProposalDto>();
            //proposalList.Add(proposalDto);
            //if (proposalDto.FacilityType == ProposalFacilityType.Home_Loan && proposalDto.PropertyType == LandedPropertyValuationType.Flat_Purchase)
            //{
            var applicants = "";
            if(proposalDto.ClientProfiles.Count > 0)
            {
                //applicants = "";
                for(int i = 0; proposalDto.ClientProfiles.Count > i; )
                {
                    applicants += proposalDto.ClientProfiles[i++].Name;
                    
                    if(proposalDto.ClientProfiles.Count > 1)
                    {
                        if ((i + 1) == proposalDto.ClientProfiles.Count)
                            applicants += " and ";
                        else
                            applicants += ", ";
                    }
                }
            }
            ReportDataSource rd = new ReportDataSource("CreditMemo", proposalDummyList);
            ReportDataSource rd1 = new ReportDataSource("ClientProfile", proposalDto.ClientProfiles);
            ReportDataSource rd2 = new ReportDataSource("Guarantors", proposalDto.Guarantors);
            ReportDataSource rd3 = new ReportDataSource("AssetBackup", proposalDto.Texts.Where(r => r.Type == ProposalTextTypes.AssetBackup));
            ReportDataSource rd4 = new ReportDataSource("Liabilities", proposalDto.Liabilities);
            ReportDataSource rd5 = new ReportDataSource("CIBs", proposalDto.CIBs);
            ReportDataSource rd6 = new ReportDataSource("NetWorths", proposalDto.NetWorths);
            ReportDataSource rd7 = new ReportDataSource("Incomes", proposalDto.Incomes.Where(r => r.IsConsidered));
            ReportDataSource rd8 = new ReportDataSource("IncomesNotConsiderd", proposalDto.Incomes.Where(r => r.IsConsidered == false));
            ReportDataSource rd9 = new ReportDataSource("Strength", proposalDto.Texts.Where(r => r.Type == ProposalTextTypes.Strength));
            ReportDataSource rd10 = new ReportDataSource("WeakNess", proposalDto.Texts.Where(r => r.Type == ProposalTextTypes.Weakness));
            ReportDataSource rd11 = new ReportDataSource("SecurityDetails", proposalDto.SecurityDetails);
            ReportDataSource rd12 = new ReportDataSource("OverallAssesments", proposalDto.OverallAssessments);
            ReportDataSource rd13 = new ReportDataSource("StressTesting", proposalDto.StressRates);
            ReportDataSource rd14 = new ReportDataSource("Exceptions", proposalDto.Texts.Where(r => r.Type == ProposalTextTypes.Exceptions));
            ReportDataSource rd15 = new ReportDataSource("ModeOfDisburseMent", proposalDto.Texts.Where(r => r.Type == ProposalTextTypes.ModeOfDisbursement));
            ReportDataSource rd16 = new ReportDataSource("FinalDisburseMent", proposalDto.Texts.Where(r => r.Type == ProposalTextTypes.FinalDisbursementConditions));
            ReportDataSource rd17 = new ReportDataSource("dsProposalSignatory", proposalDto.Signatories);
            ReportDataSource rd18 = new ReportDataSource("ProposalOtherCost", proposalDto.OtherCosts);
            ReportDataSource rd19 = new ReportDataSource("ProposalValuationOtherCosts", proposalDto.ValuationOtherCosts);
            ReportDataSource rd20 = new ReportDataSource("ProposalCreditCards", proposalDto.ProposalCreditCards);
            ReportParameter rp1 = new ReportParameter("ApplicantNames", applicants);
            ReportParameter rp2 = new ReportParameter("CustomRefNo", CustomRefNo);
            //ReportParameter rp2 = new ReportParameter("ToDate", toDate.ToString());
            lr.DataSources.Add(rd);
            lr.DataSources.Add(rd1);
            lr.DataSources.Add(rd2);
            lr.DataSources.Add(rd3);
            lr.DataSources.Add(rd4);
            lr.DataSources.Add(rd5);
            lr.DataSources.Add(rd6);
            lr.DataSources.Add(rd7);
            lr.DataSources.Add(rd8);
            lr.DataSources.Add(rd9);
            lr.DataSources.Add(rd10);
            lr.DataSources.Add(rd11);
            lr.DataSources.Add(rd12);
            lr.DataSources.Add(rd13);
            lr.DataSources.Add(rd14);
            lr.DataSources.Add(rd15);
            lr.DataSources.Add(rd16);
            lr.DataSources.Add(rd17);
            lr.DataSources.Add(rd18);
            lr.DataSources.Add(rd19);
            lr.DataSources.Add(rd20);
            //}
            lr.SetParameters(new ReportParameter[] { rp1, rp2 });

            string reportType = reportTypeId;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =
                  "<DeviceInfo>" +
                "  <OutputFormat>EMF</OutputFormat>" +
                "  <PageWidth>8.2in</PageWidth>" +
                "  <PageHeight>11.6in</PageHeight>" +
                "  <MarginTop>0.25in</MarginTop>" +
                "  <MarginLeft>0.25in</MarginLeft>" +
                "  <MarginRight>0.25in</MarginRight>" +
                "  <MarginBottom>0.25in</MarginBottom>" +
                "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            var renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings
                );
            return File(renderedBytes, mimeType);
        }

        //public JsonResult GetAuthority(decimal loanAmount) ///, MemoType memoTypelong productId, 
        //{
        //    var data = _crmFacade.GetAuthority(loanAmount);//productId, ,  memoType
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult OfferLetter()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveOfferLetter(OfferLetterDto dto)
        {
            try
            {
                DateTime offerLetterDate = DateTime.Now;
                var offerLetterConverted = DateTime.TryParseExact(dto.OfferLetterDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out offerLetterDate);
                if (offerLetterConverted)
                    dto.OfferLetterDate = offerLetterDate;

                var result = _crmFacade.SaveOfferLetter(dto, SessionHelper.UserProfile.UserId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult LoadOfferLetter(long? proposalId, long? id)
        {
            var data = _crmFacade.LoadOfferLetter(proposalId, id);//productId, ,  memoType
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CRMCreditMemos(string sortOrder, string currentFilter, string searchString, int page = 1, int itemPerPage = 10)
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
            var temp = _crmFacade.GetCreditMemosForApproval(itemPerPage, page, searchString, SessionHelper.UserProfile.UserId);
            return View(temp);
        }

        [HttpPost]
        public JsonResult CreditMemoApproval(long ProposalId, bool ApprovalStatus)
        {
            var result = _crmFacade.CreditMemoApproval(ProposalId, ApprovalStatus, SessionHelper.UserProfile.UserId);
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public ActionResult OfferLetterApprovalListCRM(string sortOrder, string currentFilter, string searchString, int page = 1, int itemPerPage = 10)
        {
            ViewBag.CurrentSort = sortOrder;
            if (!string.IsNullOrEmpty(searchString))
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;
            var temp = _crmFacade.GetOfferLettersForApprovalCRM(itemPerPage, page, searchString);
            return View(temp);
        }
        [HttpPost]
        public JsonResult OfferLetterApprovalCRM(long OfferLetterId, bool ApprovalStatus)
        {
            var result = _crmFacade.OfferLetterApproval(OfferLetterId, ApprovalStatus, SessionHelper.UserProfile.UserId, 1);
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public ActionResult OfferLetterApprovalListOPS(string sortOrder, string currentFilter, string searchString, int page = 1, int itemPerPage = 10)
        {
            ViewBag.CurrentSort = sortOrder;
            if (!string.IsNullOrEmpty(searchString))
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;
            var temp = _crmFacade.GetOfferLettersForApprovalOPS(itemPerPage, page, searchString);
            return View(temp);
        }
        [HttpPost]
        public JsonResult OfferLetterApprovalOPS(long OfferLetterId, bool ApprovalStatus)
        {
            var result = _crmFacade.OfferLetterApproval(OfferLetterId, ApprovalStatus, SessionHelper.UserProfile.UserId, 2);
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        public JsonResult OfferLetterApprovalCUS(long OfferLetterId, bool ApprovalStatus)
        {
            var result = _crmFacade.OfferLetterApproval(OfferLetterId, ApprovalStatus, SessionHelper.UserProfile.UserId, 3);
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        public ActionResult AutoLoanOfferLetterReport(string reportTypeId, long? ProposalId, long OfferLetterId)
        {
            var offerLetterDto = _crmFacade.AutoLoanOfferLetterReport(OfferLetterId, ProposalId);//_crmFacade.LoadProposalById(ProposalId);
            List<OfferLetterDto> offerLetterDummy = new List<OfferLetterDto>();
            //List<GuarantorDto> guarantorList = new List<GuarantorDto>();
            offerLetterDummy.Add(offerLetterDto);
            string CustomRefNo = "IPDC/OL/";
            LocalReport lr = new LocalReport();
            string path;
            if (offerLetterDto.FacilityType == ProposalFacilityType.Auto_Loan)
            {
                CustomRefNo += "AL/";
                path = Path.Combine(Server.MapPath("~/Areas/IPDC/Reports"), "AutoOfferLetter.rdlc");
            }
            else if (offerLetterDto.FacilityType == ProposalFacilityType.RLS)
            {
                CustomRefNo += "SOD/";
                path = Path.Combine(Server.MapPath("~/Areas/IPDC/Reports"), "RLSOfferLetter.rdlc");
            }
            else
            {
                CustomRefNo += "PL/";
                path = Path.Combine(Server.MapPath("~/Areas/IPDC/Reports"), "PersonalOfferLetter.rdlc");
            }
            CustomRefNo += offerLetterDto.ApplicationDate.Year + "/" + offerLetterDto.ApplicationNo;

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("OfferLetter");
            }

            ReportDataSource rd = new ReportDataSource("OfferLetter", offerLetterDummy);
            ReportDataSource rd1 = new ReportDataSource("AssetBackup", offerLetterDto.OfferLetterTexts);
            ReportParameter rp2 = new ReportParameter("CustomRefNo", CustomRefNo);
            //ReportDataSource rd2 = new ReportDataSource("Guarantors", proposalDto.Guarantors);
            //ReportDataSource rd3 = new ReportDataSource("AssetBackup", proposalDto.Texts.Where(r => r.Type == ProposalTextTypes.AssetBackup));

            lr.DataSources.Add(rd);
            lr.DataSources.Add(rd1);
            lr.SetParameters(new ReportParameter[] { rp2 });
            //lr.DataSources.Add(rd2);
            //lr.DataSources.Add(rd3);

            string reportType = reportTypeId;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =
                  "<DeviceInfo>" +
                "  <OutputFormat>EMF</OutputFormat>" +
                "  <PageWidth>8.2in</PageWidth>" +
                "  <PageHeight>11.6in</PageHeight>" +
                "  <MarginTop>0.25in</MarginTop>" +
                "  <MarginLeft>0.25in</MarginLeft>" +
                "  <MarginRight>0.25in</MarginRight>" +
                "  <MarginBottom>0.25in</MarginBottom>" +
                "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;

            var renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings
                );


            return File(renderedBytes, mimeType);
        }

        public ActionResult HomeLoanOfferLetterReport(string reportTypeId, long? ProposalId, long OfferLetterId)
        {
            var offerLetterDto = _crmFacade.AutoLoanOfferLetterReport(OfferLetterId, ProposalId);//_crmFacade.LoadProposalById(ProposalId);
            List<OfferLetterDto> offerLetterDummy = new List<OfferLetterDto>();
            //List<GuarantorDto> guarantorList = new List<GuarantorDto>();
            offerLetterDummy.Add(offerLetterDto);
            string CustomRefNo = "IPDC/OL/";
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Areas/IPDC/Reports"), "HomeOfferLetter.rdlc");
            CustomRefNo += "AL/";
            CustomRefNo += offerLetterDto.ApplicationDate.Year + "/" + offerLetterDto.ApplicationNo;
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("OfferLetter");
            }

            ReportDataSource rd = new ReportDataSource("OfferLetter", offerLetterDummy);
            ReportDataSource rd1 = new ReportDataSource("AssetBackup", offerLetterDto.OfferLetterTexts);
            ReportDataSource rd2 = new ReportDataSource("Proposal_ClientProfile", offerLetterDto.Proposal.ClientProfiles);
            ReportParameter rp2 = new ReportParameter("CustomRefNo", CustomRefNo);

            lr.DataSources.Add(rd);
            lr.DataSources.Add(rd1);
            lr.DataSources.Add(rd2);
            lr.SetParameters(new ReportParameter[] { rp2 });

            string reportType = reportTypeId;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =
                  "<DeviceInfo>" +
                "  <OutputFormat>EMF</OutputFormat>" +
                "  <PageWidth>8.2in</PageWidth>" +
                "  <PageHeight>11.6in</PageHeight>" +
                "  <MarginTop>0.25in</MarginTop>" +
                "  <MarginLeft>0.25in</MarginLeft>" +
                "  <MarginRight>0.25in</MarginRight>" +
                "  <MarginBottom>0.25in</MarginBottom>" +
                "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;

            var renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings
                );


            return File(renderedBytes, mimeType);
        }
        public ActionResult RLSReport(string reportTypeId, long ProposalId)
        {
            var proposalDto = _crmFacade.LoadProposalByAppId(null, ProposalId);//_crmFacade.LoadProposalById(ProposalId);
            List<ProposalDto> proposalDummyList = new List<ProposalDto>();
            //List<GuarantorDto> guarantorList = new List<GuarantorDto>();
            proposalDummyList.Add(proposalDto);
            LocalReport lr = new LocalReport();
            string path = "";
            if (proposalDto.FacilityType == ProposalFacilityType.RLS )
            {
                path = Path.Combine(Server.MapPath("~/Areas/IPDC/Reports"), "RLSReport.rdlc");
            }
            else
            {
                return View("Proposal");
            }
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("Proposal");
            }
            string CustomRefNo = "IPDC/CM/SOD/" + proposalDto.Application.ApplicationDate.Year + "/" + proposalDto.ApplicationNo;
            //IEnumerable<ProposalDto> proposalList;// = new IEnumerable<ProposalDto>();
            //proposalList = new List<ProposalDto>();
            //proposalList.Add(proposalDto);
            //if (proposalDto.FacilityType == ProposalFacilityType.Home_Loan && proposalDto.PropertyType == LandedPropertyValuationType.Flat_Purchase)
            //{
            ReportDataSource rd = new ReportDataSource("CreditMemo", proposalDummyList);
            ReportDataSource rd1 = new ReportDataSource("ClientProfile", proposalDto.ClientProfiles);
            ReportDataSource rd2 = new ReportDataSource("CIBs", proposalDto.CIBs);
            ReportDataSource rd3 = new ReportDataSource("Incomes", proposalDto.Incomes.Where(r => r.IsConsidered));
            ReportDataSource rd4 = new ReportDataSource("Proposal_FDR", proposalDto.FDRs);
            ReportDataSource rd5 = new ReportDataSource("dsProposalSignatory", proposalDto.Signatories);
            ReportParameter rp1 = new ReportParameter("CustomRefNo", CustomRefNo);

            lr.DataSources.Add(rd);
            lr.DataSources.Add(rd1);
            lr.DataSources.Add(rd2);
            lr.DataSources.Add(rd3);
            lr.DataSources.Add(rd4);
            lr.DataSources.Add(rd5);


            lr.SetParameters(new ReportParameter[] { rp1 });

            string reportType = reportTypeId;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =
                  "<DeviceInfo>" +
                "  <OutputFormat>EMF</OutputFormat>" +
                "  <PageWidth>8.2in</PageWidth>" +
                "  <PageHeight>11.6in</PageHeight>" +
                "  <MarginTop>0.25in</MarginTop>" +
                "  <MarginLeft>0.25in</MarginLeft>" +
                "  <MarginRight>0.25in</MarginRight>" +
                "  <MarginBottom>0.25in</MarginBottom>" +
                "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            var renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings
                );
            return File(renderedBytes, mimeType);
        }
        public JsonResult SaveProposalApproval(ProposalDto dto)
        {
            try
            {
                var result = _crmFacade.SaveProposalApproval(dto, SessionHelper.UserProfile.UserId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ProposalList(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _crmFacade.ProposalList(10, page, searchString);
            return View(model);
        }
        public ActionResult ProposalListIdWise(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _crmFacade.ProposalListIdWise(10, page, searchString);//, SessionHelper.UserProfile.UserId);
            return View(model);
        }
        //SaveOfferLetterApproval
        public JsonResult SaveOfferLetterApproval(OfferLetterApprovalDto dto)
        {
            try
            {
                if (!string.IsNullOrEmpty(dto.QuotationDateTxt))
                {
                    DateTime changeDate;
                    var fromConverted = DateTime.TryParseExact(dto.QuotationDateTxt, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out changeDate);
                    if (fromConverted)
                        dto.QuotationDate = changeDate;
                }
                var result = _crmFacade.SaveOfferLetterApproval(dto, SessionHelper.UserProfile.UserId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAllSignatories()
        {
            var data = _crmFacade.GetAllSignatories();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAuthorityLevel()
        {
            var data = _enumFacade.GetAuthorityLevel();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RejectProposal(long id)
        {
            var data = _crmFacade.RejectProposal(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DiscardCreditMemo(long id)
        {
            var data = _crmFacade.DiscardCreditMemo(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        } 
    }
}