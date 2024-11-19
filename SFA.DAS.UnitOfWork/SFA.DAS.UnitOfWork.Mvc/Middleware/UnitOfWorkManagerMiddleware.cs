//#if net_core_8
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SFA.DAS.UnitOfWork.Managers;

namespace SFA.DAS.UnitOfWork.Mvc.Middleware
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
                await _next(context).ConfigureAwait(false);
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
//#endif