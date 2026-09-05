using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PDR.Startup))]
namespace PDR
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
