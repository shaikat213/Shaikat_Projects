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
    public class OfficeDesignationAreaFacade : BaseFacade
    {
        private readonly GenService _service = new GenService();

        public List<OfficeDesignationAreaDto> GetByOfficeDesignationMap(long upazilaOrThana, long dist, long settingId)
        {
            List<long> existingthanaIds = new List<long>();
            //var data = _service.GetAll<OfficeDesignationSetting>();
            //var result = (from ods in data
            //    join oda in _service.GetAll<OfficeDesignationArea>() on ods.Id equals oda.OfficeDesignationSettingId
            //    select new OfficeDesignationAreaDto()
            //    {
            //        Id = oda.Id,
            //        Name = oda.Name,
            //        OfficeDesignationSettingId = oda.OfficeDesignationSettingId,
            //        UpozilaOrThana = oda.UpozilaOrThana,
            //        RefId = oda.RefId
            //    }).Where(r => r.UpozilaOrThana == (UpozilaOrThana)id);
            var data = _service.GetAll<OfficeDesignationArea>().Where(o=>o.OfficeDesignationSettingId == settingId && o.UpozilaOrThana == (UpozilaOrThana)upazilaOrThana && o.Status == EntityStatus.Active);
            if (data.Any())
            {

                existingthanaIds = data.Select(d => (long)d.RefId).ToList();
            }
            if ((UpozilaOrThana)upazilaOrThana == UpozilaOrThana.Thana)
            {
                var thana = (from th in _service.GetAll<Thana>().Where(r => r.DistrictId == dist && !existingthanaIds.Contains(r.Id))
                                 //join res in result on th.Id equals res.RefId
                             select new OfficeDesignationAreaDto
                             {
                                 Id = th.Id,
                                 Name = th.ThanaNameEng,
                                 //OfficeDesignationSettingId = res.OfficeDesignationSettingId,
                                 //UpozilaOrThana = res.UpozilaOrThana,
                                 //RefId = res.RefId
                             });
                return thana.ToList();
            }
            else
            {
                var thana = (from th in _service.GetAll<Upazila>().Where(r => r.DistrictId == dist && !existingthanaIds.Contains(r.Id))
                                 //join res in result on th.Id equals res.RefId
                             select new OfficeDesignationAreaDto
                             {
                                 Id = th.Id,
                                 Name = th.UpazilaNameEng,
                                 //OfficeDesignationSettingId = res.OfficeDesignationSettingId,
                                 //UpozilaOrThana = res.UpozilaOrThana,
                                 //RefId = res.RefId
                             });
                return thana.ToList();
            }

        }
        public List<OfficeDesignationAreaDto> GetOfficeDesgArea(long id)
        {
            List<OfficeDesignationAreaDto> area = new List<OfficeDesignationAreaDto>();
            OfficeDesignationAreaDto aOfficeDesignationAreaDto = new OfficeDesignationAreaDto();
            var oda = _service.GetAll<OfficeDesignationArea>().Where(r => r.OfficeDesignationSettingId == id && r.Status == EntityStatus.Active).ToList();
            foreach (var officeDesignationArea in oda)
            {
                if (officeDesignationArea.UpozilaOrThana==UpozilaOrThana.Upozilla)
                {
                    aOfficeDesignationAreaDto = new OfficeDesignationAreaDto();
                    aOfficeDesignationAreaDto.Id = officeDesignationArea.Id;
                    aOfficeDesignationAreaDto.Name =
                        _service.GetAll<Upazila>()
                            .Where(r => r.Id == officeDesignationArea.RefId)
                            .FirstOrDefault()
                            .UpazilaNameEng;
                    area.Add(aOfficeDesignationAreaDto);
                }
                else
                {
                    aOfficeDesignationAreaDto = new OfficeDesignationAreaDto();
                    aOfficeDesignationAreaDto.Id = officeDesignationArea.Id;
                    aOfficeDesignationAreaDto.Name =
                       _service.GetAll<Thana>()
                           .Where(r => r.Id == officeDesignationArea.RefId)
                           .FirstOrDefault()
                           .ThanaNameEng;
                    area.Add(aOfficeDesignationAreaDto);
                }
            }
            return area;
        }

        public ResponseDto SaveOfficeDesignationArea(OfficeDesignationAreaDto officeDesignationArea)
        {
            var entity = new OfficeDesignationArea();
            ResponseDto responce = new ResponseDto();
            if (officeDesignationArea.OfficeDesignationSettingId ==null)
            {
                responce.Message = "Select Designation Settings First";
                return responce;
            }
            try
            {
                    //salesLeadDto.LeadStatus = LeadStatus.Submitted;
                    entity = Mapper.Map<OfficeDesignationArea>(officeDesignationArea);
                    entity.Status = EntityStatus.Active;
                    //entity.LeadStatus = LeadStatus.Submitted; //todo- Lead Status Extra button To Save and Save And Draft
                    _service.Save(entity);
                    responce.Success = true;
                    responce.Message = "Office Designation Area Saved Successfully";
                
            }
            catch (Exception ex)
            {
                responce.Message = "Office Designation Area Save Failed";
            }
            _service.SaveChanges();
            return responce;
        }

        public ResponseDto UpdateOfficeDesignationArea(long  id)
        {
            ResponseDto responce = new ResponseDto();
            var entity = _service.GetById<OfficeDesignationArea>(id);
            entity.Status= EntityStatus.Inactive;
            _service.Save(entity);
            responce.Success = true;
            responce.Message = "Office Designation Area Removed Successfully";
            return responce;
        }

        public List<DivisionDto> GetDivisionByCountry(long id)
        {
            var divisions = _service.GetAll<Division>().Where(r => r.CountryId == id).ToList();
            return Mapper.Map<List<DivisionDto>>(divisions);
        }
    }
}
