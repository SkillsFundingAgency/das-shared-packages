using System;
using System.Threading.Tasks;
using StructureMap;

namespace SFA.DAS.UnitOfWork
{
    public interface IUnitOfWorkScope
    {
        Task RunAsync(Func<IContainer, Task> operation);
    }
}