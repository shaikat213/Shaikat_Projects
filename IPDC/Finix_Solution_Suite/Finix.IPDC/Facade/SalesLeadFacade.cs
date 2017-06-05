using Finix.IPDC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Finix.Auth.Facade;
using Finix.IPDC.DTO;
using Finix.IPDC.Infrastructure.Models;
using Finix.IPDC.Infrastructure;
using Microsoft.Practices.ObjectBuilder2;
using PagedList;
using Finix.IPDC.Util;

namespace Finix.IPDC.Facade
{
    public class SalesLeadFacade : BaseFacade
    {
        //private readonly GenService GenService = new GenService();
        private readonly UserFacade _user = new UserFacade();
        private readonly EmployeeFacade _employee = new EmployeeFacade();
        public List<SalesLeadDto> GetSalesLeads()
        {
            var salesLead = GenService.GetAll<SalesLead>();
            //var temp = GenService.GetAll<SalesLead>().ToList();
            return Mapper.Map<List<SalesLeadDto>>(salesLead.ToList());
        }

        public List<SalesLeadDto> GetSubmittedSalesLeads(long userId)
        {
            //long employeeId = _user.GetEmployeeIdByUserId(userId);
            var salesLead = GenService.GetAll<SalesLead>().Where(s => s.LeadStatus == LeadStatus.Drafted && s.CreatedBy == userId);
            return Mapper.Map<List<SalesLeadDto>>(salesLead.ToList());
        }

        public bool EmailFormatCheck(string email)
        {
            bool isEmail = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            return isEmail;
        }

        public ResponseDto SaveSalesLead(SalesLeadDto salesLeadDto, long userId)
        {
            long employeeId = _user.GetEmployeeIdByUserId(userId);
            var entity = new SalesLead();
            var address = new Address();
            ResponseDto responce = new ResponseDto();
            try
            {
                //if (salesLeadDto.CustomerAddress != null && !string.IsNullOrEmpty(salesLeadDto.CustomerAddress.Email))
                //{
                //    if (!EmailFormatCheck(salesLeadDto.CustomerAddress.Email))
                //    {
                //        responce.Message = "Email Format Incorrect";
                //        return responce;
                //    }
                //}
                if (salesLeadDto.Id > 0)
                {
                    entity = GenService.GetById<SalesLead>(salesLeadDto.Id);
                    if(entity.LeadStatus == LeadStatus.Prospective)
                    {
                        responce.Message = "This lead has already made Prospective. It cannot be edited again.";
                        return responce;
                    }
                    salesLeadDto.CreateDate = entity.CreateDate;
                    salesLeadDto.CreatedBy = entity.CreatedBy;
                    salesLeadDto.Status = entity.Status;
                    salesLeadDto.Call_Id = entity.Call_Id;


                    if (salesLeadDto.CustomerAddress.IsChanged)
                    {
                        if (salesLeadDto.CustomerAddress.Id != null)
                        {
                            address = GenService.GetById<Address>((long)salesLeadDto.CustomerAddress.Id);
                            salesLeadDto.CustomerAddress.PhoneNo = salesLeadDto.CustomerPhone;
                            salesLeadDto.CustomerAddress.CreateDate = address.CreateDate;
                            salesLeadDto.CustomerAddress.CreatedBy = address.CreatedBy;
                            address = Mapper.Map(salesLeadDto.CustomerAddress, address);
                            GenService.Save(address, false);
                            salesLeadDto.CustomerAddressId = address.Id;
                        }
                        else
                        {
                            var customerAddress = Mapper.Map<Address>(salesLeadDto.CustomerAddress);
                            GenService.Save(customerAddress);
                            salesLeadDto.CustomerAddressId = customerAddress.Id;
                        }

                    }
                    else
                    {
                        salesLeadDto.CustomerAddressId = entity.CustomerAddressId;
                    }

                    entity = Mapper.Map(salesLeadDto, entity);
                    entity.EditDate = DateTime.Now;
                    if (entity.LeadStatus == LeadStatus.Prospective)
                        entity.ProspectiveByEmpId = employeeId;
                    GenService.Save(entity, false);
                    GenService.SaveChanges();
                    //GenService.Save(entity);
                    if(entity.LeadStatus == LeadStatus.Prospective)
                    {
                        N.CreateNotificationForService(NotificationType.LeadDeclaredAsSuccessful, (long)entity.Id);
                    }
                    else if(entity.LeadStatus == LeadStatus.Unsuccessful)
                    {
                        N.CreateNotificationForService(NotificationType.LeadDeclaredAsUnSuccessful, (long)entity.Id);
                    }
                    responce.Id = entity.Id;
                    responce.Success = true;
                    responce.Message = "Sales Lead Edited Successfully";
                }
                else
                {
                    //salesLeadDto.LeadStatus = LeadStatus.Submitted;
                    entity = Mapper.Map<SalesLead>(salesLeadDto);
                    address = Mapper.Map<Address>(salesLeadDto.CustomerAddress);
                    if (address.CountryId != null)
                    {
                        address.PhoneNo = entity.CustomerPhone;
                        GenService.Save(address);
                        entity.CustomerAddressId = address.Id;
                    }
                    entity.Status = EntityStatus.Active;
                    entity.CreatedBy = userId;
                    entity.CreateDate = DateTime.Now;
                    //entity.LeadStatus = LeadStatus.Submitted; //todo- Lead Status Extra button To Save and Save And Draft
                    if (entity.LeadStatus == LeadStatus.Prospective)
                        entity.ProspectiveByEmpId = employeeId;
                    GenService.Save(entity);
                    if (entity.LeadStatus == LeadStatus.Prospective)
                    {
                        N.CreateNotificationForService(NotificationType.LeadDeclaredAsSuccessful, (long)entity.Id);
                    }
                    else if (entity.LeadStatus == LeadStatus.Unsuccessful)
                    {
                        N.CreateNotificationForService(NotificationType.LeadDeclaredAsUnSuccessful, (long)entity.Id);
                    }
                    responce.Id = entity.Id;
                    responce.Success = true;
                    responce.Message = "Sales Lead Saved Successfully";
                }
            }
            catch (Exception ex)
            {
                responce.Message = "Sales Lead Save Failed";
            }
            GenService.SaveChanges();
            return responce;
        }

