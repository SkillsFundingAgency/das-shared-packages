using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.EmployerUrlHelper.Sample.Startup
{
    public class AspNetStartup
    {
        private readonly IConfiguration _configuration;

        public AspNetStartup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEmployerUrlHelper(_configuration)
                .AddMvc();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage()
                .UseStaticFiles()
                .UseMvc();
        }
    }
}