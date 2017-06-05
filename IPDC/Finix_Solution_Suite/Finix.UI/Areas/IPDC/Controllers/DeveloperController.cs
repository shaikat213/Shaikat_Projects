using Finix.IPDC.DTO;
using Finix.IPDC.Facade;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Finix.UI.Areas.IPDC.Controllers
{
    public class DeveloperController : Controller
    {
        private readonly DeveloperFacade _developer = new DeveloperFacade();
        private readonly EnumFacade _enumFacade = new EnumFacade();
        // GET: IPDC/Developer
        public ActionResult DeveloperEntry()
        {
            return View();
        }
        public JsonResult SaveDeveloper(DeveloperDto dto)
        {
            var result = _developer.SaveDeveloper(dto, SessionHelper.UserProfile.UserId);
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        public JsonResult LoadDeveloper(long Id)
        {
            var result = _developer.GetDeveloperById(Id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeveloperList(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _developer.DeveloperList(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }
        public ActionResult ProjectList(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _developer.ProjectList(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }
        public JsonResult LoadProject(long Id)
        {
            var result = _developer.LoadProjectById(Id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ProjectEntry()
        {
            return View();
        }
        public ActionResult DeveloperApproval()
        {
            return View();
        }
        public JsonResult SaveProject(ProjectDto dto)
        {

            if (!string.IsNullOrEmpty(dto.HandoverDateText))
            {
                DateTime callTime = DateTime.Now;
                var FromConverted = DateTime.TryParseExact(dto.HandoverDateText, "dd/MM/yyyy HH:mm",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out callTime);
                if (FromConverted)
                {
                    dto.HandoverDate = callTime;
                }
            }
            if (!string.IsNullOrEmpty(dto.AsOfDateText))
            {
                DateTime dob = DateTime.Now;
                var FromConverted = DateTime.TryParseExact(dto.AsOfDateText, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out dob);
                if (FromConverted)
                {
                    dto.AsOfDate = dob;
                }
            }

            var result = _developer.SaveProject(dto, SessionHelper.UserProfile.UserId);
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        public JsonResult GetDeveloperCategory()
        {
            var data = _enumFacade.GetDeveloperCategory();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDeveloperEnlistmentStatuses()
        {
            var data = _enumFacade.GetDeveloperEnlistmentStatuses();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBankAccountTypes()
        {
            var data = _enumFacade.GetBankAccountTypes();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDocumentStatusList()
        {
            var data = _enumFacade.GetDocumentStatusList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDeveloperType()
        {
            var data = _enumFacade.GetDeveloperType();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllDevelopers()
        {
            var data = _developer.GetAllDevelopers();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDeveloperProjectStatus()
        {
            var data = _enumFacade.GetDeveloperProjectStatus();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}