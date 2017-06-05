using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Finix.Auth.DTO;
using Finix.Auth.Infrastructure;
using RoleTask = Finix.Auth.Infrastructure.Task;
using System;
using Finix.Auth.Util;

namespace Finix.Auth.Facade
{
    public class LoginFacade : BaseFacade
    {
        public UserResourceDto DoLogin(LogOnDto dto)
        {
            dto.Password = KeyVault.EncryptOrDecryptUserPassForDatabase(dto.Password);
            var userResourceDto = new UserResourceDto();
            var user = GenService.GetAll<User>().FirstOrDefault(r => r.UserName.ToLower() == dto.UserName);
            
            if (user == null || user.Password != dto.Password) return userResourceDto;

            // get all the autorizations
            userResourceDto.UserId = user.Id;
            userResourceDto.IsAdmin = user.Roles.Any(x => x.Id == (int)RoleEnum.Admin);

            //var menus = userResourceDto.IsAdmin ? GenService.GetAll<Menu>().ToList() : user.Menus;
            //var subModules = menus.Select(menu => menu.SubModule).Distinct().ToList();
            //var modules = subModules.Select(x => x.Module).Distinct().ToList();
            //var companies = user.Companies;

            //userResourceDto.Menus = Mapper.Map<List<MenuDto>>(menus);
            //userResourceDto.SubModules = Mapper.Map<List<SubModuleDto>>(subModules);
            //userResourceDto.Modules = Mapper.Map<List<ModuleDto>>(modules);

            if (user != null)
            {
                userResourceDto.UserCompanyApplications = new List<UserCompanyApplicationDto>();
                var userCompanies = user.UserCompanyApplications.GroupBy(c => c.CompanyProfile).Select(c=>c.First());
                foreach (var company in userCompanies)
                {
                    var instance = new UserCompanyApplicationDto();

                    instance.CompanyProfileId = company.CompanyId;
                    instance.CompanyProfileName = company.CompanyProfile.Name;
                    instance.Applications = new List<ApplicationDto>();

                    instance.Applications = Mapper.Map<List<ApplicationDto>>(user.UserCompanyApplications.Where(c => c.CompanyId == company.CompanyId).Select(a => a.Application).ToList());

                    userResourceDto.UserCompanyApplications.Add(instance);
                }
                //userResourceDto.UserCompanyApplications = Mapper.Map<List<UserCompanyApplicationDto>>(user.UserCompanyApplications.GroupBy(c => c.CompanyProfile)).First();//GenService.GetAll<UserCompanyApplication>().Where(a => a.UserId == user.Id)
                
                //userResourceDto.Applications = Mapper.Map<List<ApplicationDto>>(user.Applications);
            }
            
            var roles = userResourceDto.IsAdmin ? GenService.GetAll<Role>().ToList() : user.Roles;
            var roleIds = roles.Select(er => er.Id).ToList();
            var roleMenus = GenService.GetAll<RoleMenu>().Where(r => roleIds.Contains(r.RoleId)).ToList();//user.Menus;
            //var tasks = roles.Select(x => x.Tasks).Distinct().ToList();
            List<TaskDto> tasks = new List<TaskDto>();
            if (userResourceDto.IsAdmin)
                tasks = (from t in GenService.GetAll<Task>()
                         select new TaskDto
                         {
                             Id = t.Id,
                             Name = t.Name,
                             Read = true,
                             Add = true,
                             Edit = true,
                             Delete = true
                         }).ToList();
            else
                tasks = (from menu in roleMenus
                         from task in menu.RoleMenuTasks
                         select new TaskDto
                         {
                             Id = task.Id,
                             Name = task.Task.Name,
                             Read = (menu.RoleMenuTasks.Any() && menu.RoleMenuTasks.Where(rm => rm.Read == true).Any()) ? true : false,
                             Add = (menu.RoleMenuTasks.Any() && menu.RoleMenuTasks.Where(rm => rm.Add == true).Any()) ? true : false,
                             Edit = (menu.RoleMenuTasks.Any() && menu.RoleMenuTasks.Where(rm => rm.Edit == true).Any()) ? true : false,
                             Delete = (menu.RoleMenuTasks.Any() && menu.RoleMenuTasks.Where(rm => rm.Delete == true).Any()) ? true : false,
                         }).Distinct().ToList();

            var proxyTasks = (from proxy in GenService.GetAll<UserProxy>().Where(p => p.ProxyUserId == user.Id && p.FromDate <= DateTime.Now && p.ToDate >= DateTime.Now)
                              from proxyMenus in proxy.ProxyRoleMenus
                              from t in proxyMenus.RoleMenuTasks
                              select new TaskDto { 
                                  Id = t.Id,
                                  Name = t.Task.Name,
                                  Read = (proxyMenus.RoleMenuTasks.Any() && proxyMenus.RoleMenuTasks.Where(rm => rm.Read == true).Any()) ? true : false,
                                  Add = (proxyMenus.RoleMenuTasks.Any() && proxyMenus.RoleMenuTasks.Where(rm => rm.Add == true).Any()) ? true : false,
                                  Edit = (proxyMenus.RoleMenuTasks.Any() && proxyMenus.RoleMenuTasks.Where(rm => rm.Edit == true).Any()) ? true : false,
                                  Delete = (proxyMenus.RoleMenuTasks.Any() && proxyMenus.RoleMenuTasks.Where(rm => rm.Delete == true).Any()) ? true : false,
                              }).ToList();

            userResourceDto.Tasks = tasks;
            userResourceDto.Tasks.AddRange(Mapper.Map<List<TaskDto>>(proxyTasks));
            //foreach (var task in tasks)
            //{
            //    userResourceDto.Tasks.AddRange(Mapper.Map<List<TaskDto>>(task));
            //}
            //userResourceDto.Tasks.AddRange(tasks);
            userResourceDto.Roles = Mapper.Map<List<RoleDto>>(roles);
            userResourceDto.UserName = user.UserName;
            //userResourceDto.
            return userResourceDto;
        }

