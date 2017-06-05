using AutoMapper;
using Finix.IPDC.DTO;
using Finix.IPDC.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Finix.IPDC.Infrastructure;
using PagedList;
using Finix.Auth.Facade;

namespace Finix.IPDC.Facade
{
    public class MessagingFacade : BaseFacade
    {
        private SequencerFacade _sequencer = new SequencerFacade();
        private readonly UserFacade _user = new UserFacade();
        private readonly EmployeeFacade _employee = new EmployeeFacade();
        public ResponseDto SaveNewMessage(IPDCMessagingDto dto, long userId)
        {
            var employeeId = _user.GetEmployeeIdByUserId(userId);
            var fromDesigid = _employee.GetOfficeDesignationIdOfEmployee(employeeId);
            var response = new ResponseDto();
            if (dto.ToEmpId != null)
            {
                var toDesigId = _employee.GetOfficeDesignationIdOfEmployee((long)dto.ToEmpId);
                try
                {
                    var entity = new IPDCMessaging();
                    if (dto.Id != null && dto.Id > 0)
                    {
                        entity = GenService.GetById<IPDCMessaging>((long)dto.Id);
                        if (entity.FromEmpId != null)
                        {
                            var fromEmpId = _employee.GetEmployeeByEmployeeId((long)entity.FromEmpId).Id;
                            var fromDraftDesId = _employee.GetOfficeDesignationIdOfEmployee((long)fromEmpId);
                            if (entity.ToEmpId != null)
                            {
                                var toDraftDesId = _employee.GetOfficeDesignationIdOfEmployee((long)entity.ToEmpId);
                                entity = Mapper.Map(dto, entity);
                                entity.FromEmpId = fromEmpId;
                                entity.FromOfficeDesignationSettingId = fromDraftDesId;
                                entity.ToOfficeDesignationSettingId = toDraftDesId;
                            }
                        }
                        entity.Status = EntityStatus.Active;
                        entity.EditDate = DateTime.Now;
                        entity.EditedBy = userId;
                        GenService.Save(entity);
                      
                            N.CreateNotificationForService(NotificationType.MessageSentFromCRMOrOpsToAnyUser, (long)entity.Id);
                        
                        response.Success = true;
                        response.Message = "Message Send Successfully";
                    }
                    else
                    {
                        entity = Mapper.Map<IPDCMessaging>(dto);
                        entity.FromEmpId = employeeId;
                        //entity.ToEmpId 
                        entity.FromOfficeDesignationSettingId = fromDesigid;
                        entity.ToOfficeDesignationSettingId = toDesigId;
                        entity.Status = EntityStatus.Active;
                        entity.CreatedBy = userId;
                        entity.CreateDate = DateTime.Now;
                        GenService.Save(entity);

                        N.CreateNotificationForService(NotificationType.MessageSentFromCRMOrOpsToAnyUser, (long)entity.Id);

                        response.Id = entity.Id;
                        response.Success = true;
                        response.Message = "New Message Sent Successfully";
                    }
                }
                catch (Exception ex)
                {
                    response.Message = "Error Message : " + ex;
                }
            }
            return response;
        }
        public ResponseDto SaveReplyMessage(IPDCMessagingDto dto, long userId)
        {
            var employeeId = _user.GetEmployeeIdByUserId(userId);
            var fromDesigid = _employee.GetOfficeDesignationIdOfEmployee(employeeId);
            var response = new ResponseDto();
            if (dto.FromEmpId != null)
            {
                var toemployeeId = _employee.GetEmployeeByEmployeeId((long)dto.FromEmpId).Id;
                try
                {
                    var toDesigId = _employee.GetOfficeDesignationIdOfEmployee((long)toemployeeId);
                    var entity = Mapper.Map<IPDCMessaging>(dto);
                    entity.ToEmpId = toemployeeId;
                    entity.FromEmpId = employeeId;
                    entity.FromOfficeDesignationSettingId = fromDesigid;
                    entity.ToOfficeDesignationSettingId = toDesigId;
                    entity.Status = EntityStatus.Active;
                    entity.CreatedBy = userId;
                    entity.CreateDate = DateTime.Now;
                    GenService.Save(entity);
                    N.CreateNotificationForService(NotificationType.MessageReplySentFromCRMOrOpsToRM, (long)entity.Id);
                    //N.CreateNotificationForService(NotificationType.MessageReplySentFromCRMOrOpsToBM, (long)entity.Id);

                    response.Id = entity.Id;
                    response.Success = true;
                    response.Message = "Message Sent Successfully";
                }
                catch (Exception ex)
                {
                    response.Message = "Error Message : " + ex;
                }
            }
            return response;
        }
        public ResponseDto SaveDraftMessage(IPDCMessagingDto dto, long userId)
        {
            var employeeId = _user.GetEmployeeIdByUserId(userId);
            var fromDesigid = _employee.GetOfficeDesignationIdOfEmployee(employeeId);
            ResponseDto response = new ResponseDto();
            try
            {
                var entity = Mapper.Map<IPDCMessaging>(dto);

                entity.FromEmpId = employeeId;
                entity.Status = EntityStatus.Active;
                entity.CreatedBy = userId;
                entity.CreateDate = DateTime.Now;
                GenService.Save(entity);
                response.Id = entity.Id;

                response.Success = true;
                response.Message = "New Message Saved Successfully";
            }
            catch (Exception ex)
            {
                response.Message = "Error Message : " + ex;
            }

            return response;
        }
        public IPagedList<IPDCMessagingDto> GetInboxMessagingPagedList(int pageSize, int pageCount, string searchString, long UserId)
        {
            long employeeId = _user.GetEmployeeIdByUserId(UserId);
            //var fromDesignation = _employee.GetOfficeDesignationIdOfEmployee(employeeId);
            var allApp = GenService.GetAll<IPDCMessaging>().Where(s => s.Status == EntityStatus.Active && s.ToEmpId == employeeId && (s.IsDraft == false || s.IsDraft == null)).Select(s => new IPDCMessagingDto()
            {
                Id = s.Id,
                FromEmpId = s.FromEmpId,
                FromEmpName = s.FromEmployee.Person.FirstName + " " + s.FromEmployee.Person.LastName,
                FromOfficeDesignationSettingId = s.FromOfficeDesignationSettingId,
                FromDesignationName = s.FromOfficeDesignationSetting.Name,
                ApplicationId = s.ApplicationId,
                ApplicationNo = s.Application.ApplicationNo
            });
            if (!string.IsNullOrEmpty(searchString))
                allApp = allApp.Where(a => a.FromEmpName.ToLower().Contains(searchString.ToLower()) || a.ApplicationNo.ToLower().Contains(searchString.ToLower()));
            var temp2 = allApp.ToList();
            //var appIds = allApp.Select(a => a.ApplicationId).Distinct().ToList();
            var temp = temp2.OrderBy(r => r.Id).GroupBy(g=>g.ApplicationId)
                .Select(s=> new IPDCMessagingDto
                {
                    ApplicationId = s.Key,
                    ApplicationNo = s.FirstOrDefault().ApplicationNo,
                    FromEmpId = s.FirstOrDefault().FromEmpId,
                    FromEmpName = s.FirstOrDefault().FromEmpName,
                    FromOfficeDesignationSettingId = s.FirstOrDefault().FromOfficeDesignationSettingId,
                    FromDesignationName = s.FirstOrDefault().FromDesignationName

                }).ToList();
            //var temp2 = temp1.ToList();
            //var temp = temp2.ToPagedList(pageCount, pageSize);
            return temp.ToPagedList(pageCount, pageSize);
        }

