using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.Auth.DTO;
using Finix.Auth.Infrastructure;
using System.Globalization;
using System.Transactions;
using AutoMapper;



namespace Finix.Auth.Facade
{
    public class MenuFacade : BaseFacade
    {
        public List<ModuleDto> GetModules(List<long?> ApplicationId)
        {
            IQueryable<Module> modules = GenService.GetAll<Module>();
            List<Module> refinedModules = new List<Module>();
            if (ApplicationId != null && ApplicationId.Count > 0)
            {
                foreach (var id in ApplicationId)
                    refinedModules.AddRange(modules.Where(m => m.Applications.Select(a=>a.Id).Contains((long)id)).ToList());
            }
            else
            {
                refinedModules = modules.ToList();
            }

            refinedModules = refinedModules.Distinct().ToList();

            var data = refinedModules.Select(x => new ModuleDto
            {
                Id = x.Id,
                Name = x.Name,
                DisplayName = x.DisplayName,
                Sl = x.Sl,
                Description = x.Description
            }).ToList();
            return data;
        }

        public void SaveModule(ModuleDto moduleDto)
        {
            if (moduleDto.Id > 0)
            {
                var module = GenService.GetById<Module>((long)moduleDto.Id);
                module.Name = moduleDto.Name;
                module.DisplayName = moduleDto.DisplayName;
                module.Sl = (int)moduleDto.Sl;
                module.Description = moduleDto.Description;
                GenService.Save(module);

            }
            else
            {
                GenService.Save(new Module
                {
                    Name = moduleDto.Name,
                    DisplayName = moduleDto.DisplayName,
                    Sl = (int)moduleDto.Sl,
                    Description = moduleDto.Description
                });
            }
            GenService.SaveChanges();
        }

        public void DeleteModule(long id)
        {
            GenService.Delete<Module>(id);
            GenService.SaveChanges();
        }

        /*  public List<SubModuleDto> GetSubModules()        //retreive the list of submodules
          {
              var submodules = GenService.GetAll<SubModule>().ToList();
              var data = submodules.Select(x => new SubModuleDto
              {
                  Id = x.Id,
                  Name = x.Name,
                  DisplayName=x.DisplayName,
                  Description = x.Description,
                  Sl=x.Sl,
                  ModuleName = x.Module.Name

              }).ToList();
              return data;
          }*/
        public List<SubModuleDto> GetSubModules(int moduleId = 0)  // retreive submodule by moduleid
        {
            //var subModules = GenService.GetAll<SubModule>().Where(x => x.ModuleId == moduleId).ToList();
            var subModules = moduleId > 0
                ? GenService.GetAll<SubModule>()
                    .Where(x => x.ModuleId == moduleId).ToList()
                : GenService.GetAll<SubModule>().ToList();
            var data = subModules.Select(x => new SubModuleDto
            {
                Id = x.Id,
                Name = x.Name,
                DisplayName = x.DisplayName,
                Description = x.Description,
                Sl = x.Sl,
                ModuleId = x.ModuleId,
                ModuleName = x.Module.Name,

            }).OrderBy(r=>r.Sl).ToList();
            return data;

        }
        public void SaveSubModule(SubModuleDto submoduledto)
        {
            if (submoduledto.Id > 0)
            {
                var submodule = GenService.GetById<SubModule>(submoduledto.Id);
                submodule.Name = submoduledto.Name;
                submodule.DisplayName = submoduledto.DisplayName;
                submodule.Sl = submoduledto.Sl;
                submodule.Description = submoduledto.Description;

                submodule.ModuleId = submoduledto.ModuleId;

            }
            else
            {
                GenService.Save(new SubModule
             {
                 Name = submoduledto.Name,
                 DisplayName = submoduledto.DisplayName,
                 Sl = submoduledto.Sl,
                 Description = submoduledto.Description,
                 ModuleId = submoduledto.ModuleId,

             });

            }
            GenService.SaveChanges();
        }

        public void DeleteSubModule(long id)
        {
            GenService.Delete<SubModule>(id);
            GenService.SaveChanges();

        }