        public bool IsTaskRegistered()
        {
            return GenService.GetAll<RoleTask>().Any();
        }

        public void RegisterTasks(List<string> taskNames)
        {
            var taskList = taskNames.Select(n => new RoleTask { Name = n }).ToList();
            GenService.Save(taskList);
        }

        public UserResourceDto ChooseApplication(long UserId, long CompanyProfileId, long ApplicationId, List<long> roleIds)
        {
            var userResourceDto = new UserResourceDto();
            var user = GenService.GetById<User>(UserId);
            
            userResourceDto.EmployeeId = user.EmployeeId;
            //if(user.EmployeeId != null && user.EmployeeId > 0)
            //{
            //    //var degId = GenService.ge
            //}
            userResourceDto.UserId = user.Id;
            userResourceDto.IsAdmin = user.Roles.Any(x => x.Id == (int)RoleEnum.Admin);
            userResourceDto.UserName = user.UserName;

            //var roleIds = user.Roles.Select(r => r.Id).ToList();
            //var roleMenus = GenService.GetAll<RoleMenu>().Where(r => roles.Select(er => er.Id).Contains(r.RoleId));//user.Menus; 
            
            var menus = userResourceDto.IsAdmin ?
                GenService.GetAll<Menu>().Where(i=>i.Status == EntityStatus.Active).OrderBy(r=>r.Sl).ToList() : 
                GenService.GetAll<RoleMenu>()
                    .Where(r => roleIds.Contains(r.RoleId)
                        && r.Menu.SubModule.Module.Applications.Select(a => a.Id).Contains(ApplicationId))
                    .Select(m => m.Menu).Distinct().ToList();
            var subModules = menus.Select(menu => menu.SubModule).Distinct().OrderBy(r=>r.Sl).ToList();
            var modules = subModules.Select(x => x.Module).Distinct().ToList();

            userResourceDto.Menus = Mapper.Map<List<MenuDto>>(menus);
            userResourceDto.SubModules = Mapper.Map<List<SubModuleDto>>(subModules);
            userResourceDto.Modules =  Mapper.Map<List<ModuleDto>>(modules);

            var proxyMenus =  (from proxy in GenService.GetAll<UserProxy>().Where(p => p.ProxyUserId == user.Id && p.FromDate <= DateTime.Now && p.ToDate >= DateTime.Now)
                              from menu in proxy.ProxyRoleMenus
                                   select menu.Menu).ToList();
            var proxySubModules = proxyMenus.Select(p => p.SubModule).Distinct().ToList();
            var proxyModules = proxySubModules.Select(p => p.Module).Distinct().ToList();

            userResourceDto.Menus.AddRange(Mapper.Map<List<MenuDto>>(proxyMenus));
            userResourceDto.SubModules.AddRange(Mapper.Map<List<SubModuleDto>>(proxySubModules));
            userResourceDto.Modules.AddRange(Mapper.Map<List<ModuleDto>>(proxyModules));

            userResourceDto.Menus = userResourceDto.Menus.Distinct().ToList();
            userResourceDto.SubModules = userResourceDto.SubModules.Distinct().ToList();
            userResourceDto.Modules = userResourceDto.Modules.Distinct().ToList();

            var _company = new CompanyProfileFacade();
            userResourceDto.CurrentlyAccessibleCompanies = _company.GetAccessbleSubCompaniesAndProjects(user.Id, CompanyProfileId)
                                                            .Select(c => new CompanyProfileDto 
                                                                    { 
                                                                        Id = c.Id,
                                                                        Name = c.Name,
                                                                        ParentId = c.ParentId,
                                                                        ParentName = c.ParentName
                                                                    }).Distinct().ToList();

            //var proxyTasks = (from proxy in GenService.GetAll<UserProxy>().Where(p => p.ProxyUserId == user.Id && p.FromDate <= DateTime.Now && p.ToDate >= DateTime.Now)
            //                  from proxyMenus in proxy.ProxyRoleMenus
            //                  select proxyMenus.RoleMenuTasks).ToList();



            return userResourceDto;
        }

