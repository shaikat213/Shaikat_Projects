using AutoMapper;

namespace Finix.IPDC.API.App_Start
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
                cfg.AddProfile(new Auth.Facade.AutoMaps.AuthMappingProfile());
                //cfg.AddProfile(new Finix.Accounts.Facade.AutoMaps.AccountsMappingProfile());
                cfg.AddProfile(new Facade.AutoMaps.IPDCMappingProfile());
            });

        }
    }
}