using Finix.IPDC.DTO;
using Finix.IPDC.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Finix.UI.Areas.IPDC.Controllers
{
    public class OfficeDesignationSettingController : BaseController
    {
        private readonly OfficeDesignationSettingFacade _setting = new OfficeDesignationSettingFacade();
        private readonly EmployeeFacade _employee = new EmployeeFacade();
        // GET: IPDC/OfficeDesignationSetting
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetAllDesignationSettings()
        {
            var designations = _setting.GetAllActiveSettings();
            return Json(designations, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOffDegSettingsForAssignment()
        {
            var designations = _setting.GetOffDegSettingsForAssignment();
            return Json(designations, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOffDegSettingsForBMAssignment()
        {
            var userId = SessionHelper.UserProfile.UserId;
            var data = _setting.GetAllEmpByBM(userId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveDesignationSetting(OfficeDesignationSettingDto dto)
        {
            var result = _setting.SaveOfficeDesignationSetting(dto, SessionHelper.UserProfile.UserId);
            return Json("", JsonRequestBehavior.DenyGet);
        }

        [HttpGet]
        public string GetDesignationSettingList()
        {
            //todo- load all the possible parent designations - needs logical filter
            List<OfficeDesignationSettingDto> designations = _setting.GetAllActiveSettings();
            string strret = "<select><option></option>";
            foreach (var item in designations)
            {
                strret += "<option value='" + item.Id + "'>" + item.Name + "</option>";
            }
            strret += "</select>";
            return strret;
        }

        [HttpGet]
        public JsonResult GetDesignationSettingListByOffice(long OfficeId)
        {
            var result = _setting.GetSettingByOffice(OfficeId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveEmployeeDesignation(EmployeeDesignationMappingDto dto)
        {
            var result = _setting.SaveEmployeeDesignation(dto, SessionHelper.UserProfile.UserId);
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetDesignatedEmployees(long SettingId)
        {
            var result = _setting.GetEmployeesOfCurrrentDesignation(SettingId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteOfficeDesignationSetting(int id)
        {
            ResponseDto aResponse = new ResponseDto();
            try
            {
                var result = _setting.DeleteOfficeDesignationSetting(id);
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                aResponse.Message = "Following Problem Occured "+ex;
                return Json(aResponse, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        public JsonResult RemoveEmployeeDesignation(EmployeeDesignationMappingDto dto)
        {
            var result = _setting.RemoveDesignatedEmployee(dto, SessionHelper.UserProfile.UserId);
            return Json(result);
        }
    }
}