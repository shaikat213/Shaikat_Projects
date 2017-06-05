using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Finix.Auth.Util;
using Finix.IPDC.Service;
using Finix.IPDC.DTO;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.Facade
{
    public class NationalityFacade : BaseFacade
    {
        private readonly GenService _service = new GenService();
        public List<NationalityDto> GetAllNationality()
        {
            var data = _service.GetAll<Nationality>().ToList();
            return Mapper.Map<List<NationalityDto>>(data);
        }
        //public string GetNationalityList()
        //{
        //    List<KeyValuePair<int, string>> nationality = UiUtil.EnumToKeyVal<Nationality>();
        //    string strret = "<select><option></option>";
        //    foreach (var item in nationality)
        //    {
        //        strret += "<option value='" + item.Key + "'>" + item.Value + "</option>";
        //    }
        //    strret += "</select>";
        //    return strret;
        //}

    }
}
