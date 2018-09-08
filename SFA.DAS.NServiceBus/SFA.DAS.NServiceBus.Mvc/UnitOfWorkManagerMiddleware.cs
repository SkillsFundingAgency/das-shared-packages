#if NETCOREAPP2_0
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SFA.DAS.NServiceBus.ClientOutbox;

namespace SFA.DAS.NServiceBus.Mvc
{
    public class UnitOfWorkManagerMiddleware
    {
        private readonly RequestDelegate _next;

        public UnitOfWorkManagerMiddleware(RequestDelegate next)
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
            catch (Exception ex)
            {
                unitOfWorkManager.End(ex);
                throw;
            }

            unitOfWorkManager.End();
        }
    }
}
#endif