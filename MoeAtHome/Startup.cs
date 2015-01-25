using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MoeAtHome.Startup))]
namespace MoeAtHome
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
