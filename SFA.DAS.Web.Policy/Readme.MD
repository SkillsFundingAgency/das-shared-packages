# SFA.DAS.Web.Policy

![badge](https://sfa-gov-uk.visualstudio.com/DefaultCollection/_apis/public/build/definitions/c39e0c0b-7aff-4606-b160-3566f3bbce23/405/badge)

[![NuGet Badge](https://buildstats.info/nuget/SFA.DAS.Http)](https://www.nuget.org/packages/SFA.DAS.Http/)

Library containing Web policy provider and related policy implementation classes.

The provider will apply http request and or response header modifications to implement functionality or security features.

The current implementation only provides Response header policy implementations but the design is capable of modifying incoming headers if invoked at the appropriate point in the request pipeline.

##Policies provided

The following policies are recommended by SFA Security team and should be applied at the last possible moment in an Http request pipeline before the response is returned to the caller.

### ResponseHeaderRestrictionPolicy (Recommended)

Applies a policy to remove certain header values; The current list is in the code but includes the following;

X-Powered-By
X-AspNet-Version
X-AspNetMvc-Version
Server


### ResponseHeaderRestrictedCachePolicy (Recommended)

Applies a policy to restrict caching.

Sets Cacheability to HttpCacheability.NoCache
Appends Cache Extensions "no-store, must-revalidate"
Adds header Pragma, no-cache
Adds header Expires, 0

###  ResponseHeaderXOptionsPolicy (Recommended)

Applies a policy to provide recomended 'X' Options to th headers

X-Frame-Options, deny
X-Content-Type-Options,nosniff
X-XSS-Protection,1
        

## Example Usage

Instead of writing and maintaining something like inside every web application, and then neding to update all code when new recommendations are provided by the security team;

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
	new HttpContextPolicyProvider(new List<IHttpContextPolicy>()
            {
                new ResponseHeaderRestrictionPolicy(),
                new ResponseHeaderXOptionsPolicy(),
                new ResponseHeaderRestrictedCachePolicy()
            }).Apply(
			new System.Web.HttpContextWrapper(HttpContext.Current), PolicyConcerns.HttpResponse);
}
```
Or

```csharp

protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
{
	new HttpContextPolicyProvider().Apply(new System.Web.HttpContextWrapper(HttpContext.Current), PolicyConcerns.HttpResponse);
}
```
Will apply all known Http Response policies. The two examples above are equivalent at this release.




