using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Testing;
using NUnit.Framework;
using Owin;

namespace SFA.DAS.OidcMiddleware.UnitTests
{
    public abstract class Test
    {
        [OneTimeSetUp]
        public async Task SetUp()
        {
            Given();
            await When();
        }

        protected abstract void Given();
        protected abstract Task When();

        protected TestServer CreateServer(Action<IAppBuilder> configure, Func<IOwinContext, bool> handler = null)
        {
            return TestServer.Create(app =>
            {
                app.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    AuthenticationType = "Cookies",
                    ExpireTimeSpan = new TimeSpan(0, 10, 0),
                    SlidingExpiration = true
                });

                app.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    AuthenticationType = "TempState",
                    AuthenticationMode = AuthenticationMode.Passive
                });

                configure?.Invoke(app);

                app.Use(async (context, next) =>
                {
                    if (handler == null || !handler(context))
                    {
                        await next();
                    }
                });
            });
        }
    }
}