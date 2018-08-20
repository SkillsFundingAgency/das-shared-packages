﻿#if NETCOREAPP2_0
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.Mvc.UnitTests
{
    [TestFixture]
    public class UnitOfWorkMiddlewareTests : FluentTest<UnitOfWorkMiddlewareTestsFixture>
    {
        [Test]
        public void WhenMiddlewareInvoked_ThenUnitOfWorkBegan_AndNextMiddlwareInvoked_AndUnitOfWorkEnded()
        {
            Run(f => f.InvokeMiddleware(), a =>
            {
                a.UnitOfWorkManager.Verify(m => m.Begin(), Times.Once());
                a.UnitOfWorkManager.Verify(m => m.End(null), Times.Once());
            });
        }
        
        [Test]
        public void WhenMiddlewareInvoked_AndNextMiddlwareRaisedException_ThenExceptionIsRaised()
        {
            Run(f => f.SetException(), f => f.InvokeMiddleware(), (f, a) => a.ShouldThrow<Exception>());
        }

        [Test]
        public void WhenMiddlewareInvoked_AndNextMiddlwareRaisedException_ThenUnitOfWorkBegan_AndUnitOfWorkEndedWithException()
        {
            Run(f => f.SetException(), f => f.InvokeMiddlewareAndSwallowException(), f =>
            {

                f.UnitOfWorkManager.Verify(m => m.Begin(), Times.Once());
                f.UnitOfWorkManager.Verify(m => m.End(f.Exception), Times.Once());
            });
        }
    }

    public class UnitOfWorkMiddlewareTestsFixture : FluentTestFixture
    {

        public Mock<RequestDelegate> RequestDelegateMock { get; set; }
        public Mock<IUnitOfWorkManager> UnitOfWorkManager { get; set; }
        public NserviceBusUnitOfWorkMiddleware UnitOfWorkMiddleware { get; set; }
        public Exception Exception { get; set; }
        public HttpContext Context { get; set; }
        public string ContextBody { get; }

        public UnitOfWorkMiddlewareTestsFixture()
        {
            Context = new DefaultHttpContext();
            ContextBody = "Next Middleware Invoked";
            UnitOfWorkManager = new Mock<IUnitOfWorkManager>();
            UnitOfWorkMiddleware = new NserviceBusUnitOfWorkMiddleware(Next(false));
            Exception = new Exception();
        }

        private RequestDelegate Next(bool setException)
        {
            return async (context) =>
            {
                if (setException)
                {
                    throw Exception;
                }

                await context.Response.WriteAsync(ContextBody);

                Context = context;

            };
        }

        public async void InvokeMiddleware()
        {
            await UnitOfWorkMiddleware.InvokeAsync(Context, UnitOfWorkManager.Object);
        }

        public async void InvokeMiddlewareAndSwallowException()
        {
            try
            {
                await UnitOfWorkMiddleware.InvokeAsync(Context, UnitOfWorkManager.Object);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public void SetException()
        {
            UnitOfWorkMiddleware = new NserviceBusUnitOfWorkMiddleware(Next(true));
        }

        public string ResponseContextBody()
        {
            string bodyContent = new StreamReader(Context.Request.Body).ReadToEnd();

            return bodyContent;
        }

        public interface IDelegateMock
        {
            Task RequestDelegate(HttpContext context);
        }
    }
}
#endif