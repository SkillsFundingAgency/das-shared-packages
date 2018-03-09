using System.Collections.Generic;
using System.Collections.Specialized;
using NUnit.Framework;

namespace SFA.DAS.Web.Policy.UnitTests
{
    public class WhenTestingWebPolicyProviderResponseHeaderRestrictionPolicy : WhenTestingWebPolicyProvider
    {
        [SetUp]
        public override void Setup()
        {
            Unit = new HttpContextPolicyProvider(new List<IHttpContextPolicy>()
            {
                new ResponseHeaderRestrictionPolicy()
            });

            _requestHeaders = new NameValueCollection();
            _responseHeaders = new NameValueCollection();

            HttpContextManager.SetCurrentContext(GetMockedHttpContext());

        }
        [Test]
        public void ItShouldApplyItsPolicy()
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