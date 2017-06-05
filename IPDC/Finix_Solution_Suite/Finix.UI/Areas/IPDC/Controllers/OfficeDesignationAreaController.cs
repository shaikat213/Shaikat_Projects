using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Finix.IPDC.DTO;
using Finix.IPDC.Facade;

namespace Finix.UI.Areas.IPDC.Controllers
{
    public class OfficeDesignationAreaController : BaseController
    {
        private readonly  OfficeDesignationAreaFacade _officeDesignationAreaFacade = new OfficeDesignationAreaFacade();
        private readonly  AddressFacade _addressFacade = new AddressFacade();

        public ActionResult Index()
        {
            return View();
        }
        
        public JsonResult GetAllDivisions()
        {
            var divisionList = _addressFacade.GetAllDivisions();
            return Json(divisionList, JsonRequestBehavior.AllowGet);
            //return Json(, JsonRequestBehavior.AllowGet); 
        }
        public JsonResult GetDistrictByDivision(long id)
        {
            var divisionList = _addressFacade.GetDistrictByDivision(id);
            return Json(divisionList, JsonRequestBehavior.AllowGet);
            //return Json(, JsonRequestBehavior.AllowGet); GetDistrictByDivision
        }
        public JsonResult GetUpzilaByDistrict(long id)
        {
            var divisionList = _addressFacade.GetUpzilaByDistrict(id);
            return Json(divisionList, JsonRequestBehavior.AllowGet);
            //return Json(, JsonRequestBehavior.AllowGet); GetDistrictByDivision 
        }
        public JsonResult GetByOfficeDesignationMap(long upazilaOrThana, long dist, long settingId)
        {
            var divisionList = _officeDesignationAreaFacade.GetByOfficeDesignationMap(upazilaOrThana, dist, settingId);
            return Json(divisionList, JsonRequestBehavior.AllowGet);
            //return Json(, JsonRequestBehavior.AllowGet); GetDistrictByDivision getOfficeDesgArea
        }
        public JsonResult GetOfficeDesgArea(long id)
        {
            var divisionList = _officeDesignationAreaFacade.GetOfficeDesgArea(id);
            return Json(divisionList, JsonRequestBehavior.AllowGet);
            //return Json(, JsonRequestBehavior.AllowGet); GetDistrictByDivision 
        }
        //SaveOfficeDesignationArea

        public JsonResult SaveOfficeDesignationArea(OfficeDesignationAreaDto officeDesignationArea)
        {
            try
            {
                //if (salesLeadDto.FollowUpCallTimeText != null)
                //{
                //    DateTime nextFollowUp = DateTime.Now;
                //    var FromConverted = DateTime.TryParseExact(salesLeadDto.FollowUpCallTimeText, "dd/MM/yyyy HH:mm",
                //        CultureInfo.InvariantCulture, DateTimeStyles.None, out nextFollowUp);
                //    if (FromConverted)
                //    {
                //        salesLeadDto.FollowUpCallTime = nextFollowUp;
                //    }
                //}
                var result = _officeDesignationAreaFacade.SaveOfficeDesignationArea(officeDesignationArea);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult UpdateOfficeDesignationArea(long id)
        {
            try
            {
             
                var result = _officeDesignationAreaFacade.UpdateOfficeDesignationArea(id);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult GetDivisionByCountry(long id)
        {
            var divisionList = _officeDesignationAreaFacade.GetDivisionByCountry(id);
            return Json(divisionList, JsonRequestBehavior.AllowGet);
            //return Json(, JsonRequestBehavior.AllowGet); GetDistrictByDivision 
        }

    }
}