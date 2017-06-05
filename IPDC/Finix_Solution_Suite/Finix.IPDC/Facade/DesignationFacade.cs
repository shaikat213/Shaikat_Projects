using AutoMapper;
using Finix.IPDC.DTO;
using Finix.IPDC.Infrastructure.Models;
using Finix.IPDC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.Auth.DTO;
using Finix.Auth.Facade;
using Finix.IPDC.Infrastructure;
using ResponseDto = Finix.IPDC.DTO.ResponseDto;

namespace Finix.IPDC.Facade
{
    public class DesignationFacade
    {
        private readonly GenService _service = new GenService();
        private readonly UserFacade _user = new UserFacade();
        public List<DesignationDto> GetDesignations()
        {
            var orgs = _service.GetAll<Designation>();
            return Mapper.Map<List<DesignationDto>>(orgs.ToList());
        }

        public ResponseDto SaveDesignations(DesignationDto dto)
        {
            var entity = new Designation();
            ResponseDto responce = new ResponseDto();
            if (dto.ParentId != null && (dto.ParentId == 0 || dto.ParentId == dto.Id))
                dto.ParentId = null;
            try
            {
                if (dto.Id != null && dto.Id > 0)
                {
                    entity = _service.GetById<Designation>((long)dto.Id);
                    dto.CreateDate = entity.CreateDate;
                    dto.CreatedBy = entity.CreatedBy;
                    entity = Mapper.Map(dto, entity);
                    entity.EditDate = DateTime.Now;
                    _service.Save(entity);
                    responce.Success = true;
                    responce.Message = "Designation Edited Successfully";
                }
                else
                {
                    entity = Mapper.Map<Designation>(dto);
                    _service.Save(entity);
                    responce.Success = true;
                    responce.Message = "Designation Saved Successfully";
                }
            }
            catch (Exception)
            {
                responce.Message = "Organization Save Failed";
            }
            _service.SaveChanges();
            return responce;
        }

        public List<RoleDto> GetAllRoles()
        {
            var data = _user.GetAllRoles();
            return data;
        }

        public ResponseDto SaveDesignationRoleMapping(DesignationRoleMappingDto dto)
        {
            var entity = new DesignationRoleMapping();
            ResponseDto responce = new ResponseDto();
            var ifExist =
                _service.GetAll<DesignationRoleMapping>()
                    .Where(
                        r =>
                            r.OfficeDesignationSettingId == dto.OfficeDesignationSettingId && r.RoleId == dto.RoleId &&
                            r.Status == EntityStatus.Active);
            try
            {
                if (!ifExist.Any())
                {
                    if (dto.Id > 0)
                    {
                        entity = _service.GetById<DesignationRoleMapping>((long) dto.Id);
                        dto.CreateDate = entity.CreateDate;
                        dto.CreatedBy = entity.CreatedBy;
                        entity = Mapper.Map(dto, entity);
                        entity.EditDate = DateTime.Now;
                        _service.Save(entity);
                        responce.Success = true;
                        responce.Message = "Designation Role Mapping Edited Successfully";
                    }
                    else
                    {
                        entity = Mapper.Map<DesignationRoleMapping>(dto);
                        entity.Status= EntityStatus.Active;
                        _service.Save(entity);
                        responce.Success = true;
                        responce.Message = "Designation Role Mapping Saved Successfully";
                    }
                }
                else
                {
                    responce.Message = "Designation Role Mapping Already Exist";
                    return responce;
                }
            }
            catch (Exception)
            {
                responce.Message = "Designation Role Mapping Save Failed";
                return responce;
            }
            _service.SaveChanges();
            return responce;
        }

        public List<DesignationRoleMappingDto> GetDesignationRoleMapping(long id)
        {
            var data =
                _service.GetAll<DesignationRoleMapping>()
                    .Where(r => r.OfficeDesignationSettingId == id && r.Status == EntityStatus.Active).ToList();
            var result = (from drm in data
                join rl in _user.GetAllRoles() on drm.RoleId equals rl.Id
                select new DesignationRoleMappingDto
                {
                    Id = drm.Id,
                    OfficeDesignationSettingName = drm.OfficeDesignationSetting.Name,
                    RoleId = drm.RoleId,
                    RoleName = rl.Name
                });
            return result.ToList();
        }
        
        public ResponseDto RemoveDesignationRoleMap(long id)
        {
            ResponseDto responce = new ResponseDto();
            var entity = _service.GetById<DesignationRoleMapping>(id);
            entity.Status = EntityStatus.Inactive;
            _service.Save(entity);
            responce.Success = true;
            responce.Message = "Designation Role Mapping Removed Successfully";
            return responce;
        }

        public ResponseDto SaveDesignationProductMapping(DesignationProductMappingDto dto)
        {
            var entity = new DesignationProductMapping();
            ResponseDto responce = new ResponseDto();
            var ifExist =
                _service.GetAll<DesignationProductMapping>()
                    .Where(
                        r =>
                            r.OfficeDesignationSettingId == dto.OfficeDesignationSettingId && r.ProductId == dto.ProductId &&
                            r.Status == EntityStatus.Active);
            try
            {
                if (!ifExist.Any())
                {
                    if (dto.Id > 0)
                    {
                        entity = _service.GetById<DesignationProductMapping>((long)dto.Id);
                        dto.CreateDate = entity.CreateDate;
                        dto.CreatedBy = entity.CreatedBy;
                        entity = Mapper.Map(dto, entity);
                        entity.EditDate = DateTime.Now;
                        _service.Save(entity);
                        responce.Success = true;
                        responce.Message = "Designation Product Mapping Edited Successfully";
                    }
                    else
                    {
                        entity = Mapper.Map<DesignationProductMapping>(dto);
                        entity.Status = EntityStatus.Active;
                        _service.Save(entity);
                        responce.Success = true;
                        responce.Message = "Designation Product Mapping Saved Successfully";
                    }
                }
                else
                {
                    responce.Message = "Designation Product Mapping Already Exist";
                    return responce;
                }
            }
            catch (Exception)
            {
                responce.Message = "Designation Product Mapping Save Failed";
                return responce;
            }
            _service.SaveChanges();
            return responce;
            //throw new NotImplementedException();
        }

        public List<DesignationProductMappingDto> GetDesignationProductMapping(long id)
        {
            var data =
               _service.GetAll<DesignationProductMapping>()
                   .Where(r => r.OfficeDesignationSettingId == id && r.Status == EntityStatus.Active).ToList();
            return Mapper.Map<List<DesignationProductMappingDto>>(data);
        }

        public ResponseDto RemoveDesignationProductMap(long id)
        {
            ResponseDto responce = new ResponseDto();
            var entity = _service.GetById<DesignationProductMapping>(id);
            entity.Status = EntityStatus.Inactive;
            _service.Save(entity);
            responce.Success = true;
            responce.Message = "Designation Product Mapping Removed Successfully";
            return responce;
        }
    }
}
