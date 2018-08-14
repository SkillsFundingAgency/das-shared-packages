#if NETSTANDARD2_0
using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using NServiceBus;

namespace SFA.DAS.NServiceBus.Mvc
{
    public class NserviceBusUnitOfWorkMiddleware
    {
        private readonly RequestDelegate _next;

        public NserviceBusUnitOfWorkMiddleware(RequestDelegate next)
        {
            _next = next;

        }

        public async Task InvokeAsync(HttpContext context, IUnitOfWorkManager unitOfWorkManager)
        {


            unitOfWorkManager.Begin();

            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                unitOfWorkManager.End(e);

                throw;
            }

            unitOfWorkManager.End();
        }
    }
    public static class NserviceBusOutboxMiddlewareExtensions
    {
        public static IApplicationBuilder UseNserviceBusUnitOfWork(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<NserviceBusUnitOfWorkMiddleware>();
        }
    }
}
