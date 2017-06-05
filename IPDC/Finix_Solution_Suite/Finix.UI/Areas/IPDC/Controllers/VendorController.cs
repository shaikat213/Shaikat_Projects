using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Finix.IPDC.DTO;
using Finix.IPDC.Facade;

namespace Finix.UI.Areas.IPDC.Controllers
{
    public class VendorController : BaseController
    {
        private readonly VendorFacade _vendorFacade = new VendorFacade();
        private readonly EnumFacade _enumFacade = new EnumFacade();

        public ActionResult VendorEntry()
        {
            return View();
        }
        public JsonResult SaveVendor(VendorDto dto)
        {
            var result = _vendorFacade.SaveVendor(dto, SessionHelper.UserProfile.UserId);
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        public JsonResult LoadVendor(long Id)
        {
            var result = _vendorFacade.GetVendorById(Id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VendorList(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _vendorFacade.VendorList(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }
        public JsonResult GetVendorProductType()
        {
            var vendorProductType = _enumFacade.GetVendorProductType();
            return Json(vendorProductType, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllVendors()
        {
            var vendors = _vendorFacade.GetAllVendors();
            return Json(vendors, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllVendorShowrooms(long vendorId)
        {
            var vendors = _vendorFacade.GetAllVendorShowrooms(vendorId);
            return Json(vendors, JsonRequestBehavior.AllowGet);
        }
        //GetOnlyShowRooms
        public JsonResult GetOnlyShowRooms()
        {
            var showroom = _vendorFacade.GetOnlyShowRooms();
            return Json(showroom, JsonRequestBehavior.AllowGet);
        }
    }
}