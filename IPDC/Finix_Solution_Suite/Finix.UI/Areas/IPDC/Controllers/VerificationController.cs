using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Finix.Auth.Service;
using Finix.IPDC.DTO;
using Finix.IPDC.Facade;

namespace Finix.UI.Areas.IPDC.Controllers
{
    public class VerificationController : BaseController
    {
        private readonly VerificationFacade _verification = new VerificationFacade();
        private readonly ApplicationFacade _application = new ApplicationFacade();
        private readonly ProfessionFacade _professionFacade = new ProfessionFacade();
        private readonly CIFFacade _cifFacade = new CIFFacade();
        private readonly EnumFacade _enum = new EnumFacade();

        //Views
        public ActionResult LppPrimarySecurityValuation()
        {
            return View();
        }
        public ActionResult VehiclePrimarySecurityValuation()
        {
            return View();
        }
        public ActionResult ConsumerPrimarySecurityValuation()
        {
            return View();
        }
        public ActionResult CIB()
        {
            return View();
        }
        public ActionResult CPV()
        {
            return View();
        }
        public ActionResult NIDVerification()
        {
            return View();
        }
        public ActionResult IncomeVerification()
        {
            return View();
        }
        public ActionResult NetWorthVerification()
        {
            return View();
        }
        public ActionResult VisitReport()
        {
            return View();
        }
        public ActionResult CIBOrganizational()
        {
            return View();
        }
        public ActionResult ProjectLegalVerification()
        {
            return View();
        }
        public ActionResult ProjectTechnicalVerification()
        {
            return View();
        }
        public ActionResult CibVerificationHistory()
        {
            return View();
        }
        public ActionResult CPVVerificationHistory()
        {
            return View();
        }
        public ActionResult NetWorthVerificationHistory()
        {
            return View();
        }
        public ActionResult NIDVerificationHistory()
        {
            return View();
        }
        public ActionResult IncomeVerificationHistory()
        {
            return View();
        }
        public ActionResult VisitReportHistory()
        {
            return View();
        }
        public ActionResult LegalDocumentVerification()
        {
            return View();
        }
        public JsonResult GetVerificationAs()
        {
            var varificationAs = _enum.GetVerificationAs();
            return Json(varificationAs, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="cibdto"></param>
        /// <returns></returns>
        /// 
        //CIB-Personal
        [HttpPost]
        public JsonResult SaveCIBPersonal(CIB_PersonalDto cibdto)
        {
            var userId = SessionHelper.UserProfile.UserId;
            cibdto.VerifiedByUserId = userId;
            if (!string.IsNullOrEmpty(cibdto.VerificationDateTxt))
            {
                DateTime verificationDate = DateTime.Now;
                var FromConverted = DateTime.TryParseExact(cibdto.VerificationDateTxt, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out verificationDate);
                if (FromConverted)
                {
                    cibdto.VerificationDate = verificationDate;
                }
            }
            var verification = _verification.SaveCIBPersonal(cibdto, userId);
            return Json(verification, JsonRequestBehavior.AllowGet);
        }
        public FileResult Download(string fileBytes,string fileName)
        {
            //return File(imageName, System.Net.Mime.MediaTypeNames.Application.Octet);
            //byte[] fileBytes = new byte[] {};
            //string fileName = imageName;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        public JsonResult LoadCIBPersonal(long? AppId, long? CIFPId, long? Id)
        {
            var data = _verification.LoadCIBPersonal(AppId, CIFPId, Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadCibPersonalHistory(long? AppId, long CIFPId, int? CibType)
        {

            var data = _verification.LoadCibHistory(AppId, CIFPId, CibType);
            return Json(data, JsonRequestBehavior.AllowGet);

        }
        //CIB-Org
        public JsonResult SaveCIBOrganizational(CIB_OrganizationalDto cibdto)
        {
            var userId = SessionHelper.UserProfile.UserId;
            cibdto.VerifiedByUserId = userId;
            if (!string.IsNullOrEmpty(cibdto.VerificationDateTxt))
            {
                DateTime verificationDate = DateTime.Now;
                var FromConverted = DateTime.TryParseExact(cibdto.VerificationDateTxt, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out verificationDate);
                if (FromConverted)
                {
                    cibdto.VerificationDate = verificationDate;
                }
            }
            var verification = _verification.SaveCIBOrganizational(cibdto, userId);
            return Json(verification, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadCIBOrganizational(long? AppId, long? CifOrgId, long? Id)
        {
            var data = _verification.LoadCIBOrganizational(AppId, CifOrgId, Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //CPV
        public JsonResult SaveCpvVerification(ContactPointVerificationDto valuation)
        {
            var userId = SessionHelper.UserProfile.UserId;
            valuation.VerifiedByUserId = userId;
            DateTime verificationDate;
            var fromConverted = DateTime.TryParseExact(valuation.VerificationDateText, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out verificationDate);
            if (fromConverted)
            {
                valuation.VerificationDate = verificationDate;
            }
            var verification = _verification.SaveCpvVerification(valuation, userId);
            return Json(verification, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadCpvById(long? AppId, long? CifId, long? CpvId)
        {
            var data = _verification.LoadCpvById(AppId, CifId, CpvId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadCpvHIstoryById(long? AppId, long CifId)
        {
            var data = _verification.LoadCpvHIstoryById(AppId, CifId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //NetWorth
        public JsonResult SaveNetWorthVerification(NetWorthVerificationDto cibdto)
        {
            var userId = SessionHelper.UserProfile.UserId;
            cibdto.VerifiedByUserId = userId;
            if (!string.IsNullOrEmpty(cibdto.VerificationDateTxt))
            {
                DateTime verificationDate = DateTime.Now;
                var FromConverted = DateTime.TryParseExact(cibdto.VerificationDateTxt, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out verificationDate);
                if (FromConverted)
                {
                    cibdto.VerificationDate = verificationDate;
                }
            }
            var verification = _verification.SaveNetWorthVerification(cibdto, userId);
            return Json(verification, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadNetWorthVerification(long? AppId, long? CIFPId, long? Id)
        {
            if (CIFPId != null)
            {
                var netWorthId = _cifFacade.GetNetWorthIdbyCifId((long)CIFPId);
                var data = _verification.LoadNetWorthVerification(AppId, netWorthId, CIFPId, Id);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }
        public JsonResult LoadNWVerificationlHistory(long? AppId, long? NetWorthId, long? CIFPId)
        {
            var data = _verification.LoadNWVerificationlHistory(AppId, NetWorthId, CIFPId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //NID
        public JsonResult SaveNidVerification(NIDVerificationDto dto)
        {
            var userId = SessionHelper.UserProfile.UserId;
            dto.VerifiedByUserId = userId;
            DateTime verificationDate;
            DateTime dob;
            var fromConverted = DateTime.TryParseExact(dto.VerificationDateText, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out verificationDate);
            if (fromConverted)
            {
                dto.VerificationDate = verificationDate;
            }
            fromConverted = DateTime.TryParseExact(dto.DateOfBirthText, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dob);
            if (fromConverted)
            {
                dto.DateOfBirth = dob;
            }
            var verification = _verification.SaveNidVerification(dto, SessionHelper.UserProfile.UserId);
            return Json(verification, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadNidVerification(long? AppId, long? CIFPId, long? Id)
        {
            var result = _verification.LoadNidVerification(AppId, CIFPId, Id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadNIDHIstoryById(long? AppId, long CifId)
        {
            var result = _verification.LoadNIDHIstoryById(AppId, CifId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //Income Statement
        public JsonResult SaveIncomeVerification(IncomeVerificationDto dto)
        {
            var userId = SessionHelper.UserProfile.UserId;
            DateTime verificationDate;
            var fromConverted = DateTime.TryParseExact(dto.IncomeAssessmentDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out verificationDate);
            if (fromConverted)
            {
                dto.IncomeAssessmentDate = verificationDate;
            }
            var verification = _verification.SaveIncomeVerification(dto, userId);
            return Json(verification, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadIncomeHistory(long? AppId, long CifId)
        {
            var data = _verification.LoadIncomeHistory(AppId, CifId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadIncomeStatementVerification(long? AppId, long? CIFPId, long? Id)
        {
            var result = _verification.LoadIncomeStatementVerification(AppId, CIFPId, Id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //VisitReport
        [HttpPost]
        public JsonResult SaveVisitReport(VisitReportDto dto)
        {
            var userId = SessionHelper.UserProfile.UserId;
            DateTime verificationDate;
            var fromConverted = DateTime.TryParseExact(dto.VisitTimeText, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out verificationDate);
            if (fromConverted)
            {
                dto.VisitTime = verificationDate;
            }
            var verification = _verification.SaveVisitReport(dto, userId);
            return Json(verification, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadVisitReport(long? AppId, long? CIFPId, long? Id)
        {
            var result = _verification.LoadVisitReport(AppId, CIFPId, Id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadVisitReportHistory(long? AppId, long CIFPId)
        {
            var data = _verification.LoadVisitReportHistory(AppId, CIFPId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //Project Legal Verification
        public JsonResult SaveProjectLegalVerification(ProjectLegalVerificationDto dto)
        {
            DateTime titleDeedDate = DateTime.Now;
            if (dto.Owners != null)
            {
                foreach (var item in dto.Owners)
                {
                    var FromConvertedDeedDate = DateTime.TryParseExact(item.TitleDeedDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out titleDeedDate);
                    if (FromConvertedDeedDate)
                        item.TitleDeedDate = titleDeedDate;
                }
            }

            var userId = SessionHelper.UserProfile.UserId;
            var saveCif = _verification.SaveProjectLegalVerification(dto, userId);
            return Json(saveCif, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadProjectLegalVerification(long? ProjectId, long? ProjectLegalId)
        {
            var data = _verification.LoadProjectLegalVerification(ProjectId, ProjectLegalId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //LPPrimary Security
        public JsonResult SaveVerification(LPPrimarySecurityValuationDto valuation)
        {
            var userId = SessionHelper.UserProfile.UserId;
            valuation.VerifiedByUserId = userId;
            var verification = _verification.SaveLPVerification(valuation, SessionHelper.UserProfile.UserId);
            return Json(verification, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadLPPrimarySecurityValuation(long id)
        {
            var data = _verification.LoadLPPrimarySecurityValuation(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //VehiclePrimarySecurity
        public JsonResult SaveVehiclePrimarySecurityValuation(VehiclePrimarySecurityValuationDto valuation)
        {
            var userId = SessionHelper.UserProfile.UserId;
            valuation.VerifiedByUserId = userId;
            DateTime verificationDate = DateTime.Now;
            var fromConverted = DateTime.TryParseExact(valuation.VerificationDateText, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out verificationDate);
            if (fromConverted)
            {
                valuation.VerificationDate = verificationDate;
            }
            var verification = _verification.SaveVehiclePrimarySecurityValuation(valuation, SessionHelper.UserProfile.UserId);
            return Json(verification, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadVehiclePrimarySecurity(long id)
        {
            var data = _verification.LoadVehiclePrimarySecurity(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //ProjectTechnicalVerification
        [HttpPost]
        public JsonResult SaveProjectTechnicalVerification(ProjectTechnicalVerificationDto dto)
        {
            var technicalVrification = _verification.SaveProjectTechnicalVerification(dto, SessionHelper.UserProfile.UserId);
            return Json(technicalVrification, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadProjectTechnicalData(long? id)
        {
            var result = _verification.LoadProjectTechnicalData(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //Consumer Primary Security
        public JsonResult SaveConsumerPrimarySecurityValuation(ConsumerGoodsPrimarySecurityValuationDto valuation)
        {
            var userId = SessionHelper.UserProfile.UserId;
            valuation.VerifiedByUserId = userId;
            DateTime verificationDate = DateTime.Now;
            var fromConverted = DateTime.TryParseExact(valuation.VerificationDateText, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out verificationDate);
            if (fromConverted)
            {
                valuation.VerificationDate = verificationDate;
            }
            var verification = _verification.SaveConsumerPrimarySecurityValuation(valuation, SessionHelper.UserProfile.UserId);
            return Json(verification, JsonRequestBehavior.AllowGet);
        }
        //Legal Document Verification
        public JsonResult SaveLegalDocumentVerification(LegalDocumentVerificationDto dto)
        {
            try
            {
                var userId = SessionHelper.UserProfile.UserId;
                var result = _verification.SaveLegalDocumentVerification(dto, userId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.DenyGet);
            }
        }
        public JsonResult LoadConsumerPrimarySecurity(long id)
        {
            var data = _verification.LoadConsumerPrimarySecurity(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadLegalDocumentCheckList(long? AppId, long? id)
        {
            var data = _verification.LoadLegalDocumentCheckList(AppId, id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <returns></returns>
        #region General Functions
        public JsonResult GetCIBClassificationStatus()
        {
            var varificationAs = _enum.GetClassificationStatus();
            return Json(varificationAs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLoanApplicationById(long AppId)
        {
            var primarySecurity = _application.LoadLoanAppByAppId(AppId);
            return Json(primarySecurity, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProjectStatuses()
        {
            var primarySecurity = _enum.GetProjectStatuses();
            return Json(primarySecurity, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApplicantsFlatStatuses()
        {
            var primarySecurity = _enum.GetApplicantsFlatStatuses();
            return Json(primarySecurity, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVerificationStates()
        {
            var verificationState = _enum.GetVerificationStates();
            return Json(verificationState, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetValuationType()
        {
            var primarySecurity = _enum.GetValuationType();
            return Json(primarySecurity, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLocationFindibility()
        {
            var data = _enum.GetLocationFindibility();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetYearsCurrentResidence()
        {
            var data = _enum.GetYearsCurrentResidence();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRelationshipWithApplicant()
        {
            var data = _enum.GetRelationshipWithApplicant();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllProfession()
        {
            var data = _professionFacade.GetAllProfession();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVerificationStatuses()
        {
            var data = _enum.GetVerificationStatuses();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProjectApprovalAuthority()
        {
            var data = _enum.GetProjectApprovalAuthority();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPropertyType()
        {
            var data = _enum.GetPropertyType();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPropertyBounds()
        {
            var data = _enum.GetPropertyBounds();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApprovalStatus()
        {
            var data = _enum.GetApprovalStatus();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLandTypes()
        {
            var data = _enum.GetLandTypes();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDocumentStatusList()
        {
            var data = _enum.GetDocumentStatusList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetHomeOwnerships()
        {
            var varificationAs = _enum.GetHomeOwnerships();
            return Json(varificationAs, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}