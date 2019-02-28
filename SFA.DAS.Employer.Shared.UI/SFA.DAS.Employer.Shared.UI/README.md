

SFA.DAS.Employer.Shared.UI
==========

This project provides shared Employer site UI components via a [Razor Class Library](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/ui-class?view=aspnetcore-2.2&tabs=visual-studio). It currently consists of the following:

- Header
- Footer
- Menu

## Usage

Add the package to the web project and add the following into the *ConfigureServices* method:

```csharp
private IConfiguration _configuration;

public void ConfigureServices(IServiceCollection services)
{
    services.AddMaMenuConfiguration(_configuration, "Logout_Route", "idamsClientId");
}
```
Where in the above example *Logout_Route* is the Route Name of the action used to sign the user out of the site and *idamsClientId* is the Employer Idams Relying Party ClientId.


The components can be added into the web apps' Razor views by adding the partial views that the RCL contains e.g.

```html
<partial name="_Header">
<partial name="_Menu" model="@accountId">
<partial name="_Footer">
```

**Note**: the *Menu* parital view requires an *accountId* (the hashed account id used in the url) to enable it to generate the menu links.

### Configuration
The RCL library uses `IConfiguration` as it's source for items such as the urls of the other employer sites and paths within those site that links are generated for. 

The idea is that there is a shared configuration location that all the Employer apps can share (Azure Table Storage) which they'd all consume. However this is not essential as long as the web app includes the matching configuration values expected.

Here is the configuration structure that is expected by the component:
```json
"MaPageConfiguration": {
    "Ma": {
      "RootUrl": "https://das-test-accui-as.azurewebsites.net",
      "Routes": {
        "AccountsHome": "/accounts/{0}/teams",
        "Help": "/service/help",
        "Privacy": "/service/privacy",
        "Accounts": "/service/accounts",
        "RenameAccount": "/accounts/{0}/rename",
        "Notifications": "/settings/notifications",
        "AccountsFinance": "/accounts/{0}/finance",
        "AccountsTeamsView": "/accounts/{0}/teams/view",
        "AccountsAgreements": "/accounts/{0}/agreements",
        "AccountsSchemes": "/accounts/{0}/schemes"
      }
    },
    "Commitments": {
      "RootUrl": "https://test-empc.apprenticeships.sfa.bis.gov.uk",
      "Routes": {
        "ApprenticesHome": "/commitments/accounts/{0}/apprentices/home"
      }
    },
    "Recruit": {
      "RootUrl": "https://recruit.test-eas.apprenticeships.sfa.bis.gov.uk",
      "Routes": {
        "RecruitHome": "/accounts/{0}"
      }
    },
    "Identity": {
      "RootUrl": "https://test-login.apprenticeships.sfa.bis.gov.uk",
      "Routes": {
        "ChangePassword": "/account/changepassword?clientId={0}&returnUrl={1}",
        "ChangeEmailAddress": "/account/changeemail?clientId={0}&returnUrl={1}"
      }
    }
  }
```
### Menu Behaviour
There are scenarios where the menu needs to display differently. There are 2 additional *modes* that are supported. These modes are currenly controlled by *ViewBag* boolean values. 

| ViewBag property | Description                                                |
| ---------------- |:----------------------------------------------------------:|
| IsErrorPage      | Hides *Rename Account* & *Notfications* items              |
| ShowNav          | Hides the top level menu (Doesn't include *settings*)      |


### Override Partial Views
Any of the parital views that are defined in the RCL can be overriden in the consuming application simply by defining a partial view with the same name ane location e.g. */Views/Shared/_Header.cshtl*
