using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Finix.IPDC.DTO;
using Finix.IPDC.Service;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.Facade
{
    public class ProfessionFacade : BaseFacade
    {
        private readonly  GenService _service = new GenService();

        public List<ProfessionDto> GetAllProfession()
        {
            var data = _service.GetAll<Profession>().ToList();
            return Mapper.Map<List<ProfessionDto>>(data);
        }
    }
}
