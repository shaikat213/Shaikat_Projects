using AutoMapper;
using Finix.Auth.DTO;
using Finix.Auth.Facade;
//using Finix.Auth.Infrastructure;
using Finix.IPDC.DTO;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;
using Finix.IPDC.Service;
using Finix.IPDC.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Finix.IPDC.Facade
{
    public class NotificationFacade : BaseFacade
    {
        public void SaveNotifications(List<NotificationDto> notifications)
        {
            var entityList = Mapper.Map<List<Notification>>(notifications);
            foreach (var e in entityList)
                GenService.Save(e, false);
            SaveChanges();

        }
        private readonly UserFacade _user = new UserFacade();
        private readonly OfficeDesignationSettingFacade _organogram = new OfficeDesignationSettingFacade();
        private readonly MenuFacade _menu = new MenuFacade();
        private readonly EmployeeFacade _employee = new EmployeeFacade();
        public List<NotificationDto> GetNotifications(long userId = 0, bool unreadOnly = true)
        {
            var notificationStatuses = new List<NotificationStatusType> { NotificationStatusType.New, NotificationStatusType.Viwed };
            var notifications = GenService.GetAll<Notification>();
            if (userId > 0)
                notifications = notifications.Where(x => x.UserId == userId);
            if (unreadOnly)
                notifications = notifications.Where(x => x.NotificationStatusType == NotificationStatusType.New);
            else
                notifications = notifications.Where(x => notificationStatuses.Contains(x.NotificationStatusType));
            var retVal = notifications.OrderByDescending(x => x.Id).Take(500).ToList();

            return Mapper.Map<List<NotificationDto>>(retVal);
        }
        public void MarkAsRead(List<long> notificationIdList)
        {
            foreach (var id in notificationIdList)
            {
                var n = GenService.GetById<Notification>(id);
                n.NotificationStatusType = NotificationStatusType.Viwed;
                GenService.Save(n, false);
            }
            GenService.SaveChanges();
        }
        internal List<Notification> GetNotificationsForService()
        {
            return GenService.GetAll<Notification>().Where(x => x.NotificationStatusType == NotificationStatusType.ServiceNew).ToList();
        }
        internal void SaveNotification(Notification notification, bool persist = true)
        {
            GenService.Save(notification, persist);
        }
        internal void DeletePreviousNotifications(int day = 10)
        {
            var d = IPDCSystem.SystemDate.AddDays(-day).Date;
            var notifications = GenService.GetAll<Notification>()
                .Where(x => x.NotificationStatusType == NotificationStatusType.Viwed
                && x.CreateDate.Value.Date < d).ToList();
            foreach (var n in notifications)
                GenService.Delete(n, false);
            SaveChanges();

        }
        internal void SaveChanges()
        {
            GenService.SaveChanges();
        }
        List<long> UserListProductwise(NotificationType notificationType,Application application)
        {
            var userIdList = new List<long>();
            var recipients = GenService.GetAll<NotificationSettings>()
                                        .Where(n => n.Status == EntityStatus.Active &&
                                                    n.NotificationType == notificationType)
                                        .ToList();
            if (application.ProductType == ProductType.Loan)
            {
                if (application.Product.FacilityType == ProposalFacilityType.Auto_Loan)
                {
                    recipients = GenService.GetAll<NotificationSettings>()
                                       .Where(n => n.Status == EntityStatus.Active &&
                                                   n.ProposalFacilityType == ProposalFacilityType.Auto_Loan &&
                                                   n.ProposalFacilityType == null &&
                                                   n.NotificationType == notificationType)
                                       .ToList();
                }
                if (application.Product.FacilityType == ProposalFacilityType.Home_Loan)
                {
                    recipients = GenService.GetAll<NotificationSettings>()
                                       .Where(n => n.Status == EntityStatus.Active &&
                                                   n.ProposalFacilityType != ProposalFacilityType.Personal_Loan &&
                                                   n.ProposalFacilityType != ProposalFacilityType.Auto_Loan &&
                                                   n.ProposalFacilityType != ProposalFacilityType.RLS &&
                                                   n.ProposalFacilityType != ProposalFacilityType.Fixed &&
                                                   n.ProposalFacilityType != ProposalFacilityType.Recurring &&
                                                   n.NotificationType == notificationType)
                                       .ToList();
                }
                if (application.Product.FacilityType == ProposalFacilityType.Personal_Loan)
                {
                    recipients = GenService.GetAll<NotificationSettings>()
                                      .Where(n => n.Status == EntityStatus.Active &&
                                                  n.ProposalFacilityType != ProposalFacilityType.Home_Loan &&
                                                  n.ProposalFacilityType != ProposalFacilityType.Auto_Loan &&
                                                  n.ProposalFacilityType != ProposalFacilityType.RLS &&
                                                  n.ProposalFacilityType != ProposalFacilityType.Fixed &&
                                                  n.ProposalFacilityType != ProposalFacilityType.Recurring &&
                                                  n.NotificationType == notificationType)
                                      .ToList();
                }
                if (application.Product.FacilityType == ProposalFacilityType.RLS)
                {
                    recipients = GenService.GetAll<NotificationSettings>()
                                       .Where(n => n.Status == EntityStatus.Active &&
                                                   n.ProposalFacilityType != ProposalFacilityType.Personal_Loan &&
                                                   n.ProposalFacilityType != ProposalFacilityType.Auto_Loan &&
                                                   n.ProposalFacilityType != ProposalFacilityType.Home_Loan&&
                                                   n.ProposalFacilityType != ProposalFacilityType.Fixed &&
                                                   n.ProposalFacilityType != ProposalFacilityType.Recurring &&
                                                   n.NotificationType == notificationType)
                                       .ToList();
                }
                
            }
            if (application.ProductType == ProductType.Deposit)
            {
                if (application.Product.FacilityType == ProposalFacilityType.Fixed)
                {
                    recipients = GenService.GetAll<NotificationSettings>()
                                       .Where(n => n.Status == EntityStatus.Active &&
                                                   n.ProposalFacilityType != ProposalFacilityType.Personal_Loan &&
                                                   n.ProposalFacilityType != ProposalFacilityType.Auto_Loan &&
                                                   n.ProposalFacilityType != ProposalFacilityType.Home_Loan &&
                                                   n.ProposalFacilityType != ProposalFacilityType.RLS &&
                                                   n.ProposalFacilityType != ProposalFacilityType.Recurring &&
                                                   n.NotificationType == notificationType)
                                       .ToList();
                }
                if (application.Product.FacilityType == ProposalFacilityType.Recurring)
                {
                    recipients = GenService.GetAll<NotificationSettings>()
                                       .Where(n => n.Status == EntityStatus.Active &&
                                                   n.ProposalFacilityType != ProposalFacilityType.Personal_Loan &&
                                                   n.ProposalFacilityType != ProposalFacilityType.Auto_Loan &&
                                                   n.ProposalFacilityType != ProposalFacilityType.Home_Loan &&
                                                   n.ProposalFacilityType != ProposalFacilityType.Fixed &&
                                                   n.ProposalFacilityType != ProposalFacilityType.RLS &&
                                                   n.NotificationType == notificationType)
                                       .ToList();
                }

            }
            var recipientEmpIds = new List<long>();

            //Use Role Id To Get Employee
            if (recipients.Where(r => r.RoleId != null).Any())
            {
                var recipientRoles = recipients.Where(r => r.RoleId != null).Select(r => r.RoleId).ToList();
                foreach (var role in recipientRoles)
                {
                    var empIds = _employee.GetEmployeeIdsByRoleId((long)role);
                    if (empIds != null)
                    {
                        foreach (var emp in empIds)
                        {
                            recipientEmpIds.Add((long)emp);
                        }
                    }

                }
            }
            if (recipients.Where(r => r.FunctionalDesignationId != null).Any())
            {
                var recipientDesignation = recipients.Where(r => r.FunctionalDesignationId != null).Select(r => r.FunctionalDesignationId).ToList();
                foreach (var designation in recipientDesignation)
                {
                    var empIds = _organogram.GetEmployeesByOfficeSettingId((long)designation);
                    if (empIds != null)
                    {
                        foreach (var emp in empIds)
                        {
                            recipientEmpIds.Add((long)emp.Id);
                        }
                    }

                }
            }

            if (recipients.Where(r => r.EmployeeId != null).Any())
            {
                var recipientEmps = recipients.Where(r => r.EmployeeId != null).Select(r => r.EmployeeId).ToList();
                foreach (var emp in recipientEmps)
                {
                     recipientEmpIds.Add((long)emp);

                }
            }
            foreach (var recipientEmp in recipientEmpIds)
            {
                var userid = _user.GetUserByEmployeeId(recipientEmp);
                if (userid != null)
                {
                    userIdList.Add((long)userid.Id);
                }

            }
            return userIdList;
        }
        List<long> UserList(NotificationType notificationType)
        {
            var userIdList = new List<long>();
            var recipients = GenService.GetAll<NotificationSettings>()
                                        .Where(n => n.Status == EntityStatus.Active &&
                                                    n.NotificationType == notificationType)
                                        .ToList();
            var recipientEmpIds = new List<long>();

            //Use Role Id To Get Employee
            if (recipients.Where(r => r.RoleId != null).Any())
            {
                var recipientRoles = recipients.Where(r => r.RoleId != null).Select(r => r.RoleId).ToList();
                foreach (var role in recipientRoles)
                {
                    var empIds = _employee.GetEmployeeIdsByRoleId((long)role);
                   if(empIds != null)
                    {
                        foreach (var emp in empIds)
                        {
                            recipientEmpIds.Add((long)emp);
                        }
                    }
                  
                }
            }
            if (recipients.Where(r => r.FunctionalDesignationId != null).Any())
            {
                var recipientDesignation = recipients.Where(r => r.FunctionalDesignationId != null).Select(r => r.FunctionalDesignationId).ToList();
                foreach (var designation in recipientDesignation)
                {
                    var empIds = _organogram.GetEmployeesByOfficeSettingId((long)designation);
                    if (empIds != null)
                    {
                        foreach (var emp in empIds)
                        {
                            recipientEmpIds.Add((long)emp.Id);
                        }
                    }

                }
            }
            if (recipients.Where(r => r.EmployeeId != null).Any())
            {
                var recipientEmps = recipients.Where(r => r.EmployeeId != null).Select(r => r.EmployeeId).ToList();
                foreach (var emp in recipientEmps)
                {
                    recipientEmpIds.Add((long)emp);

                }
            }
            foreach (var recipientEmp in recipientEmpIds)
            {
                var userid = _user.GetUserByEmployeeId(recipientEmp);
                if (userid != null)
                {
                    userIdList.Add((long)userid.Id);
                }

            }
            return userIdList;
        } 
        #region Manual Notifications
        internal void CreateNotifications(NotificationType type, long? refId)
         {
            
            var notification = new Notification { NotificationType = type, RefId = refId, NotificationStatusType = NotificationStatusType.New };
            var userIdList = new List<long>();
            switch (type)
            {
                case NotificationType.ApplicationRejectionByCreditMemoDisapproval:
                    #region
                    if (refId != null)
                    {
                        var application = GenService.GetById<Finix.IPDC.Infrastructure.Models.Application>((long)refId);
                        var AppRef = application.ApplicationNo;
                        var userId = _user.GetUserIdByEmployeeId((long)application.RMId);

                        userIdList.Add(userId);
                        Employee RejectedBy;
                        string RejectedByName = "";

                        var appLogs = GenService.GetAll<Infrastructure.Models.ApplicationLog>()
                       .Where(l => l.ApplicationId == application.Id &&
                          l.ToStage == ApplicationStage.RejectedByMCC &&
                          l.Status == Infrastructure.EntityStatus.Active).FirstOrDefault();

                        if (appLogs != null)
                        {
                            var getEmp = _user.GetEmployeeIdByUserId((long)appLogs.FromUserId);
                            if (getEmp != null)
                            {
                                RejectedBy = GenService.GetById<Employee>((long)getEmp);
                                RejectedByName = RejectedBy.Person.FirstName + " " + RejectedBy.Person.LastName;
                            }

                        }
                        var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)application.RMId);
                        if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                        {
                            foreach (var supervisor in supervisorEmpIdList)
                            {
                                var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                                userIdList.Add(supervisorUserId);
                            }
                        }
                        var userList = UserListProductwise(NotificationType.ApplicationRejectionByCreditMemoDisapproval, application);
                        if(userList != null)
                        {
                            userIdList.AddRange(userList);
                        }
                        userIdList = userIdList.Distinct().ToList();
                        foreach (var u in userIdList)
                        {
                            notification = new Notification { NotificationType = type, RefId = refId, NotificationStatusType = NotificationStatusType.New };
                            notification.UserId = u;

                            //notification.Url = "/IPDC/Application/SearchApplication?AppId=" + application.Id;
                            //var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");

                            notification.Url = "/IPDC/Application/Application?applicationId=" + application.Id;
                            var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");

                            if (menu1 != null)
                            {
                                notification.MenuId = menu1.Id;
                                notification.MenuName = menu1.DisplayName;
                            }


                            notification.Message = "Application Rejected By Credit Memo Disapproval: "+ RejectedByName + "App.Ref: " + AppRef;

                            SaveNotification(notification, false);
                        }


                        SaveChanges();
                    }
                    break;
                #endregion
                case NotificationType.ApplicationRejectionByOpsAndFacilityCloser:
                    #region
                    if (refId != null)
                    {
                        var application = GenService.GetById<Finix.IPDC.Infrastructure.Models.Application>((long)refId);
                        var AppRef = application.ApplicationNo;
                        var userId = _user.GetUserIdByEmployeeId((long)application.RMId);

                        userIdList.Add(userId);
                        Employee RejectedBy;
                        string RejectedByName = "";

                        var appLogs = GenService.GetAll<Infrastructure.Models.ApplicationLog>()
                       .Where(l => l.ApplicationId == application.Id &&
                          l.ToStage == ApplicationStage.RejectedByOperations &&
                          l.Status == Infrastructure.EntityStatus.Active).FirstOrDefault();

                        if (appLogs != null)
                        {
                            var getEmp = _user.GetEmployeeIdByUserId((long)appLogs.FromUserId);
                            if (getEmp != null)
                            {
                                RejectedBy = GenService.GetById<Employee>((long)getEmp);
                                RejectedByName = RejectedBy.Person.FirstName + " " + RejectedBy.Person.LastName;
                            }

                        }
                        var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)application.RMId);
                        if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                        {
                            foreach (var supervisor in supervisorEmpIdList)
                            {
                                var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                                userIdList.Add(supervisorUserId);
                            }
                        }
                        var userList = UserListProductwise(NotificationType.ApplicationRejectionByOpsAndFacilityCloser, application);
                        if(userList != null)
                        {
                            userIdList.AddRange(userList);
                        }
                        userIdList = userIdList.Distinct().ToList();

                        foreach (var u in userIdList)
                        {
                            notification = new Notification { NotificationType = type, RefId = refId, NotificationStatusType = NotificationStatusType.New };
                            notification.UserId = u;

                            //notification.Url = "/IPDC/Application/SearchApplication?AppId=" + application.Id;
                            //var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");

                            notification.Url = "/IPDC/Application/Application?applicationId=" + application.Id;
                            var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");
                            if (menu1 != null)
                            {
                                notification.MenuId = menu1.Id;
                                notification.MenuName = menu1.DisplayName;
                            }


                            notification.Message = "Application Rejected By: "+ RejectedByName + ", App.Ref: "+ AppRef;

                            SaveNotification(notification, false);
                        }


                        SaveChanges();
                    }
                    break;
                #endregion
                case NotificationType.ApplicationRejectionByTL:
                    #region
                    if (refId != null)
                    {
                        var application = GenService.GetById<Finix.IPDC.Infrastructure.Models.Application>((long)refId);
                        if (application != null)
                        {

                            var AppRef = application.ApplicationNo;

                            var userId = _user.GetUserIdByEmployeeId((long)application.RMId);
                            Employee RejectedBy;
                            string RejectedByName = "";

                            var appLogs = GenService.GetAll<Infrastructure.Models.ApplicationLog>()
                           .Where(l => l.ApplicationId == application.Id &&
                              l.ToStage == ApplicationStage.RejectedByTL &&
                              l.Status == Infrastructure.EntityStatus.Active).FirstOrDefault();

                            if (appLogs != null)
                            {
                                var getEmp = _user.GetEmployeeIdByUserId((long)appLogs.FromUserId);
                                if (getEmp != null)
                                {
                                    RejectedBy = GenService.GetById<Employee>((long)getEmp);
                                    RejectedByName = RejectedBy.Person.FirstName + " " + RejectedBy.Person.LastName;
                                }

                            }


                            notification = new Notification { NotificationType = type, RefId = refId, NotificationStatusType = NotificationStatusType.New };
                            notification.UserId = userId;

                            //notification.Url = "/IPDC/Application/SearchApplication?AppId=" + application.Id;
                            //var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");

                            notification.Url = "/IPDC/Application/Application?applicationId=" + application.Id;
                            var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");
                            if (menu1 != null)
                            {
                                notification.MenuId = menu1.Id;
                                notification.MenuName = menu1.DisplayName;
                            }


                            notification.Message = "Application Rejected By TL: " + RejectedByName;

                            SaveNotification(notification, false);

                        }

                        SaveChanges();
                    }
                    break;
                #endregion
                case NotificationType.ApplicationRejectionByBM:
                    #region
                    if (refId != null)
                    {
                        var application = GenService.GetById<Finix.IPDC.Infrastructure.Models.Application>((long)refId);
                        if (application != null)
                        {
                            var AppRef = application.ApplicationNo;
                            var userId = _user.GetUserIdByEmployeeId((long)application.RMId);
                            userIdList.Add(userId);
                            Employee RejectedBy;
                            string RejectedByName = "";

                            var appLogs = GenService.GetAll<Infrastructure.Models.ApplicationLog>()
                           .Where(l => l.ApplicationId == application.Id &&
                              l.ToStage == ApplicationStage.RejectedByBM &&
                              l.Status == Infrastructure.EntityStatus.Active).FirstOrDefault();

                            if (appLogs != null)
                            {
                                var getEmp = _user.GetEmployeeIdByUserId((long)appLogs.FromUserId);
                                if (getEmp != null)
                                {
                                    RejectedBy = GenService.GetById<Employee>((long)getEmp);
                                    RejectedByName = RejectedBy.Person.FirstName + " " + RejectedBy.Person.LastName;
                                }

                            }
                            var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)application.RMId);
                            if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                            {
                                foreach (var supervisor in supervisorEmpIdList)
                                {
                                    var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                                    userIdList.Add(supervisorUserId);
                                }
                            }

                            var bmEmployeeId = _user.GetEmployeeIdByUserId((long)userIdList[1]);
                            var bmEmployee = GenService.GetById<Employee>((long)bmEmployeeId);
                            var bmName = bmEmployee.Person.FirstName + " " + bmEmployee.Person.LastName;

                            foreach (var u in supervisorEmpIdList)
                            {
                                notification = new Notification { NotificationType = type, RefId = refId, NotificationStatusType = NotificationStatusType.New };
                                notification.UserId = u;

                                //notification.Url = "/IPDC/Application/SearchApplication?AppId=" + application.Id;
                                //var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");

                                notification.Url = "/IPDC/Application/Application?applicationId=" + application.Id;
                                var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");
                                if (menu1 != null)
                                {
                                    notification.MenuId = menu1.Id;
                                    notification.MenuName = menu1.DisplayName;
                                }


                                notification.Message = "Application Rejected By BM: " + RejectedByName + "App.Ref: " + AppRef;

                                SaveNotification(notification, false);
                            }

                        }
                        SaveChanges();
                    }
                    break;
                #endregion
                case NotificationType.ApplicationRejectionByCA:
                    #region
                    if (refId != null)
                    {
                        var application = GenService.GetById<Application>((long)refId);
                        var AppRef = application.ApplicationNo;
                        var userId = _user.GetUserIdByEmployeeId((long)application.RMId);
                        userIdList.Add(userId);
                        Employee RejectedBy;
                        string RejectedByName = "";

                        var appLogs = GenService.GetAll<Infrastructure.Models.ApplicationLog>()
                       .Where(l => l.ApplicationId == application.Id &&
                          l.ToStage == ApplicationStage.RejectedByCRM &&
                          l.Status == Infrastructure.EntityStatus.Active).FirstOrDefault();

                        if (appLogs != null)
                        {
                            var getEmp = _user.GetEmployeeIdByUserId((long)appLogs.FromUserId);
                            if (getEmp != null)
                            {
                                RejectedBy = GenService.GetById<Employee>((long)getEmp);
                                RejectedByName = RejectedBy.Person.FirstName + " " + RejectedBy.Person.LastName;
                            }

                        }
                        var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)application.RMId);
                        if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                        {
                            foreach (var supervisor in supervisorEmpIdList)
                            {
                                var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                                userIdList.Add(supervisorUserId);
                            }
                        }

                        var userList = UserListProductwise(NotificationType.ApplicationRejectionByCA,application);
                        if (userList != null)
                        {
                            userIdList.AddRange(userList);
                        }
                        userIdList = userIdList.Distinct().ToList();

                        foreach (var u in userIdList)
                        {
                            notification = new Notification { NotificationType = type, RefId = refId, NotificationStatusType = NotificationStatusType.New };
                            notification.UserId = u;

                            //notification.Url = "/IPDC/Application/SearchApplication?AppId=" + application.Id;
                            //var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");

                            notification.Url = "/IPDC/Application/Application?applicationId=" + application.Id;
                            var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");
                            if (menu1 != null)
                            {
                                notification.MenuId = menu1.Id;
                                notification.MenuName = menu1.DisplayName;
                            }

                            notification.Message = "Application Rejected By: " + RejectedByName + ", App.Ref: " + AppRef;
                            

                            SaveNotification(notification, false);
                        }


                        SaveChanges();
                    }
                    break;
                #endregion
                case NotificationType.ApplicationWaitingForApprovalByBM:
                    #region
                    if (refId != null)
                    {
                        var application = GenService.GetById<Finix.IPDC.Infrastructure.Models.Application>((long)refId);
                        var AppRef = application.ApplicationNo;
                        var userId = _user.GetUserIdByEmployeeId((long)application.RMId);
                        userIdList.Add(userId);

                        var rmName = application.RMEmp.Person.FirstName + " " + application.RMEmp.Person.LastName;

                        var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)application.RMId);
                        if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                        {
                            foreach (var supervisor in supervisorEmpIdList)
                            {
                                var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                                userIdList.Add(supervisorUserId);
                            }
                        }
                        int index = 0;
                        foreach (var u in userIdList)
                        {
                            notification = new Notification { NotificationType = type, RefId = refId, NotificationStatusType = NotificationStatusType.New };
                            notification.UserId = u;

                            if (index != 1)
                            {
                                if (index == 0)
                                {
                                    //notification.Url = "/IPDC/Application/SearchApplication?AppId=" + application.Id;
                                    //var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");

                                    notification.Url = "/IPDC/Application/Application?applicationId=" + application.Id;
                                    var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");
                                    if (menu1 != null)
                                    {
                                        notification.MenuId = menu1.Id;
                                        notification.MenuName = menu1.DisplayName;
                                    }
                                }
                                if (index == 2)
                                {
                                    notification.Url = "/IPDC/Application/ApplicationApprovalBM?applicationId=" + application.Id;
                                    var menu2 = _menu.GetMenuByUrl("/IPDC/Application/ApplicationApprovalBM");
                                    if (menu2 != null)
                                    {
                                        notification.MenuId = menu2.Id;
                                        notification.MenuName = menu2.DisplayName;
                                    }
                                }


                                var tlEmployeeId = _user.GetEmployeeIdByUserId((long)userIdList[1]);
                                var tlEmployee = GenService.GetById<Employee>((long)tlEmployeeId);
                                var tlName = tlEmployee.Person.FirstName + " " + tlEmployee.Person.LastName;

                                notification.Message = "RM's Name: " + rmName + ", TL's Name: " + tlName + ", App Ref No: " + AppRef;

                                SaveNotification(notification, false);
                            }
                            index++;
                        }

                        SaveChanges();
                    }
                    break;
                #endregion
                case NotificationType.ApplicationWaitingForApprovalByTL:
                    #region
                    if (refId != null)
                    {
                        var application = GenService.GetById<Finix.IPDC.Infrastructure.Models.Application>((long)refId);
                        var AppRef = application.ApplicationNo;
                        var userId = _user.GetUserIdByEmployeeId((long)application.RMId);
                        userIdList.Add(userId);

                        var App = GenService.GetById<Finix.IPDC.Infrastructure.Models.Application>((long)application.Id);
                        var rmName = App.RMEmp.Person.FirstName + " " + App.RMEmp.Person.LastName;

                        var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)application.RMId);
                        if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                        {
                            foreach (var supervisor in supervisorEmpIdList)
                            {
                                var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                                userIdList.Add(supervisorUserId);
                            }
                        }
                        int index = 0;
                        foreach (var u in userIdList)
                        {
                            if (index == 1)
                            {
                                notification = new Notification { NotificationType = type, RefId = refId, NotificationStatusType = NotificationStatusType.New };
                                notification.UserId = u;
                                notification.Url = "/IPDC/Application/ApplicationApprovalTL?applicationId=" + application.Id;
                                var menu = _menu.GetMenuByUrl("/IPDC/Application/ApplicationApprovalTL");
                                if (menu != null)
                                {
                                    notification.MenuId = menu.Id;
                                    notification.MenuName = menu.DisplayName;
                                }
                                notification.Message = "RM's Name: " + rmName + ", App Ref No: " + AppRef;

                                SaveNotification(notification, false);
                            }
                            index++;
                        }

                        SaveChanges();
                    }
                    break;

                #endregion
                case NotificationType.CallReferedByEmployee:
                    #region
                    if (refId != null)
                    {
                        var calls = GenService.GetById<Call>((long)refId);
                        var officeDesignationId = calls.ReferredTo;
                        var getEmp = GenService.GetAll<EmployeeDesignationMapping>().Where(e => e.OfficeDesignationSettingId == officeDesignationId && e.Status == Infrastructure.EntityStatus.Active).FirstOrDefault();
                        var userId = _user.GetUserIdByEmployeeId(getEmp.EmployeeId);

                        var refferedName = getEmp.Employee.Person.FirstName + " " + getEmp.Employee.Person.LastName;

                        notification = new Notification { NotificationType = type, RefId = refId, NotificationStatusType = NotificationStatusType.New };
                        notification.UserId = userId;
                        notification.Url = "/IPDC/Call/SearchCallBMInformation?callEntryId=" + refId;
                        var menu = _menu.GetMenuByUrl("/IPDC/Call/SearchCallBMInformation");
                        if (menu != null)
                        {
                            notification.MenuId = menu.Id;
                            notification.MenuName = menu.DisplayName;
                        }
                        notification.Message = "Call is reffered from " + calls.CallCreatorName + " to " + refferedName + ", Call Id: "+calls.CallId;
                        SaveNotification(notification, false);

                        SaveChanges();
                    }
                    break;
                #endregion
                case NotificationType.CallAutoAssignedByEmployee:
                    #region
                    if (refId != null)
                    {

                        var calls = GenService.GetById<Call>((long)refId);
                        var address = GenService.GetById<Address>((long)calls.CustomerAddress.Id);
                        var area = GenService.GetById<OfficeDesignationArea>((long)address.ThanaId);
                        var officeDesignationId = area.OfficeDesignationSettingId;
                        var getEmp = GenService.GetAll<EmployeeDesignationMapping>().Where(e => e.OfficeDesignationSettingId == officeDesignationId && e.Status == Infrastructure.EntityStatus.Active).FirstOrDefault();
                        var userId = _user.GetUserIdByEmployeeId(getEmp.EmployeeId);

                        var refferedName = getEmp.Employee.Person.FirstName + " " + getEmp.Employee.Person.LastName;

                        notification = new Notification { NotificationType = type, RefId = refId, NotificationStatusType = NotificationStatusType.New };
                        notification.UserId = userId;

                        notification.Url = "/IPDC/Call/SearchCallBMInformation?callEntryId=" + refId;
                        var menu = _menu.GetMenuByUrl("/IPDC/Call/SearchCallBMInformation");
                        if (menu != null)
                        {
                            notification.MenuId = menu.Id;
                            notification.MenuName = menu.DisplayName;
                        }
                        notification.Message = "Call automatically assigned from " + calls.CallCreatorName + " to " + refferedName + ", Call Id: "+calls.CallId;
                        SaveNotification(notification, false);

                        SaveChanges();
                    }
                    break;
                #endregion

                case NotificationType.CallAssignedRMByBM:
                    #region
                    if (refId != null)
                    {
                        var calls = GenService.GetById<Call>((long)refId);
                        var officeDesignationId = calls.AssignedTo;
                        var getEmp = GenService.GetAll<EmployeeDesignationMapping>().Where(e => e.OfficeDesignationSettingId == officeDesignationId && e.Status == Infrastructure.EntityStatus.Active).FirstOrDefault();
                        var userId = _user.GetUserIdByEmployeeId(getEmp.EmployeeId);
                        userIdList.Add((long)userId);

                        var refferedName = getEmp.Employee.Person.FirstName + " " + getEmp.Employee.Person.LastName;

                        var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice(getEmp.EmployeeId);
                        if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                        {
                            foreach (var supervisor in supervisorEmpIdList)
                            {
                                var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                                userIdList.Add(supervisorUserId);
                            }
                        }
                  
                        if (supervisorEmpIdList != null)
                        {
                            foreach (var u in supervisorEmpIdList)
                            {
                                notification = new Notification { NotificationType = type, RefId = refId, NotificationStatusType = NotificationStatusType.New };
                                notification.UserId = u;
                               
                                notification.Url = "/IPDC/Call/SearchCallInformation?callEntryId=" + refId;
                                var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchCallInformation");
                                if (menu1 != null)
                                {
                                    notification.MenuId = menu1.Id;
                                    notification.MenuName = menu1.DisplayName;
                                }
                                
                                notification.Message = "Call is assigned from "+calls.CallCreatorName + " to "+refferedName+", Call Id: "+calls.CallId;
                                SaveNotification(notification, false);
                            }
                        }

                        SaveChanges();
                    }
                    break;
                #endregion
                case NotificationType.ApplicationForwardedToCRM:
                    #region
                    if (refId != null)
                    {
                        var application = GenService.GetById<Finix.IPDC.Infrastructure.Models.Application>((long)refId);
                        var AppRef = application.ApplicationNo;
                        var userId = _user.GetUserIdByEmployeeId((long)application.RMId);
                        userIdList.Add(userId);

                        Employee sentEmployee;
                        string senderName = "";

                        var appLogs = GenService.GetAll<Infrastructure.Models.ApplicationLog>()
                       .Where(l => l.ApplicationId == application.Id &&
                          l.ToStage == ApplicationStage.SentToCRM &&
                          l.Status == Infrastructure.EntityStatus.Active).FirstOrDefault();

                        if(appLogs != null)
                        {
                            var getEmp = _user.GetEmployeeIdByUserId((long)appLogs.FromUserId);
                            if(getEmp != null)
                            {
                                sentEmployee = GenService.GetById<Employee>((long)getEmp);
                                senderName = sentEmployee.Person.FirstName + " " + sentEmployee.Person.LastName;
                            }
                           
                        }

                        var rmName = application.RMEmp.Person.FirstName + " " + application.RMEmp.Person.LastName;

                        var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)application.RMId);
                        if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                        {
                            foreach (var supervisor in supervisorEmpIdList)
                            {
                                var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                                userIdList.Add(supervisorUserId);
                            }
                        }
                        userIdList = supervisorEmpIdList.Distinct().ToList();
                        var userList = UserList(NotificationType.ApplicationForwardedToCRM);
                        userIdList.AddRange(userList);
                        userIdList = userIdList.Distinct().ToList();

                        foreach (var u in userIdList)
                        {
                            notification = new Notification { NotificationType = type, RefId = refId, NotificationStatusType = NotificationStatusType.New };
                            notification.UserId = u;
                            notification.Url = "/IPDC/Application/SearchApplication?AppId=" + application.Id;

                            var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");
                            if (menu1 != null)
                            {
                                notification.MenuId = menu1.Id;
                                notification.MenuName = menu1.DisplayName;
                            }
                            var tlEmployeeId = _user.GetEmployeeIdByUserId((long)userIdList[0]);
                            var tlEmployee = GenService.GetById<Employee>((long)tlEmployeeId);
                            var tlName = tlEmployee.Person.FirstName + " " + tlEmployee.Person.LastName;
                            

                            notification.Message = "RM's Name: " + rmName + ", TL's Name: " + tlName + ", App Ref No: " + AppRef;

                            SaveNotification(notification, false);
                        }

                        SaveChanges();
                    }

                    break;
                #endregion
                case NotificationType.ApplicationReceivedFromBMForDA:
                    #region
                    if (refId != null)
                    {
                        var application = GenService.GetById<Finix.IPDC.Infrastructure.Models.Application>((long)refId);
                        var AppRef = application.ApplicationNo;
                        var userId = _user.GetUserIdByEmployeeId((long)application.RMId);
                        userIdList.Add(userId);

                        Employee sentEmployee;
                        string senderName = "";

                        var appLogs = GenService.GetAll<Infrastructure.Models.ApplicationLog>()
                       .Where(l => l.ApplicationId == application.Id &&
                          l.ToStage == ApplicationStage.SentToOperations &&
                          l.Status == Infrastructure.EntityStatus.Active).FirstOrDefault();

                        if(appLogs != null)
                        {
                            var getEmp = _user.GetEmployeeIdByUserId((long)appLogs.FromUserId);
                            if(getEmp != null)
                            {
                                sentEmployee = GenService.GetById<Employee>((long)getEmp);
                                senderName = sentEmployee.Person.FirstName + " " + sentEmployee.Person.LastName;
                            }

                        }
                        var rmName = application.RMEmp.Person.FirstName + " " + application.RMEmp.Person.LastName;

                        var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)application.RMId);
                        if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                        {
                            foreach (var supervisor in supervisorEmpIdList)
                            {
                                var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                                userIdList.Add(supervisorUserId);
                            }
                        }
                        userIdList = supervisorEmpIdList.Distinct().ToList();
                        var userList = UserList(NotificationType.ApplicationReceivedFromBMForDA);
                        userIdList.AddRange(userList);
                        userIdList = userIdList.Distinct().ToList();

                        foreach (var u in userIdList)
                        {
                            notification = new Notification { NotificationType = type, RefId = refId, NotificationStatusType = NotificationStatusType.New };
                            notification.UserId = u;
                            notification.Url = "/IPDC/Application/SearchApplication?AppId=" + application.Id;

                            var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");
                            if (menu1 != null)
                            {
                                notification.MenuId = menu1.Id;
                                notification.MenuName = menu1.DisplayName;
                            }

                            var tlEmployeeId = _user.GetEmployeeIdByUserId((long)userIdList[1]);
                            var tlEmployee = GenService.GetById<Employee>((long)tlEmployeeId);
                            var tlName = tlEmployee.Person.FirstName + " " + tlEmployee.Person.LastName;


                            notification.Message = "RM's Name: " + rmName + ", TL's Name: " + tlName +", Received From: "+ senderName +", App Ref No: " + AppRef;

                            SaveNotification(notification, false);
                        }

                        SaveChanges();
                    }

                    break;
                #endregion
                case NotificationType.CallConvertToLead:
                    #region
                    if (refId != null)
                    {
                        long rmEmployeeId = 0;
                        var salesLead = GenService.GetById<SalesLead>((long)refId);
                        var callGenerator = salesLead.Call.CreatedBy;
                        userIdList.Add((long)callGenerator);
                        var rmUserId = salesLead.CreatedBy;
                        if(rmUserId != null)
                        {
                            rmEmployeeId = _user.GetEmployeeIdByUserId((long)rmUserId);
                        }
                      
                        var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice(rmEmployeeId);
                        if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                        {
                            foreach (var supervisor in supervisorEmpIdList)
                            {
                                var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                                userIdList.Add(supervisorUserId);
                            }
                        }


                        foreach (var u in userIdList)
                        {
                            notification = new Notification { NotificationType = type, RefId = refId, NotificationStatusType = NotificationStatusType.New };
                            notification.UserId = u;
                            notification.Url = "/IPDC/Call/RMSuccessfulCalls?callEntryId=" + salesLead.Call.Id;
                            var menu1 = _menu.GetMenuByUrl("/IPDC/Call/RMSuccessfulCalls");
                            if (menu1 != null)
                            {
                                notification.MenuId = menu1.Id;
                                notification.MenuName = menu1.DisplayName;
                            }
                            notification.Message = "Call Convert To Lead By " + salesLead.CallCreatorName + ", Call Id: "+salesLead.Call.CallId;
                            SaveNotification(notification, false);
                        }
                        SaveChanges();
                    }

                    break;
                #endregion
                case NotificationType.CallDeclaredAsUnsuccessful:
                    #region
                    if (refId != null)
                    {
                        long rmEmployeeId = 0;
                        //userIdList = new List<long>();
                        var calls = GenService.GetById<Call>((long)refId);
                        var callGenerator = calls.CreatedBy;
                        userIdList.Add((long)callGenerator);
                        var rmUserId = calls.CreatedBy;
                        if (rmUserId != null)
                        {
                            rmEmployeeId = _user.GetEmployeeIdByUserId((long)rmUserId);
                        }

                        //
                        var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice(rmEmployeeId);
                        if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                        {
                            foreach (var supervisor in supervisorEmpIdList)
                            {
                                var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                                userIdList.Add(supervisorUserId);
                            }
                        }
                        //

                        foreach (var u in userIdList)
                        {
                            notification = new Notification { NotificationType = type, RefId = refId, NotificationStatusType = NotificationStatusType.New };
                            notification.UserId = u;
                            notification.Url = "/IPDC/Call/RMUnsuccessfulCalls?callEntryId=" + refId;
                            var menu1 = _menu.GetMenuByUrl("/IPDC/Call/RMUnsuccessfulCalls");
                            if (menu1 != null)
                            {
                                notification.MenuId = menu1.Id;
                                notification.MenuName = menu1.DisplayName;
                            }
                            notification.Message = "Call Declared as Unsuccessful By:" + calls.CallCreatorName + ",Call Id: "+ calls.CallId;
                            SaveNotification(notification, false);
                        }
                        SaveChanges();
                    }

                    break;
                #endregion
                case NotificationType.LeadDeclaredAsSuccessful:
                    #region
                    if (refId != null)
                    {
                        long rmEmployeeId = 0;
                        var salesLead = GenService.GetById<SalesLead>((long)refId);
                        var callGenerator = salesLead.CreatedBy;
                        userIdList.Add((long)callGenerator);
                        var rmUserId = salesLead.CreatedBy;
                        if (rmUserId != null)
                        {
                            rmEmployeeId = _user.GetEmployeeIdByUserId((long)rmUserId);
                        }
                        var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice(rmEmployeeId);
                        if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                        {
                            foreach (var supervisor in supervisorEmpIdList)
                            {
                                var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                                userIdList.Add(supervisorUserId);
                            }
                        }
                        //

                        foreach (var u in userIdList)
                        {
                            notification = new Notification { NotificationType = type, RefId = refId, NotificationStatusType = NotificationStatusType.New };
                            notification.UserId = u;
                            notification.Url = "/IPDC/Call/RMSuccessfulCalls?callEntryId=" + salesLead.Call.Id;
                            var menu1 = _menu.GetMenuByUrl("/IPDC/Call/RMSuccessfulCalls");
                            if (menu1 != null)
                            {
                                notification.MenuId = menu1.Id;
                                notification.MenuName = menu1.DisplayName;
                            }
                            notification.Message = "Lead Declared as Successful By: "+salesLead.CallCreatorName;
                            SaveNotification(notification, false);
                        }
                        SaveChanges();
                    }

                    break;
                #endregion
                case NotificationType.LeadDeclaredAsUnSuccessful:
                    #region
                    if (refId != null)
                    {
                        long rmEmployeeId = 0;
                        var salesLead = GenService.GetById<SalesLead>((long)refId);
                        var callGenerator = salesLead.CreatedBy;
                        userIdList.Add((long)callGenerator);
                        var rmUserId = salesLead.CreatedBy;
                        if (rmUserId != null)
                        {
                            rmEmployeeId = _user.GetEmployeeIdByUserId((long)rmUserId);
                        }
                        var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice(rmEmployeeId);
                        if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                        {
                            foreach (var supervisor in supervisorEmpIdList)
                            {
                                var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                                userIdList.Add(supervisorUserId);
                            }
                        }
                        //
                        var index = 0;
                        foreach (var u in userIdList)
                        {
                            notification = new Notification { NotificationType = type, RefId = refId, NotificationStatusType = NotificationStatusType.New };
                            notification.UserId = u;
                            if (index == 0)
                            {
                                notification.Url = "/IPDC/Call/OwnCallList?ownCallList=" + salesLead.Call.Id;
                                var menu1 = _menu.GetMenuByUrl("/IPDC/Call/OwnCallList");
                                if (menu1 != null)
                                {
                                    notification.MenuId = menu1.Id;
                                    notification.MenuName = menu1.DisplayName;
                                }
                            }
                            else if (index == 1 || index == 2)
                            {
                                notification.Url = "/IPDC/SalesLead/UnsuccessfulLeads?leadEntryId=" + refId;
                                var menu1 = _menu.GetMenuByUrl("/IPDC/SalesLead/UnsuccessfulLeads");
                                if (menu1 != null)
                                {
                                    notification.MenuId = menu1.Id;
                                    notification.MenuName = menu1.DisplayName;
                                }
                            }
                            
                            notification.Message = "Lead Declared as UnSuccessful By: "+salesLead.CallCreatorName;
                            SaveNotification(notification, false);
                            index++;
                        }
                        SaveChanges();
                    }

                    break;
                #endregion
                case NotificationType.MessageSentFromCRMOrOpsToAnyUser:
                    #region
                    if (refId != null)
                    {
                        var messaging = GenService.GetById<IPDCMessaging>((long)refId);
                        var msgReceivedByEmpId = messaging.ToEmpId;
                        var AppRef = messaging.Application.ApplicationNo;
                        var userId = _user.GetUserIdByEmployeeId((long)msgReceivedByEmpId);
                        userIdList.Add(userId);

                        var App = GenService.GetById<Finix.IPDC.Infrastructure.Models.Application>((long)messaging.ApplicationId);
                        var rmName = App.RMEmp.Person.FirstName + " " + App.RMEmp.Person.LastName;
                        string senderName = "";
                        if (messaging.FromEmpId != null)
                        {
                            var sentEmployee = GenService.GetById<Employee>((long)messaging.FromEmpId);
                            if (sentEmployee != null)
                            {
                                senderName = sentEmployee.Person.FirstName + " " + sentEmployee.Person.LastName;
                            }
                        }
                        var rmUserIdList = new List<long>();
                        var rmsupervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)messaging.Application.RMId);
                        if (rmsupervisorEmpIdList != null && rmsupervisorEmpIdList.Count > 0)
                        {
                            foreach (var supervisor in rmsupervisorEmpIdList)
                            {
                                var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                                rmUserIdList.Add(supervisorUserId);
                            }
                        }
                        string tlName = "";
                        foreach (var rmUser in rmUserIdList)
                        {
                            var employeeId = _user.GetEmployeeIdByUserId((long)rmUserIdList[0]);
                            var tlEmployee = GenService.GetById<Employee>((long)employeeId);
                            tlName = tlEmployee.Person.FirstName + " " + tlEmployee.Person.LastName;
                        }

                        var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)msgReceivedByEmpId);
                        if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                        {
                            foreach (var supervisor in supervisorEmpIdList)
                            {
                                var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                                userIdList.Add(supervisorUserId);
                            }
                        }
                       
                        var userIdsList = userIdList.Distinct().ToList();
                        foreach (var u in userIdsList)
                        {
                            notification = new Notification { NotificationType = type, RefId = refId, NotificationStatusType = NotificationStatusType.New };
                            notification.UserId = u;
                            notification.Url = "/IPDC/Messaging/ReplyForwardMessage?AppId=" + messaging.ApplicationId + "&msgId=" + refId;
                            var menu1 = _menu.GetMenuByUrl("/IPDC/Messaging/Inbox");
                            if (menu1 != null)
                            {
                                notification.MenuId = menu1.Id;
                                notification.MenuName = menu1.DisplayName;
                            }
                            notification.Message = "RM's Name: " + rmName + ", TL's Name: " + tlName + ", Message Sender: " + senderName + ", App Ref No: " + AppRef;

                            SaveNotification(notification, false);
                        }

                        SaveChanges();
                    }

                    break;
                #endregion
                case NotificationType.MessageReplySentFromCRMOrOpsToRM:
                    #region
                    if (refId != null)
                    {
                        var messaging = GenService.GetById<IPDCMessaging>((long)refId);
                        if (messaging != null)
                        {
                            var msgReceivedByEmpId = messaging.ToEmpId;
                            var AppRef = messaging.Application.ApplicationNo;
                            var userId = _user.GetUserIdByEmployeeId((long)msgReceivedByEmpId);


                            var rmUserId = _user.GetUserIdByEmployeeId((long)messaging.Application.RMId);

                            string senderName = "";
                            if (messaging.FromEmpId != null)
                            {
                                var sentEmployee = GenService.GetById<Employee>((long)messaging.FromEmpId);
                                if (sentEmployee != null)
                                {
                                    senderName = sentEmployee.Person.FirstName + " " + sentEmployee.Person.LastName;
                                }
                            }
                            var App = GenService.GetById<Finix.IPDC.Infrastructure.Models.Application>((long)messaging.ApplicationId);
                            var rmName = App.RMEmp.Person.FirstName + " " + App.RMEmp.Person.LastName;

                            var rmUserIdList = new List<long>();
                            var rmsupervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)messaging.Application.RMId);
                            if (rmsupervisorEmpIdList != null && rmsupervisorEmpIdList.Count > 0)
                            {
                                foreach (var supervisor in rmsupervisorEmpIdList)
                                {
                                    var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                                    rmUserIdList.Add(supervisorUserId);
                                }
                            }
                            string tlName = "";
                            foreach (var rmUser in rmUserIdList)
                            {
                                var employeeId = _user.GetEmployeeIdByUserId((long)rmUserIdList[0]);
                                var tlEmployee = GenService.GetById<Employee>((long)employeeId);
                                tlName = tlEmployee.Person.FirstName + " " + tlEmployee.Person.LastName;
                            }

                            //var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)messaging.FromEmpId);
                            var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)messaging.FromEmpId);
                            if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                            {
                                foreach (var supervisor in supervisorEmpIdList)
                                {
                                    var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                                    userIdList.Add(supervisorUserId);
                                }
                            }
                            userIdList.Add(userId);
                            var userList = userIdList.Distinct().ToList();
                            int i = 0;
                            foreach (var u in userList)
                            {
                                notification = new Notification { NotificationType = type, RefId = refId, NotificationStatusType = NotificationStatusType.New };
                                notification.UserId = u;
                                notification.Url = "/IPDC/Messaging/ReplyForwardMessage?AppId=" + messaging.ApplicationId + "&msgId=" + refId;
                                var menu = _menu.GetMenuByUrl("/IPDC/Messaging/Inbox");
                                if (menu != null)
                                {
                                    notification.MenuId = menu.Id;
                                    notification.MenuName = menu.DisplayName;
                                }

                                notification.Message = "RM's Name: " + rmName + ", TL's Name: " + tlName + ", Message Sender : " + senderName + ", App Ref No: " + AppRef;

                                SaveNotification(notification, false);
                                i++;
                            }
                        }
                        SaveChanges();
                    }

                    break;
                #endregion
                case NotificationType.ApplicationOfferLetterIssued:
                    #region
                    if (refId != null)
                    {
                        Employee sentEmployee;
                        string senderName = "";

                        var offerLetter = GenService.GetById<OfferLetter>((long)refId);
                        if (offerLetter != null)
                        {
                            var getProposal = GenService.GetById<Proposal>((long)offerLetter.ProposalId);
                            var application = GenService.GetById<Finix.IPDC.Infrastructure.Models.Application>((long)getProposal.ApplicationId);
                            var AppRef = application.ApplicationNo;
                            var userId = _user.GetUserIdByEmployeeId((long)application.RMId);
                            userIdList.Add(userId);

                            var offerLetterText = GenService.GetAll<Infrastructure.Models.OfferLetterText>()
                           .Where(l => l.OfferLetterId == offerLetter.Id
                            &&
                            l.Status == Infrastructure.EntityStatus.Active).FirstOrDefault();

                            var getEmp = _user.GetEmployeeIdByUserId((long)offerLetterText.CreatedBy);
                            if(getEmp != null)
                            {
                                sentEmployee = GenService.GetById<Employee>((long)getEmp);
                                if(sentEmployee != null)
                                {
                                    senderName = sentEmployee.Person.FirstName + " " + sentEmployee.Person.LastName;
                                }
                            }

                            var rmName = application.RMEmp.Person.FirstName + " " + application.RMEmp.Person.LastName;

                            var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)application.RMId);
                            if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                            {
                                foreach (var supervisor in supervisorEmpIdList)
                                {
                                    var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                                    userIdList.Add(supervisorUserId);
                                }
                            }

                            List<string> nameList = new List<string>();
                            var i = 0;
                            foreach (var user in userIdList)
                            {
                                var empId = _user.GetEmployeeIdByUserId(user);
                                var emp = GenService.GetById<Employee>((long)empId);
                                if (i == 0)
                                {
                                    nameList.Add("RM's Name: " + emp.Person.FirstName + " " + emp.Person.LastName);
                                }
                                if (i == 1)
                                {
                                    nameList.Add("TL's Name: " + emp.Person.FirstName + " " + emp.Person.LastName);
                                }
                                if (i == 2)
                                {
                                    nameList.Add("BM's Name: " + emp.Person.FirstName + " " + emp.Person.LastName);
                                }
                                i++;
                            }
                            
                            foreach (var u in userIdList)
                            {
                                notification = new Notification { NotificationType = type, RefId = refId, NotificationStatusType = NotificationStatusType.New };
                                notification.UserId = u;
                                //notification.Url = "/IPDC/Application/SearchApplication?AppId=" + application.Id;
                                //var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");

                                notification.Url = "/IPDC/Application/Application?applicationId=" + application.Id;
                                var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");
                                if (menu1 != null)
                                {
                                    notification.MenuId = menu1.Id;
                                    notification.MenuName = menu1.DisplayName;
                                }
                                
                                notification.Message = "RM's Name: " + rmName + ", TL's Name: "+ nameList[1] +", BM's Name" + nameList[2]+", Issued by: " + senderName + ", App Ref No: " + AppRef;


                                SaveNotification(notification, false);
                               
                            }
                        }
                        SaveChanges();
                    }


                    #endregion
                    break;
                case NotificationType.ApplicationCreditMemoApproved:
                    #region
                    if (refId != null)
                    {
                        var getProposal = GenService.GetById<Proposal>((long)refId);
                        var application = GenService.GetById<Finix.IPDC.Infrastructure.Models.Application>((long)getProposal.ApplicationId);
                        var AppRef = application.ApplicationNo;
                        var userId = _user.GetUserIdByEmployeeId((long)application.RMId);
                        userIdList.Add(userId);
                        
                        Employee sentEmployee;
                        string senderName = "";

                        var appLogs = GenService.GetAll<Infrastructure.Models.ApplicationLog>()
                       .Where(l => l.ApplicationId == application.Id &&
                          l.ToStage == ApplicationStage.SentToOperations &&
                          l.Status == Infrastructure.EntityStatus.Active).FirstOrDefault();
                        if (appLogs != null)
                        {
                            var getEmp = _user.GetEmployeeIdByUserId((long)appLogs.FromUserId);
                            if (getEmp != null)
                            {
                                sentEmployee = GenService.GetById<Employee>((long)getEmp);
                                if (sentEmployee != null)
                                {
                                    senderName = sentEmployee.Person.FirstName + " " + sentEmployee.Person.LastName;
                                }
                            }
                        }
                        var rmName = application.RMEmp.Person.FirstName + " " + application.RMEmp.Person.LastName;

                        var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)application.RMId);
                        if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                        {
                            foreach (var supervisor in supervisorEmpIdList)
                            {
                                var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                                userIdList.Add(supervisorUserId);
                            }
                        }
                        var rmUserIdList = new List<long>();
                        var rmsupervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)application.RMId);
                        if (rmsupervisorEmpIdList != null && rmsupervisorEmpIdList.Count > 0)
                        {
                            foreach (var supervisor in rmsupervisorEmpIdList)
                            {
                                var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                                rmUserIdList.Add(supervisorUserId);
                            }
                        }
                        string tlName = "", bmName = "";
                        int i = 0;
                        foreach (var rmUser in rmUserIdList)
                        {
                            if (i == 0)
                            {
                                var tlemployeeId = _user.GetEmployeeIdByUserId((long)rmUserIdList[0]);
                                var tlEmployee = GenService.GetById<Employee>((long)tlemployeeId);
                                tlName = tlEmployee.Person.FirstName + " " + tlEmployee.Person.LastName;
                            }
                            if (i == 1)
                            {
                                var bmemployeeId = _user.GetEmployeeIdByUserId((long)rmUserIdList[1]);
                                var bmEmployee = GenService.GetById<Employee>((long)bmemployeeId);
                                bmName = bmEmployee.Person.FirstName + " " + bmEmployee.Person.LastName;
                            }
                            i++;
                        }
                      
                        var userList = UserList(NotificationType.ApplicationCreditMemoApproved);
                        userIdList.AddRange(userList);
                        userIdList = userIdList.Distinct().ToList();
                        var ni = 0;
                        foreach (var u in userIdList)
                        {
                            notification = new Notification { NotificationType = type, RefId = refId, NotificationStatusType = NotificationStatusType.New };
                            notification.UserId = u;
                            //notification.Url = "/IPDC/Application/SearchApplication?AppId=" + application.Id;
                            //var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");

                            notification.Url = "/IPDC/Application/Application?applicationId=" + application.Id;
                            var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");
                            if (menu1 != null)
                            {
                                notification.MenuId = menu1.Id;
                                notification.MenuName = menu1.DisplayName;
                            }

                            
                            notification.Message = "RM's Name : " + rmName + ", TL's Name: " + tlName + ", BM's Name: " + bmName + ", Approved by: " + senderName + ",App Ref No: " + AppRef;


                            SaveNotification(notification, false);
                            ni++;
                        }
                        
                        SaveChanges();
                    }

                    
                    #endregion
                    break;
                case NotificationType.ApplicationAvailableForGeneratingOfferLetter:
                    #region
                    if (refId != null)
                    {
                        var getProposal = GenService.GetById<Proposal>((long)refId);
                        var application = GenService.GetById<Finix.IPDC.Infrastructure.Models.Application>((long)getProposal.ApplicationId);
                        var AppRef = application.ApplicationNo;
                        var userId = _user.GetUserIdByEmployeeId((long)application.RMId);
                        userIdList.Add(userId);

                        Employee sentEmployee;
                        string senderName = "";

                        var appLogs = GenService.GetAll<Infrastructure.Models.ApplicationLog>()
                       .Where(l => l.ApplicationId == application.Id &&
                          l.ToStage == ApplicationStage.SentToOperations &&
                          l.Status == Infrastructure.EntityStatus.Active).FirstOrDefault();
                        if(appLogs.FromUserId != null)
                        {
                            var getEmp = _user.GetEmployeeIdByUserId((long)appLogs.FromUserId);
                            if(getEmp != null)
                            {
                                sentEmployee = GenService.GetById<Employee>((long)getEmp);
                                if (sentEmployee != null)
                                {
                                    senderName = sentEmployee.Person.FirstName + " " + sentEmployee.Person.LastName;
                                }
                            }
                        }
                      

                        var rmName = application.RMEmp.Person.FirstName + " " + application.RMEmp.Person.LastName;

                        var rmUserIdList = new List<long>();
                        var rmsupervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)application.RMId);
                        if (rmsupervisorEmpIdList != null && rmsupervisorEmpIdList.Count > 0)
                        {
                            foreach (var supervisor in rmsupervisorEmpIdList)
                            {
                                var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                                rmUserIdList.Add(supervisorUserId);
                            }
                        }
                        string tlName = "",bmName="";
                        int i = 0;
                        foreach (var rmUser in rmUserIdList)
                        {
                            if(i==0)
                            {
                                var tlemployeeId = _user.GetEmployeeIdByUserId((long)rmUserIdList[0]);
                                var tlEmployee = GenService.GetById<Employee>((long)tlemployeeId);
                                tlName = tlEmployee.Person.FirstName + " " + tlEmployee.Person.LastName;
                            }
                            if (i == 1)
                            {
                                var bmemployeeId = _user.GetEmployeeIdByUserId((long)rmUserIdList[1]);
                                var bmEmployee = GenService.GetById<Employee>((long)bmemployeeId);
                                bmName = bmEmployee.Person.FirstName + " " + bmEmployee.Person.LastName;
                            }
                            i++;
                        }

                        var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)application.RMId);
                        if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                        {
                            foreach (var supervisor in supervisorEmpIdList)
                            {
                                var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                                userIdList.Add(supervisorUserId);
                            }
                        }
                       

                        //get list from notification settings
                     
                        userIdList.Add((long)appLogs.FromUserId);
                        userIdList = userIdList.Distinct().ToList();

                        foreach (var u in userIdList)
                        {
                            notification = new Notification { NotificationType = type, RefId = refId, NotificationStatusType = NotificationStatusType.New };
                            notification.UserId = u;

                            if(u == appLogs.FromUserId)
                            {
                                notification.Url = "/IPDC/CRM/ProposalList?ProposalId=" + getProposal.Id;
                                var menu1 = _menu.GetMenuByUrl("/IPDC/CRM/ProposalList");
                                if (menu1 != null)
                                {
                                    notification.MenuId = menu1.Id;
                                    notification.MenuName = menu1.DisplayName;
                                }
                            }
                            else
                            {
                                notification.Url = "/IPDC/Application/Application?applicationId=" + application.Id;
                                var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");

                                //notification.Url = "/IPDC/Application/SearchApplication?AppId=" + application.Id;
                                //var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");
                                if (menu1 != null)
                                {
                                    notification.MenuId = menu1.Id;
                                    notification.MenuName = menu1.DisplayName;
                                }
                            }
                         

                            notification.Message = "RM's Name : "+ rmName + ", TL's Name: "+ tlName +", BM's Name: "+ bmName + ", Approved by: " + senderName + ",App Ref No: " + AppRef;


                            SaveNotification(notification, false);
                            
                        }

                        SaveChanges();
                    }


                    #endregion
                    break;
                case NotificationType.ApplicationUnderProcessInCRM:
                    #region
                    if (refId != null)
                    {
                        var application = GenService.GetById<Finix.IPDC.Infrastructure.Models.Application>((long)refId);
                        var AppRef = application.ApplicationNo;
                        var userId = _user.GetUserIdByEmployeeId((long)application.RMId);
                        userIdList.Add(userId);
                        Employee sentEmployee;
                        string senderName = "";

                        var appLogs = GenService.GetAll<Infrastructure.Models.ApplicationLog>()
                       .Where(l => l.ApplicationId == application.Id &&
                          l.ToStage == ApplicationStage.UnderProcessAtCRM &&
                          l.Status == Infrastructure.EntityStatus.Active).FirstOrDefault();

                        if (appLogs != null)
                        {
                            var getEmp = _user.GetEmployeeIdByUserId((long)appLogs.FromUserId);
                            if (getEmp != null)
                            {
                                sentEmployee = GenService.GetById<Employee>((long)getEmp);
                                senderName = sentEmployee.Person.FirstName + " " + sentEmployee.Person.LastName;
                            }

                        }

                        var rmName = application.RMEmp.Person.FirstName + " " + application.RMEmp.Person.LastName;

                        var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)application.RMId);
                        if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                        {
                            foreach (var supervisor in supervisorEmpIdList)
                            {
                                var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                                userIdList.Add(supervisorUserId);
                            }
                        }
                        int i = 0;
                        foreach (var u in userIdList)
                        {
                            notification = new Notification { NotificationType = type, RefId = refId, NotificationStatusType = NotificationStatusType.New };
                            notification.UserId = u;
                            //notification.Url = "/IPDC/Application/SearchApplication?AppId=" + application.Id;
                            //var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");

                            notification.Url = "/IPDC/Application/Application?applicationId=" + application.Id;
                            var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");
                            if (menu1 != null)
                            {
                                notification.MenuId = menu1.Id;
                                notification.MenuName = menu1.DisplayName;
                            }
                            string tlName = "";
                            if(i==1)
                            {

                                var tlEmployeeId = _user.GetEmployeeIdByUserId((long)userIdList[1]);
                                var tlEmployee = GenService.GetById<Employee>((long)tlEmployeeId);
                                tlName = tlEmployee.Person.FirstName + " " + tlEmployee.Person.LastName;
                            }
                            
                            i++;

                            notification.Message = "RM's Name: " + rmName + ", TL's Name: " + tlName + ", CRM Name: " + senderName + ", App Ref No: " + AppRef;

                            SaveNotification(notification, false);
                        }

                        SaveChanges();
                    }

                    break;
                #endregion
                case NotificationType.ApplicationUnderProcessInOperations:
                    #region
                    if (refId != null)
                    {
                        var application = GenService.GetById<Finix.IPDC.Infrastructure.Models.Application>((long)refId);
                        //var appReceivedByEmpId = application.ToEmpId;
                        var AppRef = application.ApplicationNo;
                        var userId = _user.GetUserIdByEmployeeId((long)application.RMId);
                        userIdList.Add(userId);

                        Employee sentEmployee;
                        string senderName = "";

                        var appLogs = GenService.GetAll<Infrastructure.Models.ApplicationLog>()
                       .Where(l => l.ApplicationId == application.Id &&
                          l.ToStage == ApplicationStage.UnderProcessAtOperations &&
                          l.Status == Infrastructure.EntityStatus.Active).FirstOrDefault();

                        if (appLogs != null)
                        {
                            var getEmp = _user.GetEmployeeIdByUserId((long)appLogs.FromUserId);
                            if (getEmp != null)
                            {
                                sentEmployee = GenService.GetById<Employee>((long)getEmp);
                                senderName = sentEmployee.Person.FirstName + " " + sentEmployee.Person.LastName;
                            }

                        }


                        var App = GenService.GetById<Finix.IPDC.Infrastructure.Models.Application>((long)application.Id);
                        var rmName = App.RMEmp.Person.FirstName + " " + App.RMEmp.Person.LastName;

                        var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)application.RMId);
                        if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                        {
                            foreach (var supervisor in supervisorEmpIdList)
                            {
                                var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                                userIdList.Add(supervisorUserId);
                            }
                        }
                        int i = 0;
                        foreach (var u in userIdList)
                        {
                            notification = new Notification { NotificationType = type, RefId = refId, NotificationStatusType = NotificationStatusType.New };
                            notification.UserId = u;
                            //notification.Url = "/IPDC/Application/SearchApplication?AppId=" + application.Id;
                            //var menu = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");

                            notification.Url = "/IPDC/Application/Application?applicationId=" + application.Id;
                            var menu = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");
                            if (menu != null)
                            {
                                notification.MenuId = menu.Id;
                                notification.MenuName = menu.DisplayName;
                            }
                            string tlName = "";
                            string bmName = "";
                            if (i==1)
                            {
                                var tlEmployeeId = _user.GetEmployeeIdByUserId((long)userIdList[1]);
                                var tlEmployee = GenService.GetById<Employee>((long)tlEmployeeId);
                                tlName = tlEmployee.Person.FirstName + " " + tlEmployee.Person.LastName;
                            }

                            if(i==2)
                            {
                                var bmEmployeeId = _user.GetEmployeeIdByUserId((long)userIdList[2]);
                                var bmEmployee = GenService.GetById<Employee>((long)bmEmployeeId);
                                bmName = bmEmployee.Person.FirstName + " " + bmEmployee.Person.LastName;
                            }

                            i++;
                            notification.Message = "RM's Name: " + rmName + ", TL's Name: " + tlName + ", BM's Name: " + bmName + ", Operations Name: " + senderName + ", App Ref No: " + AppRef;

                            SaveNotification(notification, false);
                        }

                        SaveChanges();
                    }

                    break;
                #endregion
                case NotificationType.ApplicationReadyForDisbursement:
                    #region
                    if (refId != null)
                    {
                        var application = GenService.GetById<Finix.IPDC.Infrastructure.Models.Application>((long)refId);
                        //var appReceivedByEmpId = application.ToEmpId;
                        var AppRef = application.ApplicationNo;
                        var userId = _user.GetUserIdByEmployeeId((long)application.RMId);
                        userIdList.Add(userId);
                        Employee OperationsBy;
                        string OperationsByName = "";

                        var appLogs = GenService.GetAll<Infrastructure.Models.ApplicationLog>()
                       .Where(l => l.ApplicationId == application.Id &&
                          l.ToStage == ApplicationStage.ReadyForDeisbursement &&
                          l.Status == Infrastructure.EntityStatus.Active).FirstOrDefault();

                        if (appLogs != null)
                        {
                            var getEmp = _user.GetEmployeeIdByUserId((long)appLogs.FromUserId);
                            if (getEmp != null)
                            {
                                OperationsBy = GenService.GetById<Employee>((long)getEmp);
                                OperationsByName = OperationsBy.Person.FirstName + " " + OperationsBy.Person.LastName;
                            }

                        }

                        var App = GenService.GetById<Finix.IPDC.Infrastructure.Models.Application>((long)application.Id);
                        var rmName = App.RMEmp.Person.FirstName + " " + App.RMEmp.Person.LastName;

                        var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)application.RMId);
                        if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                        {
                            foreach (var supervisor in supervisorEmpIdList)
                            {
                                var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                                userIdList.Add(supervisorUserId);
                            }
                        }
                        int i = 0;
                        string tlName = "";
                        string bmName = "";
                        foreach (var u in userIdList)
                        {
                            notification = new Notification { NotificationType = type, RefId = refId, NotificationStatusType = NotificationStatusType.New };
                            notification.UserId = u;
                            //notification.Url = "/IPDC/Application/SearchApplication?AppId=" + application.Id;
                            //var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");

                            notification.Url = "/IPDC/Application/Application?applicationId=" + application.Id;
                            var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");

                            if (menu1 != null)
                            {
                                notification.MenuId = menu1.Id;
                                notification.MenuName = menu1.DisplayName;
                            }
                            if (i == 1)
                            {
                                var tlEmployeeId = _user.GetEmployeeIdByUserId((long)userIdList[1]);
                                var tlEmployee = GenService.GetById<Employee>((long)tlEmployeeId);
                                tlName = tlEmployee.Person.FirstName + " " + tlEmployee.Person.LastName;
                            }
                            if (i == 1)
                            {
                                var bmEmployeeId = _user.GetEmployeeIdByUserId((long)userIdList[2]);
                                var bmEmployee = GenService.GetById<Employee>((long)bmEmployeeId);
                                bmName = bmEmployee.Person.FirstName + " " + bmEmployee.Person.LastName;
                            }

                            i++;


                            notification.Message = "RM's Name: " + rmName + ", TL's Name: " + tlName + ", BM's Name: " + bmName + ", Disbursed By(Operations): " + OperationsByName + ", App Ref No: " + AppRef;

                            SaveNotification(notification, false);
                        }

                        SaveChanges();
                    }

                    break;
                #endregion
                case NotificationType.ApplicationFullyPartialyDisbursement:
                    #region
                    if (refId != null)
                    {
                        var application = GenService.GetById<Finix.IPDC.Infrastructure.Models.Application>((long)refId);
                        //var appReceivedByEmpId = application.ToEmpId;
                        var AppRef = application.ApplicationNo;
                        var userId = _user.GetUserIdByEmployeeId((long)application.RMId);
                        userIdList.Add(userId);
                        Employee OpsBy;
                        string OpsByName = "";

                        if(application.ApplicationStage == ApplicationStage.DisbursementComplete)
                        {
                            var appLogs = GenService.GetAll<Infrastructure.Models.ApplicationLog>()
                           .Where(l => l.ApplicationId == application.Id &&
                              l.ToStage == ApplicationStage.DisbursementComplete &&
                              l.Status == Infrastructure.EntityStatus.Active).FirstOrDefault();

                            if (appLogs != null)
                            {
                                var getEmp = _user.GetEmployeeIdByUserId((long)appLogs.FromUserId);
                                if (getEmp != null)
                                {
                                    OpsBy = GenService.GetById<Employee>((long)getEmp);
                                    OpsByName = OpsBy.Person.FirstName + " " + OpsBy.Person.LastName;
                                }

                            }
                        }
                        if (application.ApplicationStage == ApplicationStage.PartialDisbursementComplete)
                        {
                            var appLogs = GenService.GetAll<Infrastructure.Models.ApplicationLog>()
                           .Where(l => l.ApplicationId == application.Id &&
                              l.ToStage == ApplicationStage.PartialDisbursementComplete &&
                              l.Status == Infrastructure.EntityStatus.Active).FirstOrDefault();

                            if (appLogs != null)
                            {
                                var getEmp = _user.GetEmployeeIdByUserId((long)appLogs.FromUserId);
                                if (getEmp != null)
                                {
                                    OpsBy = GenService.GetById<Employee>((long)getEmp);
                                    OpsByName = OpsBy.Person.FirstName + " " + OpsBy.Person.LastName;
                                }

                            }
                        }
                        var App = GenService.GetById<Finix.IPDC.Infrastructure.Models.Application>((long)application.Id);
                        var rmName = App.RMEmp.Person.FirstName + " " + App.RMEmp.Person.LastName;

                        var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)application.RMId);
                        if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                        {
                            foreach (var supervisor in supervisorEmpIdList)
                            {
                                var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                                userIdList.Add(supervisorUserId);
                            }
                        }
                        var i = 0;
                        foreach (var u in userIdList)
                        {
                            notification = new Notification { NotificationType = type, RefId = refId, NotificationStatusType = NotificationStatusType.New };
                            notification.UserId = u;
                            //notification.Url = "/IPDC/Application/SearchApplication?AppId=" + application.Id;
                            //var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");

                            notification.Url = "/IPDC/Application/Application?applicationId=" + application.Id;
                            var menu1 = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");
                            if (menu1 != null)
                            {
                                notification.MenuId = menu1.Id;
                                notification.MenuName = menu1.DisplayName;
                            }
                            string tlName = "";
                            string bmName = "";
                            if(i == 1)
                            {
                                var tlEmployeeId = _user.GetEmployeeIdByUserId((long)userIdList[1]);
                                var tlEmployee = GenService.GetById<Employee>((long)tlEmployeeId);
                                tlName = tlEmployee.Person.FirstName + " " + tlEmployee.Person.LastName;
                            }
                            if(i==2)
                            {
                                var bmEmployeeId = _user.GetEmployeeIdByUserId((long)userIdList[2]);
                                var bmEmployee = GenService.GetById<Employee>((long)bmEmployeeId);
                                bmName = bmEmployee.Person.FirstName + " " + bmEmployee.Person.LastName;
                            }

                            i++;

                            notification.Message = "RM's Name: " + rmName + ", TL's Name: " + tlName + ", BM's Name: " + bmName + ", Disbursed By(Operations): " + OpsByName + ", App Ref No: " + AppRef;

                            SaveNotification(notification, false);
                        }

                        SaveChanges();
                    }

                    break;
                #endregion
      
            }

            
        }
        #endregion

        #region Auto Notifications
        internal void CreateNotificationsByMonitoring()
        {
            //register 30 mins notification
            CreateLeadFollowupNotifications(30);

            //Document Collection As Per DCL
            DocumentCollectionDateIsOverDue3DaysAsPerDCL();
            DocumentCollectionDateIsOverDue2DaysAsPerDCL();
            DocumentCollectionDateIsOverDue1DaysAsPerDCL();
            DocumentCollectionDateIsPastDueAsPerDCL();
            Before1DayDeferredDocumentCollectionNotificationsAsPerDCL();
            OnScheduledDayDeferredDocumentCollectionNotificationsAsPerDCL();

            //Document Collection As Per Application
            OnScheduledDayDeferredDocumentCollectionNotificationsAsPerApplication();
            Before1DayDeferredDocumentCollectionNotificationsAsPerApplication();

            //sales leads
            RmMissedLeadFollowupBy1Day();
            RmMissedLeadFollowupBy2Days();

            //Applications Approval
            ApplicationApprovalPendingTL2Days();
            ApplicationApprovalPendingTL3Days();
            ApplicationApprovalPendingBM3Days();

            ApplicationPendingCRM1Day();
        }
        private void CreateLeadFollowupNotifications(int minutes)
        {
            var now = IPDCSystem.SystemDate;
            var nowDate = now.Date;
            var maxTime = now.AddMinutes(minutes);

            // get respective entities having followup-time <= maxTime and followup-date>=today.
            // say, we found 1,2,3... 10
            // check the notificaions for respective notificaiton type and with these ids
            //if found then exclude those ids.
            //register notificaitons for the remaining ids

            var salesLeads = GenService.GetAll<SalesLead>().Where(s => s.FollowupTime >= nowDate && s.FollowupTime <= maxTime).Select(s => s.Id).ToList();
            var notifications = GenService.GetAll<Notification>()
                .Where(n => n.NotificationType == NotificationType.BeforeScheduled30MinsLeadFollowUp &&
                            n.Status == Infrastructure.EntityStatus.Active &&
                            n.RefId != null &&
                            salesLeads.Contains((long)n.RefId)).ToList();
            var salesLeadIdsRemoveList = notifications.Select(n => n.RefId).ToList();
            salesLeads.RemoveAll(s => salesLeadIdsRemoveList.Contains(s));
            foreach (var lead in salesLeads)
            {

                var notification = new Notification
                {
                    NotificationType = NotificationType.BeforeScheduled30MinsLeadFollowUp,
                    RefId = lead,
                    NotificationStatusType = NotificationStatusType.New
                };
                var leadAssignment = GenService.GetAll<SalesLeadAssignment>()
                        .Where(s => s.SalesLeadId == lead && s.Status == Infrastructure.EntityStatus.Active)
                        .OrderByDescending(s => s.Id)
                        .FirstOrDefault();
                if (leadAssignment != null)
                {
                    long userId = _user.GetUserIdByEmployeeId((long)leadAssignment.AssignedToEmpId);
                    notification.UserId = userId;
                }


                notification.Url = "/IPDC/SalesLead/SalesLeadEntry?leadEntryId=" + lead;
                var menu = _menu.GetMenuByUrl("/IPDC/SalesLead/SalesLeadEntry");
                if (menu != null)
                {
                    notification.MenuId = menu.Id;
                    notification.MenuName = menu.DisplayName;
                }
                notification.Message = UiUtil.GetDisplayName(NotificationType.BeforeScheduled30MinsLeadFollowUp);

                SaveNotification(notification, false);

            }
            SaveChanges();

        }

        // deffered document collection date is overdue for 3 Days as per DCL
        private void DocumentCollectionDateIsOverDue3DaysAsPerDCL()
        {
            var now = IPDCSystem.SystemDate;
            var minDate = now.Date.AddDays(-3);
            var getDCLExp = GenService.GetAll<DocumentCheckListException>()
                .Where(s => s.CollectionDate == minDate &&
                            s.Status == Infrastructure.EntityStatus.Active &&
                            s.DCL.Status == Infrastructure.EntityStatus.Active &&
                            s.DCL.IsApproved != null &&
                            s.DCL.IsApproved == true &&
                            s.DCL.Application.Status == Infrastructure.EntityStatus.Active)
                .Select(s => new { DCLId = s.DCLId, ExceptionId = s.Id }).ToList();

            var dclList = new List<long>();
            if (getDCLExp.Count > 0)
                dclList.AddRange(getDCLExp.Select(s => s.DCLId).ToList());
            var getDCLDet = GenService.GetAll<DocumentCheckListDetail>()
                .Where(s => s.CollectionDate == minDate &&
                            s.Status == Infrastructure.EntityStatus.Active &&
                            s.DCL.Status == Infrastructure.EntityStatus.Active &&
                            s.DCL.IsApproved != null &&
                            s.DCL.IsApproved == true &&
                            s.DCL.Application.Status == Infrastructure.EntityStatus.Active)
                .Select(s => new { DCLId = s.DCLId, DetailId = s.Id }).ToList();
            if (getDCLDet.Count > 0)
                dclList.AddRange(getDCLDet.Select(s => s.DCLId).ToList());
            dclList = dclList.Distinct().ToList();

            //Update
            #region
            var appList = new List<long>();
            foreach (var dce in dclList)
            {
                var getDCLD = GenService.GetById<DocumentCheckList>(dce);

                appList.Add(getDCLD.Application.Id);
            }


            appList = appList.Distinct().ToList();

            var notificationsTest = GenService.GetAll<Notification>()
                 .Where(n => n.NotificationType == NotificationType.DocumentCollectionDateIsOverdueFor3daysAsPerDCL &&
                             n.Status == Infrastructure.EntityStatus.Active &&
                             n.RefId != null &&
                             appList.Contains((long)n.RefId)).ToList();
            var getApplicationIdsRemoveListTest = notificationsTest.Select(n => n.RefId).ToList();
            appList.RemoveAll(s => getApplicationIdsRemoveListTest.Contains(s));

            string listOfDocument = "";
            foreach (var app in appList)
            {
                var getApp = GenService.GetById<Application>(app);

                var getDocumentCheckList = GenService.GetAll<DocumentCheckList>().Where(d => d.ApplicationId == app).ToList();
                foreach (var g in getDocumentCheckList)
                {
                    var getDocumentCheckListDetails = GenService.GetAll<DocumentCheckListDetail>()
                    .Where(d => d.DCLId == g.Id && d.IsRequired == true).ToList();
                    listOfDocument = string.Join(",", getDocumentCheckListDetails.Select(p => p.Name));
                }

                var AppRef = getApp.ApplicationNo;
                var userIdList = new List<long>();
                var userId = _user.GetUserIdByEmployeeId((long)getApp.RMId);
                var rmName = getApp.RMEmp.Person.FirstName + " " + getApp.RMEmp.Person.LastName;
                userIdList.Add(userId);
                Employee sentEmployee;
                string senderName = "";

                var appLogs = GenService.GetAll<Infrastructure.Models.ApplicationLog>()
               .Where(l => l.ApplicationId == getApp.Id &&
                  l.ToStage == ApplicationStage.UnderProcessAtOperations &&
                  l.Status == Infrastructure.EntityStatus.Active).FirstOrDefault();

                if (appLogs != null)
                {
                    var getEmp = _user.GetEmployeeIdByUserId((long)appLogs.FromUserId);
                    if (getEmp != null)
                    {
                        sentEmployee = GenService.GetById<Employee>((long)getEmp);
                        senderName = sentEmployee.Person.FirstName + " " + sentEmployee.Person.LastName;
                    }

                }
                var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)getApp.RMId);
                if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                {
                    foreach (var supervisor in supervisorEmpIdList)
                    {
                        var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                        userIdList.Add(supervisorUserId);
                    }
                }
                int i = 0;
                string tlName = "", bmName = "";
                foreach (var u in userIdList)
                {
                    if (i == 1)
                    {
                        var tlEmployeeId = _user.GetEmployeeIdByUserId((long)userIdList[1]);
                        var tlEmployee = GenService.GetById<Employee>((long)tlEmployeeId);
                        tlName = tlEmployee.Person.FirstName + " " + tlEmployee.Person.LastName;
                    }
                    if (i == 2)
                    {
                        var bmEmployeeId = _user.GetEmployeeIdByUserId((long)userIdList[2]);
                        var bmEmployee = GenService.GetById<Employee>((long)bmEmployeeId);
                        bmName = bmEmployee.Person.FirstName + " " + bmEmployee.Person.LastName;

                    }
                    i++;
                }
                userIdList.Add(appLogs.FromUserId);
                var userList = UserListProductwise(NotificationType.DocumentCollectionDateIsOverdueFor3daysAsPerDCL, getApp);
                if (userList != null)
                {
                    userIdList.AddRange(userList);
                }
                userIdList = userIdList.Distinct().ToList();

                foreach (var u in userIdList)
                {
                    var notification = new Notification
                    {
                        NotificationType = NotificationType.DocumentCollectionDateIsOverdueFor3daysAsPerDCL,
                        RefId = app,
                        NotificationStatusType = NotificationStatusType.New
                    };

                    long FromUserId = 0;
                    if (appLogs != null)
                    {
                        FromUserId = appLogs.FromUserId;
                    }

                    if (u == FromUserId)
                    {
                        notification.UserId = u;
                        //notification.Url = "/IPDC/Application/Application?applicationId=" + getApp.Id;
                        //var menu = _menu.GetMenuByUrl("/IPDC/Operations/LoanApplications");
                        //if (menu != null)
                        //{
                        //    notification.MenuId = menu.Id;
                        //    notification.MenuName = menu.DisplayName;
                        //}
                    }
                    else
                    {
                        notification.UserId = u;
                        notification.Url = "/IPDC/Application/Application?applicationId=" + getApp.Id;
                        var menu = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");
                        if (menu != null)
                        {
                            notification.MenuId = menu.Id;
                            notification.MenuName = menu.DisplayName;
                        }
                    }
                    notification.Message = "RM's Name: "
                          + rmName +
                          ", TL's Name: "
                          + tlName +
                          ", BM's Name: "
                          + bmName +
                          ", Operations Name: "
                          + senderName +
                          ", App Ref No: " + AppRef
                          + ", Document Name :" + listOfDocument;


                    SaveNotification(notification, false);
                }

            }
            #endregion
            //Update



            SaveChanges();

        }
        // deffered document collection date is overdue for 2 Days as per DCL
        private void DocumentCollectionDateIsOverDue2DaysAsPerDCL()
        {
            var now = IPDCSystem.SystemDate;
            var minDate = now.Date.AddDays(-2);
            var getDCLExp = GenService.GetAll<DocumentCheckListException>()
                .Where(s => s.CollectionDate == minDate &&
                            s.Status == Infrastructure.EntityStatus.Active &&
                            s.DCL.Status == Infrastructure.EntityStatus.Active &&
                            s.DCL.IsApproved != null &&
                            s.DCL.IsApproved == true &&
                            s.DCL.Application.Status == Infrastructure.EntityStatus.Active)
                .Select(s => new { DCLId = s.DCLId, ExceptionId = s.Id }).ToList();

            var dclList = new List<long>();
            if (getDCLExp.Count > 0)
                dclList.AddRange(getDCLExp.Select(s => s.DCLId).ToList());
            var getDCLDet = GenService.GetAll<DocumentCheckListDetail>()
                .Where(s => s.CollectionDate == minDate &&
                            s.Status == Infrastructure.EntityStatus.Active &&
                            s.DCL.Status == Infrastructure.EntityStatus.Active &&
                            s.DCL.IsApproved != null &&
                            s.DCL.IsApproved == true &&
                            s.DCL.Application.Status == Infrastructure.EntityStatus.Active)
                .Select(s => new { DCLId = s.DCLId, DetailId = s.Id }).ToList();
            if (getDCLDet.Count > 0)
                dclList.AddRange(getDCLDet.Select(s => s.DCLId).ToList());
            dclList = dclList.Distinct().ToList();

            //Update
            #region
            var appList = new List<long>();
            foreach (var dce in dclList)
            {
                var getDCLD = GenService.GetById<DocumentCheckList>(dce);

                appList.Add(getDCLD.Application.Id);
            }


            appList = appList.Distinct().ToList();

            var notificationsTest = GenService.GetAll<Notification>()
                 .Where(n => n.NotificationType == NotificationType.DocumentCollectionDateIsOverdueFor2daysAsPerDCL &&
                             n.Status == Infrastructure.EntityStatus.Active &&
                             n.RefId != null &&
                             appList.Contains((long)n.RefId)).ToList();
            var getApplicationIdsRemoveListTest = notificationsTest.Select(n => n.RefId).ToList();
            appList.RemoveAll(s => getApplicationIdsRemoveListTest.Contains(s));

            string listOfDocument = "";
            foreach (var app in appList)
            {
                var getApp = GenService.GetById<Application>(app);

                var getDocumentCheckList = GenService.GetAll<DocumentCheckList>().Where(d => d.ApplicationId == app).ToList();
                foreach (var g in getDocumentCheckList)
                {
                    var getDocumentCheckListDetails = GenService.GetAll<DocumentCheckListDetail>()
                    .Where(d => d.DCLId == g.Id && d.IsRequired == true).ToList();
                    listOfDocument = string.Join(",", getDocumentCheckListDetails.Select(p => p.Name));
                }

                var AppRef = getApp.ApplicationNo;
                var userIdList = new List<long>();
                var userId = _user.GetUserIdByEmployeeId((long)getApp.RMId);
                var rmName = getApp.RMEmp.Person.FirstName + " " + getApp.RMEmp.Person.LastName;
                userIdList.Add(userId);
                Employee sentEmployee;
                string senderName = "";

                var appLogs = GenService.GetAll<Infrastructure.Models.ApplicationLog>()
               .Where(l => l.ApplicationId == getApp.Id &&
                  l.ToStage == ApplicationStage.UnderProcessAtOperations &&
                  l.Status == Infrastructure.EntityStatus.Active).FirstOrDefault();

                if (appLogs != null)
                {
                    var getEmp = _user.GetEmployeeIdByUserId((long)appLogs.FromUserId);
                    if (getEmp != null)
                    {
                        sentEmployee = GenService.GetById<Employee>((long)getEmp);
                        senderName = sentEmployee.Person.FirstName + " " + sentEmployee.Person.LastName;
                    }

                }
                var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)getApp.RMId);
                if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                {
                    foreach (var supervisor in supervisorEmpIdList)
                    {
                        var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                        userIdList.Add(supervisorUserId);
                    }
                }
                int i = 0;
                string tlName = "", bmName = "";
                foreach (var u in userIdList)
                {
                    if (i == 1)
                    {
                        var tlEmployeeId = _user.GetEmployeeIdByUserId((long)userIdList[1]);
                        var tlEmployee = GenService.GetById<Employee>((long)tlEmployeeId);
                        tlName = tlEmployee.Person.FirstName + " " + tlEmployee.Person.LastName;
                    }
                    if (i == 2)
                    {
                        var bmEmployeeId = _user.GetEmployeeIdByUserId((long)userIdList[2]);
                        var bmEmployee = GenService.GetById<Employee>((long)bmEmployeeId);
                        bmName = bmEmployee.Person.FirstName + " " + bmEmployee.Person.LastName;

                    }
                    i++;
                }
                userIdList.Add(appLogs.FromUserId);
                var userList = UserListProductwise(NotificationType.DocumentCollectionDateIsOverdueFor2daysAsPerDCL, getApp);
                if (userList != null)
                {
                    userIdList.AddRange(userList);
                }
                userIdList = userIdList.Distinct().ToList();

                foreach (var u in userIdList)
                {
                    var notification = new Notification
                    {
                        NotificationType = NotificationType.DocumentCollectionDateIsOverdueFor2daysAsPerDCL,
                        RefId = app,
                        NotificationStatusType = NotificationStatusType.New
                    };

                    long FromUserId = 0;
                    if (appLogs != null)
                    {
                        FromUserId = appLogs.FromUserId;
                    }

                    if (u == FromUserId)
                    {
                        notification.UserId = u;
                        //notification.Url = "/IPDC/Application/Application?applicationId=" + getApp.Id;
                        //var menu = _menu.GetMenuByUrl("/IPDC/Operations/LoanApplications");
                        //if (menu != null)
                        //{
                        //    notification.MenuId = menu.Id;
                        //    notification.MenuName = menu.DisplayName;
                        //}
                    }
                    else
                    {
                        notification.UserId = u;
                        notification.Url = "/IPDC/Application/Application?applicationId=" + getApp.Id;
                        var menu = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");
                        if (menu != null)
                        {
                            notification.MenuId = menu.Id;
                            notification.MenuName = menu.DisplayName;
                        }
                    }
                    notification.Message = "RM's Name: "
                        + rmName +
                        ", TL's Name: "
                        + tlName +
                        ", BM's Name: "
                        + bmName +
                        ", Operations Name: "
                        + senderName +
                        ", App Ref No: " + AppRef
                        + ", Document Name :" + listOfDocument;

                    SaveNotification(notification, false);
                }

            }
            #endregion
            //Update

            SaveChanges();

        }
        // deffered document collection date is overdue for 1 Days as per DCL
        private void DocumentCollectionDateIsOverDue1DaysAsPerDCL()
          {
            var now = IPDCSystem.SystemDate;
            var minDate = now.Date.AddDays(-1);
            var getDCLExp = GenService.GetAll<DocumentCheckListException>()
                .Where(s => s.CollectionDate == minDate &&
                            s.Status == Infrastructure.EntityStatus.Active &&
                            s.DCL.Status == Infrastructure.EntityStatus.Active &&
                            s.DCL.IsApproved != null &&
                            s.DCL.IsApproved == true &&
                            s.DCL.Application.Status == Infrastructure.EntityStatus.Active)
                .Select(s => new { DCLId = s.DCLId, ExceptionId = s.Id }).ToList();

            var dclList = new List<long>();
            if (getDCLExp.Count > 0)
                dclList.AddRange(getDCLExp.Select(s => s.DCLId).ToList());
            var getDCLDet = GenService.GetAll<DocumentCheckListDetail>()
                .Where(s => s.CollectionDate == minDate &&
                            s.Status == Infrastructure.EntityStatus.Active &&
                            s.DCL.Status == Infrastructure.EntityStatus.Active &&
                            s.DCL.IsApproved != null &&
                            s.DCL.IsApproved == true &&
                            s.DCL.Application.Status == Infrastructure.EntityStatus.Active)
                .Select(s => new { DCLId = s.DCLId, DetailId = s.Id ,docName=s.Name}).ToList();
            if (getDCLDet.Count > 0)
                dclList.AddRange(getDCLDet.Select(s => s.DCLId).ToList());
            dclList = dclList.Distinct().ToList();

        
            //Update
            #region
            var appList = new List<long>();
            foreach (var dce in dclList)
            {
                var getDCLD = GenService.GetById<DocumentCheckList>(dce);
        
                appList.Add(getDCLD.Application.Id);
            }


            appList = appList.Distinct().ToList();

            var notificationsTest = GenService.GetAll<Notification>()
                 .Where(n => n.NotificationType == NotificationType.DocumentCollectionDateIsOverdueFor1daysAsPerDCL &&
                             n.Status == Infrastructure.EntityStatus.Active &&
                             n.RefId != null &&
                             appList.Contains((long)n.RefId)).ToList();
            var getApplicationIdsRemoveListTest = notificationsTest.Select(n => n.RefId).ToList();
            appList.RemoveAll(s => getApplicationIdsRemoveListTest.Contains(s));

            string listOfDocument = "";
            foreach (var app in appList)
            {
                var getApp = GenService.GetById<Application>(app);

                var getDocumentCheckList = GenService.GetAll<DocumentCheckList>().Where(d => d.ApplicationId == app).ToList();
                foreach(var g in getDocumentCheckList)
                {
                    var getDocumentCheckListDetails = GenService.GetAll<DocumentCheckListDetail>()
                    .Where(d => d.DCLId == g.Id && d.IsRequired == true).ToList();
                    listOfDocument = string.Join(",", getDocumentCheckListDetails.Select(p => p.Name));
                }

                var AppRef = getApp.ApplicationNo;
                var userIdList = new List<long>();
                var userId = _user.GetUserIdByEmployeeId((long)getApp.RMId);
                var rmName = getApp.RMEmp.Person.FirstName + " " + getApp.RMEmp.Person.LastName;
                userIdList.Add(userId);
                Employee sentEmployee;
                string senderName = "";

                var appLogs = GenService.GetAll<Infrastructure.Models.ApplicationLog>()
               .Where(l => l.ApplicationId == getApp.Id &&
                  l.ToStage == ApplicationStage.UnderProcessAtOperations &&
                  l.Status == Infrastructure.EntityStatus.Active).FirstOrDefault();

                if (appLogs != null)
                {
                    var getEmp = _user.GetEmployeeIdByUserId((long)appLogs.FromUserId);
                    if (getEmp != null)
                    {
                        sentEmployee = GenService.GetById<Employee>((long)getEmp);
                        senderName = sentEmployee.Person.FirstName + " " + sentEmployee.Person.LastName;
                    }

                }
                var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)getApp.RMId);
                if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                {
                    foreach (var supervisor in supervisorEmpIdList)
                    {
                        var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                        userIdList.Add(supervisorUserId);
                    }
                }
                int i = 0;
                string tlName = "", bmName = "";
                foreach (var u in userIdList)
                {  
                    if (i == 1)
                    {
                        var tlEmployeeId = _user.GetEmployeeIdByUserId((long)userIdList[1]);
                        var tlEmployee = GenService.GetById<Employee>((long)tlEmployeeId);
                        tlName = tlEmployee.Person.FirstName + " " + tlEmployee.Person.LastName;
                    }
                    if (i == 2)
                    {
                        var bmEmployeeId = _user.GetEmployeeIdByUserId((long)userIdList[2]);
                        var bmEmployee = GenService.GetById<Employee>((long)bmEmployeeId);
                        bmName = bmEmployee.Person.FirstName + " " + bmEmployee.Person.LastName;

                    }
                    i++;
                }
                userIdList.Add(appLogs.FromUserId);
                userIdList = userIdList.Distinct().ToList();

                foreach (var u in userIdList)
                {
                    var notification = new Notification
                    {
                        NotificationType = NotificationType.DocumentCollectionDateIsOverdueFor1daysAsPerDCL,
                        RefId = app,
                        NotificationStatusType = NotificationStatusType.New
                    };

                    long FromUserId = 0;
                    if (appLogs != null)
                    {
                        FromUserId = appLogs.FromUserId;
                    }

                    if (u == FromUserId)
                    {
                        notification.UserId = u;
                        //notification.Url = "/IPDC/Application/Application?applicationId=" + getApp.Id;
                        //var menu = _menu.GetMenuByUrl("/IPDC/Operations/LoanApplications");
                        //if (menu != null)
                        //{
                        //    notification.MenuId = menu.Id;
                        //    notification.MenuName = menu.DisplayName;
                        //}
                    }
                    else
                    {
                        notification.UserId = u;
                        notification.Url = "/IPDC/Application/Application?applicationId=" + getApp.Id;
                        var menu = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");
                        if (menu != null)
                        {
                            notification.MenuId = menu.Id;
                            notification.MenuName = menu.DisplayName;
                        }
                    }
                    notification.Message = "RM's Name: "
                        + rmName +
                        ", TL's Name: "
                        + tlName +
                        ", BM's Name: "
                        + bmName +
                        ", Operations Name: "
                        + senderName +
                        ", App Ref No: " + AppRef
                        + ", Document Name :" + listOfDocument;


                    SaveNotification(notification, false);
                }

            }
            #endregion
            //Update

            SaveChanges();

        }
        // deffered document collection date is past as per DCL
        private void DocumentCollectionDateIsPastDueAsPerDCL()
        {
            var now = IPDCSystem.SystemDate;
            var nowDate = now.Date;

            var getDCLExp = GenService.GetAll<DocumentCheckListException>()
                .Where(s => s.CollectionDate < nowDate &&
                            s.Status == Infrastructure.EntityStatus.Active &&
                            s.DCL.Status == Infrastructure.EntityStatus.Active &&
                            s.DCL.IsApproved != null &&
                            s.DCL.IsApproved == true &&
                            s.DCL.Application.Status == Infrastructure.EntityStatus.Active)
                .Select(s => new { DCLId = s.DCLId, ExceptionId = s.Id }).ToList();

            var dclList = new List<long>();
            if (getDCLExp.Count > 0)
                dclList.AddRange(getDCLExp.Select(s => s.DCLId).ToList());
            var getDCLDet = GenService.GetAll<DocumentCheckListDetail>()
                .Where(s => s.CollectionDate < nowDate &&
                            s.Status == Infrastructure.EntityStatus.Active &&
                            s.DCL.Status == Infrastructure.EntityStatus.Active &&
                            s.DCL.IsApproved != null &&
                            s.DCL.IsApproved == true &&
                            s.DCL.Application.Status == Infrastructure.EntityStatus.Active)
                .Select(s => new { DCLId = s.DCLId, DetailId = s.Id }).ToList();
            if (getDCLDet.Count > 0)
                dclList.AddRange(getDCLDet.Select(s => s.DCLId).ToList());
            dclList = dclList.Distinct().ToList();

            //Update
            #region
            var appList = new List<long>();
            foreach (var dce in dclList)
            {
                var getDCLD = GenService.GetById<DocumentCheckList>(dce);

                appList.Add(getDCLD.Application.Id);
            }


            appList = appList.Distinct().ToList();

            var notificationsTest = GenService.GetAll<Notification>()
                 .Where(n => n.NotificationType == NotificationType.DocumentCollectionDateIsPastDueAsPerDCL &&
                             n.Status == Infrastructure.EntityStatus.Active &&
                             n.RefId != null &&
                             appList.Contains((long)n.RefId)).ToList();
            var getApplicationIdsRemoveListTest = notificationsTest.Select(n => n.RefId).ToList();
            appList.RemoveAll(s => getApplicationIdsRemoveListTest.Contains(s));

            string listOfDocument = "";
            foreach (var app in appList)
            {
                var getApp = GenService.GetById<Application>(app);

                var getDocumentCheckList = GenService.GetAll<DocumentCheckList>().Where(d => d.ApplicationId == app).ToList();
                foreach (var g in getDocumentCheckList)
                {
                    var getDocumentCheckListDetails = GenService.GetAll<DocumentCheckListDetail>()
                    .Where(d => d.DCLId == g.Id && d.IsRequired == true).ToList();
                    listOfDocument = string.Join(",", getDocumentCheckListDetails.Select(p => p.Name));
                }

                var AppRef = getApp.ApplicationNo;
                var userIdList = new List<long>();
                var userId = _user.GetUserIdByEmployeeId((long)getApp.RMId);
                var rmName = getApp.RMEmp.Person.FirstName + " " + getApp.RMEmp.Person.LastName;
                userIdList.Add(userId);
                Employee sentEmployee;
                string senderName = "";

                var appLogs = GenService.GetAll<Infrastructure.Models.ApplicationLog>()
               .Where(l => l.ApplicationId == getApp.Id &&
                  l.ToStage == ApplicationStage.UnderProcessAtOperations &&
                  l.Status == Infrastructure.EntityStatus.Active).FirstOrDefault();

              
                var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)getApp.RMId);
                if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                {
                    foreach (var supervisor in supervisorEmpIdList)
                    {
                        var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                        userIdList.Add(supervisorUserId);
                    }
                }
                if (appLogs != null)
                {
                    var getEmp = _user.GetEmployeeIdByUserId((long)appLogs.FromUserId);
                    if (getEmp != null)
                    {
                        sentEmployee = GenService.GetById<Employee>((long)getEmp);
                        senderName = sentEmployee.Person.FirstName + " " + sentEmployee.Person.LastName;
                    }
                    userIdList.Add(appLogs.FromUserId);
                }
                int i = 0;
                string tlName = "", bmName = "";
                foreach (var u in userIdList)
                {
                    if (i == 1)
                    {
                        var tlEmployeeId = _user.GetEmployeeIdByUserId((long)userIdList[1]);
                        var tlEmployee = GenService.GetById<Employee>((long)tlEmployeeId);
                        tlName = tlEmployee.Person.FirstName + " " + tlEmployee.Person.LastName;
                    }
                    if (i == 2)
                    {
                        var bmEmployeeId = _user.GetEmployeeIdByUserId((long)userIdList[2]);
                        var bmEmployee = GenService.GetById<Employee>((long)bmEmployeeId);
                        bmName = bmEmployee.Person.FirstName + " " + bmEmployee.Person.LastName;

                    }
                    i++;
                }
               
                    
                

                userIdList = userIdList.Distinct().ToList();

                foreach (var u in userIdList)
                {
                    var notification = new Notification
                    {
                        NotificationType = NotificationType.DocumentCollectionDateIsPastDueAsPerDCL,
                        RefId = app,
                        NotificationStatusType = NotificationStatusType.New
                    };
                    long FromUserId = 0;
                    if (appLogs != null)
                    {
                        FromUserId = appLogs.FromUserId;
                    }
                  
                        if (u == FromUserId)
                        {
                            notification.UserId = u;
                            //notification.Url = "/IPDC/Application/Application?applicationId=" + getApp.Id;
                            //var menu = _menu.GetMenuByUrl("/IPDC/Operations/LoanApplications");
                            //if (menu != null)
                            //{
                            //    notification.MenuId = menu.Id;
                            //    notification.MenuName = menu.DisplayName;
                            //}
                        }
                        else
                        {
                            notification.UserId = u;
                            notification.Url = "/IPDC/Application/Application?applicationId=" + getApp.Id;
                            var menu = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");
                            if (menu != null)
                            {
                                notification.MenuId = menu.Id;
                                notification.MenuName = menu.DisplayName;
                            }
                        }

                    notification.Message = "RM's Name: "
                        + rmName +
                        ", TL's Name: "
                        + tlName +
                        ", BM's Name: "
                        + bmName +
                        ", Operations Name: "
                        + senderName +
                        ", App Ref No: " + AppRef
                        + ", Document Name :" + listOfDocument;


                    SaveNotification(notification, false);
                }

            }
            #endregion
            //Update

            SaveChanges();

        }

        //Before 1 day deffered document collection as per DCL
        private void Before1DayDeferredDocumentCollectionNotificationsAsPerDCL()
        {
            var now = IPDCSystem.SystemDate;
            //var nowDate = now.Date;
            var nowDate = now.Date.AddDays(1);
            var getDCLExp = GenService.GetAll<DocumentCheckListException>()
                .Where(s => s.CollectionDate == nowDate &&
                            s.Status == Infrastructure.EntityStatus.Active &&
                            s.DCL.Status == Infrastructure.EntityStatus.Active &&
                            s.DCL.IsApproved != null &&
                            s.DCL.IsApproved == true &&
                            s.DCL.Application.Status == Infrastructure.EntityStatus.Active)
                .Select(s => new { DCLId = s.DCLId, ExceptionId = s.Id }).ToList();

            var dclList = new List<long>();
            if (getDCLExp.Count > 0)
                dclList.AddRange(getDCLExp.Select(s => s.DCLId).ToList());
            var getDCLDet = GenService.GetAll<DocumentCheckListDetail>()
                .Where(s => s.CollectionDate == nowDate &&
                            s.Status == Infrastructure.EntityStatus.Active &&
                            s.DCL.Status == Infrastructure.EntityStatus.Active &&
                            s.DCL.IsApproved != null &&
                            s.DCL.IsApproved == true &&
                            s.DCL.Application.Status == Infrastructure.EntityStatus.Active)
                .Select(s => new { DCLId = s.DCLId, DetailId = s.Id }).ToList();
            if (getDCLDet.Count > 0)
                dclList.AddRange(getDCLDet.Select(s => s.DCLId).ToList());
            dclList = dclList.Distinct().ToList();

            var notifications = GenService.GetAll<Notification>()
                   .Where(n => n.NotificationType == NotificationType.Day1BeforeScheduledDocumentCollectionDateAsPerDCL &&
                               n.Status == Infrastructure.EntityStatus.Active &&
                               n.RefId != null &&
                               dclList.Contains((long)n.RefId)).ToList();
            var getApplicationIdsRemoveList = notifications.Select(n => n.RefId).ToList();
            dclList.RemoveAll(s => getApplicationIdsRemoveList.Contains(s));

            //Update
            #region
            var appList = new List<long>();
            foreach (var dce in dclList)
            {
                var getDCLD = GenService.GetById<DocumentCheckList>(dce);

                appList.Add(getDCLD.Application.Id);
            }


            appList = appList.Distinct().ToList();

            var notificationsTest = GenService.GetAll<Notification>()
                 .Where(n => n.NotificationType == NotificationType.Day1BeforeScheduledDocumentCollectionDateAsPerDCL &&
                             n.Status == Infrastructure.EntityStatus.Active &&
                             n.RefId != null &&
                             appList.Contains((long)n.RefId)).ToList();
            var getApplicationIdsRemoveListTest = notificationsTest.Select(n => n.RefId).ToList();
            appList.RemoveAll(s => getApplicationIdsRemoveListTest.Contains(s));

            string listOfDocument = "";
            foreach (var app in appList)
            {
                var getApp = GenService.GetById<Application>(app);

                var getDocumentCheckList = GenService.GetAll<DocumentCheckList>().Where(d => d.ApplicationId == app).ToList();
                foreach (var g in getDocumentCheckList)
                {
                    var getDocumentCheckListDetails = GenService.GetAll<DocumentCheckListDetail>()
                    .Where(d => d.DCLId == g.Id && d.IsRequired == true).ToList();
                    listOfDocument = string.Join(",", getDocumentCheckListDetails.Select(p => p.Name));
                }

                var AppRef = getApp.ApplicationNo;
                var userIdList = new List<long>();
                var userList = new List<long>();
                var userId = _user.GetUserIdByEmployeeId((long)getApp.RMId);
                var rmName = getApp.RMEmp.Person.FirstName + " " + getApp.RMEmp.Person.LastName;
                userIdList.Add(userId);
                userList.Add(userId);
                Employee sentEmployee;
                string senderName = "";

                var appLogs = GenService.GetAll<Infrastructure.Models.ApplicationLog>()
               .Where(l => l.ApplicationId == getApp.Id &&
                  l.ToStage == ApplicationStage.UnderProcessAtOperations &&
                  l.Status == Infrastructure.EntityStatus.Active).FirstOrDefault();

                if (appLogs != null)
                {
                    var getEmp = _user.GetEmployeeIdByUserId((long)appLogs.FromUserId);
                    if (getEmp != null)
                    {
                        sentEmployee = GenService.GetById<Employee>((long)getEmp);
                        senderName = sentEmployee.Person.FirstName + " " + sentEmployee.Person.LastName;
                    }
                    userList.Add(appLogs.FromUserId);
                }
                var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)getApp.RMId);
                if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                {
                    foreach (var supervisor in supervisorEmpIdList)
                    {
                        var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                        userIdList.Add(supervisorUserId);
                    }
                }
                int i = 0;
                string tlName = "", bmName = "";
                foreach (var u in userIdList)
                {
                    if (i == 1)
                    {
                        var tlEmployeeId = _user.GetEmployeeIdByUserId((long)userIdList[1]);
                        var tlEmployee = GenService.GetById<Employee>((long)tlEmployeeId);
                        tlName = tlEmployee.Person.FirstName + " " + tlEmployee.Person.LastName;
                    }
                    if (i == 2)
                    {
                        var bmEmployeeId = _user.GetEmployeeIdByUserId((long)userIdList[2]);
                        var bmEmployee = GenService.GetById<Employee>((long)bmEmployeeId);
                        bmName = bmEmployee.Person.FirstName + " " + bmEmployee.Person.LastName;

                    }
                    i++;
                }
                userList = userList.Distinct().ToList();

                foreach (var u in userList)
                {
                    var notification = new Notification
                    {
                        NotificationType = NotificationType.DocumentCollectionDateIsOverdueFor1daysAsPerDCL,
                        RefId = app,
                        NotificationStatusType = NotificationStatusType.New
                    };

                    long FromUserId = 0;
                    if (appLogs != null)
                    {
                        FromUserId = appLogs.FromUserId;
                    }

                    if (u == FromUserId)
                    {
                        notification.UserId = u;
                        //notification.Url = "/IPDC/Application/Application?applicationId=" + getApp.Id;
                        //var menu = _menu.GetMenuByUrl("/IPDC/Operations/LoanApplications");
                        //if (menu != null)
                        //{
                        //    notification.MenuId = menu.Id;
                        //    notification.MenuName = menu.DisplayName;
                        //}
                    }
                    else
                    {
                        notification.UserId = u;
                        notification.Url = "/IPDC/Application/Application?applicationId=" + getApp.Id;
                        var menu = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");
                        if (menu != null)
                        {
                            notification.MenuId = menu.Id;
                            notification.MenuName = menu.DisplayName;
                        }
                    }
                    notification.Message = "RM's Name: "
                        + rmName +
                        ", TL's Name: "
                        + tlName +
                        ", BM's Name: "
                        + bmName +
                        ", Operations Name: "
                        + senderName +
                        ", App Ref No: " + AppRef
                        + ", Document Name :" + listOfDocument;


                    SaveNotification(notification, false);
                }

            }
            #endregion
            //Update

            SaveChanges();

        }

        //On scheduled day deffered document collection as per DCL
        private void OnScheduledDayDeferredDocumentCollectionNotificationsAsPerDCL()
        {
            var now = IPDCSystem.SystemDate;
            var nowDate = now.Date;

            var getDCLExp = GenService.GetAll<DocumentCheckListException>()
                .Where(s => s.CollectionDate == nowDate &&
                            s.Status == Infrastructure.EntityStatus.Active &&
                            s.DCL.Status == Infrastructure.EntityStatus.Active &&
                            s.DCL.IsApproved != null &&
                            s.DCL.IsApproved == true &&
                            s.DCL.Application.Status == Infrastructure.EntityStatus.Active)
                .Select(s => new { DCLId = s.DCLId, ExceptionId = s.Id }).ToList();

            var dclList = new List<long>();
            if (getDCLExp.Count > 0)
                dclList.AddRange(getDCLExp.Select(s => s.DCLId).ToList());
            var getDCLDet = GenService.GetAll<DocumentCheckListDetail>()
                .Where(s => s.CollectionDate == nowDate &&
                            s.Status == Infrastructure.EntityStatus.Active &&
                            s.DCL.Status == Infrastructure.EntityStatus.Active &&
                            s.DCL.IsApproved != null &&
                            s.DCL.IsApproved == true &&
                            s.DCL.Application.Status == Infrastructure.EntityStatus.Active)
                .Select(s => new { DCLId = s.DCLId, DetailId = s.Id }).ToList();
            if (getDCLDet.Count > 0)
                dclList.AddRange(getDCLDet.Select(s => s.DCLId).ToList());
            dclList = dclList.Distinct().ToList();

            var notifications = GenService.GetAll<Notification>()
                   .Where(n => n.NotificationType == NotificationType.OnDayScheduledDocumentCollectionAsPerDCL &&
                               n.Status == Infrastructure.EntityStatus.Active &&
                               n.RefId != null &&
                               dclList.Contains((long)n.RefId)).ToList();
            var getApplicationIdsRemoveList = notifications.Select(n => n.RefId).ToList();
            dclList.RemoveAll(s => getApplicationIdsRemoveList.Contains(s));

            //Update
            #region
            var appList = new List<long>();
            foreach (var dce in dclList)
            {
                var getDCLD = GenService.GetById<DocumentCheckList>(dce);

                appList.Add(getDCLD.Application.Id);
            }


            appList = appList.Distinct().ToList();

            var notificationsTest = GenService.GetAll<Notification>()
                 .Where(n => n.NotificationType == NotificationType.OnDayScheduledDocumentCollectionAsPerDCL &&
                             n.Status == Infrastructure.EntityStatus.Active &&
                             n.RefId != null &&
                             appList.Contains((long)n.RefId)).ToList();
            var getApplicationIdsRemoveListTest = notificationsTest.Select(n => n.RefId).ToList();
            appList.RemoveAll(s => getApplicationIdsRemoveListTest.Contains(s));

            string listOfDocument = "";
            foreach (var app in appList)
            {
                var getApp = GenService.GetById<Application>(app);

                var getDocumentCheckList = GenService.GetAll<DocumentCheckList>().Where(d => d.ApplicationId == app).ToList();
                foreach (var g in getDocumentCheckList)
                {
                    var getDocumentCheckListDetails = GenService.GetAll<DocumentCheckListDetail>()
                    .Where(d => d.DCLId == g.Id && d.IsRequired == true).ToList();
                    listOfDocument = string.Join(",", getDocumentCheckListDetails.Select(p => p.Name));
                }

                var AppRef = getApp.ApplicationNo;
                var userIdList = new List<long>();
                var userId = _user.GetUserIdByEmployeeId((long)getApp.RMId);
                var rmName = getApp.RMEmp.Person.FirstName + " " + getApp.RMEmp.Person.LastName;
                userIdList.Add(userId);
                Employee sentEmployee;
                string senderName = "";

                var appLogs = GenService.GetAll<Infrastructure.Models.ApplicationLog>()
               .Where(l => l.ApplicationId == getApp.Id &&
                  l.ToStage == ApplicationStage.UnderProcessAtOperations &&
                  l.Status == Infrastructure.EntityStatus.Active).FirstOrDefault();

              
                var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)getApp.RMId);
                if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                {
                    foreach (var supervisor in supervisorEmpIdList)
                    {
                        var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                        userIdList.Add(supervisorUserId);
                    }
                }
                int i = 0;
                string tlName = "", bmName = "";
                foreach (var u in userIdList)
                {
                    if (i == 1)
                    {
                        var tlEmployeeId = _user.GetEmployeeIdByUserId((long)userIdList[1]);
                        var tlEmployee = GenService.GetById<Employee>((long)tlEmployeeId);
                        tlName = tlEmployee.Person.FirstName + " " + tlEmployee.Person.LastName;
                    }
                    if (i == 2)
                    {
                        var bmEmployeeId = _user.GetEmployeeIdByUserId((long)userIdList[2]);
                        var bmEmployee = GenService.GetById<Employee>((long)bmEmployeeId);
                        bmName = bmEmployee.Person.FirstName + " " + bmEmployee.Person.LastName;

                    }
                    i++;
                }
                if (appLogs != null)
                {
                    var getEmp = _user.GetEmployeeIdByUserId((long)appLogs.FromUserId);
                    if (getEmp != null)
                    {
                        sentEmployee = GenService.GetById<Employee>((long)getEmp);
                        senderName = sentEmployee.Person.FirstName + " " + sentEmployee.Person.LastName;
                    }
                    userIdList.Add(appLogs.FromUserId);
                }

                userIdList = userIdList.Distinct().ToList();

                foreach (var u in userIdList)
                {
                    var notification = new Notification
                    {
                        NotificationType = NotificationType.OnDayScheduledDocumentCollectionAsPerDCL,
                        RefId = app,
                        NotificationStatusType = NotificationStatusType.New
                    };

                    long FromUserId = 0;
                    if (appLogs != null)
                    {
                        FromUserId = appLogs.FromUserId;
                    }

                    if (u == FromUserId)
                    {
                        notification.UserId = u;
                        //notification.Url = "/IPDC/Application/Application?applicationId=" + getApp.Id;
                        //var menu = _menu.GetMenuByUrl("/IPDC/Operations/LoanApplications");
                        //if (menu != null)
                        //{
                        //    notification.MenuId = menu.Id;
                        //    notification.MenuName = menu.DisplayName;
                        //}
                    }
                    else
                    {
                        notification.UserId = u;
                        notification.Url = "/IPDC/Application/Application?applicationId=" + getApp.Id;
                        var menu = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");
                        if (menu != null)
                        {
                            notification.MenuId = menu.Id;
                            notification.MenuName = menu.DisplayName;
                        }
                    }
                    notification.Message = "RM's Name: "
                        + rmName +
                        ", TL's Name: "
                        + tlName +
                        ", BM's Name: "
                        + bmName +
                        ", Operations Name: "
                        + senderName +
                        ", App Ref No: " + AppRef
                        + ", Document Name :" + listOfDocument;


                    SaveNotification(notification, false);
                }

            }
            #endregion
            //Update

            SaveChanges();

        }

        //On scheduled day deffered document collection as per Application
        private void OnScheduledDayDeferredDocumentCollectionNotificationsAsPerApplication()
        {
            var now = IPDCSystem.SystemDate;
            var nowDate = now.Date;
            var getApplication = GenService.GetAll<AppDocChecklist>().Where(s => s.SubmissionDeadline == nowDate && s.DocumentStatus == Infrastructure.DocumentStatus.Deferred).Select(s => s.Id).ToList();

            var notifications = GenService.GetAll<Notification>()
                .Where(n => n.NotificationType == NotificationType.OnDayScheduledDocumentCollectionAsPerApplication &&
                            n.Status == Infrastructure.EntityStatus.Active &&
                            n.RefId != null &&
                            getApplication.Contains((long)n.RefId)).ToList();
            var getApplicationIdsRemoveList = notifications.Select(n => n.RefId).ToList();
            getApplication.RemoveAll(s => getApplicationIdsRemoveList.Contains(s));



            foreach (var app in getApplication)
            {
                var notification = new Notification
                {
                    NotificationType = NotificationType.OnDayScheduledDocumentCollectionAsPerApplication,
                    RefId = app,
                    NotificationStatusType = NotificationStatusType.New
                };

                var applicationId = GenService.GetById<Finix.IPDC.Infrastructure.Models.AppDocChecklist>(app);
                var application = GenService.GetById<Finix.IPDC.Infrastructure.Models.Application>((long)applicationId.ApplicationId);
                var AppRef = application.ApplicationNo;
                var userIdList = new List<long>();
                var userId = _user.GetUserIdByEmployeeId((long)application.RMId);
                userIdList.Add(userId);

                var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)application.RMId);
                if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                {
                    foreach (var supervisor in supervisorEmpIdList)
                    {
                        var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                        userIdList.Add(supervisorUserId);
                    }
                }
                foreach (var u in supervisorEmpIdList)
                {
                    notification.UserId = u;
                    notification.Url = "/IPDC/Application/SearchApplication?AppId=" + app;
                    var menu = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");
                    if (menu != null)
                    {
                        notification.MenuId = menu.Id;
                        notification.MenuName = menu.DisplayName;
                    }
                    notification.Message = "Upcoming Documents collection date is today," + " App.Ref.No: " + AppRef;
                    SaveNotification(notification, false);
                }


            }
            SaveChanges();

        }
        #region
        //1 Day before scheduled deffered document collection as per Application
        private void Before1DayDeferredDocumentCollectionNotificationsAsPerApplication()
        {
            var now = IPDCSystem.SystemDate;
            //var nowDate = now.Date;
            var nowDate = now.Date.AddDays(1);

            var getApplication = GenService.GetAll<AppDocChecklist>().Where(s => s.SubmissionDeadline == nowDate && s.DocumentStatus == Infrastructure.DocumentStatus.Deferred).Select(s => s.Id).ToList();

            var notifications = GenService.GetAll<Notification>()
                .Where(n => n.NotificationType == NotificationType.Day1BeforeScheduledDocumentCollectionDateAsPerApplication &&
                            n.Status == Infrastructure.EntityStatus.Active &&
                            n.RefId != null &&
                            getApplication.Contains((long)n.RefId)).ToList();
            var getApplicationIdsRemoveList = notifications.Select(n => n.RefId).ToList();
            getApplication.RemoveAll(s => getApplicationIdsRemoveList.Contains(s));

            foreach (var app in getApplication)
            {
                var notification = new Notification
                {
                    NotificationType = NotificationType.Day1BeforeScheduledDocumentCollectionDateAsPerApplication,
                    RefId = app,
                    NotificationStatusType = NotificationStatusType.New
                };

                var ApplicationId = GenService.GetById<Finix.IPDC.Infrastructure.Models.AppDocChecklist>(app);
                var Application = GenService.GetById<Finix.IPDC.Infrastructure.Models.Application>((long)ApplicationId.ApplicationId);
                var AppRef = Application.ApplicationNo;
                var userIdList = new List<long>();
                var userId = _user.GetUserIdByEmployeeId((long)Application.RMId);
                userIdList.Add(userId);

                foreach (var u in userIdList)
                {
                    notification.UserId = u;
                    notification.Url = "/IPDC/Application/SearchApplication?AppId=" + app;
                    var menu = _menu.GetMenuByUrl("/IPDC/Application/SearchApplication");
                    if (menu != null)
                    {
                        notification.MenuId = menu.Id;
                        notification.MenuName = menu.DisplayName;
                    }
                    notification.Message = "Upcoming Documents collection date is tomorrow," + " App.Ref.No: " + AppRef;
                    SaveNotification(notification, false);
                }


            }
            SaveChanges();

        }
        #endregion
        private void CreateSomeOtherNotifications(int minutes)
        {
            var now = IPDCSystem.SystemDate;
            var maxTime = now.AddMinutes(minutes);

            // get respective entities having followup-time <= maxTime and followup-date>=today.
            // say we found 1,2,3... 10
            // check the notificaions for respective notificaiton type and with these ids
            //if found then exclude those ids.
            //register notificaitons for the remaining ids


        }

        #region lead missed auto notifications
        private void RmMissedLeadFollowupBy1Day()
        {
            var now = IPDCSystem.SystemDate;
            var minDate = now.Date.AddDays(-1);
            var salesLeadIds = GenService.GetAll<SalesLead>()
                .Where(s => s.FollowupTime >= minDate &&
                            s.FollowupTime <= now &&
                            s.Status == Infrastructure.EntityStatus.Active &&
                            (s.LeadStatus != LeadStatus.Unsuccessful || s.LeadStatus != LeadStatus.Prospective))
                .Select(s => s.Id).ToList();
            var notifications = GenService.GetAll<Notification>()
                .Where(n => n.NotificationType == NotificationType.RMMissedLeadFollowupBy1Day &&
                            n.Status == Infrastructure.EntityStatus.Active &&
                            n.RefId != null &&
                            salesLeadIds.Contains((long)n.RefId)).ToList();
            var salesLeadIdsRemoveList = notifications.Select(n => n.RefId).ToList();
            salesLeadIds.RemoveAll(s => salesLeadIdsRemoveList.Contains(s));

            foreach (var lead in salesLeadIds)
            {
                var notification = new Notification
                {
                    NotificationType = NotificationType.RMMissedLeadFollowupBy1Day,
                    RefId = lead,
                    NotificationStatusType = NotificationStatusType.New
                };
                var leadAssignment = GenService.GetAll<SalesLeadAssignment>()
                        .Where(s => s.SalesLeadId == lead && s.Status == Infrastructure.EntityStatus.Active && s.AssignedToEmpId != null)
                        .OrderByDescending(s => s.Id)
                        .FirstOrDefault();
                if (leadAssignment != null)
                {
                    long parentEmpId = _employee.GetEmployeeSupervisorEmpId((long)leadAssignment.AssignedToEmpId);
                    long? parentUserId = _user.GetUserByEmployeeId(parentEmpId).Id;

                    notification.UserId = parentUserId;
                }

                notification.Url = "/IPDC/SalesLead/SalesLeadEntry?leadEntryId=" + lead;
                var menu = _menu.GetMenuByUrl("/IPDC/SalesLead/SalesLeadEntry");
                if (menu != null)
                {
                    notification.MenuId = menu.Id;
                    notification.MenuName = menu.DisplayName;
                }
                notification.Message = UiUtil.GetDisplayName(notification.NotificationType) +
                    " - RM Name: " +
                    leadAssignment.AssignedToEmp.Person.FirstName + " " +
                    leadAssignment.AssignedToEmp.Person.LastName + " " +
                    "- Customer Name: " +
                    leadAssignment.SalesLead.CustomerName;

                SaveNotification(notification, false);
            }
            SaveChanges();
        }
        private void RmMissedLeadFollowupBy2Days()
        {
            var now = IPDCSystem.SystemDate;
            var minDate = now.Date.AddDays(-2);
            var salesLeadIds = GenService.GetAll<SalesLead>()
                .Where(s => s.FollowupTime <= minDate &&
                            s.Status == Infrastructure.EntityStatus.Active &&
                            (s.LeadStatus != LeadStatus.Unsuccessful || s.LeadStatus != LeadStatus.Prospective))
                .Select(s => s.Id).ToList();
            var notifications = GenService.GetAll<Notification>()
                .Where(n => n.NotificationType == NotificationType.RMMissesLeadFollowupBy2Days &&
                            n.Status == Infrastructure.EntityStatus.Active &&
                            n.RefId != null &&
                            salesLeadIds.Contains((long)n.RefId)).ToList();
            var salesLeadIdsRemoveList = notifications.Select(n => n.RefId).ToList();
            salesLeadIds.RemoveAll(s => salesLeadIdsRemoveList.Contains(s));

            foreach (var lead in salesLeadIds)
            {
                var recipientUserIds = new List<long>();

                var leadAssignment = GenService.GetAll<SalesLeadAssignment>()
                        .Where(s => s.SalesLeadId == lead && s.Status == Infrastructure.EntityStatus.Active && s.AssignedToEmpId != null)
                        .OrderByDescending(s => s.Id)
                        .FirstOrDefault();
                if (leadAssignment != null)
                {
                    var supervisorEmpIdList = _organogram.GetEmployeeIdsOfSupervisorsOfSameOffice((long)leadAssignment.AssignedToEmpId);
                    if (supervisorEmpIdList != null && supervisorEmpIdList.Count > 0)
                    {
                        foreach (var supervisor in supervisorEmpIdList)
                        {
                            var supervisorUserId = _user.GetUserIdByEmployeeId(supervisor);
                            recipientUserIds.Add(supervisorUserId);
                        }
                    }
                }

                var recipients = GenService.GetAll<NotificationSettings>()
                    .Where(n => n.Status == Infrastructure.EntityStatus.Active &&
                                n.NotificationType == NotificationType.RMMissesLeadFollowupBy2Days)
                    .ToList();
                var recipientEmpIds = new List<long>();
                if (recipients.Where(r => r.RoleId != null).Any())
                {
                    var recipientRoles = recipients.Where(r => r.RoleId != null).Select(r => r.RoleId).ToList();
                    foreach (var role in recipientRoles)
                    {
                        var empIds = _employee.GetEmployeeIdsByRoleId((long)role);
                        if (empIds != null)
                            recipientEmpIds.AddRange(empIds);
                    }
                }
                if (recipientUserIds != null && recipientUserIds.Count > 0)
                {
                    foreach (var user in recipientUserIds)
                    {
                        var notification = new Notification
                        {
                            NotificationType = NotificationType.RMMissesLeadFollowupBy2Days,
                            RefId = lead,
                            NotificationStatusType = NotificationStatusType.New,
                            Url = "/IPDC/SalesLead/SalesLeadEntry?leadEntryId=" + lead,
                            UserId = user
                        };
                        var menu = _menu.GetMenuByUrl("/IPDC/SalesLead/SalesLeadEntry");
                        if (menu != null)
                        {
                            notification.MenuId = menu.Id;
                            notification.MenuName = menu.DisplayName;
                        }

                        notification.Message = UiUtil.GetDisplayName(notification.NotificationType) +
                        " - RM Name: " +
                        leadAssignment.AssignedToEmp.Person.FirstName + " " +
                        leadAssignment.AssignedToEmp.Person.LastName + " " +
                        "- Customer Name: " +
                        leadAssignment.SalesLead.CustomerName;



                        SaveNotification(notification, false);
                    }
                }

            }
            SaveChanges();
        }
        #endregion

        #region Application approval missed notifiations
        private void ApplicationApprovalPendingTL2Days()
        {
            var now = IPDCSystem.SystemDate;
            var minDate = now.Date.AddDays(-2);

            var applicationIds = GenService.GetAll<Infrastructure.Models.Application>()
                .Where(a => a.ApplicationStage == ApplicationStage.SentToTL && a.Status == Infrastructure.EntityStatus.Active)
                .Select(a => a.Id).ToList();

            var notifications = GenService.GetAll<Notification>()
                .Where(n => n.NotificationType == NotificationType.ApplicationWaitingForApprovalByTLForMore2Days &&
                            n.Status == Infrastructure.EntityStatus.Active &&
                            n.RefId != null &&
                            applicationIds.Contains((long)n.RefId)).ToList();
            var applicationIdsRemoveList = notifications.Select(n => n.RefId).ToList();
            applicationIds.RemoveAll(s => applicationIdsRemoveList.Contains(s));

            var appLogs = GenService.GetAll<Infrastructure.Models.ApplicationLog>()
                .Where(l => applicationIds.Contains(l.ApplicationId) &&
                            l.ToStage == ApplicationStage.SentToTL &&
                            l.CreateDate <= minDate &&
                            l.Status == Infrastructure.EntityStatus.Active)
                .OrderBy(l => l.CreateDate).ToList();

            foreach (var log in appLogs.Where(l=>l.Application.CurrentHoldingEmpId != null))
            {
                long parentEmpId = _employee.GetEmployeeSupervisorEmpId((long)log.Application.CurrentHoldingEmpId);
                long? parentUserId = _user.GetUserByEmployeeId(parentEmpId).Id;

                var notification = new Notification
                {
                    NotificationType = NotificationType.ApplicationWaitingForApprovalByTLForMore2Days,
                    RefId = log.ApplicationId,
                    NotificationStatusType = NotificationStatusType.New,
                    Url = "/IPDC/Application/Application?applicationId=" + log.ApplicationId,
                    UserId = parentUserId
                };
                var menu = _menu.GetMenuByUrl("/IPDC/Application/Application");
                if (menu != null)
                {
                    notification.MenuId = menu.Id;
                    notification.MenuName = menu.DisplayName;
                }

                notification.Message = UiUtil.GetDisplayName(notification.NotificationType) +
                    " - RM Name: " +
                    log.Application.RMEmp.Person.FirstName + " " +
                    log.Application.RMEmp.Person.LastName + " " +
                    " - TL Name: " +
                    log.Application.CurrentHoldingEmp.Person.FirstName + " " +
                    log.Application.CurrentHoldingEmp.Person.LastName + " " +
                    "- Application No.: " + " " +
                    log.Application.ApplicationNo + " " +
                    "- Application Title: " +
                    log.Application.AccountTitle;

                SaveNotification(notification, false);
            }


            SaveChanges();
        }
        private void ApplicationApprovalPendingTL3Days()
        {
            var now = IPDCSystem.SystemDate;
            var minDate = now.Date.AddDays(-3);

            var applicationIds = GenService.GetAll<Infrastructure.Models.Application>()
                .Where(a => a.ApplicationStage == ApplicationStage.SentToTL && a.Status == Infrastructure.EntityStatus.Active)
                .Select(a => a.Id).ToList();

            var notifications = GenService.GetAll<Notification>()
                .Where(n => n.NotificationType == NotificationType.ApplicationWaitingForApprovalByTLForMore3Days &&
                            n.Status == Infrastructure.EntityStatus.Active &&
                            n.RefId != null &&
                            applicationIds.Contains((long)n.RefId)).ToList();
            var applicationIdsRemoveList = notifications.Select(n => n.RefId).ToList();
            applicationIds.RemoveAll(s => applicationIdsRemoveList.Contains(s));

            var appLogs = GenService.GetAll<ApplicationLog>()
                .Where(l => applicationIds.Contains(l.ApplicationId) &&
                            l.ToStage == ApplicationStage.SentToTL &&
                            l.CreateDate <= minDate &&
                            l.Status == EntityStatus.Active)
                .OrderBy(l => l.CreateDate).ToList();

            foreach (var log in appLogs.Where(l=>l.Application.CurrentHoldingEmpId != null))
            {
                var recipientUserIds = new List<long>();
                long parentEmpId = _employee.GetEmployeeSupervisorEmpId((long)log.Application.CurrentHoldingEmpId);
                long? parentUserId = _user.GetUserByEmployeeId(parentEmpId).Id;
                if (parentUserId != null)
                    recipientUserIds.Add((long)parentUserId);

                var recipients = GenService.GetAll<NotificationSettings>()
                    .Where(n => n.Status == Infrastructure.EntityStatus.Active &&
                                n.NotificationType == NotificationType.ApplicationWaitingForApprovalByTLForMore3Days)
                    .ToList();
                var recipientEmpIds = new List<long>();
                if (recipients.Where(r => r.RoleId != null).Any())
                {
                    var recipientRoles = recipients.Where(r => r.RoleId != null).Select(r => r.RoleId).ToList();
                    foreach (var role in recipientRoles)
                    {
                        var empIds = _employee.GetEmployeeIdsByRoleId((long)role);
                        if (empIds != null)
                            recipientEmpIds.AddRange(empIds);
                    }
                }
                foreach (var recipientEmp in recipientEmpIds)
                {
                    long? userid = _user.GetUserByEmployeeId(recipientEmp).Id;
                    if (userid != null)
                        recipientUserIds.Add((long)userid);
                }
                recipientUserIds = recipientUserIds.Distinct().ToList();
                foreach (var user in recipientUserIds)
                {
                    var notification = new Notification
                    {
                        NotificationType = NotificationType.ApplicationWaitingForApprovalByTLForMore3Days,
                        RefId = log.ApplicationId,
                        NotificationStatusType = NotificationStatusType.New,
                        Url = "/IPDC/Application/Application?applicationId=" + log.ApplicationId,
                        UserId = user
                    };
                    var menu = _menu.GetMenuByUrl("/IPDC/Application/Application");
                    if (menu != null)
                    {
                        notification.MenuId = menu.Id;
                        notification.MenuName = menu.DisplayName;
                    }

                    notification.Message = UiUtil.GetDisplayName(notification.NotificationType) +
                        " - RM Name: " +
                        log.Application.RMEmp.Person.FirstName + " " +
                        log.Application.RMEmp.Person.LastName + " " +
                        " - TL Name: " +
                        log.Application.CurrentHoldingEmp.Person.FirstName + " " +
                        log.Application.CurrentHoldingEmp.Person.LastName + " " +
                        "- Application No.: " + " " +
                        log.Application.ApplicationNo + " " +
                        "- Application Title: " +
                        log.Application.AccountTitle;

                    SaveNotification(notification, false);
                }

            }


            SaveChanges();
        }
        private void ApplicationApprovalPendingBM3Days()
        {
            var now = IPDCSystem.SystemDate;
            var minDate = now.Date.AddDays(-3);

            var applicationIds = GenService.GetAll<Application>()
                .Where(a => a.ApplicationStage == ApplicationStage.SentToBM && a.Status == EntityStatus.Active)
                .Select(a => a.Id).ToList();

            var notifications = GenService.GetAll<Notification>()
                .Where(n => n.NotificationType == NotificationType.ApplicationWaitingForApprovalByBMForMore3Days &&
                            n.Status == EntityStatus.Active &&
                            n.RefId != null &&
                            applicationIds.Contains((long)n.RefId)).ToList();
            var applicationIdsRemoveList = notifications.Select(n => n.RefId).ToList();
            applicationIds.RemoveAll(s => applicationIdsRemoveList.Contains(s));

            var appLogs = GenService.GetAll<ApplicationLog>()
                .Where(l => applicationIds.Contains(l.ApplicationId) &&
                            l.ToStage == ApplicationStage.SentToTL &&
                            l.CreateDate <= minDate &&
                            l.Status == EntityStatus.Active)
                .OrderBy(l => l.CreateDate).ToList();

            foreach (var log in appLogs)
            {
                var recipientUserIds = new List<long>();
                long parentEmpId = _employee.GetEmployeeSupervisorEmpId((long)log.Application.CurrentHoldingEmpId);
                var parentUser = _user.GetUserByEmployeeId(parentEmpId);
                if (parentUser != null)
                    recipientUserIds.Add((long)parentUser.Id);

                var recipients = GenService.GetAll<NotificationSettings>()
                    .Where(n => n.Status == Infrastructure.EntityStatus.Active &&
                                n.NotificationType == NotificationType.ApplicationWaitingForApprovalByBMForMore3Days)
                    .ToList();
                var recipientEmpIds = new List<long>();
                if (recipients.Where(r => r.RoleId != null).Any())
                {
                    var recipientRoles = recipients.Where(r => r.RoleId != null).Select(r => r.RoleId).ToList();
                    foreach (var role in recipientRoles)
                    {
                        var empIds = _employee.GetEmployeeIdsByRoleId((long)role);
                        if (empIds != null)
                            recipientEmpIds.AddRange(empIds);
                    }
                }
                foreach (var recipientEmp in recipientEmpIds)
                {
                    long? userid = _user.GetUserByEmployeeId(recipientEmp).Id;
                    if (userid != null)
                        recipientUserIds.Add((long)userid);
                }
                recipientUserIds = recipientUserIds.Distinct().ToList();
                foreach (var user in recipientUserIds)
                {
                    var notification = new Notification
                    {
                        NotificationType = NotificationType.ApplicationWaitingForApprovalByBMForMore3Days,
                        RefId = log.ApplicationId,
                        NotificationStatusType = NotificationStatusType.New,
                        Url = "/IPDC/Application/Application?applicationId=" + log.ApplicationId,
                        UserId = user
                    };
                    var menu = _menu.GetMenuByUrl("/IPDC/Application/Application");
                    if (menu != null)
                    {
                        notification.MenuId = menu.Id;
                        notification.MenuName = menu.DisplayName;
                    }

                    notification.Message = UiUtil.GetDisplayName(notification.NotificationType) +
                        " - RM Name: " +
                        log.Application.RMEmp.Person.FirstName + " " +
                        log.Application.RMEmp.Person.LastName + " " +
                        " - TL Name: " +
                        log.Application.CurrentHoldingEmp.Person.FirstName + " " +
                        log.Application.CurrentHoldingEmp.Person.LastName + " " +
                        "- Application No.: " + " " +
                        log.Application.ApplicationNo + " " +
                        "- Application Title: " +
                        log.Application.AccountTitle;

                    SaveNotification(notification, false);
                }

            }


            SaveChanges();
        }
        private void ApplicationPendingCRM1Day()
        {
            var now = IPDCSystem.SystemDate;
            var minDate = now.Date.AddDays(-1);

            var applicationIds = GenService.GetAll<Application>()
                .Where(a => a.ApplicationStage == ApplicationStage.SentToCRM && a.Status == EntityStatus.Active)
                .Select(a => a.Id).ToList();

            var notifications = GenService.GetAll<Notification>()
                .Where(n => n.NotificationType == NotificationType.ApplicationReceivedFromBMAndUnassignedFor1Days &&
                            n.Status == EntityStatus.Active &&
                            n.RefId != null &&
                            applicationIds.Contains((long)n.RefId)).ToList();
            var applicationIdsRemoveList = notifications.Select(n => n.RefId).ToList();
            applicationIds.RemoveAll(s => applicationIdsRemoveList.Contains(s));

            var appLogs = GenService.GetAll<ApplicationLog>()
                .Where(l => applicationIds.Contains(l.ApplicationId) &&
                            l.ToStage == ApplicationStage.SentToCRM &&
                            l.CreateDate <= minDate &&
                            l.Status == EntityStatus.Active)
                .OrderBy(l => l.CreateDate).ToList();

            foreach (var log in appLogs)
            {
                var recipientUserIds = new List<long>();
                //long parentEmpId = _employee.GetEmployeeSupervisorEmpId((long)log.Application.CurrentHoldingEmpId);
                //long? parentUserId = _user.GetUserByEmployeeId(parentEmpId).Id;
                //if (parentUserId != null)
                //    recipientUserIds.Add((long)parentUserId);

                var recipients = GenService.GetAll<NotificationSettings>()
                    .Where(n => n.Status == EntityStatus.Active &&
                                n.NotificationType == NotificationType.ApplicationReceivedFromBMAndUnassignedFor1Days)
                    .ToList();
                var recipientEmpIds = new List<long>();
                if (recipients.Where(r => r.RoleId != null).Any())
                {
                    var recipientRoles = recipients.Where(r => r.RoleId != null).Select(r => r.RoleId).ToList();
                    foreach (var role in recipientRoles)
                    {
                        var empIds = _employee.GetEmployeeIdsByRoleId((long)role);
                        if (empIds != null)
                            recipientEmpIds.AddRange(empIds);
                    }
                }
                foreach (var recipientEmp in recipientEmpIds)
                {
                    long? userid = _user.GetUserByEmployeeId(recipientEmp).Id;
                    if (userid != null)
                        recipientUserIds.Add((long)userid);
                }
                recipientUserIds = recipientUserIds.Distinct().ToList();
                foreach (var user in recipientUserIds)
                {
                    var notification = new Notification
                    {
                        NotificationType = NotificationType.ApplicationReceivedFromBMAndUnassignedFor1Days,
                        RefId = log.ApplicationId,
                        NotificationStatusType = NotificationStatusType.New,
                        //Url = "/IPDC/Application/Application?applicationId=" + log.ApplicationId,
                        UserId = user
                    };
                    //var menu = _menu.GetMenuByUrl("/IPDC/Application/Application");
                    //if (menu != null)
                    //{
                    //    notification.MenuId = menu.Id;
                    //    notification.MenuName = menu.DisplayName;
                    //}

                    notification.Message = UiUtil.GetDisplayName(notification.NotificationType) +
                        " - RM Name: " +
                        log.Application.RMEmp.Person.FirstName + " " +
                        log.Application.RMEmp.Person.LastName + " " +
                        " - TL Name: " +
                        log.Application.CurrentHoldingEmp.Person.FirstName + " " +
                        log.Application.CurrentHoldingEmp.Person.LastName + " " +
                        "- Application No.: " + " " +
                        log.Application.ApplicationNo + " " +
                        "- Application Title: " +
                        log.Application.AccountTitle;

                    SaveNotification(notification, false);
                }

            }


            SaveChanges();
        }
        private void ApplicationPendingOperations1Day()
        {
            var now = IPDCSystem.SystemDate;
            var minDate = now.Date.AddDays(-1);

            var applicationIds = GenService.GetAll<Application>()
                .Where(a => a.ApplicationStage == ApplicationStage.SentToOperations && a.Status == EntityStatus.Active)
                .Select(a => a.Id).ToList();

            var notifications = GenService.GetAll<Notification>()
                .Where(n => n.NotificationType == NotificationType.ApplicationReceivedFromCRMAndUnassignedFor1Days &&
                            n.Status == EntityStatus.Active &&
                            n.RefId != null &&
                            applicationIds.Contains((long)n.RefId)).ToList();
            var applicationIdsRemoveList = notifications.Select(n => n.RefId).ToList();
            applicationIds.RemoveAll(s => applicationIdsRemoveList.Contains(s));

            var appLogs = GenService.GetAll<ApplicationLog>()
                .Where(l => applicationIds.Contains(l.ApplicationId) &&
                            l.ToStage == ApplicationStage.SentToOperations &&
                            l.CreateDate <= minDate &&
                            l.Status == EntityStatus.Active)
                .OrderBy(l => l.CreateDate).ToList();

            foreach (var log in appLogs)
            {
                var recipientUserIds = new List<long>();
                //long parentEmpId = _employee.GetEmployeeSupervisorEmpId((long)log.Application.CurrentHoldingEmpId);
                //long? parentUserId = _user.GetUserByEmployeeId(parentEmpId).Id;
                //if (parentUserId != null)
                //    recipientUserIds.Add((long)parentUserId);

                var recipients = GenService.GetAll<NotificationSettings>()
                    .Where(n => n.Status == EntityStatus.Active &&
                                n.NotificationType == NotificationType.ApplicationReceivedFromCRMAndUnassignedFor1Days)
                    .ToList();
                var recipientEmpIds = new List<long>();
                if (recipients.Where(r => r.RoleId != null).Any())
                {
                    var recipientRoles = recipients.Where(r => r.RoleId != null).Select(r => r.RoleId).ToList();
                    foreach (var role in recipientRoles)
                    {
                        var empIds = _employee.GetEmployeeIdsByRoleId((long)role);
                        if (empIds != null)
                            recipientEmpIds.AddRange(empIds);
                    }
                }
                foreach (var recipientEmp in recipientEmpIds)
                {
                    long? userid = _user.GetUserByEmployeeId(recipientEmp).Id;
                    if (userid != null)
                        recipientUserIds.Add((long)userid);
                }
                recipientUserIds = recipientUserIds.Distinct().ToList();
                foreach (var user in recipientUserIds)
                {
                    var notification = new Notification
                    {
                        NotificationType = NotificationType.ApplicationReceivedFromCRMAndUnassignedFor1Days,
                        RefId = log.ApplicationId,
                        NotificationStatusType = NotificationStatusType.New,
                        //Url = "/IPDC/Application/Application?applicationId=" + log.ApplicationId,
                        UserId = user
                    };
                    //var menu = _menu.GetMenuByUrl("/IPDC/Application/Application");
                    //if (menu != null)
                    //{
                    //    notification.MenuId = menu.Id;
                    //    notification.MenuName = menu.DisplayName;
                    //}

                    notification.Message = UiUtil.GetDisplayName(notification.NotificationType) +
                        " - RM Name: " +
                        log.Application.RMEmp.Person.FirstName + " " +
                        log.Application.RMEmp.Person.LastName + " " +
                        " - TL Name: " +
                        log.Application.CurrentHoldingEmp.Person.FirstName + " " +
                        log.Application.CurrentHoldingEmp.Person.LastName + " " +
                        "- Application No.: " + " " +
                        log.Application.ApplicationNo + " " +
                        "- Application Title: " +
                        log.Application.AccountTitle;

                    SaveNotification(notification, false);
                }

            }


            SaveChanges();
        }
        #endregion

        #endregion

    }
    public static class N
    {
        static List<MenuDto> _menus;
        public static void CreateNotificationForService(NotificationType type, long objId)
        {
            var facade = new NotificationFacade();
            facade.SaveNotification(new Notification
            {
                NotificationType = type,
                RefId = objId,
                NotificationStatusType = NotificationStatusType.ServiceNew
            });
        }
        public static void ProcessNotifications()
        {
            var facade = new NotificationFacade();
            var notificationsForService = facade.GetNotificationsForService();
            foreach (var n in notificationsForService)
            {
                facade.CreateNotifications(n.NotificationType, n.RefId);
                n.NotificationStatusType = NotificationStatusType.ServiceDone;
                facade.SaveNotification(n, false);
            }
            facade.SaveChanges();
            facade.CreateNotificationsByMonitoring();

            //update cached menus
            _menus = new MenuFacade().GetMenus();
        }
        public static List<MenuDto> Menus
        {
            get
            {
                if (_menus == null)
                    _menus = new MenuFacade().GetMenus();
                return _menus;
            }
        }
    }
}