        public List<SalesLeadDto> GetDataForLeadAssignment(long userId)//todo- filter by tl area
        {
            List<long> existingthanaIds = new List<long>();
            long employeeId = _user.GetEmployeeIdByUserId(userId);

            var thanaOfEmployee = _employee.GetThanaOfEmployee(employeeId);
            if (thanaOfEmployee.Any())
            {
                existingthanaIds = thanaOfEmployee.Select(d => (long)d.Id).ToList();
            }
            var salesLead = GenService.GetAll<SalesLead>().Where(s => s.LeadStatus == LeadStatus.Prospective  && existingthanaIds.Contains((long)s.CustomerAddress.ThanaId));
            var result = Mapper.Map<List<SalesLeadDto>>(salesLead.ToList());
            return result;
        }

        public List<SalesLeadDto> GetProspectiveSalesLeads(long userId)
        {
            //List<long> existingthanaIds = new List<long>();
            //long employeeId = _user.GetEmployeeIdByUserId(userId);

            //var thanaOfEmployee = _employee.GetThanaOfEmployee(employeeId);
            //if (thanaOfEmployee.Any())
            //{
            //    existingthanaIds = thanaOfEmployee.Select(d => (long)d.Id).ToList();
            //}
            //var salesLead = GenService.GetAll<SalesLead>().Where(s => (s.LeadStatus == LeadStatus.Submitted && s.LeadType == LeadType.Referral && existingthanaIds.Contains((long) s.ThanaId)) || (s.LeadStatus == LeadStatus.Prospective && s.LeadType == LeadType.Referral && existingthanaIds.Contains((long)s.ThanaId)));
            var salesLead = GenService.GetAll<SalesLead>().Where(s => s.LeadStatus == LeadStatus.Submitted && s.CallCategory == CallCategory.Referral);
            return Mapper.Map<List<SalesLeadDto>>(salesLead.ToList());
        }

