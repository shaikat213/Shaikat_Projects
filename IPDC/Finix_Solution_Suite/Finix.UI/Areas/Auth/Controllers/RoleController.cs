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
    public class RoleController : BaseController
    {
        MenuFacade _menu;
        public RoleController(MenuFacade menuFacade)
        {
            this._menu = menuFacade;
        }
        //
        // GET: /Auth/Role/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RolePermissions()
        {
            return View();
        }

        public ActionResult ProxyRolePermissions()
        {
            ViewBag.CompanyId = SessionHelper.UserProfile.SelectedCompanyId;
            return View();
        }
        public JsonResult GetRoles()
        {
            var rolelist = _menu.GetRoles().ToList();
            return Json(rolelist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveRole(RoleDto roledto)
        {

            try
            {
                _menu.SaveRole(roledto);
                return Json(new { Result = "OK", Message = "Role saved successfully" });
            }
            catch (Exception ex)
            {

                return Json(new { Result = "ERROR", Message = ex.Message });
            }

        }

        public JsonResult DeleteRole(int id)
        {
            try
            {
                _menu.DeleteRole(id);
                return Json(new { Result = "OK", Message = "Role deleted successfully" });
            }
            catch (Exception ex)
            {

                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetRolePermissionData()//long? RoleId
        {
            var _menu = new MenuFacade();

            var data = _menu.GetRolePermissionData();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetCurrentRolePermissionData(long RoleId)
        {
            var _menu = new MenuFacade();

            var data = _menu.GetCurrentPermissionData(RoleId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveRolePermissions(RolePermissionDto rolePermission)//List<string> ids, long roleId
        {
            var _menu = new MenuFacade();
            //var data = ids;
            var success = _menu.SaveRolePermission(rolePermission, SessionHelper.UserProfile.UserId);
            return Json(success, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult SaveProxyRolePermissions(RolePermissionDto rolePermission)
        {
            var _menu = new MenuFacade();
            //var data = ids;
            var success = _menu.SaveProxyRolePermission(rolePermission, SessionHelper.UserProfile.UserId);
            return Json(success, JsonRequestBehavior.DenyGet);
        }
	}
}