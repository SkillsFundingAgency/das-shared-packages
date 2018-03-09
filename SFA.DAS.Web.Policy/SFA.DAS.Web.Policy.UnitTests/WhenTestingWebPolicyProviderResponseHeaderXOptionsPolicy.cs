using NUnit.Framework;

namespace SFA.DAS.Web.Policy.UnitTests
{
    public class WhenTestingWebPolicyProviderResponseHeaderXOptionsPolicy : WhenTestingWebPolicyProvider
    {
        [Test]
        public void ItShouldApplyItsPolicy()
        {
            var context = HttpContextManager.Current;

            _responseHeaders.Clear();

            Unit.Apply(context, PolicyConcern.HttpResponse);

            Assert.IsNotNull(_responseHeaders["Pragma"]);
            Assert.IsNotNull(_responseHeaders["Expires"]);

        }
    }
}