        public List<MenuDto> GetMenus(long submoduleId = 0)
        {
            var menus = submoduleId > 0
                ? GenService.GetAll<Menu>().Where(i => i.Status == EntityStatus.Active)
                    .Where(x => x.SubModuleId == submoduleId).ToList()
                : GenService.GetAll<Menu>().Where(i => i.Status == EntityStatus.Active).ToList();

            var data = menus.Select(x => new MenuDto
            {
                Id = x.Id,
                Name = x.Name,
                DisplayName = x.DisplayName,
                Description = x.Description,
                Sl = x.Sl,
                ModuleName = x.SubModule.Module.Name,
                ModuleId = x.SubModule.ModuleId,
                SubModuleName = x.SubModule.Name,
                SubModuleId = x.SubModuleId,
                Url = x.Url
            }).ToList();
            return data;
        }

        public void SaveMenu(MenuDto menudto)
        {
            if (menudto.Id > 0)
            {
                var menu = GenService.GetById<Menu>(menudto.Id);
                menu.Name = menudto.Name;
                menu.DisplayName = menudto.DisplayName;
                menu.Sl = menudto.Sl;
                menu.Description = menudto.Description;
                menu.Url = menudto.Url;

                menu.SubModuleId = menudto.SubModuleId;

            }
            else
            {
                GenService.Save(new Menu
                {
                    Name = menudto.Name,
                    DisplayName = menudto.DisplayName,
                    Sl = menudto.Sl,
                    Description = menudto.Description,
                    SubModuleId = menudto.SubModuleId,
                    Url = menudto.Url

                });

            }
            GenService.SaveChanges();
        }

        public void DeleteMenu(long id)
        {
            GenService.Delete<Menu>(id);
            GenService.SaveChanges();

        }


        public List<TaskDto> GetTasks()
        {
            var tasks = GenService.GetAll<Finix.Auth.Infrastructure.Task>();
            var data = tasks.Select(x => new TaskDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).ToList();
            return data;
        }

        public void SaveTask(TaskDto taskdto)
        {
            if (taskdto.Id > 0)
            {
                var task = GenService.GetById<Finix.Auth.Infrastructure.Task>(taskdto.Id);
                task.Name = taskdto.Name;
                task.Description = taskdto.Description;

            }
            else
            {
                GenService.Save(new Finix.Auth.Infrastructure.Task
                {
                    Name = taskdto.Name,
                    Description = taskdto.Description

                });

            }

            GenService.SaveChanges();
        }

        public void DeleteTask(long id)
        {
            GenService.Delete<Finix.Auth.Infrastructure.Task>(id);
            GenService.SaveChanges();
        }

        public List<RoleDto> GetRoles()
        {
            var roles = GenService.GetAll<Role>().ToList();
            var data = roles.Select(x => new RoleDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).ToList();
            return data;
        }

        public void SaveRole(RoleDto roleDto)
        {
            if (roleDto.Id > 0)
            {
                var role = GenService.GetById<Role>(roleDto.Id);
                role.Name = roleDto.Name;
                role.Description = roleDto.Description;
                GenService.Save(role);

            }
            else
            {
                GenService.Save(new Role
                {
                    Name = roleDto.Name,
                    Description = roleDto.Description
                });
            }
            GenService.SaveChanges();
        }

        public void DeleteRole(long id)
        {
            GenService.Delete<Role>(id);
            GenService.SaveChanges();
        }

        public object GetRolePermissionData()//long? RoleId
        {
            List<RoleMenu> currentPermissions = new List<RoleMenu>();
            //if (RoleId != null && RoleId > 0)
            //{
            //    currentPermissions = GenService.GetAll<RoleMenu>().Where(r => r.RoleId == RoleId && r.Status == EntityStatus.Active).ToList();
            //}

            var taskList = GenService.GetAll<Finix.Auth.Infrastructure.Task>();

