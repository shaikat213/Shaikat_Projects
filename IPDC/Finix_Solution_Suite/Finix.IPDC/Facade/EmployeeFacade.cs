using Finix.IPDC.DTO;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using AutoMapper;
using Finix.Auth.Facade;
using Finix.Auth.Infrastructure;
using BloodGroup = Finix.IPDC.Infrastructure.BloodGroup;
using EmployeeType = Finix.IPDC.Infrastructure.EmployeeType;
using EntityStatus = Finix.IPDC.Infrastructure.EntityStatus;
using Gender = Finix.IPDC.Infrastructure.Gender;
using MaritalStatus = Finix.IPDC.Infrastructure.MaritalStatus;
using Religion = Finix.IPDC.Infrastructure.Religion;
using System.Text.RegularExpressions;

namespace Finix.IPDC.Facade
{
    public class EmployeeFacade : BaseFacade
    {
        private readonly UserFacade _user = new UserFacade();
        public List<EmployeeDto> GetEmployees()
        {
            var employees = GenService.GetAll<Employee>().Where(x => x.Status == EntityStatus.Active);

            List<EmployeeDto> data = new List<EmployeeDto>();
            foreach (var emp in employees.ToList())
            {
                var dto = new EmployeeDto();
                dto.Id = emp.Id;
                dto.BasicInfo = new EmpBasicInfoDto
                {
                    Id = emp.Person.Id,
                    Name = emp.Person.FirstName + " " + emp.Person.LastName,
                    FirstName = emp.Person.FirstName,
                    LastName = emp.Person.LastName,
                    DateOfBirth = emp.Person.DateOfBirth,
                    BloodGroup = (int)emp.Person.BloodGroup,
                    Gender = (int)emp.Person.Gender,
                    MaritalStatus = (int)emp.Person.MaritalStatus,
                    Religion = (int)emp.Person.Religion,
                    NID = emp.Person.NID,
                    FatherFirstName = emp.Person.FatherId == null ? "" : emp.Person.Father.FirstName,
                    FatherLastName = emp.Person.FatherId == null ? "" : emp.Person.Father.LastName,
                    MotherFirstName = emp.Person.MotherId == null ? "" : emp.Person.Mother.FirstName,
                    MotherLastName = emp.Person.MotherId == null ? "" : emp.Person.Mother.LastName,
                    Photo = emp.Person.Photo == null ? "" : emp.Person.Photo
                    //CountryId = emp.Person.CountryId

                };
                dto.ContactInformation = new ContactInformationDto
                {

                };

                dto.UserInformation = new UserInformationDto
                {

                };
                data.Add(dto);
            }
            return data;

        }

        public List<EmpBasicInfoDto> GetEmployeeList(string prefix)
        {
            var employees = GenService.GetAll<Employee>().Where(x => x.Status == EntityStatus.Active);
            if (!string.IsNullOrEmpty(prefix))
            {
                employees = employees.Where(e => e.Person.FirstName.ToLower().Contains(prefix) || e.Person.LastName.ToLower().Contains(prefix));
            }
            var data = employees.Select(x => new EmpBasicInfoDto
            {
                Id = x.Id,
                FirstName = x.Person.FirstName,
                LastName = x.Person.LastName,
                DateOfBirth = x.Person.DateOfBirth,
                BloodGroup = (int)x.Person.BloodGroup,
                Gender = (int)x.Person.Gender,
                MaritalStatus = (int)x.Person.MaritalStatus,
                Religion = (int)x.Person.Religion,
                NID = x.Person.NID,
                FatherFirstName = x.Person.FatherId == null ? "" : x.Person.Father.FirstName,
                FatherLastName = x.Person.FatherId == null ? "" : x.Person.Father.LastName,
                MotherFirstName = x.Person.MotherId == null ? "" : x.Person.Mother.FirstName,
                MotherLastName = x.Person.MotherId == null ? "" : x.Person.Mother.LastName,
                //CountryId = x.Person.CountryId,

                EmployeeType = x.EmployeeType,
                EmployeeTypeName = x.EmployeeType.ToString()
            }).ToList();
            return data;
        }

