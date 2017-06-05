using Finix.Auth.DTO;
using Finix.Auth.Infrastructure;
using Finix.Auth.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Finix.Auth.Facade
{
    public class ApplicationFacade : BaseFacade
    {
        private readonly GenService _service = new GenService();

        public ResponseDto SaveApplication(ApplicationDto application, long? userId)
        {
            var response = new ResponseDto();
            Application entity;
            if (application.Id != null && application.Id > 0)
            {
                entity = _service.GetById<Application>((long)application.Id);

                entity.Name = application.Name;
                //Mapper.Map(application, entity);
                entity.EditDate = DateTime.Now;
                entity.EditedBy = userId;
            }
            else
            {
                entity = Mapper.Map<Application>(application);
                entity.Status = EntityStatus.Active;
                entity.CreateDate = DateTime.Now;
                entity.CreatedBy = userId;
            }
            try
            {
                _service.Save(entity);
                response.Message = "Application added successfully.";
                response.Success = true;
            }
            catch (Exception) { }
            return response;
        }

        public List<ApplicationDto> GetAllActiveApplication()
        {
            return Mapper.Map<List<ApplicationDto>>(_service.GetAll<Application>().Where(a => a.Status == EntityStatus.Active));
        }

        public ResponseDto DeleteApplication(long ApplicationId, long UserId)
        {
            var response = new ResponseDto();
            try
            {
                var entity = _service.GetById<Application>(ApplicationId);
                entity.Status = EntityStatus.Deleted;
                entity.EditedBy = UserId;
                entity.EditDate = DateTime.Now;

                _service.Save(entity);
                response.Success = true;
            }
            catch (Exception) { }
            return response;
        }
    }
}
