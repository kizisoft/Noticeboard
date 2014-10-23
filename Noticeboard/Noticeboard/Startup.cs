using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Noticeboard.Startup))]
namespace Noticeboard
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
