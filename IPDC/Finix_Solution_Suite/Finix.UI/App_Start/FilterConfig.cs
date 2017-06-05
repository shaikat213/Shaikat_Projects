using Finix.Auth.Infrastructure;
using System;
using System.Security.Authentication;
using System.Web;
using System.Web.Mvc;

namespace Finix.UI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new HrmPrivilegeFilter());

            // apply to specific actions instead
            //filters.Add(new JqGridActionFilter());
        }
    }
    /// <summary>
    /// HRM Privilege Filter
    /// </summary>
    public class HrmPrivilegeFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var action = filterContext.ActionDescriptor.ActionName;
            string area = "";
            if(filterContext.RouteData.DataTokens["area"] != null)
                area = filterContext.RouteData.DataTokens["area"].ToString();
            if (!SessionHelper.HasPermission(area.ToLower(), controller.ToLower().Replace("controller", ""), action.ToLower()))
                throw new AuthenticationException(string.Format("You have no access to: {0}/{1}/{2}", area, controller, action));
            base.OnActionExecuting(filterContext);
        }
    }
    /// <summary>
    /// JqGrid Request Data Filter
    /// </summary>
    public class JqGridActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var jqModel = new JqGridModel();
            //set data

            var search = filterContext.Controller.ValueProvider.GetValue("_search");
            jqModel._search = search != null && search.AttemptedValue.ToLower() == "true";

            var rows = filterContext.Controller.ValueProvider.GetValue("rows");
            jqModel.rows = rows == null ? 0 : Convert.ToInt32(rows.AttemptedValue);

            var pages = filterContext.Controller.ValueProvider.GetValue("pages");
            jqModel.pages = pages == null ? 0 : Convert.ToInt32(pages.AttemptedValue);

            var sidx = filterContext.Controller.ValueProvider.GetValue("sidx");
            jqModel.sidx = sidx == null ? "" : sidx.AttemptedValue;

            var sord = filterContext.Controller.ValueProvider.GetValue("sord");
            jqModel.sord = sord == null ? "" : sord.AttemptedValue;

            var filters = filterContext.Controller.ValueProvider.GetValue("filters");
            jqModel.filters = filters == null ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<JqFilter>(filters.AttemptedValue);

            filterContext.HttpContext.Items["_jqgfilterdata_"] = jqModel;

            base.OnActionExecuting(filterContext);
        }
    }
}
