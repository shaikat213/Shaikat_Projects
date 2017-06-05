using Finix.IPDC.Facade;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Finix.IPDC.Util;
using Finix.IPDC.DTO;
using Finix.IPDC.Infrastructure;

namespace Finix.UI.Areas.IPDC.Controllers
{
    public class SalesLeadController : BaseController
    {
        private readonly SalesLeadFacade _salesLeadFacade = new SalesLeadFacade();
        private readonly AddressFacade _addressFacade = new AddressFacade();
        private readonly ProductFacade _productFacade = new ProductFacade();
        private readonly OrganizationFacade _organizationFacade = new OrganizationFacade();
        private readonly EnumFacade _enumFacade = new EnumFacade();
        //private readonly SalesLeadFacade _salesLeadFacade = new SalesLeadFacade();

        #region Sales Lead Entry
        public ActionResult SalesLeadEntry()
        {
            return View();
        }
        public JsonResult GetSalesLeads()
        {
            var salesLead = _salesLeadFacade.GetSalesLeads();
            return Json(salesLead, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSubmittedSalesLeads()
        {
            var userId = SessionHelper.UserProfile.UserId;
            var salesLead = _salesLeadFacade.GetSubmittedSalesLeads(userId);
            return Json(salesLead, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public string GetLeadTypes()
        {
            List<KeyValuePair<int, string>> leadTypes = UiUtil.EnumToKeyVal<LeadType>();
            string strret = "<select><option></option>";
            foreach (KeyValuePair<int, string> item in leadTypes)
            {
                strret += "<option value='" + item.Key + "'>" + item.Value + "</option>";
            }
            strret += "</select>";
            return strret;
        }

        [HttpGet]
        public string GetLeadStatusList()
        {
            List<KeyValuePair<int, string>> leadstatus = UiUtil.EnumToKeyVal<LeadStatusEntry>();
            string strret = "<select><option></option>";
            foreach (KeyValuePair<int, string> item in leadstatus)
            {
                strret += "<option value='" + item.Key + "'>" + item.Value + "</option>";
            }
            strret += "</select>";
            return strret;
        }

        //[HttpGet] 
        //public string GetLoanTypes()
        //{
        //    List<KeyValuePair<int, string>> loanTypes = UiUtil.EnumToKeyVal<LoanType>();
        //    string strret = "<select>";
        //    foreach (KeyValuePair<int, string> item in loanTypes)
        //    {
        //        strret += "<option value='" + item.Key + "'>" + item.Value + "</option>";
        //    }
        //    strret += "</select>";
        //    return strret;
        //}
        public string GetProductList()
        {
            List<ProductDto> thanas = _productFacade.GetAllProducts();
            string strret = "<select><option></option>";
            foreach (var item in thanas)
            {
                strret += "<option value='" + item.Id + "'>" + item.Name + "</option>";
            }
            strret += "</select>";
            return strret;
        }
        public string GetThanaList()
        {
            List<ThanaDto> thanas = _addressFacade.GetAllThanas();
            string strret = "<select><option></option>";
            foreach (var item in thanas)
            {
                strret += "<option value='" + item.Id + "'>" + item.ThanaNameEng + "</option>";
            }
            strret += "</select>";
            return strret;
        }

        public string GetThanasByDistrictInGrid(long districtId)
        {
            List<ThanaDto> thanas = _addressFacade.GetAllThanas().Where(r=>r.DistrictId == districtId).ToList();
            string strret = "<select><option></option>";
            foreach (var item in thanas)
            {
                strret += "<option value='" + item.Id + "'>" + item.ThanaNameEng + "</option>";
            }
            strret += "</select>";
            return strret;
        }
        public string GetDistrictList()
        {
            List<DistrictDto> districts = _addressFacade.GetAllDistricts();
            string strret = "<select><option></option>";
            foreach (var item in districts)
            {
                strret += "<option value='" + item.Id + "'>" + item.Name + "</option>";
            }
            strret += "</select>";
            return strret;
        }
        public JsonResult SaveSalesLead(SalesLeadDto salesLeadDto)
        {
            try
            {
                if (salesLeadDto.FollowupTimeText != null)
                {
                    DateTime nextFollowUp = DateTime.Now;
                    var FromConverted = DateTime.TryParseExact(salesLeadDto.FollowupTimeText, "dd/MM/yyyy HH:mm",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out nextFollowUp);
                    if (FromConverted)
                    {
                        salesLeadDto.FollowupTime = nextFollowUp;
                    }
                }
                var userId = SessionHelper.UserProfile.UserId;
                var result = _salesLeadFacade.SaveSalesLead(salesLeadDto, userId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region Sales Lead FollowUp

        public ActionResult SalesLeadFollowUp()
        {
            return View();
        }

        public JsonResult GetProspectiveSalesLeads()
        {
            var userId = SessionHelper.UserProfile.UserId;
            var salesLead = _salesLeadFacade.GetProspectiveSalesLeads(userId);
            return Json(salesLead, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNonProspectiveSalesLeads()
        {
            var userId = SessionHelper.UserProfile.UserId;
            var salesLead = _salesLeadFacade.GetNonProspectiveSalesLeads(userId);
            return Json(salesLead, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveFollowUpTime(FollowupTrackDto followupTrack)
        {
            try
            {
                DateTime nextFollowUp = DateTime.Now;
                DateTime callTime = DateTime.Now;
                var FromConverted = DateTime.TryParseExact(followupTrack.NextFollowUpTxt, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out nextFollowUp);
                var callTimeConverted = DateTime.TryParseExact(followupTrack.CallTimeTxt, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out callTime);
                if (FromConverted)
                {
                    followupTrack.NextFollowUp = nextFollowUp;
                }
                if (callTimeConverted)
                {
                    followupTrack.CallTime = callTime;
                }
                followupTrack.SubmittedBy = SessionHelper.UserProfile.UserId;
                var result = _salesLeadFacade.SaveFollowUpTime(followupTrack);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult FollowUpScheduleSR()
        {
            return View();
        }

        public JsonResult GetAssignedSalesLeads()
        {
            var userId = SessionHelper.UserProfile.UserId;
            var salesLead = _salesLeadFacade.GetAssignedSalesLeads(userId);
            return Json(salesLead, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProspectiveAssignedSalesLeads()
        {
            var userId = SessionHelper.UserProfile.UserId;
            var salesLead = _salesLeadFacade.GetProspectiveAssignedSalesLeads(userId);
            return Json(salesLead, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UnsuccessfulLeads(string sortOrder, string currentFilter, string searchString, int page = 1)
        {
            ViewBag.CurrentSort = sortOrder;
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;
            var temp = _salesLeadFacade.GetUnsuccessfulAssignedSalesLeads(SessionHelper.UserProfile.UserId, 10, page, searchString);
            return View(temp);
        }
        #endregion
       
        #region SalesLeadEdit
        public JsonResult GetSalesLeadForEdit(long leadId)
        {
            var data = _salesLeadFacade.GetSalesLeadForEdit(leadId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllDistricts()
        {
            List<DistrictDto> districts = _addressFacade.GetAllDistricts();
            return Json(districts, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetThanasByDistrict(long districtId)
        {
            List<ThanaDto> thanas = _addressFacade.GetThanasByDistrict(districtId);
            return Json(thanas, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllProducts()
        {
            List<ProductDto> products = _productFacade.GetAllProducts();
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllOrganaizations()
        {
            List<OrganizationDto> organizations = _organizationFacade.GetOrganizations();
            return Json(organizations, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllLeadTypes()
        {
            var leadTypeList = new List<LeadTypeDto>(); ;
            LeadTypeDto leadType;
            List<KeyValuePair<int, string>> leadTypes = UiUtil.EnumToKeyVal<LeadType>();
            //string strret = "<select>";
            foreach (KeyValuePair<int, string> item in leadTypes)
            {
                leadType = new LeadTypeDto();
                leadType.Id = item.Key;
                leadType.Name = item.Value;
                leadTypeList.Add(leadType);
            }
            //strret += "</select>";
            return Json(leadTypeList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllLeadStatus()
        {
            var leadStatusList = new List<LeadStatusDto>(); ;
            LeadStatusDto leadStatus;
            List<KeyValuePair<int, string>> leadStatuses = UiUtil.EnumToKeyVal<LeadStatus>();
            foreach (KeyValuePair<int, string> item in leadStatuses)
            {
                leadStatus = new LeadStatusDto();
                leadStatus.Id = item.Key;
                leadStatus.Name = item.Value;
                leadStatusList.Add(leadStatus);
            }
            return Json(leadStatusList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllLeadStatusRM()
        {
            var leadStatusList = new List<LeadStatusDto>(); ;
            LeadStatusDto leadStatus;
            List<KeyValuePair<int, string>> leadStatuses = UiUtil.EnumToKeyVal<LeadStatus>();
            leadStatuses = leadStatuses.Where(x => x.Key != 0 && x.Key != 1 && x.Key != 3 && x.Key != 5 && x.Key != 6).ToList();
            foreach (KeyValuePair<int, string> item in leadStatuses)
            {
                leadStatus = new LeadStatusDto();
                leadStatus.Id = item.Key;
                leadStatus.Name = item.Value;
                leadStatusList.Add(leadStatus);
            }
            //leadStatusList = leadStatusList.Where(x => x.Id == 2 && x.Id == 4).ToList();
            return Json(leadStatusList, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetAllLeadStatusRM()
        //{
        //    var riskLevels = _enumFacade.GetAllLeadStatusRM();
        //    return Json(riskLevels, JsonRequestBehavior.AllowGet);
        //}

        #endregion

        #region Sales Lead Assignment 
        public ActionResult LeadAssignment()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetDataForLeadAssignment()//todo- this have to filter by tl ares taken from session helper
        {
            var userId = SessionHelper.UserProfile.UserId;
            var data = _salesLeadFacade.GetDataForLeadAssignment(userId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //GetAllSR SaveSalesLeadAssignment GeSalesLeadAssignment
        [HttpGet]
        public JsonResult GetAllSR()
        {
            var userId = SessionHelper.UserProfile.UserId;
            var data = _salesLeadFacade.GetAllSR(userId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveSalesLeadAssignment(SalesLeadAssignmentDto salesLeadAssignmentDto)
        {
            try
            {
                DateTime followUp = DateTime.Now;
                var FromConverted = DateTime.TryParseExact(salesLeadAssignmentDto.FollowUpTimeTxt, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out followUp);

                if (FromConverted)
                {
                    salesLeadAssignmentDto.FollowUpTime = followUp;
                    //return Json(null, JsonRequestBehavior.AllowGet);
                }
                var userId = SessionHelper.UserProfile.UserId;
                var result = _salesLeadFacade.SaveSalesLeadAssignment(salesLeadAssignmentDto,userId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpGet]
        public JsonResult GeSalesLeadAssignment(long id , long assaignto)//todo- this have to filter by tl ares taken from session helper
        {
            var userId = SessionHelper.UserProfile.UserId;
            var data = _salesLeadFacade.GeSalesLeadAssignment(id, assaignto, userId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public JsonResult GetDateWiseTimeSchedule(long id, string dateString)//todo- this have to filter by tl ares taken from session helper
        {
            DateTime followUp = DateTime.Now;


            var FromConverted = DateTime.TryParseExact(dateString, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out followUp);

            if (!FromConverted)
            {
                //followupTrack.NextFollowUp = nextFollowUp;
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var userId = SessionHelper.UserProfile.UserId;
            var data = _salesLeadFacade.GetDateWiseTimeSchedule(id, followUp);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion


        public ActionResult OccupationIncomeDetails()
        {
            return View();
        }

        [HttpGet]
        public string GetCustomerPriority()
        {
            var typeList = Enum.GetValues(typeof(CustomerPriority))
               .Cast<CustomerPriority>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = UiUtil.GetDisplayName(t)
               });
            string strret = "<select><option></option>";
            foreach (var item in typeList)
            {
                strret += "<option value='" + item.Id + "'>" + item.Name + "</option>";
            }
            strret += "</select>";
            return strret;
        }

        [HttpGet]
        public string GetFollowupTypes()
        {
            var typeList = Enum.GetValues(typeof(FollowupType))
               .Cast<FollowupType>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = UiUtil.GetDisplayName(t)
               });
            string strret = "<select><option></option>";
            foreach (var item in typeList)
            {
                strret += "<option value='" + item.Id + "'>" + item.Name + "</option>";
            }
            strret += "</select>";
            return strret;
        }

        [HttpGet]
        public JsonResult GetCustomerPriorityList()
        {
            var typeList = Enum.GetValues(typeof(CustomerPriority))
               .Cast<CustomerPriority>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = UiUtil.GetDisplayName(t)
               });
            
            return Json(typeList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetFollowupTypesList()
        {
            var typeList = Enum.GetValues(typeof(FollowupType))
               .Cast<FollowupType>()
               .Select(t => new EnumDto
               {
                   Id = ((int)t),
                   Name = UiUtil.GetDisplayName(t)
               });
            
            return Json(typeList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SalesLeadFollowupRm()
        {
            return View();
        }

        #region Followup track
        [HttpGet]
        public JsonResult GetCallLogBySLNo (long SlNo)
        {
            var result = _salesLeadFacade.GetCallLogBySLNo(SlNo);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //GetCallLogBySLNo
        #endregion

        public JsonResult GetCustomerSensitivities()
        {
            var result = _enumFacade.GetCustomerSensitivities();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}