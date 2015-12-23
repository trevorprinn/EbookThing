using FluentScheduler;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EbookSite.Startup))]
namespace EbookSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            TaskManager.Initialize(new TaskRegistry());
        }
    }
}
