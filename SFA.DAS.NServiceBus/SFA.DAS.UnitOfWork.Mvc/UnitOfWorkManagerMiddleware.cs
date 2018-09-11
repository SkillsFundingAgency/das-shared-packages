#if NETCOREAPP2_0
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.UnitOfWork.Mvc
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
            await unitOfWorkManager.BeginAsync().ConfigureAwait(false);

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await unitOfWorkManager.EndAsync(ex).ConfigureAwait(false);

                throw;
            }

            await unitOfWorkManager.EndAsync().ConfigureAwait(false);
        }
    }
}
#endif