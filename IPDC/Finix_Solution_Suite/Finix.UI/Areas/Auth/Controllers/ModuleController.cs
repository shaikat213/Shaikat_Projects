using Finix.Auth.DTO;
using Finix.Auth.Facade;
using Finix.Auth.Util;
using Finix.UI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Finix.UI.Areas.Auth.Controllers
{
    public class ModuleController : BaseController
    {
        private readonly MenuFacade _menu;
        public ModuleController(MenuFacade menuFacade)
        {
            this._menu = menuFacade;
        }

        // GET: /Module/
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetModules()
        {
            var ModuleList = _menu.GetModules(null);
            return Json(ModuleList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetModulesJqDdl()
        {
            var moduleList = _menu.GetModules(null);
            var retVal = UiUtil.GetOptionsForJqGridDdl(moduleList, "Id", "DisplayName");
            return Json(retVal, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveModule(ModuleDto moduleDto)
        {
            try
            {
                _menu.SaveModule(moduleDto);
                return Json(new { Result = "OK", Record = "", Message = "Module saved successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeleteModule(int id)
        {
            try
            {
                _menu.DeleteModule(id);
                return Json(new { Result = "OK", Message = "Module deleted successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { Result = "ERROR", Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }
	}
}