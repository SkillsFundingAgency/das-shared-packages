using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.Web.Policy.UnitTests
{
    public class WhenTestingWebPolicyProviderResponseHeaderXOptionsPolicy : WhenTestingWebPolicyProvider
    {
        [SetUp]
        public override void Setup()
        {
            Unit = new HttpContextPolicyProvider(new List<IHttpContextPolicy>()
            {
                new ResponseHeaderXOptionsPolicy()
            });

            _requestHeaders = new NameValueCollection();
            _responseHeaders = new NameValueCollection();

            HttpContextManager.SetCurrentContext(GetMockedHttpContext());

        }
        [Test]
        public void ItShouldApplyItsPolicy()
        {
            var context = HttpContextManager.Current;

            _responseHeaders.Clear();

            Unit.Apply(context, PolicyConcern.HttpResponse);


            Assert.IsNotNull(_responseHeaders["X-Frame-Options"]);
            Assert.IsNotNull(_responseHeaders["X-Content-Type-Options"]);
            Assert.IsNotNull(_responseHeaders["X-XSS-Protection"]);

        }
    }
}