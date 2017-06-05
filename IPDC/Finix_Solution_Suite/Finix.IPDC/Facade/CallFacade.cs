using AutoMapper;
using Finix.IPDC.DTO;
using Finix.IPDC.Infrastructure.Models;
using Finix.IPDC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using Finix.IPDC.Infrastructure;
using System.Transactions;
using PagedList;
using Finix.Auth.Facade;
using Finix.IPDC.Util;

namespace Finix.IPDC.Facade
{
    public class CallFacade : BaseFacade
    {
        //private readonly  GenService _service = new GenService();
        private readonly SequencerFacade _sequencer = new SequencerFacade();
        private readonly SalesLeadFacade _salesLeadFacade = new SalesLeadFacade();
        private readonly UserFacade _user = new UserFacade();
        private readonly EmployeeFacade _employee = new EmployeeFacade();

        //public ResponseDto SaveCallEntry(CallDto dto, long UserId)
        //{
        //    ResponseDto responce = new ResponseDto();
        //    var entity = Mapper.Map<Call>(dto);
        //    entity.CreateDate = DateTime.Now;
        //    entity.CreatedBy = UserId;
        //    var customerAddress = Mapper.Map<Address>(dto.CustomerAddress);

        //    using (var tran = new TransactionScope())
        //    {
        //        try
        //        {
        //            if (customerAddress.CountryId != null)
        //            {
        //                GenService.Save(customerAddress);
        //                entity.CustomerAddressId = customerAddress.Id;
        //            }
        //            GenService.Save(entity);
        //            entity.CallId = _sequencer.GetUpdatedCallNo();
        //            GenService.Save(entity);
        //            tran.Complete();

        //        }
        //        catch (Exception ex)
        //        {
        //            tran.Dispose();
        //            responce.Message = "Call Information Save Failed";
        //        }
        //    }
        //    GenService.SaveChanges();
        //    responce.Message = "Call Information Saved Successfully";
        //    return responce;

        //}


