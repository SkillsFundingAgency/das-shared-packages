using DfE.Example.Web.Configuration;
using DfE.Example.Web.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Employer.Shared.UI;

namespace DfE.Example.Web
{
    public partial class Startup
    {
        public IConfiguration Configuration { get; }
        private IHostingEnvironment _hostingEnvironment;
        private readonly OidcConfiguration _authConfig;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _hostingEnvironment = env;
            _authConfig = Configuration.GetSection("Oidc").Get<OidcConfiguration>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc(options =>
            {
                //options.Filters.Add(new AuthorizeFilter("EmployerAccountPolicy"));
                options.Filters.Add(new AuthorizeFilter());

            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddMaMenuConfiguration(Configuration, RouteNames.Logout_Get, _authConfig.ClientId);

            services.AddAuthenticationService(_authConfig, _hostingEnvironment);
            //services.AddAuthorizationService();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
