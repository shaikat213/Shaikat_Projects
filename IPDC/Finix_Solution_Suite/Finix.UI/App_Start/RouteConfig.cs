using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Finix.UI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Login",
            //    url: "Auth/{controller}/{action}/{id}",
            //    defaults: new { controller = "Login", action = "Login", id = UrlParameter.Optional }
            //);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Finix.UI.Controllers"}
            );

         //   routes.MapRoute(
         //    name: "AccountsRoute",
         //    url: "Accounts/{controller}/{action}/{id}",
         //    defaults: new { controller = "Homex", action = "Index", id = UrlParameter.Optional },
         //    namespaces: new[] { "Finix.Accounts.Controller" }
         //);
   
             



        }
    }
}
