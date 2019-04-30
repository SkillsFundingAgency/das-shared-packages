using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.UnitOfWork
{
    public class UnitOfWorkScope : IUnitOfWorkScope
    {
        private readonly IServiceProvider _serviceProvider;

        public UnitOfWorkScope(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task RunAsync(Func<IServiceProvider, Task> operation)
        {
            using (var serviceScope = _serviceProvider.CreateScope())
            {
                var unitOfWorkManager = serviceScope.ServiceProvider.GetService<IUnitOfWorkManager>();

                await unitOfWorkManager.BeginAsync().ConfigureAwait(false);

                try
                {
                    await operation(serviceScope.ServiceProvider).ConfigureAwait(false);
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
}