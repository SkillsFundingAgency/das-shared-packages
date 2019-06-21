# SFA.DAS.Provider.Shared.UI

This package provides shared Provider UI components via a [Razor Class Library](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/ui-class?view=aspnetcore-2.2&tabs=visual-studio).

This consists of _Layout and _Menu razor views.

## Requirements, Limitations and Restrictions

* Only Core web apps are supported.
* In order for the menu to render, the user must be authenticated and the Provider Id (UKPRN) must be in the claims
* This package references the ProviderUrlHelper package, which makes use of StructureMap and AutoConfiguration. The current implementation will add StructureMap references to your project and make use of AutoConfiguration. This dependency will be removed in a future version.
* Feature Toggling is not supported. It is necessary to build and release new versions of the package and consume them in order to obtain different navigation menu versions.

## Usage

* Add a reference to the [SFA.DAS.Provider.Shared.UI](https://www.nuget.org/packages/SFA.DAS.Provider.Shared.UI/) package
* Remove any local shared views called \_Layout.cshtml or \_Menu.cshtml, otherwise these will take precedence over those in the package
* Your application must set the selected navigation section by:
   * Calling the `SetDefaultNavigationSection` in the application startup, passing your default navigation section as a parameter (if this is adequate, nothing further is required)
   * Decorating controllers and/or individual action methods with the `SetNavigationSection` attribute filter, passing the appropriate navigation section as a parameter
* Prevent the menu from being displayed by decorating controllers or individual action methods with the `HideNavigationBar` attribute filter
* Show a "Beta" phase banner in the layout for your service by calling the `ShowBetaPhaseBanner` startup extension method (or adding the `ShowBetaPhaseBanner` attribute filter as appropriate)
 
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
