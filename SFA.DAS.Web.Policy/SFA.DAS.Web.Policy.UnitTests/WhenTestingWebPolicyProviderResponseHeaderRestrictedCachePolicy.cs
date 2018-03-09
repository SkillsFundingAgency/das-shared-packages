using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.Web.Policy.UnitTests
{
    public class WhenTestingWebPolicyProviderResponseHeaderRestrictedCachePolicy : WhenTestingWebPolicyProvider
    {

        [SetUp]
        public override void Setup()
        {
            Unit = new HttpContextPolicyProvider(new List<IHttpContextPolicy>()
            {
                new ResponseHeaderRestrictedCachePolicy()
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

            Assert.IsNotNull(_responseHeaders["Pragma"]);
            Assert.IsNotNull(_responseHeaders["Expires"]);

            Cache.Verify(x => x.SetCacheability(It.IsAny<HttpCacheability>()), Times.Once);
            Cache.Verify(x => x.AppendCacheExtension(It.IsAny<string>()), Times.Once);
        }

    }
}