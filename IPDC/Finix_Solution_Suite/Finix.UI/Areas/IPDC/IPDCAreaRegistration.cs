using System.Web.Mvc;

namespace Finix.UI.Areas.IPDC
{
    public class IPDCAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "IPDC";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "IPDC_default",
                "IPDC/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}