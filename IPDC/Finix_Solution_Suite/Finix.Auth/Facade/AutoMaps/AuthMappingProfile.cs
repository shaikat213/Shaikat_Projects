using AutoMapper;
using Finix.Auth.DTO;
using Finix.Auth.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.Facade.AutoMaps
{
    public class AuthMappingProfile : Profile
    {
        protected override void Configure()
        {
            //base.Configure();
            CreateMap<Module, ModuleDto>();
            CreateMap<ModuleDto, Module>();

            CreateMap<Role, RoleDto>();
            CreateMap<RoleDto, Role>();

            CreateMap<Finix.Auth.Infrastructure.Task, TaskDto>();
            CreateMap<TaskDto, Finix.Auth.Infrastructure.Task>();

            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

            CreateMap<Application, ApplicationDto>();
            CreateMap<ApplicationDto, Application>();

            CreateMap<CompanyProfile, CompanyProfileDto>()
                .ForMember(d => d.ParentName, o => o.MapFrom(m => m.Parent.Name))
                .ForMember(d => d.CompanyTypeName, o => o.MapFrom(m => Enum.GetName(typeof(CompanyType), m.CompanyType)));
            
            CreateMap<RoleMenu, UserProxyRoleMenu>();
            CreateMap<UserProxyRoleMenu, RoleMenu>();

            CreateMap<RoleMenuTask, UserProxyRoleMenuTask>();
            CreateMap<UserProxyRoleMenuTask, RoleMenuTask>();

            // extended mappings


            CreateMap<SubModule, SubModuleDto>()
                .ForMember(d => d.ModuleId, o => o.MapFrom(m => m.ModuleId))
                .ForMember(d => d.ModuleName, o => o.MapFrom(m => m.Module.Name));
            CreateMap<SubModuleDto, SubModule>();

            CreateMap<Menu, MenuDto>()
                .ForMember(d => d.SubModuleId, o => o.MapFrom(m => m.SubModuleId))
                .ForMember(d => d.SubModuleName, o => o.MapFrom(m => m.SubModule.Name));
            CreateMap<MenuDto, Menu>();

            CreateMap<UserCompanyApplication, UserCompanyApplicationDto>()
                .ForMember(d => d.UserName, o => o.MapFrom(m => m.User.UserName))
                .ForMember(d => d.CompanyProfileName, o => o.MapFrom(m => m.CompanyProfile.Name));
        }
    }
}
