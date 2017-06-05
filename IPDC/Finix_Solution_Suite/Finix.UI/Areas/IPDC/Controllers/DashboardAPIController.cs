using Finix.IPDC.Facade;
using Finix.IPDC.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Finix.UI.Areas.IPDC.Controllers
{
    public class DashboardAPIController : Controller
    {
        private readonly DashboardFacade _dashboard = new DashboardFacade();
        // GET: IPDC/DashboardAPI
        public ActionResult DashboardRM(string ApiKey, long userId)
        {
            if(!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                var data = _dashboard.GetRMDashboardData(userId);
                return View(data);
            }
            return View();
        }

        public ActionResult DashboardBM(string ApiKey, long userId)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                ViewBag.UserId = userId;
                ViewBag.ApiKey = ApiKey;
            }
            return View();
        }

        public ActionResult DashboardTL(string ApiKey, long userId)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                ViewBag.UserId = userId;
                ViewBag.ApiKey = ApiKey;
            }
            return View();
        }

        public ActionResult DashboardMD(string ApiKey, long userId)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                ViewBag.UserId = userId;
                ViewBag.ApiKey = ApiKey;
            }
            return View();
        }

        public ActionResult Leaderboard(string ApiKey, long userId)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                ViewBag.UserId = userId;
                ViewBag.ApiKey = ApiKey;
            }
            return View();
        }

        public ActionResult DashboardNSM(string ApiKey, long userId)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                ViewBag.UserId = userId;
                ViewBag.ApiKey = ApiKey;
            }
            return View();
        }

        public ActionResult DemographicAnalysis(string ApiKey, long userId)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                ViewBag.UserId = userId;
                ViewBag.ApiKey = ApiKey;
            }
            return View();
        }
        public ActionResult ProductivityMatrix(string ApiKey, long userId)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                ViewBag.UserId = userId;
                ViewBag.ApiKey = ApiKey;
            }
            return View();
        }
        #region data functions
        [HttpGet]
        public JsonResult GetBMDashboardHighlights(string ApiKey, long userId, TimeLine? timeLine)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                var data = _dashboard.GetBMDashboardHighlights(userId, timeLine);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetActivitySummaryOfBm(string ApiKey, long userId, long? productId, TimeLine? timeLine)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                var data = _dashboard.GetActivitySummaryOfBm(userId, productId, timeLine);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetAllProducts(string ApiKey)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                ProductFacade _productFacade = new ProductFacade();
                var products = _productFacade.GetAllProducts();
                return Json(products, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
            
        }
        [HttpGet]
        public JsonResult GetTLDashboardHighlights(string ApiKey, long userId, TimeLine? timeLine)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                var data = _dashboard.GetBMDashboardHighlights(userId, timeLine);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
            
        }
        [HttpGet]
        public JsonResult GetActivitySummaryOfTl(string ApiKey, long userId, long? productId, TimeLine? timeLine)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                var data = _dashboard.GetActivitySummaryOfBm(userId, productId, timeLine);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
            
        }
        [HttpGet]
        public JsonResult GetOfficeByLayer(string ApiKey, long officelayerid)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                OfficeFacade officefacade = new OfficeFacade();
                return Json(officefacade.GetOfficesByLayerId(officelayerid), JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetMDDashboardHightlidhts(string ApiKey, TimeLine? timeLine, Criteria? criteria, List<long?> branchIds)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                var data = _dashboard.GetMDDashboardCallHightlidhts(timeLine, criteria, branchIds);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
            
        }
        [HttpPost]
        public JsonResult GetMDDashboardHighlightsRight(string ApiKey, TimeLine? timeLine, List<long?> branchId)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                var data = _dashboard.GetMDDashboardHighlightsRight(timeLine, branchId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
            
        }
        [HttpGet]
        public JsonResult GetMDDashboardHighlightsRightBranch(string ApiKey, TimeLine? timeLine)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                var data = _dashboard.GetMDDashboardHighlightsRightBranch(timeLine);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
            
        }
        [HttpGet]
        public JsonResult GetCostCenters(string ApiKey)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                ApplicationFacade _application = new ApplicationFacade();
                var result = _application.GetAllCostCenters();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetLeaderboardInfo(string ApiKey, TimeLine? timeLine, long? stageId, long? criteriaId, long? centerId, long? productId, long? branchId)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                var data = _dashboard.GetLeaderboardInfo(timeLine, stageId, criteriaId, centerId, productId, branchId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
            
        }
        [HttpGet]
        public JsonResult GetAgingForNSM(string ApiKey, long? productId)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                var data = _dashboard.GetAgingForNSM(productId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
            
        }

        [HttpGet]
        public JsonResult GetApprovalRatioNSM(string ApiKey, long? productId)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                var data = _dashboard.GetApprovalRatioNSM(productId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
            
        }

        [HttpGet]
        public JsonResult GetFileApprovedForNSM(string ApiKey, long? productId)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                var data = _dashboard.GetFileApprovedForNSM(productId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
            
        }
        [HttpGet]
        public JsonResult GetFileSubmissionForNSM(string ApiKey, long? productId)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                var data = _dashboard.GetFileSubmissionForNSM(productId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetProductTypes(string ApiKey)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                var productTypes = new EnumFacade().GetProductTypes();
                return Json(productTypes, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
            
        }
        [HttpGet]
        public JsonResult GetProductByType(string ApiKey, ProductType typeId)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                var products = new ProductFacade().GetProductByType(typeId);
                return Json(products, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetNSMTLDashboardHighlights(string ApiKey, TimeLine? timeLine, List<long?> branchId)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                var data = _dashboard.GetNSMBMDashboardHighlights(timeLine, branchId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetPMDashboard(string ApiKey, string fromdate, string todate, Stages? stage, Criteria? criteria, long? costCenterId, ProductSelection? product, long? branchId)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {

                DateTime fromDate;
                DateTime convertedFromDate = new DateTime();
                DateTime toDate;
                DateTime convertedToDate = new DateTime();
                var fromDateConverted = DateTime.TryParseExact(fromdate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate);
                if (fromDateConverted)
                {
                    convertedFromDate = fromDate;
                }

                var toDateConverted = DateTime.TryParseExact(todate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate);
                if (toDateConverted)
                {
                    convertedToDate = toDate;
                }

                var data = _dashboard.GetPMDashboard(convertedFromDate, convertedToDate, stage, criteria, costCenterId, product, branchId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult GetNSMDisiburesedReceivedMTD(string ApiKey, TimeLine? timeLine, List<long?> branchId)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                var data = _dashboard.GetNSMDisiburesedReceivedMTD(timeLine, branchId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetNSMDisiburesedReceivedLMTD(string ApiKey, TimeLine? timeLine, List<long?> branchId)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                var data = _dashboard.GetNSMDisiburesedReceivedLMTD(timeLine, branchId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetAllDivisions(string ApiKey)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                var divisionList = new AddressFacade().GetAllDivisions();
                return Json(divisionList, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetDistrictsByDivision(string ApiKey, long divisionId)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                var districts = new AddressFacade().GetAllDistricts().Where(r => r.DivisionId == divisionId).ToList();
                return Json(districts, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetThanasByDistrict(string ApiKey, long districtId)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                var thanas = new AddressFacade().GetAllThanas().Where(r => r.DistrictId == districtId).ToList();
                return Json(thanas, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetDemographicResidence(string ApiKey, string fromdate, string todate, long? divisionId, long? districtId, long? thanaId, Stages? stage, Criteria? criteria, List<ProductSelection?> products, List<long?> branchIds)
        {
            if (!string.IsNullOrEmpty(ApiKey) && ApiKey == BizConstants.ApiKey)
            {
                DateTime fromDate = DateTime.Now.AddDays(-1);
                DateTime toDate = DateTime.Now;

                var fromDateConverted = DateTime.TryParseExact(fromdate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate);

                var toDateConverted = DateTime.TryParseExact(todate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate);

                var data = _dashboard.GetDemoGraphicResidenceDashboard(fromDate, toDate, divisionId, districtId, thanaId, stage, criteria, products, branchIds);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
            
        }
        #endregion
    }
}