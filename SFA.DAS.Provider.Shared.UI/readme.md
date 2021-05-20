# SFA.DAS.Provider.Shared.UI

This package provides shared Provider UI components via a [Razor Class Library](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/ui-class?view=aspnetcore-3.1&tabs=visual-studio).

This consists of _Layout and _Menu razor views.

## Requirements, Limitations and Restrictions

* Only Core web apps are supported.
* In order for the menu to render, the user must be authenticated and the Provider Id (UKPRN) must be in the claims

## Usage

* Add a reference to the [SFA.DAS.Provider.Shared.UI](https://www.nuget.org/packages/SFA.DAS.Provider.Shared.UI/) package
* In your configuration add a section called **ProviderSharedUIConfiguration** with DashboardUrl defined.
```
   { 
      "ProviderSharedUIConfiguration" :
      {
         "DashboardUrl":"https://localhost:1234"
      }   
   }
```
* In startup.cs add the following, where _configuration is an instance of IConfiguration with the above config loaded:
```
services.AddProviderUiServiceRegistration(_configuration);
```
* Remove any local shared views called \_Layout.cshtml or \_Menu.cshtml, otherwise these will take precedence over those in the package
* Your application must set the selected navigation section by:
   * Calling the `SetDefaultNavigationSection` in the application startup, passing your default navigation section as a parameter (if this is adequate, nothing further is required)
   * Decorating controllers and/or individual action methods with the `SetNavigationSection` attribute filter, passing the appropriate navigation section as a parameter
* Prevent the menu from being displayed by decorating controllers or individual action methods with the `HideNavigationBar` attribute filter
* Show a "Beta" phase banner in the layout for your service by calling the `ShowBetaPhaseBanner` startup extension method (or adding the `ShowBetaPhaseBanner` attribute filter as appropriate)
* Suppress navigation sections (for example, if an up-coming service is toggled off) by using the `SuppressNavigationSection` extension method
* The package also requires that you implement your own sign out action. This is expecting a controller action with the route name of `provider-signout` 
Examples: 

 ```csharp
 services
    .AddMvc(...)
    .SetDefaultNavigationSection(NavigationSection.YourCohorts)
  ```
  
  ```csharp
  [HttpGet]
  [Route("add-apprentice")]
  [SetNavigationSection(NavigationSection.YourCohorts)]
  public async Task<IActionResult> AddDraftApprenticeship(CreateCohortWithDraftApprenticeshipRequest request)
  {...
  ```
 
 ```csharp
[HideNavigationBar]
public class ErrorController : Controller
 ```


 ```csharp
builder.SuppressNavigationSection(NavigationSection.Reservations);
 ```