        public List<IPDCMessagingDto> APIGetInboxMessages(int pageSize, int pageCount, string searchString, long UserId)
        {
            long employeeId = _user.GetEmployeeIdByUserId(UserId);
            var allApp =
                GenService.GetAll<IPDCMessaging>()
                    .Where(
                        s =>
                            s.Status == EntityStatus.Active && s.ToEmpId == employeeId &&
                            (s.IsDraft == false || s.IsDraft == null))
                .Select(s => new IPDCMessagingDto()
                {
                    Id = s.Id,
                    ApplicationId = s.ApplicationId,
                    ApplicationNo = s.Application.ApplicationNo,
                    FromEmpId = s.FromEmpId,
                    FromEmpName = s.FromEmployee.Person.FirstName + " " + s.FromEmployee.Person.LastName,
                    FromOfficeDesignationSettingId = s.FromOfficeDesignationSettingId,
                    FromDesignationName = s.FromOfficeDesignationSetting.Name,
                    ToEmpId = s.ToEmpId,
                    ToEmpName = s.ToEmployeee.Person.FirstName + " " + s.ToEmployeee.Person.LastName,
                    ToOfficeDesignationSettingId = s.ToOfficeDesignationSettingId,
                    ToDesignationName = s.ToOfficeDesignationSetting.Name,
                    Message = s.Message,
                    CreateDate = s.CreateDate

                }).OrderByDescending(m => m.Id).ToList();
            var temp = allApp.GroupBy(x => x.ApplicationId,
           (k, each) => each.FirstOrDefault()).ToList();
            
            return temp;//Mapper.Map<List<IPDCMessagingDto>>(temp);

        }
        //a.FromEmpName.ToLower().Contains(searchString.ToLower()) ||
        public IPagedList<IPDCMessagingDto> GetDraftMessagingPagedList(int pageSize, int pageCount, string searchString, long UserId)
        {
            long employeeId = _user.GetEmployeeIdByUserId(UserId);
            var allApp = GenService.GetAll<IPDCMessaging>().Where(s => s.Status == EntityStatus.Active && s.IsDraft == true).Select(s => new IPDCMessagingDto()
            {
                Id = s.Id,
                ToEmpId = s.ToEmpId,
                ToEmpName = s.ToEmployeee.Person.FirstName + " " + s.ToEmployeee.Person.LastName,
                ToOfficeDesignationSettingId = s.ToOfficeDesignationSettingId,
                ToDesignationName = s.ToOfficeDesignationSetting.Name,
                ApplicationId = s.ApplicationId,
                ApplicationNo = s.Application.ApplicationNo
            });
            if (!string.IsNullOrEmpty(searchString))
                allApp = allApp.Where(a => a.ToEmpName.ToLower().Contains(searchString.ToLower()) || a.ApplicationNo.ToLower().Contains(searchString.ToLower()));
            var temp = allApp.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }
        public List<IPDCMessagingDto> APIGetDraftedMessages(int pageSize, int pageCount, string searchString, long UserId)
        {
            long employeeId = _user.GetEmployeeIdByUserId(UserId);
            var allApp = GenService.GetAll<IPDCMessaging>().Where(s => s.Status == EntityStatus.Active && s.IsDraft == true)
                .Select(s => new IPDCMessagingDto()
                {
                    Id = s.Id,
                    ApplicationId = s.ApplicationId,
                    ApplicationNo = s.Application.ApplicationNo,
                    FromEmpId = s.FromEmpId,
                    FromEmpName = s.FromEmployee.Person.FirstName + " " + s.FromEmployee.Person.LastName,
                    FromOfficeDesignationSettingId = s.FromOfficeDesignationSettingId,
                    FromDesignationName = s.FromOfficeDesignationSetting.Name,
                    ToEmpId = s.ToEmpId,
                    ToEmpName = s.ToEmployeee.Person.FirstName + " " + s.ToEmployeee.Person.LastName,
                    ToOfficeDesignationSettingId = s.ToOfficeDesignationSettingId,
                    ToDesignationName = s.ToOfficeDesignationSetting.Name,
                    Message = s.Message,
                    CreateDate = s.CreateDate
                });
            var temp = allApp.OrderByDescending(r => r.Id)
                .Skip((pageCount - 1) * pageSize)
                .Take(pageSize).ToList();
            return Mapper.Map<List<IPDCMessagingDto>>(temp);

        }
        public IPagedList<IPDCMessagingDto> GetOutboxMessagingPagedList(int pageSize, int pageCount, string searchString, long UserId)
        {
            long employeeId = _user.GetEmployeeIdByUserId(UserId);
            var allApp = GenService.GetAll<IPDCMessaging>().Where(s => s.Status == EntityStatus.Active && s.FromEmpId == employeeId && (s.IsDraft == false || s.IsDraft == null)).Select(s => new IPDCMessagingDto()
            {
                Id = s.Id,
                ToEmpId = s.ToEmpId,
                ToEmpName = s.ToEmployeee.Person.FirstName + " " + s.ToEmployeee.Person.LastName,
                ToOfficeDesignationSettingId = s.ToOfficeDesignationSettingId,
                ToDesignationName = s.ToOfficeDesignationSetting.Name,
                ApplicationId = s.ApplicationId,
                ApplicationNo = s.Application.ApplicationNo
            });
            if (!string.IsNullOrEmpty(searchString))
                allApp = allApp.Where(a => a.ToEmpName.ToLower().Contains(searchString.ToLower()) || a.ApplicationNo.ToLower().Contains(searchString.ToLower()));
            var temp = allApp.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }
        public List<IPDCMessagingDto> APIGetOutBoxMessages(int pageSize, int pageCount, string searchString, long UserId)
        {
            long employeeId = _user.GetEmployeeIdByUserId(UserId);
            var allApp = GenService.GetAll<IPDCMessaging>().Where(s => s.Status == EntityStatus.Active && s.FromEmpId == employeeId && (s.IsDraft == false || s.IsDraft == null))
                .Select(s => new IPDCMessagingDto()
                {
                    Id = s.Id,
                    ApplicationId = s.ApplicationId,
                    ApplicationNo = s.Application.ApplicationNo,
                    FromEmpId = s.FromEmpId,
                    FromEmpName = s.FromEmployee.Person.FirstName + " " + s.FromEmployee.Person.LastName,
                    FromOfficeDesignationSettingId = s.FromOfficeDesignationSettingId,
                    FromDesignationName = s.FromOfficeDesignationSetting.Name,
                    ToEmpId = s.ToEmpId,
                    ToEmpName = s.ToEmployeee.Person.FirstName + " " + s.ToEmployeee.Person.LastName,
                    ToOfficeDesignationSettingId = s.ToOfficeDesignationSettingId,
                    ToDesignationName = s.ToOfficeDesignationSetting.Name,
                    Message = s.Message,
                    CreateDate = s.CreateDate
                }).OrderByDescending(m => m.Id);
            var temp = allApp.GroupBy(x => x.ApplicationId,
           (k, each) => each.FirstOrDefault()).ToList();

            return temp;//Mapper.Map<List<IPDCMessagingDto>>(temp);
        }