        public ResponseDto ChangePassword(ChangePasswordDto dto)
        {
            var response = new ResponseDto();
            if(dto.UserId != null && dto.UserId > 0)
            {
                var user = GenService.GetById<User>((long)dto.UserId);
                if(user != null)
                {
                    dto.OldPassword = KeyVault.EncryptOrDecryptUserPassForDatabase(dto.OldPassword);
                    dto.NewPassword = KeyVault.EncryptOrDecryptUserPassForDatabase(dto.NewPassword);
                    if (user.Password == dto.OldPassword)
                    {
                        if (dto.NewPassword == dto.ConfirmPassword)
                        {
                            if (dto.NewPassword.Length > 5)
                            {
                                user.Password = dto.NewPassword;
                                GenService.Save(user);
                                GenService.SaveChanges();
                                response.Success = true;
                                response.Message = "Password changed successfully.";
                            }
                            else
                                response.Message = "Password must be minimum 6 charecters long. Please try again.";
                        }
                        else
                            response.Message = "New passwords didn't match. Please try again.";
                    }
                    else
                    {
                        response.Message = "Old password didn't match. Please try again.";
                    }
                }
                else
                {
                    response.Message = "User not found.";
                }
            }
            else
            {
                response.Message = "User not found.";
            }
            return response;
        }

        public UserResourceDto RoleAssignmentByRoleIdList(List<long> roleIds, bool isAdmin)
        {
            UserResourceDto userResourceDto = new UserResourceDto();
            if (roleIds == null)
            {
                return null;
            }   
            var roleMenus = GenService.GetAll<RoleMenu>().Where(r => roleIds.Contains(r.RoleId)).ToList();
            var roles = GenService.GetAll<Role>().Where(r => roleIds.Contains(r.Id)).ToList();

            List<TaskDto> tasks = new List<TaskDto>();
            if (isAdmin)
                tasks = (from t in GenService.GetAll<Task>()
                         select new TaskDto
                         {
                             Id = t.Id,
                             Name = t.Name,
                             Read = true,
                             Add = true,
                             Edit = true,
                             Delete = true
                         }).ToList();
            else
                tasks = (from menu in roleMenus
                         from task in menu.RoleMenuTasks
                         select new TaskDto
                         {
                             Id = task.Id,
                             Name = task.Task.Name,
                             Read = (menu.RoleMenuTasks.Any() && menu.RoleMenuTasks.Where(rm => rm.Read == true).Any()) ? true : false,
                             Add = (menu.RoleMenuTasks.Any() && menu.RoleMenuTasks.Where(rm => rm.Add == true).Any()) ? true : false,
                             Edit = (menu.RoleMenuTasks.Any() && menu.RoleMenuTasks.Where(rm => rm.Edit == true).Any()) ? true : false,
                             Delete = (menu.RoleMenuTasks.Any() && menu.RoleMenuTasks.Where(rm => rm.Delete == true).Any()) ? true : false,
                         }).Distinct().ToList();

            //var proxyTasks = (from proxy in GenService.GetAll<UserProxy>().Where(p => p.ProxyUserId == user.Id && p.FromDate <= DateTime.Now && p.ToDate >= DateTime.Now)
            //                  from proxyMenus in proxy.ProxyRoleMenus
            //                  from t in proxyMenus.RoleMenuTasks
            //                  select new TaskDto
            //                  {
            //                      Id = t.Id,
            //                      Name = t.Task.Name,
            //                      Read = (proxyMenus.RoleMenuTasks.Any() && proxyMenus.RoleMenuTasks.Where(rm => rm.Read == true).Any()) ? true : false,
            //                      Add = (proxyMenus.RoleMenuTasks.Any() && proxyMenus.RoleMenuTasks.Where(rm => rm.Add == true).Any()) ? true : false,
            //                      Edit = (proxyMenus.RoleMenuTasks.Any() && proxyMenus.RoleMenuTasks.Where(rm => rm.Edit == true).Any()) ? true : false,
            //                      Delete = (proxyMenus.RoleMenuTasks.Any() && proxyMenus.RoleMenuTasks.Where(rm => rm.Delete == true).Any()) ? true : false,
            //                  }).ToList();

            userResourceDto.Tasks = tasks;
            //userResourceDto.Tasks.AddRange(Mapper.Map<List<TaskDto>>(proxyTasks));

            userResourceDto.Roles = Mapper.Map<List<RoleDto>>(roles);

            return userResourceDto;
        }

        public object UploadPicture(string dto, long userId)
        {
            throw new NotImplementedException();
        }
    }
}
