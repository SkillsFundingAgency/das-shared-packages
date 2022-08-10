using Microsoft.AspNetCore.Mvc;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.SampleSite.AppStart;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<GovUkOidcConfiguration>(builder.Configuration.GetSection(nameof(GovUkOidcConfiguration)));

builder.Services.AddServiceRegistration(builder.Configuration);
builder.Services
    .AddMvc(options =>
    {
        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    });

var app = builder.Build();

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();