        public List<IPDCMessagingDto> APIMessageListByAppId(int pageSize, int pageCount, string searchString, long applicationId)
        {
            //long employeeId = _user.GetEmployeeIdByUserId(UserId);
            var allApp = GenService.GetAll<IPDCMessaging>().Where(s => s.ApplicationId == applicationId && s.Status == EntityStatus.Active)
                .Select(s => new IPDCMessagingDto()
                {
                    Id = s.Id,
                    ApplicationId = s.ApplicationId,
                    ApplicationNo = s.Application.ApplicationNo,
                    FromEmpId = s.FromEmpId,
                    FromEmpName = s.FromEmployee.Person.FirstName + " " + s.FromEmployee.Person.LastName,
                    FromOfficeDesignationSettingId = s.FromOfficeDesignationSettingId,
                    FromDesignationName = s.FromOfficeDesignationSetting.Name,
                    ToEmpId = s.ToEmpId,
                    ToEmpName = s.ToEmployeee.Person.FirstName + " " + s.ToEmployeee.Person.LastName,
                    ToOfficeDesignationSettingId = s.ToOfficeDesignationSettingId,
                    ToDesignationName = s.ToOfficeDesignationSetting.Name,
                    Message = s.Message,
                    IsDraft = s.IsDraft,
                    Drafted = s.IsDraft == true ? "Yes" : "No",
                    IsRead = s.IsRead,
                    Read = s.IsRead == true ? "Yes" : "No",
                    CreateDate = s.CreateDate
                }).OrderByDescending(m => m.Id);
            var temp = allApp.OrderBy(r => r.Id)
                .ToList();
            return Mapper.Map<List<IPDCMessagingDto>>(temp);
        }
        //public IPDCMessageListDto LoadMessage(long msgId, long? userId)
        //{
        //    long employeeId = _user.GetEmployeeIdByUserId((long)userId);
        //    IPDCMessageListDto ipdcMessaging = new IPDCMessageListDto();
        //    var message = GenService.GetById<IPDCMessaging>(msgId);

