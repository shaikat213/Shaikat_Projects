using Finix.IPDC.DTO;
using Finix.IPDC.Facade;
using Finix.IPDC.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Finix.UI.Areas.IPDC.Controllers
{
    public class DesignationController : BaseController
    {
        private readonly DesignationFacade _designation = new DesignationFacade();
        // GET: IPDC/Designation
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetDesignations()
        {
            var organizationList = _designation.GetDesignations();
            return Json(organizationList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveDesignations(DesignationDto dto)
        {
            try
            {
                var result = _designation.SaveDesignations(dto);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public string GetParentDesignationList()
        {
            //todo- load all the possible parent designations - needs logical filter
            List<DesignationDto> designations = _designation.GetDesignations();
            string strret = "<select><option> </option>";
            foreach (var item in designations)
            {
                strret += "<option value='" + item.Id + "'>" + item.Name + "</option>";
            }
            strret += "</select>";
            return strret;
        }

        [HttpGet]
        public string GetDesignationList()
        {
            //todo- load all the possible parent designations - needs logical filter
            List<DesignationDto> designations = _designation.GetDesignations();
            string strret = "<select>";
            foreach (var item in designations)
            {
                strret += "<option value='" + item.Id + "'>" + item.Name + "</option>";
            }
            strret += "</select>";
            return strret;
        }

        public ActionResult EmployeeDesignationMapping()
        {
            return View();
        }

        #region Designation Role Mapping

        public ActionResult DesignationRoleMapping()
        {
            return View();
        }

        //GetAllDesignations SaveDesignationRoleMapping  SaveDesignationProductMapping
        [HttpGet]
        public JsonResult GetAllRoles()
        {
            var roleList = _designation.GetAllRoles();
            return Json(roleList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveDesignationRoleMapping(DesignationRoleMappingDto dto)
        {
            try
            {
                var result = _designation.SaveDesignationRoleMapping(dto);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetDesignationRoleMapping(long id)
        {
            var roleList = _designation.GetDesignationRoleMapping(id);
            return Json(roleList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveDesignationRoleMap(long id)
        {
            var roleMap = _designation.RemoveDesignationRoleMap(id);
            return Json(roleMap, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Designation Setting Product Mapping

        public ActionResult DesignationProductMapping()
        {
            return View();
        }

        public JsonResult SaveDesignationProductMapping(DesignationProductMappingDto dto)
        {
            try
            {
                var result = _designation.SaveDesignationProductMapping(dto);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }


        }
        public JsonResult GetDesignationProductMapping(long id)
        {
            var roleList = _designation.GetDesignationProductMapping(id);
            return Json(roleList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveDesignationProductMap(long id)
        {
            var roleMap = _designation.RemoveDesignationProductMap(id);
            return Json(roleMap, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}