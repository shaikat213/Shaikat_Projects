using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Finix.IPDC.DTO;
using Finix.IPDC.Facade;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Util;

namespace Finix.UI.Areas.IPDC.Controllers
{
    public class ProfessionController : BaseController
    {
        private readonly ProfessionFacade _profession = new ProfessionFacade();

        public JsonResult GetAllProfession()
        {
            var profession = _profession.GetAllProfession().ToList();
            return Json(profession, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOccupationType()
        {
            var typeList = Enum.GetValues(typeof(OccupationType))
               .Cast<OccupationType>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = UiUtil.GetDisplayName(t)
               });
            return Json(typeList, JsonRequestBehavior.AllowGet);
        }
    }
}