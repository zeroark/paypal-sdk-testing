using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Zeroark.Paypal.Website.Startup))]
namespace Zeroark.Paypal.Website
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
