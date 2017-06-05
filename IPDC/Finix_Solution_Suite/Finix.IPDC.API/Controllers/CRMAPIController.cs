using Finix.IPDC.Facade;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Finix.IPDC.API.Helper;
using Finix.IPDC.API.Models;
using Finix.Auth.DTO;
using Finix.Auth.Facade;
using Finix.IPDC.DTO;
using System;
using Finix.IPDC.Infrastructure;
using System.Globalization;
using System.Linq;
using ApplicationFacade = Finix.IPDC.Facade.ApplicationFacade;
using System.Collections.Generic;
using System.IO;
namespace Finix.IPDC.Areas.API.Controllers
{
    public class CRMAPIController : ApiController
    {
        private readonly CallFacade _call = new CallFacade();
        private readonly LoginFacade _loginFacade = new LoginFacade();
        private readonly CallFacade _callFacade = new CallFacade();
        private readonly ProductFacade _productFacade = new ProductFacade();
        private readonly OfficeDesignationSettingFacade _DegSetting = new OfficeDesignationSettingFacade();
        private readonly SalesLeadFacade _salesLeadFacade = new SalesLeadFacade();
        private readonly MessagingFacade _messaging = new MessagingFacade();
        private readonly ProfessionFacade _profession = new ProfessionFacade();
        private readonly OfficeDesignationSettingFacade _officeDesignationSetting = new OfficeDesignationSettingFacade();
        private readonly ApplicationFacade _application = new ApplicationFacade();

        #region Login
        [HttpPost]
        public HttpResponseMessage ApiLogin(LogOnDto dto)
        {
            var res = BaseResponse.CreateWith(HttpStatusCode.OK);
            var login = _loginFacade.DoLogin(dto);
            if (login.UserId > 0)
            {
                login.ApiKey = BizConstants.ApiKey;


                var _employee = new EmployeeFacade();
                UserFacade _user = new UserFacade();
                var empId = _user.GetEmployeeIdByUserId(login.UserId);
                if (empId > 0)
                {
                    var emp = _employee.GetEmployeeByEmployeeId(empId);
                    var deg = _employee.GetOfficeAndDesignationByEmployeeId(empId);

                    res.Data.Add(new { UserId = login.UserId, UserName = login.UserName, Name = emp.Name, OfficeAndDesignation = deg });
                }
                else
                {
                    res.Data.Add(new { UserId = login.UserId, UserName = login.UserName });
                }
                //var roleIdList = _employee.GetEmpRoleIdList(empId);
                //var empAuthData = _loginFacade.RoleAssignmentByRoleIdList(roleIdList, SessionHelper.UserProfile.IsAdmin);
                //if (empAuthData != null)
                //{
                //    SessionHelper.UserProfile.Roles = empAuthData.Roles;
                //    SessionHelper.UserProfile.Tasks = empAuthData.Tasks;
                //}
            }

            return HttpResponseBuilder.BuildResponse(res);
        }

        [HttpGet]
        public HttpResponseMessage GetApiLogin(string UserName, string Password)
        {
            var res = BaseResponse.CreateWith(HttpStatusCode.OK);
            LogOnDto loginDto = new LogOnDto();
            loginDto.UserName = UserName;
            loginDto.Password = Password;
            var login = _loginFacade.DoLogin(loginDto);
            if (login.UserId > 0)
            {
                login.ApiKey = BizConstants.ApiKey;


                var _employee = new EmployeeFacade();
                UserFacade _user = new UserFacade();
                var empId = _user.GetEmployeeIdByUserId(login.UserId);
                if (empId > 0)
                {
                    var emp = _employee.GetEmployeeByEmployeeId(empId);
                    var deg = _employee.GetOfficeAndDesignationByEmployeeId(empId);
                    var roleIdList = _employee.GetEmpRoleIdList(empId);

                    res.Data.Add(new
                    {
                        UserId = login.UserId,
                        UserName = login.UserName,
                        Name = emp.Name,
                        OfficeAndDesignation = deg,
                        RoleIds = roleIdList,
                        IMEI = emp.IMEINo,
                        ProfilePic = Path.GetFileName(emp.Photo)
                    });
                }
                else
                {
                    res.Data.Add(new { UserId = login.UserId, UserName = login.UserName });
                }
            }
            return HttpResponseBuilder.BuildResponse(res);
        }

