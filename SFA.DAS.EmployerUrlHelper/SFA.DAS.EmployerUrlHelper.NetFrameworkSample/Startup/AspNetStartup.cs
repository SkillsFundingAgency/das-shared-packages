using Microsoft.Owin;
using Owin;
using SFA.DAS.EmployerUrlHelper.NetFrameworkSample.Startup;

[assembly: OwinStartup(typeof(AspNetStartup))]
namespace SFA.DAS.EmployerUrlHelper.NetFrameworkSample.Startup
{
    public class AspNetStartup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}