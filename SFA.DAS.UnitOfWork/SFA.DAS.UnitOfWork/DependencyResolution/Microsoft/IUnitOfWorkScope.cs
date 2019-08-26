using System;
using System.Threading.Tasks;

namespace SFA.DAS.UnitOfWork.DependencyResolution.Microsoft
{
    public interface IUnitOfWorkScope
    {
        Task RunAsync(Func<IServiceProvider, Task> operation);
    }
}