        public ResponseDto SaveEmployee(EmpBasicInfoDto basicinfodto, int? id = null)
        {
            var response = new ResponseDto();
            Employee emp = GenService.GetAll<Employee>().SingleOrDefault(x => x.Id == id);
            if (emp != null)
            {
                emp.Person.FirstName = basicinfodto.FirstName;
                emp.Person.LastName = basicinfodto.LastName;
                emp.Person.BloodGroup = (BloodGroup)basicinfodto.BloodGroup;
                emp.Person.Nationality = basicinfodto.Nationality;
                emp.IMEINo = basicinfodto.IMEINo;
                //emp.Person.CountryId = basicinfodto.CountryId;
                emp.Person.DateOfBirth = basicinfodto.DateOfBirth;
                if (emp.Person.FatherId == null)
                {
                    emp.Person.Father = new Person
                    {
                        FirstName = basicinfodto.FatherFirstName,
                        LastName = basicinfodto.FatherLastName,
                        Gender = Gender.Male,
                        MaritalStatus = MaritalStatus.Married,
                        Religion = (Religion)basicinfodto.Religion,
                        //CountryId = basicinfodto.CountryId

                    };
                }
                else
                {
                    emp.Person.Father.FirstName = basicinfodto.FatherFirstName;
                    emp.Person.Father.LastName = basicinfodto.FatherLastName;
                }

                if (emp.Person.MotherId == null)
                {
                    emp.Person.Mother = new Person
                    {
                        FirstName = basicinfodto.MotherFirstName,
                        LastName = basicinfodto.MotherLastName,
                        Gender = Gender.Female,
                        MaritalStatus = MaritalStatus.Married,
                        Religion = (Religion)basicinfodto.Religion,
                        //CountryId = basicinfodto.CountryId
                    };
                }
                else
                {
                    emp.Person.Mother.FirstName = basicinfodto.MotherFirstName;
                    emp.Person.Mother.LastName = basicinfodto.MotherLastName;
                }

                emp.Person.NID = basicinfodto.NID;
                emp.Person.Religion = (Religion)basicinfodto.Religion;
                emp.Person.MaritalStatus = (MaritalStatus)basicinfodto.MaritalStatus;
                emp.Person.Gender = (Gender)basicinfodto.Gender;

            }
            else
            {
                Person p;
                p = new Person();
                //GenService.Save(p= new Person
                //{
                //p.CountryId = basicinfodto.CountryId;
                p.DateOfBirth = basicinfodto.DateOfBirth;
                p.Father = new Person
                {
                    FirstName = basicinfodto.FatherFirstName,
                    LastName = basicinfodto.FatherLastName,
                    Gender = Gender.Male,
                    MaritalStatus = MaritalStatus.Married,
                    Religion = (Religion)basicinfodto.Religion,
                    //CountryId = basicinfodto.CountryId

                };
                p.Mother = new Person
                {
                    FirstName = basicinfodto.MotherFirstName,
                    LastName = basicinfodto.MotherLastName,
                    Gender = Gender.Female,
                    MaritalStatus = MaritalStatus.Married,
                    Religion = (Religion)basicinfodto.Religion
                };
                p.BloodGroup = (BloodGroup)basicinfodto.BloodGroup;
                p.Gender = (Gender)basicinfodto.Gender;
                p.MaritalStatus = (MaritalStatus)basicinfodto.MaritalStatus;
                p.NID = basicinfodto.NID;
                p.Religion = (Religion)basicinfodto.Religion;
                p.FirstName = basicinfodto.FirstName;
                p.LastName = basicinfodto.LastName;
                p.Nationality = basicinfodto.Nationality;
                //});

                GenService.Save(p);
                //GenService.Save(new Employee
                //{
                Employee e = new Employee();
                e.PersonId = p.Id;
                e.EmployeeType = (EmployeeType)basicinfodto.EmployeeType;
                e.IMEINo = basicinfodto.IMEINo;
                //e.EmployeeType = basicinfodto.EmployeeType;
                e.Status = EntityStatus.Active;
                GenService.Save(e);
                //JoiningDate=basicinfodto.JoiningDate
                //});

            }
            try
            {
                GenService.SaveChanges();
                response.Success = true;
                response.Message = "Employee Saved.";
            }
            catch (Exception ex)
            {
                response.Message = "Employee not saved.";
            }

            return response;
        }

