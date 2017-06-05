using AutoMapper;
using Finix.Auth.DTO;
using Finix.Auth.Infrastructure;
using Finix.Auth.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.Facade
{
    public class UserFacade : BaseFacade
    {
        //private readonly GenService _service;
        public ResponseDto UserRegistration(UserDto userDto)
        {
            userDto.Password = KeyVault.EncryptOrDecryptUserPassForDatabase(userDto.Password);
            userDto.ConfirmPassword = KeyVault.EncryptOrDecryptUserPassForDatabase(userDto.ConfirmPassword);
            ResponseDto response = new ResponseDto();
            User user = new User();
            try
            {
                if (userDto.Password != userDto.ConfirmPassword)
                {
                    response.Message = "Passwords Don't Match. Please Retype";
                    return response;
                }
                if (userDto.Id != null && userDto.Id > 0)
                    user = GenService.GetAll<User>().SingleOrDefault(x => x.Id == userDto.Id);
                if (user != null && user.Id > 0)
                {
                    user.UserName = userDto.UserName;
                    user.Password = userDto.Password;
                    user.EmployeeId = userDto.EmployeeId;
                    user.IsActive = true;
                }
                else
                {
                    user = GenService.GetAll<User>().Where(u => u.UserName.ToLower() == userDto.UserName.ToLower()).FirstOrDefault();
                    if(user == null)
                    {
                        user = new User
                        {
                            UserName = userDto.UserName,
                            Password = userDto.Password,
                            EmployeeId = userDto.EmployeeId,
                            IsActive = true
                        };
                        GenService.Save(user);
                        var companyEntry = new UserCompanyApplication
                        {
                            ApplicationId = 1,
                            CompanyId = 1,
                            UserId = user.Id
                        };
                        GenService.Save(companyEntry);
                    }
                    else
                    {
                        response.Message = "User Name Already Exist . Please Give Another Username";
                        return response;
                    }
                }
                GenService.SaveChanges();
                response.Success = true;
                response.Message = "User Saved Successfully";
                return response;
            }
            catch (Exception)
            {
                response.Message = "User Save Failed";
                return response;
            }

        }

        public List<UserDto> GetUsersByCompanyId(long CompanyId)
        {
            var data = GenService.GetAll<UserCompanyApplication>().Where(u => u.CompanyId == CompanyId).Select(u => u.User).Distinct();
            List<UserDto> users = new List<UserDto>();
            foreach (var item in data)
            {
                users.Add(Mapper.Map<UserDto>(item));
            }
            return users;
        }

        public long GetEmployeeIdByUserId(long userId)
        {
            var employeeId = GenService.GetSingleById<User>(userId).EmployeeId;
            long empId=0;
            if (employeeId != null)
                empId = (long)employeeId;

            return empId;
        }

        public UserDto GetUserByEmployeeId(long empId)
        {

            var user = GenService.GetAll<User>().Where(u=>u.EmployeeId == empId).FirstOrDefault();
            if (user != null)
                return Mapper.Map<UserDto>(user);
            return null;
        }


        public List<UserDto> GetAllUserByEmployeeId(long empId)
        {
            var userlist = GenService.GetAll<User>().Where(u => u.EmployeeId == empId).ToList();
            if (userlist != null)
                return Mapper.Map<List<UserDto>>(userlist);
            return null;
        }

        public long GetUserIdByEmployeeId(long empId)
        {
            var user = GenService.GetAll<User>().Where(u => u.EmployeeId == empId).FirstOrDefault();
            if (user != null)
            {
                var result = Mapper.Map<UserDto>(user);
                return (long)result.Id;
            }
            
            return 0;
        }

        public ResponseDto SaveUserInformation(UserDto userinfo, int id)
        {
            var response = new ResponseDto();
            var user = GenService.GetAll<User>().SingleOrDefault(x => x.EmployeeId == id);
            if (user != null)
            {
                user.UserName = userinfo.UserName;
                user.Password = userinfo.Password;
                user.EmployeeId = userinfo.EmployeeId;
                user.IsActive = true;
            }
            else
            {
                GenService.Save(new User
                {
                    UserName = userinfo.UserName,
                    Password = userinfo.Password,
                    EmployeeId = userinfo.EmployeeId,
                    IsActive = true
                });
            }
            GenService.SaveChanges();
            response.Success = true;
            response.Message = "User information updted successfully.";
            return response;
        }

        public List<RoleDto> GetAllRoles()
        {
            var data = GenService.GetAll<Role>().ToList();
            return Mapper.Map<List<RoleDto>>(data);
        }

        //public bool SaveUserDegSetId
        public List<UserEmployeeMapping> GetUserEmployeeMapping()
        {
            var result = GenService.GetAll<User>().Where(u => u.Status == EntityStatus.Active && u.EmployeeId != null).Select(u => new UserEmployeeMapping { UserId = u.Id, EmployeeId = u.EmployeeId }).Distinct().ToList();
            return result;
        }
      
    }
    public class UserEmployeeMapping
    {
        public long? UserId { get; set; }
        public long? EmployeeId { get; set; }
    }
}
