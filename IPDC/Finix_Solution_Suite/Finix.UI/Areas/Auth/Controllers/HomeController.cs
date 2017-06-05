using Finix.Auth.Facade;
using Finix.IPDC.DTO;
using Finix.IPDC.Facade;
using Finix.IPDC.Infrastructure;
using Finix.UI.Areas.Auth.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Finix.Auth.Service;
using Finix.IPDC.Util;

namespace Finix.UI.Areas.Auth.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserFacade _user = new UserFacade();
        //readonly BasicDataFacade basicDataFacade = new BasicDataFacade();
        readonly MenuFacade menuFacade = new MenuFacade();
        public ActionResult Index()
        {
            ViewBag.Title = "Finix- Solutions";
            return View();
        }

        public ActionResult Menu(int smId)
        {
            ViewBag.smId = smId;
            return View();
        }

        public ActionResult AuthError()
        {
            ViewBag.Message = "An authorization error occured.";
            return View();
        }

        public ActionResult Error()
        {
            ViewBag.Message = "An unexpected error occured.";
            return View();
        }


        public JsonResult GetMenus(int smId = 0, string _search = "false", string nd = "1462793528262", int rows = 10000, int page = 1, int sidx = 1, string sord = "asc")
        {
            var menus = SessionHelper.UserProfile.Menus.Where(m => m.SubModuleId == smId);//menuFacade.GetMenus(smId);
            var data = menus.OrderBy(m => m.Sl).Select(m => new List<string> { m.Id.ToString(), m.DisplayName, m.Url }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //public ContentResult GetMenus_old(int smId = 0, string _search = "false", string nd = "1462793528262", int rows = 10000, int page = 1, int sidx = 1, string sord = "asc")
        //{

        //    var menus = menuFacade.GetMenus(smId);
        //    var xmlString = menus.Count > 0
        //        ? MenuWebModel.GetMenuXmlStringForTreeGrid(menus)
        //        : GetSampleMenuTreeData();

        //    return Content(xmlString, "text/xml");
        //}

        public JsonResult GetModuleSubModules()
        {
            var _employee = new EmployeeFacade();
            var moduleSubModuleList = new MenuWebModel().GetModuleAndSubModules();
            var UserName = SessionHelper.UserProfile.UserName;
            var UserId = SessionHelper.UserProfile.UserId;
            var employeeId = _user.GetEmployeeIdByUserId(UserId);
            var employee = _employee.GetEmployeeByEmployeeId(employeeId);
            var Profilepicture = Path.GetFileName(employee.Photo);
            var data = new
            {
                UserName,
                moduleSubModuleList,
                Profilepicture
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //private string GetSampleMenuTreeData()
        //{
        //    return System.IO.File.ReadAllText(HttpContext.Server.MapPath("~/App_Data/tree.xml"));
        //}
        public JsonResult GetNotifications(bool unreadOnly = true)
        {
            var facade = new NotificationFacade();
            if(SessionHelper.UserProfile != null)
            {
                var notifications = facade.GetNotifications(SessionHelper.UserProfile.UserId, unreadOnly: unreadOnly);

                //start of populate submodule id
                var menus = N.Menus;
                foreach (var n in notifications)
                {
                    if(menus.Any(x => x.Id == n.MenuId))
                        n.SubModuleId = menus.First(x => x.Id == n.MenuId).SubModuleId;
                }
                //end of populate submodue id

                var data = notifications.GroupBy(x => x.NotificationType,
                (k, g) => new
                {
                    NotificationType = UiUtil.GetDisplayName(k),
                    Notifications = g
                }).ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult MarkAsRead(int notificationId)
        {
            try
            {
                new NotificationFacade().MarkAsRead(new List<long> { notificationId });
            }
            catch { }
            return null;
        }
    }
}