using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Finix.UI.App_Start
{
    public class AutoMapperBootstrapper
    {
        public static void BootStrapAutoMaps()
        {
            DefaultMappings();
        }

        private static void DefaultMappings()
        {
            //GenService _service = new GenService();

            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile(new Finix.Auth.Facade.AutoMaps.AuthMappingProfile());
                //cfg.AddProfile(new Finix.Accounts.Facade.AutoMaps.AccountsMappingProfile());
                cfg.AddProfile(new Finix.IPDC.Facade.AutoMaps.IPDCMappingProfile());
            });

        }
    }
}