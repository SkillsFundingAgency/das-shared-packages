using NUnit.Framework;

namespace SFA.DAS.Web.Policy.UnitTests
{
    public class WhenTestingWebPolicyProviderResponsePolicy : WhenTestingWebPolicyProvider
    {

        [Test]
        public void ItShouldModifyTheResponse()
        {
            var context = HttpContextManager.Current;

            _requestHeaders.Add("X-Powered-By", "");
            _requestHeaders.Add("X-AspNet-Version", "");
            _requestHeaders.Add("X-AspNetMvc-Version", "");
            _requestHeaders.Add("Server", "");
            
            Unit.Apply(context, PolicyConcern.HttpResponse);

            Assert.IsNull(context.Response.Headers["X-Powered-By"]);
            Assert.IsNull(context.Response.Headers["X-AspNet-Version"]);
            Assert.IsNull(context.Response.Headers["X-AspNetMvc-Version"]);
            Assert.IsNull(context.Response.Headers["Server"]);

        }
    }
}