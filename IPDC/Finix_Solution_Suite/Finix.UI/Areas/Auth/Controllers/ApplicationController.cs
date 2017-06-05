using Finix.Auth.DTO;
using Finix.Auth.Facade;
using Finix.UI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Finix.UI.Areas.Auth.Controllers
{
    public class ApplicationController : BaseController
    {
        readonly ApplicationFacade _application = new ApplicationFacade();
        //
        // GET: /Auth/Application/
        public ActionResult Index()
        {
            ViewBag.CompanyId = SessionHelper.UserProfile.SelectedCompanyId;
            return View();
        }

        [HttpGet]
        public JsonResult GetAllApplications()
        {
            var response = _application.GetAllActiveApplication();
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveApplication(ApplicationDto application)
        {
            var response = _application.SaveApplication(application, SessionHelper.UserProfile.UserId);
            return Json(response);
        }
        [HttpPost]
        public JsonResult DeleteApplication(ApplicationDto application)
        {
            throw new NotImplementedException();
            //return Json("");
        }
	}
}