        public ResponseDto SaveContactInfo(ContactInformationDto contactinfodto, int id)
        {
            var response = new ResponseDto();
            Employee emp = GenService.GetAll<Employee>().SingleOrDefault(x => x.Id == id);
            if (emp.Person.PresentAddressId == null && emp.Person.PermanentAddressId == null)
            {
                emp.Person.PermanentAddress = new Address
                {
                    AddressLine1 = contactinfodto.ParmanentAddress.AddressLine1,
                    AddressLine2 = contactinfodto.ParmanentAddress.AddressLine2,
                    ThanaId = contactinfodto.ParmanentAddress.ThanaId,
                    DistrictId = contactinfodto.ParmanentAddress.DistrictId

                };
                emp.Person.PresentAddress = new Address
                {
                    AddressLine1 = contactinfodto.PresentAddress.AddressLine1,
                    AddressLine2 = contactinfodto.PresentAddress.AddressLine2,
                    ThanaId = contactinfodto.PresentAddress.ThanaId,
                    DistrictId = contactinfodto.PresentAddress.DistrictId
                };
            }
            else
            {
                if (emp.Person.PresentAddressId != null && emp.Person.PresentAddressId > 0)
                {
                    emp.Person.PresentAddress.AddressLine1 = contactinfodto.ParmanentAddress.AddressLine1;
                    emp.Person.PresentAddress.AddressLine2 = contactinfodto.ParmanentAddress.AddressLine2;
                    emp.Person.PresentAddress.ThanaId = contactinfodto.ParmanentAddress.ThanaId;
                    emp.Person.PresentAddress.DistrictId = contactinfodto.ParmanentAddress.DistrictId;
                }
                if (emp.Person.PermanentAddressId == null && emp.Person.PermanentAddressId > 0)
                {
                    emp.Person.PresentAddress.AddressLine1 = contactinfodto.PresentAddress.AddressLine1;
                    emp.Person.PresentAddress.AddressLine2 = contactinfodto.PresentAddress.AddressLine2;
                    emp.Person.PresentAddress.ThanaId = contactinfodto.PresentAddress.ThanaId;
                    emp.Person.PresentAddress.DistrictId = contactinfodto.PresentAddress.DistrictId;
                }

            }
            emp.Person.PhoneNo = contactinfodto.PhoneNo;
            emp.Person.Email = contactinfodto.Email;
            emp.Person.EmergencyContactPerson = new Person { FirstName = contactinfodto.EmergencyContactPerson };
            //emp.Person.EmergencyContactPerson = contactinfodto.EmergencyContactPerson;
            emp.Person.EmergencyContactPersonPhone = contactinfodto.EmergencyContactPhone;
            emp.Person.EmergencyContactPersonRelation = contactinfodto.EmergencyContactRelation;
            try
            {
                GenService.SaveChanges();
                response.Message = "Contact Information saved.";
                response.Success = true;
            }
            catch (Exception)
            {
                response.Message = "Contact Information not saved.";
            }

            return response;
        }

        public ContactInformationDto GetContactInfoById(int empid)
        {
            var emp = GenService.GetById<Employee>(empid);
            ContactInformationDto contactdto = new ContactInformationDto();
            if (emp.Person.PresentAddressId != null && emp.Person.PermanentAddressId != null)
            {
                contactdto.PresentAddress = new AddressDto
                {
                    AddressLine1 = emp.Person.PresentAddress.AddressLine1,
                    AddressLine2 = emp.Person.PresentAddress.AddressLine2,
                    ThanaId = emp.Person.PresentAddress.ThanaId,
                    ThanaName = emp.Person.PresentAddress.Thana.ThanaNameEng,
                    DistrictId = emp.Person.PresentAddress.DistrictId,
                    DistrictName = emp.Person.PresentAddress.District.DistrictNameEng
                    //PostalCode = emp.Person.PresentAddress.PostOffice.Code
                };
                contactdto.ParmanentAddress = new AddressDto
                {
                    AddressLine1 = emp.Person.PermanentAddress.AddressLine1,
                    AddressLine2 = emp.Person.PermanentAddress.AddressLine2,
                    ThanaId = emp.Person.PermanentAddress.ThanaId,
                    ThanaName = emp.Person.PermanentAddress.Thana.ThanaNameEng,
                    DistrictId = emp.Person.PermanentAddress.DistrictId,
                    DistrictName = emp.Person.PermanentAddress.District.DistrictNameEng
                    //PostalCode = emp.Person.PermanentAddress.PostOffice.Code
                };
                contactdto.PhoneNo = emp.Person.PhoneNo;
                contactdto.Email = emp.Person.Email;
                contactdto.EmergencyContactPerson = emp.Person.EmergencyContactPerson == null ? "" : emp.Person.EmergencyContactPerson.FirstName;
                contactdto.EmergencyContactPhone = emp.Person.EmergencyContactPersonPhone;
                contactdto.EmergencyContactRelation = emp.Person.EmergencyContactPersonRelation;
            }

            return contactdto;
        }

