using Finix.Auth.Facade;
using Finix.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Finix.UI.Controllers
{
    public class HomeController : Controller
    {
        //readonly BasicDataFacade basicDataFacade = new BasicDataFacade();
        //readonly MenuFacade menuFacade = new MenuFacade();

        //public ActionResult AuthError()
        //{
        //    ViewBag.Message = "An authorization error occured.";
        //    return View();
        //}

        //public ActionResult Error()
        //{
        //    ViewBag.Message = "An unexpected error occured.";
        //    return View();
        //}

        public ActionResult Index()
        {

            return RedirectToAction("Login", "Login", new { area = "Auth" });
            //ViewBag.Title = "Finix- Solutions";
            //return View();
        }
        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}

        //public ActionResult Menu(int smId)
        //{
        //    ViewBag.smId = smId;
        //    return View();
        //}

        //public JsonResult GetMenus(int smId = 0, string _search = "false", string nd = "1462793528262", int rows = 10000, int page = 1, int sidx = 1, string sord = "asc")
        //{

        //    var menus = menuFacade.GetMenus(smId);
        //    var data = menus.Select(m => new List<string> { m.Id.ToString(), m.DisplayName, m.Url }).ToList();
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        //public ContentResult GetMenus_old(int smId = 0, string _search = "false", string nd = "1462793528262", int rows = 10000, int page = 1, int sidx = 1, string sord = "asc")
        //{

        //    var menus = menuFacade.GetMenus(smId);
        //    var xmlString = menus.Count > 0
        //        ? MenuWebModel.GetMenuXmlStringForTreeGrid(menus)
        //        : GetSampleMenuTreeData();

        //    return Content(xmlString, "text/xml");
        //}

        //public JsonResult GetModuleSubModules()
        //{
        //    var moduleSubModuleList = new MenuWebModel().GetModuleAndSubModules();
        //    var data = new
        //    {
        //        moduleSubModuleList
        //    };
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //private string GetSampleMenuTreeData()
        //{
        //    return System.IO.File.ReadAllText(HttpContext.Server.MapPath("~/App_Data/tree.xml"));
        //}
    }
}