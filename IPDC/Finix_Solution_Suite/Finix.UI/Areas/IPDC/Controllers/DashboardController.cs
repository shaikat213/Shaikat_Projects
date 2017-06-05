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
    public class DashboardController : BaseController
    {
        private readonly DashboardFacade _dashboard = new DashboardFacade();
        // GET: IPDC/Dashboard
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TLBM()
        {
            return View();
        }

        public ActionResult DashboardRM()
        {
            var data = _dashboard.GetRMDashboardData(SessionHelper.UserProfile.UserId);
            //return Json(data, JsonRequestBehavior.AllowGet);
            return View(data);
        }
        

        public ActionResult LeaderBoard()
        {
            return View();
        }
        public ActionResult LeaderBoardAutoLoan()
        {
            return View();
        }
        public ActionResult LeaderBoardPersonalLoan()
        {
            return View();
        }
        public ActionResult LeaderBoardHomeLoan()
        {
            return View();
        }
        public ActionResult LeaderBoardLiability()
        {
            return View();
        }
        public ActionResult ProductivityMatrix()
        {
            return View();
        }

        public ActionResult MdDashboard()
        {
            return View();
        }

        public ActionResult NSMDashboard()
        {
            return View();
        }
        public ActionResult DashboardHomeLoan()
        {
            return View();
        }
        public ActionResult DashboardAutoLoan()
        {
            return View();
        }
        public ActionResult DashboardPersonalLoan()
        {
            return View();
        }
        public ActionResult DashboardLiability()
        {
            return View();
        }

        public ActionResult DemographicAnalysis()
        {
            return View();
        }
        public ActionResult DemographicAnalysisAutoLoan()
        {
            return View();
        }
        public ActionResult DemographicAnalysisHomeLoan()
        {
            return View();
        }
        public ActionResult DemographicAnalysisPersonalLoan()
        {
            return View();
        }
        public ActionResult DemographicAnalysisLiability()
        {
            return View();
        }

        public ActionResult TLDashboard()
        {
            return View();
        }

        public ActionResult BMDashboard()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetBMDashboardHighlights(TimeLine? timeLine)
        {
            var data = _dashboard.GetBMDashboardHighlights(SessionHelper.UserProfile.UserId, timeLine);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetMDDashboardHightlidhts(TimeLine? timeLine, Criteria? criteria, List<long?> branchIds)
        {
            var data = _dashboard.GetMDDashboardCallHightlidhts(timeLine, criteria, branchIds);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetNSMDisiburesedReceivedMTD(TimeLine? timeLine, long? branchId)
        {   
            var data = _dashboard.GetNSMDisiburesedReceivedMTD(timeLine, branchId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetNSMDisiburesedReceivedMTD(TimeLine? timeLine, List<long?> branchId)
        {
            var data = _dashboard.GetNSMDisiburesedReceivedMTD(timeLine, branchId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetNSMDisiburesedReceivedLMTD(TimeLine? timeLine, long? branchId)
        {
            var data = _dashboard.GetNSMDisiburesedReceivedLMTD(timeLine, branchId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetNSMDisiburesedReceivedLMTD(TimeLine? timeLine, List<long?> branchId)
        {
            var data = _dashboard.GetNSMDisiburesedReceivedLMTD(timeLine, branchId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetMDDashboardHighlightsRight(TimeLine? timeLine,  List<long?> branchId)
        {
            //SessionHelper.UserProfile.UserId,
            var data = _dashboard.GetMDDashboardHighlightsRight(timeLine, branchId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetMDDashboardHighlightsRightBranch(TimeLine? timeLine)
        {
            //SessionHelper.UserProfile.UserId,
            var data = _dashboard.GetMDDashboardHighlightsRightBranch(timeLine);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetActivitySummaryOfBm(long? productId, TimeLine? timeLine)
        {
            var data = _dashboard.GetActivitySummaryOfBm(SessionHelper.UserProfile.UserId, productId, timeLine);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetTLDashboardHighlights(TimeLine? timeLine, long? branchId)
        {
            var data = _dashboard.GetBMDashboardHighlights(SessionHelper.UserProfile.UserId, timeLine);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetNSMTLDashboardHighlights(TimeLine? timeLine, long? branchId)
        {
            var data = _dashboard.GetNSMBMDashboardHighlights(timeLine, branchId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetNSMTLDashboardHighlights(TimeLine? timeLine, List<long?> branchId)
        {
            var data = _dashboard.GetNSMBMDashboardHighlights(timeLine, branchId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetActivitySummaryOfTl(long? productId, TimeLine? timeLine)
        {
            var data = _dashboard.GetActivitySummaryOfBm(SessionHelper.UserProfile.UserId, productId, timeLine);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetLeaderboardInfo( TimeLine? timeLine, long? stageId, long? criteriaId, long? centerId, long? productId, long? branchId)
        {
            var data = _dashboard.GetLeaderboardInfo( timeLine, stageId, criteriaId, centerId, productId, branchId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPMDashboard(string fromdate, string todate, Stages? stage, Criteria? criteria, long? costCenterId, ProductSelection? product, long? branchId)
        {
            DateTime fromDate;
            DateTime convertedFromDate = new DateTime();
            DateTime toDate;
            DateTime convertedToDate = new DateTime();
            var fromDateConverted = DateTime.TryParseExact(fromdate, "MMM/yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate);
            if (fromDateConverted)
            {
                convertedFromDate = fromDate;
            }
            
            var toDateConverted = DateTime.TryParseExact(todate, "MMM/yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate);
            if (toDateConverted)
            {
                convertedToDate = toDate.AddMonths(1).AddDays(-1);
            }

            var data = _dashboard.GetPMDashboard(convertedFromDate, convertedToDate, stage, criteria, costCenterId, product, branchId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetPMDashboard(string fromdate, string todate, Stages? stage, Criteria? criteria, long? costCenterId, List<ProductSelection?> product, List<long?> branchId)
        {
            DateTime fromDate;
            DateTime convertedFromDate = new DateTime();
            DateTime toDate;
            DateTime convertedToDate = new DateTime();
            var fromDateConverted = DateTime.TryParseExact(fromdate, "MMM/yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate);
            if (fromDateConverted)
            {
                convertedFromDate = fromDate;
            }

            var toDateConverted = DateTime.TryParseExact(todate, "MMM/yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate);
            if (toDateConverted)
            {
                convertedToDate = toDate.AddMonths(1).AddDays(-1);
            }

            var data = _dashboard.GetPMDashboard(convertedFromDate, convertedToDate, stage, criteria, costCenterId, product, branchId);
            return Json(data, JsonRequestBehavior.AllowGet);
            //return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetFileSubmissionForNSM(long? productId)
        {
            var data = _dashboard.GetFileSubmissionForNSM( productId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetFileApprovedForNSM(long? productId)
        {
            var data = _dashboard.GetFileApprovedForNSM(productId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetApprovalRatioNSM(long? productId)
        {
            var data = _dashboard.GetApprovalRatioNSM(productId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAgingForNSM(long? productId)
        {
            var data = _dashboard.GetAgingForNSM(productId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDemographicResidence(string fromdate, string todate, long? divisionId, long? districtId, long? thanaId, Stages? stage, Criteria? criteria, List<ProductSelection?> products, List<long?> branchIds)
        {
            DateTime fromDate = DateTime.Now.AddDays(-1);
            DateTime toDate = DateTime.Now;
            
            var fromDateConverted = DateTime.TryParseExact(fromdate, "dd/MM/yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate);
            
            var toDateConverted = DateTime.TryParseExact(todate, "dd/MM/yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate);
            

            var data = _dashboard.GetDemoGraphicResidenceDashboard(fromDate, toDate, divisionId, districtId, thanaId, stage, criteria, products, branchIds);//
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}