        public List<SalesLeadDto> GetNonProspectiveSalesLeads(long userId)
        {
            var salesLead = GenService.GetAll<SalesLead>().Where(r => r.CreatedBy == userId && r.LeadStatus == LeadStatus.Unsuccessful || r.LeadStatus== LeadStatus.Submitted);
            return Mapper.Map<List<SalesLeadDto>>(salesLead.ToList());
        }

        public ResponseDto SaveFollowUpTime(FollowupTrackDto followupTrack)
        {
            ResponseDto responce = new ResponseDto();
            if (followupTrack.NextFollowUp != null || followupTrack.Remarks != null)
            {
                var salesLead = GenService.GetById<SalesLead>(followupTrack.SalesLeadId);
                followupTrack.CurrentFollowUp = salesLead.FollowupTime;
                salesLead.FollowupTime = (DateTime)(followupTrack.NextFollowUp != null? followupTrack.NextFollowUp: salesLead.FollowupTime);
                

                var entity = new FollowupTrack();

                try
                {
                    entity = Mapper.Map<FollowupTrack>(followupTrack);
                    entity.Status = EntityStatus.Active;
                    entity.CreateDate = DateTime.Now;
                    //entity.CreatedBy = userId;
                    GenService.Save(entity);
                    GenService.Save(salesLead);
                    responce.Success = true;
                    responce.Message = "Next Follow Up Saved Successfully";
                }
                catch (Exception)
                {
                    responce.Message = "Next Follow Up Save Failed";
                }
                GenService.SaveChanges();
                return responce;
            }
            else
            {
                responce.Message = "Please Provide Next Follow Up Time & Remarks";
                return responce;
            }
        }
        
        public List<QuestionAnswerDto> GetQuestionSetForLead(long leadId, long user)
        {
            var salesLead = GenService.GetById<SalesLead>(leadId);
            //long employeeId = _user.GetEmployeeIdByUserId(user);
            var questionsAnswerSet =
                (from qsa in GenService.GetAll<QuestionAssignment>().Where(r => r.ProductId == salesLead.ProductId)
                 join qs in GenService.GetAll<Question>() on qsa.QuestionId equals qs.Id
                 select new QuestionAnswerDto
                 {
                     SalesLeadId = leadId,
                     SalesLeadName = salesLead.CustomerName,
                     QuestionId = qs.Id,
                     QuestionText = qs.Questions,
                     Answer = "",
                     QuestionedBy = user

                 });
            return questionsAnswerSet.ToList();
        }

        public ResponseDto SaveQuestionnaire(List<QuestionAnswerDto> quesAns, long user)
        {
            var entity = new List<QuestionAnswer>();
            var preventity = new List<QuestionAnswer>();
            ResponseDto response = new ResponseDto();
            //long employeeId = _user.GetEmployeeIdByUserId(user);

            try
            {
                //quesAns.ForEach(q=>q.Status = EntityStatus.Active);
                foreach (var questionAnswerDto in quesAns)
                {
                    var prevQuesAns = GenService.GetAll<QuestionAnswer>().Where(r => r.SalesLeadId == questionAnswerDto.SalesLeadId && r.QuestionId == questionAnswerDto.QuestionId && r.Status == EntityStatus.Active);
                    if (prevQuesAns.Any())
                    {
                        prevQuesAns.ForEach(r => r.Status = EntityStatus.Inactive);
                        preventity.AddRange(prevQuesAns);
                    }
                    questionAnswerDto.CreateDate = DateTime.Now;
                    questionAnswerDto.CreatedBy = user;
                    questionAnswerDto.Status = EntityStatus.Active;
                }
                entity = Mapper.Map<List<QuestionAnswer>>(quesAns);
                GenService.Save(entity);
                GenService.Save(preventity);
                response.Success = true;
                response.Message = "Questionnaire Saved Successfully";

            }
            catch (Exception ex)
            {
                response.Message = "Questionnaire Save Failed";
            }
            GenService.SaveChanges();
            return response;
        }

