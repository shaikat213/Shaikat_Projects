using Finix.IPDC.Facade;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Finix.IPDC.Facade;
using Finix.IPDC.Util;
using Finix.IPDC.DTO;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.UI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ControllerBuilder.Current.DefaultNamespaces.Add("Finix.Accounts.Controllers");
            //automapping
            Finix.UI.App_Start.AutoMapperBootstrapper.BootStrapAutoMaps();
            //Finix.Auth.Facade.AutoMaps.AutoMapperBootstrapper.BootStrapAutoMaps();
            //Finix.Accounts.Facade.AutoMaps.AutoMapperBootstrapper.BootStrapAutoMaps();

            //set seed_data path
            Finix.Auth.Util.AuthSystem.SeedDataPath = string.Format(@"{0}\..\..\Finix.Auth\{1}\{2}",
                AppDomain.CurrentDomain.GetData("DataDirectory"),
                "App_Data",
                "seed_data");
            //Finix.Accounts.Util.AccountsSystem.SeedDataPath = string.Format(@"{0}\..\..\Finix.Accounts\{1}\{2}",
            //    AppDomain.CurrentDomain.GetData("DataDirectory"),
            //    "App_Data",
            //    "seed_data");
            Finix.IPDC.Util.IPDCSystem.SeedDataPath = string.Format(@"{0}\..\..\Finix.IPDC\{1}\{2}",
               AppDomain.CurrentDomain.GetData("DataDirectory"),
               "App_Data",
               "seed_data");
            Task.Run(() => BGWorker.DoWork(null, null));
        }
    }
    public static class BGWorker
    {
        public static void DoWork(object sender, DoWorkEventArgs e)
        {
            int delay = 5;
            try { delay = Convert.ToInt16(ConfigReader.GetAppSetting("service_interval_in_seconds")); }
            catch { }
           // Dummywork();
            while (true)
            {
                N.ProcessNotifications();
                System.Threading.Thread.Sleep(delay * 1000);
            }
        }
        private static void Dummywork()
        {
            var notifications = new List<NotificationDto>();
            notifications.Add(
                new NotificationDto
                {
                    NotificationType = NotificationType.LeadAssigned,
                    Message = "NotificationType_LeadAssigned",
                    NotificationStatusType = NotificationStatusType.New,
                    RefId=67,
                    MenuName="Sub Menu",
                    MenuId=23,
                    Url = "/Auth/Submodule/Index"
                }); notifications.Add(
                 new NotificationDto
                 {
                     NotificationType = NotificationType.LeadAssigned,
                     Message = "NotificationType_LeadAssigned",
                     NotificationStatusType = NotificationStatusType.New,
                     RefId=98,
                     MenuName = "Menu",
                     MenuId = 23,
                     Url = "/Auth/Menu/Index"
                 });
            notifications.Add(
                new NotificationDto
                {
                    NotificationType = NotificationType.ApplicationWaitingForApprovalByTL,
                    Message = "NotificationType_ApplicationWaitingForApprovalByTL",
                    NotificationStatusType = NotificationStatusType.New,
                    RefId=90,
                    MenuName = "Role Menu",
                    MenuId = 23,
                    Url = "/Auth/Role/Index"
                });
            new NotificationFacade().SaveNotifications(notifications);
        }
    }
}
