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
    public class TaskController : BaseController
    {
        private readonly MenuFacade _menu;
        //
        // GET: /Task/
        public TaskController(MenuFacade menuFacade)
        {
            this._menu = menuFacade;
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetTasks()
        {
            var tasks = _menu.GetTasks().ToList();
            return Json(tasks, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveTask(TaskDto taskdto)
        {
            try
            {
                _menu.SaveTask(taskdto);
                return Json(new { Result = "OK", Message = "Task saved successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { Result = "ERROR", Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeleteTask(int id)
        {
            try
            {
                _menu.DeleteTask(id);
                return Json(new { Result = "OK", Message = "Task deleted successfully" });
            }
            catch (Exception ex)
            {

                return Json(new { Result = "ERROR", Message = ex.Message });
            }

        }
	}
}