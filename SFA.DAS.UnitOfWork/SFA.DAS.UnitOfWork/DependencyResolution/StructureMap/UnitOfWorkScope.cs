using System;
using System.Threading.Tasks;
using SFA.DAS.UnitOfWork.Managers;
using StructureMap;

namespace SFA.DAS.UnitOfWork.DependencyResolution.StructureMap
{
    public class UnitOfWorkScope : IUnitOfWorkScope
    {
        private readonly IContainer _container;

        public UnitOfWorkScope(IContainer container)
        {
            _container = container;
        }

        public async Task RunAsync(Func<IContainer, Task> operation)
        {
            using (var nestedContainer = _container.GetNestedContainer())
            {
                var unitOfWorkManager = nestedContainer.GetInstance<IUnitOfWorkManager>();
                
                await unitOfWorkManager.BeginAsync().ConfigureAwait(false);
                
                try
                {
                    await operation(nestedContainer).ConfigureAwait(false);
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