            #region main data population
            var data = (from mn in GenService.GetAll<Menu>().Where(i => i.Status == EntityStatus.Active)
                        group mn by new { mn.SubModule.ModuleId, mn.SubModule.Module.Name } into grp1
                        select new
                        {
                            id = "MOD_" + grp1.Key.ModuleId,
                            text = grp1.Key.Name,
                            state = new
                            {
                                opened = false,  // is the node open
                                disabled = false,  // is the node disabled
                                selected = false  // is the node selected
                            },
                            children = (from set in grp1
                                        group set by new { set.SubModuleId, set.SubModule.Name } into grp2
                                        select new
                                        {
                                            id = "MOD_" + grp1.Key.ModuleId 
                                                    + "_SMOD_" + grp2.Key.SubModuleId,
                                            text = grp2.Key.Name,
                                            state = new
                                            {
                                                opened = false,
                                                disabled = false,
                                                selected = false
                                            },
                                            children = grp2.Select(c => new
                                            {
                                                id = "MOD_" + grp1.Key.ModuleId 
                                                        + "_SMOD_" + grp2.Key.SubModuleId 
                                                        + "_M_" + c.Id,
                                                text = c.Name,
                                                state = new
                                                {
                                                    opened = false,
                                                    disabled = false,
                                                    selected = false
                                                },
                                                children = (from tasks in c.Tasks
                                                            select new
                                                            {
                                                                id = "MOD_" + grp1.Key.ModuleId 
                                                                        + "_SMOD_" + grp2.Key.SubModuleId 
                                                                        + "_M_" + c.Id 
                                                                        + "_T_" + tasks.Id,
                                                                text = tasks.Name,
                                                                children = new List<object>{
                                                                    new
                                                                    {
                                                                        id = "MOD_" + grp1.Key.ModuleId 
                                                                                + "_SMOD_" + grp2.Key.SubModuleId 
                                                                                + "_M_" + c.Id 
                                                                                + "_T_" + tasks.Id + "_P_1",
                                                                        text = "Read",
                                                                        state = new {
                                                                            opened = false,
                                                                            disabled = false,
                                                                            selected = false
                                                                          }
                                                                    },
                                                                    new
                                                                    {
                                                                        id = "MOD_" + grp1.Key.ModuleId 
                                                                                + "_SMOD_" + grp2.Key.SubModuleId 
                                                                                + "_M_" + c.Id 
                                                                                + "_T_" + tasks.Id + "_P_2",
                                                                        text = "Add",
                                                                        state = new {
                                                                            opened = false,
                                                                            disabled = false,
                                                                            selected = false
                                                                          }
                                                                    },
                                                                    new
                                                                    {
                                                                        id = "MOD_" + grp1.Key.ModuleId 
                                                                                + "_SMOD_" + grp2.Key.SubModuleId 
                                                                                + "_M_" + c.Id 
                                                                                + "_T_" + tasks.Id + "_P_3",
                                                                        text = "Edit",
                                                                        state = new {
                                                                            opened = false,
                                                                            disabled = false,
                                                                            selected = false
                                                                          }
                                                                    },
                                                                    new
                                                                    {
                                                                        id = "MOD_" + grp1.Key.ModuleId 
                                                                                + "_SMOD_" + grp2.Key.SubModuleId 
                                                                                + "_M_" + c.Id 
                                                                                + "_T_" + tasks.Id + "_P_4",
                                                                        text = "Delete",
                                                                        state = new {
                                                                            opened = false,
                                                                            disabled = false,
                                                                            selected = false
                                                                          }
                                                                    }
                                                                }
                                                            })
                                            })

                                        })
                        }
                ).ToList();

            #endregion


            return data;
        }

        public object GetCurrentPermissionData(long? RoleId)
        {
            List<string> data = new List<string>();
            List<RoleMenu> currentPermissions = new List<RoleMenu>();

            if (RoleId != null && RoleId > 0)
            {
                currentPermissions = GenService.GetAll<RoleMenu>().Where(r => r.RoleId == RoleId && r.Status == EntityStatus.Active).ToList();
            }
            if(currentPermissions != null && currentPermissions.Count > 0)
            {
                foreach (var menu in currentPermissions)
                {
                    foreach (var task in menu.RoleMenuTasks)
                    {
                        if (task.Read)
                            data.Add("MOD_" + menu.Menu.SubModule.ModuleId 
                                + "_SMOD_" + menu.Menu.SubModuleId
                                + "_M_" + menu.MenuId
                                + "_T_" + task.TaskId + "_P_1");
                        if (task.Add)
                            data.Add("MOD_" + menu.Menu.SubModule.ModuleId
                                + "_SMOD_" + menu.Menu.SubModuleId
                                + "_M_" + menu.MenuId
                                + "_T_" + task.TaskId + "_P_2");
                        if (task.Edit)
                            data.Add("MOD_" + menu.Menu.SubModule.ModuleId
                                + "_SMOD_" + menu.Menu.SubModuleId
                                + "_M_" + menu.MenuId
                                + "_T_" + task.TaskId + "_P_3");
                        if (task.Delete)
                            data.Add("MOD_" + menu.Menu.SubModule.ModuleId
                                + "_SMOD_" + menu.Menu.SubModuleId
                                + "_M_" + menu.MenuId
                                + "_T_" + task.TaskId + "_P_4");
                    }
                }
                //var data = from cp in currentPermissions
                           //from tasks in cp.RoleMenuTasks
            }
            //var data = from cp in currentPermissions

