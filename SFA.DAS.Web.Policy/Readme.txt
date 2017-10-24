This solution provides a common Web Policy provider for applying best practice to http request/Response.

Example:

Instead of writing and maintaining;

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

Inside every web application


The ResponseHeaderRestrictionPolicy performs this action.

protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
{
	_webPolicyProvider.Apply(
			new System.Web.HttpContextWrapper(HttpContext.Current), PolicyConcerns.HttpResponse);
}

now the headers and all other web policies are managed by one code change and a refresh of the shared packages!