        public EmpBasicInfoDto GetBasicInfoById(int empid)
        {
            var emp = GenService.GetById<Employee>(empid);
            EmpBasicInfoDto basicdto = new EmpBasicInfoDto();
            basicdto.FirstName = emp.Person.FirstName;
            basicdto.LastName = emp.Person.LastName;
            if (emp.Person.Father != null)
            {
                basicdto.FatherFirstName = emp.Person.Father.FirstName;
                basicdto.FatherLastName = emp.Person.Father.LastName;
            }
            if (emp.Person.Mother != null)
            {
                basicdto.MotherFirstName = emp.Person.Mother.FirstName;
                basicdto.MotherLastName = emp.Person.Mother.LastName;
            }
            basicdto.DateOfBirth = emp.Person.DateOfBirth;
            basicdto.Gender = (int)emp.Person.Gender;
            basicdto.MaritalStatus = (int)emp.Person.MaritalStatus;
            basicdto.Religion = (int)emp.Person.Religion;
            basicdto.BloodGroup = (int)emp.Person.BloodGroup;
            basicdto.IMEINo = emp.IMEINo;
            basicdto.Nationality = emp.Person.Nationality;
            //basicdto.CountryId = emp.Person.CountryId;
            basicdto.NID = emp.Person.NID;
            return basicdto;
        }

        public List<ThanaDto> GetThanaOfEmployee(long employeeId)
        {
            var empDesgMapping = GenService.GetAll<EmployeeDesignationMapping>().Where(r => r.EmployeeId == employeeId && r.Status == EntityStatus.Active);
            var data = (from edp in empDesgMapping
                        join eds in
                            GenService.GetAll<OfficeDesignationArea>().Where(r => r.UpozilaOrThana == UpozilaOrThana.Thana && r.Status == EntityStatus.Active) on
                            edp.OfficeDesignationSettingId equals eds.OfficeDesignationSettingId
                        select new ThanaDto
                        {
                            Id = (long)eds.RefId
                        }).ToList();
            return data;
        }


        public long GetOfficeDesignationIdOfEmployee(long employeeId)
        {
            var empDesgMapping = GenService.GetAll<EmployeeDesignationMapping>().Where(r => r.EmployeeId == employeeId && r.Status == EntityStatus.Active).FirstOrDefault();

            if (empDesgMapping != null)
            {
                return empDesgMapping.OfficeDesignationSettingId;
            }


            return 0;
        }

        public List<long> GetEmpRoleIdList(long empId)
        {
            var empDesgMapping = GenService.GetAll<EmployeeDesignationMapping>().Where(r => r.EmployeeId == empId && r.Status == EntityStatus.Active).Select(r => r.OfficeDesignationSettingId).ToList();
            if (empDesgMapping != null)
            {
                var roles = GenService.GetAll<DesignationRoleMapping>().Where(d => empDesgMapping.Contains(d.OfficeDesignationSettingId) && d.Status == EntityStatus.Active)
                .Select(d => d.RoleId).ToList();

                return roles;

            }
            return null;
        }

