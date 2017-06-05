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
    public class MenuController : BaseController
    {
        private readonly MenuFacade _menu;

        public MenuController(MenuFacade menuFacade)
        {

            this._menu = menuFacade;
        }

        public ActionResult Index()
        {

            return View();
        }

        [HttpGet]
        public JsonResult GetMenus()
        {
            var menulist = _menu.GetMenus().ToList();
            return Json(menulist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveMenu(MenuDto menudto)
        {
            try
            {
                _menu.SaveMenu(menudto);
                return Json(new { Result = "OK", Message = "Menu saved succesfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetCorrespondingSubModules(int? moduleId)
        {
            List<SubModuleDto> submodules = _menu.GetSubModules(moduleId ?? 0).ToList();
            // return "<select><option value='1'>Sideboom</option><option value='2'>Truck</option><option value='3'>Car</option></select>";
            //string strret = "<select>";
            //foreach (SubModuleDto item in submodules)
            //{
            //   strret += "<option value='" + item.Id + "'>" + item.Name + "</option>";
            // }
            // strret += "</select>";
            return Json(submodules, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMenu(int id)
        {
            try
            {
                _menu.DeleteMenu(id);
                return Json(new { Result = "OK", Message = "Menu deleted successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { Result = "ERROR", Message = ex.Message });
            }

        }
	}
}