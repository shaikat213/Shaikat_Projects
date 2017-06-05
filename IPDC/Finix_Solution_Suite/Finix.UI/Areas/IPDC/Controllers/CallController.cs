using Finix.IPDC.Infrastructure;
using Finix.IPDC.Util;
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
    public class CallController : BaseController
    {
        private readonly CallFacade _callFacade = new CallFacade();
        private readonly EnumFacade _enumFacade = new EnumFacade();
        private readonly SalesLeadFacade _saleLeadFacade = new SalesLeadFacade();
        private readonly EmployeeFacade _employee = new EmployeeFacade();
        // GET: IPDC/Employee
        public ActionResult CallEntry()
        {
            return View();
        }

        public ActionResult CallEntryCC()
        {
            return View();
        }        

        public ActionResult CallDetailsBM()
        {
            return View();
        }

        public JsonResult SaveCallEntry(CallDto dto)
        {
            try
            {
                ResponseDto response = new ResponseDto();

                if(dto.CustomerName == null && dto.CustomerPhone == null)
                {
                    response.Message = "Please Enter the Customer Name and Phone number";
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                if((dto.CallType != null && (dto.CallType == CallType.Auto_assign )) && (dto.CustomerAddress == null || dto.CustomerAddress.ThanaId == null || dto.CustomerAddress.ThanaId < 1))
                {
                    response.Message = "Address is mendatory for auto assignment.";
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                if (dto.CallType != null && dto.CallType == CallType.User_Assigned && (dto.ReferredTo == null || dto.ReferredTo <= 0))
                {
                    response.Message = "Referred to is mendatory for Referral assignment.";
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                if (dto.CallStatus != null && dto.CallStatus == CallStatus.Successful)
                {
                    bool hasFault = false;
                    if(dto.ProductId == null || dto.ProductId < 0)
                    {
                        response.Message = "Product";
                        hasFault = true;
                    }
                    if (dto.Amount == null || dto.Amount < 0)
                    {
                        response.Message += ", Amount";
                        hasFault = true;
                    }
                    if (dto.CustomerAddress.ThanaId == null)
                    {
                        response.Message += ", Full Address";
                        hasFault = true;
                    }
                    if (hasFault)
                    {
                        response.Message += " is mandetory.";
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                }
                if (!string.IsNullOrEmpty(dto.FollowUpCallTimeText))
                {
                    DateTime callTime = DateTime.Now;
                    var FromConverted = DateTime.TryParseExact(dto.FollowUpCallTimeText, "dd/MM/yyyy HH:mm",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out callTime);
                    if (FromConverted)
                    {
                        dto.FollowUpCallTime = callTime;
                    }
                }
                if (!string.IsNullOrEmpty(dto.DateOfBirthText))
                {
                    DateTime dob = DateTime.Now;
                    var FromConverted = DateTime.TryParseExact(dto.DateOfBirthText, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out dob);
                    if (FromConverted)
                    {
                        dto.DateOfBirth = dob;
                    }
                }
                var userId = SessionHelper.UserProfile.UserId;
                var result = _callFacade.SaveCallEntry(dto, userId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SaveCallEntryToLead(CallDto dto)
        {
            try
            {
                var userId = SessionHelper.UserProfile.UserId;
                if (!string.IsNullOrEmpty(dto.FollowUpCallTimeText))
                {
                    DateTime callTime = DateTime.Now;
                    var FromConverted = DateTime.TryParseExact(dto.FollowUpCallTimeText, "dd/MM/yyyy HH:mm",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out callTime);
                    if (FromConverted)
                    {
                        dto.FollowUpCallTime = callTime;
                    }
                }
                var result = _callFacade.SaveCallEntryToLead(dto, userId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SaveCallFailReason(CallDto dto)
        {
            try
            {
                var userId = SessionHelper.UserProfile.UserId;
                var result = _callFacade.SaveCallFailReason(dto, userId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SaveCallAssigned(CallDto dto)
        {
            try
            {
                var userId = SessionHelper.UserProfile.UserId;
                var result = _callFacade.SaveCallAssigned(dto, userId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetAllCallType()
        {
            var riskLevels = _enumFacade.GetAllCallType();
            return Json(riskLevels, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllCallTypeCC()
        {
            var riskLevels = _enumFacade.GetAllCallTypeCC();
            return Json(riskLevels, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllProduct()
        {
            var designations = _callFacade.GetAllProduct();
            return Json(designations, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllCallSource()
        {
            var riskLevels = _enumFacade.GetAllCallSource();
            return Json(riskLevels, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetAllCallStatus()
        //{
        //    var riskLevels = _enumFacade.GetAllCallStatus();
        //    return Json(riskLevels, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetAllSR()
        //{
        //    var userId = SessionHelper.UserProfile.UserId;
        //    var data = _callFacade.GetAllSR(userId);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetAllCallFailReason()
        {
            var riskLevels = _enumFacade.GetAllCallFailReason();
            return Json(riskLevels, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCallEntry(long callEntryId)
        {
            var data = _callFacade.GetCallEntry(callEntryId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerAgeRange()
        {
            var result = _enumFacade.GetAgeRange();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetIncomeRange()
        {
            var result = _enumFacade.GetIncomeRange();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCustomerPriorities()
        {
            var result = _enumFacade.GetCustomerPriorities();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCallModes()
        {
            var result = _enumFacade.GetCallModes();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMaritalStatuses()
        {
            var result = _enumFacade.GetMaritalStatuses();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLeadPriorities()
        {
            var result = _enumFacade.GetLeadPriorities();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region Search Call Information 
        
        public ActionResult SearchCallBMInformation(string sortOrder, string currentFilter, string searchString, int page = 1)
        {
            ViewBag.CurrentSort = sortOrder;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var temp = _callFacade.GetBMCallPagedList(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(temp);
        }

        public ActionResult SearchCallInformation(string sortOrder, string currentFilter, string searchString, int page = 1)
        {
            ViewBag.CurrentSort = sortOrder;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var temp = _callFacade.GetCallPagedList(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(temp);
        }

        public ActionResult RMUnsuccessfulCalls(string sortOrder, string currentFilter, string searchString, int page = 1)
        {
            ViewBag.CurrentSort = sortOrder;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var temp = _callFacade.GetUnsuccessfulCallPagedList(10, page, searchString, SessionHelper.UserProfile.UserId);
            return View(temp);
        }
        public ActionResult RMSuccessfulCalls(string sortOrder, string currentFilter, string searchString, int page = 1)
        {
            ViewBag.CurrentSort = sortOrder;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var temp = _callFacade.GetSuccessfulCallPagedList(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(temp);
        }
        #endregion

        public JsonResult GetAllCalls()
        {
            var result = _callFacade.GetAllCalls();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OwnCallList(string sortOrder, string currentFilter, string searchString, int page = 1)
        {

            ViewBag.CurrentSort = sortOrder;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var temp = _callFacade.GetOwnCallsPagedList(20, page, searchString, SessionHelper.UserProfile.UserId);
            return View(temp);
        }
    }
}