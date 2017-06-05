using AutoMapper;
using Finix.IPDC.DTO;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;
using Finix.IPDC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Finix.IPDC.Facade
{
    public class OrganizationFacade
    {
        private readonly GenService _service = new GenService();
        public List<OrganizationDto> GetOrganizations()
        {
            var orgs = _service.GetAll<Organization>();
            return Mapper.Map<List<OrganizationDto>>(orgs.ToList());
        }

        public ResponseDto SaveOrganizations(OrganizationDto dto)
        {
            var entity = new Organization();
            ResponseDto responce = new ResponseDto();
            try
            {


                if (dto.Id > 0)
                {
                    entity = _service.GetById<Organization>(dto.Id);
                    dto.CreateDate = entity.CreateDate;
                    dto.CreatedBy = entity.CreatedBy;
                    entity = Mapper.Map(dto, entity);
                    entity.EditDate = DateTime.Now;
                    _service.Save(entity);
                    responce.Success = true;
                    responce.Message = "Organization Edited Successfully";
                }
                else
                {
                    entity = Mapper.Map<Organization>(dto);
                    entity.Status = EntityStatus.Active;
                    _service.Save(entity);
                    responce.Success = true;
                    responce.Message = "Organization Saved Successfully";
                }
            }
            catch (Exception)
            {
                responce.Message = "Organization Save Failed";
            }
            _service.SaveChanges();
            return responce;
        }
    }
}
