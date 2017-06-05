using AutoMapper;
using Finix.IPDC.DTO;
using Finix.IPDC.Infrastructure.Models;
using Finix.IPDC.Service;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Transactions;
using Finix.IPDC.Infrastructure;
using PagedList;
using Finix.Auth.Facade;
using Finix.IPDC.Util;
using Microsoft.Practices.ObjectBuilder2;

namespace Finix.IPDC.Facade
{
    public class ApplicationFacade : BaseFacade
    {
        private SequencerFacade _sequencer = new SequencerFacade();
        private readonly UserFacade _user = new UserFacade();
        private readonly EmployeeFacade _employee = new EmployeeFacade();
        private readonly OfficeDesignationSettingFacade _designation = new OfficeDesignationSettingFacade();

        #region common application
        public ResponseDto SaveApplication(ApplicationDto dto, long userId)
        {
            var entity = new Application();
            Address address;
            ResponseDto response = new ResponseDto();
            if (dto.ApplicationDate == DateTime.MinValue)
            {
                response.Message = "Please Provide Application Date";
                return response;
            }
            var log = new ApplicationLog();
            long employeeId = _user.GetEmployeeIdByUserId((long)userId);
            long parentEmpId = employeeId != 0 ? _employee.GetEmployeeSupervisorEmpId(employeeId) : 0;
            var parentUser = parentEmpId != 0 ? _user.GetUserByEmployeeId(parentEmpId) : null;
            if (dto != null && dto.Id > 0)
            {
                log.Activity = Activity.Submit;
                log.AppIdRef = dto.Id;
                log.ApplicationId = (long)dto.Id;
                log.AppType = AppType.Application;
                log.CreateDate = DateTime.Now;
                log.CreatedBy = userId;
                log.FromUserId = userId;
                log.FromStage = dto.ApplicationStage;
                log.ToStage = ApplicationStage.Drafted; // Previous 'Edit'
                log.ToUserId = parentUser != null ? (long)parentUser.Id : 0;
                log.Status = EntityStatus.Active;
                #region application update

                if (dto.Id != null) entity = GenService.GetById<Application>((long)dto.Id);
                dto.RMId = entity.RMId;
                dto.RMCode = entity.RMCode;
                dto.ApplicationNo = entity.ApplicationNo;
                dto.BranchId = entity.BranchId;
                dto.CreateDate = entity.CreateDate;
                dto.CreatedBy = entity.CreatedBy;
                dto.CurrentHolding = entity.CurrentHolding;
                dto.RejectedOn = entity.RejectedOn;
                dto.RejectionReason = entity.RejectionReason;
                dto.RejectedByEmpId = entity.RejectedByEmpId;
                if (dto.Status == null)
                    dto.Status = entity.Status;
                if (dto.ApplicationStage == null)
                    dto.ApplicationStage = entity.ApplicationStage;
                using (var tran = new TransactionScope())
                {
                    try
                    {
                        entity = Mapper.Map(dto, entity);
                        entity.EditDate = DateTime.Now;

                        if (dto.GroupAddress.IsChanged)
                        {
                            if (dto.GroupAddress.Id != null)
                            {
                                address = GenService.GetById<Address>((long)dto.GroupAddress.Id);
                                dto.GroupAddress.CreateDate = address.CreateDate;
                                dto.GroupAddress.CreatedBy = address.CreatedBy;
                                address = Mapper.Map(dto.GroupAddress, address);
                                GenService.Save(address);
                                //dto.GroupAddressId = address.Id;
                                entity.GroupAddressId = address.Id;
                            }
                            else if (dto.GroupAddress != null && dto.GroupAddress.CountryId != null && dto.GroupAddress.CountryId > 0)
                            {
                                var permanentAddress = Mapper.Map<Address>(dto.GroupAddress);
                                GenService.Save(permanentAddress);
                                //dto.GroupAddressId = permanentAddress.Id;
                                entity.GroupAddressId = permanentAddress.Id;
                            }

                        }
                        GenService.Save(entity);
                        #region list updates
                        if (dto.DocChecklist != null)
                        {
                            foreach (var item in dto.DocChecklist)
                            {
                                AppDocChecklist docChecklist;
                                if (item.Id != null && item.Id > 0)
                                {
                                    docChecklist = GenService.GetById<AppDocChecklist>((long)item.Id);
                                    if (item.Status == null)
                                        item.Status = docChecklist.Status;
                                    item.CreateDate = docChecklist.CreateDate;
                                    item.CreatedBy = docChecklist.CreatedBy;
                                    item.ApplicationId = docChecklist.ApplicationId;
                                    item.EditDate = DateTime.Now;
                                    item.EditedBy = userId;
                                    Mapper.Map(item, docChecklist);
                                    GenService.Save(docChecklist);
                                }
                                else
                                {
                                    docChecklist = new AppDocChecklist();
                                    item.CreateDate = DateTime.Now;
                                    item.CreatedBy = userId;
                                    item.Status = EntityStatus.Active;
                                    item.ApplicationId = entity.Id;
                                    docChecklist = Mapper.Map<AppDocChecklist>(item);
                                    GenService.Save(docChecklist);
                                }
                            }
                        }

                        foreach (var item in dto.CIFList)
                        {
                            ApplicationCIFs cif;
                            if (item.Id != null && item.Id > 0)
                            {
                                cif = GenService.GetById<ApplicationCIFs>((long)item.Id);
                                if (item.Status == null)
                                    item.Status = cif.Status;
                                item.CreateDate = cif.CreateDate;
                                item.CreatedBy = cif.CreatedBy;
                                item.ApplicationId = cif.ApplicationId;
                                item.EditDate = DateTime.Now;
                                item.EditedBy = userId;
                                Mapper.Map(item, cif);
                                GenService.Save(cif);
                            }
                            else
                            {
                                cif = new ApplicationCIFs();
                                item.CreateDate = DateTime.Now;
                                item.CreatedBy = userId;
                                item.Status = EntityStatus.Active;
                                item.ApplicationId = entity.Id;
                                cif = Mapper.Map<ApplicationCIFs>(item);
                                GenService.Save(cif);
                            }
                        }

                        #endregion

                        #region list deletes
                        if (dto.RemovedCIFList != null)
                        {
                            foreach (var item in dto.RemovedCIFList)
                            {
                                var cif = GenService.GetById<ApplicationCIFs>(item);
                                if (cif != null)
                                {
                                    cif.Status = EntityStatus.Inactive;
                                    cif.EditDate = DateTime.Now;
                                    cif.EditedBy = userId;
                                }
                                GenService.Save(cif);
                            }
                        }

                        //if(dto.r)
                        #endregion
                        GenService.Save(log);
                        response.Id = entity.Id;
                        tran.Complete();
                    }
                    catch (Exception ex)
                    {
                        tran.Dispose();
                        response.Id = entity.Id;
                        response.Message = "Application Edit Operation Failed";
                        return response;
                    }
                }
                response.Success = true;
                response.Message = "Application Edited Successfully";
                return response;
                #endregion
            }
            else
            {
                #region application create
                //employeeId = _user.GetEmployeeIdByUserId(userId);
                var emp = _employee.GetEmployeeByEmployeeId(employeeId);
                entity.RMId = employeeId;
                if (emp != null)
                {
                    entity.RMCode = emp.RmCode;
                }
                entity = Mapper.Map<Application>(dto);
                if (entity.SalesLeadId != null)
                {
                    var salesLead = GenService.GetById<SalesLead>((long)entity.SalesLeadId);
                    entity.RMId = salesLead.ProspectiveByEmpId;
                    var officeId = _employee.GetEmployeeOfficeByEmployeeID(employeeId);
                    if (officeId > 0)
                        entity.BranchId = officeId;
                }
                entity.Status = EntityStatus.Active;
                entity.CreatedBy = userId;
                entity.CreateDate = DateTime.Now;
                entity.ApplicationStage = ApplicationStage.Drafted; //Previous - 'Save'
                if (dto != null)
                {
                    var groupAddress = Mapper.Map<Address>(dto.GroupAddress);
                    using (var tran = new TransactionScope())
                    {
                        try
                        {
                            if (groupAddress.CountryId != null)
                            {
                                GenService.Save(groupAddress);
                                entity.GroupAddressId = groupAddress.Id;
                            }

                            entity.ApplicationNo = _sequencer.GetUpdatedApplicationNo();
                            if (dto.DocChecklist != null && dto.DocChecklist.Count > 0)
                            {
                                entity.DocChecklist = new List<AppDocChecklist>();
                                entity.DocChecklist = Mapper.Map<List<AppDocChecklist>>(dto.DocChecklist);
                                foreach (var item in entity.DocChecklist)
                                {
                                    item.CreateDate = DateTime.Now;
                                    item.CreatedBy = userId;
                                    item.Status = EntityStatus.Active;
                                }
                            }
                            if (dto.CIFList != null && dto.CIFList.Count > 0)
                            {
                                entity.CIFList = new List<ApplicationCIFs>();
                                entity.CIFList = Mapper.Map<List<ApplicationCIFs>>(dto.CIFList);
                                foreach (var item in entity.CIFList)
                                {
                                    item.CreateDate = DateTime.Now;
                                    item.CreatedBy = userId;
                                    item.Status = EntityStatus.Active;
                                    item.ApplicationId = entity.Id;
                                }
                            }
                            GenService.Save(entity);
                            log.Activity = Activity.Submit;
                            log.AppIdRef = entity.Id;
                            log.ApplicationId = entity.Id;
                            log.AppType = AppType.Application;
                            log.CreateDate = DateTime.Now;
                            log.CreatedBy = userId;
                            log.FromUserId = userId;
                            log.FromStage = entity.ApplicationStage;
                            log.ToStage = ApplicationStage.Drafted; //Previous - 'Save'
                            log.ToUserId = parentUser != null ? (long)parentUser.Id : 0;
                            log.Status = EntityStatus.Active;
                            GenService.Save(log);
                            response.Id = entity.Id;
                            tran.Complete();
                        }
                        catch (Exception ex)
                        {
                            tran.Dispose();
                            response.Message = "Application Save Failed";
                            return response;
                        }
                    }
                }

                response.Success = true;
                response.Message = "Application Saved Successfully";
                return response;
                #endregion
            }

        }

        public ResponseDto CancelApplication(long appId, string RejectionReason, ApplicationStage? toApplicationStage, long userId)
        {
            var response = new ResponseDto();
            var application = GenService.GetById<Application>(appId);
            if (application == null)
            {
                response.Message = "No record found.";
                return response;
            }

            if (application.CurrentHolding == null || application.CurrentHolding == userId)
            {
                long employeeId = _user.GetEmployeeIdByUserId((long)userId);
                //long parentEmpId = _employee.GetEmployeeSupervisorEmpId(employeeId);
                //long? parentUserId = _user.GetUserByEmployeeId(parentEmpId).Id;
                var log = new ApplicationLog();
                var depoApplication = new DepositApplication();
                var loanApplication = new LoanApplication();
                if (application.DepositApplicationId != null)
                {
                    depoApplication = GenService.GetById<DepositApplication>((long)application.DepositApplicationId);
                    if (depoApplication != null)
                        depoApplication.Status = EntityStatus.Inactive;
                }
                if (application.LoanApplicationId != null)
                {
                    loanApplication = GenService.GetById<LoanApplication>((long)application.LoanApplicationId);
                    if (loanApplication != null)
                        loanApplication.Status = EntityStatus.Inactive;
                }
                log.Activity = Activity.Submit;
                log.AppIdRef = application.Id;
                log.ApplicationId = application.Id;
                log.AppType = AppType.Application;
                log.CreateDate = DateTime.Now;
                log.CreatedBy = userId;
                log.FromUserId = userId;
                log.FromStage = application.ApplicationStage;
                if (toApplicationStage != null)
                {
                    log.ToStage = (ApplicationStage)toApplicationStage;
                    application.ApplicationStage = (ApplicationStage)toApplicationStage;
                }
                else
                {
                    log.ToStage = ApplicationStage.ApplicationDisposed;
                    application.ApplicationStage = ApplicationStage.ApplicationDisposed;
                }

                log.Status = EntityStatus.Active;

                application.RejectedByEmpId = employeeId;
                application.RejectedOn = DateTime.Now;
                if (!string.IsNullOrEmpty(RejectionReason))
                {
                    application.RejectionReason = RejectionReason;
                }
                application.Status = EntityStatus.Inactive;
                using (var tran = new TransactionScope())
                {
                    try
                    {
                        GenService.Save(application);
                        GenService.Save(depoApplication);
                        GenService.Save(loanApplication);
                        GenService.Save(log);
                        if (application.ApplicationStage == ApplicationStage.RejectedByCRM)
                        {
                            N.CreateNotificationForService(NotificationType.ApplicationRejectionByCA, (long)application.Id);
                        }
                        if (application.ApplicationStage == ApplicationStage.RejectedByBM)
                        {
                            N.CreateNotificationForService(NotificationType.ApplicationRejectionByBM, (long)application.Id);
                        }
                        if (application.ApplicationStage == ApplicationStage.RejectedByTL)
                        {
                            N.CreateNotificationForService(NotificationType.ApplicationRejectionByTL, (long)application.Id);
                        }
                        if (application.ApplicationStage == ApplicationStage.RejectedByOperations)
                        {
                            N.CreateNotificationForService(NotificationType.ApplicationRejectionByOpsAndFacilityCloser, (long)application.Id);
                        }
                        if (application.ApplicationStage == ApplicationStage.RejectedByMCC)
                        {
                            N.CreateNotificationForService(NotificationType.ApplicationRejectionByCreditMemoDisapproval, (long)application.Id);
                        }
                        tran.Complete();
                        response.Success = true;
                        response.Message = "Application Canceled.";
                    }
                    catch (Exception ex)
                    {
                        tran.Dispose();
                        response.Message = "Application not Canceled";
                    }
                }
            }
            else
            {
                response.Message = "You dont have access to this application.";
            }

            return response;
        }

        public ResponseDto ApplicationSendToRM(long ApplicationId, long UserId)
        {
            var response = new ResponseDto();
            long employeeId = _user.GetEmployeeIdByUserId(UserId);
            var app = GenService.GetById<Application>(ApplicationId);
            var oldLogs = new List<ApplicationLog>();
            if (app != null)
            {
                oldLogs = GenService.GetAll<ApplicationLog>().Where(a => a.ApplicationId == ApplicationId).ToList();
                if (oldLogs != null && oldLogs.Count > 0)
                    oldLogs.ForEach(a => a.Status = EntityStatus.Inactive);
                var log = new ApplicationLog();
                log.Activity = Activity.Submit;
                log.AppIdRef = ApplicationId;
                log.ApplicationId = ApplicationId;
                log.AppType = AppType.Application;
                log.CreateDate = DateTime.Now;
                log.CreatedBy = UserId;
                log.FromUserId = UserId;
                log.FromStage = app.ApplicationStage;

                log.ToStage = ApplicationStage.Drafted;
                app.ApplicationStage = ApplicationStage.Drafted;
                app.Status = EntityStatus.Active;
                app.EditDate = DateTime.Now;
                app.EditedBy = UserId;
                log.Status = EntityStatus.Active;

                GenService.Save(app, false);
                GenService.Save(log, false);
                if (oldLogs != null && oldLogs.Count > 0)
                    GenService.Save(oldLogs, false);
                GenService.SaveChanges();
                response.Id = ApplicationId;
                response.Success = true;
                response.Message = "Application sent to respective RM";
            }
            else
            {
                response.Message = "Application not found.";
            }

            return response;

        }

