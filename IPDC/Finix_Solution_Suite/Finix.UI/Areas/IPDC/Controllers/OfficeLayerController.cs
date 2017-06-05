using Finix.IPDC.DTO;
using Finix.IPDC.Facade;
using System;
using System.Web.Mvc;

namespace Finix.UI.Areas.IPDC.Controllers
{
    public class OfficeLayerController : BaseController
    {
        //
        // GET: /OfficeLayer/
        private readonly OfficeFacade officefacade;
        public OfficeLayerController()
        {
            this.officefacade = new OfficeFacade();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetOfficeLayers()
        {
            var officeLayers = officefacade.GetOfficeLayers();
            return Json(officeLayers, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveOfficeLayer(OfficeLayerDto officelayerdto)
        {
            try
            {
                officefacade.SaveOfficeLayer(officelayerdto, SessionHelper.UserProfile.UserId);
                return Json(new { Result = "OK", Message = "Office layer saved successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeleteOfficeLayer(long id)
        {
            try
            {
                officefacade.DeleteOfficeLayer(id);
                return Json(new { Result = "OK", Message = "Office layer deleted successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { Result = "ERROR", Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpGet]
        public string GetAllOfficeLayers()
        {
            //return Json(officefacade.GetOffice(), JsonRequestBehavior.AllowGet);
            var officeList = officefacade.GetOfficeLayers();
            string strret = "<select>";
            foreach (OfficeLayerDto item in officeList)
            {
                strret += "<option value='" + item.Id + "'>" + item.Name + "</option>";
            }
            strret += "</select>";
            return strret;
        }

        [HttpGet]
        public JsonResult GetAllOfficeLayersJsonResult()
        {
            var data = officefacade.GetAllOfficeLayersJsonResult();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}