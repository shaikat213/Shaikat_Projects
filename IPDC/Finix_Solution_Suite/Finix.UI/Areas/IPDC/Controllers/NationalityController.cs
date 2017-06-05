using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Finix.IPDC.Facade;

namespace Finix.UI.Areas.IPDC.Controllers
{
    public class NationalityController : BaseController
    {
        private readonly NationalityFacade  _nationalityFacade = new NationalityFacade();

        public JsonResult GetAllNationality()
        {
            var result = _nationalityFacade.GetAllNationality();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}