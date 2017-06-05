using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Finix.IPDC.Facade;
using Finix.IPDC.DTO;
using System.Globalization;

namespace Finix.UI.Areas.IPDC.Controllers
{
    public class ProductController : BaseController
    {
        private readonly ProductFacade _productFacade = new ProductFacade();
        private readonly EnumFacade _enumFacade = new EnumFacade();
        public JsonResult GetAllProducts()
        {
            var data = _productFacade.GetAllProducts().ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProductEntry()
        {
            return View();
        }
        public JsonResult GetProductTypes()
        {
            var productTypes = _enumFacade.GetProductTypes();
            return Json(productTypes, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadProductById(long id)
        {
            var data = _productFacade.LoadProductById(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //LoadProductById
        public JsonResult SaveProduct(ProductDto dto)
        {
            DateTime creditedDate = DateTime.Now;
            try
            {
               
                if (dto.ProductRates != null)
                {
                    foreach (var item in dto.ProductRates)
                    {
                        var FromConvertedCreditDate = DateTime.TryParseExact(item.EffectiveDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out creditedDate);
                        if (FromConvertedCreditDate)
                            item.EffectiveDate = creditedDate;

                    }
                }
                if (dto.ProductSpecialRate != null)
                {
                    foreach (var item in dto.ProductSpecialRate)
                    {
                        var FromConvertedCreditDate = DateTime.TryParseExact(item.EffectiveDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out creditedDate);
                        if (FromConvertedCreditDate)
                            item.EffectiveDate = creditedDate;

                    }
                }
                if (dto.DPSMaturitySchedule != null)
                {
                    foreach (var item in dto.DPSMaturitySchedule)
                    {
                        var FromConvertedCreditDate = DateTime.TryParseExact(item.EffectiveDateTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out creditedDate);
                        if (FromConvertedCreditDate)
                            item.EffectiveDate = creditedDate;

                    }
                }
                var userId = SessionHelper.UserProfile.UserId;
                var result = _productFacade.SaveProduct(dto, userId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.DenyGet);
            }
        }

        public JsonResult GetAllDocuments()
        {
            var data = _productFacade.GetAllDocuments().ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetProducts(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _productFacade.GetProducts(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }
    }
}