using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web.Mvc;
using Finix.IPDC.DTO;
using Finix.IPDC.Facade;
using Finix.IPDC.Infrastructure;
using Microsoft.Reporting.WebForms;

namespace Finix.UI.Areas.IPDC.Controllers
{
    public class OperationsController : BaseController
    {

        private readonly ApplicationFacade _applicationFacade = new ApplicationFacade();
        private readonly OperationsFacade _operatinsFacade = new OperationsFacade();
        private readonly CIFFacade _cifFacade = new CIFFacade();
        private readonly CRMFacade _crmFacade = new CRMFacade();
        private readonly EnumFacade _enum = new EnumFacade();
        // GET: IPDC/Operations
        public ActionResult FundConfirmation()
        {
            return View();
        }
        public ActionResult DepositApplications(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _applicationFacade.GetOperationDepositApplications(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }

        public ActionResult CADepositApplications(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _applicationFacade.GetCADepositApplications(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }

        public ActionResult LoanApplications(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _applicationFacade.GetOperationLoanApplications(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }

        public ActionResult CaLoanApplications(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _applicationFacade.GetCaLoanApplications(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }

        public JsonResult SaveOpDepositApplication(ApplicationDto dto)
        {
            try
            {
                if (!string.IsNullOrEmpty(dto.HardCopyReceiveDateText))
                {
                    DateTime harCopyReceivedDate;
                    var fromConverted = DateTime.TryParseExact(dto.HardCopyReceiveDateText, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out harCopyReceivedDate);
                    if (fromConverted)
                        dto.HardCopyReceiveDate = harCopyReceivedDate;
                }
                var result = _operatinsFacade.SaveApplication(dto, SessionHelper.UserProfile.UserId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SaveCaDepositApplication(long Id, ApplicationStage fromApplicationStage)
        {
            try
            {
                var result = _applicationFacade.LockCurrentHolding(Id, fromApplicationStage, SessionHelper.UserProfile.UserId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        //public JsonResult SaveCaLoanApplication(long Id, ApplicationStage fromApplicaitonStage)
        //{
        //    try
        //    {
        //        var result = _applicationFacade.LockCurrentHolding(Id, fromApplicaitonStage, SessionHelper.UserProfile.UserId);
        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(ex, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public JsonResult SaveApprovedCreditMemoCurrentHoldings(ApplicationDto dto)
        {
            try
            {
                var result = _operatinsFacade.SaveApprovedCreditMemoCurrentHoldings(dto, SessionHelper.UserProfile.UserId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SaveReleseApplication(long Id, ApplicationStage fromApplicationStage)
        {
            try
            {
                var result = _applicationFacade.ReleseCurrentHolding(Id, fromApplicationStage, SessionHelper.UserProfile.UserId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SaveFundConfirm(FundConfirmationDto dto)
        {
            try
            {
                DateTime creditedDate = DateTime.Now;
                if (dto.Fundings != null)
                {
                    foreach (var item in dto.Fundings)
                    {
                        var FromConvertedCreditDate = DateTime.TryParseExact(item.CreditDateText, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out creditedDate);
                        if (FromConvertedCreditDate)
                            item.CreditDate = creditedDate;
                    }
                }

                var userId = SessionHelper.UserProfile.UserId;
                var result = _operatinsFacade.SaveFundConfirm(dto, userId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult SaveCbsInfo(CBSInfoDto dto)
        {

            if (dto.AccountOpenDateTxt != null)
            {
                DateTime accntOpenDate = DateTime.Now;
                var fromConverted = DateTime.TryParseExact(dto.AccountOpenDateTxt, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out accntOpenDate);
                if (fromConverted)
                {
                    dto.AccountOpenDate = accntOpenDate;
                }
            }

            if (dto.InstrumentDateText != null)
            {
                DateTime instrumentDate = DateTime.Now;
                var fromConverted = DateTime.TryParseExact(dto.InstrumentDateText, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out instrumentDate);
                if (fromConverted)
                {
                    dto.InstrumentDate = instrumentDate;
                }
            }

            if (dto.MaturityDateTxt != null)
            {
                DateTime maturityDate = DateTime.Now;
                var fromConverted = DateTime.TryParseExact(dto.MaturityDateTxt, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out maturityDate);
                if (fromConverted)
                {
                    dto.MaturityDate = maturityDate;
                }
            }

            var result = _operatinsFacade.SaveCBSInfo(dto, SessionHelper.UserProfile.UserId);
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        public JsonResult LoadFundConfirmationByAppId(long AppId)
        {
            var fundConfirmId = _operatinsFacade.GetFundConfirmIdByAppId(AppId);

            var depApp = _operatinsFacade.LoadFundConfirmationByAppId(AppId, fundConfirmId);
            return Json(depApp, JsonRequestBehavior.AllowGet);


        }

        //public JsonResult LoadFundConfirmationByAppId(long AppId, long Id)
        //{
        //    var proposalId = _crm.GetProposalByAppId(AppId);
        //    var depApp = _operatinsFacade.LoadFundConfirmationByAppId(AppId, proposalId, Id);
        //    return Json(depApp, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult FundReceivedList(string sortOrder, string searchString, int page = 1)
        //{
        //    ViewBag.SearchString = searchString;
        //    ViewBag.CurrentSort = sortOrder;
        //    var model = _operatinsFacade.GetOperationFundReceived(20, page, searchString, SessionHelper.UserProfile.UserId);
        //    return View(model);
        //}

        public ActionResult FundReceivedList(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _operatinsFacade.GetOperationFundReceived(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }

        public ActionResult CaFundReceivedList(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _operatinsFacade.GetCaFundReceived(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }

        public ActionResult ApplicationsTrackList(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _operatinsFacade.GetApplicationsTrackList(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }

        public ActionResult ApplicationsTrackListUserWise(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _operatinsFacade.GetApplicationsTrackListUserWise(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }

        public ActionResult AccountOpeningList(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _operatinsFacade.GetApplicationsOpeningList(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }

        public ActionResult CaAccountOpeningList(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _operatinsFacade.GetCaApplicationsOpeningList(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }

        public ActionResult ApplicationCifList()
        {
            return View();
        }
        public ActionResult CbsInfo()
        {
            return View();
        }
        public ActionResult CifListAppSummery()
        {
            return View();
        }
        public JsonResult GetAllCifByAppId(long AppId)
        {
            var allCif = _operatinsFacade.GetAllCifByAppId(AppId);
            return Json(allCif, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCBSData(long AppId)
        {
            var allCif = _operatinsFacade.GetAllCifByAppId(AppId);
            var cbsData = _operatinsFacade.GetCBSInfoForApplication(AppId);
            return Json(new { CIFs = allCif, CBSInfo = cbsData }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllCifOrgByAppId(long AppId)
        {
            var allCif = _operatinsFacade.GetAllCifOrgByAppId(AppId);
            return Json(allCif, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInstrumentStateList()
        {
            var data = _enum.GetInstrumentStateList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OpenedDepositAccountsList(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _operatinsFacade.GetOpendDepositAccountsList(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }

        public JsonResult GetDocumentStatusList()
        {
            var data = _enum.GetDocumentStatusList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult LoadDocumentCheckList(long? proposalId, long? id)
        public JsonResult LoadDocumentCheckList(long? AppId)//, long? id)
        {
            if (AppId != null)
            {
                var dockChkListId = _operatinsFacade.GetDocCheckListIdByAppId((long)AppId);
                var proposalId = _crmFacade.GetProposalByAppId(AppId);
                if (dockChkListId > 0 && proposalId > 0)
                {
                    var data = _operatinsFacade.LoadDocumentCheckList(AppId, proposalId, dockChkListId);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = _operatinsFacade.LoadDocumentCheckList(AppId, proposalId, dockChkListId);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }

            }
            return null;


        }

        public JsonResult LoadDocumentCheckListForDeposit(long? AppId)//, long? id)
        {
            if (AppId != null)
            {
                var dockChkListId = _operatinsFacade.GetDocCheckListIdByAppId((long)AppId);
                if (dockChkListId > 0)
                {
                    var data = _operatinsFacade.LoadDocumentCheckListForDeposit(AppId, dockChkListId);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = _operatinsFacade.LoadDocumentCheckListForDeposit(AppId, dockChkListId);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }

            }
            return null;


        }
        //SaveDocumentCheckList
        public JsonResult SaveDocumentCheckList(DocumentCheckListDto dto)
        {
            try
            {
                DateTime creditedDate = DateTime.Now;
                var DclDate = DateTime.TryParseExact(dto.DCLDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out creditedDate);
                if (DclDate)
                    dto.DCLDate = creditedDate;
                if (dto.Documents != null)
                {
                    foreach (var item in dto.Documents)
                    {
                        var FromConvertedCreditDate = DateTime.TryParseExact(item.CollectionDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out creditedDate);
                        if (FromConvertedCreditDate)
                            item.CollectionDate = creditedDate;

                    }
                }
                if (dto.Exceptions != null)
                {
                    foreach (var item in dto.Exceptions)
                    {
                        var FromConvertedCreditDate = DateTime.TryParseExact(item.CollectionDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out creditedDate);
                        if (FromConvertedCreditDate)
                            item.CollectionDate = creditedDate;
                        var FromObtainedDate = DateTime.TryParseExact(item.ObtainedDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out creditedDate);
                        if (FromObtainedDate)
                            item.ObtainedDate = creditedDate;
                    }
                }

                var userId = SessionHelper.UserProfile.UserId;
                var result = _operatinsFacade.SaveDocumentCheckList(dto, userId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.DenyGet);
            }
        }

        public JsonResult SaveApprovedDocumentCheckList(DocumentCheckListDto dto)
        {
            try
            {
                //DateTime creditedDate = DateTime.Now;
                //var DclDate = DateTime.TryParseExact(dto.DCLDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out creditedDate);
                //if (DclDate)
                //    dto.DCLDate = creditedDate;
                //if (dto.Documents != null)
                //{
                //    foreach (var item in dto.Documents)
                //    {
                //        var FromConvertedCreditDate = DateTime.TryParseExact(item.CollectionDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out creditedDate);
                //        if (FromConvertedCreditDate)
                //            item.CollectionDate = creditedDate;
                //    }
                //}
                //if (dto.Exceptions != null)
                //{
                //    foreach (var item in dto.Exceptions)
                //    {
                //        var FromConvertedCreditDate = DateTime.TryParseExact(item.CollectionDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out creditedDate);
                //        if (FromConvertedCreditDate)
                //            item.CollectionDate = creditedDate;
                //    }
                //}

                var userId = SessionHelper.UserProfile.UserId;
                var result = _operatinsFacade.SaveApprovedDocumentCheckList(dto, userId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.DenyGet);
            }
        }
        public ActionResult DocumentCheckList()
        {
            return View();
        }
        public ActionResult DocumentExceptionList(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _operatinsFacade.GetDocumentExceptionList(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }
        public JsonResult LoadDocumentCheckListById(long? id)
        {
            var data = _operatinsFacade.LoadDocumentCheckListById(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DCLReport(string reportTypeId, long dclId)
        {
            var dclDto = _operatinsFacade.LoadDocumentCheckListById(dclId);//_crmFacade.LoadProposalById(ProposalId);
            List<DocumentCheckListDto> documentCheckList = new List<DocumentCheckListDto>();
            documentCheckList.Add(dclDto);
            LocalReport lr = new LocalReport();
            string path = "";
            path = Path.Combine(Server.MapPath("~/Areas/IPDC/Reports"), "DSLReport.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("DocumentCheckList");
            }
            ReportDataSource rd = new ReportDataSource("DocumentCheckList", documentCheckList);
            ReportDataSource rd1 = new ReportDataSource("DocumentExceptions", dclDto.Exceptions);
            ReportDataSource rd2 = new ReportDataSource("Documents", dclDto.Documents);
            ReportDataSource rd3 = new ReportDataSource("dsProposalSignatory", dclDto.Signatories);
            lr.DataSources.Add(rd);
            lr.DataSources.Add(rd1);
            lr.DataSources.Add(rd2);
            lr.DataSources.Add(rd3);

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
        public ActionResult LoanApplicationDCL()
        {
            return View();
        }
        public ActionResult DCLLoanReport(string reportTypeId, long dclId)
        {
            var dclDto = _operatinsFacade.LoadDocumentCheckListById(dclId);//_crmFacade.LoadProposalById(ProposalId);
            List<DocumentCheckListDto> documentCheckList = new List<DocumentCheckListDto>();
            documentCheckList.Add(dclDto);
            LocalReport lr = new LocalReport();
            string path = "";
            string CustomRefNo = "IPDC/DCL/";
            string CustomCMRefNo = "IPDC/CM/";
            string CustomOLRefNo = "IPDC/OL/";
            if (dclDto.FacilityType == ProposalFacilityType.Auto_Loan)
            {
                CustomRefNo += "AL/" + dclDto.ApplicationDate.Value.Year + "/" + dclDto.ApplicationNo;
                CustomCMRefNo += "AL/" + dclDto.ApplicationDate.Value.Year + "/" + dclDto.ApplicationNo;
                CustomOLRefNo += "AL/" + dclDto.ApplicationDate.Value.Year + "/" + dclDto.ApplicationNo;
            }
            else if (dclDto.FacilityType == ProposalFacilityType.Home_Loan)
            {
                CustomRefNo += "HL/" + dclDto.ApplicationDate.Value.Year + "/" + dclDto.ApplicationNo;
                CustomCMRefNo += "HL/" + dclDto.ApplicationDate.Value.Year + "/" + dclDto.ApplicationNo;
                CustomOLRefNo += "HL/" + dclDto.ApplicationDate.Value.Year + "/" + dclDto.ApplicationNo;
            }
            else if (dclDto.FacilityType == ProposalFacilityType.Personal_Loan)
            {
                CustomRefNo += "PL/" + dclDto.ApplicationDate.Value.Year + "/" + dclDto.ApplicationNo;
                CustomCMRefNo += "PL/" + dclDto.ApplicationDate.Value.Year + "/" + dclDto.ApplicationNo;
                CustomOLRefNo += "PL/" + dclDto.ApplicationDate.Value.Year + "/" + dclDto.ApplicationNo;
            }
            else if (dclDto.FacilityType == ProposalFacilityType.RLS)
            {
                CustomRefNo += "SOD/" + dclDto.ApplicationDate.Value.Year + "/" + dclDto.ApplicationNo;
                CustomCMRefNo += "SOD/" + dclDto.ApplicationDate.Value.Year + "/" + dclDto.ApplicationNo;
                CustomOLRefNo += "SOD/" + dclDto.ApplicationDate.Value.Year + "/" + dclDto.ApplicationNo;
            }
            path = Path.Combine(Server.MapPath("~/Areas/IPDC/Reports"), "DSLLoanReport.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("LoanApplicationDCL");
            }
            ReportDataSource rd = new ReportDataSource("DocumentCheckList", documentCheckList);
            ReportDataSource rd1 = new ReportDataSource("DocumentExceptions", dclDto.Exceptions);
            ReportDataSource rd2 = new ReportDataSource("Documents", dclDto.Documents);
            ReportDataSource rd3 = new ReportDataSource("DocumentSecurities", dclDto.Securities);
            ReportDataSource rd4 = new ReportDataSource("dsProposalSignatory", dclDto.Signatories);
            

            ReportParameter rp1 = new ReportParameter("CustomCMRefNo", CustomCMRefNo);
            ReportParameter rp2 = new ReportParameter("CustomRefNo", CustomRefNo);
            ReportParameter rp3 = new ReportParameter("CustomOLRefNo", CustomOLRefNo);
            lr.DataSources.Add(rd);
            lr.DataSources.Add(rd1);
            lr.DataSources.Add(rd2);
            lr.DataSources.Add(rd3);
            lr.DataSources.Add(rd4);
            lr.SetParameters(new ReportParameter[] { rp1, rp2, rp3 });

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
        public ActionResult PreparePOandApproval(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _operatinsFacade.GetApplicationPOList(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }

        public ActionResult DepositApplicationTracking()
        {
            return View();
        }

        public JsonResult SaveDepositAppTracking(DepositApplicationTrackingDto dto)
        {
            try
            {
                if (!string.IsNullOrEmpty(dto.ChangeDateText))
                {
                    DateTime changeDate;
                    var fromConverted = DateTime.TryParseExact(dto.ChangeDateText, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out changeDate);
                    if (fromConverted)
                        dto.ChangeDate = changeDate;
                }
                var result = _operatinsFacade.SaveDepositAppTracking(dto, SessionHelper.UserProfile.UserId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetWelcomeLetterStatus()
        {
            var data = _enum.GetWelcomeLetterStatus();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //, long DepAppId, long Id
        public JsonResult LoadDepositAppTrackingbyAppId(long AppId, long DepAppId)
        {
            var trackingId = _operatinsFacade.GetTrackingIdByAppId(AppId);

            var depApp = _operatinsFacade.LoadDepositAppTrackingbyAppId(AppId, DepAppId, trackingId);
            return Json(depApp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PurchaseOrder()
        {
            return View();
        }

        public JsonResult LoadPurchaseOrder(long? proposalId, long? appId, long? id)
        {
            var loanApp = _operatinsFacade.LoadPurchaseOrder(proposalId, appId, id);
            return Json(loanApp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SavePurchaseOrder(PurchaseOrderDto dto)
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
                var result = _operatinsFacade.SavePurchaseOrder(dto, SessionHelper.UserProfile.UserId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PurchaseOrderReport(string reportTypeId, long poId)
        {
            var poDto = _operatinsFacade.LoadPurchaseOrder(null, null, poId);//_crmFacade.LoadProposalById(ProposalId);
            List<PurchaseOrderDto> poList = new List<PurchaseOrderDto>();
            poList.Add(poDto);
            LocalReport lr = new LocalReport();
            string CustomRefNo = "IPDC/PO/AL/"+poDto.ApplicationDate.Value.Year+"/"+poDto.ApplicationNo;
            string path = "";
            path = Path.Combine(Server.MapPath("~/Areas/IPDC/Reports"), "POReport.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("PurchaseOrder");
            }
            ReportDataSource rd = new ReportDataSource("PurchaseOrder", poList);
            ReportDataSource rd1 = new ReportDataSource("PODocument", poDto.Documents);
            ReportParameter rp1 = new ReportParameter("CustomRefNo", CustomRefNo);
            lr.DataSources.Add(rd);
            lr.DataSources.Add(rd1);
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
        //SavePOApproval
        public JsonResult SavePOApproval(PoApprovalDto dto)
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
                var result = _operatinsFacade.SavePOApproval(dto, SessionHelper.UserProfile.UserId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AppSummeryReportLoan(string reportTypeId, long AppId, long? ProposalId)
        {
            var proposalDto = _crmFacade.LoadProposalByAppId(AppId, ProposalId);
            List<ProposalDto> proposalDummyList = new List<ProposalDto>();

            var offerletterId = _crmFacade.GetOfferLetterId((long)proposalDto.Id);
            var offerLetterDto = _crmFacade.LoadOfferLetter(proposalDto.Id, offerletterId);
            List<OfferLetterDto> offerLetterList = new List<OfferLetterDto>();
            
            var appDto = _applicationFacade.LoadApplicationByAppId(AppId);
            List<ApplicationDto> applicationDummyList = new List<ApplicationDto>();

            var appCifsDto = _operatinsFacade.GetAllAppCifInfo(AppId);
            List<ApplicationCIFsDto> applicationCiFsList = new List<ApplicationCIFsDto>();

            var loanApplicationDto = _applicationFacade.LoadLoanAppByAppId(AppId);
            List<LoanApplicationDto> loanApplList = new List<LoanApplicationDto>();

            proposalDummyList.Add(proposalDto);
            applicationDummyList.Add(appDto);
            loanApplList.Add(loanApplicationDto);
            offerLetterList.Add(offerLetterDto);

            if (appCifsDto.Count >= 1)
            {
                foreach (var cifs in appCifsDto)
                {
                    applicationCiFsList.Add(cifs);
                }
            }

            LocalReport lr = new LocalReport();
            string path = "";
            path = Path.Combine(Server.MapPath("~/Areas/IPDC/Reports"), "AppSummeryLoan.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }

            //string CustomRefNo = "IPDC/DCL/";
            string CustomCMRefNo = "IPDC/CM/";
            string CustomOLRefNo = "IPDC/OL/";
            if (proposalDto.FacilityType == ProposalFacilityType.Auto_Loan)
            {
                CustomCMRefNo += "AL/" + appDto.ApplicationDate.Year + "/" + appDto.ApplicationNo;
                CustomOLRefNo += "AL/" + appDto.ApplicationDate.Year + "/" + appDto.ApplicationNo;
            }
            else if (proposalDto.FacilityType == ProposalFacilityType.Home_Loan)
            {
                CustomCMRefNo += "HL/" + appDto.ApplicationDate.Year + "/" + appDto.ApplicationNo;
                CustomOLRefNo += "HL/" + appDto.ApplicationDate.Year + "/" + appDto.ApplicationNo;
            }
            else if (proposalDto.FacilityType == ProposalFacilityType.Personal_Loan)
            {
                CustomCMRefNo += "PL/" + appDto.ApplicationDate.Year + "/" + appDto.ApplicationNo;
                CustomOLRefNo += "PL/" + appDto.ApplicationDate.Year + "/" + appDto.ApplicationNo;
            }
            else if (proposalDto.FacilityType == ProposalFacilityType.RLS)
            {
                CustomCMRefNo += "SOD/" + appDto.ApplicationDate.Year + "/" + appDto.ApplicationNo;
                CustomOLRefNo += "SOD/" + appDto.ApplicationDate.Year + "/" + appDto.ApplicationNo;
            }

            ReportDataSource rd = new ReportDataSource("Application", applicationDummyList);
            ReportDataSource rd1 = new ReportDataSource("AppSummery", applicationCiFsList);
            ReportDataSource rd2 = new ReportDataSource("AppSumGurantors", loanApplicationDto.Guarantors);
            ReportDataSource rd3 = new ReportDataSource("CreditMemo", proposalDummyList);
            ReportDataSource rd4 = new ReportDataSource("OfferLetter", offerLetterList);
            ReportParameter rp1 = new ReportParameter("CustomCMRefNo", CustomCMRefNo);
            ReportParameter rp2 = new ReportParameter("CustomOLRefNo", CustomOLRefNo);

            lr.DataSources.Add(rd);
            lr.DataSources.Add(rd1);
            lr.DataSources.Add(rd2);
            lr.DataSources.Add(rd3);
            lr.DataSources.Add(rd4);
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

        public ActionResult AppSummeryReportDeposit(string reportTypeId, long AppId)
        {
            var depoAppDto = _applicationFacade.GetDepositApplicationbyAppId(AppId);

            var guardian = _applicationFacade.GetGuardianByDepoAppId((long)depoAppDto);
            var guardianCifs = _cifFacade.GetCifByguardiaId((long)guardian);
            List<CIF_PersonalDto> guardianCiFsList = new List<CIF_PersonalDto>();

            var appDto = _applicationFacade.LoadApplicationByAppId(AppId);
            List<ApplicationDto> applicationDummyList = new List<ApplicationDto>();

            var appCifsDto = _operatinsFacade.GetAllAppCifInfo(AppId);
            List<ApplicationCIFsDto> applicationCiFsList = new List<ApplicationCIFsDto>();

            var depositAppDto = _applicationFacade.LoadDepositAppByAppId(AppId);
            List<DepositApplicationDto> deposiApplList = new List<DepositApplicationDto>();
            
            applicationDummyList.Add(appDto);
            deposiApplList.Add(depositAppDto);
            guardianCiFsList.Add(guardianCifs);

            if (appCifsDto.Count >= 1)
            {
                foreach (var cifs in appCifsDto)
                {
                    applicationCiFsList.Add(cifs);
                }
            }

            LocalReport lr = new LocalReport();
            string path = "";
            path = Path.Combine(Server.MapPath("~/Areas/IPDC/Reports"), "AppSummeryDeposit.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }

            ReportDataSource rd = new ReportDataSource("Application", applicationDummyList);
            ReportDataSource rd1 = new ReportDataSource("AppSummery", applicationCiFsList);
            ReportDataSource rd2 = new ReportDataSource("DepositApplication", deposiApplList);
            ReportDataSource rd3 = new ReportDataSource("Nominees", depositAppDto.Nominees);
            ReportDataSource rd4 = new ReportDataSource("Guardians", guardianCiFsList);
            ReportDataSource rd5 = new ReportDataSource("ChequeDepositList", depositAppDto.ChequeDeposits);
            ReportDataSource rd6 = new ReportDataSource("CashDepositList", depositAppDto.CashDeposits);
            ReportDataSource rd7 = new ReportDataSource("TransferDepositList", depositAppDto.TransferDeposits);

            lr.DataSources.Add(rd);
            lr.DataSources.Add(rd1);
            lr.DataSources.Add(rd2);
            lr.DataSources.Add(rd3);
            lr.DataSources.Add(rd4);
            lr.DataSources.Add(rd5);
            lr.DataSources.Add(rd6);
            lr.DataSources.Add(rd7);

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

        public ActionResult ReadyForDisbursmentMemo(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _operatinsFacade.ReadyForDisbursmentMemo(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }
        #region DisbursementMemo
        public ActionResult DisbursementMemo()
        {
            return View();
        }
        public JsonResult SaveDisbursmentMemo(DisbursementMemoDto dto)
        {
            try
            {
                if (!string.IsNullOrEmpty(dto.DMDateTxt))
                {
                    DateTime changeDate;
                    var fromConverted = DateTime.TryParseExact(dto.DMDateTxt, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out changeDate);
                    if (fromConverted)
                        dto.DMDate = changeDate;
                }
                var result = _operatinsFacade.SaveDisbursmentMemo(dto, SessionHelper.UserProfile.UserId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult LoadDisbursmentMemo(long? proposalId, long? appId, long? id, long? parentId)
        {
            var loanApp = _operatinsFacade.LoadDisbursmentMemo(proposalId, appId, id, parentId);
            return Json(loanApp, JsonRequestBehavior.AllowGet);
        }
        #endregion
        public ActionResult OperationApproval()
        {
            return View();
        }
        //SaveDclApproval
        public JsonResult SaveDclApproval(long id, long? appId)
        {
            var loanApp = _operatinsFacade.SaveDclApproval(id, appId, SessionHelper.UserProfile.UserId);
            return Json(loanApp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SubmitToDisbursmentMemo(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _operatinsFacade.SubmitToDisbursmentMemo(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }
        //SaveOprApproval PreparedDisbursmentMemo
        public JsonResult SaveOprApproval(long? appId)
        {
            var loanApp = _operatinsFacade.SaveOprApproval(appId, SessionHelper.UserProfile.UserId);
            return Json(loanApp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DisbursmentMemoReport(string reportTypeId, long? proposalId, long? appId, long? id, long? parentId)
        {
            var disbursmentMemo = _operatinsFacade.LoadDisbursmentMemo(proposalId, appId, id, parentId);//_crmFacade.LoadProposalById(ProposalId);
            List<DisbursementMemoDto> disbursmentMemoList = new List<DisbursementMemoDto>();
            //List<GuarantorDto> guarantorList = new List<GuarantorDto>();
            disbursmentMemoList.Add(disbursmentMemo);
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Areas/IPDC/Reports"), "DisbursmentMemo.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("DisbursementMemo");
            }
            string CustomRefNo = "IPDC/DM/";
            CustomRefNo += disbursmentMemo.FacilityType == ProposalFacilityType.Auto_Loan ? "AL" :
                disbursmentMemo.FacilityType == ProposalFacilityType.Home_Loan ? "HL" :
                disbursmentMemo.FacilityType == ProposalFacilityType.Personal_Loan ? "PL" : "";
            CustomRefNo += "/" + disbursmentMemo.ApplicattionDate.Value.Year + "/" + disbursmentMemo.ApplicationNo;


            ReportDataSource rd = new ReportDataSource("disbursmentMemoList", disbursmentMemoList);
            ReportDataSource rd1 = new ReportDataSource("DmText", disbursmentMemo.Texts);
            ReportDataSource rd2 = new ReportDataSource("dsProposalSignatory", disbursmentMemo.Signatories);
            

            lr.DataSources.Add(rd);
            lr.DataSources.Add(rd1);
            lr.DataSources.Add(rd2);

            ReportParameter rp1 = new ReportParameter("CustomRefNo", CustomRefNo);
            ReportParameter rp2 = new ReportParameter("GeneratedBy", disbursmentMemo.CreatedByName);
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
        public ActionResult PreparedDisbursmentMemo(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _operatinsFacade.PreparedDisbursmentMemo(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }
        //SaveMemoApproval
        public JsonResult SaveMemoApproval(DisbursementMemoDto dto)
        {
            if (!string.IsNullOrEmpty(dto.ApprovalDateTxt))
            {
                DateTime changeDate;
                var fromConverted = DateTime.TryParseExact(dto.ApprovalDateTxt, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out changeDate);
                if (fromConverted)
                    dto.ApprovalDate = changeDate;
            }
            var loanApp = _operatinsFacade.SaveMemoApproval(dto, SessionHelper.UserProfile.UserId);
            return Json(loanApp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PartiallyDisbursedList(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _operatinsFacade.PartiallyDisbursedList(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }
        public ActionResult DisbursedLoanApplication(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _operatinsFacade.DisbursedLoanApplication(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }
        public ActionResult PreparedDisbursedList(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _operatinsFacade.PreparedDisbursedList(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }
        //public JsonResult SaveDisbursmentMemoApproval(DisbursementMemoDto dto)
        //{
        //    if (!string.IsNullOrEmpty(dto.DisbursedDateTxt))
        //    {
        //        DateTime changeDate;
        //        var fromConverted = DateTime.TryParseExact(dto.DisbursedDateTxt, "dd/MM/yyyy",
        //            CultureInfo.InvariantCulture, DateTimeStyles.None, out changeDate);
        //        if (fromConverted)
        //            dto.DisbursedDate = changeDate;
        //    }
        //    var loanApp = _operatinsFacade.SaveDisbursmentMemoApproval(dto, SessionHelper.UserProfile.UserId);
        //    return Json(loanApp, JsonRequestBehavior.AllowGet);
        //}
        //ApprovedDisbursedList
        public ActionResult ApprovedDisbursedList(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _operatinsFacade.ApprovedDisbursedList(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }
        public ActionResult DisbursementMemoDetails()
        {
            return View();
        }
        public JsonResult GetDisbursementModes()
        {
            var data = _enum.GetDisbursementModes();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetChequeDeliveryOptions()
        {
            var data = _enum.GetChequeDeliveryOptions();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveDisbursmentMemoDetails(DisbursementMemoDto dto)
        {
            try
            {
                if (!string.IsNullOrEmpty(dto.DisbursedDateTxt))
                {
                    DateTime changeDate;
                    var fromConverted = DateTime.TryParseExact(dto.DisbursedDateTxt, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out changeDate);
                    if (fromConverted)
                        dto.DisbursedDate = changeDate;
                }
                foreach (var item in dto.DisbursementDetails)
                {
                    if (!string.IsNullOrEmpty(item.ChequeDateTxt))
                    {
                        DateTime changeDate;
                        var fromConverted = DateTime.TryParseExact(item.ChequeDateTxt, "dd/MM/yyyy",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out changeDate);
                        if (fromConverted)
                            item.ChequeDate = changeDate;
                    }
                }
                var userId = SessionHelper.UserProfile.UserId;
                var result = _operatinsFacade.SaveDisbursmentMemoDetails(dto, userId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }
        //GetIPDCBankAccounts GetAllIpdcBankAccntiWithName LoadDMDetails
        public JsonResult GetIPDCBankAccounts()
        {
            var ipdcBankAccount = _applicationFacade.GetAllIpdcBankAccntiWithName();
            return Json(ipdcBankAccount, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadDMDetails(long dmId)
        {
            var dmDetails = _operatinsFacade.LoadDMDetails(dmId);
            return Json(dmDetails, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ApprovedRLSApplications(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _applicationFacade.GetApprovedRLSApplications(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }
        public JsonResult LoadApplicationByAppIdForCRM(long AppId)
        {
            var depApp = _operatinsFacade.LoadApplicationByAppIdForCRM(AppId, SessionHelper.UserProfile.UserId);
            return Json(depApp, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetExceptioApplications(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _applicationFacade.GetExceptioApplications(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }
        //DCLExceptions
        public ActionResult DCLExceptions()
        {
            return View();
        }
    }
}