        [HttpPost]
        public HttpResponseMessage ChangePassword(ChangePasswordDto dto)
        {
            BaseResponse res;
            if (!string.IsNullOrEmpty(dto.ApiKey) && dto.ApiKey == BizConstants.ApiKey)
            {                
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                var result = _loginFacade.ChangePassword(dto);
                res.Data.Add(new { Success = result.Success, Message = result.Message });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }
            return HttpResponseBuilder.BuildResponse(res);
        }
        #endregion

        #region Call
        public HttpResponseMessage GetAllCalls(string ApiKey)
        {
            BaseResponse res;
            if (ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                var callList = _call.GetAllCalls();
                res.Data.Add(new { CallList = callList });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }
            return HttpResponseBuilder.BuildResponse(res);
        }
        [HttpGet]
        public HttpResponseMessage GetBMListForCallReferal(string ApiKey)
        {
            BaseResponse res;
            if (ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                var designations = _officeDesignationSetting.GetOffDegSettingsForAssignment();
                res.Data.Add(new { Bms = designations });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }
            return HttpResponseBuilder.BuildResponse(res);
        }
        [HttpPost]
        public HttpResponseMessage SaveCallEntry(CallDto Call)//string ApiKey, long UserId, 
        {
            BaseResponse res;
            if (Call.ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                try
                {

                    if (Call.CustomerName == null && Call.CustomerPhone == null)
                    {
                        res.Data.Add(new { Success = false, Message = "Please Enter the Customer Name and Phone number" });
                        return HttpResponseBuilder.BuildResponse(res);
                    }

                    if ((Call.CallType != null && (Call.CallType == CallType.Auto_assign)) && (Call.CustomerAddress == null || Call.CustomerAddress.ThanaId == null || Call.CustomerAddress.ThanaId < 1))
                    {
                        res.Data.Add(new { Success = false, Message = "Address is mendatory for auto assignment." });
                        return HttpResponseBuilder.BuildResponse(res);
                    }
                    if (Call.CallType != null && Call.CallType == CallType.User_Assigned && (Call.ReferredTo == null || Call.ReferredTo <= 0))
                    {
                        res.Data.Add(new { Success = false, Message = "Referred to is mendatory for Referral assignment." });
                        return HttpResponseBuilder.BuildResponse(res);
                    }
                    if (Call.CallStatus != null && Call.CallStatus == CallStatus.Successful)
                    {
                        string Message = "";
                        bool hasFault = false;
                        if (Call.ProductId == null || Call.ProductId < 0)
                        {
                            Message = "Product";
                            hasFault = true;
                        }
                        if (Call.Amount == null || Call.Amount < 0)
                        {
                            Message += ", Amount";
                            hasFault = true;
                        }
                        if (Call.CustomerAddress.ThanaId == null)
                        {
                            Message += ", Full Address";
                            hasFault = true;
                        }
                        if (hasFault)
                        {
                            Message += " is mandetory.";
                            res.Data.Add(new { Success = false, Message = Message });
                            return HttpResponseBuilder.BuildResponse(res);
                        }
                    }
                    if (!string.IsNullOrEmpty(Call.FollowUpCallTimeText))
                    {
                        DateTime callTime = DateTime.Now;
                        var FromConverted = DateTime.TryParseExact(Call.FollowUpCallTimeText, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out callTime);
                        if (FromConverted)
                        {
                            Call.FollowUpCallTime = callTime;
                        }
                    }
                    if (!string.IsNullOrEmpty(Call.DateOfBirthText))
                    {
                        DateTime dob = DateTime.Now;
                        var FromConverted = DateTime.TryParseExact(Call.DateOfBirthText, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dob);
                        if (FromConverted)
                        {
                            Call.DateOfBirth = dob;
                        }
                    }
                    var result = _callFacade.SaveCallEntry(Call, (long)Call.UserId);
                    res.Data.Add(new { Success = result.Success, Message = result.Message, result.Id });
                }
                catch (Exception ex)
                {
                    //return Json(ex, JsonRequestBehavior.AllowGet);
                    res = BaseResponse.CreateWith(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }

            return HttpResponseBuilder.BuildResponse(res);
        }
        [HttpGet]
        public HttpResponseMessage RMUnfinishedCalls(string ApiKey, long userId, string searchString = "", int page = 1, int pageSize = 20)
        {
            BaseResponse res;
            if (ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);

                var CallList = _callFacade.RMUnfinishedCallsApi(pageSize, page, searchString, userId);
                res.Data.Add(new { CallList = CallList });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }

            return HttpResponseBuilder.BuildResponse(res);
        }
        [HttpGet]
        public HttpResponseMessage UnsuccessfulCalls(string ApiKey, long userId, string searchString = "", int page = 1, int pageSize = 20)
        {
            BaseResponse res;
            if (ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);

                var CallList = _callFacade.GetUnsuccessfulCallApi(pageSize, page, searchString, userId);
                res.Data.Add(new { CallList = CallList });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }

            return HttpResponseBuilder.BuildResponse(res);
        }
        [HttpGet]
        public HttpResponseMessage BMCallListForAssignment(string ApiKey, long userId, string searchString = "", int page = 1, int pageSize = 20)
        {
            BaseResponse res;
            if (ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);

                var CallList = _callFacade.GetBMCallForAssignmentApi(pageSize, page, searchString, userId);
                res.Data.Add(new { CallList = CallList });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }

