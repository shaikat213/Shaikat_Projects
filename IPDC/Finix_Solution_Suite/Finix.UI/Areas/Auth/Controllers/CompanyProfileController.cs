//using Finix.Accounts.Facade;
using Finix.Auth.DTO;
using Finix.Auth.Facade;
using Finix.UI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Finix.Auth.Infrastructure;
using Finix.Auth.Util;

namespace Finix.UI.Areas.Auth.Controllers
{
    public class CompanyProfileController : BaseController
    {
        private readonly CompanyProfileFacade _companyProfileFacade;

        public CompanyProfileController(CompanyProfileFacade companyProfileFacade)
        {
            this._companyProfileFacade = companyProfileFacade;
        }
        [HttpGet]
        public JsonResult GetAllActiveCompanies()
        {
            var companyList = _companyProfileFacade.GetAllActiveCompanyProfiles();
            return Json(companyList, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetAllCompanyList()
        {
            var companyList = _companyProfileFacade.GetAllCompanyProfiles().Select(c => new { c.Name, c.Id }).ToList();
            return Json(companyList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveCompanyProfile(CompanyProfileDto company)
        {
            var response = _companyProfileFacade.SaveCompanyProfile(company, SessionHelper.UserProfile.UserId);
            return Json(response);
        }
        
        //
        // GET: /CompanyProfile/
        public ActionResult Index()
        {
            ViewBag.CompanyId = SessionHelper.UserProfile.SelectedCompanyId;
            return View();
        }

        public ActionResult ClosingDate()
        {
            return View();
        }
        [HttpGet]
        public JsonResult UpdateClosingDate(long? CompanyProfileId)
        {

            //var _accounts = new AccountsFacade();
            var closingDate = _companyProfileFacade.DateToday(CompanyProfileId);
            //var productionCostPerKg = _production.CalculateCostPerKG(closingDate);
            //_accounts.ArchieveVouchers(closingDate);

            //var _reportAccount = new ReportAccountFacade();
            //_reportAccount.GetInventoryAmount(DateTime.Now.AddYears(-1), closingDate);

            var cp = _companyProfileFacade.UpdateClosingDate();
            return Json(cp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DateToday(long? CompanyProfileId)
        {
            var cp = _companyProfileFacade.DateToday(CompanyProfileId);
            return Json(cp, JsonRequestBehavior.AllowGet);
        }
        /*sabiha*/
        public JsonResult GetUpdateBillNo()
        {
            var cp = _companyProfileFacade.GetUpdateBillNo();
            return Json(cp, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUpdateVoucherNo()
        {
            var cp = _companyProfileFacade.GetUpdateVoucherNo();
            return Json(cp, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUpdateSpecificVoucherNo(string type)
        {
            string voucherNo = "";// = _companyProfileFacade.GetUpdateCDvNo();
            switch (type)
            {
                case "CDV":
                    voucherNo = _companyProfileFacade.GetUpdateCDvNo();
                    break;
                case "BDV":
                    voucherNo = _companyProfileFacade.GetUpdateBDvNo();
                    break;
                case "CCV":
                    voucherNo = _companyProfileFacade.GetUpdateCCvNo();
                    break;
                case "BCV":
                    voucherNo = _companyProfileFacade.GetUpdateBCvNo();
                    break;
                case "JV":
                    voucherNo = _companyProfileFacade.GetUpdateJvNo();
                    break;
                default:
                    break;
                //return Json(voucherNo, JsonRequestBehavior.AllowGet);
            }

            return Json(voucherNo, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetInvoiceNo()
        {
            var cp = _companyProfileFacade.GetUpdateInvoiceNo();
            return Json(cp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetChalanNo()
        {
            var cp = _companyProfileFacade.GetUpdatedChalanNo();
            return Json(cp, JsonRequestBehavior.AllowGet);
        }
        /*Sabiha Modified 18.10.2016*/
        public string GetAllCompanyListForGrid()
        {
            var companyList = _companyProfileFacade.GetAllCompanyProfiles().Select(c => new { c.Name, c.Id }).ToList();

            string strret = "<select>";
            foreach (var item in companyList)
            {
                strret += "<option value='" + item.Id + "'>" + item.Name + "</option>";
            }
            strret += "</select>";
            return strret;
        }
        public string GetCompanyProfile()
        {
            List<KeyValuePair<int, string>> leaveApps = UiUtil.EnumToKeyVal<CompanyType>();
            string strret = "<select>";
            foreach (KeyValuePair<int, string> item in leaveApps)
            {
                strret += "<option value='" + item.Key + "'>" + item.Value + "</option>";
            }
            strret += "</select>";
            return strret;
        }

    }
}