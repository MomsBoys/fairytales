using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FairyTales.Startup))]
namespace FairyTales
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
