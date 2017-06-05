using Finix.IPDC.DTO;
using Finix.IPDC.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Finix.UI.Areas.IPDC.Controllers
{
    public class OfficeController : BaseController
    {
        //
        // GET: /Office/
        private readonly OfficeFacade officefacade;

        public OfficeController()
        {
            officefacade = new OfficeFacade();

        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetOffice()
        {
            ResolveJqFilterData(officefacade);
            return Json(officefacade.GetOffice(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveOffice(OfficeDto officedto)
        {

            try
            {
                officefacade.SaveOffice(officedto);
                return Json(new { Result = "OK", Message = "Office saved successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { Result = "ERROR", Message = ex.Message }, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public JsonResult DeleteOffice(int id)
        {
            try
            {
                officefacade.DeleteOffice(id);
                return Json(new { Result = "OK", Message = "Office deleted successfully" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                return Json(new { Result = "ERROR", Message = ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpGet]
        public string GetOfficeLayerLevels()
        {
            var officelevels = officefacade.GetOfficeLayers();
            string strret = "<select>";
            foreach (OfficeLayerDto item in officelevels)
            {
                strret += "<option value='" + item.Id + "'>" + item.Name + "</option>";
            }
            strret += "</select>";
            return strret;
        }

        [HttpGet]
        public JsonResult GetParentOffices(long officelayerid)
        {
            return Json(officefacade.GetCorrespondingParentOffices(officelayerid), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetOfficeByLayers(long officelevel)
        {
            return Json(officefacade.GetOfficesByLayer(officelevel), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetOfficeByLayer(long officelayerid)
        {
            return Json(officefacade.GetOfficesByLayerId(officelayerid), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public string GetAllOffice()
        {
            //return Json(officefacade.GetOffice(), JsonRequestBehavior.AllowGet);
            var officeList = officefacade.GetOffice();
            string strret = "<select>";
            foreach (OfficeDto item in officeList)
            {
                strret += "<option value='" + item.Id + "'>" + item.Name + "</option>";
            }
            strret += "</select>";
            return strret;
        }

        [HttpGet]
        public JsonResult GetAllOffices(int officeLayerId)
        {
            return Json(officefacade.GetAllOffices(officeLayerId), JsonRequestBehavior.AllowGet);
        }

        /*
         * Developed By Sabiha
         */
        [HttpGet]
        public JsonResult GetAllActiveOffices()
        {
            return Json(officefacade.GetAllActiveOffices(), JsonRequestBehavior.AllowGet);
        }
        /**/


    }
}