        public ApplicationDto LoadApplication(long AppId)
        {
            var app = GenService.GetById<Application>(AppId);
            if (app != null)
            {
                var data = Mapper.Map<ApplicationDto>(app);
                data.CIFList.RemoveAll(c => c.Status != EntityStatus.Active);
                data.DocChecklist = data.DocChecklist.Select(d => { d.IsChecked = true; return d; }).ToList();
                List<DocumentSetup> temp;
                if (data.CIFList != null && data.CIFList.FirstOrDefault().CIF_OrganizationalId != null)
                {
                    var org = GenService.GetById<CIF_Organizational>((long)data.CIFList.FirstOrDefault().CIF_OrganizationalId);
                    temp = app.Product.DocumentSetups.Where(d => d.CustomerType == ApplicationCustomerType.Organizational && d.CompanyLegalStatus == org.LegalStatus).ToList();
                }
                else
                {
                    temp = app.Product.DocumentSetups.Where(d => d.CustomerType == ApplicationCustomerType.Individual).ToList();
                }
                temp.RemoveAll(p => data.DocChecklist.Select(l => l.ProductDocId).ToList().Contains(p.Id));
                var extraDocCheckList = (from chk in temp
                                         select new AppDocChecklistDto
                                         {
                                             ProductDocId = chk.Id,
                                             ProductId = chk.ProductId,
                                             DocName = chk.Document.Name,
                                             IsChecked = false
                                         }).ToList();


                data.DocChecklist.AddRange(extraDocCheckList);
                return data;
            }

            else
                return null;
        }
        public ResponseDto SubmitApplicationToBm(long AppId, long UserId, string Comment, bool IsTL = false)
        {

            var response = new ResponseDto();
            var application = GenService.GetById<Application>(AppId);
            if (application == null)
            {
                response.Message = "No record found.";
                return response;
            }

            if (application.CreatedBy == UserId || application.CurrentHolding == UserId)
            {
                long employeeId = _user.GetEmployeeIdByUserId((long)UserId);
                long parentEmpId = _employee.GetEmployeeSupervisorEmpId(employeeId);
                long? parentUserId = _user.GetUserByEmployeeId(parentEmpId).Id;
                var log = new ApplicationLog();
                log.Activity = Activity.Submit;
                log.AppIdRef = application.Id;
                log.ApplicationId = application.Id;
                log.AppType = AppType.Application;
                log.CreateDate = DateTime.Now;
                log.CreatedBy = UserId;
                log.FromUserId = UserId;
                if (parentUserId != null)
                    log.ToUserId = (long)parentUserId;
                log.FromStage = application.ApplicationStage;
                if (IsTL == false)
                {
                    log.ToStage = ApplicationStage.SentToTL; // Previous 'SubmittedToBM'
                    application.ApplicationStage = ApplicationStage.SentToTL;// Previous 'SubmittedToBM'
                    N.CreateNotificationForService(NotificationType.ApplicationWaitingForApprovalByTL, (long)application.Id);
                }
                else
                {
                    log.ToStage = ApplicationStage.SentToBM; // Previous 'SubmittedToBM'
                    application.ApplicationStage = ApplicationStage.SentToBM;// Previous 'SubmittedToBM'
                    N.CreateNotificationForService(NotificationType.ApplicationWaitingForApprovalByBM, (long)application.Id);
                }
                log.ToUserId = parentUserId != null ? (long)parentUserId : UserId;
                log.Status = EntityStatus.Active;
                application.CurrentHolding = log.ToUserId;
                application.CurrentHoldingEmpId = parentEmpId;
                if (IsTL)
                {
                    application.TLComment = Comment;
                }
                using (var tran = new TransactionScope())
                {
                    try
                    {
                        GenService.Save(application);
                        GenService.Save(log);
                        tran.Complete();
                        response.Success = true;
                        response.Message = "Application submitted.";
                    }
                    catch (Exception ex)
                    {
                        tran.Dispose();
                        response.Message = "Application not submitted.";
                    }
                }
            }
            else
            {
                response.Message = "You dont have access to this application.";
            }

            return response;
        }
        public ResponseDto SubmitApplicationToCRM(long AppId, long UserId, string Comment)
        {
            var response = new ResponseDto();

            var application = GenService.GetById<Application>(AppId);
            if (application == null)
            {
                response.Message = "No record found.";
                return response;
            }

            if (application.CreatedBy == UserId || application.CurrentHolding == UserId)
            {

                var log = new ApplicationLog();
                log.Activity = Activity.Submit;
                log.AppIdRef = application.Id;
                log.ApplicationId = application.Id;
                log.AppType = AppType.Application;
                log.CreateDate = DateTime.Now;
                log.CreatedBy = UserId;
                log.FromUserId = UserId;
                log.FromStage = application.ApplicationStage;

                var userList = GenService.GetAll<ApplicationLog>()
                        .Where(a => a.ApplicationId == AppId)
                        .Select(a => a.FromUserId)
                        .Distinct()
                        .ToList();

                if (application.ProductType == ProductType.Loan)
                {
                    application.ApplicationStage = ApplicationStage.SentToCRM; //Previous 'SubmittedToCRM';
                    log.ToStage = ApplicationStage.SentToCRM; // Previous 'SubmittedToCRM'
                    //N.CreateNotificationForService(NotificationType.ApplicationForwardedToCRM, AppId);

                }
                else if (application.ProductType == ProductType.Deposit)
                {
                    application.ApplicationStage = ApplicationStage.SentToOperations; //Previous 'SubmitedToOperation'
                    log.ToStage = ApplicationStage.SentToOperations; // Previous 'SubmitedToOperations'
                    //N.CreateNotificationForService(NotificationType.DCLApprovedForDA, AppId);

                }
                log.ToUserId = UserId;
                log.Status = EntityStatus.Active;

                application.CurrentHolding = null;
                application.CurrentHoldingEmpId = null;
                application.BMComment = Comment;

                using (var tran = new TransactionScope())
                {
                    try
                    {
                        GenService.Save(application);
                        GenService.Save(log);
                        if (application.ProductType == ProductType.Loan && application.ApplicationStage == ApplicationStage.SentToCRM)
                        {
                            N.CreateNotificationForService(NotificationType.ApplicationForwardedToCRM, AppId);
                        }
                        if (application.ProductType == ProductType.Deposit && application.ApplicationStage == ApplicationStage.SentToOperations)
                        {
                            N.CreateNotificationForService(NotificationType.ApplicationReceivedFromBMForDA, AppId);
                        }
                        tran.Complete();
                        response.Success = true;
                        response.Message = "Application submitted.";
                    }
                    catch (Exception ex)
                    {
                        tran.Dispose();
                        response.Message = "Application not submitted.";
                    }
                }
                //}
            }
            else
            {
                response.Message = "You dont have access to this application.";
            }
            return response;
        }
        #endregion
        #region Deposit application
        public ResponseDto SaveDepositApplication(DepositApplicationDto dto, long UserId)
        {
            DepositApplication entity = new DepositApplication();
            ResponseDto responce = new ResponseDto();
            var log = new ApplicationLog();
            long employeeId = _user.GetEmployeeIdByUserId((long)UserId);
            long parentEmpId = employeeId != 0 ? _employee.GetEmployeeSupervisorEmpId(employeeId) : 0;
            //long? parentUserId = parentEmpId != 0 ? _user.GetUserByEmployeeId(parentEmpId).Id : 0;
            var parentUser = parentEmpId != 0 ? _user.GetUserByEmployeeId(parentEmpId) : null;
            if (dto.Id > 0)
            {
                if (dto.Id != null) entity = GenService.GetById<DepositApplication>((long)dto.Id);
                dto.CreateDate = entity.CreateDate;
                dto.CreatedBy = entity.CreatedBy;
                if (dto.Status == null)
                    dto.Status = entity.Status;
                using (var tran = new TransactionScope())
                {
                    try
                    {
                        Mapper.Map(dto, entity);
                        entity.EditDate = DateTime.Now;
                        GenService.Save(entity);
                        log.Activity = Activity.Submit;
                        log.AppIdRef = dto.Id;
                        log.ApplicationId = (long)dto.Application_Id;
                        log.AppType = AppType.DepositApplication;
                        log.CreateDate = DateTime.Now;
                        log.CreatedBy = UserId;
                        log.FromUserId = UserId;
                        log.FromStage = ApplicationStage.Drafted; // Previous 'Edit'
                        log.ToStage = ApplicationStage.Drafted; //Previous 'DepositEdit'
                        log.ToUserId = parentUser != null ? (long)parentUser.Id : 0;
                        log.Status = EntityStatus.Active;

                        #region list updates
                        //List<DepAppChequeDeposit> chequeDeposits = new List<DepAppChequeDeposit>();
                        if (dto.ChequeDeposits != null)
                        {
                            foreach (var item in dto.ChequeDeposits)
                            {
                                if (item.DepositAmount != null && item.DepositAmount > 0)
                                {
                                    DepAppChequeDeposit chequeDeposit;
                                    if (item.Id != null && item.Id > 0)
                                    {
                                        chequeDeposit = GenService.GetById<DepAppChequeDeposit>((long)item.Id);
                                        if (item.Status == null)
                                            item.Status = chequeDeposit.Status;
                                        item.CreateDate = chequeDeposit.CreateDate;
                                        item.CreatedBy = chequeDeposit.CreatedBy;
                                        item.DepositApplicationId = chequeDeposit.DepositApplicationId;
                                        item.EditDate = DateTime.Now;
                                        item.EditedBy = UserId;
                                        Mapper.Map(item, chequeDeposit);
                                        GenService.Save(chequeDeposit);
                                    }
                                    else
                                    {
                                        chequeDeposit = new DepAppChequeDeposit();
                                        item.CreateDate = DateTime.Now;
                                        item.CreatedBy = UserId;
                                        item.Status = EntityStatus.Active;
                                        item.DepositApplicationId = entity.Id;
                                        chequeDeposit = Mapper.Map<DepAppChequeDeposit>(item);
                                        GenService.Save(chequeDeposit);
                                    }
                                }
                            }
                        }

                        if (dto.TransferDeposits != null)
                        {
                            foreach (var item in dto.TransferDeposits)
                            {
                                if (item.DepositAmount != null && item.DepositAmount > 0)
                                {
                                    DepAppTransfer transferDeposit;
                                    if (item.Id != null && item.Id > 0)
                                    {
                                        transferDeposit = GenService.GetById<DepAppTransfer>((long)item.Id);
                                        if (item.Status == null)
                                            item.Status = transferDeposit.Status;
                                        item.CreateDate = transferDeposit.CreateDate;
                                        item.CreatedBy = transferDeposit.CreatedBy;
                                        item.DepositApplicationId = transferDeposit.DepositApplicationId;
                                        item.EditDate = DateTime.Now;
                                        item.EditedBy = UserId;
                                        Mapper.Map(item, transferDeposit);
                                        GenService.Save(transferDeposit);
                                    }
                                    else
                                    {
                                        transferDeposit = new DepAppTransfer();
                                        item.CreateDate = DateTime.Now;
                                        item.CreatedBy = UserId;
                                        item.Status = EntityStatus.Active;
                                        item.DepositApplicationId = entity.Id;
                                        transferDeposit = Mapper.Map<DepAppTransfer>(item);
                                        GenService.Save(transferDeposit);
                                    }
                                }
                            }
                        }

                        if (dto.CashDeposits != null)
                        {
                            foreach (var item in dto.CashDeposits)
                            {
                                if (item.DepositAmount != null && item.DepositAmount > 0)
                                {
                                    DepAppCash cashDeposit;
                                    if (item.Id != null && item.Id > 0)
                                    {
                                        cashDeposit = GenService.GetById<DepAppCash>((long)item.Id);
                                        if (item.Status == null)
                                            item.Status = cashDeposit.Status;
                                        item.CreateDate = cashDeposit.CreateDate;
                                        item.CreatedBy = cashDeposit.CreatedBy;
                                        item.DepositApplicationId = cashDeposit.DepositApplicationId;
                                        item.EditDate = DateTime.Now;
                                        item.EditedBy = UserId;
                                        Mapper.Map(item, cashDeposit);
                                        GenService.Save(cashDeposit);
                                    }
                                    else
                                    {
                                        cashDeposit = new DepAppCash();
                                        item.CreateDate = DateTime.Now;
                                        item.CreatedBy = UserId;
                                        item.Status = EntityStatus.Active;
                                        item.DepositApplicationId = entity.Id;
                                        cashDeposit = Mapper.Map<DepAppCash>(item);
                                        GenService.Save(cashDeposit);
                                    }
                                }
                            }
                        }

                        if (dto.Nominees != null)
                        {
                            foreach (var item in dto.Nominees)
                            {
                                //if (item.GuiardianCifId != null && item.GuiardianCifId > 0)
                                //{

                                DepositNominee nominee;
                                if (item.Id != null && item.Id > 0)
                                {
                                    nominee = GenService.GetById<DepositNominee>((long)item.Id);
                                    if (item.Status == null)
                                        item.Status = nominee.Status;
                                    item.CreateDate = nominee.CreateDate;
                                    item.CreatedBy = nominee.CreatedBy;
                                    item.DepositApplicationId = nominee.DepositApplicationId;
                                    item.EditDate = DateTime.Now;
                                    item.EditedBy = UserId;
                                    Mapper.Map(item, nominee);
                                    GenService.Save(nominee);
                                }
                                else
                                {
                                    nominee = new DepositNominee();
                                    item.CreateDate = DateTime.Now;
                                    item.CreatedBy = UserId;
                                    item.Status = EntityStatus.Active;
                                    item.DepositApplicationId = entity.Id;
                                    nominee = Mapper.Map<DepositNominee>(item);
                                    GenService.Save(nominee);
                                }
                                //}
                            }
                        }
                        #endregion

                        #region list deletes
                        if (dto.RemovedCashDeposits != null)
                        {
                            foreach (var item in dto.RemovedCashDeposits)
                            {
                                var cash = GenService.GetById<DepAppCash>(item);
                                if (cash != null)
                                {
                                    cash.Status = EntityStatus.Inactive;
                                    cash.EditDate = DateTime.Now;
                                    cash.EditedBy = UserId;
                                }
                                GenService.Save(cash);
                            }
                        }

                        if (dto.RemovedChequeDeposits != null)
                        {
                            foreach (var item in dto.RemovedChequeDeposits)
                            {
                                var cash = GenService.GetById<DepAppChequeDeposit>(item);
                                if (cash != null)
                                {
                                    cash.Status = EntityStatus.Inactive;
                                    cash.EditDate = DateTime.Now;
                                    cash.EditedBy = UserId;
                                }
                                GenService.Save(cash);
                            }
                        }

                        if (dto.RemovedTransferDeposits != null)
                        {
                            foreach (var item in dto.RemovedTransferDeposits)
                            {
                                var cash = GenService.GetById<DepAppTransfer>(item);
                                if (cash != null)
                                {
                                    cash.Status = EntityStatus.Inactive;
                                    cash.EditDate = DateTime.Now;
                                    cash.EditedBy = UserId;
                                }
                                GenService.Save(cash);
                            }
                        }

                        if (dto.RemovedNominees != null)
                        {
                            foreach (var item in dto.RemovedNominees)
                            {
                                var cash = GenService.GetById<DepositNominee>(item);
                                if (cash != null)
                                {
                                    cash.Status = EntityStatus.Inactive;
                                    cash.EditDate = DateTime.Now;
                                    cash.EditedBy = UserId;
                                }
                                GenService.Save(cash);
                            }
                        }

                        #endregion

                        GenService.Save(log);
                        tran.Complete();
                    }
                    catch (Exception ex)
                    {
                        tran.Dispose();
                        responce.Message = "Deposit Application Operation Failed";
                        return responce;
                    }
                }
                responce.Success = true;
                responce.Id = entity.Id;
                responce.Message = "Deposit Application Edited Successfully";
                return responce;
            }
            else
            {
                if (dto.Application_Id != null)
                {
                    var application = GenService.GetById<Application>((long)dto.Application_Id);

                    entity = Mapper.Map<DepositApplication>(dto);
                    //entity.CIFNo = _sequencer.GetUpdatedCIFPersonalNo();
                    entity.Status = EntityStatus.Active;
                    entity.CreatedBy = UserId;
                    entity.CreateDate = DateTime.Now;
                    using (var tran = new TransactionScope())
                    {
                        if (application.DepositApplicationId == null && application.LoanApplicationId == null)
                        {
                            try
                            {
                                #region populate list
                                if (dto.ChequeDeposits != null)
                                    dto.ChequeDeposits = dto.ChequeDeposits.Where(c => c.DepositAmount != null && c.DepositAmount > 0).ToList();
                                if (dto.ChequeDeposits != null && dto.ChequeDeposits.Count > 0)
                                {
                                    entity.ChequeDeposits = Mapper.Map<List<DepAppChequeDeposit>>(dto.ChequeDeposits);
                                    foreach (var item in entity.ChequeDeposits)
                                    {
                                        item.CreateDate = DateTime.Now;
                                        item.CreatedBy = UserId;
                                        item.Status = EntityStatus.Active;
                                    }
                                }
                                if (dto.TransferDeposits != null)
                                    dto.TransferDeposits = dto.TransferDeposits.Where(c => c.DepositAmount != null && c.DepositAmount > 0).ToList();
                                if (dto.TransferDeposits != null && dto.TransferDeposits.Count > 0)
                                {
                                    entity.TransferDeposits = Mapper.Map<List<DepAppTransfer>>(dto.TransferDeposits);
                                    foreach (var item in entity.TransferDeposits)
                                    {
                                        item.CreateDate = DateTime.Now;
                                        item.CreatedBy = UserId;
                                        item.Status = EntityStatus.Active;
                                    }
                                }
                                if (dto.CashDeposits != null)
                                    dto.CashDeposits = dto.CashDeposits.Where(c => c.DepositAmount != null && c.DepositAmount > 0).ToList();
                                if (dto.CashDeposits != null && dto.CashDeposits.Count > 0)
                                {
                                    entity.CashDeposits = Mapper.Map<List<DepAppCash>>(dto.CashDeposits);
                                    foreach (var item in entity.CashDeposits)
                                    {
                                        item.CreateDate = DateTime.Now;
                                        item.CreatedBy = UserId;
                                        item.Status = EntityStatus.Active;
                                    }
                                }
                                if (dto.Nominees != null)
                                    dto.Nominees = dto.Nominees.Where(c => c.GuiardianCifId != null).ToList();
                                if (dto.Nominees != null && dto.Nominees.Count > 0)
                                {

                                    entity.Nominees = Mapper.Map<List<DepositNominee>>(dto.Nominees);
                                    foreach (var item in entity.Nominees)
                                    {
                                        item.CreateDate = DateTime.Now;
                                        item.CreatedBy = UserId;
                                        item.Status = EntityStatus.Active;
                                    }
                                }

                                #endregion

                                GenService.Save(entity);
                                var check = entity.Id;
                                application.DepositApplicationId = entity.Id;
                                GenService.Save(application);
                                log.Activity = Activity.Submit;
                                log.AppIdRef = entity.Id;
                                log.ApplicationId = (long)dto.Application_Id;
                                log.AppType = AppType.DepositApplication;
                                log.CreateDate = DateTime.Now;
                                log.CreatedBy = UserId;
                                log.FromUserId = UserId;
                                log.FromStage = ApplicationStage.Drafted; // Previous 'Save'
                                log.ToStage = ApplicationStage.Drafted; //Previous 'DepositSave'
                                log.ToUserId = parentUser != null ? (long)parentUser.Id : 0;
                                log.Status = EntityStatus.Active;
                                GenService.Save(log);
                                tran.Complete();
                            }
                            catch (Exception ex)
                            {
                                tran.Dispose();
                                responce.Message = "Deposit Application Save Failed";
                                return responce;
                            }
                        }
                        else
                        {
                            responce.Message = "Already an application exists. Please edit or try to add a new application.";
                        }

                    }
                }

                responce.Success = true;
                responce.Id = entity.Id;
                responce.Message = "Deposit Application Saved Successfully";
                return responce;
            }
        }
        public DepositApplicationDto LoadDepositAppByAppId(long AppId)
        {
            var app = GenService.GetById<Application>(AppId);
            if (app.ProductType == ProductType.Deposit && app.DepositApplicationId != null && app.DepositApplicationId > 0)
            {
                var depApp = GenService.GetById<DepositApplication>((long)app.DepositApplicationId);
                if (depApp != null) { }
                return Mapper.Map<DepositApplicationDto>(depApp);
            }
            return null;
        }
        public static DateTime? ValueOrMin(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return DateTime.Parse(value);
        }
        #endregion