        //    ipdcMessaging.IpdcMessaging = new List<IPDCMessagingDto>();
        //    if (message != null)
        //    {
        //        ipdcMessaging.ApplicationId = message.ApplicationId;
        //        ipdcMessaging.ApplicationNo = message.Application.ApplicationNo;
        //        ipdcMessaging.AccountTitle = message.Application.AccountTitle;
        //        ipdcMessaging.FromEmpId = message.FromEmpId;
        //        ipdcMessaging.FromEmpName = message.FromEmployee.Person.FirstName + " " + message.FromEmployee.Person.LastName;

        //        ipdcMessaging.IpdcMessaging.Add(Mapper.Map<IPDCMessagingDto>(message));

        //        if (message.RepliedTo != null)
        //        {
        //            ipdcMessaging.IpdcMessaging.AddRange(LoadMessageThread((long)message.RepliedTo));
        //            ipdcMessaging.IpdcMessaging = ipdcMessaging.IpdcMessaging.OrderByDescending(i => i.Id).ToList();
        //        }

        //    }
        //    return ipdcMessaging;
        //}

        public IPDCMessageListDto LoadMessage(long msgId, long? appId)
        {
            IPDCMessageListDto ipdcMessaging = new IPDCMessageListDto();
            var message = GenService.GetAll<IPDCMessaging>().Where(m =>m.ApplicationId == appId).Distinct().ToList();
            ipdcMessaging.IpdcMessaging = new List<IPDCMessagingDto>();

            if (message.Count > 0)
            {
                foreach (var msg in message)
                {
                    ipdcMessaging.ApplicationId = msg.ApplicationId;
                    ipdcMessaging.ApplicationNo = msg.Application.ApplicationNo;
                    ipdcMessaging.AccountTitle = msg.Application.AccountTitle;
                    ipdcMessaging.FromEmpId = msg.FromEmpId;
                    ipdcMessaging.FromEmpName = msg.FromEmployee.Person.FirstName + " " + msg.FromEmployee.Person.LastName;
                    //msg.
                    ipdcMessaging.IpdcMessaging.Add(Mapper.Map<IPDCMessagingDto>(msg));

                    if (msg.RepliedTo != null)
                    {
                        //ipdcMessaging.IpdcMessaging.AddRange(LoadMessageThread((long)msg.RepliedTo));
                        ipdcMessaging.IpdcMessaging = ipdcMessaging.IpdcMessaging.OrderByDescending(i => i.Id)
                            .Distinct().ToList();
                    }
                }
            }
            return ipdcMessaging;
        }

