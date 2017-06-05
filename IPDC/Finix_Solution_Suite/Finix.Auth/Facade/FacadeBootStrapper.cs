using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.Auth.Infrastructure;
using Finix.Auth.Service;
using Microsoft.Practices.Unity;

namespace Finix.Auth.Facade
{
    public static class FacadeBootStrapper
    {

        public static void RegisterFacadesAndServices(IUnityContainer container)
        {
            RegisterServices(container);
            RegisterFacades(container);
        }
        public static void RegisterFacades(IUnityContainer container)
        {
            //container.RegisterType<IProductFacade, ProductFacade>(new HierarchicalLifetimeManager());
        }
        public static void RegisterServices(IUnityContainer container)
        {
            container.RegisterType<IUnitOfWork, FinixAuthUnitOfWork>(new HierarchicalLifetimeManager());
            container.RegisterType(typeof(IFinixAuthService<>), typeof(FinixHrmService<>));
        }
    }
}