        public SalesLeadDto GetSalesLeadForEdit(long leadId)
        {
            var salesLead = GenService.GetAll<SalesLead>().FirstOrDefault(r => r.Id == leadId && r.Status == EntityStatus.Active);
            return Mapper.Map<SalesLeadDto>(salesLead);
        }

        public List<OfficeDesignationSettingDto> GetAllSR(long userId)
        {
            var employeeId = _user.GetEmployeeIdByUserId(userId);
            var getEmployeeWiseSR = _employee.GetEmployeeWiseSR(employeeId);
            return getEmployeeWiseSR.ToList();
            //var employees = GenService.GetAll<Employee>().Where(r => r.Id != employeeId);
            //return Mapper.Map<List<EmployeeDto>>(employees.ToList());
            //throw new NotImplementedException();
        }

        public List<SalesLeadDto> GetAssignedSalesLeads(long UserId)
        {
            long employeeId = _user.GetEmployeeIdByUserId(UserId);
            long offDegSettingId = _employee.GetOfficeDesignationIdOfEmployee(employeeId);

            var leadIds =
                GenService.GetAll<SalesLeadAssignment>()
                    .Where(s => s.AssignedToEmpId == employeeId && s.Status == EntityStatus.Active).Select(s=>s.SalesLeadId).ToList();
            var result =
                GenService.GetAll<SalesLead>().Where(s => leadIds.Contains(s.Id) && s.Status == EntityStatus.Active && s.LeadStatus == LeadStatus.Drafted).OrderByDescending(s=>s.Id);
            return Mapper.Map<List<SalesLeadDto>>(result.ToList());

        }
        public List<SalesLeadDto> GetAssignedSalesLeadsApi(long UserId, string searchString = "", int pageCount = 1, int pageSize = 20)
        {
            long employeeId = _user.GetEmployeeIdByUserId(UserId);
            long offDegSettingId = _employee.GetOfficeDesignationIdOfEmployee(employeeId);

            var leadIds =
                GenService.GetAll<SalesLeadAssignment>()
                    .Where(s => s.AssignedToEmpId == employeeId && s.Status == EntityStatus.Active).Select(s => s.SalesLeadId).ToList();
            var result =
                GenService.GetAll<SalesLead>().Where(s => leadIds.Contains(s.Id) && s.Status == EntityStatus.Active && s.LeadStatus == LeadStatus.Drafted).OrderByDescending(s => s.Id).ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                result = result.Where(a => a.CustomerName.ToLower().Contains(searchString.ToLower())
                || a.CustomerPhone.ToLower().Contains(searchString.ToLower())
                || a.CallSourceText.ToLower().Contains(searchString.ToLower())).ToList();
            }
            var temp = Mapper.Map<List<SalesLeadDto>>(result.OrderByDescending(r => r.Id).ToList());
            temp.ForEach(r => r.CallFailReasonName = (r.CallFailReason > 0 ? UiUtil.GetDisplayName(r.CallFailReason) : ""));
            temp = temp.Skip((pageCount - 1) * pageSize)
                .Take(pageSize).ToList();
            return temp;

        }

        public List<SalesLeadDto> GetSalesLeadsTillToday(long UserId)
        {
            long employeeId = _user.GetEmployeeIdByUserId(UserId);
            long offDegSettingId = _employee.GetOfficeDesignationIdOfEmployee(employeeId);

            var leadIds =
                GenService.GetAll<SalesLeadAssignment>()
                    .Where(s => s.AssignedToEmpId == employeeId && s.Status == EntityStatus.Active).Select(s => s.SalesLeadId).ToList();
            var result =
                GenService.GetAll<SalesLead>().Where(s => leadIds.Contains(s.Id) && s.Status == EntityStatus.Active && s.LeadStatus == LeadStatus.Drafted && s.FollowupTime <= DateTime.Now).OrderByDescending(s => s.Id);
            return Mapper.Map<List<SalesLeadDto>>(result.ToList());

        }