        //this is a recursive function to pull only direct child messages
        //public List<IPDCMessagingDto> LoadMessageThread(long id)
        //{
        //    var resultList = new List<IPDCMessagingDto>();

        //    var msg = GenService.GetById<IPDCMessaging>(id);
        //    if (msg != null)
        //    {
        //        resultList.Add(Mapper.Map<IPDCMessagingDto>(msg));
        //        if (msg.RepliedTo != null)
        //            resultList.AddRange(LoadMessageThread((long)msg.RepliedTo));
        //    }

        //    return resultList;
        //}

        public IPDCMessagingDto LoadDraftMessages(long? msgId, long? AppId, long? userId)
        {
            long employeeId = _user.GetEmployeeIdByUserId((long)userId);
            var cifData = GenService.GetAll<IPDCMessaging>().FirstOrDefault(r => r.Id == msgId);
            var res = Mapper.Map<IPDCMessagingDto>(cifData);
            if (cifData != null)
            {
                res.ApplicationId = cifData.ApplicationId;
                res.ApplicationNo = cifData.Application.ApplicationNo;
                res.AccountTitle = cifData.Application.AccountTitle;
                res.FromEmpId = employeeId;
                res.FromEmpName = cifData.FromEmployee.Person.FirstName + " " + cifData.FromEmployee.Person.LastName;
                res.ToEmpId = cifData.ToEmpId;
                res.ToEmpName = cifData.ToEmployeee.Person.FirstName + " " + cifData.ToEmployeee.Person.LastName;
                res.Message = cifData.Message;
            }
            return res;
        }
    }
}