        public ResponseDto SaveCallEntry(CallDto dto, long UserId)
        {
            var entity = new Call();
            if (dto.CallType == CallType.Self)
                dto.CallCategory = CallCategory.Solicited;
            else
                dto.CallCategory = CallCategory.Referral;
            //N.CreateNotificationForService(NotificationType.CallReferedOrAutoAssignedByEmployee, (long)dto.Id);
            var address = new Address();
            ResponseDto responce = new ResponseDto();
            long employeeId = _user.GetEmployeeIdByUserId((long)UserId);
            long offDegSettingId = _employee.GetOfficeDesignationIdOfEmployee(employeeId);

            if (dto.Id != null && dto.Id > 0)
            {
                #region update
                entity = GenService.GetById<Call>((long)dto.Id);
                if (entity.ConvertedToLead != null && entity.ConvertedToLead == true)
                {
                    responce.Message = "This call has already been converted to lead. It cannot be edited again.";
                    return responce;
                }
                dto.CallId = entity.CallId;
                dto.CreateDate = entity.CreateDate;
                dto.CreatedBy = entity.CreatedBy;
                dto.CallCreatorName = entity.CallCreatorName;
                dto.AssignedTo = entity.AssignedTo;
                using (var tran = new TransactionScope())
                {
                    try
                    {
                        if (dto.CustomerAddress.IsChanged)
                        {
                            if (dto.CustomerAddress.Id != null)
                            {
                                address = GenService.GetById<Address>((long)dto.CustomerAddress.Id);
                                dto.CustomerAddress.PhoneNo = dto.CustomerPhone;
                                dto.CustomerAddress.CreateDate = address.CreateDate;
                                dto.CustomerAddress.CreatedBy = address.CreatedBy;
                                address = Mapper.Map(dto.CustomerAddress, address);
                                GenService.Save(address);
                                dto.CustomerAddressId = address.Id;
                            }
                            else
                            {
                                var customerAddress = Mapper.Map<Address>(dto.CustomerAddress);
                                GenService.Save(customerAddress);
                                dto.CustomerAddressId = customerAddress.Id;
                            }

                        }
                        else
                        {
                            dto.CustomerAddressId = entity.CustomerAddressId;
                        }

                        entity = Mapper.Map(dto, entity);
                        entity.EditDate = DateTime.Now;
                        entity.EditedBy = UserId;
                        if (entity.CallStatus == CallStatus.Successful)
                        {
                            entity.CallFailReason = null;
                            entity.FailedRemarks = null;
                        }
                        GenService.Save(entity);
                        if (entity.CallType == CallType.Auto_assign)
                        {
                            N.CreateNotificationForService(NotificationType.CallAutoAssignedByEmployee, (long)entity.Id);
                        }
                        if (entity.CallType == CallType.User_Assigned)
                        {
                            N.CreateNotificationForService(NotificationType.CallReferedByEmployee, (long)entity.Id);
                        }
                        tran.Complete();
                        responce.Success = true;
                        responce.Message = "Call Information Edited Successfully";
                      

                    }
                    catch (Exception ex)
                    {
                        tran.Dispose();
                        responce.Message = "Call Information Edit Operation Failed";
                    }
                   
                       
                    
                }
                if (entity.CallStatus == CallStatus.Successful)
                {
                    SalesLeadDto saleslead = new SalesLeadDto();
                    saleslead = Mapper.Map<SalesLeadDto>(entity);
                    saleslead.FollowupTime = dto.FollowUpCallTime;
                    saleslead.LeadStatus = LeadStatus.Drafted;
                    var salesLeadSave = _salesLeadFacade.SaveSalesLead(saleslead, UserId);
                    if ((bool)salesLeadSave.Success)
                    {
                        entity.ConvertedToLead = true;
                        GenService.Save(entity);
                        responce.Success = true;
                        responce.Message = "Call Information Edited Successfully";
                        N.CreateNotificationForService(NotificationType.CallConvertToLead, (long)salesLeadSave.Id);
                        SalesLeadAssignmentDto selfAssignment = new SalesLeadAssignmentDto();
                        selfAssignment.AssignedByEmpId = employeeId;
                        selfAssignment.AssignedToEmpId = employeeId;
                        selfAssignment.AssignedByOffDegId = offDegSettingId;
                        selfAssignment.AssignedToOffDegId = offDegSettingId;
                        selfAssignment.FollowUpTime = saleslead.FollowupTime;
                        selfAssignment.SalesLeadId = salesLeadSave.Id;

                        var selfAssignmentResult = _salesLeadFacade.SaveSalesLeadAssignment(selfAssignment, UserId);
                    }

                }
                
                #endregion

            }
            else
            {
                #region save new
                entity = Mapper.Map<Call>(dto);
                //entity.CIFNo = _sequencer.GetUpdatedCIFPersonalNo();
                entity.Status = EntityStatus.Active;
                entity.CreatedBy = UserId;
                entity.CreateDate = DateTime.Now;
                if (employeeId > 0)
                {
                    var employee = GenService.GetById<Employee>(employeeId);
                    if (employee.PersonId != null)
                        entity.CallCreatorName = employee.Person.FirstName + " " + employee.Person.LastName;

                }
                var customerAddress = Mapper.Map<Address>(dto.CustomerAddress);

                using (var tran = new TransactionScope())
                {
                    try
                    {
                        if (customerAddress.CountryId != null)
                        {
                            customerAddress.PhoneNo = entity.CustomerPhone;
                            GenService.Save(customerAddress);
                            entity.CustomerAddressId = customerAddress.Id;
                        }

                        GenService.Save(entity);
                        entity.CallId = _sequencer.GetUpdatedCallNo();
                        GenService.Save(entity);
                        if(entity.CallType == CallType.Auto_assign)
                        {
                            N.CreateNotificationForService(NotificationType.CallAutoAssignedByEmployee, (long)entity.Id);
                        }
                        if (entity.CallType == CallType.User_Assigned)
                        {
                            N.CreateNotificationForService(NotificationType.CallReferedByEmployee, (long)entity.Id);
                        }
                       
                        tran.Complete();
                        responce.Success = true;
                        responce.Message = "Call Information Saved Successfuly.";
                        responce.Id = entity.Id;
                        
                        
                    }
                    catch (Exception ex)
                    {
                        tran.Dispose();
                        responce.Message = "Call Information Save Failed";

                    }
                   
                }
                if (dto.CallStatus == CallStatus.Successful)
                {
                    SalesLeadDto saleslead = new SalesLeadDto();
                    saleslead = Mapper.Map<SalesLeadDto>(entity);
                    saleslead.Call_Id = entity.Id;
                    saleslead.FollowupTime = dto.FollowUpCallTime;
                    saleslead.LeadStatus = LeadStatus.Drafted;
                    var salesLeadSave = _salesLeadFacade.SaveSalesLead(saleslead, UserId);
                    if ((bool)salesLeadSave.Success)
                    {
                        entity.ConvertedToLead = true;
                        GenService.Save(entity);
                        responce.Success = true;
                        N.CreateNotificationForService(NotificationType.CallConvertToLead, (long)salesLeadSave.Id);
                        responce.Message = "Call Information Saved and Sales Lead generated.";
                        SalesLeadAssignmentDto selfAssignment = new SalesLeadAssignmentDto();
                        selfAssignment.AssignedByEmpId = employeeId;
                        selfAssignment.AssignedToEmpId = employeeId;
                        selfAssignment.AssignedByOffDegId = offDegSettingId;
                        selfAssignment.AssignedToOffDegId = offDegSettingId;
                        selfAssignment.FollowUpTime = saleslead.FollowupTime;
                        selfAssignment.SalesLeadId = salesLeadSave.Id;

                        var selfAssignmentResult = _salesLeadFacade.SaveSalesLeadAssignment(selfAssignment, UserId);
                    }
                    else
                    {
                        return salesLeadSave;
                    }
                }
                else if (dto.CallStatus == CallStatus.Unsuccessful)
                {
                    N.CreateNotificationForService(NotificationType.CallDeclaredAsUnsuccessful, (long)entity.Id);
                    //responce.Success = true;
                    //responce.Message = "Call Information Saved Successfuly.....";
                }
                //responce.Success = true;
                //responce.Message = "Client Information Saved Successfully";
                #endregion
            }

            //this will trigger notification(s)
            //N.CreateNotificationForService(NotificationType.LeadWaitingForAssignment, entity.Id);

            return responce;
        }