        public IPagedList<SalesLeadDto> GetUnsuccessfulAssignedSalesLeads(long UserId, int pageSize, int pageCount, string searchString)
        {
            long employeeId = _user.GetEmployeeIdByUserId(UserId);
            var degignationOfEmployee = _employee.GetOfficeDesignationIdOfEmployee(employeeId);
            List<long> employeeIds = new List<long>();
            employeeIds = _employee.GetEmployeeWiseBM(employeeId).Select(e => (long)e.EmployeeId).ToList();
            employeeIds.Add(employeeId);
            
            var leadIds =
                GenService.GetAll<SalesLeadAssignment>()
                    .Where(s => employeeIds.Contains((long)s.AssignedToEmpId) && s.Status == EntityStatus.Active).Select(s => s.SalesLeadId).ToList();
            var result =
                GenService.GetAll<SalesLead>().Where(s => leadIds.Contains(s.Id) && s.Status == EntityStatus.Active && s.LeadStatus == LeadStatus.Unsuccessful).OrderByDescending(s => s.Id)
                .Select(s=> new SalesLeadDto {
                    Id = s.Id,
                    CustomerName = s.CustomerName,
                    CustomerPhone = s.CustomerPhone,
                    LeadStatus = s.LeadStatus,
                    LeadStatusName = s.LeadStatus.ToString(),
                    FollowupTime = s.FollowupTime,
                    FailedRemarks = s.FailedRemarks,
                    CallFailReason = s.CallFailReason
                });
            
            
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                result = result.Where(a => a.CustomerName.ToLower().Contains(searchString.ToLower())
                || a.CustomerPhone.ToLower().Contains(searchString.ToLower())
                || a.CallSourceText.ToLower().Contains(searchString.ToLower()));
            }
            var temp = result.OrderByDescending(r => r.Id).ToList();
            temp.ForEach(r => r.CallFailReasonName = (r.CallFailReason > 0 ? UiUtil.GetDisplayName(r.CallFailReason) : ""));
            
