using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Finix.IPDC.DTO;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;
using Finix.IPDC.Service;

namespace Finix.IPDC.Facade
{
    public class AddressFacade
    {
        private readonly GenService _service = new GenService();
       
        public List<ThanaDto> GetAllThanas()
        {
            var thanas = _service.GetAll<Thana>().ToList();
            return Mapper.Map<List<ThanaDto>>(thanas);
        }
      
        public List<DistrictDto> GetAllDistricts()
        {
            var districts = _service.GetAll<District>().ToList();
            return Mapper.Map<List<DistrictDto>>(districts);
        }

        public List<ThanaDto> GetThanasByDistrict(long districtId)
        {
            var thanas = _service.GetAll<Thana>().Where(r=>r.DistrictId == districtId).OrderBy(t=>t.ThanaNameEng).ToList();
            return Mapper.Map<List<ThanaDto>>(thanas);
        }

        public List<DivisionDto> GetAllDivisions()
        {
            var division = _service.GetAll<Division>().ToList();
            return Mapper.Map<List<DivisionDto>>(division);
        }

        public List<DistrictDto> GetDistrictByDivision(long id)
        {
            var district = _service.GetAll<District>().Where(r=>r.DivisionId==id).OrderBy(t=>t.DistrictNameEng).ToList();
            return Mapper.Map<List<DistrictDto>>(district);
        }

        public List<UpzilaDto> GetUpzilaByDistrict(long id)
        {
            var district = _service.GetAll<Upazila>().Where(r=>r.DistrictId==id).OrderBy(t=>t.UpazilaNameEng).ToList();
            return Mapper.Map<List<UpzilaDto>>(district);
        }
        public List<CountryDto> GetAllCountries()
        {
            var country = _service.GetAll<Country>().Where(c => c.Status == EntityStatus.Active).OrderBy(t=>t.Name).ToList();
            return Mapper.Map<List<CountryDto>>(country.ToList());
        }
        //public List<Are> GetOfficeDesgArea(long id)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
