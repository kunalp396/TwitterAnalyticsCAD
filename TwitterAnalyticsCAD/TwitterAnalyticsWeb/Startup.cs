using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TwitterAnalyticsWeb.Startup))]
namespace TwitterAnalyticsWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