        #region Loan Application
        public ResponseDto SaveLoanApplication(LoanApplicationDto dto, long userId)
        {
            var entity = new LoanApplication();
            ResponseDto responce = new ResponseDto();
            var log = new ApplicationLog();
            long employeeId = _user.GetEmployeeIdByUserId((long)userId);
            long parentEmpId = employeeId != 0 ? _employee.GetEmployeeSupervisorEmpId(employeeId) : 0;
            long? parentUserId = parentEmpId != 0 ? _user.GetUserByEmployeeId(parentEmpId).Id : 0;

            if (dto.Id > 0)
            {
                #region update
                if (dto.Id != null)
                    entity = GenService.GetById<LoanApplication>((long)dto.Id);
                dto.CreateDate = entity.CreateDate;
                dto.CreatedBy = entity.CreatedBy;
                if (dto.Status == null)
                    dto.Status = entity.Status;
                entity = Mapper.Map(dto, entity);
                log.Activity = Activity.Submit;
                log.AppIdRef = dto.Id;
                log.ApplicationId = (long)dto.Application_Id;
                log.AppType = AppType.LoanApplication;
                log.CreateDate = DateTime.Now;
                log.CreatedBy = userId;
                log.FromUserId = userId;
                log.FromStage = ApplicationStage.Drafted; // Previous-'Edit'
                log.ToStage = ApplicationStage.Drafted; // Previous-'LoanEdit'
                log.ToUserId = parentUserId != null ? (long)parentUserId : 0;
                log.Status = EntityStatus.Active;
                using (var tran = new TransactionScope())
                {
                    try
                    {
                        entity.EditDate = DateTime.Now;
                        GenService.Save(entity);

                        #region List Updates

                        if (dto.Guarantors != null)
                        {
                            foreach (var item in dto.Guarantors)
                            {
                                Guarantor guarantor;
                                if (item.Id != null && item.Id > 0)
                                {
                                    guarantor = GenService.GetById<Guarantor>((long)item.Id);
                                    if (item.Status == null)
                                        item.Status = guarantor.Status;
                                    item.CreateDate = guarantor.CreateDate;
                                    item.CreatedBy = guarantor.CreatedBy;
                                    item.LoanApplicationId = guarantor.LoanApplicationId;
                                    item.EditDate = DateTime.Now;
                                    item.EditedBy = userId;
                                    Mapper.Map(item, guarantor);
                                    GenService.Save(guarantor);
                                }
                                else
                                {
                                    guarantor = new Guarantor();
                                    item.CreateDate = DateTime.Now;
                                    item.CreatedBy = userId;
                                    item.Status = EntityStatus.Active;
                                    item.LoanApplicationId = entity.Id;
                                    guarantor = Mapper.Map<Guarantor>(item);
                                    GenService.Save(guarantor);
                                }
                            }
                        }

                        else
                        {
                            responce.Message = "Guarantors Information Mandatory...!!!";
                        }

                        if (dto.WaiverRequests != null)
                        {
                            foreach (var item in dto.WaiverRequests)
                            {
                                LoanAppWaiverReq waiverReq;
                                if (item.Id != null && item.Id > 0)
                                {
                                    waiverReq = GenService.GetById<LoanAppWaiverReq>((long)item.Id);
                                    if (item.Status == null)
                                        item.Status = waiverReq.Status;
                                    item.CreateDate = waiverReq.CreateDate;
                                    item.CreatedBy = waiverReq.CreatedBy;
                                    item.LoanApplicationId = waiverReq.LoanApplicationId;
                                    item.EditDate = DateTime.Now;
                                    item.EditedBy = userId;
                                    Mapper.Map(item, waiverReq);
                                    GenService.Save(waiverReq);
                                }
                                else
                                {
                                    waiverReq = new LoanAppWaiverReq();
                                    item.CreateDate = DateTime.Now;
                                    item.CreatedBy = userId;
                                    item.Status = EntityStatus.Active;
                                    item.LoanApplicationId = entity.Id;
                                    waiverReq = Mapper.Map<LoanAppWaiverReq>(item);
                                    GenService.Save(waiverReq);
                                }
                            }
                        }
                        if (dto.LoanAppColSecurities != null)
                        {
                            foreach (var item in dto.LoanAppColSecurities)
                            {
                                LoanAppColSecurity colSecurity;
                                if (item.Id != null && item.Id > 0)
                                {
                                    colSecurity = GenService.GetById<LoanAppColSecurity>((long)item.Id);
                                    if (item.Status == null)
                                        item.Status = colSecurity.Status;
                                    item.CreateDate = colSecurity.CreateDate;
                                    item.CreatedBy = colSecurity.CreatedBy;
                                    item.LoanApplicationId = colSecurity.LoanApplicationId;
                                    item.EditDate = DateTime.Now;
                                    item.EditedBy = userId;
                                    Mapper.Map(item, colSecurity);
                                    GenService.Save(colSecurity);
                                }
                                else
                                {
                                    colSecurity = new LoanAppColSecurity();
                                    item.CreateDate = DateTime.Now;
                                    item.CreatedBy = userId;
                                    item.Status = EntityStatus.Active;
                                    item.LoanApplicationId = entity.Id;
                                    colSecurity = Mapper.Map<LoanAppColSecurity>(item);
                                    GenService.Save(colSecurity);
                                }
                            }
                        }
                        if (dto.OtherSecurities != null)
                        {
                            foreach (var item in dto.OtherSecurities)
                            {
                                LoanOtherSecurities otherSecurity;
                                if (item.Id != null && item.Id > 0)
                                {
                                    otherSecurity = GenService.GetById<LoanOtherSecurities>((long)item.Id);
                                    if (item.Status == null)
                                        item.Status = otherSecurity.Status;
                                    item.CreateDate = otherSecurity.CreateDate;
                                    item.CreatedBy = otherSecurity.CreatedBy;
                                    item.LoanApplicationId = otherSecurity.LoanApplicationId;
                                    item.EditDate = DateTime.Now;
                                    item.EditedBy = userId;
                                    Mapper.Map(item, otherSecurity);
                                    GenService.Save(otherSecurity);
                                }
                                else
                                {
                                    otherSecurity = new LoanOtherSecurities();
                                    item.CreateDate = DateTime.Now;
                                    item.CreatedBy = userId;
                                    item.Status = EntityStatus.Active;
                                    item.LoanApplicationId = entity.Id;
                                    otherSecurity = Mapper.Map<LoanOtherSecurities>(item);
                                    GenService.Save(otherSecurity);
                                }
                            }
                        }

                        if (dto.FDRPrimarySecurity != null)
                        {
                            if (dto.FDRPrimarySecurity.Id > 0)
                            {
                                if (dto.FDRPrimarySecurity.FDRPSDetails != null)
                                {
                                    foreach (var item in dto.FDRPrimarySecurity.FDRPSDetails)
                                    {
                                        FDRPSDetail detail;
                                        if (item.Id != null && item.Id > 0)
                                        {
                                            detail = GenService.GetById<FDRPSDetail>((long)item.Id);
                                            if (item.Status == null)
                                                item.Status = detail.Status;
                                            item.CreateDate = detail.CreateDate;
                                            item.CreatedBy = detail.CreatedBy;
                                            item.FDRPrimarySecurityId = detail.FDRPrimarySecurityId;
                                            item.EditDate = DateTime.Now;
                                            item.EditedBy = userId;
                                            Mapper.Map(item, detail);
                                            GenService.Save(detail);
                                        }
                                        else
                                        {
                                            detail = new FDRPSDetail();
                                            item.CreateDate = DateTime.Now;
                                            item.CreatedBy = userId;
                                            item.Status = EntityStatus.Active;
                                            item.LoanApplicationId = entity.Id;
                                            item.FDRPrimarySecurityId = dto.FDRPrimarySecurity.Id;
                                            detail = Mapper.Map<FDRPSDetail>(item);
                                            GenService.Save(detail);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var fdrSecurity = Mapper.Map<FDRPrimarySecurity>(dto.FDRPrimarySecurity);
                                fdrSecurity.LoanApplicationId = entity.Id;
                                GenService.Save(fdrSecurity);
                                entity.RefId = fdrSecurity.Id;
                                if (dto.FDRPrimarySecurity.FDRPSDetails != null)
                                {
                                    foreach (var item in dto.FDRPrimarySecurity.FDRPSDetails)
                                    {
                                        FDRPSDetail detail;
                                        if (item.Id != null && item.Id > 0)
                                        {
                                            detail = GenService.GetById<FDRPSDetail>((long)item.Id);
                                            if (item.Status == null)
                                                item.Status = detail.Status;
                                            item.CreateDate = detail.CreateDate;
                                            item.CreatedBy = detail.CreatedBy;
                                            item.FDRPrimarySecurityId = fdrSecurity.Id;
                                            item.EditDate = DateTime.Now;
                                            item.EditedBy = userId;
                                            Mapper.Map(item, detail);
                                            GenService.Save(detail);
                                        }
                                        else
                                        {
                                            detail = new FDRPSDetail();
                                            item.CreateDate = DateTime.Now;
                                            item.CreatedBy = userId;
                                            item.Status = EntityStatus.Active;
                                            item.LoanApplicationId = entity.Id;
                                            item.FDRPrimarySecurityId = fdrSecurity.Id;
                                            detail = Mapper.Map<FDRPSDetail>(item);
                                            GenService.Save(detail);
                                        }
                                    }
                                }
                            }

                        }
                        #endregion 

                        #region list deletes
                        if (dto.RemovedGuarantors != null)
                        {
                            foreach (var item in dto.RemovedGuarantors)
                            {
                                var guarantor = GenService.GetById<Guarantor>(item);//entity.FactoryAddress.Where(o => o.Id == item).FirstOrDefault();//
                                if (guarantor != null)
                                {
                                    guarantor.Status = EntityStatus.Inactive;
                                    guarantor.EditDate = DateTime.Now;
                                    guarantor.EditedBy = userId;
                                }
                                GenService.Save(guarantor);
                            }
                        }
                        if (dto.RemovedWaiverRequests != null)
                        {
                            foreach (var item in dto.RemovedWaiverRequests)
                            {
                                var waiverReq = GenService.GetById<LoanAppWaiverReq>(item);//entity.FactoryAddress.Where(o => o.Id == item).FirstOrDefault();//
                                if (waiverReq != null)
                                {
                                    waiverReq.Status = EntityStatus.Inactive;
                                    waiverReq.EditDate = DateTime.Now;
                                    waiverReq.EditedBy = userId;
                                }
                                GenService.Save(waiverReq);
                            }
                        }
                        if (dto.RemovedFDRPSDetails != null)
                        {
                            foreach (var item in dto.RemovedFDRPSDetails)
                            {
                                var detail = GenService.GetById<FDRPSDetail>(item);
                                if (detail != null)
                                {
                                    detail.Status = EntityStatus.Inactive;
                                    detail.EditDate = DateTime.Now;
                                    detail.EditedBy = userId;
                                }
                                GenService.Save(detail);
                            }
                        }
                        if (dto.RemovedOtherSecurities != null)
                        {
                            foreach (var item in dto.RemovedOtherSecurities)
                            {
                                var detail = GenService.GetById<LoanOtherSecurities>(item);
                                if (detail != null)
                                {
                                    detail.Status = EntityStatus.Inactive;
                                    detail.EditDate = DateTime.Now;
                                    detail.EditedBy = userId;
                                }
                                GenService.Save(detail);
                            }
                        }

                        #endregion

                        #region populate primary security
                        if (dto.LoanPrimarySecurityType != null && dto.LoanPrimarySecurityType == LoanPrimarySecurityType.ConsumerGoodsPrimarySecurity)
                        {
                            if (dto.ConsumerGoodsPrimarySecurity.Id > 0)
                            {
                                var data = GenService.GetById<ConsumerGoodsPrimarySecurity>(dto.ConsumerGoodsPrimarySecurity.Id);
                                if (dto.ConsumerGoodsPrimarySecurity.Status == null)
                                    dto.ConsumerGoodsPrimarySecurity.Status = data.Status;
                                dto.ConsumerGoodsPrimarySecurity.CreateDate = data.CreateDate;
                                dto.ConsumerGoodsPrimarySecurity.CreatedBy = data.CreatedBy;
                                //dto.ConsumerGoodsPrimarySecurity.FDRPrimarySecurityId = data.FDRPrimarySecurityId;
                                dto.ConsumerGoodsPrimarySecurity.LoanApplicationId = entity.Id;
                                dto.ConsumerGoodsPrimarySecurity.EditDate = DateTime.Now;
                                dto.ConsumerGoodsPrimarySecurity.EditedBy = userId;
                                Mapper.Map(dto.ConsumerGoodsPrimarySecurity, data);
                                GenService.Save(data);
                                entity.RefId = data.Id;
                            }
                            else
                            {
                                var consumerSecurity = Mapper.Map<ConsumerGoodsPrimarySecurity>(dto.ConsumerGoodsPrimarySecurity);
                                consumerSecurity.LoanApplicationId = entity.Id;
                                consumerSecurity.CreateDate = DateTime.Now;
                                consumerSecurity.CreatedBy = userId;
                                consumerSecurity.Status = EntityStatus.Active;
                                GenService.Save(consumerSecurity);
                                entity.RefId = consumerSecurity.Id;
                            }

                        }
                        else if (dto.LoanPrimarySecurityType != null && dto.LoanPrimarySecurityType == LoanPrimarySecurityType.VehiclePrimarySecurity)
                        {
                            if (dto.VehiclePrimarySecurity.Id > 0)
                            {
                                var data = GenService.GetById<VehiclePrimarySecurity>(dto.VehiclePrimarySecurity.Id);
                                if (dto.VehiclePrimarySecurity.Status == null)
                                    dto.VehiclePrimarySecurity.Status = data.Status;
                                dto.VehiclePrimarySecurity.CreateDate = data.CreateDate;
                                dto.VehiclePrimarySecurity.CreatedBy = data.CreatedBy;
                                //dto.VehiclePrimarySecurity.FDRPrimarySecurityId = data.FDRPrimarySecurityId;
                                dto.VehiclePrimarySecurity.LoanApplicationId = entity.Id;
                                dto.VehiclePrimarySecurity.EditDate = DateTime.Now;
                                dto.VehiclePrimarySecurity.EditedBy = userId;
                                if (dto.VehiclePrimarySecurity.SellersAddress.IsChanged)
                                {
                                    if (dto.VehiclePrimarySecurity.SellersAddress.Id != null)
                                    {
                                        var selleraddress = GenService.GetById<Address>((long)dto.VehiclePrimarySecurity.SellersAddress.Id);
                                        dto.VehiclePrimarySecurity.SellersAddress.CreateDate = selleraddress.CreateDate;
                                        dto.VehiclePrimarySecurity.SellersAddress.CreatedBy = selleraddress.CreatedBy;
                                        dto.VehiclePrimarySecurity.SellersAddress.EditedBy = userId;
                                        dto.VehiclePrimarySecurity.SellersAddress.EditDate = DateTime.Now;
                                        dto.VehiclePrimarySecurity.Status = EntityStatus.Active;
                                        selleraddress = Mapper.Map(dto.VehiclePrimarySecurity.SellersAddress, selleraddress);
                                        GenService.Save(selleraddress);
                                        dto.VehiclePrimarySecurity.SellersAddressId = selleraddress.Id;
                                    }
                                    else if (dto.VehiclePrimarySecurity.SellersAddress != null && dto.VehiclePrimarySecurity.SellersAddress.CountryId != null && dto.VehiclePrimarySecurity.SellersAddress.CountryId > 0)
                                    {
                                        var permanentAddress = Mapper.Map<Address>(dto.VehiclePrimarySecurity.SellersAddress);
                                        permanentAddress.Status = EntityStatus.Active;
                                        permanentAddress.CreateDate = DateTime.Now;
                                        permanentAddress.CreatedBy = userId;
                                        GenService.Save(permanentAddress);
                                        dto.VehiclePrimarySecurity.SellersAddressId = permanentAddress.Id;
                                    }

                                }
                                Mapper.Map(dto.VehiclePrimarySecurity, data);
                                GenService.Save(data);
                                entity.RefId = data.Id; //todo Add address  edit
                            }
                            else
                            {
                                var vehicleSecurity = Mapper.Map<VehiclePrimarySecurity>(dto.VehiclePrimarySecurity);
                                vehicleSecurity.LoanApplicationId = entity.Id; //todo Add address  edit
                                if (dto.VehiclePrimarySecurity.SellersAddress != null)
                                {
                                    var sellerAddress = Mapper.Map<Address>(dto.VehiclePrimarySecurity.SellersAddress);
                                    if (dto.VehiclePrimarySecurity.SellersAddress.CountryId != null)
                                    {
                                        GenService.Save(sellerAddress);
                                        vehicleSecurity.SellersAddressId = sellerAddress.Id;
                                    }
                                }
                                vehicleSecurity.CreatedBy = userId;
                                vehicleSecurity.CreateDate = DateTime.Now;
                                vehicleSecurity.Status = EntityStatus.Active;
                                GenService.Save(vehicleSecurity);
                                entity.RefId = vehicleSecurity.Id;
                            }


                        }
                        //else if (dto.LoanPrimarySecurityType != null && dto.LoanPrimarySecurityType == LoanPrimarySecurityType.FDRPrimarySecurity)
                        //{
                        //    var fdrSecurity = Mapper.Map<FDRPrimarySecurity>(dto.FDRPrimarySecurity);
                        //    fdrSecurity.FDRPSDetails = Mapper.Map<List<FDRPSDetail>>(dto.FDRPrimarySecurity.FDRPSDetails);
                        //    fdrSecurity.LoanApplicationId = entity.Id;
                        //    GenService.Save(fdrSecurity);
                        //    entity.RefId = fdrSecurity.Id;
                        //}
                        else if (dto.LoanPrimarySecurityType != null && dto.LoanPrimarySecurityType == LoanPrimarySecurityType.LPPrimarySecurity)
                        {
                            if (dto.LPPrimarySecurity.Id > 0)
                            {
                                var data = GenService.GetById<LPPrimarySecurity>(dto.LPPrimarySecurity.Id);
                                if (dto.LPPrimarySecurity.Status == null)
                                    dto.LPPrimarySecurity.Status = data.Status;
                                dto.LPPrimarySecurity.CreateDate = data.CreateDate;
                                dto.LPPrimarySecurity.CreatedBy = data.CreatedBy;
                                //dto.LPPrimarySecurity.FDRPrimarySecurityId = data.FDRPrimarySecurityId;
                                dto.LPPrimarySecurity.LoanApplicationId = entity.Id;
                                dto.LPPrimarySecurity.EditDate = DateTime.Now;
                                dto.LPPrimarySecurity.EditedBy = userId;
                                if (dto.LPPrimarySecurity.PropertyAddress.IsChanged)
                                {
                                    if (dto.LPPrimarySecurity.PropertyAddress.Id != null)
                                    {
                                        var propAddress = GenService.GetById<Address>((long)dto.LPPrimarySecurity.PropertyAddress.Id);
                                        dto.LPPrimarySecurity.PropertyAddress.CreateDate = propAddress.CreateDate;
                                        dto.LPPrimarySecurity.PropertyAddress.CreatedBy = propAddress.CreatedBy;
                                        propAddress = Mapper.Map(dto.LPPrimarySecurity.PropertyAddress, propAddress);
                                        GenService.Save(propAddress);
                                        dto.LPPrimarySecurity.PropertyAddressId = propAddress.Id;
                                    }
                                    else if (dto.LPPrimarySecurity.PropertyAddress != null && dto.LPPrimarySecurity.PropertyAddress.CountryId != null && dto.LPPrimarySecurity.PropertyAddress.CountryId > 0)
                                    {
                                        var permanentAddress = Mapper.Map<Address>(dto.LPPrimarySecurity.PropertyAddress);
                                        GenService.Save(permanentAddress);
                                        dto.LPPrimarySecurity.PropertyAddressId = permanentAddress.Id;
                                    }

                                }
                                Mapper.Map(dto.LPPrimarySecurity, data);
                                GenService.Save(data);
                                entity.RefId = data.Id;
                            }
                            else
                            {
                                var lppSecurity = Mapper.Map<LPPrimarySecurity>(dto.LPPrimarySecurity);
                                lppSecurity.LoanApplicationId = entity.Id;
                                if (dto.LPPrimarySecurity.PropertyAddress != null)
                                {
                                    var propAddress = Mapper.Map<Address>(dto.LPPrimarySecurity.PropertyAddress);
                                    if (dto.LPPrimarySecurity.PropertyAddress.CountryId != null)
                                    {
                                        GenService.Save(propAddress);
                                        lppSecurity.PropertyAddressId = propAddress.Id;
                                    }
                                }
                                lppSecurity.CreateDate = DateTime.Now;
                                lppSecurity.CreatedBy = userId;
                                lppSecurity.Status = EntityStatus.Active;
                                GenService.Save(lppSecurity);
                                entity.RefId = lppSecurity.Id;
                            }

                        }
                        #endregion
                        GenService.Save(log);
                        tran.Complete();
                    }
                    catch (Exception ex)
                    {
                        tran.Dispose();
                        responce.Message = "Loan Application Operation Failed";
                        return responce;
                    }
                }
                responce.Success = true;
                responce.Id = entity.Id;
                responce.Message = "Loan Application Edited Successfully";
                return responce;
                #endregion
            }
            else
            {
                #region create new
                var application = GenService.GetById<Application>((long)dto.Application_Id);

                entity = Mapper.Map<LoanApplication>(dto);
                //entity.CIFNo = _sequencer.GetUpdatedCIFPersonalNo();
                entity.Status = EntityStatus.Active;
                entity.CreatedBy = userId;
                entity.CreateDate = DateTime.Now;
                var sellerAddress = Mapper.Map<Address>(dto.VehiclePrimarySecurity.SellersAddress);
                var propertyAddress = Mapper.Map<Address>(dto.LPPrimarySecurity.PropertyAddress);
                var dealerAddress = Mapper.Map<Address>(dto.ConsumerGoodsPrimarySecurity.DealerAddress);
                using (var tran = new TransactionScope())
                {
                    if (application.DepositApplicationId == null && application.LoanApplicationId == null)
                    {
                        try
                        {
                            if (sellerAddress.CountryId != null)
                            {
                                GenService.Save(sellerAddress);
                                dto.VehiclePrimarySecurity.SellersAddressId = sellerAddress.Id;
                            }
                            if (propertyAddress.CountryId != null)
                            {
                                GenService.Save(propertyAddress);
                                dto.LPPrimarySecurity.PropertyAddressId = propertyAddress.Id;
                            }
                            if (dealerAddress.CountryId != null)
                            {
                                GenService.Save(dealerAddress);
                                dto.ConsumerGoodsPrimarySecurity.DealerAddressId = dealerAddress.Id;
                            }
                            GenService.Save(entity);

                            #region populate primary security
                            if (dto.LoanPrimarySecurityType != null && dto.LoanPrimarySecurityType == LoanPrimarySecurityType.ConsumerGoodsPrimarySecurity)
                            {
                                var consumerSecurity = Mapper.Map<ConsumerGoodsPrimarySecurity>(dto.ConsumerGoodsPrimarySecurity);
                                consumerSecurity.LoanApplicationId = entity.Id;
                                consumerSecurity.CreateDate = DateTime.Now;
                                consumerSecurity.CreatedBy = userId;
                                consumerSecurity.Status = EntityStatus.Active;
                                GenService.Save(consumerSecurity);
                                entity.RefId = consumerSecurity.Id;
                            }
                            else if (dto.LoanPrimarySecurityType != null && dto.LoanPrimarySecurityType == LoanPrimarySecurityType.VehiclePrimarySecurity)
                            {
                                var vehicleSecurity = Mapper.Map<VehiclePrimarySecurity>(dto.VehiclePrimarySecurity);
                                vehicleSecurity.LoanApplicationId = entity.Id;
                                vehicleSecurity.CreateDate = DateTime.Now;
                                vehicleSecurity.CreatedBy = userId;
                                vehicleSecurity.Status = EntityStatus.Active;
                                GenService.Save(vehicleSecurity);
                                entity.RefId = vehicleSecurity.Id;
                            }
                            else if (dto.LoanPrimarySecurityType != null && dto.LoanPrimarySecurityType == LoanPrimarySecurityType.FDRPrimarySecurity)
                            {
                                var fdrSecurity = Mapper.Map<FDRPrimarySecurity>(dto.FDRPrimarySecurity);
                                fdrSecurity.FDRPSDetails = Mapper.Map<List<FDRPSDetail>>(dto.FDRPrimarySecurity.FDRPSDetails);
                                fdrSecurity.LoanApplicationId = entity.Id;
                                fdrSecurity.CreateDate = DateTime.Now;
                                fdrSecurity.CreatedBy = userId;
                                fdrSecurity.Status = EntityStatus.Active;
                                GenService.Save(fdrSecurity);
                                entity.RefId = fdrSecurity.Id;
                            }
                            else if (dto.LoanPrimarySecurityType != null && dto.LoanPrimarySecurityType == LoanPrimarySecurityType.LPPrimarySecurity)
                            {
                                var lppSecurity = Mapper.Map<LPPrimarySecurity>(dto.LPPrimarySecurity);
                                lppSecurity.LoanApplicationId = entity.Id;
                                lppSecurity.CreateDate = DateTime.Now;
                                lppSecurity.CreatedBy = userId;
                                lppSecurity.Status = EntityStatus.Active;
                                GenService.Save(lppSecurity);
                                entity.RefId = lppSecurity.Id;
                            }
                            #endregion

                            #region populate list

                            if (dto.Guarantors != null && dto.Guarantors.Count > 0)
                            {
                                entity.Guarantors = Mapper.Map<List<Guarantor>>(dto.Guarantors);
                                foreach (var item in entity.Guarantors)
                                {
                                    item.CreateDate = DateTime.Now;
                                    item.CreatedBy = userId;
                                    item.Status = EntityStatus.Active;
                                }
                            }

                            if (dto.LoanAppColSecurities != null && dto.LoanAppColSecurities.Count > 0)
                            {
                                entity.LoanAppColSecurities = Mapper.Map<List<LoanAppColSecurity>>(dto.LoanAppColSecurities);
                                foreach (var item in entity.LoanAppColSecurities)
                                {
                                    item.CreateDate = DateTime.Now;
                                    item.CreatedBy = userId;
                                    item.Status = EntityStatus.Active;
                                }
                            }

                            if (dto.WaiverRequests != null && dto.WaiverRequests.Count > 0)
                            {
                                entity.WaiverRequests = Mapper.Map<List<LoanAppWaiverReq>>(dto.WaiverRequests);
                                foreach (var item in entity.WaiverRequests)
                                {
                                    item.CreateDate = DateTime.Now;
                                    item.CreatedBy = userId;
                                    item.Status = EntityStatus.Active;
                                }
                            }

                            if (dto.OtherSecurities != null && dto.OtherSecurities.Count > 0)
                            {
                                entity.OtherSecurities = Mapper.Map<List<LoanOtherSecurities>>(dto.OtherSecurities);
                                foreach (var item in entity.OtherSecurities)
                                {
                                    item.CreateDate = DateTime.Now;
                                    item.CreatedBy = userId;
                                    item.Status = EntityStatus.Active;
                                }
                            }

                            #endregion

                            GenService.Save(entity);
                            var check = entity.Id;
                            log.Activity = Activity.Submit;
                            log.AppIdRef = entity.Id;
                            log.ApplicationId = (long)dto.Application_Id;
                            log.AppType = AppType.LoanApplication;
                            log.CreateDate = DateTime.Now;
                            log.CreatedBy = userId;
                            log.FromUserId = userId;
                            log.FromStage = ApplicationStage.Drafted; //// Previous-'Save'
                            log.ToStage = ApplicationStage.Drafted; //// Previous-'LoanSave'
                            log.ToUserId = parentUserId != null ? (long)parentUserId : 0;
                            log.Status = EntityStatus.Active;
                            GenService.Save(log);
                            application.LoanApplicationId = entity.Id;
                            GenService.Save(application);
                            tran.Complete();
                        }
                        catch (Exception ex)
                        {
                            tran.Dispose();
                            responce.Message = "Loan Application Save Failed";
                            return responce;
                        }
                    }
                    else
                    {
                        responce.Message = "Already an application exists. Please edit or try to add a new application.";
                        return responce;
                    }

                }

                responce.Success = true;
                responce.Id = entity.Id;
                responce.Message = "Loan Application Saved Successfully";
                return responce;

                #endregion
            }

        }
        public LoanApplicationDto LoadLoanAppByAppId(long AppId)
        {
            List<ApplicationCIFsDto> cifList = new List<ApplicationCIFsDto>();
            AddressDto prjAddress = new AddressDto();
            var app = GenService.GetById<Application>(AppId);
            var temp = app.Product.ProductSecurity.ToList();
            if (app.ProductType == ProductType.Loan && app.LoanApplicationId != null && app.LoanApplicationId > 0)
            {
                try
                {
                    var loanApp = GenService.GetById<LoanApplication>((long)app.LoanApplicationId);

                    var data = Mapper.Map<LoanApplicationDto>(loanApp);
                    data.Guarantors.RemoveAll(f => f.Status != EntityStatus.Active);
                    data.WaiverRequests.RemoveAll(f => f.Status != EntityStatus.Active);
                    data.OtherSecurities.RemoveAll(f => f.Status != EntityStatus.Active);
                    data.LoanAppColSecurities = data.LoanAppColSecurities.Select(d => { d.IsChecked = true; return d; }).ToList();

                    temp.RemoveAll(p => loanApp.LoanAppColSecurities.Select(l => l.ColSecurityId).ToList().Contains(p.Id));
                    var extraColSecurities = (from chk in temp
                                              select new LoanAppColSecurityDto
                                              {
                                                  LoanApplicationId = 0,
                                                  ColSecurityId = chk.Id,
                                                  ProductId = chk.ProductId,
                                                  ProductName = chk.Product.Name,
                                                  SecurityDescription = chk.SecurityDescription,
                                                  IsChecked = false
                                              }).ToList();
                    data.LoanAppColSecurities.AddRange(extraColSecurities);
                    cifList = Mapper.Map<List<ApplicationCIFsDto>>(app.CIFList);
                    cifList = cifList.Where(x => x.Status == EntityStatus.Active).ToList();
                    data.CIFList.AddRange(cifList);
                    data.ApplicationNo = app.ApplicationNo;
                    //data.FDRPrimarySecurity.RemoveAll(f => f.Status != EntityStatus.Active);
                    if (loanApp != null)
                    {
                        var consumer =
                            GenService.GetAll<ConsumerGoodsPrimarySecurity>().OrderByDescending(r => r.Id).FirstOrDefault(r => r.LoanApplicationId == loanApp.Id && data.LoanPrimarySecurityType == LoanPrimarySecurityType.ConsumerGoodsPrimarySecurity);
                        var vehicle =
                            GenService.GetAll<VehiclePrimarySecurity>().OrderByDescending(r => r.Id).FirstOrDefault(r => r.LoanApplicationId == loanApp.Id && data.LoanPrimarySecurityType == LoanPrimarySecurityType.VehiclePrimarySecurity && r.Status == EntityStatus.Active);
                        var fdr =
                            GenService.GetAll<FDRPrimarySecurity>().OrderByDescending(r => r.Id).FirstOrDefault(r => r.LoanApplicationId == loanApp.Id && data.LoanPrimarySecurityType == LoanPrimarySecurityType.FDRPrimarySecurity);
                        var lpSecurity =
                            GenService.GetAll<LPPrimarySecurity>().OrderByDescending(r => r.Id).FirstOrDefault(r => r.LoanApplicationId == loanApp.Id && data.LoanPrimarySecurityType == LoanPrimarySecurityType.LPPrimarySecurity);
                        if (consumer != null)
                        {
                            data.ConsumerGoodsPrimarySecurity = Mapper.Map<ConsumerGoodsPrimarySecurityDto>(consumer);
                            if (data.ConsumerGoodsPrimarySecurity.ShowRoomId > 0)
                            {
                                data.ConsumerGoodsPrimarySecurity.VendorType = VendorType.Showroom;
                            }
                            else
                            {
                                data.ConsumerGoodsPrimarySecurity.VendorType = VendorType.Individual;
                            }
                        }
                        else if (vehicle != null)
                        {
                            data.VehiclePrimarySecurity = Mapper.Map<VehiclePrimarySecurityDto>(vehicle);
                        }
                        else if (fdr != null)
                        {
                            data.FDRPrimarySecurity = Mapper.Map<FDRPrimarySecurityDto>(fdr);
                            data.FDRPrimarySecurity.FDRPSDetails.RemoveAll(f => f.Status != EntityStatus.Active);
                        }
                        else if (lpSecurity != null)
                        {
                            if (lpSecurity.Project != null && lpSecurity.Project.ProjectAddress != null)
                            {
                                prjAddress = Mapper.Map<AddressDto>(lpSecurity.Project.ProjectAddress);
                                data.LPPrimarySecurity = Mapper.Map<LPPrimarySecurityDto>(lpSecurity);
                                data.LPPrimarySecurity.ProjectAddress = prjAddress;
                            }
                        }
                        return data;
                    }

                }
                catch (Exception ex)
                {
                    //
                }

            }

            return null;
        }
        public long? GetLoanApplicationbyAppId(long AppId)
        {
            if (AppId > 0)
            {
                var loanAppId = GenService.GetById<Application>(AppId).LoanApplicationId;
                //var loanAppId = GenService.GetAll<Application>().Where(e => e.Id == AppId && e.Status == EntityStatus.Active).FirstOrDefault().LoanApplicationId;
                if (loanAppId > 0)
                    return loanAppId;
            }
            return 0;
        }

        public long? GetDepositApplicationbyAppId(long AppId)
        {
            if (AppId > 0)
            {
                var depositAppId = GenService.GetById<Application>(AppId).DepositApplicationId;
                if (depositAppId > 0)
                    return depositAppId;
            }
            return 0;
        }

        public long? GetGuardianByDepoAppId(long depoAppId)
        {
            if (depoAppId > 0)
            {
                var guardianCifId = GenService.GetById<DepositApplication>(depoAppId).GuiardianCifId;
                if (guardianCifId > 0)
                    return guardianCifId;
            }
            return 0;
        }

        public List<GuarantorDto> LoadGuarantorsByloanAppId(long loanAppId)
        {
            if (loanAppId > 0)
            {

                var guarantors = GenService.GetAll<Guarantor>().Where(g => g.LoanApplicationId == loanAppId).ToList();

                return Mapper.Map<List<GuarantorDto>>(guarantors);
            }
            return null;
        }

        public List<DepositNomineeDto> LoadNomineesBydepositAppId(long depositAppId)
        {
            if (depositAppId > 0)
            {

                var nominees = GenService.GetAll<DepositNominee>().Where(g => g.DepositApplicationId == depositAppId).ToList();

                return Mapper.Map<List<DepositNomineeDto>>(nominees);
            }
            return null;
        }
        #endregion
        public List<ApplicationDto> GetAllApplications()
        {
            List<ApplicationDto> result = new List<ApplicationDto>();
            try
            {
                var settings = GenService.GetAll<Application>();
                result = (from setting in settings
                          select new ApplicationDto
                          {
                              Id = setting.Id,
                              ApplicationNo = setting.ApplicationNo,
                              AccountTitle = setting.AccountTitle

                          }).ToList();
                //return result;
            }
            catch (Exception ex)
            {
                //
            }
            return result;
        }
        public List<IPDCBankAccountsDto> GetIPDCBankAccount()
        {
            var orgs = GenService.GetAll<IPDCBankAccounts>();
            return Mapper.Map<List<IPDCBankAccountsDto>>(orgs.ToList());
        }
        public List<LoanAppColSecurityDto> GetLoanAppColSecurities(long? appId)
        {
            long prodId = GenService.GetById<Application>((long)appId).ProductId;
            var securityList = GenService.GetAll<ProductSecurity>().Where(r => r.ProductId == prodId);
            var data = (from chk in securityList
                        select new LoanAppColSecurityDto
                        {
                            LoanApplicationId = 0,
                            ColSecurityId = chk.Id,
                            ProductId = chk.ProductId,
                            ProductName = chk.Product.Name,
                            SecurityDescription = chk.SecurityDescription,
                            IsChecked = false
                        }).ToList();
            return data;
        }
        public List<IPDCBankAccountsDto> GetAllIpdcBankAccntiWithName()
        {
            List<IPDCBankAccountsDto> cifAll = GenService.GetAll<IPDCBankAccounts>().Select(r => new IPDCBankAccountsDto
            {
                Id = r.Id,
                BankName = r.AccountNo + "-" + r.BankName
            }).ToList();
            return cifAll;
        }
        public ApplicationDto LoadApplicationByAppId(long appId)
        {
            var application = GenService.GetById<Application>(appId);
            var data = Mapper.Map<ApplicationDto>(application);
            if (data.DocChecklist != null)
            {
                data.DocChecklist = data.DocChecklist.Select(d =>
                {
                    d.IsChecked = true;
                    return d;
                }).ToList();
            }
            var temp = application.Product.DocumentSetups.ToList();
            temp.RemoveAll(p => data.DocChecklist.Select(l => l.ProductDocId).ToList().Contains(p.Id));
            var extraDocCheckList = (from chk in temp
                                     select new AppDocChecklistDto
                                     {
                                         ProductDocId = chk.Id,
                                         ProductId = chk.ProductId,
                                         DocName = chk.Document.Name,
                                         IsChecked = false
                                     }).ToList();


            data.DocChecklist.AddRange(extraDocCheckList);
            return data;
        }
        public IPagedList<ApplicationDto> GetApplicationPagedList(int pageSize, int pageCount, string searchString)
        {
            var allApp = (from app in GenService.GetAll<Application>().Where(s => s.Status == EntityStatus.Active)
                          join empDegMap in GenService.GetAll<EmployeeDesignationMapping>().Where(e => e.Status == EntityStatus.Active) on app.RMId equals empDegMap.EmployeeId into rmList
                          from rm in rmList.DefaultIfEmpty()
                          select new ApplicationDto()
                          {
                              Id = app.Id,
                              ApplicationNo = app.ApplicationNo,
                              AccountTitle = app.AccountTitle,
                              ApplicationType = app.ApplicationType,
                              ApplicationDate = app.ApplicationDate,
                              ApplicationDateText = app.ApplicationDate.ToString(),
                              CustomerTypeName = app.CustomerType.ToString(),
                              ProductTypeName = app.ProductType.ToString(),
                              ProductName = app.Product.Name,
                              BranchId = app.BranchId,
                              BranchName = app.BranchId != null ? app.BranchOffice.Name : "",
                              RMId = app.RMId,
                              RMName = rm != null ? (rm.Employee.Person.FirstName != null ? rm.Employee.Person.FirstName : "") + (rm.Employee.Person.LastName != null ? rm.Employee.Person.LastName : "") : "",
                              ApplicationStageName = app.ApplicationStage.ToString()
                          });
            //var temp = allApp.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
            //return temp;
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                allApp = allApp
                    .Where(s => s.ApplicationNo.ToLower().Contains(searchString) ||
                                s.ProductName.ToLower().Contains(searchString) ||
                                s.AccountTitle.ToLower().Contains(searchString));
            }
            var temp = allApp.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }//GetApplicationPagedListByUser
        public IPagedList<ApplicationDto> GetApplicationPagedListByUser(int pageSize, int pageCount, string searchString, long userId)
        {
            var allApp = (from app in GenService.GetAll<Application>().Where(s => s.Status == EntityStatus.Active)
                          join empDegMap in GenService.GetAll<EmployeeDesignationMapping>().Where(e => e.Status == EntityStatus.Active) on app.RMId equals empDegMap.EmployeeId into rmList
                          from rm in rmList.DefaultIfEmpty()
                          select new ApplicationDto()
                          {
                              Id = app.Id,
                              ApplicationNo = app.ApplicationNo,
                              AccountTitle = app.AccountTitle,
                              ApplicationType = app.ApplicationType,
                              ApplicationDate = app.ApplicationDate,
                              ApplicationDateText = app.ApplicationDate.ToString(),
                              CustomerTypeName = app.CustomerType.ToString(),
                              ProductTypeName = app.ProductType.ToString(),
                              ProductName = app.Product.Name,
                              BranchId = app.BranchId,
                              BranchName = app.BranchId != null ? app.BranchOffice.Name : "",
                              RMId = app.RMId,
                              RMName = rm != null ? (rm.Employee.Person.FirstName != null ? rm.Employee.Person.FirstName : "") + (rm.Employee.Person.LastName != null ? rm.Employee.Person.LastName : "") : "",
                              ApplicationStageName = app.ApplicationStage.ToString(),
                              CreatedBy = app.CreatedBy
                          });
            //var temp = allApp.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
            //return temp;
            if (userId > 0)
            {
                allApp = allApp.Where(r => r.CreatedBy == userId);
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                allApp = allApp
                    .Where(s => s.ApplicationNo.ToLower().Contains(searchString) ||
                                s.ProductName.ToLower().Contains(searchString) ||
                                s.AccountTitle.ToLower().Contains(searchString));
            }
            var temp = allApp.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }

        public List<ApplicationDto> GetApplicationListApi(int pageSize, int pageCount, string searchString, List<ApplicationStage> appStage, ProductType productType, long userId)
        {
            long employeeId = _user.GetEmployeeIdByUserId(userId);
            List<long> employeeIds = new List<long>();
            List<long> userIds = new List<long>();
            userIds.Add(userId);
            employeeIds = _employee.GetEmployeeWiseBM(employeeId).Select(e => (long)e.EmployeeId).ToList();
            employeeIds.Add(employeeId);

            var allApp = (from app in GenService.GetAll<Application>()
                    .Where(s => s.Status == EntityStatus.Active && appStage.Contains((ApplicationStage)s.ApplicationStage) && s.ProductType == productType && employeeIds.Contains((long)s.RMId))
                          join cif in GenService.GetAll<ApplicationCIFs>().Where(c => c.Status == EntityStatus.Active && c.ApplicantRole == ApplicantRole.Primary)
                          on app.Id equals cif.ApplicationId into Ciflist
                          from appcifs in Ciflist.DefaultIfEmpty()
                          select new ApplicationDto
                          {
                              Id = app.Id,
                              ApplicationNo = app.ApplicationNo,
                              AccountTitle = app.AccountTitle,
                              ApplicantName = appcifs.CIF_Personal.Title + " " + appcifs.CIF_Personal.Name,
                              MaturityAmount = app.LoanApplicationId != null ? app.LoanApplication.LoanAmountApplied :
                                     app.DepositApplicationId != null ? app.DepositApplication.TotalDepositAmount : 0,
                              Rate = app.LoanApplicationId != null ? app.LoanApplication.Rate :
                                     app.DepositApplicationId != null ? app.DepositApplication.OfferRate : 0,
                              Term = app.LoanApplicationId != null && app.LoanApplication.Term != null ? (int)app.LoanApplication.Term :
                                     app.DepositApplicationId != null && app.DepositApplication.Term != null ? (int)app.DepositApplication.Term : 0,
                              ApplicationStage = app.ApplicationStage,
                              ApplicationStageName = app.ApplicationStage.ToString(),
                              CurrentHolding = app.CurrentHolding,
                              CurrentHoldingName = "",
                              CreateDate = app.CreateDate

                          }).OrderByDescending(m => m.Id).Distinct();
            var applist = allApp.OrderByDescending(r => r.Id)
                .Skip((pageCount - 1) * pageSize)
                .Take(pageSize).ToList();

            foreach (var app in applist.Where(x => x.CurrentHolding != null))
            {
                long empId = _user.GetEmployeeIdByUserId((long)app.CurrentHolding);
                var designation = _employee.GetDesignationByEmployeeId((long)empId);//_designation.GetEmployeeWithDesignation(empId);
                var person = GenService.GetById<Employee>(empId);
                app.CurrentHoldingName = person.Person.FirstName + " " + person.Person.LastName + " - " + designation;
            }

            return applist;

        }

        public IPagedList<ApplicationDto> GetApplicationListForCancel(int pageSize, int pageCount, string searchString)
        {
            var allApp = GenService.GetAll<Application>().Where(s => s.Status == EntityStatus.Active).Select(s => new ApplicationDto()
            {
                Id = s.Id,
                ApplicationNo = s.ApplicationNo,
                ApplicationType = s.ApplicationType,
                ApplicationDateText = s.ApplicationDate.ToString(),
                CustomerTypeName = s.CustomerType.ToString(),
                ProductTypeName = s.ProductType.ToString(),
                ProductName = s.Product.Name,
                //Status = s.Status == EntityStatus.Active?1:0
            });
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                allApp = allApp.Where(s => s.ApplicationNo.ToLower().Contains(searchString) || s.ProductName.ToLower().Contains(searchString));
            }
            var temp = allApp.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }
        public ApplicationDetailDto LoadApplicationByAppIdForCRM(long appId, long userId)
        {
            List<ApplicationCIFsDto> cif = new List<ApplicationCIFsDto>();
            List<CIF_Org_OwnersDto> owners = new List<CIF_Org_OwnersDto>();

            try
            {
                var app = GenService.GetById<Application>(appId);
                if (app != null)
                {
                    var data = Mapper.Map<ApplicationDetailDto>(app);
                    data.LoanApplication = Mapper.Map<LoanApplicationDto>(app.LoanApplication);
                    if (data.LoanApplication != null)
                    {
                        if (data.LoanApplication.Guarantors != null)
                        {
                            data.LoanApplication.Guarantors = data.LoanApplication.Guarantors.Where(g => g.Status == EntityStatus.Active).ToList();
                            data.LoanApplication.Guarantors.ForEach(z => z.ApplicationId = appId);
                        }
                    }
                    foreach (var applicationCiFse in app.CIFList.Where(x => x.Status == EntityStatus.Active))
                    {
                        var refce = Mapper.Map<List<CIF_ReferenceDto>>(applicationCiFse.CIF_Personal.References.Where(r => r.Status == EntityStatus.Active)).ToList();
                        data.References.AddRange(refce);

                        if (applicationCiFse.CIF_Organizational != null && applicationCiFse.CIF_Organizational.Owners != null)
                        {
                            owners.AddRange(Mapper.Map<List<CIF_Org_OwnersDto>>(applicationCiFse.CIF_Organizational.Owners.Where(o => o.Status == EntityStatus.Active)));
                        }
                    }
                    owners.ForEach(z => z.ApplicationId = appId);
                    foreach (var applicationCiFsDto in data.CIFList)
                    {
                        var test = GenService.GetAll<CIF_IncomeStatement>().Where(r => r.CIF_PersonalId == applicationCiFsDto.CIF_PersonalId && r.Status == EntityStatus.Active).OrderByDescending(r => r.Id).FirstOrDefault();
                        if (test != null)
                        {
                            applicationCiFsDto.MonthlyIncome = (test.MonthlyIncomeTotalDeclared != null ? (decimal)test.MonthlyIncomeTotalDeclared : 0);
                        }
                        //applicationCiFsDto.ProfessionName = 
                        cif.Add(applicationCiFsDto);
                    }
                    if (data.LoanApplication != null)
                    {
                        var consumer =
                            GenService.GetAll<ConsumerGoodsPrimarySecurity>().Where(C => C.Status == EntityStatus.Active).OrderByDescending(C => C.Id).FirstOrDefault(r => r.LoanApplicationId == data.LoanApplication.Id && data.LoanApplication.LoanPrimarySecurityType == LoanPrimarySecurityType.ConsumerGoodsPrimarySecurity);
                        //var vehicle =
                        //    GenService.GetAll<VehiclePrimarySecurity>().Where(C => C.Status == EntityStatus.Active).OrderByDescending(C => C.Id).OrderByDescending(C => C.Id).FirstOrDefault(r => r.LoanApplicationId == data.LoanApplication.Id && data.LoanApplication.LoanPrimarySecurityType == LoanPrimarySecurityType.VehiclePrimarySecurity);
                        var vehicle =
                            GenService.GetAll<VehiclePrimarySecurity>().FirstOrDefault(r => r.LoanApplicationId == data.LoanApplication.Id && r.LoanApplication.LoanPrimarySecurityType == LoanPrimarySecurityType.VehiclePrimarySecurity && r.Status == EntityStatus.Active);
                        var fdr =
                            GenService.GetAll<FDRPrimarySecurity>().Where(C => C.Status == EntityStatus.Active).OrderByDescending(C => C.Id).OrderByDescending(C => C.Id).FirstOrDefault(r => r.LoanApplicationId == data.LoanApplication.Id && data.LoanApplication.LoanPrimarySecurityType == LoanPrimarySecurityType.FDRPrimarySecurity);
                        var lpSecurity =
                            GenService.GetAll<LPPrimarySecurity>().Where(C => C.Status == EntityStatus.Active).OrderByDescending(C => C.Id).OrderByDescending(C => C.Id).FirstOrDefault(r => r.LoanApplicationId == data.LoanApplication.Id && data.LoanApplication.LoanPrimarySecurityType == LoanPrimarySecurityType.LPPrimarySecurity);
                        if (consumer != null)
                        {
                            data.LoanApplication.ConsumerGoodsPrimarySecurity = Mapper.Map<ConsumerGoodsPrimarySecurityDto>(consumer);
                        }
                        else if (vehicle != null)
                        {
                            data.LoanApplication.VehiclePrimarySecurity = Mapper.Map<VehiclePrimarySecurityDto>(vehicle);
                            var verification = GenService.GetAll<VehiclePrimarySecurityValuation>()
                                .Where(t => t.Status == EntityStatus.Active && t.VerificationState == VerificationState.Verified && t.VehiclePrimarySecurityId == vehicle.Id)
                                .OrderByDescending(r => r.Id); //.Select(r => r.Id);
                            if (verification.Any())
                            {
                                data.LoanApplication.VehiclePrimarySecurity.VerificationId = verification.FirstOrDefault().VerificationState;
                            }
                            else
                            {
                                data.LoanApplication.VehiclePrimarySecurity.VerificationId = VerificationState.Pending;
                            }
                        }
                        else if (fdr != null)
                        {
                            data.LoanApplication.FDRPrimarySecurity = Mapper.Map<FDRPrimarySecurityDto>(fdr);
                            data.LoanApplication.FDRPrimarySecurity.FDRPSDetails.RemoveAll(f => f.Status != EntityStatus.Active);
                        }
                        else if (lpSecurity != null)
                        {
                            data.LoanApplication.LPPrimarySecurity = Mapper.Map<LPPrimarySecurityDto>(lpSecurity);
                            if (lpSecurity.Valuations.Count > 0)
                            {
                                data.LoanApplication.LPPrimarySecurity.Valuations = data.LoanApplication.LPPrimarySecurity.Valuations.Where(s => s.Status == EntityStatus.Active).ToList();
                                data.LoanApplication.LPPrimarySecurity.Valuations.ForEach(s => s.Remarks = data.LoanApplication.Purpose != null ? data.LoanApplication.Purpose : "");
                                if (data.LoanApplication.LPPrimarySecurity.Valuations != null)
                                {
                                    data.LoanApplication.LPPrimarySecurity.Valuations.ForEach(c => { c.ProjectId = lpSecurity.ProjectId != null ? (long)lpSecurity.ProjectId : 0; });
                                    if (lpSecurity.ProjectId != null)
                                    {
                                        var legaldoc = GenService.GetAll<LegalDocumentVerification>().Where(r => r.ProjectId == lpSecurity.ProjectId && r.Status == EntityStatus.Active).OrderByDescending(l => l.Id).FirstOrDefault();
                                        if (legaldoc != null)
                                        {
                                            data.LoanApplication.LPPrimarySecurity.Valuations.ForEach(c => { c.LegalDocId = legaldoc.Id != null ? (long)legaldoc.Id : 0; });
                                        }
                                        var technical = GenService.GetAll<ProjectTechnicalVerification>().Where(r => r.ProjectId == lpSecurity.ProjectId && r.Status == EntityStatus.Active).OrderByDescending(l => l.Id).FirstOrDefault();
                                        if (technical != null)
                                        {
                                            data.LoanApplication.LPPrimarySecurity.Valuations.ForEach(c => { c.ProjectTechnicalId = technical.Id != null ? (long)technical.Id : 0; });
                                        }
                                        var legal = GenService.GetAll<ProjectLegalVerification>().Where(r => r.ProjectId == lpSecurity.ProjectId && r.Status == EntityStatus.Active).OrderByDescending(l => l.Id).FirstOrDefault();
                                        if (legal != null)
                                        {
                                            data.LoanApplication.LPPrimarySecurity.Valuations.ForEach(c => { c.ProjectLegalId = legal.Id != null ? (long)legal.Id : 0; });
                                        }
                                    }

                                }
                            }
                        }
                        //return data;
                    }
                    data.CIFList = cif;
                    var res = GenService.GetAll<Proposal>().Where(i => i.ApplicationId == appId && i.Status == EntityStatus.Active).OrderByDescending(r => r.Id).FirstOrDefault();
                    if (res != null)
                        data.ProposalId = res.Id;
                    data.OwnerList = owners;
                    data.IsSelfSubmitted = app.CurrentHolding != null ? app.CurrentHolding == userId ? true : false : false;
                    if (data.ProposalId != null || data.ProposalId > 0)
                    {
                        var firstOrDefault = GenService.GetAll<OfferLetter>()
                            .Where(t => t.ProposalId == data.ProposalId).OrderByDescending(u => u.Id).FirstOrDefault();
                        if (firstOrDefault != null)
                            data.OfferLetterId = firstOrDefault.Id;
                    }
                    var disbursment = GenService.GetAll<DisbursementMemo>()
                           .Where(t => t.ApplicationId == appId).OrderByDescending(u => u.Id).FirstOrDefault();
                    if (disbursment != null)
                        data.DmId = disbursment.Id;

                    // For Operation Approval Loan Application DCL Report Print

                    var docCeckList = GenService.GetAll<DocumentCheckList>().FirstOrDefault(d => d.ApplicationId == appId && d.Status == EntityStatus.Active);
                    if (docCeckList != null)
                        data.DclId = docCeckList.Id;
                    // For Operation Approval Loan Application DCL Report Print

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public IPagedList<ApplicationDto> GetCRMApplicationsPagedList(int pageSize, int pageCount, string searchString)
        {
            //var userMapping = _user.GetUserEmployeeMapping();
            var check = GenService.GetAll<Application>().Where(s => (s.ApplicationStage == ApplicationStage.SentToCRM ||
                              s.ApplicationStage == ApplicationStage.UnderProcessAtCRM) &&
                             s.Status == EntityStatus.Active && s.Product.FacilityType != ProposalFacilityType.RLS);
            var allApp = (from app in GenService.GetAll<Application>().Where(s => (s.ApplicationStage == ApplicationStage.SentToCRM || s.ApplicationStage == ApplicationStage.UnderProcessAtCRM) && s.Status == EntityStatus.Active && s.Product.FacilityType != ProposalFacilityType.RLS)
                          join emp in GenService.GetAll<Employee>().Where(e => e.Status == EntityStatus.Active) on app.CurrentHoldingEmpId equals emp.Id into empList
                          from e in empList.DefaultIfEmpty()
                          select new ApplicationDto() //// Previous-'SubmitedToCRM'
                          {
                              Id = app.Id,
                              ApplicationNo = app.ApplicationNo,
                              AccountTitle = app.AccountTitle,
                              ApplicationType = app.ApplicationType,
                              ApplicationTypeName = app.ApplicationType != null ? app.ApplicationType.ToString() : "",
                              ApplicationDate = app.ApplicationDate,
                              ApplicationDateText = app.ApplicationDate.ToString(),
                              CustomerTypeName = app.CustomerType.ToString(),
                              ProductTypeName = app.ProductType.ToString(),
                              ProductName = app.Product.Name,
                              CurrentHolding = app.CurrentHolding,
                              CurrentHoldingName = e != null && e.PersonId != null ? e.Person.FirstName + " " + e.Person.LastName : ""
                          }).ToList();
            foreach (var applicationDto in allApp)
            {
                var app = GenService.GetById<Application>((long)applicationDto.Id);
                if (app.CIFList != null)
                {
                    applicationDto.ApplicantName = app.CIFList.Where(r => r.ApplicantRole == ApplicantRole.Primary).FirstOrDefault().CIF_Personal != null ?
                        app.CIFList.Where(r => r.ApplicantRole == ApplicantRole.Primary).FirstOrDefault().CIF_Personal.Name : "";
                }
            }
            if (!string.IsNullOrEmpty(searchString))
                allApp = allApp.Where(a => a.ApplicationNo.ToLower().Contains(searchString.ToLower())).ToList();
            var temp = allApp.OrderBy(r => r.CurrentHolding).ThenByDescending(r => r.Id).ToPagedList(pageCount, pageSize);

            return temp;
        }
        public IPagedList<ApplicationDto> GetCRMApplicationsOfSODPagedList(int pageSize, int pageCount, string searchString)
        {
            var allApp = GenService.GetAll<Application>().Where(s => (s.ApplicationStage == ApplicationStage.SentToCRM || s.ApplicationStage == ApplicationStage.UnderProcessAtCRM) && s.Status == EntityStatus.Active && s.Product.FacilityType == ProposalFacilityType.RLS).Select(s => new ApplicationDto() //// Previous-'SubmitedToCRM'
            {
                Id = s.Id,
                ApplicationNo = s.ApplicationNo,
                AccountTitle = s.AccountTitle,
                ApplicationType = s.ApplicationType,
                ApplicationDate = s.ApplicationDate,
                ApplicationDateText = s.ApplicationDate.ToString(),
                CustomerTypeName = s.CustomerType.ToString(),
                ProductTypeName = s.ProductType.ToString(),
                ProductName = s.Product.Name,
                CurrentHolding = s.CurrentHolding
            });
            if (!string.IsNullOrEmpty(searchString))
                allApp = allApp.Where(a => a.ApplicationNo.ToLower().Contains(searchString.ToLower()));
            var temp = allApp.OrderBy(r => r.CurrentHolding).ThenByDescending(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }
        public IPagedList<ApplicationDto> CreditAnalystApplications(int pageSize, int pageCount, string searchString, long UserId)
        {
            var allApp = GenService.GetAll<Application>().Where(s => s.ApplicationStage == ApplicationStage.UnderProcessAtCRM && s.CurrentHolding == UserId && s.Status == EntityStatus.Active).Select(s => new ApplicationDto() // Previous-'SubmitedToCRM'
            {
                Id = s.Id,
                ApplicationNo = s.ApplicationNo,
                ApplicationType = s.ApplicationType,
                AccountTitle = s.AccountTitle,
                ApplicationTypeName = s.ApplicationType.ToString(),
                ApplicationDate = s.ApplicationDate,
                ApplicationDateText = s.ApplicationDate.ToString(),
                CustomerTypeName = s.CustomerType.ToString(),
                ProductTypeName = s.ProductType.ToString(),
                ProductName = s.Product.Name,
                ApplicantName = null,
                ProposalNo = ""
                //CIFList = Mapper.Map<List<ApplicationCIFsDto>>(s.CIFList)
            }).ToList();
            //allApp.ForEach(r=>r.ApplicantName = r.CIFList.Where(l=> l.ApplicantRole == ApplicantRole.Primary).FirstOrDefault().ApplicantName);
            foreach (var applicationDto in allApp)
            {
                var app = GenService.GetById<Application>((long)applicationDto.Id);
                if (app.CIFList != null)
                {
                    applicationDto.ApplicantName = app.CIFList.Where(r => r.ApplicantRole == ApplicantRole.Primary).FirstOrDefault().CIF_Personal != null ?
                        app.CIFList.Where(r => r.ApplicantRole == ApplicantRole.Primary).FirstOrDefault().CIF_Personal.Name : "";
                    var proposal = GenService.GetAll<Proposal>().Where(r => r.ApplicationId == applicationDto.Id && r.Status == EntityStatus.Active).OrderByDescending(r => r.Id).FirstOrDefault();
                    if (proposal != null)
                    {
                        applicationDto.ProposalNo = proposal.CreditMemoNo;
                    }

                }
            }
            if (!string.IsNullOrEmpty(searchString))
                allApp = allApp.Where(a => a.ApplicationNo.ToLower().Contains(searchString.ToLower()) || a.ApplicantName.ToLower().Contains(searchString.ToLower())).ToList();
            var temp = allApp.OrderByDescending(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }
        public IPagedList<ApplicationDto> GetApplicationApprovalTLPagedList(int pageSize, int pageCount, string searchString, long UserId)
        {
            var allApp = GenService.GetAll<Application>().Where(s => (s.ApplicationStage == ApplicationStage.SentToBM || s.ApplicationStage == ApplicationStage.SentToTL) && s.CurrentHolding == UserId && s.Status == EntityStatus.Active).Select(s => new ApplicationDto() // Previous-'SubmitedToBm'
            {
                Id = s.Id,
                AccountTitle = s.AccountTitle,
                ApplicationNo = s.ApplicationNo,
                ApplicationType = s.ApplicationType,
                ApplicationTypeName = s.ApplicationType.ToString(),
                ApplicationDate = s.ApplicationDate,
                ApplicationDateText = s.ApplicationDate.ToString(),
                CustomerTypeName = s.CustomerType.ToString(),
                ProductTypeName = s.ProductType.ToString(),
                ProductName = s.Product.Name
            });
            if (!string.IsNullOrEmpty(searchString))
                allApp = allApp.Where(a => a.ApplicationNo.ToLower().Contains(searchString.ToLower()));
            var temp = allApp.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }
        public List<ApplicationDto> GetApplicationApprovalListApi(int pageSize, int pageCount, string searchString, long UserId)
        {
            var allApp = from s in GenService.GetAll<Application>().Where(s => (s.ApplicationStage == ApplicationStage.SentToBM || s.ApplicationStage == ApplicationStage.SentToTL) && s.CurrentHolding == UserId && s.Status == EntityStatus.Active)
                         join cif in GenService.GetAll<ApplicationCIFs>().Where(c => c.Status == EntityStatus.Active && c.ApplicantRole == ApplicantRole.Primary)
                            on s.Id equals cif.ApplicationId into Ciflist
                         from appcifs in Ciflist.DefaultIfEmpty()
                         select new ApplicationDto()
                         {
                             Id = s.Id,
                             ApplicationNo = s.ApplicationNo,
                             AccountTitle = s.AccountTitle,
                             ApplicantName = appcifs.CIF_Personal.Title + " " + appcifs.CIF_Personal.Name,
                             MaturityAmount = s.LoanApplicationId != null ? s.LoanApplication.LoanAmountApplied :
                                     s.DepositApplicationId != null ? s.DepositApplication.TotalDepositAmount : 0,
                             Rate = s.LoanApplicationId != null ? s.LoanApplication.Rate :
                                     s.DepositApplicationId != null ? s.DepositApplication.OfferRate : 0,
                             Term = s.LoanApplicationId != null && s.LoanApplication.Term != null ? (int)s.LoanApplication.Term :
                                     s.DepositApplicationId != null && s.DepositApplication.Term != null ? (int)s.DepositApplication.Term : 0,
                             ApplicationStage = s.ApplicationStage,
                             ApplicationStageName = s.ApplicationStage.ToString(),
                             CurrentHolding = s.CurrentHolding,
                             CurrentHoldingName = "",
                             CreateDate = s.CreateDate
                         };
            searchString = searchString.ToLower();
            if (!string.IsNullOrEmpty(searchString))
                allApp = allApp.Where(a => a.ApplicationNo.ToLower().Contains(searchString) ||
                                           a.AccountTitle.ToLower().Contains(searchString));
            var temp = allApp.OrderByDescending(r => r.Id)
                .Skip((pageCount - 1) * pageSize)
                .Take(pageSize).ToList();
            return temp;
        }
        public int GetPrimaryApplicantAge(long AppId)
        {
            var app = GenService.GetById<Application>(AppId);
            if (app != null)
            {
                var primaryApplicant = app.CIFList.Where(c => c.ApplicantRole == ApplicantRole.Primary).FirstOrDefault();
                if (primaryApplicant != null)
                {
                    if (primaryApplicant.CIF_Personal.DateOfBirth != null)
                    {
                        var age = (DateTime.Now.Year - ((DateTime)primaryApplicant.CIF_Personal.DateOfBirth).Year);
                        return age;
                    }
                }
            }
            return 0;
        }
        public int GetApplicantYoungestAge(long AppId)
        {
            var app = GenService.GetById<Application>(AppId);
            if (app != null)
            {
                var applicantAges = app.CIFList.Where(c => c.CIF_Personal != null && c.CIF_Personal.DateOfBirth != null).Select(c => c.CIF_Personal.DateOfBirth).OrderByDescending(c => c.Value).FirstOrDefault();
                //var test = app.CIFList.Where(c => c.CIF_Personal.DateOfBirth != null).OrderByDescending(c => c.CIF_Personal.DateOfBirth).ToList();
                if (applicantAges != null)
                {
                    var age = (DateTime.Now.Year - ((DateTime)applicantAges).Year);
                    return age;

                }
            }
            return 0;
        }
        // Operations Module
        public IPagedList<ApplicationDto> GetOperationDepositApplications(int pageSize, int pageCount, string searchString, long UserId)
        {
            var allApp = GenService.GetAll<Application>().Where(s => s.ApplicationStage == ApplicationStage.UnderProcessAtOperations && s.CurrentHolding == UserId && /// Previous-'SubmitedtoOperations'
                                                               (s.DepositApplicationId != null && s.DepositApplicationId > 0) && s.Status == EntityStatus.Active).Select(s => new ApplicationDto()
                                                               {
                                                                   Id = s.Id,
                                                                   ApplicationNo = s.ApplicationNo,
                                                                   AccountTitle = s.AccountTitle,
                                                                   CustomerType = s.CustomerType,
                                                                   CustomerTypeName = s.CustomerType.ToString(),
                                                                   ProductName = s.Product.Name,
                                                                   MaturityAmount = s.Product.DepositType == DepositType.Recurring ? s.DepositApplication.MaturityAmount : s.DepositApplication.TotalDepositAmount,
                                                                   ApplicationType = s.ApplicationType,
                                                                   ApplicationDate = s.ApplicationDate,
                                                                   ApplicationDateText = s.ApplicationDate.ToString(),
                                                                   HardCopyReceived = s.HardCopyReceived,
                                                                   HardCopyReceiveDate = s.HardCopyReceiveDate,
                                                                   HardCopyReceiveDateText = s.HardCopyReceiveDate.ToString()
                                                               });
            if (!string.IsNullOrEmpty(searchString))
                allApp = allApp.Where(a => a.ApplicationNo.ToLower().Contains(searchString.ToLower()));
            var temp = allApp.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }
        public IPagedList<ApplicationDto> GetCADepositApplications(int pageSize, int pageCount, string searchString, long UserId)
        {
            var allApp = GenService.GetAll<Application>().Where(s => s.ApplicationStage == ApplicationStage.SentToOperations && // Previous-'SubmitedtoOperations'
                                                               (s.DepositApplicationId != null && s.DepositApplicationId > 0) && s.Status == EntityStatus.Active).Select(s => new ApplicationDto()
                                                               {
                                                                   Id = s.Id,
                                                                   ApplicationNo = s.ApplicationNo,
                                                                   AccountTitle = s.AccountTitle,
                                                                   CustomerType = s.CustomerType,
                                                                   CustomerTypeName = s.CustomerType.ToString(),
                                                                   ProductName = s.Product.Name,
                                                                   MaturityAmount = s.Product.DepositType == DepositType.Recurring ? s.DepositApplication.MaturityAmount : s.DepositApplication.TotalDepositAmount,
                                                                   ApplicationType = s.ApplicationType,
                                                                   ApplicationDate = s.ApplicationDate,
                                                                   CurrentHolding = s.CurrentHolding,
                                                                   ApplicationDateText = s.ApplicationDate.ToString(),
                                                                   HardCopyReceived = s.HardCopyReceived,
                                                                   HardCopyReceiveDate = s.HardCopyReceiveDate,
                                                                   HardCopyReceiveDateText = s.HardCopyReceiveDate.ToString()
                                                               });
            if (!string.IsNullOrEmpty(searchString))
                allApp = allApp.Where(a => a.ApplicationNo.ToLower().Contains(searchString.ToLower()));
            var temp = allApp.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }
        public IPagedList<ApplicationDto> GetOperationLoanApplications(int pageSize, int pageCount, string searchString, long UserId)
        {
            var application = GenService.GetAll<Application>()
                    .Where(s => (s.ApplicationStage == ApplicationStage.SentToOperations ||
                                 s.ApplicationStage == ApplicationStage.UnderProcessAtOperations ||
                                 s.ApplicationStage == ApplicationStage.DCLUnderProcess)
                    && s.CurrentHolding == UserId && (s.LoanApplicationId != null && s.LoanApplicationId > 0) && s.Status == EntityStatus.Active); // Previous-'SubmitedtoOperations'
            var proposal = GenService.GetAll<Proposal>().Where(r => r.IsApproved == true && r.Status == EntityStatus.Active);
            var allApp = (from app in application
                          join prop in proposal on app.Id equals prop.ApplicationId into extra
                          from extr in extra.DefaultIfEmpty()
                          join dcl in GenService.GetAll<DocumentCheckList>().Where(r => r.Status == EntityStatus.Active) on app.Id equals dcl.ApplicationId into dclExtra
                          from check in dclExtra.DefaultIfEmpty()
                          select new ApplicationDto
                          {
                              Id = app.Id,
                              ApplicationNo = app.ApplicationNo,
                              AccountTitle = app.AccountTitle,
                              CustomerType = app.CustomerType,
                              CustomerTypeName = app.CustomerType.ToString(),
                              ProductName = app.Product.Name,
                              AppliedLoanAmount = app.LoanApplication.LoanAmountApplied,
                              ApplicationType = app.ApplicationType,
                              ApplicationDate = app.ApplicationDate,
                              ApplicationDateText = app.ApplicationDate.ToString(),
                              LoanPrimarySecurityType =
                                  app.LoanApplication != null ? app.LoanApplication.LoanPrimarySecurityType : 0,
                              HardCopyReceived = app.HardCopyReceived,
                              HardCopyReceiveDate = app.HardCopyReceiveDate,
                              HardCopyReceiveDateText = app.HardCopyReceiveDate.ToString(),
                              ProposalId = extr != null ? extr.Id : 0,
                              DclId = check != null ? check.Id : 0,
                              RMName = extr != null ? extr.RMName : ""
                          });
            if (!string.IsNullOrEmpty(searchString))
                allApp = allApp.Where(a =>
                a.ApplicationNo.ToLower().Contains(searchString.ToLower())
                || a.AccountTitle.ToLower().Contains(searchString.ToLower())
                || a.ProductName.ToLower().Contains(searchString.ToLower())
                || a.RMName.ToLower().Contains(searchString.ToLower()));
            var temp = allApp.OrderByDescending(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }

        public IPagedList<ApplicationDto> GetCaLoanApplications(int pageSize, int pageCount, string searchString, long UserId)
        {
            var allApp = GenService.GetAll<Application>()
                .Where(s => (s.ApplicationStage == ApplicationStage.SentToOperations ||
                             s.ApplicationStage == ApplicationStage.UnderProcessAtOperations ||
                             s.ApplicationStage == ApplicationStage.DCLUnderProcess) &&
                            s.Product.FacilityType != ProposalFacilityType.RLS &&
                            s.CurrentHolding == null && // Previous-'SubmitedtoOperations'
                            (s.LoanApplicationId != null && s.LoanApplicationId > 0) && s.Status == EntityStatus.Active);
            var proposal = GenService.GetAll<Proposal>().Where(r => r.IsApproved == true && r.Status == EntityStatus.Active);
            var data = (from s in allApp
                        join prop in proposal on s.Id equals prop.ApplicationId into extra
                        from extr in extra.DefaultIfEmpty()
                        select new ApplicationDto
                        {
                            Id = s.Id,
                            ApplicationNo = s.ApplicationNo,
                            AccountTitle = s.AccountTitle,
                            CustomerType = s.CustomerType,
                            CustomerTypeName = s.CustomerType.ToString(),
                            ProductName = s.Product.Name,
                            AppliedLoanAmount = extr != null ? extr.RecomendedLoanAmountFromIPDC : 0,
                            ApplicationType = s.ApplicationType,
                            ApplicationDate = s.ApplicationDate,
                            ApplicationDateText = s.ApplicationDate.ToString(),
                            HardCopyReceived = s.HardCopyReceived,
                            HardCopyReceiveDate = s.HardCopyReceiveDate,
                            HardCopyReceiveDateText = s.HardCopyReceiveDate.ToString(),
                            RMName = extr != null ? extr.RMName : ""
                        });
            if (!string.IsNullOrEmpty(searchString))
                data = data.Where(a => a.ApplicationNo.ToLower().Contains(searchString.ToLower()) || a.AccountTitle.ToLower().Contains(searchString.ToLower()) || a.ProductName.ToLower().Contains(searchString.ToLower()) || a.RMName.ToLower().Contains(searchString.ToLower()));
            var temp = data.OrderByDescending(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }
        public IPagedList<ApplicationDto> GetApprovedRLSApplications(int pageSize, int pageCount, string searchString, long UserId)
        {
            var allApp = (from application in GenService.GetAll<Application>()
                                                .Where(s => (s.ApplicationStage == ApplicationStage.SentToOperations ||
                                                             s.ApplicationStage == ApplicationStage.UnderProcessAtOperations ||
                                                             s.ApplicationStage == ApplicationStage.DCLUnderProcess) &&
                                                             s.Product.FacilityType == ProposalFacilityType.RLS &&
                                                             s.CurrentHolding == null && // Previous-'SubmitedtoOperations'
                                                            (s.LoanApplicationId != null && s.LoanApplicationId > 0) && s.Status == EntityStatus.Active)
                          join prop in GenService.GetAll<Proposal>() on application.Id equals prop.ApplicationId into extra
                          from extr in extra.DefaultIfEmpty()
                          select new ApplicationDto
                          {
                              Id = application.Id,
                              ApplicationNo = application.ApplicationNo,
                              AccountTitle = application.AccountTitle,
                              CustomerType = application.CustomerType,
                              CustomerTypeName = application.CustomerType.ToString(),
                              ProductName = application.Product.Name,
                              AppliedLoanAmount = extr != null ? extr.RecomendedLoanAmountFromIPDC : 0,
                              ApplicationType = application.ApplicationType,
                              ApplicationDate = application.ApplicationDate,
                              ApplicationDateText = application.ApplicationDate.ToString(),
                              HardCopyReceived = application.HardCopyReceived,
                              HardCopyReceiveDate = application.HardCopyReceiveDate,
                              HardCopyReceiveDateText = application.HardCopyReceiveDate.ToString(),
                              ProposalId = extr != null ? extr.Id : 0
                          });//.OrderByDescending(s => s.Id)
            if (!string.IsNullOrEmpty(searchString))
                allApp = allApp.Where(a => a.ApplicationNo.ToLower().Contains(searchString.ToLower()));
            var temp = allApp.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }
        //public IPagedList<ApplicationDto> GetAutoLoanApplicationsForPO(int pageSize, int pageCount, string searchString, long UserId)
        //{
        //    var allApp = GenService.GetAll<DocumentCheckList>().Where(d => d.IsApproved != null && (bool)d.IsApproved)
        //        .Select(d => d.Application).Where(s => s.ApplicationStage == ApplicationStage.SentToOperations && // Previous-'SubmitedtoOperations'
        //        (s.LoanApplicationId != null && s.LoanApplicationId > 0) && s.Status == EntityStatus.Active).Select(s => new ApplicationDto()
        //        {
        //            Id = s.Id,
        //            ApplicationNo = s.ApplicationNo,
        //            AccountTitle = s.AccountTitle,
        //            CustomerType = s.CustomerType,
        //            CustomerTypeName = s.CustomerType.ToString(),
        //            ProductName = s.Product.Name,
        //            AppliedLoanAmount = s.LoanApplication.LoanAmountApplied,
        //            ApplicationType = s.ApplicationType,
        //            ApplicationDate = s.ApplicationDate,
        //            ApplicationDateText = s.ApplicationDate.ToString(),
        //            HardCopyReceived = s.HardCopyReceived,
        //            HardCopyReceiveDate = s.HardCopyReceiveDate,
        //            HardCopyReceiveDateText = s.HardCopyReceiveDate.ToString()
        //        });
        //    if (!string.IsNullOrEmpty(searchString))
        //        allApp = allApp.Where(a => a.ApplicationNo.ToLower().Contains(searchString.ToLower()));
        //    var temp = allApp.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
        //    return temp;
        //}
        public List<CostCenterDto> GetAllCostCenters()
        {
            return Mapper.Map<List<CostCenterDto>>(GenService.GetAll<CostCenter>().Where(c => c.Status == EntityStatus.Active).ToList());
        }
        public ResponseDto LockCurrentHolding(long ApplicationId, ApplicationStage fromApplicaitonStage, long userId)
        {
            var entity = new Application();
            ResponseDto response = new ResponseDto();
            long employeeId = _user.GetEmployeeIdByUserId((long)userId);
            entity = GenService.GetById<Application>(ApplicationId);
            var log = new ApplicationLog();
            if (entity != null)
            {
                try
                {
                    if (entity.CurrentHolding == null)
                    {
                        entity.CurrentHolding = userId;
                        entity.CurrentHoldingEmpId = employeeId;
                        log.FromStage = fromApplicaitonStage;
                        log.Activity = Activity.Submit;
                        log.AppIdRef = ApplicationId;
                        log.ApplicationId = ApplicationId;
                        log.AppType = AppType.Application;
                        log.CreateDate = DateTime.Now;
                        log.CreatedBy = userId;
                        log.FromUserId = userId;
                        log.ToUserId = userId;
                        log.Status = EntityStatus.Active;

                        if (fromApplicaitonStage == ApplicationStage.SentToCRM)
                        {
                            log.ToStage = ApplicationStage.UnderProcessAtCRM;
                            entity.ApplicationStage = ApplicationStage.UnderProcessAtCRM;
                            //N.CreateNotificationForService(NotificationType.ApplicationUnderProcessInCRM, (long)entity.Id);
                        }

                        else if (fromApplicaitonStage == ApplicationStage.SentToOperations)
                        {
                            log.ToStage = ApplicationStage.UnderProcessAtOperations;
                            entity.ApplicationStage = ApplicationStage.UnderProcessAtOperations;
                            //N.CreateNotificationForService(NotificationType.ApplicationUnderProcessInOperations, (long)entity.Id);
                        }
                        else if (entity.ProductType == ProductType.Deposit && fromApplicaitonStage == ApplicationStage.DCLApproved)
                        {
                            log.ToStage = ApplicationStage.AccountOpeningUnderProcess;
                            entity.ApplicationStage = ApplicationStage.AccountOpeningUnderProcess;
                        }

                        if (log.FromUserId > 0)
                            GenService.Save(log);
                        GenService.Save(entity);
                        if (log.ToStage == ApplicationStage.UnderProcessAtCRM)
                        {
                            N.CreateNotificationForService(NotificationType.ApplicationUnderProcessInCRM, (long)entity.Id);
                        }

                        else if (log.ToStage == ApplicationStage.UnderProcessAtOperations)
                        {
                            N.CreateNotificationForService(NotificationType.ApplicationUnderProcessInOperations, (long)entity.Id);
                        }
                        GenService.SaveChanges();
                        response.Id = entity.Id;
                        response.Success = true;
                        response.Message = "Application Current Holding Assigned";
                    }
                    else
                    {
                        response.Message = "Application already assigned to another user.";
                    }
                }
                catch (Exception ex)
                {
                    response.Message = "Operation Failed";
                }

            }
            return response;
        }
        public ResponseDto ReleseCurrentHolding(long ApplicationId, ApplicationStage fromApplicaitonStage, long userId)
        {
            ResponseDto response = new ResponseDto();
            long employeeId = _user.GetEmployeeIdByUserId((long)userId);
            ApplicationLog log = new ApplicationLog();
            var entity = GenService.GetById<Application>(ApplicationId);
            if (entity != null)
            {
                try
                {
                    if (entity.CurrentHolding != null && entity.CurrentHolding == userId)
                    {
                        entity.CurrentHolding = null;
                        entity.CurrentHoldingEmpId = null;
                        log.FromStage = fromApplicaitonStage;
                        log.Activity = Activity.Submit;
                        log.AppIdRef = ApplicationId;
                        log.ApplicationId = ApplicationId;
                        log.AppType = AppType.Application;
                        log.CreateDate = DateTime.Now;
                        log.CreatedBy = userId;
                        log.FromUserId = userId;
                        log.ToUserId = userId;
                        log.Status = EntityStatus.Active;

                        if (fromApplicaitonStage == ApplicationStage.UnderProcessAtCRM)
                        {
                            log.ToStage = ApplicationStage.SentToCRM;
                            entity.ApplicationStage = ApplicationStage.SentToCRM;
                        }
                        else if (fromApplicaitonStage == ApplicationStage.UnderProcessAtOperations)
                        {
                            log.ToStage = ApplicationStage.SentToOperations;
                            entity.ApplicationStage = ApplicationStage.SentToOperations;
                        }
                        else if (entity.ProductType == ProductType.Deposit &&
                                 fromApplicaitonStage == ApplicationStage.AccountOpeningUnderProcess)
                        {
                            log.ToStage = ApplicationStage.DCLApproved;
                            entity.ApplicationStage = ApplicationStage.DCLApproved;
                        }
                        if (log.FromUserId > 0)
                            GenService.Save(log);
                        GenService.Save(entity);
                        GenService.SaveChanges();
                        response.Id = entity.Id;
                        response.Success = true;
                        response.Message = "Application Current Holding Relesed";
                    }
                    else
                    {
                        response.Message = "You do not have access to release this application";
                    }

                }
                catch (Exception ex)
                {
                    response.Id = entity.Id;
                    response.Message = "Operation Failed";
                }
            }

            return response;
        }

        public IPagedList<ApplicationDto> GetRejectedApplicationsPagedList(int pageSize, int pageCount, string searchString)
        {
            var allApp = from applications in GenService.GetAll<Application>().Where(s => s.ApplicationStage < ApplicationStage.Drafted)
                         join rejectedBy in GenService.GetAll<Employee>() on applications.RejectedByEmpId equals rejectedBy.Id into rejectedBygrp
                         from emp in rejectedBygrp.DefaultIfEmpty()
                         select new ApplicationDto() //// Previous-'SubmitedToCRM'
                         {
                             Id = applications.Id,
                             ApplicationNo = applications.ApplicationNo,
                             AccountTitle = applications.AccountTitle,
                             FacilityType = applications.Product.FacilityType,
                             FacilityTypeName = applications.Product.FacilityType.ToString(),
                             ApplicationType = applications.ApplicationType,
                             ApplicationDate = applications.ApplicationDate,
                             ApplicationDateText = applications.ApplicationDate.ToString(),
                             CustomerTypeName = applications.CustomerType.ToString(),
                             ProductTypeName = applications.ProductType.ToString(),
                             ProductName = applications.Product.Name,
                             CurrentHolding = applications.CurrentHolding,
                             ApplicationStage = applications.ApplicationStage,
                             ApplicationStageName = applications.ApplicationStage.ToString(),
                             RejectionReason = applications.RejectionReason,
                             RejectedByEmpId = applications.RejectedByEmpId,
                             RejectedByEmpName = emp != null ? emp.Person.FirstName + " " + emp.Person.LastName : "",
                             RejectedOn = applications.RejectedOn
                         };
            if (!string.IsNullOrEmpty(searchString))
                allApp = allApp.Where(a => a.ApplicationNo.ToLower().Contains(searchString.ToLower()));
            return allApp.OrderBy(r => r.CurrentHolding).ThenByDescending(r => r.Id).ToPagedList(pageCount, pageSize);
        }
        public IPagedList<ApplicationDto> GetRejectedApplicationsRMPagedList(long UserId, int pageSize, int pageCount, string searchString)
        {
            var employeeId = _user.GetEmployeeIdByUserId(UserId);
            if (employeeId > 0)
            {
                var allApp = from applications in GenService.GetAll<Application>().Where(s => s.ApplicationStage < ApplicationStage.Drafted && s.RMId == employeeId)
                             join rejectedBy in GenService.GetAll<Employee>() on applications.RejectedByEmpId equals rejectedBy.Id into rejectedBygrp
                             from emp in rejectedBygrp.DefaultIfEmpty()
                             select new ApplicationDto() //// Previous-'SubmitedToCRM'
                             {
                                 Id = applications.Id,
                                 ApplicationNo = applications.ApplicationNo,
                                 AccountTitle = applications.AccountTitle,
                                 FacilityType = applications.Product.FacilityType,
                                 FacilityTypeName = applications.Product.FacilityType.ToString(),
                                 ApplicationType = applications.ApplicationType,
                                 ApplicationDate = applications.ApplicationDate,
                                 ApplicationDateText = applications.ApplicationDate.ToString(),
                                 CustomerTypeName = applications.CustomerType.ToString(),
                                 ProductTypeName = applications.ProductType.ToString(),
                                 ProductName = applications.Product.Name,
                                 CurrentHolding = applications.CurrentHolding,
                                 ApplicationStage = applications.ApplicationStage,
                                 ApplicationStageName = applications.ApplicationStage.ToString(),
                                 RejectionReason = applications.RejectionReason,
                                 RejectedByEmpId = applications.RejectedByEmpId,
                                 RejectedByEmpName = emp != null ? emp.Person.FirstName + " " + emp.Person.LastName : "",
                                 RejectedOn = applications.RejectedOn
                             };
                if (!string.IsNullOrEmpty(searchString))
                    allApp = allApp.Where(a => a.ApplicationNo.ToLower().Contains(searchString.ToLower()));
                return allApp.OrderBy(r => r.CurrentHolding).ThenByDescending(r => r.Id).ToPagedList(pageCount, pageSize);
            }
            var dummy = new List<ApplicationDto>();
            return dummy.ToPagedList(pageCount, pageSize);
        }

        public AddressDto GetProjectAddress(long projectId)
        {
            var project = GenService.GetById<Project>(projectId);
            var result = Mapper.Map<AddressDto>(project.ProjectAddress);
            return result;
        }

        //public IPagedList<ApplicationDto> GetOperationDepositApplications2(int pageSize, int pageCount, string searchString, long UserId)
        //{
        //    var application = GenService.GetAll<Application>()
        //            .Where(s => (s.ApplicationStage == ApplicationStage.SentToOperations ||
        //                         s.ApplicationStage == ApplicationStage.UnderProcessAtOperations ||
        //                         s.ApplicationStage == ApplicationStage.DCLUnderProcess)
        //            && s.CurrentHolding == UserId && (s.DepositApplicationId != null && s.DepositApplicationId > 0) && s.Status == EntityStatus.Active); // Previous-'SubmitedtoOperations'
        //    var proposal = GenService.GetAll<Proposal>().Where(r => r.IsApproved == true && r.Status == EntityStatus.Active);
        //    var allApp = (from app in application
        //                  join prop in proposal on app.Id equals prop.ApplicationId into extra
        //                  from extr in extra.DefaultIfEmpty()
        //                  join dcl in GenService.GetAll<DocumentCheckList>().Where(r => r.Status == EntityStatus.Active) on app.Id equals dcl.ApplicationId into dclExtra
        //                  from check in dclExtra.DefaultIfEmpty()
        //                  select new ApplicationDto
        //                  {
        //                      Id = app.Id,
        //                      ApplicationNo = app.ApplicationNo,
        //                      AccountTitle = app.AccountTitle,
        //                      CustomerType = app.CustomerType,
        //                      CustomerTypeName = app.CustomerType.ToString(),
        //                      ProductName = app.Product.Name,
        //                      AppliedLoanAmount = app.LoanApplication.LoanAmountApplied,
        //                      ApplicationType = app.ApplicationType,
        //                      ApplicationDate = app.ApplicationDate,
        //                      ApplicationDateText = app.ApplicationDate.ToString(),
        //                      LoanPrimarySecurityType =
        //                          app.LoanApplication != null ? app.LoanApplication.LoanPrimarySecurityType : 0,
        //                      HardCopyReceived = app.HardCopyReceived,
        //                      HardCopyReceiveDate = app.HardCopyReceiveDate,
        //                      HardCopyReceiveDateText = app.HardCopyReceiveDate.ToString(),
        //                      ProposalId = extr != null ? extr.Id : 0,
        //                      DclId = check != null ? check.Id : 0,
        //                      RMName = extr != null ? extr.RMName : ""
        //                  });
        //    if (!string.IsNullOrEmpty(searchString))
        //        allApp = allApp.Where(a =>
        //        a.ApplicationNo.ToLower().Contains(searchString.ToLower())
        //        || a.AccountTitle.ToLower().Contains(searchString.ToLower())
        //        || a.ProductName.ToLower().Contains(searchString.ToLower())
        //        || a.RMName.ToLower().Contains(searchString.ToLower()));
        //    var temp = allApp.OrderByDescending(r => r.Id).ToPagedList(pageCount, pageSize);
        //    return temp;
        //}
        public IPagedList<ApplicationDto> RejectedApplicationsByUserId(int pageSize, int pageCount, string searchString, long UserId)
        {
            #region data population
            long employeeId = _user.GetEmployeeIdByUserId(UserId);
            var subordinateEmpList = _employee.GetEmployeeWiseBM(employeeId);
            var subordinateEmpIdList = subordinateEmpList.Where(s => s.EmployeeId != null).Select(s => s.EmployeeId).ToList();

            subordinateEmpIdList.Add(employeeId);
            var allApplications = GenService.GetAll<Application>().Where(a => subordinateEmpIdList.Contains(a.RMId) && a.ApplicationStage < ApplicationStage.Drafted);
            #endregion
            var allApp = from applications in allApplications
                         join rejectedBy in GenService.GetAll<Employee>() on applications.RejectedByEmpId equals rejectedBy.Id into rejectedBygrp
                         from emp in rejectedBygrp.DefaultIfEmpty()
                         select new ApplicationDto() //// Previous-'SubmitedToCRM'
                         {
                             Id = applications.Id,
                             ApplicationNo = applications.ApplicationNo,
                             AccountTitle = applications.AccountTitle,
                             FacilityType = applications.Product.FacilityType,
                             FacilityTypeName = applications.Product.FacilityType.ToString(),
                             ApplicationType = applications.ApplicationType,
                             ApplicationDate = applications.ApplicationDate,
                             ApplicationDateText = applications.ApplicationDate.ToString(),
                             CustomerTypeName = applications.CustomerType.ToString(),
                             ProductTypeName = applications.ProductType.ToString(),
                             ProductName = applications.Product.Name,
                             CurrentHolding = applications.CurrentHolding,
                             ApplicationStage = applications.ApplicationStage,
                             ApplicationStageName = applications.ApplicationStage.ToString(),
                             RejectionReason = applications.RejectionReason,
                             RejectedByEmpId = applications.RejectedByEmpId,
                             RejectedByEmpName = emp != null ? emp.Person.FirstName + " " + emp.Person.LastName : "",
                             RejectedOn = applications.RejectedOn
                         };
            if (!string.IsNullOrEmpty(searchString))
                allApp = allApp.Where(a => a.ApplicationNo.ToLower().Contains(searchString.ToLower()));
            return allApp.OrderBy(r => r.CurrentHolding).ThenByDescending(r => r.Id).ToPagedList(pageCount, pageSize);
        }
        public IPagedList<ApplicationDto> GetExceptioApplications(int pageSize, int pageCount, string searchString, long UserId)
        {
            var application = GenService.GetAll<Application>()
                    .Where(s => (s.ApplicationStage == ApplicationStage.SentToOperations ||
                                 s.ApplicationStage == ApplicationStage.UnderProcessAtOperations ||
                                 s.ApplicationStage == ApplicationStage.DCLUnderProcess)
                    && s.CurrentHolding == UserId && (s.LoanApplicationId != null && s.LoanApplicationId > 0) && s.Status == EntityStatus.Active); // Previous-'SubmitedtoOperations'
            var proposal = GenService.GetAll<Proposal>().Where(r => r.IsApproved == true && r.Status == EntityStatus.Active);
            var allApp = (from app in application
                          join prop in proposal on app.Id equals prop.ApplicationId into extra
                          from extr in extra.DefaultIfEmpty()
                          join dcl in GenService.GetAll<DocumentCheckList>().Where(r => r.Status == EntityStatus.Active && r.Exceptions.Count > 0) on app.Id equals dcl.ApplicationId //into dclExtra
                          //from check in dclExtra.DefaultIfEmpty()
                          select new ApplicationDto
                          {
                              Id = app.Id,
                              ApplicationNo = app.ApplicationNo,
                              AccountTitle = app.AccountTitle,
                              CustomerType = app.CustomerType,
                              CustomerTypeName = app.CustomerType.ToString(),
                              ProductName = app.Product.Name,
                              AppliedLoanAmount = app.LoanApplication.LoanAmountApplied,
                              ApplicationType = app.ApplicationType,
                              ApplicationDate = app.ApplicationDate,
                              ApplicationDateText = app.ApplicationDate.ToString(),
                              LoanPrimarySecurityType =
                                  app.LoanApplication != null ? app.LoanApplication.LoanPrimarySecurityType : 0,
                              HardCopyReceived = app.HardCopyReceived,
                              HardCopyReceiveDate = app.HardCopyReceiveDate,
                              HardCopyReceiveDateText = app.HardCopyReceiveDate.ToString(),
                              ProposalId = extr != null ? extr.Id : 0,
                              DclId = dcl != null ? dcl.Id : 0,
                              RMName = extr != null ? extr.RMName : ""
                          });
            if (!string.IsNullOrEmpty(searchString))
                allApp = allApp.Where(a =>
                a.ApplicationNo.ToLower().Contains(searchString.ToLower())
                || a.AccountTitle.ToLower().Contains(searchString.ToLower())
                || a.ProductName.ToLower().Contains(searchString.ToLower())
                || a.RMName.ToLower().Contains(searchString.ToLower()));
            var temp = allApp.OrderByDescending(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }
    }
}
