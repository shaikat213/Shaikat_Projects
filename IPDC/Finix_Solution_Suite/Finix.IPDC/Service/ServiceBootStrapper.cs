using System;
using Finix.IPDC.Infrastructure;
using Microsoft.Practices.Unity;

namespace Finix.IPDC.Service
{
    public static class ServicesBootStrapper
    {
        public static Func<LifetimeManager> DefaultLifetime = () => new HierarchicalLifetimeManager();
        public static void InitializeAll(IUnityContainer container)
        {
            Initialize(container);
        }

        public static void Initialize(IUnityContainer container)
        {
            container.RegisterType<IUnitOfWork, FinixIPDCUnitOfWork>(DefaultLifetime());
        }

    }
}
