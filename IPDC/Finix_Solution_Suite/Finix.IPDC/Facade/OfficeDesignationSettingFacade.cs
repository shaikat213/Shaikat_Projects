using AutoMapper;
using Finix.IPDC.DTO;
using Finix.IPDC.Infrastructure.Models;
using Finix.IPDC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.Auth.Facade;
using Finix.Auth.Infrastructure;
using EntityStatus = Finix.IPDC.Infrastructure.EntityStatus;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.Facade
{
    public class OfficeDesignationSettingFacade : BaseFacade
    {
        //private readonly GenService _service = new GenService();
        private readonly UserFacade _user = new UserFacade();
        private readonly EmployeeFacade _employee = new EmployeeFacade();
        public List<OfficeDesignationSettingDto> GetAllActiveSettings()
        {
            var settings = GenService.GetAll<OfficeDesignationSetting>().ToList();
            return Mapper.Map<List<OfficeDesignationSettingDto>>(settings);
        }

        public ResponseDto SaveOfficeDesignationSetting(OfficeDesignationSettingDto dto, long UserId)
        {
            var response = new ResponseDto();
            try
            {
                if (dto.Id != null && dto.Id > 0)
                {
                    var setting = GenService.GetById<OfficeDesignationSetting>((long)dto.Id);
                    dto.CreateDate = setting.CreateDate;
                    dto.CreatedBy = setting.CreatedBy;
                    dto.EditDate = DateTime.Now;
                    dto.EditedBy = UserId;
                    Mapper.Map(dto, setting);
                    GenService.Save(setting);
                }
                else
                {
                    var setting = Mapper.Map<OfficeDesignationSetting>(dto);
                    setting.CreateDate = DateTime.Now;
                    setting.CreatedBy = UserId;
                    GenService.Save(setting);
                }
                GenService.SaveChanges();
            }
            catch (Exception)
            {
                response.Message = "Not saved. Please contact support.";
            }


            response.Message = "Setting Saved Successfully.";


            return response;
        }

        public List<OfficeDesignationSettingDto> GetSettingByOffice(long OfficeId)
        {
            var settings = GenService.GetAll<OfficeDesignationSetting>().Where(o => o.Status == Infrastructure.EntityStatus.Active && o.OfficeId == OfficeId).ToList();
            return Mapper.Map<List<OfficeDesignationSettingDto>>(settings);
        }

        public List<OfficeDesignationSettingDto> GetAllEmpByBM(long userId)
        {
            var employeeId = _user.GetEmployeeIdByUserId(userId);
            var getEmployeeWiseSR = _employee.GetEmployeeWiseBM(employeeId);
            return getEmployeeWiseSR.ToList();
            //var employees = GenService.GetAll<Employee>().Where(r => r.Id != employeeId);
            //return Mapper.Map<List<EmployeeDto>>(employees.ToList());
            //throw new NotImplementedException();
        }

        //public List<OfficeDesignationSettingDto> GetAllEmpByOffice(long empId)
        //{
        //    var officeId = _employee.GetEmployeeOfficeByEmployeeID(empId);
        //    var getEmployeeWiseSR = _employee.GetEmployeeWiseBM(employeeId);
        //    return getEmployeeWiseSR.ToList();
        //    var employees = GenService.GetAll<Employee>().Where(r => r.Id != employeeId);
        //    return Mapper.Map<List<EmployeeDto>>(employees.ToList());
        //    throw new NotImplementedException();
        //}
        public ResponseDto SaveEmployeeDesignation(EmployeeDesignationMappingDto dto, long UserId)
        {
            var response = new ResponseDto();
            try
            {
                if (dto.Id != null && dto.Id > 0)
                {
                    var setting = GenService.GetById<EmployeeDesignationMapping>((long)dto.Id);
                    dto.CreateDate = setting.CreateDate;
                    dto.CreatedBy = setting.CreatedBy;
                    dto.EditDate = DateTime.Now;
                    dto.EditedBy = UserId;
                    Mapper.Map(dto, setting);
                    GenService.Save(setting);
                }
                else
                {
                    var setting = Mapper.Map<EmployeeDesignationMapping>(dto);
                    setting.CreateDate = DateTime.Now;
                    setting.CreatedBy = UserId;
                    GenService.Save(setting);
                }
                GenService.SaveChanges();
            }
            catch (Exception)
            {
                response.Message = "Not saved. Please contact support.";
            }

            response.Message = "Setting Saved Successfully.";

            return response;
        }
        public List<OfficeDesignationSettingDto> GetOffDegSettingsForAssignment()
        {
            List<OfficeDesignationSettingDto> result = new List<OfficeDesignationSettingDto>();
            try
            {
                var designationSettingIds = GenService.GetAll<DesignationRoleMapping>().Where(d => d.RoleId == BizConstants.BmRoleId && d.Status == EntityStatus.Active).Select(d => d.OfficeDesignationSettingId).ToList();
                var settings = GenService.GetAll<EmployeeDesignationMapping>().Where(o => o.Status == EntityStatus.Active && designationSettingIds.Contains(o.OfficeDesignationSettingId));
                result = (from setting in settings
                          select new OfficeDesignationSettingDto()
                          {
                              Id = setting.OfficeDesignationSettingId,
                              Name = setting.Employee.Person.FirstName + " " + setting.Employee.Person.LastName + " - " + setting.OfficeDesignationSetting.Office.Name + " - " + setting.OfficeDesignationSetting.Designation.Name
                          }).ToList();
                //return result;
            }
            catch (Exception ex)
            {
                //
            }
            return result;
        }

        public List<OfficeDesignationSettingDto> GetEmployeeWithDesignation()
        {
            List<OfficeDesignationSettingDto> result = new List<OfficeDesignationSettingDto>();
            try
            {
                var settings = GenService.GetAll<EmployeeDesignationMapping>();
                result = (from setting in settings
                          select new OfficeDesignationSettingDto
                          {
                              Id = setting.EmployeeId,
                              DesignationId = setting.OfficeDesignationSettingId,

                              Name = setting.Employee.Person.FirstName + " " + setting.Employee.Person.LastName + " - " + setting.OfficeDesignationSetting.Office.Name + " - " + setting.OfficeDesignationSetting.Designation.Name
                          }).ToList();
                //return result;
            }
            catch (Exception ex)
            {
                //
            }
            return result;
        }

        public List<OfficeDesignationSettingDto> GetEmployeeWithDesignationOfffice(long userId)
        {
            var employeeId = _user.GetEmployeeIdByUserId(userId);
            var officeId = _employee.GetEmployeeOfficeByEmployeeID(employeeId);
            List<OfficeDesignationSettingDto> result = new List<OfficeDesignationSettingDto>();
            try
            {
                var settings = GenService.GetAll<EmployeeDesignationMapping>().Where(s => s.OfficeDesignationSetting.OfficeId == officeId);
                result = (from setting in settings
                          select new OfficeDesignationSettingDto
                          {
                              Id = setting.EmployeeId,
                              DesignationId = setting.OfficeDesignationSettingId,
                              OfficeId = officeId,
                              Name = setting.Employee.Person.FirstName + " " + setting.Employee.Person.LastName + " - " + setting.OfficeDesignationSetting.Office.Name + " - " + setting.OfficeDesignationSetting.Designation.Name
                          }).ToList();
                //return result;
            }
            catch (Exception ex)
            {
                //
            }
            return result;
        }

        public List<EmployeeDesignationMappingDto> GetEmployeesOfCurrrentDesignation(long settingId)
        {
            var result = GenService.GetAll<EmployeeDesignationMapping>().Where(e => e.OfficeDesignationSettingId == settingId && e.Status == EntityStatus.Active);
            return Mapper.Map<List<EmployeeDesignationMappingDto>>(result.ToList());
        }

        public ResponseDto RemoveDesignatedEmployee(EmployeeDesignationMappingDto dto, long UserId)
        {
            var result = new ResponseDto();
            if (dto.Id != null && dto.Id > 0)
            {
                var setting = GenService.GetById<EmployeeDesignationMapping>((long)dto.Id);
                if (setting == null)
                    result.Message = "No Data Found.";
                if (dto.EmployeeId == setting.EmployeeId && dto.OfficeDesignationSettingId == setting.OfficeDesignationSettingId)
                {
                    setting.Status = Infrastructure.EntityStatus.Inactive;
                    setting.EditDate = DateTime.Now;
                    setting.EditedBy = UserId;
                    GenService.Save(setting);
                    GenService.SaveChanges();
                    result.Message = "Employee removed from the designation.";
                    result.Success = true;
                }
                else
                    result.Message = "No Data Found.";

            }
            else
            {
                result.Message = "No Data Found.";
            }
            return result;
        }

        public ResponseDto DeleteOfficeDesignationSetting(int id)
        {
            ResponseDto responce = new ResponseDto();
            var entity = GenService.GetById<OfficeDesignationSetting>(id);
            List<OfficeDesignationArea> officeDesgAreaMap = GenService.GetAll<OfficeDesignationArea>().Where(r => r.OfficeDesignationSettingId == entity.Id && r.Status == EntityStatus.Active).ToList().Select(c => { c.Status = EntityStatus.Inactive; return c; }).ToList();
            List<EmployeeDesignationMapping> empDesgAreaMap = GenService.GetAll<EmployeeDesignationMapping>().Where(r => r.OfficeDesignationSettingId == entity.Id && r.Status == EntityStatus.Active).ToList().Select(c => { c.Status = EntityStatus.Inactive; return c; }).ToList();
            entity.Status = EntityStatus.Inactive;
            GenService.Save(entity);
            GenService.Save(officeDesgAreaMap);
            GenService.Save(empDesgAreaMap);
            responce.Success = true;
            responce.Message = "Office Designation Mapping Removed Successfully";
            return responce;
        }

        public List<EmployeeDto> GetEmployeesByOfficeId(long officeId)
        {
            var empTestList = GenService.GetAll<EmployeeDesignationMapping>()
                .Where(e => e.Status == EntityStatus.Active &&
                            e.OfficeDesignationSetting.Status == EntityStatus.Active &&
                            e.OfficeDesignationSetting.OfficeId == officeId &&
                            e.Employee.Status == EntityStatus.Active)
                .Select(e => e.Employee).ToList();
            return Mapper.Map<List<EmployeeDto>>(empTestList);
        }
        public List<EmployeeDto> GetEmployeesByOfficeSettingId(long officeId)
        {
            var empTestList = GenService.GetAll<EmployeeDesignationMapping>()
                .Where(e => e.Status == EntityStatus.Active &&
                            e.OfficeDesignationSetting.Status == EntityStatus.Active &&
                            e.OfficeDesignationSetting.Id == officeId &&
                            e.Employee.Status == EntityStatus.Active)
                .Select(e => e.Employee);
            return Mapper.Map<List<EmployeeDto>>(empTestList.ToList());
        }
        public List<long> GetEmployeeIdsOfSupervisorsOfSameOffice(long empId)
        {
            var empList = new List<long>();
            var empOrganogram = GenService.GetAll<EmployeeDesignationMapping>()
                .Where(e => e.Status == EntityStatus.Active &&
                            e.EmployeeId == empId).OrderByDescending(e => e.Id).FirstOrDefault();

            if (empOrganogram != null)
            {
                if (empOrganogram.OfficeDesignationSetting.ParentDesignationSettingId != null)
                {
                    var supervisor = GenService.GetAll<EmployeeDesignationMapping>()
                        .Where(e => e.Status == EntityStatus.Active &&
                                    e.OfficeDesignationSettingId == empOrganogram.OfficeDesignationSetting.ParentDesignationSettingId &&
                                    e.OfficeDesignationSetting.OfficeId == empOrganogram.OfficeDesignationSetting.OfficeId &&
                                    e.OfficeDesignationSetting.OfficeId != BizConstants.IPDCHeadOfficeId
                               ).OrderByDescending(e => e.Id).FirstOrDefault();
                    if (supervisor != null && supervisor.EmployeeId != empId)
                    {
                        empList.Add(supervisor.EmployeeId);
                        empList.AddRange(GetEmployeeIdsOfSupervisorsOfSameOffice(supervisor.EmployeeId));
                    }
                }
            }

            return empList.Distinct().ToList();
        }


    }
}
