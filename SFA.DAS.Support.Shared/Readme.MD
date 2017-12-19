# SFA.DAS.Web.Policy

![badge](https://sfa-gov-uk.visualstudio.com/DefaultCollection/_apis/public/build/definitions/c39e0c0b-7aff-4606-b160-3566f3bbce23/405/badge)

[![NuGet Badge](https://buildstats.info/nuget/SFA.DAS.Http)](https://www.nuget.org/packages/SFA.DAS.Http/)

Library containing Web policy provider and related policy implementation classes.


### Example Usage

Instead of writing and maintaining something like inside every web application;

```csharp
protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
{
    var headersToRemove = new string[] {
        "X-Powered-By",
        "X-AspNet-Version",
        "X-AspNetMvc-Version",
        "Server"
    };
    foreach (var s in headersToRemove)
    {
        HttpContext.Current.Response.Headers.Remove(s);
    }
}

```

The ResponseHeaderRestrictionPolicy class performs this action.

Configuring and Injecting the HttpContextPolicyProvider class provides a common library of policies to apply to the HttpContext at any stage of the pipeline.

E.g.

```csharp

protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
{
	_httpContextPolicyProvider.Apply(
			new System.Web.HttpContextWrapper(HttpContext.Current), PolicyConcerns.HttpResponse);
}
```