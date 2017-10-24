using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Web;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.Web.Policy.UnitTests
{
    public class WhenTestingWebPolicyProviderWithMultiplePolicies : WhenTestingWebPolicyProvider
    {
        private new List<Mock<IHttpContextPolicy>> _policies;

        public override void Setup()
        {
            base.Setup();

            _policies = Enumerable.Range(0, 10)
                .Select(x => new Mock<IHttpContextPolicy>()).ToList();

            Unit = new HttpContextPolicyProvider(_policies.Select(o=> o.Object));
        }

        [Test]
        public void ItShouldCallApplyOnAllQualifyingPolicies()
        {
            var context = HttpContextManager.Current;
            
            Unit.Apply(context, PolicyConcern.HttpResponse);

            foreach (var policy in _policies)
            {
                policy.Verify( v=>v.Apply(It.IsAny<HttpContextBase>()), Times.Once);
            }
        }
        
    }
}