        public List<long> GetEmployeeIdsByRoleId(long roleId)
        {
            var mappings = GenService.GetAll<DesignationRoleMapping>()
                .Where(d => d.RoleId == roleId &&
                            d.Status == EntityStatus.Active)
                .Select(d => d.OfficeDesignationSettingId).Distinct().ToList();
            if(mappings.Count > 0)
            {
                var employees = GenService.GetAll<EmployeeDesignationMapping>()
                    .Where(e => e.Status == EntityStatus.Active &&
                                mappings.Contains(e.OfficeDesignationSettingId))
                    .Select(e => e.EmployeeId).Distinct().ToList();
                return employees;
            }
            return null;
        }

        public long GetEmployeeSupervisorEmpId(long employeeId)
        {
            var empDesgMapping = GenService.GetAll<EmployeeDesignationMapping>().Where(r => r.EmployeeId == employeeId && r.Status == EntityStatus.Active).FirstOrDefault();

            if (empDesgMapping != null)
            {
                var parentDegId = empDesgMapping.OfficeDesignationSetting.ParentOfficeDesignationSetting.Id;
                var empParentDesgMapping = GenService.GetAll<EmployeeDesignationMapping>().Where(r => r.OfficeDesignationSettingId == parentDegId && r.Status == EntityStatus.Active).FirstOrDefault();
                if (empParentDesgMapping != null)
                    return empParentDesgMapping.EmployeeId;
            }


            return 0;
        }
        public List<OfficeDesignationSettingDto> GetEmployeeWiseSR(long employeeId)
        {
            var empDesMap = GenService.GetAll<EmployeeDesignationMapping>().Where(r => r.EmployeeId == employeeId && r.Status == EntityStatus.Active);
            var data = (from edm in empDesMap
                        join ods in GenService.GetAll<OfficeDesignationSetting>() on edm.OfficeDesignationSettingId equals ods.Id
                        join odsSub in GenService.GetAll<OfficeDesignationSetting>() on ods.Id equals odsSub.ParentDesignationSettingId
                        join subEdmAreaSet in GenService.GetAll<EmployeeDesignationMapping>().Where(r => r.Status == EntityStatus.Active) on odsSub.Id equals subEdmAreaSet.OfficeDesignationSettingId
                        //join dsAreaSubStng in GenService.GetAll<OfficeDesignationArea>() on new  { odsSub.Id, edmAreaSet.UpozilaOrThana, edmAreaSet.RefId } equals new {dsAreaSubStng.OfficeDesignationSettingId,dsAreaSubStng.UpozilaOrThana,dsAreaSubStng.RefId } into extr
                        select new OfficeDesignationSettingDto()
                        {
                            Id = odsSub.Id,
                            OfficeId = odsSub.OfficeId,
                            OfficeName = odsSub.Office.Name,
                            DesignationId = odsSub.DesignationId,
                            DesignationName = odsSub.Designation.Name,
                            ParentDesignationSettingId = odsSub.ParentDesignationSettingId,
                            ParentDesignationSettingName = odsSub.ParentOfficeDesignationSetting.Designation.Name,
                            EmployeeId = subEdmAreaSet.EmployeeId,
                            EmployeeName = subEdmAreaSet.Employee.Person.FirstName + " " + subEdmAreaSet.Employee.Person.FirstName
                        });

            return data.ToList();
        }

