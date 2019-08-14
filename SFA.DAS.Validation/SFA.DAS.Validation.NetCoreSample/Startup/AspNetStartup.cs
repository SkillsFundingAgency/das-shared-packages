using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Validation.Mvc;
using SFA.DAS.Validation.Mvc.Extensions;

namespace SFA.DAS.Validation.NetCoreSample.Startup
{
    public class AspNetStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(o => o.AddValidation());
        }
        
        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage()
                .UseMvc();
        }
    }
}