            return temp.ToPagedList(pageCount, pageSize);
        }

        public List<SalesLeadDto> GetUnsuccessfulAssignedSalesLeadsApi(long UserId, int pageSize, int pageCount, string searchString)
        {
            long employeeId = _user.GetEmployeeIdByUserId(UserId);
            var degignationOfEmployee = _employee.GetOfficeDesignationIdOfEmployee(employeeId);
            List<long> employeeIds = new List<long>();
            employeeIds = _employee.GetEmployeeWiseBM(employeeId).Select(e => (long)e.EmployeeId).ToList();
            employeeIds.Add(employeeId);
            var leadIds =
                GenService.GetAll<SalesLeadAssignment>()
                    .Where(s => employeeIds.Contains((long)s.AssignedToEmpId) && s.Status == EntityStatus.Active).Select(s => s.SalesLeadId).ToList();
            var result =
                GenService.GetAll<SalesLead>().Where(s => leadIds.Contains(s.Id) && s.Status == EntityStatus.Active && s.LeadStatus == LeadStatus.Unsuccessful);

            if (!string.IsNullOrEmpty(searchString))
                result = result.Where(a => a.CustomerName.ToLower().Contains(searchString.ToLower())
                || a.CustomerPhone.ToLower().Contains(searchString.ToLower())
                || a.CallSourceText.ToLower().Contains(searchString.ToLower()));
            var temp = result.OrderByDescending(r => r.Id)
                .Skip((pageCount - 1) * pageSize)
                .Take(pageSize).ToList();
            return Mapper.Map<List<SalesLeadDto>>(temp);
        }

        public List<SalesLeadDto> GetProspectiveAssignedSalesLeads(long UserId)
        {
            long employeeId = _user.GetEmployeeIdByUserId(UserId);
            long offDegSettingId = _employee.GetOfficeDesignationIdOfEmployee(employeeId);

            var leadIds =
                GenService.GetAll<SalesLeadAssignment>()
                    .Where(s => s.AssignedToEmpId == employeeId && s.Status == EntityStatus.Active).Select(s => s.SalesLeadId).ToList();
            var query = GenService.GetAll<SalesLead>().Where(s => leadIds.Contains(s.Id) && s.Status == EntityStatus.Active && s.LeadStatus == LeadStatus.Prospective).ToList();
            var notAcceptableLeadIds =
                GenService.GetAll<Application>()
                    .Where(s => s.ApplicationStage != ApplicationStage.Drafted)
                    .Select(s => s.SalesLeadId)
                    .ToList();
            var result = Mapper.Map<List<SalesLeadDto>>(query);
            if(result != null && result.Count > 0)
            {
                result = (from r in result.Where(r=> !notAcceptableLeadIds.Contains(r.Id))
                          join application in GenService.GetAll<Application>().Where(a => a.Status == EntityStatus.Active) on r.Id equals application.SalesLeadId into appJoin
                          from app in appJoin.DefaultIfEmpty()
                          select new SalesLeadDto
                          {
                              #region sales lead mapping
                              
                              AgeRange = r.AgeRange,
                              ApplicationId = app != null ? app.Id : 0,
                              Call_Id = r.Call_Id,
                              CustomerName = r.CustomerName,
                              CustomerAddress = r.CustomerAddress != null ? r.CustomerAddress : null,
                              Id = r.Id,
                              CreateDate = r.CreateDate,
                              CreatedBy = r.CreatedBy,
                              CreatedByName = r.CreatedByName,
                              CustomerPriority = r.CustomerPriority,
                              CustomerPriorityName = r.CustomerPriorityName,
                              CustomerSensitivity = r.CustomerSensitivity,
                              CustomerSensitivityName = r.CustomerSensitivityName,
                              ContactPersonDesignation = r.ContactPersonDesignation,
                              CustomerDesignation = r.CustomerDesignation,
                              EditDate = r.EditDate,
                              EditedBy = r.EditedBy,
                              Amount = r.Amount,
                              FollowupTime = r.FollowupTime,
                              FollowupTimeText = r.FollowupTimeText,
                              LeadStatus = r.LeadStatus,
                              LeadStatusName = r.LeadStatusName,
                              CallCategory = r.CallCategory,
                              CallCategoryName = r.CallCategoryName,
                              ProductId = r.ProductId,
                              ProductName = r.ProductName,
                              Remarks = r.Remarks,
                              Status = r.Status,
                              LeadPriorityName = r.LeadPriorityName
                              #endregion
                          }).ToList();
            }
            return result;

        }

        public ResponseDto SaveSalesLeadAssignment(SalesLeadAssignmentDto salesLeadAssignmentDto, long userId)
        {
            // var salesLead = GetSalesLeadForEdit()
            var entity = new SalesLeadAssignment();
            var salesLead = new SalesLead();
            var followup = new FollowupTrack();
            ResponseDto response = new ResponseDto();
            long employeeId = _user.GetEmployeeIdByUserId(userId);
            long offDegSettingId = _employee.GetOfficeDesignationIdOfEmployee(employeeId);
            try
            {
                if (salesLeadAssignmentDto.SalesLeadId != null)
                {
                    salesLead = GenService.GetSingleById<SalesLead>((long)salesLeadAssignmentDto.SalesLeadId);
                    //if (salesLead.FollowUpCallTime != salesLeadAssignmentDto.FollowUpTime)
                    //{

                    //    followup.CurrentFollowUp = salesLead.FollowUpCallTime;
                    //    followup.NextFollowUp = salesLeadAssignmentDto.FollowUpTime;
                    //    followup.SalesLeadId = salesLead.Id;
                    //    followup.SubmittedBy = employeeId;
                    //    followup.Status = EntityStatus.Active;
                    //    GenService.Save(followup);
                    //    salesLead.FollowUpCallTime = (DateTime)salesLeadAssignmentDto.FollowUpTime;

                    //}
                    //else
                    //{
                    //    response.Message = "Sales Lead Already Engaged On Following TIme";
                    //    return response;
                    //}
                    GenService.Save(salesLead);
                }
                entity = Mapper.Map<SalesLeadAssignment>(salesLeadAssignmentDto);
                entity.AssignedByEmpId = employeeId;
                entity.AssignedByOffDegId = offDegSettingId;
                GenService.Save(entity);

                response.Success = true;
                response.Message = "Sales Lead Assignment Saved Successfully";

            }
            catch (Exception ex)
            {
                response.Message = "Sales Lead Assignment Save Failed";
            }
            GenService.SaveChanges();
            return response;
        }

        public List<SalesLeadAssignmentDto> GeSalesLeadAssignment(long id, long assaignto, long userId) //todo - throw exception for Assigned Person
        {
            var salesAssignmentList = new List<SalesLeadAssignmentDto>();
            var salesLeadAssignment = new SalesLeadAssignmentDto();
            //var employeeId = _user.GetEmployeeIdByUserId(userId);
            var salesLead = GetSalesLeadForEdit(id);
            DateTime fromDateTime = ((DateTime)salesLead.FollowupTime).Date;
            DateTime toDateTime = ((DateTime)salesLead.FollowupTime).Date.AddDays(1);
            var srRoutine =
                GenService.GetAll<SalesLeadAssignment>()
                    .Where(r => r.AssignedToEmpId == assaignto && r.FollowUpTime >= fromDateTime && r.FollowUpTime <= toDateTime);
            salesAssignmentList.AddRange(Mapper.Map<List<SalesLeadAssignmentDto>>(srRoutine.ToList()));
            //if (salesLead.Id > 0)
            //{
            //    salesLeadAssignment.AssignedBy = employeeId;
            //    salesLeadAssignment.AssignedByName = GenService.GetSingleById<Employee>(employeeId).Person.FirstName;
            //    salesLeadAssignment.AssignedTo = assaignto;
            //    salesLeadAssignment.AssignedToName = GenService.GetSingleById<Employee>(assaignto).Person.FirstName;
            //    salesLeadAssignment.FollowUpTime = salesLead.FollowUpCallTime;
            //    salesLeadAssignment.SalesLeadId = salesLead.Id;
            //    salesLeadAssignment.SalesLeadName = salesLead.Name;
            //    salesLeadAssignment.SalesLeadAddress = salesLead.Address;
            //    salesLeadAssignment.FollowUpTimeTxt = ((DateTime)salesLead.FollowUpCallTime).ToString("dd/MM/yyyy HH:mm");
            //    salesAssignmentList.Add(salesLeadAssignment);
            //}
            return salesAssignmentList;
            //throw new NotImplementedException();
            // return 
        }

        public List<SalesLeadAssignmentDto> GetDateWiseTimeSchedule(long id, DateTime followUp)
        {
            DateTime fromDateTime = followUp.Date;
            DateTime toDateTime = followUp.Date.AddDays(1);
            var srRoutine =
               GenService.GetAll<SalesLeadAssignment>()
                   .Where(r => r.AssignedToEmpId == id && r.FollowUpTime >= fromDateTime && r.FollowUpTime <= toDateTime);
            return Mapper.Map<List<SalesLeadAssignmentDto>>(srRoutine.ToList());
            //throw new NotImplementedException();
        }

        public List<FollowupTrackDto> GetCallLogBySLNo(long slno)
        {
            var result = GenService.GetAll<FollowupTrack>().Where(f => f.SalesLeadId == slno);

            return Mapper.Map<List<FollowupTrackDto>>(result.ToList());
        }
        public List<FollowupTrackDto> GetCallLogBySLNoAndUserno(long slno, long UserId)
        {
            var result = GenService.GetAll<FollowupTrack>().Where(f => f.SalesLeadId == slno && f.CreatedBy == UserId);

            return Mapper.Map<List<FollowupTrackDto>>(result.ToList());
        }
    }
}
