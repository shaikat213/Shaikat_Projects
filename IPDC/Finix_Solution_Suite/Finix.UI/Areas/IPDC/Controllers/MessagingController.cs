using System.Web.Mvc;
using Finix.IPDC.DTO;
using Finix.IPDC.Facade;

namespace Finix.UI.Areas.IPDC.Controllers
{
    public class MessagingController : BaseController
    {
        private readonly VerificationFacade _verification = new VerificationFacade();
        private readonly MessagingFacade _messaging = new MessagingFacade();
        private readonly ProfessionFacade _professionFacade = new ProfessionFacade();
        private readonly EnumFacade _enum = new EnumFacade();
        public ActionResult NewMessage() //long? applicationId
        {
            return View();
        }
        public ActionResult ReplyForwardMessage()
        {
            return View();
        }
        public ActionResult LoadDraftMessage()
        {
            return View();
        }
        public ActionResult Inbox(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _messaging.GetInboxMessagingPagedList(10, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }
        public ActionResult Outbox(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _messaging.GetOutboxMessagingPagedList(10, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }
        public ActionResult Draft(string sortOrder, string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.CurrentSort = sortOrder;
            var model = _messaging.GetDraftMessagingPagedList(10, page, searchString, SessionHelper.UserProfile.UserId);
            return View(model);
        }
        public JsonResult SaveNewMessage(IPDCMessagingDto dto)
        {
            var userId = SessionHelper.UserProfile.UserId;
            //dto.FromEmpId = SessionHelper.UserProfile.EmployeeId;
            var verification = _messaging.SaveNewMessage(dto, userId);
            return Json(verification, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveDraftMessage(IPDCMessagingDto dto)
        {
            var userId = SessionHelper.UserProfile.UserId;
            //dto.FromEmpId = SessionHelper.UserProfile.EmployeeId;
            var verification = _messaging.SaveDraftMessage(dto, userId);
            return Json(verification, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveReplyMessage(IPDCMessagingDto dto)
        {
            var userId = SessionHelper.UserProfile.UserId;
            var verification = _messaging.SaveReplyMessage(dto, userId);
            return Json(verification, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadMessage(long msgId, long? AppId)
        {
            var userId = SessionHelper.UserProfile.UserId;
            var data = _messaging.LoadMessage(msgId, AppId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult LoadMessage(long msgId, long? AppId)
        //{
        //    var userId = SessionHelper.UserProfile.UserId;
        //    var data = _messaging.LoadMessage(msgId, userId);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult LoadDraftMessages(long? msgId, long? AppId)
        {
            var userId = SessionHelper.UserProfile.UserId;
            var data = _messaging.LoadDraftMessages(msgId, AppId, userId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}