        public ResponseDto SaveCallEntryToLead(CallDto dto, long UserId)
        {
            var entity = new Call();
            ResponseDto responce = new ResponseDto();
            long employeeId = _user.GetEmployeeIdByUserId((long)UserId);
            long offDegSettingId = _employee.GetOfficeDesignationIdOfEmployee(employeeId);
            //try
            //{
            if (dto.Id != null && dto.Id > 0)
            {
                try
                {
                    entity = GenService.GetById<Call>((long)dto.Id);
                    if (dto.CallStatus == CallStatus.Successful)
                    {
                        entity.CallStatus = CallStatus.Successful;
                        entity.FailedRemarks = null;
                        entity.CallFailReason = null;
                    }

                    GenService.Save(entity);
                    GenService.SaveChanges();
                    SalesLeadDto saleslead = new SalesLeadDto();
                    saleslead = Mapper.Map<SalesLeadDto>(entity);
                    saleslead.FollowupTime = dto.FollowUpCallTime;
                    saleslead.LeadStatus = LeadStatus.Drafted;
                    var salesLeadSave = _salesLeadFacade.SaveSalesLead(saleslead, UserId);
                    var userList = GenService.GetAll<ApplicationLog>()
                        .Where(a => a.ApplicationId == UserId)
                        .Select(a => a.FromUserId)
                        .Distinct()
                        .ToList();
                    if ((bool)salesLeadSave.Success)
                    {

                        responce.Success = true;
                        responce.Message = "Call Information Saved and Sales Lead generated....";
                        SalesLeadAssignmentDto selfAssignment = new SalesLeadAssignmentDto();
                        selfAssignment.AssignedByEmpId = employeeId;
                        selfAssignment.AssignedToEmpId = employeeId;
                        selfAssignment.AssignedByOffDegId = offDegSettingId;
                        selfAssignment.AssignedToOffDegId = offDegSettingId;
                        selfAssignment.FollowUpTime = dto.FollowUpCallTime;
                        selfAssignment.SalesLeadId = salesLeadSave.Id;

                        var selfAssignmentResult = _salesLeadFacade.SaveSalesLeadAssignment(selfAssignment, UserId);
                        N.CreateNotificationForService(NotificationType.CallConvertToLead, (long)salesLeadSave.Id);
                    }
                    else
                    {
                        return salesLeadSave;
                    }
                    //_salesLeadFacade.SaveSalesLead(saleslead, UserId);
                    //responce.Success = true;
                    //responce.Message = "Sales Lead generated....";
                }
                catch (Exception ex)
                {
                    //tran.Dispose();
                    responce.Message = "Sales Lead Generation Failed";
                }
            }

            return responce;
        }

        public ResponseDto SaveCallFailReason(CallDto dto, long UserId)
        {
            var entity = new Call();
            ResponseDto responce = new ResponseDto();

            if (dto.Id != null && dto.Id > 0)
            {
                try
                {
                    entity = GenService.GetById<Call>((long)dto.Id);

                    if (dto.CallStatus == CallStatus.Unsuccessful)
                    {
                        entity.CallStatus = CallStatus.Unsuccessful;
                    }
                    if (dto.CallFailReason == CallFailReason.Customer_not_interested)
                    {
                        entity.CallFailReason = CallFailReason.Customer_not_interested;
                    }
                    else if (dto.CallFailReason == CallFailReason.Customer_out_of_reach)
                    {
                        entity.CallFailReason = CallFailReason.Customer_out_of_reach;
                    }
                    entity.Remarks = dto.Remarks;
                    GenService.Save(entity);
                    GenService.SaveChanges();
                }
                catch (Exception ex)
                {
                    responce.Message = "Call Fail Reason Failed";
                }
                responce.Success = true;
                responce.Message = "Call Fail Reason Saved....";
            }
            return responce;
        }
        public ResponseDto SaveCallAssigned(CallDto dto, long UserId)
        {
            var entity = new Call();
            ResponseDto responce = new ResponseDto();

            if (dto.Id != null && dto.Id > 0)
            {
                try
                {
                    entity = GenService.GetById<Call>((long)dto.Id);

                    if (dto.CallStatus == CallStatus.Unfinished)
                    {
                        entity.CallStatus = CallStatus.Unfinished;
                    }

                    entity.AssignedTo = dto.AssignedTo;
                    entity.Remarks = dto.Remarks;
                    GenService.Save(entity);
                    GenService.SaveChanges();
                }
                catch (Exception ex)
                {
                    responce.Message = "Assigned To RM Failed";
                }
                responce.Success = true;
                responce.Message = "Assigned To RM Saved....";
                N.CreateNotificationForService(NotificationType.CallAssignedRMByBM, (long)entity.Id);
            }
            return responce;
        }

        public List<ProductDto> GetAllProduct()
        {
            var settings = GenService.GetAll<Product>().ToList();
            return Mapper.Map<List<ProductDto>>(settings);
        }

