using Finix.Auth.DTO;
using Finix.Auth.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Finix.UI
{
    public static class SessionHelper
    {

        public static UserResourceDto UserProfile
        {
            get { return GetFromSession<UserResourceDto>("UserProfile"); }
            set { SetInSession("UserProfile", value); }
        }

        public static bool HasPermission(string areaName, string controllerName, string actionName)
        {
            //return true;
            //skip login 
            if (new List<string> { "login", "home", "calculation", "dashboardapi" }.Contains(controllerName.ToLower()))
                return true;
            
            try
            {
                return UserProfile.Roles.Any(x => x.Name.ToLower() == "admin") ||
                       UserProfile.Tasks.Any(x => x.Name == string.Format("{0}_{1}_{2}", areaName, controllerName, actionName));
            }
            catch (Exception ex)
            {
                return false;
            }
            //return false;
        }

        private static object GetFromSession(string key)
        {
            return HttpContext.Current.Session[key];
        }
        private static T GetFromSession<T>(string key)
        {
            return (T)HttpContext.Current.Session[key];
        }
        private static void SetInSession(string key, object value)
        {
            HttpContext.Current.Session[key] = value;
        }

        public static void RegisterTasks()
        {
            var f = new LoginFacade();
            if (f.IsTaskRegistered())
                return;

            var asm = Assembly.GetExecutingAssembly();
            var controlleractionlist = asm.GetTypes()
                .Where(type => typeof(System.Web.Mvc.Controller).IsAssignableFrom(type))
                .SelectMany(
                    type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                .Where(
                    m =>
                        !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute),
                            true).Any())
                //.Select(x => new { Controller = x.DeclaringType.Name, Action = x.Name, ReturnType = x.ReturnType.Name, Attributes = String.Join(",", x.GetCustomAttributes().Select(a => a.GetType().Name.Replace("Attribute", ""))) })
                .Select(x => new { Controller = x.DeclaringType.Name.ToLower().Replace("controller", ""), Action = x.Name.ToLower() })
                .OrderBy(x => x.Controller).ThenBy(x => x.Action).ToList();
            f.RegisterTasks(controlleractionlist.Select(x => string.Format("{0}_{1}", x.Controller, x.Action)).ToList());

        }

    }
}