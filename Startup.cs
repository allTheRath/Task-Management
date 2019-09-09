using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TaskAssignment.Startup))]
namespace TaskAssignment
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
