using Finix.IPDC.DTO;
using Finix.IPDC.Facade;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Util;
using System.Web.Mvc;
using System.Linq;
using Microsoft.Reporting.WebForms;

namespace Finix.UI.Areas.IPDC.Controllers
{
    public class CIFController : BaseController
    {
        private readonly CIFFacade _cifFacade = new CIFFacade();
        private readonly EnumFacade _enumFacade = new EnumFacade();
        private readonly VerificationFacade _verificationFacade = new VerificationFacade();
        private readonly OrganizationFacade _organizationFacade = new OrganizationFacade();

        //private readonly AddressDto _address = new AddressDto();

        public ActionResult CIF_Personal()
        {
            return View();
        }
        public JsonResult SaveCifIffo(CIF_PersonalDto dto)
        {
            if (!string.IsNullOrEmpty(dto.DateOfBirthText))
            {
                DateTime dob = DateTime.Now;
                var FromConverted = DateTime.TryParseExact(dto.DateOfBirthText, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dob);
                if (FromConverted)
                {
                    dto.DateOfBirth = dob;
                }
            }
            if (!string.IsNullOrEmpty(dto.PassportIssueDateText))
            {
                DateTime passportIssue = DateTime.Now;
                var FromConverted = DateTime.TryParseExact(dto.PassportIssueDateText, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out passportIssue);
                if (FromConverted)
                {
                    dto.PassportIssueDate = passportIssue;
                }
            }
            if (!string.IsNullOrEmpty(dto.CommCertIssueDateText))
            {
                DateTime CommCerIssue = DateTime.Now;
                var FromConverted = DateTime.TryParseExact(dto.CommCertIssueDateText, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out CommCerIssue);
                if (FromConverted)
                {
                    dto.CommCertIssueDate = CommCerIssue;
                }
            }
            if (!string.IsNullOrEmpty(dto.DLIssueDateTxt))
            {
                DateTime dlIssue = DateTime.Now;
                var FromConverted = DateTime.TryParseExact(dto.DLIssueDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dlIssue);
                if (FromConverted)
                {
                    dto.DLIssueDate = dlIssue;
                }
            }
            var userId = SessionHelper.UserProfile.UserId;
            var saveCif = _cifFacade.SaveCifPersonal(dto, userId);
            return Json(saveCif, JsonRequestBehavior.AllowGet);
        }
        // CIF Income Statement
        public ActionResult CIFIncomeStatement()
        {
            return View();
        }
        public JsonResult SaveCIFIncomeStatement(CIF_IncomeStatementDto dto)
        {
            try
            {
                var result = _cifFacade.SaveCIFIncomeStatement(dto, SessionHelper.UserProfile.UserId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }
        // CIF Refernece
        public ActionResult CIFReferences()
        {
            return View();
        }
        public JsonResult SaveCIFReference(List<CIF_ReferenceDto> dto)
        {
            try
            {
                //DateTime assessmentDate = DateTime.Now;
                //var FromConverted = DateTime.TryParseExact(dto.IncomeAssessmentDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out assessmentDate);
                //if (FromConverted)
                //    dto.IncomeAssessmentDate = assessmentDate;
                var userId = SessionHelper.UserProfile.UserId;
                var result = _cifFacade.SaveCIFReference(dto, userId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CIFNetWorth()
        {
            return View();
        }
        public JsonResult SaveCIFNetWorth(CIF_NetWorthDto dto)
        {
            try
            {
                //DateTime assessmentDate = DateTime.Now;
                //var FromConverted = DateTime.TryParseExact(dto.IncomeAssessmentDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out assessmentDate);
                //if (FromConverted)
                //    dto.IncomeAssessmentDate = assessmentDate;
                var userId = SessionHelper.UserProfile.UserId;
                var result = _cifFacade.SaveCIFNetWorth(dto, userId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CIFCards()
        {
            return View();
        }
        [HttpPost]
        public JsonResult SaveCIFCards(CIFCardAndBanksVM dto)
        {
            if (dto.CreditCardDtos != null)
            {
                foreach (var item in dto.CreditCardDtos)
                {
                    DateTime issueDate = DateTime.Now.Date;
                    var FromConverted = DateTime.TryParseExact(item.CreditCardIssueDateText, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out issueDate);
                    if (FromConverted)
                        item.CreditCardIssueDate = issueDate;

                }
            }
            dto.UserId = SessionHelper.UserProfile.UserId;
            var result = _cifFacade.SaveBankAccountsAndCreditCards(dto);
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        [HttpGet]
        public JsonResult GetCIFCardsByCIFId(long cifId)
        {
            var result = _cifFacade.GetBankAccountsAndCreditCards(cifId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCIF_Info(long cifId)
        {
            var data = _cifFacade.GetCIF_Info(cifId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCIFReference(long cifId)
        {
            var data = _cifFacade.GetCIFReference(cifId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCIFOrganizational(long cifOrgId)
        {
            var data = _cifFacade.GetCIFOrganizational(cifOrgId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CIF_Edit()
        {
            return View();
        }
        public ActionResult Client_Occupation()
        {
            return View();
        }
        public JsonResult GetLegalStatus()
        {
            var typeList = Enum.GetValues(typeof(CompanyLegalStatus))
               .Cast<CompanyLegalStatus>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = UiUtil.GetDisplayName(t)
               });
            return Json(typeList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBankDepositTypes()
        {
            var typeList = Enum.GetValues(typeof(BankDepositType))
               .Cast<BankDepositType>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = UiUtil.GetDisplayName(t)
               });
            //var typeList = UiUtil.EnumToKeyVal<HomeOwnership>();

            return Json(typeList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInvestmentTypes()
        {
            var typeList = Enum.GetValues(typeof(InvestmentType))
               .Cast<InvestmentType>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = UiUtil.GetDisplayName(t)
               });
            //var typeList = UiUtil.EnumToKeyVal<HomeOwnership>();

            return Json(typeList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBusinessShareTypes()
        {
            var typeList = Enum.GetValues(typeof(BusinessShareType))
               .Cast<BusinessShareType>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = UiUtil.GetDisplayName(t)
               });
            //var typeList = UiUtil.EnumToKeyVal<HomeOwnership>();

            return Json(typeList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLoanTypes()
        {
            var typeList = Enum.GetValues(typeof(LoanType))
               .Cast<LoanType>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = UiUtil.GetDisplayName(t)
               });
            //var typeList = UiUtil.EnumToKeyVal<HomeOwnership>();

            return Json(typeList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLiabilityTypes()
        {
            var typeList = Enum.GetValues(typeof(LiabilityType))
               .Cast<LiabilityType>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = UiUtil.GetDisplayName(t)
               });
            //var typeList = UiUtil.EnumToKeyVal<HomeOwnership>();

            return Json(typeList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRoleInBankOrFL()
        {
            var typeList = Enum.GetValues(typeof(RoleInBankOrFL))
               .Cast<RoleInBankOrFL>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = t.ToString()
               });
            return Json(typeList, JsonRequestBehavior.AllowGet);
        }
        //RelationshipWithIPDC
        public JsonResult GetRelationshipWithIPDC()
        {
            var typeList = Enum.GetValues(typeof(RelationshipWithIPDC))
               .Cast<RelationshipWithIPDC>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = t.ToString()
               });
            return Json(typeList, JsonRequestBehavior.AllowGet);
        }
        //PropertyType
        public JsonResult GetPropertyType()
        {
            var typeList = Enum.GetValues(typeof(PropertyType))
               .Cast<PropertyType>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = t.ToString()
               });
            return Json(typeList, JsonRequestBehavior.AllowGet);
        }
        //SaveClientOrganization
        public JsonResult SaveClientOccupation(OccupationDto dto)
        {
            var userId = SessionHelper.UserProfile.UserId;
            var saveCif = _cifFacade.SaveClientOccupation(dto, userId);
            return Json(saveCif, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllCIFPersons()
        {
            var designations = _cifFacade.GetAllCIFPersons();
            return Json(designations, JsonRequestBehavior.AllowGet);
        }
        //PropertyType
        public JsonResult GetFollowupType()
        {
            var typeList = Enum.GetValues(typeof(FollowupType))
               .Cast<FollowupType>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = t.ToString()
               });
            return Json(typeList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllCif()
        {
            var allCif = _cifFacade.GetAllCif();
            return Json(allCif, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCIF_InfoWithAge()
        {
            var allCif = _cifFacade.GetCIF_InfoWithAge();
            return Json(allCif, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetAllCifOrgList()
        {
            var result = _cifFacade.GetAllCifOrgList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllCifOrgLegalStatus()
        {
            var riskLevels = _enumFacade.GetAllCifOrgLegalStatus();
            return Json(riskLevels, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllCifOrgOwnerRoles()
        {
            var riskLevels = _enumFacade.GetAllCifOrgOwnerRoles();
            return Json(riskLevels, JsonRequestBehavior.AllowGet);
        }
        // CIR Orgranizational
        public ActionResult CIFOraganizational()
        {
            return View();
        }

        public JsonResult SaveCifOrganizational(CIF_OrganizationalDto dto)
        {
            if (!string.IsNullOrEmpty(dto.TradeLicenceDateTxt))
            {
                DateTime dob = DateTime.Now;
                var FromConverted = DateTime.TryParseExact(dto.TradeLicenceDateTxt, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out dob);
                if (FromConverted)
                {
                    dto.TradeLicenceDate = dob;
                }
            }
            if (!string.IsNullOrEmpty(dto.RegistrationDateTxt))
            {
                DateTime passportIssue = DateTime.Now;
                var FromConverted = DateTime.TryParseExact(dto.RegistrationDateTxt, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out passportIssue);
                if (FromConverted)
                {
                    dto.RegistrationDate = passportIssue;
                }
            }
            if (!string.IsNullOrEmpty(dto.DateOfIncorporationTxt))
            {
                DateTime dateOfIncorporation = DateTime.Now;
                var FromConverted = DateTime.TryParseExact(dto.DateOfIncorporationTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfIncorporation);
                if (FromConverted)
                {
                    dto.DateOfIncorporation = dateOfIncorporation;
                }
            }

            var userId = SessionHelper.UserProfile.UserId;
            var saveCif = _cifFacade.SaveCifOrganizational(dto, userId);
            return Json(saveCif, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckIfHomeCountry(long id)
        {
            bool isHomeCountry = BizConstants.Bangladesh == id;
            return Json(isHomeCountry, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetCIF_Occupation(long? personId)
        {
            var occupation = _cifFacade.GetCIF_Occupation(personId);
            return Json(occupation, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadIncomeStatement(long cifPersonId)
        {
            var incomeStatement = _cifFacade.LoadIncomeStatement(cifPersonId);
            return Json(incomeStatement, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadNetWorth(long cifPersonId)
        {
            var incomeStatement = _cifFacade.LoadNetWorth(cifPersonId);
            return Json(incomeStatement, JsonRequestBehavior.AllowGet);
        }
        #region Search Client Information 
        public ActionResult SearchCIFInformation(string sortOrder, string currentFilter, string searchString, int page = 1)
        {
            ViewBag.CurrentSort = sortOrder;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var temp = _cifFacade.GetCifPagedList(20, page, searchString);
            return View(temp);
        }
        #endregion

        #region Search Client Organization 

        public ActionResult SearchClientOrg(string sortOrder, string currentFilter, string searchString, int page = 1)
        {
            ViewBag.CurrentSort = sortOrder;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var temp = _cifFacade.GetCifOrgPagedList(20, page, searchString);
            return View(temp);
        }
        #endregion

        //RemoveRef
        public JsonResult RemoveRef(long cid)
        {
            var allRef = _cifFacade.RemoveRef(cid);
            return Json(allRef, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CifSummeryReport(string reportTypeId, long cifId)
        {
            var cifDto = _cifFacade.GetCIF_Info(cifId);
            
            var nidVerifiedDto = _verificationFacade.GetVerifiedNIDCifId(cifId);
            var occupationDto = _cifFacade.GetCIF_Occupation(cifId);
            occupationDto.ProfessionName = cifDto.ProfessionName;
            var incomeDto = _verificationFacade.GetVerifiedIncomeByCifId(cifId);

            //var residAddressDto = 


            //var verifiedIncomeDto = 
            List<CIF_PersonalDto> CifDummyList = new List<CIF_PersonalDto>();
            List<OccupationDto> CifOccupationList = new List<OccupationDto>();
            List<IncomeVerificationDto> VerifiedIncomeList = new List<IncomeVerificationDto>();
            List<NIDVerificationDto> VerifiedNidList = new List<NIDVerificationDto>();
            CifDummyList.Add(cifDto);
            CifOccupationList.Add(occupationDto);
            VerifiedIncomeList.Add(incomeDto);
            VerifiedNidList.Add(nidVerifiedDto);

            LocalReport lr = new LocalReport();
            //string path = "";
            string path = Path.Combine(Server.MapPath("~/Areas/IPDC/Reports"), "Cif_Summary.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("CIF_Personal");
            }


            ReportDataSource rd = new ReportDataSource("CifSummery", CifDummyList);
            ReportDataSource rd1 = new ReportDataSource("Occupation", CifOccupationList);
            ReportDataSource rd2 = new ReportDataSource("IncomeVerified", VerifiedIncomeList);
            ReportDataSource rd3 = new ReportDataSource("NidVerified", VerifiedNidList);
            lr.DataSources.Add(rd);
            lr.DataSources.Add(rd1);
            lr.DataSources.Add(rd2);
            lr.DataSources.Add(rd3);
            //lr.DataSources.Add(rd16);

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

        public ActionResult OrgCifSummeryReport(string reportTypeId, long cifId)
        {
            var orgCifDto = _cifFacade.GetCIFOrganizational(cifId);
            List<CIF_OrganizationalDto> CifOrgDummyList = new List<CIF_OrganizationalDto>();
            CifOrgDummyList.Add(orgCifDto);
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Areas/IPDC/Reports"), "OrgCifSummery.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return null;
            }
            ReportDataSource rd = new ReportDataSource("CifOrganization", CifOrgDummyList);
            ReportDataSource rd1 = new ReportDataSource("FactoryAddress", orgCifDto.FactoryAddress);
            ReportDataSource rd2 = new ReportDataSource("Owners", orgCifDto.Owners);
            lr.DataSources.Add(rd);
            lr.DataSources.Add(rd1);
            lr.DataSources.Add(rd2);
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
        [HttpPost]
        public JsonResult GetCIFPListForAutoFill(AutofillDto dto)
        {
            var result = _cifFacade.GetCifpForAutoFill(dto.prefix, dto.exclusionList);
            var data = result.Select(r => new { key = r.Id, value = r.CIFNo + " - " + r.Name }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBusinessTypes()
        {
            var riskLevels = _enumFacade.GetBusinessTypes();
            return Json(riskLevels, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBusinessSizes()
        {
            var riskLevels = _enumFacade.GetBusinessSizes();
            return Json(riskLevels, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCIF_Org_SectorTypes()
        {
            var riskLevels = _enumFacade.GetCIF_Org_SectorTypes();
            return Json(riskLevels, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSectorCodesForAutoFill(string prefix, SectorCodeType sectorCode)
        {
            var result = _cifFacade.GetSectorCodesForAutoFill(prefix, sectorCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UploadPicture(CIF_PersonalDto dto)
        {
            var userId = SessionHelper.UserProfile.UserId;
            //var _employee = new Finix.IPDC.Facade.EmployeeFacade();
            var result = _cifFacade.UploadPicture(dto, userId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }

    public class AutofillDto
    {
        public string prefix { get; set; }
        public List<long> exclusionList { get; set; }
    }
}