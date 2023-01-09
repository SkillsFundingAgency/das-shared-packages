using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Validation.Mvc.Extensions;

namespace SFA.DAS.Validation.NetCoreSample
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services
                .AddControllersWithViews(options => options.AddValidation())
                .AddRazorRuntimeCompilation();
            
            var app = builder.Build();
            
            app.UseRouting();
            
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
                
            app.Run();
        }
    }
}