        public List<OfficeDesignationSettingDto> GetEmployeeWiseBM(long employeeId)
        {
            var empDesMap = GenService.GetAll<EmployeeDesignationMapping>().Where(r => r.EmployeeId == employeeId && r.Status == EntityStatus.Active);
            var emp = GenService.GetById<Employee>(employeeId);
            var prs = emp != null ? emp.Person : null;
            string employeeName = "";
            if (prs != null)
            {
                employeeName = prs.Name;
            }

            var data = (from edm in empDesMap
                        join ods in GenService.GetAll<OfficeDesignationSetting>() on edm.OfficeDesignationSettingId equals ods.Id
                        join odsSub in GenService.GetAll<OfficeDesignationSetting>() on ods.Id equals odsSub.ParentDesignationSettingId
                        join subEdmAreaSet in GenService.GetAll<EmployeeDesignationMapping>().Where(r => r.Status == EntityStatus.Active) on odsSub.Id equals subEdmAreaSet.OfficeDesignationSettingId

                        //join dsAreaSubStng in GenService.GetAll<OfficeDesignationArea>() on new  { odsSub.Id, edmAreaSet.UpozilaOrThana, edmAreaSet.RefId } equals new {dsAreaSubStng.OfficeDesignationSettingId,dsAreaSubStng.UpozilaOrThana,dsAreaSubStng.RefId } into extr
                        select new OfficeDesignationSettingDto()
                        {
                            Id = subEdmAreaSet.OfficeDesignationSettingId,//.OfficeDesignationSettingId,
                            Name = subEdmAreaSet.Employee.Person.FirstName + " " + subEdmAreaSet.Employee.Person.LastName + " - " + odsSub.Office.Name + " - " + odsSub.Designation.Name,
                            EmployeeId = subEdmAreaSet.EmployeeId,
                            ParentEmployeeId = employeeId,
                            ParentEmployeeName = employeeName,
                            IMEI = subEdmAreaSet.Employee.IMEINo
                        });

            var subordinates = data.ToList();
            var inclusiveList = data.ToList();
            if (subordinates == null || subordinates.Count < 1)
                return new List<OfficeDesignationSettingDto>();

            foreach (var subordinate in subordinates.Where(s => s.EmployeeId != employeeId))
            {
                var secondStep = GetEmployeeWiseBM((long)subordinate.EmployeeId);
                inclusiveList.AddRange(secondStep);
            }

            return inclusiveList;
        }

        public long GetEmployeeOfficeByEmployeeID(long EmployeeId)
        {
            long officeId = 0;
            var empDesignation = GenService.GetAll<EmployeeDesignationMapping>().Where(e => e.EmployeeId == EmployeeId && e.Status == EntityStatus.Active)
                .OrderByDescending(e => e.Id).FirstOrDefault();
            if (empDesignation != null)
            {
                if (empDesignation.OfficeDesignationSetting != null)
                {
                    officeId = empDesignation.OfficeDesignationSetting.OfficeId;
                }
            }

            if (officeId > 0)
                return officeId;
            return 0;
        }

        public EmployeeDto GetEmployeeByEmployeeId(long EmployeeId)
        {
            var employee = new EmployeeDto();
            var emp = GenService.GetById<Employee>(EmployeeId);
            if (emp != null)
            {
                var result = AutoMapper.Mapper.Map<EmployeeDto>(emp);
                result.Photo = emp.Person.Photo == null ? "" : emp.Person.Photo;
                return result;
            }

            return employee;
        }

        public object GetOfficeAndDesignationByEmployeeId(long EmployeeId)
        {

            var empDesignation = GenService.GetAll<EmployeeDesignationMapping>().Where(e => e.EmployeeId == EmployeeId && e.Status == EntityStatus.Active)
                .OrderByDescending(e => e.Id).FirstOrDefault();
            if (empDesignation != null)
            {
                if (empDesignation.OfficeDesignationSetting != null)
                {
                    var data = new { Office = empDesignation.OfficeDesignationSetting.Office.Name, Designation = empDesignation.OfficeDesignationSetting.Designation.Name };
                    return data;
                }
            }

            return null;
        }

