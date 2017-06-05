using Finix.Auth.Facade;
using Finix.UI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Finix.UI.Areas.Auth.Controllers
{
    public class AssignmentsController : BaseController
    {
        private readonly AssigmentFacade _assignment = new AssigmentFacade();
        //public ActionResult Index()
        //{
        //    ViewBag.Message = "Basic Data Operation";
        //    return View();
        //}

        public ActionResult ManyToManyAssignment()
        {
            return View();
        }

        public JsonResult GetManyToManyDdlData()
        {
            var metaData = _assignment.GetMany2ManyDdlData();

            var leftDdlData = metaData.Keys.Select(k => new { key = k, value = k }).ToList();
            var rightDdlAllData = new[] { new { leftTable = "", key = "", value = "" } }.ToList();
            foreach (var lKey in metaData.Keys)
            {
                foreach (var rKey in metaData[lKey])
                {
                    rightDdlAllData.Add(new { leftTable = lKey, key = rKey, value = rKey });
                }
            }
            rightDdlAllData.RemoveAt(0);

            return Json(new { leftDdlData, rightDdlAllData }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetManyToManyTableData(string leftTable, string rightTable)
        {
            var data = _assignment.GetMany2ManyTableDataByReflection(leftTable, rightTable);
            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetM2MRightTableSelectedData(string leftTable, string rightTable, int leftTableRecordId)
        {
            var data = _assignment.GetMany2ManyRightTableAssignedIds(leftTable, rightTable, leftTableRecordId);
            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateM2MData(string leftTable, string rightTable, int leftTableRecordId,
            long[] rightTableRecordIds)
        {
            var data = _assignment.SetMany2ManyRightTableAssignedIds(leftTable, rightTable, leftTableRecordId,
                rightTableRecordIds.ToList());
            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }
	}
}