            return data;
        }

        public ResponseDto SaveRolePermission(RolePermissionDto rolePermission, long UserId)//List<string> checkedIds, long roleId
        {
            var response = new ResponseDto();
            var roleMenuList = new List<RoleMenu>();
            var inactiveRoleMenuList = new List<RoleMenu>();
            
            foreach (var Id in rolePermission.Ids)
            {
                var IdList = Id.Replace("_anchor", "");
                var parts = IdList.Split('_');
                switch (parts.Count())
                {
                    case 10:
                        if (parts[8] == "P")
                        {
                            //todo-create one menu one task one permission
                            //pick or create role-menu and add task with single permission
                            var menuId = Convert.ToInt64(parts[5]);
                            var taskId = Convert.ToInt64(parts[7]);
                            var permissionId = Convert.ToInt16(parts[9]);

                            var roleMenuEntry = roleMenuList.Where(r => r.RoleId == rolePermission.RoleId
                                                                        && r.MenuId == menuId
                                                                        && r.Status == EntityStatus.Active).FirstOrDefault();

                            if (roleMenuEntry == null)
                            {
                                #region
                                roleMenuEntry = new RoleMenu();
                                roleMenuEntry.RoleId = rolePermission.RoleId;
                                roleMenuEntry.MenuId = menuId;
                                roleMenuEntry.RoleMenuTasks = new List<RoleMenuTask>();

                                var menuTask = new RoleMenuTask();
                                menuTask.TaskId = taskId;
                                menuTask.Status = EntityStatus.Active;
                                menuTask.Read = permissionId == 1 ? true : false;
                                menuTask.Add = permissionId == 2 ? true : false;
                                menuTask.Edit = permissionId == 3 ? true : false;
                                menuTask.Delete = permissionId == 4 ? true : false;
                                #endregion
                                roleMenuEntry.RoleMenuTasks.Add(menuTask);

                                roleMenuList.Add(roleMenuEntry);
                            }
                            else
                            {
                                var currentTask = roleMenuEntry.RoleMenuTasks.Where(t => t.TaskId == taskId).FirstOrDefault();
                                if (currentTask == null)
                                {
                                    #region
                                    var menuTask = new RoleMenuTask();
                                    menuTask.TaskId = taskId;
                                    menuTask.Status = EntityStatus.Active;
                                    menuTask.Read = permissionId == 1 ? true : false;
                                    menuTask.Add = permissionId == 2 ? true : false;
                                    menuTask.Edit = permissionId == 3 ? true : false;
                                    menuTask.Delete = permissionId == 4 ? true : false;
                                    #endregion
                                    roleMenuEntry.RoleMenuTasks.Add(menuTask);
                                }
                                else
                                {
                                    #region
                                    if (permissionId == 1)
                                        currentTask.Read = true;
                                    else if (permissionId == 2)
                                        currentTask.Add = true;
                                    else if (permissionId == 3)
                                        currentTask.Edit = true;
                                    else if (permissionId == 4)
                                        currentTask.Delete = true;
                                    #endregion
                                }
                            }
                        }
                        break;
                    case 8:
                        if (parts[6] == "T")
                        {
                            //todo-create one menu one task multiple permission
                            //pick or create role-menu and add one task with all permission
                            var menuId = Convert.ToInt64(parts[5]);
                            var taskId = Convert.ToInt64(parts[7]);

                            var roleMenuEntry = roleMenuList.Where(r => r.RoleId == rolePermission.RoleId
                                                                        && r.MenuId == menuId
                                                                        && r.Status == EntityStatus.Active).FirstOrDefault();
                            if (roleMenuEntry == null)
                            {
                                #region
                                roleMenuEntry = new RoleMenu();
                                roleMenuEntry.RoleId = rolePermission.RoleId;
                                roleMenuEntry.MenuId = menuId;
                                roleMenuEntry.RoleMenuTasks = new List<RoleMenuTask>();
                                
                                var menuTask = new RoleMenuTask();
                                menuTask.TaskId = taskId;
                                menuTask.Status = EntityStatus.Active;
                                menuTask.Read = true;
                                menuTask.Add = true;
                                menuTask.Edit = true;
                                menuTask.Delete = true;
                                #endregion
                                roleMenuEntry.RoleMenuTasks.Add(menuTask);

                                roleMenuList.Add(roleMenuEntry);
                            }
                            else
                            {
                                #region
                                var menuTask = new RoleMenuTask();
                                menuTask.TaskId = taskId;
                                menuTask.Status = EntityStatus.Active;
                                menuTask.Read = true;
                                menuTask.Add = true;
                                menuTask.Edit = true;
                                menuTask.Delete = true;
                                #endregion
                                roleMenuEntry.RoleMenuTasks.Add(menuTask);
                            }
                        }
                        break;
                    case 6:
                        if (parts[4] == "M")
                        {
                            //todo-create one menu multiple task multiple permission
                            //create role-menu and add all task with all permission
                            var menuId = Convert.ToInt64(parts[5]);
                            List<RoleMenu> oldEntry;
                            var roleMenuEntry = roleMenuList.Where(r => r.RoleId == rolePermission.RoleId
                                                                        && r.MenuId == menuId
                                                                        && r.Status == EntityStatus.Active).FirstOrDefault();
                            oldEntry = GenService.GetAll<RoleMenu>().Where(r => r.RoleId == rolePermission.RoleId
                                                                    && r.MenuId == menuId
                                                                    && r.Status == EntityStatus.Active).ToList();
                            if (oldEntry != null)
                                inactiveRoleMenuList.AddRange(oldEntry);
                            
                            if (roleMenuEntry == null)
                            {
                                roleMenuList.Add(PrepareRoleMenuFromMenu(rolePermission.RoleId, menuId));
                            }
                            
                        }
                        break;
                    case 4:
                        if (parts[2] == "SMOD")
                        {
                            //todo-create multiple menu multiple task multiple permission
                            //create multiple role-menu and add all task with all permission
                            var subModuleId = Convert.ToInt64(parts[3]);
                            var Menus = GenService.GetById<SubModule>(subModuleId).Menus.ToList();
                            
                            foreach (var menu in Menus)
                            {
                                var oldEntry = GenService.GetAll<RoleMenu>().Where(r => r.RoleId == rolePermission.RoleId
                                                                        && r.MenuId == menu.Id
                                                                        && r.Status == EntityStatus.Active).ToList();
                                if(oldEntry != null)
                                    inactiveRoleMenuList.AddRange(oldEntry);

                                var roleMenuEntry = roleMenuList.Where(r => r.RoleId == rolePermission.RoleId
                                                                        && r.MenuId == menu.Id
                                                                        && r.Status == EntityStatus.Active).FirstOrDefault();
                                if (roleMenuEntry == null)
                                {
                                    roleMenuList.Add(PrepareRoleMenuFromMenu(rolePermission.RoleId, menu.Id));
                                }

                            }
                                                        
                        }
                        break;
                    case 2:
                        if(parts[0] == "MOD")
                        {
                            //todo-create multiple menu multiple task multiple permission
                            //create multiple role-menu and add all task with all permission
                            var moduleId = Convert.ToInt64(parts[1]);
                            var subModules = GenService.GetById<Module>(moduleId).SubModules.ToList();
                            List<Menu> menus = new List<Menu>();
                            foreach(var sM in subModules)
                            {
                                menus.AddRange(sM.Menus);
                            }
                            //menus = subModules.Select(s=>s.Menus).ToList();

                            foreach (var menu in menus)
                            {
                                var oldEntry = GenService.GetAll<RoleMenu>().Where(r => r.RoleId == rolePermission.RoleId
                                                                        && r.MenuId == menu.Id
                                                                        && r.Status == EntityStatus.Active).ToList();
                                if (oldEntry != null)
                                    inactiveRoleMenuList.AddRange(oldEntry);

                                roleMenuList.Add(PrepareRoleMenuFromMenu(rolePermission.RoleId, menu.Id));

                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            #region deactivate old role permissions
            inactiveRoleMenuList = inactiveRoleMenuList.Distinct().ToList();
            foreach (var oldRM in inactiveRoleMenuList)
            {
                oldRM.Status = EntityStatus.Inactive;
                oldRM.EditDate = DateTime.Now;
                oldRM.EditedBy = UserId;
                foreach (var oldRMT in oldRM.RoleMenuTasks)
                {
                    oldRMT.Status = EntityStatus.Inactive;
                    oldRMT.EditDate = DateTime.Now;
                    oldRMT.EditedBy = UserId;
                }
            }
            #endregion

            using (var tran = new TransactionScope())
            {
                try
                {
                    GenService.Save(inactiveRoleMenuList);
                    GenService.Save(roleMenuList);
                    tran.Complete();
                    response.Success = true;
                    response.Message = "Role Permission Saved.";
                }
                catch (Exception) 
                {
                    tran.Dispose();                
                }
            }
            //"MOD_" + grp1.Key.ModuleId + "_SMOD_" + grp2.Key.SubModuleId + "_M_" + c.Id + "_T_" + tasks.Id + "_P_1"
            return response;
        }

        public ResponseDto SaveProxyRolePermission(RolePermissionDto rolePermission, long UserId)
        {
            var response = new ResponseDto();
            var userProxy = new UserProxy();
            userProxy.AssignedBy = UserId;
            userProxy.ProxyUserId = (long)rolePermission.ProxyUserId;
            DateTime FromDate = DateTime.Now;
            DateTime ToDate = DateTime.Now;
            DateTime.TryParseExact(rolePermission.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,DateTimeStyles.None, out FromDate);
            DateTime.TryParseExact(rolePermission.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,DateTimeStyles.None, out ToDate);
            userProxy.FromDate = FromDate;
            userProxy.ToDate = ToDate;
            
            userProxy.ProxyRoleMenus = new List<UserProxyRoleMenu>();
            //var roleMenuList = new List<UserProxyRoleMenu>();
            //var inactiveRoleMenuList = new List<RoleMenu>();

            foreach (var Id in rolePermission.Ids)
            {
                var IdList = Id.Replace("_anchor", "");
                var parts = IdList.Split('_');
                switch (parts.Count())
                {
                    case 10:
                        if (parts[8] == "P")
                        {
                            //todo-create one menu one task one permission
                            //pick or create role-menu and add task with single permission
                            var menuId = Convert.ToInt64(parts[5]);
                            var taskId = Convert.ToInt64(parts[7]);
                            var permissionId = Convert.ToInt16(parts[9]);

                            var roleMenuEntry = userProxy.ProxyRoleMenus.Where(r => r.RoleId == rolePermission.RoleId
                                                                        && r.MenuId == menuId
                                                                        && r.Status == EntityStatus.Active).FirstOrDefault();

                            if (roleMenuEntry == null)
                            {
                                #region
                                roleMenuEntry = new UserProxyRoleMenu();
                                roleMenuEntry.RoleId = rolePermission.RoleId;
                                roleMenuEntry.MenuId = menuId;
                                roleMenuEntry.RoleMenuTasks = new List<UserProxyRoleMenuTask>();

                                var menuTask = new UserProxyRoleMenuTask();
                                menuTask.TaskId = taskId;
                                menuTask.Status = EntityStatus.Active;
                                menuTask.Read = permissionId == 1 ? true : false;
                                menuTask.Add = permissionId == 2 ? true : false;
                                menuTask.Edit = permissionId == 3 ? true : false;
                                menuTask.Delete = permissionId == 4 ? true : false;
                                #endregion
                                roleMenuEntry.RoleMenuTasks.Add(menuTask);

                                userProxy.ProxyRoleMenus.Add(roleMenuEntry);
                            }
                            else
                            {
                                var currentTask = roleMenuEntry.RoleMenuTasks.Where(t => t.TaskId == taskId).FirstOrDefault();
                                if (currentTask == null)
                                {
                                    #region
                                    var menuTask = new UserProxyRoleMenuTask();
                                    menuTask.TaskId = taskId;
                                    menuTask.Status = EntityStatus.Active;
                                    menuTask.Read = permissionId == 1 ? true : false;
                                    menuTask.Add = permissionId == 2 ? true : false;
                                    menuTask.Edit = permissionId == 3 ? true : false;
                                    menuTask.Delete = permissionId == 4 ? true : false;
                                    #endregion
                                    roleMenuEntry.RoleMenuTasks.Add(menuTask);
                                }
                                else
                                {
                                    #region
                                    if (permissionId == 1)
                                        currentTask.Read = true;
                                    else if (permissionId == 2)
                                        currentTask.Add = true;
                                    else if (permissionId == 3)
                                        currentTask.Edit = true;
                                    else if (permissionId == 4)
                                        currentTask.Delete = true;
                                    #endregion
                                }
                            }
                        }
                        break;
                    case 8:
                        if (parts[6] == "T")
                        {
                            //todo-create one menu one task multiple permission
                            //pick or create role-menu and add one task with all permission
                            var menuId = Convert.ToInt64(parts[5]);
                            var taskId = Convert.ToInt64(parts[7]);

                            var roleMenuEntry = userProxy.ProxyRoleMenus.Where(r => r.RoleId == rolePermission.RoleId
                                                                        && r.MenuId == menuId
                                                                        && r.Status == EntityStatus.Active).FirstOrDefault();
                            if (roleMenuEntry == null)
                            {
                                #region
                                roleMenuEntry = new UserProxyRoleMenu();
                                roleMenuEntry.RoleId = rolePermission.RoleId;
                                roleMenuEntry.MenuId = menuId;
                                roleMenuEntry.RoleMenuTasks = new List<UserProxyRoleMenuTask>();

                                var menuTask = new UserProxyRoleMenuTask();
                                menuTask.TaskId = taskId;
                                menuTask.Status = EntityStatus.Active;
                                menuTask.Read = true;
                                menuTask.Add = true;
                                menuTask.Edit = true;
                                menuTask.Delete = true;
                                #endregion
                                roleMenuEntry.RoleMenuTasks.Add(menuTask);

                                userProxy.ProxyRoleMenus.Add(roleMenuEntry);
                            }
                            else
                            {
                                #region
                                var menuTask = new UserProxyRoleMenuTask();
                                menuTask.TaskId = taskId;
                                menuTask.Status = EntityStatus.Active;
                                menuTask.Read = true;
                                menuTask.Add = true;
                                menuTask.Edit = true;
                                menuTask.Delete = true;
                                #endregion
                                roleMenuEntry.RoleMenuTasks.Add(menuTask);
                            }
                        }
                        break;
                    case 6:
                        if (parts[4] == "M")
                        {
                            //todo-create one menu multiple task multiple permission
                            //create role-menu and add all task with all permission
                            var menuId = Convert.ToInt64(parts[5]);
                            //List<RoleMenu> oldEntry;
                            var roleMenuEntry = userProxy.ProxyRoleMenus.Where(r => r.RoleId == rolePermission.RoleId
                                                                        && r.MenuId == menuId
                                                                        && r.Status == EntityStatus.Active).FirstOrDefault();
                            //oldEntry = GenService.GetAll<UserProxy>().Where(u=>u..ToList();
                            //if (oldEntry != null)
                            //    inactiveRoleMenuList.AddRange(oldEntry);

                            if (roleMenuEntry == null)
                            {
                                var roleMenu = PrepareRoleMenuFromMenu(rolePermission.RoleId, menuId);
                                userProxy.ProxyRoleMenus.Add(Mapper.Map<UserProxyRoleMenu>(roleMenu));
                            }

                        }
                        break;
                    case 4:
                        if (parts[2] == "SMOD")
                        {
                            //todo-create multiple menu multiple task multiple permission
                            //create multiple role-menu and add all task with all permission
                            var subModuleId = Convert.ToInt64(parts[3]);
                            var Menus = GenService.GetById<SubModule>(subModuleId).Menus.ToList();

                            foreach (var menu in Menus)
                            {
                                //var oldEntry = GenService.GetAll<RoleMenu>().Where(r => r.RoleId == rolePermission.RoleId
                                //                                        && r.MenuId == menu.Id
                                //                                        && r.Status == EntityStatus.Active).ToList();
                                //if (oldEntry != null)
                                //    inactiveRoleMenuList.AddRange(oldEntry);

                                var roleMenuEntry = userProxy.ProxyRoleMenus.Where(r => r.RoleId == rolePermission.RoleId
                                                                        && r.MenuId == menu.Id
                                                                        && r.Status == EntityStatus.Active).FirstOrDefault();
                                if (roleMenuEntry == null)
                                {
                                    var roleMenu = PrepareRoleMenuFromMenu(rolePermission.RoleId, menu.Id);
                                    userProxy.ProxyRoleMenus.Add(Mapper.Map<UserProxyRoleMenu>(roleMenu));
                                }

                            }

                        }
                        break;
                    case 2:
                        if (parts[0] == "MOD")
                        {
                            //todo-create multiple menu multiple task multiple permission
                            //create multiple role-menu and add all task with all permission
                            var moduleId = Convert.ToInt64(parts[1]);
                            var subModules = GenService.GetById<Module>(moduleId).SubModules.ToList();
                            List<Menu> menus = new List<Menu>();
                            foreach (var sM in subModules)
                            {
                                menus.AddRange(sM.Menus);
                            }
                            //menus = subModules.Select(s=>s.Menus).ToList();

                            foreach (var menu in menus)
                            {
                                //var oldEntry = GenService.GetAll<RoleMenu>().Where(r => r.RoleId == rolePermission.RoleId
                                //                                        && r.MenuId == menu.Id
                                //                                        && r.Status == EntityStatus.Active).ToList();
                                //if (oldEntry != null)
                                //    inactiveRoleMenuList.AddRange(oldEntry);

                                var roleMenu = PrepareRoleMenuFromMenu(rolePermission.RoleId, menu.Id);
                                userProxy.ProxyRoleMenus.Add(Mapper.Map<UserProxyRoleMenu>(roleMenu));

                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            #region deactivate old role permissions
            //inactiveRoleMenuList = inactiveRoleMenuList.Distinct().ToList();
            //foreach (var oldRM in inactiveRoleMenuList)
            //{
            //    oldRM.Status = EntityStatus.Inactive;
            //    oldRM.EditDate = DateTime.Now;
            //    oldRM.EditedBy = UserId;
            //    foreach (var oldRMT in oldRM.RoleMenuTasks)
            //    {
            //        oldRMT.Status = EntityStatus.Inactive;
            //        oldRMT.EditDate = DateTime.Now;
            //        oldRMT.EditedBy = UserId;
            //    }
            //}
            #endregion

            using (var tran = new TransactionScope())
            {
                try
                {
                    //GenService.Save(inactiveRoleMenuList);
                    GenService.Save(userProxy);
                    tran.Complete();
                    response.Success = true;
                    response.Message = "Role Permission Saved.";
                }
                catch (Exception)
                {
                    tran.Dispose();
                }
            }
            //"MOD_" + grp1.Key.ModuleId + "_SMOD_" + grp2.Key.SubModuleId + "_M_" + c.Id + "_T_" + tasks.Id + "_P_1"
            return response;
        }

        private RoleMenu PrepareRoleMenuFromMenu(long RoleId, long? MenuId)
        {
            //List<RoleMenu> result = new List<RoleMenu>();
            if (MenuId != null && MenuId > 0)
            {
                var roleMenuEntry = new RoleMenu();
                roleMenuEntry.RoleId = RoleId;
                roleMenuEntry.MenuId = (long)MenuId;
                roleMenuEntry.RoleMenuTasks = new List<RoleMenuTask>();
                var allMenuTasks = GenService.GetById<Menu>((long)MenuId).Tasks.ToList();
                foreach (var task in allMenuTasks)
                {
                    var menuTask = new RoleMenuTask();
                    menuTask.TaskId = task.Id;
                    menuTask.Status = EntityStatus.Active;
                    menuTask.Read = true;
                    menuTask.Add = true;
                    menuTask.Edit = true;
                    menuTask.Delete = true;

                    roleMenuEntry.RoleMenuTasks.Add(menuTask);
                }
                return roleMenuEntry;
                //result.Add(roleMenuEntry);
            }
            return null;
        }

        public MenuDto GetMenuByUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                var menu = GenService.GetAll<Menu>().Where(m => m.Url.ToLower().Contains(url.ToLower())).FirstOrDefault();
                if (menu != null)
                    return Mapper.Map<MenuDto>(menu);
            }
            return null;
        }
    }
}



