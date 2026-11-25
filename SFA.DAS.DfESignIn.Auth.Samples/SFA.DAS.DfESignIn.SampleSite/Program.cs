using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.DfESignIn.Auth.Controllers;
using SFA.DAS.DfESignIn.SampleSite.AppStart;
using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Startup;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

configuration.AddAzureTableStorage(options =>
{
    options.ConfigurationKeys = configuration["ConfigNames"]!
        .Split(
            ',',
            StringSplitOptions.RemoveEmptyEntries |
            StringSplitOptions.TrimEntries);

    options.StorageConnectionString =
        configuration["ConfigurationStorageConnectionString"]!;

    options.EnvironmentName = configuration["Environment"]!;
    options.PreFixConfigurationKeys = false;
});

configuration.AddEnvironmentVariables();

builder.Services.AddServiceRegistration(configuration);
builder.Services
    .AddMvc(options =>
    {
        options.Filters.Add(
            new AutoValidateAntiforgeryTokenAttribute());
    })
    .AddApplicationPart(typeof(DfESignInStubController).Assembly)
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