        public string GetDesignationByEmployeeId(long EmployeeId)
        {

            var empDesignation = GenService.GetAll<EmployeeDesignationMapping>().Where(e => e.EmployeeId == EmployeeId && e.Status == EntityStatus.Active)
                .OrderByDescending(e => e.Id).FirstOrDefault();
            if (empDesignation != null)
            {
                if (empDesignation.OfficeDesignationSetting != null)
                {
                    var data = empDesignation.OfficeDesignationSetting.Designation.Name;
                    return data;
                }
            }

            return null;
        }
        public List<OfficeDesignationSettingDto> GetEmployeeWiseSubordinate(long employeeId)
        {
            var empDesMap = GenService.GetAll<EmployeeDesignationMapping>().Where(r => r.EmployeeId == employeeId && r.Status == EntityStatus.Active);
            string employeeName = GenService.GetById<Employee>(employeeId).Person.Name;
            var data = (from edm in empDesMap
                        join ods in GenService.GetAll<OfficeDesignationSetting>() on edm.OfficeDesignationSettingId equals ods.Id
                        join odsSub in GenService.GetAll<OfficeDesignationSetting>() on ods.Id equals odsSub.ParentDesignationSettingId
                        join subEdmAreaSet in GenService.GetAll<EmployeeDesignationMapping>().Where(r => r.Status == EntityStatus.Active) on odsSub.Id equals subEdmAreaSet.OfficeDesignationSettingId

                        //join dsAreaSubStng in GenService.GetAll<OfficeDesignationArea>() on new  { odsSub.Id, edmAreaSet.UpozilaOrThana, edmAreaSet.RefId } equals new {dsAreaSubStng.OfficeDesignationSettingId,dsAreaSubStng.UpozilaOrThana,dsAreaSubStng.RefId } into extr
                        select new OfficeDesignationSettingDto()
                        {
                            Id = subEdmAreaSet.OfficeDesignationSettingId,//.OfficeDesignationSettingId,
                            Name = subEdmAreaSet.Employee.Person.FirstName + " " + subEdmAreaSet.Employee.Person.LastName,//+ " - " + odsSub.Office.Name + " - " + odsSub.Designation.Name,
                            EmployeeId = subEdmAreaSet.EmployeeId,
                            ParentEmployeeId = employeeId,
                            ParentEmployeeName = employeeName,
                            //RoleName = subEdmAreaSet.Employee
                        });

            var subordinates = data.ToList();
            var inclusiveList = data.ToList();
            if (subordinates == null || subordinates.Count < 1)
                return new List<OfficeDesignationSettingDto>();

            //foreach (var subordinate in subordinates.Where(s => s.EmployeeId != employeeId))
            //{
            //    var secondStep = GetEmployeeWiseBM((long)subordinate.EmployeeId);
            //    inclusiveList.AddRange(secondStep);
            //}

            return inclusiveList;
        }


        public long GetEmployeeIdByOfficeId(long officeId)
        {
            //var empList = GenService.GetAll<OfficeDesignationSetting>().Where(o=>o.OfficeId == officeId).Select(o=>o)
            var AllEmp = GenService.GetAll<EmployeeDesignationMapping>()
                .Where(e => e.Status == EntityStatus.Active &&
                            e.OfficeDesignationSetting.Status == EntityStatus.Active &&
                            e.OfficeDesignationSetting.OfficeId == officeId &&
                            e.Employee.Status == EntityStatus.Active)
                .Select(e => e.Employee);
            var result = Mapper.Map<EmployeeDto>(AllEmp.FirstOrDefault());
            return (long)result.Id;
        }

        public ResponseDto UploadPicture(PersonDto dto, long userId)
        {
            ResponseDto response = new ResponseDto();
            string filePath = "~/UploadedFiles/ProfilePicture/";//"~/Uploaded/ProfilePicture/";
            string path = HttpContext.Current.Server.MapPath(filePath);
            try
            {
                if (dto.Photo != null)
                {
                    if (dto.Photo != null)
                    {
                        long employeeId = _user.GetEmployeeIdByUserId(userId);
                        var employee = GenService.GetById<Employee>(employeeId);
                        //if (!string.IsNullOrEmpty(employee.EmpCode))
                        //{
                        //    path += employee.EmpCode + "/"+ employee.Person.FirstName + "/";
                        //}
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        string customfileName = "Profile_Picture_" + employee.Person.FirstName;
                        string fileExt = Path.GetExtension(dto.Photo.FileName);
                        path = Path.Combine(path, customfileName + fileExt);
                        //path = Path.Combine(path, dto.Photo.FileName);
                        for (int i = 1; File.Exists(path);)
                        {
                            var length = path.Length;
                            if (!string.IsNullOrEmpty(fileExt))
                                path = path.Substring(0, (length - 3 - fileExt.Length)) + (++i).ToString("000") + fileExt;
                            else
                                path = path.Substring(0, (length - 3)) + (++i).ToString("000");
                        }
                        //path = Path.Combine(path, dto.Photo.FileName);
                        //path = path.Trim();
                        path = Regex.Replace(path, @"\s+", "");
                        dto.Photo.SaveAs(path);
                        employee.Person.Photo = path;
                        GenService.Save(employee);
                        response.Message = "Photo Uploaded";
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return response;
        }


    }
}
