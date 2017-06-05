using Finix.IPDC.DTO;
using Finix.IPDC.Facade;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace Finix.UI.Areas.IPDC.Controllers
{
    public class AddressController : BaseController
    {
        private readonly AddressFacade _address = new AddressFacade();
        // GET: IPDC/Address
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetCountries() {
            var result = _address.GetAllCountries();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string GetThanaList()
        {
            List<ThanaDto> thanas = _address.GetAllThanas();
            string strret = "<select><option></option>";
            foreach (var item in thanas)
            {
                strret += "<option value='" + item.Id + "'>" + item.ThanaNameEng + "</option>";
            }
            strret += "</select>";
            return strret;
        }

        public string GetThanasByDistrictInGrid(long districtId)
        {
            List<ThanaDto> thanas = _address.GetAllThanas().Where(r => r.DistrictId == districtId).ToList();
            string strret = "<select><option></option>";
            foreach (var item in thanas)
            {
                strret += "<option value='" + item.Id + "'>" + item.ThanaNameEng + "</option>";
            }
            strret += "</select>";
            return strret;
        }

        public JsonResult GetThanasByDistrict(long districtId)
        {
            List<ThanaDto> thanas = _address.GetAllThanas().Where(r => r.DistrictId == districtId).ToList();
            return Json(thanas, JsonRequestBehavior.AllowGet);
        }

        public string GetDistrictList()
        {
            List<DistrictDto> districts = _address.GetAllDistricts();
            string strret = "<select><option></option>";
            foreach (var item in districts)
            {
                strret += "<option value='" + item.Id + "'>" + item.Name + "</option>";
            }
            strret += "</select>";
            return strret;
        }
        public JsonResult GetDistrictsList()
        {
            List<DistrictDto> districts = _address.GetAllDistricts();
            return Json(districts, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDistrictsByDivision(long divisionId)
        {
            List<DistrictDto> districts = _address.GetAllDistricts().Where(r => r.DivisionId == divisionId).ToList();

            return Json(districts, JsonRequestBehavior.AllowGet);
        }
        public string GetDistrictsByDivisionInGrid(long divisionId)
        {
            List<DistrictDto> districts = _address.GetAllDistricts().Where(r => r.DivisionId == divisionId).ToList();
            string strret = "<select><option></option>";
            foreach (var item in districts)
            {
                strret += "<option value='" + item.Id + "'>" + item.DistrictNameEng + "</option>";
            }
            strret += "</select>";
            return strret;
        }

        public JsonResult GetDivisionsByCountry(long countryId)
        {
            List<DivisionDto> divisions = _address.GetAllDivisions().Where(r => r.CountryId == countryId).ToList();

            return Json(divisions, JsonRequestBehavior.AllowGet);
        }

        public string GetDivisionsByCountryInGrid(long countryId)
        {
            List<DivisionDto> divisions = _address.GetAllDivisions().Where(r => r.CountryId == countryId).ToList();
            string strret = "<select><option></option>";
            foreach (var item in divisions)
            {
                strret += "<option value='" + item.Id + "'>" + item.DivisionNameEng + "</option>";
            }
            strret += "</select>";
            return strret;
        }
        public string GetCountryList()
        {
            List<CountryDto> countries = _address.GetAllCountries();
            string strret = "<select><option></option>";
            foreach (var item in countries)
            {
                strret += "<option value='" + item.Id + "'>" + item.Name + "</option>";
            }
            strret += "</select>";
            return strret;
        }

        public JsonResult GetHighestEducationLevel()
        {
            var educationlist = UiUtil.EnumToKeyVal<EducationLevel>().OrderBy(r=>r.Key);
            return Json(educationlist,JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetResidenceStatus()
        {
            var residencelist = UiUtil.EnumToKeyVal<ResidenceStatus>();
            return Json(residencelist, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetHomeOwnership()
        {
            var homeOwnershiplist = UiUtil.EnumToKeyVal<HomeOwnership>();
            return Json(homeOwnershiplist, JsonRequestBehavior.AllowGet);
        }
      
        public JsonResult GetYearsCurrentResidence()
        {
            var typeList = Enum.GetValues(typeof(YearsCurrentResidence))
               .Cast<YearsCurrentResidence>()
               .Select(t => new CurrentResidenceYearsDto
               {
                   Id = ((int)t),
                   Name = t.ToString(),
                   DisplayName = UiUtil.GetDisplayName(t)
               });
            //var typeList = UiUtil.EnumToKeyVal<HomeOwnership>();

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
    }
}