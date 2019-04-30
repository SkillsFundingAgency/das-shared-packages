using System;
using System.Threading.Tasks;

namespace SFA.DAS.UnitOfWork
{
    public interface IUnitOfWorkScope
    {
        Task RunAsync(Func<IServiceProvider, Task> operation);
    }
}