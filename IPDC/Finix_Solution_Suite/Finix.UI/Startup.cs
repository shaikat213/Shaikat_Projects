using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Finix.UI.Startup))]
namespace Finix.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
