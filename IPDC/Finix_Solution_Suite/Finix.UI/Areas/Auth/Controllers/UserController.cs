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
    public class UserController : BaseController
    {
        private UserFacade _user;
        public ActionResult UserEntry()
        {
            ViewBag.CompanyId = SessionHelper.UserProfile.SelectedCompanyId;
            return View();
        }
        public UserController(UserFacade userFacade)
        {
            this._user = userFacade;
        }
        //
        // GET: /Auth/User/
        [HttpPost]
        public JsonResult UserRegistration(UserDto userDto)
        {            
            var success = _user.UserRegistration(userDto);
            return Json(success, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUsersByCompanyId(long CompanyProfileId)
        {
            var users = _user.GetUsersByCompanyId(CompanyProfileId);
            var data = users.Select(u => new { Id = u.Id, Name = u.UserName });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveUserInfo(UserDto userinfo, int id)
        {
            string Message = "";
            try
            {
                _user.SaveUserInformation(userinfo, id);
                Message = "User info saved successfully.";

            }
            catch (Exception ex)
            {
                Message = ex.Message;

            }
            return Json(Message);

        }

        [HttpGet]
        public JsonResult GetUsersByEmployeeId(long empId)
        {
            var users = _user.GetUsersByCompanyId(empId);
            var data = users.Select(u => new { Id = u.Id, Name = u.UserName });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}