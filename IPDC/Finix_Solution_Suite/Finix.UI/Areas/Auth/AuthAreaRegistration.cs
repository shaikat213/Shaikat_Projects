using System.Web.Mvc;

namespace Finix.UI.Areas.Auth
{
    public class AuthAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Auth";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Auth_default",
                "Auth/{controller}/{action}/{id}",
                new { controller = "Login", action = "Login", id = UrlParameter.Optional },
                namespaces: new[] { "Finix.UI.Areas.Auth.Controllers" }
            );
        }
    }
}