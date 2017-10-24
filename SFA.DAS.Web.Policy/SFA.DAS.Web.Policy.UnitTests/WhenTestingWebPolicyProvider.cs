using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using System.Web.Routing;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.Web.Policy.UnitTests
{
    [TestFixture]
    public abstract class WhenTestingWebPolicyProvider
    {
        protected HttpContextPolicyProvider Unit;
        protected NameValueCollection _requestHeaders;
        protected NameValueCollection _responseHeaders;

        [SetUp]
        public void Setup()
        {
            Unit = new HttpContextPolicyProvider(new List<IHttpContextPolicy>()
            {
                new ResponseHeaderRestrictionPolicy()
            });

            _requestHeaders = new NameValueCollection();


            HttpContextManager.SetCurrentContext(GetMockedHttpContext());

        }
        private HttpContextBase GetMockedHttpContext()
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var user = new Mock<IPrincipal>();
            var identity = new Mock<IIdentity>();
          
            var requestContext = new Mock<RequestContext>();
            requestContext.Setup(x => x.HttpContext).Returns(context.Object);
            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);
            context.Setup(ctx => ctx.User).Returns(user.Object);
            
            user.Setup(ctx => ctx.Identity).Returns(identity.Object);
            identity.Setup(id => id.IsAuthenticated).Returns(true);
            identity.Setup(id => id.Name).Returns("test");
            request.Setup(req => req.Url).Returns(new Uri("http://www.google.com"));
            request.Setup(req => req.RequestContext).Returns(requestContext.Object);
            
            requestContext.Setup(x => x.RouteData).Returns(new RouteData(){});

            request.SetupGet(req => req.Headers).Returns(_requestHeaders);
            response.SetupGet(req => req.Headers).Returns(_requestHeaders);

            return context.Object;
        }

    }
}