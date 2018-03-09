using System.Web;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.Web.Policy.UnitTests
{
    public class WhenTestingWebPolicyProviderResponseHeaderRestrictedCachePolicy : WhenTestingWebPolicyProvider
    {

        [Test]
        public void ItShouldApplyItsPolicy()
        {
            var context = HttpContextManager.Current;

            _responseHeaders.Clear();

            Unit.Apply(context, PolicyConcern.HttpResponse);


            Assert.IsNotNull(_responseHeaders["X-Frame-Options"]);
            Assert.IsNotNull(_responseHeaders["X-Content-Type-Options"]);
            Assert.IsNotNull(_responseHeaders["X-XSS-Protection"]);

            Cache.Verify(x => x.SetCacheability(It.IsAny<HttpCacheability>()), Times.Once);
            Cache.Verify(x => x.AppendCacheExtension(It.IsAny<string>()), Times.Once);
        }
    }
}