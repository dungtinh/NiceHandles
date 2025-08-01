using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NiceHandles.Startup))]
namespace NiceHandles
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