            return HttpResponseBuilder.BuildResponse(res);
        }
        [HttpPost]
        public HttpResponseMessage SaveCallAssignment(CallDto Call)
        {
            BaseResponse res;
            if (Call.ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                try
                {
                    var result = _callFacade.SaveCallAssigned(Call, (long)Call.UserId);
                    res.Data.Add(new { Success = result.Success, Message = result.Message, result.Id });
                }
                catch (Exception ex)
                {
                    res = BaseResponse.CreateWith(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }

            return HttpResponseBuilder.BuildResponse(res);
        }
        #endregion

        #region Sales Lead
        [HttpGet]
        public HttpResponseMessage UnsuccessfulLeads(string ApiKey, long userId, string searchString = "", int page = 1, int pageSize = 20)
        {
            BaseResponse res;
            if (ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);

                var LeadList = _salesLeadFacade.GetUnsuccessfulAssignedSalesLeadsApi(userId, pageSize, page, searchString);
                res.Data.Add(new { LeadList = LeadList });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }

            return HttpResponseBuilder.BuildResponse(res);
        }

        [HttpGet]
        public HttpResponseMessage RMSalesLeads(string ApiKey, long userId, string searchString = "", int page = 1, int pageSize = 20)
        {

            BaseResponse res;
            if (ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                var salesLead = _salesLeadFacade.GetAssignedSalesLeadsApi(userId, searchString, page, pageSize);
                res.Data.Add(new { LeadList = salesLead });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }

            return HttpResponseBuilder.BuildResponse(res);
        }

        [HttpGet]
        public HttpResponseMessage RMSalesLeadsTillToday(string ApiKey, long userId)
        {
            BaseResponse res;
            if (ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                var salesLead = _salesLeadFacade.GetSalesLeadsTillToday(userId);
                res.Data.Add(new { LeadList = salesLead });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }
            return HttpResponseBuilder.BuildResponse(res);
        }

        [HttpPost]
        public HttpResponseMessage SaveSalesLeads(SalesLeadDto salesLead)
        {

            BaseResponse res;
            if (salesLead.ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                var result = _salesLeadFacade.SaveSalesLead(salesLead, (long)salesLead.UserId);
                res.Data.Add(new { Success = result.Success, Message = result.Message, result.Id });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }

            return HttpResponseBuilder.BuildResponse(res);
        }
        [HttpPost]
        public HttpResponseMessage SaveFollowUpTime(FollowupTrackDto followupTrack)
        {

            BaseResponse res;
            if (followupTrack.ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                DateTime nextFollowUp = DateTime.Now;
                DateTime callTime = DateTime.Now;
                var FromConverted = DateTime.TryParseExact(followupTrack.NextFollowUpTxt, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out nextFollowUp);
                var callTimeConverted = DateTime.TryParseExact(followupTrack.CallTimeTxt, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out callTime);
                if (FromConverted)
                {
                    followupTrack.NextFollowUp = nextFollowUp;
                }
                if (callTimeConverted)
                {
                    followupTrack.CallTime = callTime;
                }
                followupTrack.SubmittedBy = (long)followupTrack.UserId;
                var result = _salesLeadFacade.SaveFollowUpTime(followupTrack);
                res.Data.Add(new { Success = result.Success, Message = result.Message, result.Id });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }

            return HttpResponseBuilder.BuildResponse(res);

        }
        #endregion

        #region Messaging
        [HttpPost]
        public HttpResponseMessage SaveNewMessage(IPDCMessagingDto message)//string ApiKey, long UserId, 
        {
            BaseResponse res;
            if (!string.IsNullOrEmpty(message.ApiKey) && message.ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                try
                {
                    var result = _messaging.SaveNewMessage(message, (long)message.UserId);
                    res.Data.Add(new { Success = result.Success, Message = result.Message, result.Id });
                }
                catch (Exception ex)
                {
                    res = BaseResponse.CreateWith(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }
            return HttpResponseBuilder.BuildResponse(res);
        }
        [HttpPost]
        public HttpResponseMessage SaveReplyMessage(IPDCMessagingDto message)//string ApiKey, long UserId, 
        {
            BaseResponse res;
            if (!string.IsNullOrEmpty(message.ApiKey) && message.ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                try
                {
                    var result = _messaging.SaveReplyMessage(message, (long)message.UserId);
                    res.Data.Add(new { Success = result.Success, Message = result.Message, result.Id });
                }
                catch (Exception ex)
                {
                    res = BaseResponse.CreateWith(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }
            return HttpResponseBuilder.BuildResponse(res);
        }
        [HttpPost]
        public HttpResponseMessage SaveDraftMessage(IPDCMessagingDto message)//string ApiKey, long UserId, 
        {
            BaseResponse res;
            if (!string.IsNullOrEmpty(message.ApiKey) && message.ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                try
                {
                    var result = _messaging.SaveDraftMessage(message, (long)message.UserId);
                    res.Data.Add(new { Success = result.Success, Message = result.Message, result.Id });
                }
                catch (Exception ex)
                {
                    res = BaseResponse.CreateWith(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }
            return HttpResponseBuilder.BuildResponse(res);
        }
        [HttpGet]
        public HttpResponseMessage MessageInbox(string ApiKey, long userId, string searchString = "", int page = 1, int pageSize = 20)
        {
            BaseResponse res;
            if (ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                var inboxMessages = _messaging.APIGetInboxMessages(pageSize, page, searchString, userId);
                res.Data.Add(new { MesageList = inboxMessages });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }

            return HttpResponseBuilder.BuildResponse(res);
        }
        [HttpGet]
        public HttpResponseMessage MessageOutbox(string ApiKey, long userId, string searchString = "", int page = 1, int pageSize = 20)
        {
            BaseResponse res;
            if (ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                var inboxMessages = _messaging.APIGetOutBoxMessages(pageSize, page, searchString, userId);
                res.Data.Add(new { MesageList = inboxMessages });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }

            return HttpResponseBuilder.BuildResponse(res);
        }
        [HttpGet]
        public HttpResponseMessage MessageDrafts(string ApiKey, long userId, string searchString = "", int page = 1, int pageSize = 20)
        {

            BaseResponse res;
            if (ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                var inboxMessages = _messaging.APIGetDraftedMessages(pageSize, page, searchString, userId);
                res.Data.Add(new { MesageList = inboxMessages });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }

            return HttpResponseBuilder.BuildResponse(res);
        }
        [HttpGet]
        public HttpResponseMessage MessageHistorybyAppId(string ApiKey, long appId, string searchString = "", int page = 1, int pageSize = 20)
        {
            BaseResponse res;
            if (ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                var inboxMessages = _messaging.APIMessageListByAppId(pageSize, page, searchString, appId);
                res.Data.Add(new { MesageList = inboxMessages });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }
            return HttpResponseBuilder.BuildResponse(res);
        }
        #endregion

        #region Applicaiton
        [HttpGet]
        public HttpResponseMessage GetApplicationsbyStage(string ApiKey, ProductType productType, ApplicationStage appStage, long userId, string searchString = "", int page = 1, int pageSize = 20)
        {
            BaseResponse res;
            if (ApiKey == BizConstants.ApiKey)
            {
                var appStages = new List<ApplicationStage>();
                if (productType == ProductType.Loan)
                {
                    if (appStage == ApplicationStage.Drafted)
                        appStages.Add(ApplicationStage.Drafted);
                    else if (appStage == ApplicationStage.SentToTL)
                    {
                        appStages.Add(ApplicationStage.SentToTL);
                        appStages.Add(ApplicationStage.SentToBM);
                        appStages.Add(ApplicationStage.SentToCRM);
                        appStages.Add(ApplicationStage.UnderProcessAtCRM);
                    }
                    else if (appStage == ApplicationStage.SentToOperations)
                    {
                        appStages.Add(ApplicationStage.SentToOperations);
                        appStages.Add(ApplicationStage.UnderProcessAtOperations);
                        appStages.Add(ApplicationStage.DCLUnderProcess);
                        appStages.Add(ApplicationStage.DCLApproved);
                        appStages.Add(ApplicationStage.POUnderProcess);
                        appStages.Add(ApplicationStage.ApprovedByOperations);
                        appStages.Add(ApplicationStage.ReadyForDeisbursement);

                    }
                    else if (appStage == ApplicationStage.PartialDisbursementComplete)
                    {
                        appStages.Add(ApplicationStage.PartialDisbursementComplete);
                        appStages.Add(ApplicationStage.DisbursementComplete);
                    }
                }
                else if (productType == ProductType.Deposit)
                {
                    if (appStage == ApplicationStage.Drafted)
                    {
                        appStages.Add(ApplicationStage.Drafted);
                        appStages.Add(ApplicationStage.SentToTL);
                        appStages.Add(ApplicationStage.SentToBM);
                    }
                    else if (appStage == ApplicationStage.SentToOperations)
                    {
                        appStages.Add(ApplicationStage.SentToOperations);
                        appStages.Add(ApplicationStage.UnderProcessAtOperations);
                        appStages.Add(ApplicationStage.FundReceived);
                        appStages.Add(ApplicationStage.DCLUnderProcess);
                        appStages.Add(ApplicationStage.DCLApproved);
                        appStages.Add(ApplicationStage.AccountOpeningUnderProcess);
                    }
                    else if (appStage == ApplicationStage.AccountOpened)
                    {
                        appStages.Add(ApplicationStage.AccountOpened);
                        appStages.Add(ApplicationStage.InstrumentReady);
                        appStages.Add(ApplicationStage.InsturmentDeliveredtoClient);
                        appStages.Add(ApplicationStage.InsturmentSentToRM);
                        appStages.Add(ApplicationStage.InsturmentSentToBranch);
                        appStages.Add(ApplicationStage.InsturmentKeptinFile);
                        appStages.Add(ApplicationStage.PendingIssue);
                    }
                }
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                var applications = _application.GetApplicationListApi(pageSize, page, searchString, appStages, productType, userId);
                res.Data.Add(new
                {
                    ApplicationList = applications.Select(s => new
                    {
                        s.Id,
                        s.ApplicationNo,
                        s.AccountTitle,
                        s.ApplicantName,
                        s.MaturityAmount,
                        s.Rate,
                        s.Term,
                        s.ApplicationStage,
                        s.ApplicationStageName,
                        s.CurrentHolding,
                        s.CurrentHoldingName,
                        s.CreateDate
                    })
                });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }

            return HttpResponseBuilder.BuildResponse(res);
        }

        [HttpGet]
        public HttpResponseMessage GetApplicationsForApproval(string ApiKey, long userId, string searchString = "", int page = 1, int pageSize = 20)
        {
            BaseResponse res;
            if (ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                var applications = _application.GetApplicationApprovalListApi(pageSize, page, searchString, userId);
                res.Data.Add(new { ApplicationList = applications });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }

            return HttpResponseBuilder.BuildResponse(res);

        }

        [HttpPost]
        public HttpResponseMessage SubmitApplicationToBm(ApplicationSubmissionDto dto)
        {
            BaseResponse res;
            if (dto.ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                var result = _application.SubmitApplicationToBm(dto.ApplicationId, dto.UserId, dto.Comment, dto.IsTL);
                res.Data.Add(new { Success = result.Success, Message = result.Message, result.Id });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }

            return HttpResponseBuilder.BuildResponse(res);
        }

        [HttpPost]
        public HttpResponseMessage SubmitApplicationToCRM(ApplicationSubmissionDto dto)
        {
            BaseResponse res;
            if (dto.ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                var result = _application.SubmitApplicationToCRM(dto.ApplicationId, dto.UserId, dto.Comment);
                res.Data.Add(new { Success = result.Success, Message = result.Message, result.Id });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }

            return HttpResponseBuilder.BuildResponse(res);
        }

        [HttpPost]
        public HttpResponseMessage ApplicationRejectedByTL(ApplicationSubmissionDto dto)
        {
            BaseResponse res;
            if (dto.ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                var result = _application.CancelApplication(dto.ApplicationId, dto.Comment, ApplicationStage.RejectedByTL, dto.UserId);
                res.Data.Add(new { Success = result.Success, Message = result.Message, result.Id });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }

            return HttpResponseBuilder.BuildResponse(res);
        }

        [HttpPost]
        public HttpResponseMessage ApplicationRejectedByBM(ApplicationSubmissionDto dto)
        {
            BaseResponse res;
            if (dto.ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                var result = _application.CancelApplication(dto.ApplicationId, dto.Comment, ApplicationStage.RejectedByBM, dto.UserId);
                res.Data.Add(new { Success = result.Success, Message = result.Message, result.Id });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }
            return HttpResponseBuilder.BuildResponse(res);
        }
        #endregion

        #region Notification
        [HttpGet]
        public HttpResponseMessage GetNotifications(string ApiKey, long userId, bool unreadOnly = true)
        {
            BaseResponse res;
            if (ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                var facade = new NotificationFacade();

                var notifications = facade.GetNotifications(userId, unreadOnly: unreadOnly);

                //start of populate submodule id
                var menus = N.Menus;
                foreach (var n in notifications)
                {
                    if (menus.Any(x => x.Id == n.MenuId))
                        n.SubModuleId = menus.First(x => x.Id == n.MenuId).SubModuleId;
                }
                //end of populate submodue id

                var data = notifications.GroupBy(x => x.NotificationType.ToString(),
                (k, g) => new
                {
                    NotificationType = k.ToString(),
                    Notifications = g
                }).ToList();

                res.Data.Add(new { Notifications = data });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }

            return HttpResponseBuilder.BuildResponse(res);



        }
        #endregion

        #region Basic data
        [HttpGet]
        public HttpResponseMessage GetProducts(string ApiKey)
        {
            BaseResponse res;
            if (ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                var products = _productFacade.GetAllProducts();
                res.Data.Add(new { products });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }

            return HttpResponseBuilder.BuildResponse(res);
        }
        [HttpGet]
        public HttpResponseMessage GetBMListForAutoAssign(string ApiKey)
        {
            BaseResponse res;
            if (ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                var designations = _DegSetting.GetOffDegSettingsForAssignment();
                res.Data.Add(new { designations });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }

            return HttpResponseBuilder.BuildResponse(res);
        }
        [HttpGet]
        public HttpResponseMessage GetBMSubrodinatesForCallAssignment(string ApiKey, long UserId)
        {
            BaseResponse res;
            if (ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                var EmployeeList = _DegSetting.GetAllEmpByBM(UserId);
                res.Data.Add(new { EmployeeList });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }

            return HttpResponseBuilder.BuildResponse(res);
        }
        [HttpGet]
        public HttpResponseMessage GetEmployeeWithDesignation(string ApiKey)
        {
            BaseResponse res;
            if (ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                var designations = _officeDesignationSetting.GetEmployeeWithDesignation();
                res.Data.Add(new { LeadList = designations });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }
            return HttpResponseBuilder.BuildResponse(res);


        }
        [HttpGet]
        public HttpResponseMessage GetEmployeeWithDesignationOfffice(string ApiKey, long userId)
        {
            BaseResponse res;
            if (ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                var designations = _officeDesignationSetting.GetEmployeeWithDesignationOfffice(userId);
                res.Data.Add(new { LeadList = designations });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }
            return HttpResponseBuilder.BuildResponse(res);

        }
        [HttpGet]
        public HttpResponseMessage GetProfessions(string ApiKey)
        {
            BaseResponse res;
            if (ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                var profession = _profession.GetAllProfession();
                res.Data.Add(new { profession });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }

            return HttpResponseBuilder.BuildResponse(res);
        }
        [HttpGet]
        public HttpResponseMessage GetMobileDashboardData(string ApiKey, long userId)
        {
            BaseResponse res;
            if (ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                var _dashboard = new DashboardFacade();
                var Dashboard = _dashboard.GetMobileDashboardData(userId);
                res.Data.Add(new { Dashboard });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }

            return HttpResponseBuilder.BuildResponse(res);
            //
        }
        [HttpGet]
        public HttpResponseMessage GetMessageAndNotificationCount(string ApiKey, long userId)
        {
            BaseResponse res;
            if (ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                var _dashboard = new DashboardFacade();
                var Dashboard = _dashboard.GetMessageAndNotificationCount(userId);
                res.Data.Add(new { Dashboard });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }

            return HttpResponseBuilder.BuildResponse(res);
        }
        [HttpGet]
        public HttpResponseMessage NotificationMarkAsRead(string ApiKey, long userId, long notificationId)
        {
            BaseResponse res;
            if (ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                new NotificationFacade().MarkAsRead(new List<long> { notificationId });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }
            return HttpResponseBuilder.BuildResponse(res);
        }
        #endregion

        #region GPS 
        [HttpPost]
        public HttpResponseMessage SaveGPSInfo(GPSLogDto logDto)
        {
            BaseResponse res;
            if (logDto.ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                new GPSFacade().SaveGPSInfo(logDto);
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }

            return HttpResponseBuilder.BuildResponse(res);
        }
        [HttpGet]
        public HttpResponseMessage LoadGPSInfo(string ApiKey, string IMEI)
        {
            BaseResponse res;
            if (ApiKey == BizConstants.ApiKey)
            {
                res = BaseResponse.CreateWith(HttpStatusCode.OK);
                var result = new GPSFacade().LoadGPSInfo(IMEI);
                res.Data.Add(new { Location = result });
            }
            else
            {
                res = BaseResponse.CreateWith(HttpStatusCode.Forbidden);
            }

            return HttpResponseBuilder.BuildResponse(res);
        }
        

        #endregion
    }

    public class ApplicationSubmissionDto
    {
        public string ApiKey { get; set; }
        public long UserId { get; set; }
        public long ApplicationId { get; set; }
        public string Comment { get; set; }
        public bool IsTL { get; set; }
    }
}
