using Microsoft.AspNetCore.Mvc;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.DfESignIn.SampleSite.AppStart;
using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Startup;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration.BuildConfiguration();

builder.Services.AddProviderUiServiceRegistration(builder.Configuration);

builder.Services.AddServiceRegistration(configuration);
builder.Services
    .AddMvc(options =>
    {
        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    })
    .SetDefaultNavigationSection(NavigationSection.YourCohorts);

var app = builder.Build();

app
    .UseRouting()
    .UseAuthentication()
    .UseAuthorization()
    .UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    });

app.Run();