        //public List<OfficeDesignationSettingDto> GetAllSR(long userId)
        //{
        //    var employeeId = _user.GetEmployeeIdByUserId(userId);
        //    var getEmployeeWiseSR = _employee.GetEmployeeWiseSR(employeeId);
        //    return getEmployeeWiseSR.ToList();
        //    //var employees = GenService.GetAll<Employee>().Where(r => r.Id != employeeId);
        //    //return Mapper.Map<List<EmployeeDto>>(employees.ToList());
        //    //throw new NotImplementedException();
        //}

        public CallDto GetCallEntry(long cifId)
        {
            var cifData = GenService.GetAll<Call>().FirstOrDefault(r => r.Id == cifId);
            var result = Mapper.Map<CallDto>(cifData);
            long employeeId = _user.GetEmployeeIdByUserId((long)cifData.CreatedBy);
            var emp = _employee.GetEmployeeByEmployeeId(employeeId);
            result.CallCreatorName = emp.Name;
            return result;
        }
        public IPagedList<CallDto> GetBMCallPagedList(int pageSize, int pageCount, string searchString, long? UserId) //int pageSize, int pageCount, string searchString    IEnumerable<CIF_PersonalDto>
        {
            IQueryable<Call> callQueryableAreaWise = GenService.GetAll<Call>().Where(s => s.Status == EntityStatus.Active && s.CallStatus == CallStatus.Unfinished && s.CallType == CallType.Auto_assign);

            long reffered = 0;
            if (UserId != null)
            {
                long employeeId = _user.GetEmployeeIdByUserId((long)UserId);
                var degignationOfEmployee = _employee.GetOfficeDesignationIdOfEmployee(employeeId);
                if (degignationOfEmployee > 0)
                {
                    reffered = degignationOfEmployee;
                }
                List<long> existingthanaIds = new List<long>();
                var thanaOfEmployee = _employee.GetThanaOfEmployee(employeeId);
                if (thanaOfEmployee.Any())
                {
                    existingthanaIds = thanaOfEmployee.Select(d => (long)d.Id).ToList();
                }
                callQueryableAreaWise = callQueryableAreaWise.Where(c => existingthanaIds.Contains((long)c.CustomerAddress.ThanaId));
            }

            IQueryable<Call> callQueryableAsReferred = GenService.GetAll<Call>().Where(c => (c.ReferredTo == reffered || c.AssignedTo == reffered) && c.Status == EntityStatus.Active && c.CallStatus == CallStatus.Unfinished);
            IQueryable<EmployeeDesignationMapping> employees = GenService.GetAll<EmployeeDesignationMapping>().Where(e => e.Status == EntityStatus.Active);

            var allCif = (from areaWise in callQueryableAreaWise
                          join employee in employees on areaWise.AssignedTo equals employee.OfficeDesignationSettingId into assignedToEmployee
                          from assignedTo in assignedToEmployee.DefaultIfEmpty()
                          select new CallDto//callQueryableAreaWise.Where(s => s.Status == EntityStatus.Active && s.CallType == CallType.Auto_assign && s.CallStatus == CallStatus.Unfinished || s.ReferredTo == reffered).Select(s => new CallDto
                          {
                              Id = areaWise.Id,
                              CallId = areaWise.CallId,
                              CustomerName = areaWise.CustomerName,
                              CustomerPhone = areaWise.CustomerPhone,
                              CallSourceName = areaWise.CallCategory.ToString(),
                              CallSourceText = areaWise.CallSourceText,
                              CallTypeName = areaWise.CallType.ToString(),
                              CallStatusName = areaWise.CallStatus.ToString(),
                              AssignedTo = areaWise.AssignedTo,
                              AssignedToName = (assignedTo != null ? assignedTo.Employee.Person.FirstName + " " + assignedTo.Employee.Person.LastName : ""),
                              CallCreatorName = areaWise.CallCreatorName,
                              CreateDate = areaWise.CreateDate
                          })
                          .Union(
                                from referredTo in callQueryableAsReferred
                                join employee in employees on referredTo.AssignedTo equals employee.OfficeDesignationSettingId into assignedToEmployee
                                from assignedTo in assignedToEmployee.DefaultIfEmpty()
                                select new CallDto
                                {
                                    Id = referredTo.Id,
                                    CallId = referredTo.CallId,
                                    CustomerName = referredTo.CustomerName,
                                    CustomerPhone = referredTo.CustomerPhone,
                                    CallSourceName = referredTo.CallCategory.ToString(),
                                    CallSourceText = referredTo.CallSourceText,
                                    CallTypeName = referredTo.CallType.ToString(),
                                    CallStatusName = referredTo.CallStatus.ToString(),
                                    AssignedTo = referredTo.AssignedTo,
                                    AssignedToName = (assignedTo != null ? assignedTo.Employee.Person.FirstName + " " + assignedTo.Employee.Person.LastName : ""),
                                    CallCreatorName = referredTo.CallCreatorName,
                                    CreateDate = referredTo.CreateDate
                                }
                            );

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                allCif = allCif.Where(c => c.CallId.ToLower().Contains(searchString) || c.CustomerName.ToLower().Contains(searchString) || c.CustomerPhone.ToLower().Contains(searchString));
            }
            return allCif.OrderByDescending(r => r.Id).ToPagedList(pageCount, pageSize);
        }
        public IPagedList<CallDto> GetCallPagedList(int pageSize, int pageCount, string searchString, long? UserId)
        {
            long assigned = 0;
            if (UserId != null)
            {
                long employeeId = _user.GetEmployeeIdByUserId((long)UserId);
                var degignationOfEmployee = _employee.GetOfficeDesignationIdOfEmployee(employeeId);
                if (degignationOfEmployee > 0)
                {
                    assigned = degignationOfEmployee;
                }
            }

            var allCall = GenService.GetAll<Call>().Where(s => s.Status == EntityStatus.Active && s.CallStatus == CallStatus.Unfinished && ((s.CallType == CallType.Self && s.CreatedBy == UserId) || (s.CallType != CallType.Self && s.AssignedTo == assigned))).Select(s => new CallDto
            {
                Id = s.Id,
                CallId = s.CallId,
                IsOrganization = s.IsOrganization,
                CustomerName = s.CustomerName,
                CustomerPhone = s.CustomerPhone,
                CallStatus = s.CallStatus,
                CallSourceText = s.CallSourceText,
                CallTypeName = s.CallType.ToString(),
                CallStatusName = s.CallStatus.ToString(),
                CustomerPriority = s.CustomerPriority,
                CustomerPriorityName = s.CustomerPriority.ToString(),
                CallCategory = s.CallCategory,
                CallCategoryName = s.CallCategory.ToString(),
                CallCreatorName = s.CallCreatorName,
                CreateDate = s.CreateDate
            });
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                allCall = allCall.Where(c => c.CallId.ToLower().Contains(searchString) || c.CustomerName.ToLower().Contains(searchString) || c.CustomerPhone.ToLower().Contains(searchString));
            }
            var temp = allCall.OrderByDescending(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }
        public IPagedList<CallDto> GetUnsuccessfulCallPagedList(int pageSize, int pageCount, string searchString, long UserId)
        {
            long assigned = 0;

            long employeeId = _user.GetEmployeeIdByUserId((long)UserId);
            var degignationOfEmployee = _employee.GetOfficeDesignationIdOfEmployee(employeeId);
            List<long> employeeIds = new List<long>();
            List<long> userIds = new List<long>();
            userIds.Add(UserId);
            employeeIds = _employee.GetEmployeeWiseBM(employeeId).Select(e => (long)e.EmployeeId).ToList();
            employeeIds.Add(employeeId);
            if (degignationOfEmployee > 0)
            {
                assigned = degignationOfEmployee;
            }
            foreach (var emp in employeeIds)
            {
                var uId = _user.GetUserByEmployeeId(emp).Id;
                userIds.Add((long)uId);
            }


            var allCall = GenService.GetAll<Call>()
                .Where(s => s.Status == EntityStatus.Active
                && s.CallStatus == CallStatus.Unsuccessful
                && ((s.CallType == CallType.Self && userIds.Contains((long)s.CreatedBy))
                     || (s.CallType != CallType.Self && s.AssignedTo != null && employeeIds.Contains((long)s.AssignedTo))
                   )).Select(s => new CallDto
                   {
                       Id = s.Id,
                       CallId = s.CallId,
                       CustomerName = s.CustomerName,
                       CustomerPhone = s.CustomerPhone,
                       CallStatus = s.CallStatus,
                       CallSourceName = s.CallCategory.ToString(),
                       CallSourceText = s.CallSourceText,
                       CallTypeName = s.CallType.ToString(),
                       CallStatusName = s.CallStatus.ToString(),
                       FailedRemarks = s.FailedRemarks,
                       CallFailReason = s.CallFailReason,
                       CallCreatorName = s.CallCreatorName,
                       CreateDate = s.CreateDate
                   });
            if (!string.IsNullOrEmpty(searchString))
                allCall = allCall.Where(a => a.CallId.ToLower().Contains(searchString.ToLower())
                || a.CustomerName.ToLower().Contains(searchString.ToLower())
                || a.CustomerPhone.ToLower().Contains(searchString.ToLower())
                || a.CallSourceText.ToLower().Contains(searchString.ToLower()));
            var temp = allCall.OrderByDescending(r => r.Id).ToList();
            temp.ForEach(r => r.CallFailReasonName = (r.CallFailReason > 0 ? UiUtil.GetDisplayName(r.CallFailReason) : ""));


            return temp.ToPagedList(pageCount, pageSize);
        }
        public IPagedList<CallDto> GetSuccessfulCallPagedList(int pageSize, int pageCount, string searchString, long UserId)
        {
            long assigned = 0;

            long employeeId = _user.GetEmployeeIdByUserId((long)UserId);
            var degignationOfEmployee = _employee.GetOfficeDesignationIdOfEmployee(employeeId);
            List<long> employeeIds = new List<long>();
            List<long> userIds = new List<long>();
            userIds.Add(UserId);
            employeeIds = _employee.GetEmployeeWiseBM(employeeId).Select(e => (long)e.EmployeeId).ToList();
            employeeIds.Add(employeeId);
            if (degignationOfEmployee > 0)
            {
                assigned = degignationOfEmployee;
            }
            foreach (var emp in employeeIds)
            {
                var uId = _user.GetUserByEmployeeId(emp).Id;
                userIds.Add((long)uId);
            }


            var allCall = GenService.GetAll<Call>()
                .Where(s => s.Status == EntityStatus.Active
                && s.CallStatus == CallStatus.Successful
                && ((s.CallType == CallType.Self && userIds.Contains((long)s.CreatedBy))
                     || (s.CallType != CallType.Self && s.AssignedTo != null && employeeIds.Contains((long)s.AssignedTo))
                   )).Select(s => new CallDto
                   {
                       Id = s.Id,
                       CallId = s.CallId,
                       CustomerName = s.CustomerName,
                       CustomerPhone = s.CustomerPhone,
                       CallStatus = s.CallStatus,
                       CallSourceName = s.CallCategory.ToString(),
                       CallSourceText = s.CallSourceText,
                       CallTypeName = s.CallType.ToString(),
                       CallStatusName = s.CallStatus.ToString(),
                       FailedRemarks = s.FailedRemarks,
                       CallFailReason = s.CallFailReason,
                       CallCreatorName = s.CallCreatorName,
                       CreateDate = s.CreateDate
                       //CallFailReasonName = s.CallFailReason.ToString()
                   });
            if (!string.IsNullOrEmpty(searchString))
                allCall = allCall.Where(a => a.CallId.ToLower().Contains(searchString.ToLower())
                || a.CustomerName.ToLower().Contains(searchString.ToLower())
                || a.CustomerPhone.ToLower().Contains(searchString.ToLower())
                || a.CallSourceText.ToLower().Contains(searchString.ToLower()));
            var temp = allCall.OrderByDescending(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }
        public List<CallDto> GetAllCalls()
        {
            var result = Mapper.Map<List<CallDto>>(GenService.GetAll<Call>().ToList());
            return result;
        }
        public IPagedList<CallDto> GetOwnCallsPagedList(int pageSize, int pageCount, string searchString, long UserId)
        {
            var allCall = GenService.GetAll<Call>()
                .Where(s => s.CreatedBy == UserId).Select(s => new CallDto
                {
                    Id = s.Id,
                    CallId = s.CallId,
                    CustomerName = s.CustomerName,
                    CustomerPhone = s.CustomerPhone,
                    CallStatus = s.CallStatus,
                    CallSourceName = s.CallCategory.ToString(),
                    CallSourceText = s.CallSourceText,
                    CallTypeName = s.CallType.ToString(),
                    CallStatusName = s.CallStatus.ToString(),
                    FailedRemarks = s.FailedRemarks,
                    CallFailReason = s.CallFailReason,
                    CallCreatorName = s.CallCreatorName,
                    CreateDate = s.CreateDate
                });

            if (!string.IsNullOrEmpty(searchString))
                allCall = allCall.Where(a => a.CallId.ToLower().Contains(searchString.ToLower())
                || a.CustomerName.ToLower().Contains(searchString.ToLower())
                || a.CustomerPhone.ToLower().Contains(searchString.ToLower())
                || a.CallSourceText.ToLower().Contains(searchString.ToLower()));
            var temp = allCall.OrderByDescending(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }

        #region API functions
        public ResponseDto APISaveSuccessfulCall(long CallId, long UserId, DateTime FollowupTime)
        {
            var response = new ResponseDto();
            long employeeId = _user.GetEmployeeIdByUserId((long)UserId);
            long offDegSettingId = _employee.GetOfficeDesignationIdOfEmployee(employeeId);
            var entity = GenService.GetById<Call>(CallId);
            if (entity.ConvertedToLead != null && entity.ConvertedToLead == true)
            {
                response.Message = "This call has already been converted to lead. It cannot be edited again.";
                return response;
            }
            entity.CallStatus = CallStatus.Successful;
            entity.ConvertedToLead = true;
            entity.EditDate = DateTime.Now;
            entity.EditedBy = UserId;
            entity.FailedRemarks = null;
            entity.CallFailReason = null;
            GenService.Save(entity);
            GenService.SaveChanges();

            SalesLeadDto saleslead = new SalesLeadDto();
            saleslead = Mapper.Map<SalesLeadDto>(entity);
            saleslead.FollowupTime = FollowupTime;
            saleslead.LeadStatus = LeadStatus.Drafted;
            var salesLeadSave = _salesLeadFacade.SaveSalesLead(saleslead, UserId);
            if ((bool)salesLeadSave.Success)
            {
                response.Success = true;
                response.Message = "Call Saved as Lead";

                SalesLeadAssignmentDto selfAssignment = new SalesLeadAssignmentDto();
                selfAssignment.AssignedByEmpId = employeeId;
                selfAssignment.AssignedToEmpId = employeeId;
                selfAssignment.AssignedByOffDegId = offDegSettingId;
                selfAssignment.AssignedToOffDegId = offDegSettingId;
                selfAssignment.FollowUpTime = saleslead.FollowupTime;
                selfAssignment.SalesLeadId = salesLeadSave.Id;

                var selfAssignmentResult = _salesLeadFacade.SaveSalesLeadAssignment(selfAssignment, UserId);
            }

            return response;
        }
        public List<CallDto> RMUnfinishedCallsApi(int pageSize, int pageCount, string searchString, long UserId)
        {
            long assigned = 0;

            long employeeId = _user.GetEmployeeIdByUserId(UserId);
            var degignationOfEmployee = _employee.GetOfficeDesignationIdOfEmployee(employeeId);
            if (degignationOfEmployee > 0)
            {
                assigned = degignationOfEmployee;
            }

            var allCall = GenService.GetAll<Call>().Where(s => s.Status == EntityStatus.Active && s.CallStatus == CallStatus.Unfinished && ((s.CallType == CallType.Self && s.CreatedBy == UserId) || (s.CallType != CallType.Self && s.AssignedTo == assigned)));

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                allCall = allCall.Where(c => c.CallId.ToLower().Contains(searchString) || c.CustomerName.ToLower().Contains(searchString) || c.CustomerPhone.ToLower().Contains(searchString));
            }
            var temp = allCall.OrderByDescending(r => r.Id)
                .Skip((pageCount - 1) * pageSize)
                .Take(pageSize).ToList();
            return Mapper.Map<List<CallDto>>(temp);
        }
        public ResponseDto APISaveFailedCall(long CallId, long UserId, CallFailReason callFailReason, string failRemarks)
        {
            var response = new ResponseDto();
            long employeeId = _user.GetEmployeeIdByUserId((long)UserId);
            long offDegSettingId = _employee.GetOfficeDesignationIdOfEmployee(employeeId);
            var entity = GenService.GetById<Call>(CallId);
            try
            {
                if (entity.ConvertedToLead != null && entity.ConvertedToLead == true)
                {
                    response.Message = "This call has already been converted to lead. It cannot be edited again.";
                    return response;
                }
                entity.CallStatus = CallStatus.Unsuccessful;
                entity.ConvertedToLead = false;
                entity.CallFailReason = callFailReason;
                entity.FailedRemarks = failRemarks;
                entity.EditDate = DateTime.Now;
                entity.EditedBy = UserId;
                GenService.Save(entity);
                GenService.SaveChanges();
                response.Success = true;
                response.Message = "Call saved as failed.";
            }
            catch (Exception ex)
            {
                response.Message = "Save failed";
            }

            return response;

        }
        public List<CallDto> GetUnsuccessfulCallApi(int pageSize, int pageCount, string searchString, long UserId)
        {
            long assigned = 0;

            long employeeId = _user.GetEmployeeIdByUserId((long)UserId);
            var degignationOfEmployee = _employee.GetOfficeDesignationIdOfEmployee(employeeId);
            List<long> employeeIds = new List<long>();
            List<long> userIds = new List<long>();
            userIds.Add(UserId);
            employeeIds = _employee.GetEmployeeWiseBM(employeeId).Select(e => (long)e.EmployeeId).ToList();
            employeeIds.Add(employeeId);
            if (degignationOfEmployee > 0)
            {
                assigned = degignationOfEmployee;
            }
            foreach (var emp in employeeIds)
            {
                var uId = _user.GetUserByEmployeeId(emp).Id;
                userIds.Add((long)uId);
            }

            var allCall = GenService.GetAll<Call>()
                .Where(s => s.Status == EntityStatus.Active
                && s.CallStatus == CallStatus.Unsuccessful
                && ((s.CallType == CallType.Self && userIds.Contains((long)s.CreatedBy))
                     || (s.CallType != CallType.Self && s.AssignedTo != null && employeeIds.Contains((long)s.AssignedTo))
                   ));
            if (!string.IsNullOrEmpty(searchString))
                allCall = allCall.Where(a => a.CallId.ToLower().Contains(searchString.ToLower())
                || a.CustomerName.ToLower().Contains(searchString.ToLower())
                || a.CustomerPhone.ToLower().Contains(searchString.ToLower())
                || a.CallSourceText.ToLower().Contains(searchString.ToLower()));
            var temp = allCall.OrderByDescending(r => r.Id)
                .Skip((pageCount - 1) * pageSize)
                .Take(pageSize).ToList();
            return Mapper.Map<List<CallDto>>(temp);
        }
        public List<CallDto> GetBMCallForAssignmentApi(int pageSize, int pageCount, string searchString, long UserId)
        {
            IQueryable<Call> callQueryableAreaWise = GenService.GetAll<Call>().Where(s => s.Status == EntityStatus.Active && s.CallStatus == CallStatus.Unfinished && s.CallType == CallType.Auto_assign);

            long reffered = 0;

            long employeeId = _user.GetEmployeeIdByUserId(UserId);
            var degignationOfEmployee = _employee.GetOfficeDesignationIdOfEmployee(employeeId);
            if (degignationOfEmployee > 0)
            {
                reffered = degignationOfEmployee;
            }
            List<long> existingthanaIds = new List<long>();
            var thanaOfEmployee = _employee.GetThanaOfEmployee(employeeId);
            if (thanaOfEmployee.Any())
            {
                existingthanaIds = thanaOfEmployee.Select(d => d.Id).ToList();
            }
            callQueryableAreaWise = callQueryableAreaWise.Where(c => existingthanaIds.Contains((long)c.CustomerAddress.ThanaId));


            IQueryable<Call> callQueryableAsReferred = GenService.GetAll<Call>().Where(c => c.ReferredTo == reffered && c.Status == EntityStatus.Active && c.CallStatus == CallStatus.Unfinished);
            IQueryable<EmployeeDesignationMapping> employees = GenService.GetAll<EmployeeDesignationMapping>().Where(e => e.Status == EntityStatus.Active);


            //var allCif = (from areaWise in callQueryableAreaWise
            //              join employee in employees on areaWise.AssignedTo equals employee.OfficeDesignationSettingId into assignedToEmployee
            //              from assignedTo in assignedToEmployee.DefaultIfEmpty()
            //              select new CallDto//callQueryableAreaWise.Where(s => s.Status == EntityStatus.Active && s.CallType == CallType.Auto_assign && s.CallStatus == CallStatus.Unfinished || s.ReferredTo == reffered).Select(s => new CallDto
            //              {
            //                  Id = areaWise.Id,
            //                  CallId = areaWise.CallId,
            //                  CustomerName = areaWise.CustomerName,
            //                  CustomerPhone = areaWise.CustomerPhone,
            //                  CallSourceName = areaWise.CallCategory.ToString(),
            //                  CallSourceText = areaWise.CallSourceText,
            //                  CallTypeName = areaWise.CallType.ToString(),
            //                  CallStatusName = areaWise.CallStatus.ToString(),
            //                  AssignedTo = areaWise.AssignedTo,
            //                  AssignedToName = (assignedTo != null ? assignedTo.Employee.Person.FirstName + " " + assignedTo.Employee.Person.LastName : ""),
            //                  CallCreatorName = areaWise.CallCreatorName,
            //                  CreateDate = areaWise.CreateDate
            //              }
            //              )
            //              .Union(
            //                    from referredTo in callQueryableAsReferred
            //                    join employee in employees on referredTo.AssignedTo equals employee.OfficeDesignationSettingId into assignedToEmployee
            //                    from assignedTo in assignedToEmployee.DefaultIfEmpty()
            //                    select new CallDto
            //                    {
            //                        Id = referredTo.Id,
            //                        CallId = referredTo.CallId,
            //                        CustomerName = referredTo.CustomerName,
            //                        CustomerPhone = referredTo.CustomerPhone,
            //                        CallSourceName = referredTo.CallCategory.ToString(),
            //                        CallSourceText = referredTo.CallSourceText,
            //                        CallTypeName = referredTo.CallType.ToString(),
            //                        CallStatusName = referredTo.CallStatus.ToString(),
            //                        AssignedTo = referredTo.AssignedTo,
            //                        AssignedToName = (assignedTo != null ? assignedTo.Employee.Person.FirstName + " " + assignedTo.Employee.Person.LastName : ""),
            //                        CallCreatorName = referredTo.CallCreatorName,
            //                        CreateDate = referredTo.CreateDate
            //                    }
            //                );
            //if (!string.IsNullOrEmpty(searchString))
            //    allCif = allCif.Where(a => a.CallId.ToLower().Contains(searchString.ToLower())
            //    || a.CustomerName.ToLower().Contains(searchString.ToLower())
            //    || a.CustomerPhone.ToLower().Contains(searchString.ToLower())
            //    || a.CallSourceText.ToLower().Contains(searchString.ToLower()));
            //var temp = allCif.OrderByDescending(r => r.Id)
            //    .Skip((pageCount - 1) * pageSize)
            //    .Take(pageSize).ToList();
            //return Mapper.Map<List<CallDto>>(temp);


            var preUnion = callQueryableAreaWise.Union(callQueryableAsReferred);
            if (!string.IsNullOrEmpty(searchString))
                preUnion = preUnion.Where(a => a.CallId.ToLower().Contains(searchString.ToLower())
                || a.CustomerName.ToLower().Contains(searchString.ToLower())
                || a.CustomerPhone.ToLower().Contains(searchString.ToLower())
                || a.CallSourceText.ToLower().Contains(searchString.ToLower()));
            var calls = preUnion.OrderByDescending(r => r.Id)
                .Skip((pageCount - 1) * pageSize)
                .Take(pageSize).ToList();
            var callDtos = Mapper.Map<List<CallDto>>(calls);
            foreach (var item in callDtos.Where(c => c.AssignedTo != null))
            {
                var emp = employees.Where(e => e.OfficeDesignationSettingId == item.AssignedTo).FirstOrDefault();
                if (emp != null)
                    item.AssignedToName = emp.Employee.Person.FirstName + " " + emp.Employee.Person.LastName;
            }

            return callDtos;
        }
        #endregion
    }
}
