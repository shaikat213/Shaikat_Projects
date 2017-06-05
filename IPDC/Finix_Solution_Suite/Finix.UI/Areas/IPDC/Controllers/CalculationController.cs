using Finix.IPDC.Infrastructure;
using Finix.IPDC.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Finix.IPDC.Facade;
using Finix.IPDC.DTO;

namespace Finix.UI.Areas.IPDC.Controllers
{
    public class CalculationController : BaseController
    {
        //private readonly EmployeeFacade _employee = new EmployeeFacade();
        // GET: IPDC/Employee
        public ActionResult AutoLoan()
        {
            return View();
        }

        public ActionResult HomeLoan()
        {
            return View();
        }

        public ActionResult PersonalLoan()
        {
            return View();
        }
        public ActionResult EMICalculator()
        {
            return View();
        }
        public ActionResult PersonalLoan_PDF()
        {
            return View();
        }
        public ActionResult AutoLoan_PDF()
        {
            return View();
        }
        public ActionResult HomeLoan_PDF()
        {
            return View();
        }
        public ActionResult Deposit_Schemes()
        {
            return View();
        }
        public ActionResult Rate_Chart()
        {
            return View();
        }
    }
}