using Finix.IPDC.Infrastructure;
using Finix.Auth.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Finix.IPDC.Facade;
using Finix.IPDC.DTO;

namespace Finix.UI.Areas.IPDC.Controllers
{
    public class OrganizationController : BaseController
    {
        private readonly OrganizationFacade _organizationFacade = new OrganizationFacade();
        public ActionResult Index()
        {
            return View();
        }
  
        [HttpGet]
        public string GetOrganizationTypes()
        {
            List<KeyValuePair<int, string>> organizationTypes = UiUtil.EnumToKeyVal<OrganizationType>();
            string strret = "<select>";
            foreach (KeyValuePair<int, string> item in organizationTypes)
            {
                strret += "<option value='" + item.Key + "'>" + item.Value + "</option>";
            }
            strret += "</select>";
            return strret;
        }
        [HttpGet]
        public string GetPriorities()
        {
            List<KeyValuePair<int, string>> priorities = UiUtil.EnumToKeyVal<Priority>();
            string strret = "<select>";
            foreach (KeyValuePair<int, string> item in priorities)
            {
                strret += "<option value='" + item.Key + "'>" + item.Value + "</option>";
            }
            strret += "</select>";
            return strret;
        }
        //GetOrganizations

        [HttpGet]
        public JsonResult GetOrganizations()
        {
            var organizationList = _organizationFacade.GetOrganizations();
            return Json(organizationList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveOrganizations(OrganizationDto designdto)
        {
            try
            {
                var result = _organizationFacade.SaveOrganizations(designdto);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }

        }

    }
}